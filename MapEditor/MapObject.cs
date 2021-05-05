using Exiled.API.Enums;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;
using static MapEditor.MapEditorModels;

namespace MapEditor
{
    public class MapObject
    {
        public int ObjectID { get; set; } = 0;
        public RoomType Room { get; set; } = RoomType.Unknown;
        public ObjectType ObjectType { get; set; } = ObjectType.WorkStation;
        public DoorSettings DoorSettings { get; set; } = null;
        public ObjectPosition position { get; set; } = new ObjectPosition();
        public ObjectPosition scale { get; set; } = new ObjectPosition();
        public ObjectPosition rotation { get; set; } = new ObjectPosition();
        [YamlIgnore]
        public GameObject orginalPrefab;

        public void Init(Map map, int id, ObjectType type, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            ObjectID = id;
            ObjectType = type;
            if (type == ObjectType)
                DoorSettings = new DoorSettings();
            this.position.SetVector(position);
            this.scale.SetVector(scale);
            this.rotation.SetVector(rotation.eulerAngles);
            Load(map);
        }

        public MapObject Clone()
        {
            return new MapObject()
            {
                ObjectType = this.ObjectType,
                position = this.position,
                scale = this.scale,
                rotation = this.rotation
            };
        }

        public void UpdateData()
        {
            var room = GetRoom();
            if (room != null)
            {
                this.position.SetVector(room.transform.InverseTransformPoint(orginalPrefab.transform.position));
                this.rotation.SetVector(room.transform.InverseTransformDirection(orginalPrefab.transform.eulerAngles));
                this.Room = room.Type;
            }
            else
            {
                this.position.SetVector(orginalPrefab.transform.position);
                this.rotation.SetVector(orginalPrefab.transform.eulerAngles);
                this.Room = RoomType.Unknown;
            }
            this.scale.SetVector(orginalPrefab.transform.localScale);
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

        public void Load(Map map)
        {
            if (orginalPrefab != null)
                NetworkServer.Destroy(orginalPrefab);
            if (this.Room != RoomType.Unknown)
            {
                orginalPrefab = UnityEngine.Object.Instantiate(MainClass.GetWorkStationObject(ObjectType, DoorSettings));
                var t = Exiled.API.Features.Map.Rooms.Where(p => p.Type == this.Room).First().transform;
                orginalPrefab.transform.position = t.transform.TransformPoint(position.Vector);
                if (rotation.Vector != Vector3.zero)
                    orginalPrefab.transform.rotation = Quaternion.Euler(t.transform.TransformDirection(rotation.Vector));
            }
            else
            {
                orginalPrefab = UnityEngine.Object.Instantiate(MainClass.GetWorkStationObject(ObjectType, DoorSettings));
                orginalPrefab.transform.position = position.Vector;
                if (rotation.Vector != Vector3.zero)
                    orginalPrefab.transform.rotation = Quaternion.Euler(rotation.Vector);
            }

            if (scale.Vector != Vector3.zero)
                orginalPrefab.transform.localScale = scale.Vector;
            orginalPrefab.name = $"{map.MapName}|{ObjectID}";
            NetworkServer.Spawn(orginalPrefab);
            UpdateData();
        }
    }

    public class DoorSettings
    {
        public bool Locked { get; set; } = false;
        public KeycardPermissions Permissions { get; set; } = KeycardPermissions.None;
    }
}
