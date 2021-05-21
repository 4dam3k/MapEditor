using Exiled.API.Features;
using Hints;
using Interactables.Interobjects.DoorUtils;
using MapEditor.Models;
using MapEditor.Objects;
using MapGeneration;
using MEC;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static MapEditor.Editor;


namespace MapEditor
{
    public class MainClass : Plugin<MapEditorConfig>
    {
        public override string Author { get; } = "Killers0992";
        public override string Name { get; } = "MapEditor";
        public override string Prefix { get; } = "mapeditor";
        public override System.Version RequiredExiledVersion { get; } = new System.Version(2, 1, 29);
        public override System.Version Version { get; } = new System.Version(3,0,0);

        public static string pluginDir;

        public static GameObject workstationObj;
        public static GameObject doorLCZobj;
        public static GameObject doorHCZobj;
        public static GameObject doorENTobj;

        public static Schematic schem;

        public override void OnEnabled()
        {
            schem = new Schematic();
            Log.Info("MapEditor loaded.");
            workstationObj = NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name.ToUpper().Contains("WORK"));


            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.RestartingRound += EventHandlers.RoundRestartEvent;
            Exiled.Events.Handlers.Player.Destroying += EventHandlers.PlayerLeaveEvent;
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += EventHandlers.RemoteAdminCommandEvent;
            string appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            pluginDir = Path.Combine(appData, "EXILED", "Plugins", "MapEditor");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!Directory.Exists(Path.Combine(pluginDir, "maps")))
                Directory.CreateDirectory(Path.Combine(pluginDir, "maps"));
            if (!Directory.Exists(Path.Combine(pluginDir, "schematics")))
                Directory.CreateDirectory(Path.Combine(pluginDir, "schematics"));
            Exiled.Events.Handlers.Player.PickingUpItem += EventHandlers.PickupItem;
            Timing.RunCoroutine(InfinityLoop());

        }
        public static bool loaded = false;

        private void Server_WaitingForPlayers()
        {
            var prefabs = UnityEngine.Object.FindObjectsOfType<DoorSpawnpoint>();
            foreach(var prefab in prefabs)
            {
                switch (prefab.TargetPrefab.name.ToUpper())
                {
                    case "HCZ BREAKABLEDOOR":
                        doorHCZobj = prefab.TargetPrefab.gameObject;
                        break;
                    case "LCZ BREAKABLEDOOR":
                        doorLCZobj = prefab.TargetPrefab.gameObject;
                        break;
                    case "EZ BREAKABLEDOOR":
                        doorENTobj = prefab.TargetPrefab.gameObject;
                        break;
                }
            }
            foreach(var file in Directory.GetFiles(Path.Combine(pluginDir, "maps")))
            {
                string yml = File.ReadAllText(file);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var map = deserializer.Deserialize<Map>(yml);
                if (map.LoadOnStart.Contains(Server.Port))
                {
                    map.Load();
                    maps.Add(Name, map);
                }
            }
        }

        public IEnumerator<float> InfinityLoop()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(0.5f);
                try
                {
                    foreach (KeyValuePair<string, PlayerEditorStatus> ed in Editor.playerEditors)
                    {
                        var player = Player.Get(ed.Key);
                        if (player != null)
                        {
                            if (Editor.maps.ContainsKey(ed.Value.mapName))
                            {
                                List<string> lines = new List<string>()
                                {
                                    "<color=red>|</color> MapEditor <color=red>|</color>",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    ""
                                };
                                if (ed.Value.selectedObject != null)
                                {
                                    lines[1] = $"<size=30>Object type: <color=yellow>{ed.Value.selectedObject.Object.GetType().Name}</color> | Room: <color=yellow>{ed.Value.selectedObject.Object.Room}</color> | ID: <color=yellow>{ed.Value.selectedObject.ObjectID}</color></size>";
                                    switch (ed.Value.selectedObject.Object)
                                    {
                                        case MapClassSpawn mps:
                                            lines[2] = $"<size=30>Role: <color=yellow>{mps.Class}</color></size>";
                                            break;
                                        case MapCustomObject mps:
                                            lines[2] = $"<size=30>Object name: <color=yellow>{mps.CustomObjectName}</color></size>";
                                            break;
                                        case MapDoor mps:
                                            lines[2] = $"<size=30>Door type: <color=yellow>{mps.DoorType}</color> | IsOpen: {(mps.IsOpen ? "<color=green>YES</color>" : "<color=red>NO</color>")} | IsLocked: {(mps.IsLocked ? "<color=green>YES</color>" : "<color=red>NO</color>")}</size>";
                                            lines[3] = $"<size=30>Health: <color=yellow>{mps.Health}</color>/<color=yellow>{mps.MaxHealth}</color></size>";
                                            lines[4] = $"<size=30>Keycard perms: <color=yellow>{mps.KeycardPermission}</color></size>";
                                            lines[5] = $"<size=30>Ignore damagas: <color=yellow>{mps.IgnoredDamageSources}</color></size>";
                                            break;
                                        case MapItemSpawn mps:
                                            lines[2] = $"<size=30>Item type: <color=yellow>{mps.Item}</color></size>";
                                            break;
                                    }
                                    var pos = ed.Value.selectedObject.orginalPrefab.transform.position;
                                    var rot = ed.Value.selectedObject.orginalPrefab.transform.eulerAngles;
                                    var scale = ed.Value.selectedObject.orginalPrefab.transform.localScale;
                                    lines[8] = $"<size=20>Position {string.Format("X: <color=yellow>{0:F3}</color> Y: <color=yellow>{1:F3}</color> Z: <color=yellow>{2:F3}</color>", pos.x, pos.y, pos.z)} | Rotation {string.Format("X: <color=yellow>{0:F3}</color> Y: <color=yellow>{1:F3}</color> Z: <color=yellow>{2:F3}</color>", rot.x, rot.y, rot.z)} | Scale {string.Format("X: <color=yellow>{0:F3}</color> Y: <color=yellow>{1:F3}</color> Z: <color=yellow>{2:F3}</color>", scale.x, scale.y, scale.z)}</size>";

                                }
                                player.ReferenceHub.hints.Show(new TextHint(string.Concat(
                                    Environment.NewLine,
                                    Environment.NewLine,
                                    Environment.NewLine,
                                    Environment.NewLine,
                                    Environment.NewLine,
                                    string.Join("\n", lines)
                                ), new HintParameter[]
                                {
                                    new StringHintParameter("")
                                }, null));
                            }
                            else
                            {
                                Editor.playerEditors.Remove(player.UserId);
                                player.ClearBroadcasts();
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

    }
}
