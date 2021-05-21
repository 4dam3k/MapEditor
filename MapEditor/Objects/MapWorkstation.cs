using Exiled.API.Enums;
using MapEditor.Models;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace MapEditor.Objects
{
    public class MapWorkstation : IMapObject
    {
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
            if (MainClass.workstationObj == null)
                MainClass.workstationObj = NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name.ToUpper().Contains("WORK"));
            return UnityEngine.Object.Instantiate(MainClass.workstationObj);
        }
        public void ShowEdit()
        {
        }

        public void UnShowEdit()
        {
        }
    }
}
