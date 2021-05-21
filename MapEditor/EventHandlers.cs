using Mirror;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs;
using UnityEngine;
using Exiled.Permissions.Extensions;
using MapEditor.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using MapEditor.Objects;
using Exiled.API.Features;
using MapEditor.Models.Schematic;
using Interactables.Interobjects.DoorUtils;

namespace MapEditor
{
    public class EventHandlers
    {


        public static void RemoteAdminCommandEvent(SendingRemoteAdminCommandEventArgs ev)
        {


            switch (ev.Name.ToLower())
            {
                case "mapeditor":
                    ev.IsAllowed = false;
                    if (!ev.Sender.CheckPermission("mapeditor"))
                    {
                        ev.CommandSender.RaReply("MapEditor#No permission.", true, true, string.Empty);
                        return;
                    }
                    if (ev.Arguments.Count == 0)
                    {
                        ev.CommandSender.RaReply("MapEditor#Commands:", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR UNLOAD <NAME> - Unloads map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR LOAD <NAME> - Load map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR CREATE <NAME> - Create map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR DELETE <NAME> - Delete map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR CREATEOBJECT door/workstation/itemspawn/spawnpoint/customobject - Create object.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR DELETEOBJECT - Delete object.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SELECTOBJECT - Select object.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR CLONEOBJECT - Clone object.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR CANCEL - Cancels map editing.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR EDIT <NAME> - Edits map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SAVE - Save map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETPOS <X> <Y> <Z> - Set object position.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETROT <X> <Y> <Z> - Set object rotation.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETSCALE <X> <Y> <Z> - Set object scale.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETLOCK - Set door lock.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR BRING - Bring object to player position.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETCUSTOMOBJECT <name> - Set custom object which will be loaded from schematics.", true, true, "");
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETDOORTYPE lcz/hcz/ez - Set door type.", true, true, "");
                        return;
                    }
                    else if (ev.Arguments.Count != 0)
                    {
                        switch(ev.Arguments[0].ToLower())
                        {
                            case "unload":
                                if (ev.Arguments.Count == 2)
                                {
                                    if (Editor.maps.ContainsKey(ev.Arguments[1]))
                                    {
                                        Editor.maps[ev.Arguments[1]].Unload();
                                        Editor.maps.Remove(ev.Arguments[1]);
                                        ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} unloaded.", true, "MAPEDITOR");
                                    }
                                    else
                                    {
                                        ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} is not loaded.", true, "MAPEDITOR");
                                    }
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR unload <mapName>.", true, "MAPEDITOR");
                                }
                                break;
                            case "load":
                                if (ev.Arguments.Count == 2)
                                {
                                    if (Editor.maps.ContainsKey(ev.Arguments[1]))
                                    {
                                        ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} is already loaded.", true, "MAPEDITOR");
                                    }
                                    else
                                    {
                                        string path4 = Path.Combine(MainClass.pluginDir, "maps", ev.Arguments[1] + ".yml");
                                        if (!File.Exists(path4))
                                        {
                                            ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} not found.", true, "MAPEDITOR");
                                            return;
                                        }
                                        string yml = File.ReadAllText(path4);
                                        var deserializer = new DeserializerBuilder()
                                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                            .Build();
                                        var map9 = deserializer.Deserialize<Map>(yml);
                                        map9.Load();
                                        Editor.maps.Add(ev.Arguments[1], map9);
                                        ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} loaded.");
                                    }
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR load <mapName>.", true, "MAPEDITOR");
                                }
                                break;
                            case "create":
                                if (ev.Arguments.Count == 2)
                                {
                                    string path5 = Path.Combine(MainClass.pluginDir, "maps", ev.Arguments[1] + ".yml");
                                    if (File.Exists(path5))
                                    {
                                        ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} already exists.", true, "MAPEDITOR");
                                        return;
                                    }
                                    var serializer5 = new SerializerBuilder()
                                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                        .Build();
                                    File.WriteAllText(path5, serializer5.Serialize(new Map() { MapName = ev.Arguments[1] }));
                                    ev.Sender.RemoteAdminMessage($"Created map with name {ev.Arguments[1]}.", true, "MAPEDITOR");
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR create <mapName>.", true, "MAPEDITOR");
                                }
                                break;
                            case "delete":
                                if (ev.Arguments.Count == 2)
                                {
                                    string path6 = Path.Combine(MainClass.pluginDir, "maps", ev.Arguments[1] + ".yml");
                                    if (!File.Exists(path6))
                                    {
                                        ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} does not exist.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (Editor.maps.ContainsKey(ev.Arguments[1]))
                                    {
                                        Editor.maps[ev.Arguments[1]].Unload();
                                        Editor.maps.Remove(ev.Arguments[1]);
                                    }
                                    File.Delete(path6);
                                    ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} deleted.", true, "MAPEDITOR");
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR delete <mapName>.", true, "MAPEDITOR");
                                }
                                break;
                            case "createobject":
                                if (ev.Arguments.Count == 2)
                                {
                                    if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor0))
                                    {
                                        ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                        return;
                                    }

                                    int freeID = 0;

                                    var map7 = Editor.maps[editor0.mapName];

                                    List<int> ids = new List<int>();
                                    IMapObject defobj = null;

                                    switch (ev.Arguments[1].ToUpper())
                                    {
                                        case "DOOR":
                                            ids = map7.CustomDoors.Keys.ToList();
                                            defobj = new MapDoor();
                                            freeID = Enumerable.Range(1, int.MaxValue).Except(ids).First();
                                            map7.CustomDoors.Add(freeID, defobj as MapDoor);
                                            break;
                                        case "WORKSTATION":
                                            ids = map7.CustomWorkstations.Keys.ToList();
                                            defobj = new MapWorkstation();
                                            freeID = Enumerable.Range(1, int.MaxValue).Except(ids).First();
                                            map7.CustomWorkstations.Add(freeID, defobj as MapWorkstation);
                                            break;
                                        case "CUSTOMOBJECT":
                                            ids = map7.CustomObjects.Keys.ToList();
                                            defobj = new MapCustomObject();
                                            freeID = Enumerable.Range(1, int.MaxValue).Except(ids).First();
                                            map7.CustomObjects.Add(freeID, defobj as MapCustomObject);
                                            break;
                                        case "SPAWNPOINT":
                                            ids = map7.CustomSpawnpoints.Keys.ToList();
                                            defobj = new MapClassSpawn();
                                            freeID = Enumerable.Range(1, int.MaxValue).Except(ids).First();
                                            map7.CustomSpawnpoints.Add(freeID, defobj as MapClassSpawn);
                                            break;
                                        case "ITEMSPAWN":
                                            ids = map7.CustomItemSpawns.Keys.ToList();
                                            defobj = new MapItemSpawn();
                                            freeID = Enumerable.Range(1, int.MaxValue).Except(ids).First();
                                            map7.CustomItemSpawns.Add(freeID, defobj as MapItemSpawn);
                                            break;
                                        default:
                                            ev.Sender.RemoteAdminMessage($"Object {ev.Arguments[1]} not found, use <color=green>DOOR/WORKSTATION/ITEMSPAWN/SPAWNPOINT/CUSTOMOBJECT", true, "MAPEDITOR");
                                            return;
                                    }
                                    var gam = new GameObject("MAPEDITOR");
                                    MapObject obj = gam.AddComponent<MapObject>();

                                    RaycastHit raycastHit;

                                    if (Physics.Raycast(ev.Sender.CameraTransform.position, ev.Sender.CameraTransform.forward, out raycastHit, 40f, ev.Sender.ReferenceHub.scp106PlayerScript.teleportPlacementMask))
                                    {
                                        Log.Debug("Raycast hit " + raycastHit.point);
                                        obj.Init(map7, freeID, defobj, raycastHit.point, Vector3.zero, Quaternion.Euler(Vector3.zero));
                                        Editor.playerEditors[ev.Sender.UserId].selectedObject = obj;
                                        ev.Sender.RemoteAdminMessage($"Created object {ev.Arguments[1]}.", true, "MAPEDITOR");
                                    }
                                    else
                                    {
                                        ev.Sender.RemoteAdminMessage("Not valid surface", true, "MAPEDITOR");
                                    }
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR createobject door/workstation/itemspawn/spawnpoint/customobject.", true, "MAPEDITOR");
                                }
                                break;
                            case "deleteobject":
                                if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor))
                                {
                                    ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                    return;
                                }
                                var map = Editor.maps[editor.mapName];
                                switch (editor.selectedObject.Object)
                                {
                                    case MapClassSpawn ms:
                                        if (map.CustomSpawnpoints.ContainsKey(editor.selectedObject.ObjectID))
                                        {
                                            UnityEngine.Object.Destroy(editor.selectedObject);
                                            map.CustomSpawnpoints.Remove(editor.selectedObject.ObjectID);
                                        }
                                        ev.Sender.RemoteAdminMessage($"Object ClassSpawn deleted.", true, "MAPEDITOR");
                                        return;
                                    case MapCustomObject ms:
                                        if (map.CustomObjects.ContainsKey(editor.selectedObject.ObjectID))
                                        {
                                            UnityEngine.Object.Destroy(editor.selectedObject);
                                            map.CustomObjects.Remove(editor.selectedObject.ObjectID);
                                        }
                                        ev.Sender.RemoteAdminMessage($"Object CustomObject deleted.", true, "MAPEDITOR");
                                        return;
                                    case MapDoor ms:
                                        if (map.CustomDoors.ContainsKey(editor.selectedObject.ObjectID))
                                        {
                                            UnityEngine.Object.Destroy(editor.selectedObject);
                                            map.CustomDoors.Remove(editor.selectedObject.ObjectID);
                                        }
                                        ev.Sender.RemoteAdminMessage($"Object Door deleted.", true, "MAPEDITOR");
                                        return;
                                    case MapItemSpawn ms:
                                        if (map.CustomItemSpawns.ContainsKey(editor.selectedObject.ObjectID))
                                        {
                                            UnityEngine.Object.Destroy(editor.selectedObject);
                                            map.CustomItemSpawns.Remove(editor.selectedObject.ObjectID);
                                        }
                                        ev.Sender.RemoteAdminMessage($"Object ItemSpawn deleted.", true, "MAPEDITOR");
                                        return;
                                    case MapWorkstation ms:
                                        if (map.CustomWorkstations.ContainsKey(editor.selectedObject.ObjectID))
                                        {
                                            UnityEngine.Object.Destroy(editor.selectedObject);
                                            map.CustomWorkstations.Remove(editor.selectedObject.ObjectID);
                                        }
                                        ev.Sender.RemoteAdminMessage($"Object WorkStation deleted.", true, "MAPEDITOR");
                                        return;
                                }
                                return;
                            case "selectobject":
                                if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor2))
                                {
                                    ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                    return;
                                }
                                Physics.Raycast(ev.Sender.CameraTransform.position, ev.Sender.CameraTransform.forward, out RaycastHit where, 40f);
                                if (where.point.Equals(Vector3.zero))
                                {
                                    ev.Sender.RemoteAdminMessage("Invalid position, raycast didnt hit any of mapeditor objects.", true, "MAPEDITOR");
                                }
                                else
                                {
                                    var ob = where.transform.root.gameObject.GetComponentInChildren<MapObject>();
                                    var ob2 = where.transform.root.gameObject.GetComponentInChildren<SchematicLoaded>();
                                    if (ob != null)
                                    {
                                        if (ob.Map.MapName != editor2.mapName)
                                        {
                                            ev.Sender.RemoteAdminMessage($"Selected object <color=green>{ob.Object.GetType().Name}</color> is from map <color=green>{ob.Map.MapName}</color>, you are currently editing <color=green>{editor2.mapName}</color>.", true, "MAPEDITOR");
                                            return;
                                        }
                                        editor2.selectedObject = ob;
                                        ob.Object.ShowEdit();
                                        ev.Sender.RemoteAdminMessage($"Selected object {ob.Object.GetType().Name}.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (ob2 != null)
                                    {
                                        if (ob2.LinkedObject == null)
                                        {
                                            ev.Sender.RemoteAdminMessage($"Invalid object.", true, "MAPEDITOR");
                                            return;
                                        }
                                        if (ob2.LinkedObject.Map.MapName != editor2.mapName)
                                        {
                                            ev.Sender.RemoteAdminMessage($"Selected object <color=green>{ob.Object.GetType().Name}</color> is from map <color=green>{ob.Map.MapName}</color>, you are currently editing <color=green>{editor2.mapName}</color>.", true, "MAPEDITOR");
                                            return;
                                        }
                                        editor2.selectedObject = ob2.LinkedObject;
                                        ob2.LinkedObject.Object.ShowEdit();
                                        ev.Sender.RemoteAdminMessage($"Selected object {ob2.LinkedObject.Object.GetType().Name}.", true, "MAPEDITOR");
                                        return;
                                    }
                                    ev.Sender.RemoteAdminMessage($"Invalid object.", true, "MAPEDITOR");
                                }
                                break;
                            case "cancel":
                                if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor3))
                                {
                                    ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                    return;
                                }
                                if (editor3.selectedObject != null)
                                {
                                    editor3.selectedObject.Object.UnShowEdit();
                                }
                                ev.Sender.RemoteAdminMessage($"Editing map {editor3.mapName} finished.", true, "MAPEDITOR");
                                Editor.playerEditors.Remove(ev.Sender.UserId);
                                break;
                            case "edit":
                                if (ev.Arguments.Count == 2)
                                {
                                    if (!Editor.maps.ContainsKey(ev.Arguments[1]))
                                    {
                                        ev.Sender.RemoteAdminMessage($"Map {ev.Arguments[1]} not loaded, use <color=green>MAPEDITOR load {ev.Arguments[1]}</color>", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor4))
                                    {
                                        ev.Sender.RemoteAdminMessage($"You are currently editing the <color=green>{editor4.mapName}</color> map, to stop editing type <color=green>mapeditor cancel</color>");
                                        return;
                                    }
                                    Editor.playerEditors.Add(ev.Sender.UserId, new PlayerEditorStatus()
                                    {
                                        editingMap = true,
                                        mapName = ev.Arguments[1],
                                        selectedObject = null
                                    });
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR edit <mapName>.", true, "MAPEDITOR");
                                }

                                break;
                            case "save":
                                if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor5))
                                {
                                    ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                    return;
                                }

                                var serializer = new SerializerBuilder()
                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                    .Build();
                                string path = Path.Combine(MainClass.pluginDir, "maps", editor5.mapName + ".yml");
                                File.WriteAllText(path, serializer.Serialize(Editor.maps[editor5.mapName]));
                                ev.Sender.RemoteAdminMessage($"Map {editor5.mapName} saved.", true, "MAPEDITOR");
                                break;
                            case "setpos":
                                if (ev.Arguments.Count == 4)
                                {
                                    if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor6))
                                    {
                                        ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (editor6.selectedObject == null)
                                    {
                                        ev.Sender.RemoteAdminMessage($"Object not selected, use <color=gren>MAPEDITOR selectobject</color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (float.TryParse(ev.Arguments[1], out float x))
                                    {
                                        if (float.TryParse(ev.Arguments[2], out float y))
                                        {
                                            if (float.TryParse(ev.Arguments[3], out float z))
                                            {
                                                NetworkServer.UnSpawn(editor6.selectedObject.orginalPrefab);
                                                editor6.selectedObject.orginalPrefab.transform.position = new Vector3(x, y, z);
                                                editor6.selectedObject.UpdateData();
                                                NetworkServer.Spawn(editor6.selectedObject.orginalPrefab);
                                            }
                                        }
                                    }
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR setpos <X> <Y> <Z>.", true, "MAPEDITOR");
                                    return;
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR setpos <X> <Y> <Z>.", true, "MAPEDITOR");
                                }
                                break;
                            case "setrot":
                                if (ev.Arguments.Count == 4)
                                {
                                    if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor6))
                                    {
                                        ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (editor6.selectedObject == null)
                                    {
                                        ev.Sender.RemoteAdminMessage($"Object not selected, use <color=gren>MAPEDITOR selectobject</color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (float.TryParse(ev.Arguments[1], out float x))
                                    {
                                        if (float.TryParse(ev.Arguments[2], out float y))
                                        {
                                            if (float.TryParse(ev.Arguments[3], out float z))
                                            {
                                                NetworkServer.UnSpawn(editor6.selectedObject.orginalPrefab);
                                                editor6.selectedObject.orginalPrefab.transform.eulerAngles = new Vector3(x, y, z);
                                                editor6.selectedObject.UpdateData();
                                                NetworkServer.Spawn(editor6.selectedObject.orginalPrefab);
                                            }
                                        }
                                    }
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR setrot <X> <Y> <Z>.", true, "MAPEDITOR");
                                    return;
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR setrot <X> <Y> <Z>.", true, "MAPEDITOR");
                                }
                                break;
                            case "setscale":
                                if (ev.Arguments.Count == 4)
                                {
                                    if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor6))
                                    {
                                        ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (editor6.selectedObject == null)
                                    {
                                        ev.Sender.RemoteAdminMessage($"Object not selected, use <color=gren>MAPEDITOR selectobject</color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (float.TryParse(ev.Arguments[1], out float x))
                                    {
                                        if (float.TryParse(ev.Arguments[2], out float y))
                                        {
                                            if (float.TryParse(ev.Arguments[3], out float z))
                                            {
                                                NetworkServer.UnSpawn(editor6.selectedObject.orginalPrefab);
                                                editor6.selectedObject.orginalPrefab.transform.localScale = new Vector3(x, y, z);
                                                editor6.selectedObject.UpdateData();
                                                NetworkServer.Spawn(editor6.selectedObject.orginalPrefab);
                                            }
                                        }
                                    }
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR setpos <X> <Y> <Z>.", true, "MAPEDITOR");
                                    return;
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR setpos <X> <Y> <Z>.", true, "MAPEDITOR");
                                }
                                break;
                            case "cloneobject":
                                if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor69))
                                {
                                    ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                    return;
                                }
                                if (editor69.selectedObject == null)
                                {
                                    ev.Sender.RemoteAdminMessage($"Object not selected, use <color=gren>MAPEDITOR selectobject</color>.", true, "MAPEDITOR");
                                    return;
                                }
                                List<int> ids2 = new List<int>();
                                IMapObject defobj2 = null;
                                int freeID2 = 0;
                                var p = Editor.maps[editor69.mapName];
                                switch (editor69.selectedObject.Object)
                                {
                                    case MapDoor dr:
                                        ids2 = p.CustomDoors.Keys.ToList();
                                        defobj2 = dr;
                                        freeID2 = Enumerable.Range(1, int.MaxValue).Except(ids2).First();
                                        p.CustomDoors.Add(freeID2, defobj2 as MapDoor);
                                        break;
                                    case MapWorkstation wr:
                                        ids2 = p.CustomWorkstations.Keys.ToList();
                                        defobj2 = wr;
                                        freeID2 = Enumerable.Range(1, int.MaxValue).Except(ids2).First();
                                        p.CustomWorkstations.Add(freeID2, defobj2 as MapWorkstation);
                                        break;
                                    case MapCustomObject co:
                                        ids2 = p.CustomObjects.Keys.ToList();
                                        defobj2 = co;
                                        freeID2 = Enumerable.Range(1, int.MaxValue).Except(ids2).First();
                                        p.CustomObjects.Add(freeID2, defobj2 as MapCustomObject);
                                        break;
                                    case MapClassSpawn sp:
                                        ids2 = p.CustomSpawnpoints.Keys.ToList();
                                        defobj2 = sp;
                                        freeID2 = Enumerable.Range(1, int.MaxValue).Except(ids2).First();
                                        p.CustomSpawnpoints.Add(freeID2, defobj2 as MapClassSpawn);
                                        break;
                                    case MapItemSpawn wr2:
                                        ids2 = p.CustomItemSpawns.Keys.ToList();
                                        defobj2 = wr2;
                                        freeID2 = Enumerable.Range(1, int.MaxValue).Except(ids2).First();
                                        p.CustomItemSpawns.Add(freeID2, defobj2 as MapItemSpawn);
                                        break;
                                }

                                var gm = new GameObject("MAPEDITOR");
                                var dm = gm.AddComponent<MapObject>();
                                dm.Init(Editor.maps[editor69.mapName], freeID2, defobj2, editor69.selectedObject.orginalPrefab.transform.position, editor69.selectedObject.orginalPrefab.transform.localScale, editor69.selectedObject.orginalPrefab.transform.rotation);
                                editor69.selectedObject = dm;
                                ev.Sender.RemoteAdminMessage($"Cloned object {dm.Object.GetType().Name}.", true, "MAPEDITOR");
                                break;
                            case "setlock":
                                if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor692))
                                {
                                    ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                    return;
                                }
                                if (editor692.selectedObject == null)
                                {
                                    ev.Sender.RemoteAdminMessage($"Object not selected, use <color=gren>MAPEDITOR selectobject</color>.", true, "MAPEDITOR");
                                    return;
                                }
                                if (editor692.selectedObject.Object is MapDoor md)
                                {
                                    md.IsLocked = !md.IsLocked;
                                    editor692.selectedObject.orginalPrefab.GetComponent<DoorVariant>().ServerChangeLock(DoorLockReason.AdminCommand, md.IsLocked);
                                    ev.Sender.RemoteAdminMessage($"Changed lock state to {md.IsLocked}.", true, "MAPEDITOR");
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"To use that command object must be a door.", true, "MAPEDITOR");
                                }
                                break;
                            case "bring":
                                if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor6922))
                                {
                                    ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                    return;
                                }
                                if (editor6922.selectedObject == null)
                                {
                                    ev.Sender.RemoteAdminMessage($"Object not selected, use <color=gren>MAPEDITOR selectobject</color>.", true, "MAPEDITOR");
                                    return;
                                }

                                NetworkServer.UnSpawn(editor6922.selectedObject.orginalPrefab);
                                editor6922.selectedObject.orginalPrefab.transform.position = ev.Sender.Position;
                                editor6922.selectedObject.UpdateData();
                                NetworkServer.Spawn(editor6922.selectedObject.orginalPrefab);
                                ev.Sender.RemoteAdminMessage($"Object teleported to your location.", true, "MAPEDITOR");
                                break;
                            case "setcustomobject":
                                if (ev.Arguments.Count == 2)
                                {
                                    if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor02))
                                    {
                                        ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (editor02.selectedObject == null)
                                    {
                                        ev.Sender.RemoteAdminMessage($"Object not selected, use <color=gren>MAPEDITOR selectobject</color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (editor02.selectedObject.Object is MapCustomObject md2)
                                    {
                                        md2.CustomObjectName = ev.Arguments[1];
                                        ev.Sender.RemoteAdminMessage($"Changed custom object to {ev.Arguments[1]}.", true, "MAPEDITOR");
                                    }
                                    else
                                    {
                                        ev.Sender.RemoteAdminMessage($"To use that command object must be a customobject.", true, "MAPEDITOR");
                                    }
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR setcustomobject <schematicName>.", true, "MAPEDITOR");
                                }
                                break;
                            case "setdoortype":
                                if (ev.Arguments.Count == 2)
                                {
                                    if (!Editor.playerEditors.TryGetValue(ev.Sender.UserId, out PlayerEditorStatus editor021))
                                    {
                                        ev.Sender.RemoteAdminMessage("Enable edit mode by using command, <color=green>MAPEDITOR edit <mapName></color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (editor021.selectedObject == null)
                                    {
                                        ev.Sender.RemoteAdminMessage($"Object not selected, use <color=gren>MAPEDITOR selectobject</color>.", true, "MAPEDITOR");
                                        return;
                                    }
                                    if (editor021.selectedObject.Object is MapDoor md3)
                                    {
                                        switch (ev.Arguments[1].ToUpper())
                                        {
                                            case "HCZ":
                                                md3.DoorType = CustomDoorType.Heavyzone;
                                                break;
                                            case "LCZ":
                                                md3.DoorType = CustomDoorType.LightZone;
                                                break;
                                            case "EZ":
                                                md3.DoorType = CustomDoorType.EntranceZone;
                                                break;
                                            default:
                                                ev.Sender.RemoteAdminMessage($"Door with name {ev.Arguments[1]} not exists, use HCZ/LCZ/EZ.", true, "MAPEDITOR");
                                                return;
                                        }
                                        editor021.selectedObject.Load(editor021.selectedObject.Map);
                                        ev.Sender.RemoteAdminMessage($"Changed door type to {md3.DoorType}.", true, "MAPEDITOR");
                                    }
                                    else
                                    {
                                        ev.Sender.RemoteAdminMessage($"To use that command object must be a door.", true, "MAPEDITOR");
                                    }
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage($"Syntax: MAPEDITOR setdoortype lcz/hcz/ez.", true, "MAPEDITOR");
                                }
                                break;
                        }
                        return;
                    }
                    break;
            }
        }

        public static void PickupItem(PickingUpItemEventArgs ev)
        {
            MainClass.schem.PickupItem(ref ev);
        }

        public static void PlayerLeaveEvent(DestroyingEventArgs ev)
        {
            if (Editor.playerEditors.ContainsKey(ev.Player.UserId))
                Editor.playerEditors.Remove(ev.Player.UserId);
        }

        public static void RoundRestartEvent()
        {
            Editor.playerEditors = new Dictionary<string, PlayerEditorStatus>();
            Editor.maps.Clear();
        }
    }
}
