using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;
using MapEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace MapEditor.Objects
{
    public class MapDoor : IMapObject
    {
        public bool IsOpen { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public float Health { get; set; } = 80f;
        public float MaxHealth { get; set; } = 80f;
        public KeycardPermissions KeycardPermission { get; set; } = KeycardPermissions.None;
        public DoorDamageType IgnoredDamageSources { get; set; } = DoorDamageType.Weapon | DoorDamageType.Scp096;
        public CustomDoorType DoorType { get; set; } = CustomDoorType.EntranceZone;
        public RoomType Room { get; set; } = RoomType.Unknown;
        public ObjectPosition Position { get; set; } = new ObjectPosition();
        public ObjectPosition Rotation { get; set; } = new ObjectPosition();
        public ObjectPosition Scale { get; set; } = new ObjectPosition();
        [YamlIgnore]
        public bool IsEditMode { get; set; } = false;
        [YamlIgnore]
        public MapObject LinkedMapObject { get; set; } = null;
        public GameObject GetOrginalObject()
        {
            switch (DoorType)
            {
                case CustomDoorType.EntranceZone:
                    return MainClass.doorENTobj;
                case CustomDoorType.Heavyzone:
                    return MainClass.doorHCZobj;
                case CustomDoorType.LightZone:
                    return MainClass.doorLCZobj;
                default:
                    return null;
            }
        }

        public void ShowEdit()
        {
        }

        public void UnShowEdit()
        {
        }
    }
}
