using Exiled.API.Enums;
using Exiled.API.Features;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapEditor.Models;
using MapEditor.Models.Schematic;
using MapEditor.Objects;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace MapEditor
{
    public class MapObject : MonoBehaviour
    {
        public int ObjectID { get; set; } = 0;
        public IMapObject Object { get; set; }

        public GameObject orginalPrefab;
        public SchematicLoaded attachedCustomObject;

        public Map Map;

        void Awake()
        {
            this.gameObject.layer = 6969;
            this.gameObject.AddComponent<BoxCollider>().size = new Vector3(1f, 1f, 1f);
        }

        void OnDestroy()
        {
            if (Object is MapCustomObject pr)
                Schematic.UnloadSchematic("SCHEM" + pr.CustomObjectName + ObjectID);
            if (orginalPrefab != null)
                NetworkServer.Destroy(orginalPrefab);
            Log.Debug("Unload object with id " + ObjectID);
        }

        public void Init(Map map, int id, IMapObject obj, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            ObjectID = id;
            Object = obj;
            Map = map;
            Object.LinkedMapObject = this;
            Object.Position.SetVector(position);
            Object.Scale.SetVector(scale);
            Object.Rotation.SetVector(rotation.eulerAngles);
            Log.Debug($"Init object with values, ID: {ObjectID}, Type: {obj.GetType().Name}, position: {position}, rotation: {rotation}, scale: {scale}");
            Load(map);
        }

        public IMapObject Clone()
        {
            return Object;
        }

        public void UpdateData()
        {
            var room = GetRoom();
            if (room != null)
            {
                Object.Position.SetVector(room.transform.InverseTransformPoint(orginalPrefab.transform.position));
                this.transform.position = Object.Position.Vector;
                Object.Rotation.SetVector(room.transform.InverseTransformDirection(orginalPrefab.transform.eulerAngles));
                this.transform.eulerAngles = Object.Rotation.Vector;
                Object.Room = room.Type;
            }
            else
            {
                Object.Position.SetVector(orginalPrefab.transform.position);
                this.transform.position = Object.Position.Vector;
                Object.Rotation.SetVector(orginalPrefab.transform.eulerAngles);
                this.transform.eulerAngles = Object.Rotation.Vector;
                Object.Room = RoomType.Unknown;
            }
            switch (Object)
            {
                case MapItemSpawn obs:
                    NetworkServer.UnSpawn(orginalPrefab);
                    orginalPrefab.GetComponent<Pickup>().NetworkitemId = obs.Item;
                    NetworkServer.Spawn(orginalPrefab);
                    break;
                case MapCustomObject co:
                    Schematic.UnloadSchematic("SCHEM" + co.CustomObjectName + ObjectID);
                    attachedCustomObject = Schematic.LoadSchematic2("SCHEM" + co.CustomObjectName + ObjectID, Path.Combine(MainClass.pluginDir, "schematics", "schematic-" + co.CustomObjectName + ".json"), orginalPrefab.transform.position, orginalPrefab.transform.eulerAngles, orginalPrefab.transform.localScale, ItemType.None, true);
                    attachedCustomObject.LinkedObject = this;
                    break;
            }
            Object.Scale.SetVector(orginalPrefab.transform.localScale);
        }

        public Exiled.API.Features.Room GetRoom()
        {
            Vector3 end = orginalPrefab.transform.position - new Vector3(0f, 10f, 0f);
            bool flag = Physics.Linecast(orginalPrefab.transform.position, end, out RaycastHit raycastHit, -84058629);

            if (!flag || raycastHit.transform == null)
                return null;

            Transform latestParent = raycastHit.transform;
            while (latestParent.parent?.parent != null)
                latestParent = latestParent.parent;

            foreach (Exiled.API.Features.Room room in Exiled.API.Features.Map.Rooms)
            {
                if (room.Transform == latestParent)
                    return room;
            }
            return null;
        }

		public string GetSpawnName(RoleType classID)
		{

            switch (classID)
            {
                case RoleType.Scp106:
                    return "SP_106";
                case RoleType.Scp049:
                    return "SP_049";
                case RoleType.Scp079:
                    return "SP_079";
                case RoleType.Scp096:
                    return "SP_096";
                case RoleType.Scp93953:
                case RoleType.Scp93989:
                    return "SP_939";
                case RoleType.Scp173:
                    return "SP_173";
                case RoleType.FacilityGuard:
                    return "SP_GUARD";
                case RoleType.NtfCadet:
                case RoleType.NtfCommander:
                case RoleType.NtfLieutenant:
                case RoleType.NtfScientist:
                    return "SP_MTF";
                case RoleType.ChaosInsurgency:
                    return "SP_CI";
                case RoleType.Scientist:
                    return "SP_RSC";
                case RoleType.ClassD:
                    return "SP_CDP";
                default:
                    return "UNKNOWN";
            }
		}


		public void Load(Map map)
        {
            if (orginalPrefab != null)
                NetworkServer.Destroy(orginalPrefab);
            if (Object.Room != RoomType.Unknown)
            {
                switch (Object)
                {
                    case MapClassSpawn sp:
                        var ob = new GameObject("SPAWNPOINT");
                        ob.tag = GetSpawnName(sp.Class);
                        orginalPrefab = ob;
                        break;
                    case MapDoor dr:
                        orginalPrefab = UnityEngine.Object.Instantiate(dr.GetOrginalObject(), this.transform);
                        if (orginalPrefab.TryGetComponent<BreakableDoor>(out BreakableDoor dv))
                        {
                            dv.TargetState = dr.IsOpen;
                            if (dr.IsLocked)
                                dv.ServerChangeLock(DoorLockReason.AdminCommand, true);
                            dv.RequiredPermissions.RequiredPermissions = dr.KeycardPermission;
                            dv._ignoredDamageSources = dr.IgnoredDamageSources;
                        }
                        break;
                    case MapItemSpawn im:
                        orginalPrefab = im.GetOrginalObject();
                        orginalPrefab.GetComponent<Pickup>().SetupPickup(im.Item, -1f, ReferenceHub.HostHub.gameObject, new Pickup.WeaponModifiers(false, 0, 0, 0), gameObject.transform.position, gameObject.transform.rotation);
                        break;
                    case MapCustomObject co:
                        orginalPrefab = new GameObject("CO");
                        break;
                    case MapWorkstation wr:
                        orginalPrefab = wr.GetOrginalObject();
                        break;
                }
                var t = Exiled.API.Features.Map.Rooms.Where(p => p.Type == Object.Room).First().transform;
                orginalPrefab.transform.position = t.transform.TransformPoint(Object.Position.Vector);
                if (Object.Rotation.Vector != Vector3.zero)
                    orginalPrefab.transform.rotation = Quaternion.Euler(t.transform.TransformDirection(Object.Rotation.Vector));
                if (Object is MapCustomObject co2)
                {
                    if (File.Exists(Path.Combine(MainClass.pluginDir, "schematics", "schematic-" + co2.CustomObjectName + ".json")))
                    {
                        attachedCustomObject = Schematic.LoadSchematic2("SCHEM" + co2.CustomObjectName + ObjectID, Path.Combine(MainClass.pluginDir, "schematics", "schematic-" + co2.CustomObjectName + ".json"), orginalPrefab.transform.position, orginalPrefab.transform.eulerAngles, orginalPrefab.transform.localScale, ItemType.None, true);
                        attachedCustomObject.LinkedObject = this;
                    }
                }
            }
            else
            {
                switch (Object)
                {
                    case MapClassSpawn sp:
                        var ob = new GameObject("SPAWNPOINT");
                        ob.tag = GetSpawnName(sp.Class);
                        orginalPrefab = ob;
                        break;
                    case MapDoor dr:
                        orginalPrefab = UnityEngine.Object.Instantiate(dr.GetOrginalObject(), this.transform);
                        if (orginalPrefab.TryGetComponent<BreakableDoor>(out BreakableDoor dv))
                        {
                            dv.TargetState = dr.IsOpen;
                            if (dr.IsLocked)
                                dv.ServerChangeLock(DoorLockReason.AdminCommand, true);
                            dv.RequiredPermissions.RequiredPermissions = dr.KeycardPermission;
                            dv._ignoredDamageSources = dr.IgnoredDamageSources;
                        }
                        break;
                    case MapItemSpawn im:
                        orginalPrefab = im.GetOrginalObject();
                        orginalPrefab.GetComponent<Rigidbody>().isKinematic = true;
                        orginalPrefab.AddComponent<DisablePickupInteract>();
                        orginalPrefab.GetComponent<Pickup>().SetupPickup(im.Item, -1f, ReferenceHub.HostHub.gameObject, new Pickup.WeaponModifiers(false, 0, 0, 0), gameObject.transform.position, gameObject.transform.rotation);
                        break;
                    case MapCustomObject co:
                        orginalPrefab = new GameObject("CO");
                        break;
                    case MapWorkstation wr:
                        orginalPrefab = wr.GetOrginalObject();
                        break;
                }
                orginalPrefab.transform.position = Object.Position.Vector;
                if (Object.Rotation.Vector != Vector3.zero)
                    orginalPrefab.transform.rotation = Quaternion.Euler(Object.Rotation.Vector);
                if (Object is MapCustomObject co2)
                {
                    if (File.Exists(Path.Combine(MainClass.pluginDir, "schematics", "schematic-" + co2.CustomObjectName + ".json")))
                    {
                        attachedCustomObject = Schematic.LoadSchematic2("SCHEM" + co2.CustomObjectName + ObjectID, Path.Combine(MainClass.pluginDir, "schematics", "schematic-" + co2.CustomObjectName + ".json"), orginalPrefab.transform.position, orginalPrefab.transform.eulerAngles, orginalPrefab.transform.localScale, ItemType.None, true);
                        attachedCustomObject.LinkedObject = this;
                    }
                }
            }
            orginalPrefab.transform.parent = this.transform;
            if (Object.Scale.Vector != Vector3.zero)
                orginalPrefab.transform.localScale = Object.Scale.Vector;
            NetworkServer.Spawn(orginalPrefab);
            UpdateData();
        }
    }
}
