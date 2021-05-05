using Exiled.API.Features;
using Hints;
using Interactables.Interobjects.DoorUtils;
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
using static MapEditor.MapEditorModels;

namespace MapEditor
{
    public class MainClass : Plugin<MapEditorConfig>
    {
        public override string Author { get; } = "Killers0992";
        public override string Name { get; } = "MapEditor";
        public override string Prefix { get; } = "mapeditor";
        public override System.Version RequiredExiledVersion { get; } = new System.Version(2, 1, 29);
        public override System.Version Version { get; } = new System.Version(2,0,1);

        public static string pluginDir;

        public static GameObject workstationObj;
        public static GameObject doorLCZobj;
        public static GameObject doorHCZobj;
        public static GameObject doorENTobj;

        public override void OnEnabled()
        {
            Log.Info("MapEditor loaded.");
            workstationObj = NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name.ToUpper().Contains("WORK"));


            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.RestartingRound += EventHandlers.RoundRestartEvent;
            Exiled.Events.Handlers.Player.Left += EventHandlers.PlayerLeaveEvent;
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += EventHandlers.RemoteAdminCommandEvent;
            string appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            pluginDir = Path.Combine(appData, "EXILED", "Plugins", "MapEditor");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!Directory.Exists(Path.Combine(pluginDir, "maps")))
                Directory.CreateDirectory(Path.Combine(pluginDir, "maps"));
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
                    foreach (MapObject obj in map.objects)
                    {
                        obj.Load(map);
                    }
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
                                string line = "";
                                if (ed.Value.selectedObject != null)
                                {
                                    line = string.Concat(
                                        ed.Value.isWorkstation ? "<size=30>SelectedObject ID: " + ed.Value.selectedObject.name.Split('|')[1] + " MAP: " + ed.Value.selectedObject.name.Split('|')[0] + "</size>" : "<size=30>Object Name: " + ed.Value.selectedObject.name + "</size>",
                                        System.Environment.NewLine,
                                        "<size=30>Position</size> <size=30>X: " + (int)ed.Value.selectedObject.transform.position.x + " Y: " + (int)ed.Value.selectedObject.transform.position.y + " Z: " + (int)ed.Value.selectedObject.transform.position.z + "</size>",
                                        Environment.NewLine,
                                        "<size=30>Rotation</size> <size=30>X: " + (int)ed.Value.selectedObject.transform.rotation.x + " Y: " + (int)ed.Value.selectedObject.transform.rotation.y + " Z: " + (int)ed.Value.selectedObject.transform.rotation.z + "</size>",
                                        Environment.NewLine,
                                        "<size=30>Scale</size> <size=30>X: " + (int)ed.Value.selectedObject.transform.localScale.x + " Y: " + (int)ed.Value.selectedObject.transform.localScale.y + " Z: " + (int)ed.Value.selectedObject.transform.localScale.z + "</size>"
                                   );
                                }
                                player.ReferenceHub.hints.Show(new TextHint(string.Concat(
                                                                        System.Environment.NewLine,
                                    System.Environment.NewLine,
                                    " <color=red>|</color> MapEditor <color=red>|</color> ",
                                    System.Environment.NewLine,
                                    line
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

        public static GameObject GetWorkStationObject(ObjectType type, DoorSettings sett)
        {
            GameObject bench = null;
            switch (type)
            {
                case ObjectType.DoorEZ:
                    bench = UnityEngine.Object.Instantiate(doorENTobj);
                    if (sett.Locked)
                        bench.GetComponent<DoorVariant>().ServerChangeLock(DoorLockReason.AdminCommand, true);
                    bench.GetComponent<DoorVariant>().RequiredPermissions.RequiredPermissions = sett.Permissions;
                    break;
                case ObjectType.DoorHCZ:
                    bench = UnityEngine.Object.Instantiate(doorHCZobj);
                    if (sett.Locked)
                        bench.GetComponent<DoorVariant>().ServerChangeLock(DoorLockReason.AdminCommand, true);
                    bench.GetComponent<DoorVariant>().RequiredPermissions.RequiredPermissions = sett.Permissions;
                    break;
                case ObjectType.DoorLCZ:
                    bench = UnityEngine.Object.Instantiate(doorLCZobj);
                    if (sett.Locked)
                        bench.GetComponent<DoorVariant>().ServerChangeLock(DoorLockReason.AdminCommand, true);
                    bench.GetComponent<DoorVariant>().RequiredPermissions.RequiredPermissions = sett.Permissions;
                    break;
                case ObjectType.WorkStation:
                    bench = UnityEngine.Object.Instantiate(workstationObj);
                    bench.GetComponent<WorkStation>().NetworkisTabletConnected = false;
                    bench.GetComponent<WorkStation>().Network_playerConnected = null;
                    break;
            }
            return bench;
        }

        public static Vector3 Vector3To3(Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Vector3 Vec3ToVector3(Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
    }
}
