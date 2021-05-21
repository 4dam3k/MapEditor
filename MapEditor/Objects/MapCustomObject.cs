using Exiled.API.Enums;
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
    public class MapCustomObject : IMapObject
    {
        public string CustomObjectName { get; set; } = "";
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
            return null;
        }
        public void ShowEdit()
        {
        }

        public void UnShowEdit()
        {
        }
    }
}
