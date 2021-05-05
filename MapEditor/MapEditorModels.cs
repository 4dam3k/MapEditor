using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace MapEditor
{
    public class MapEditorModels
    {
        public enum ObjectType
        {
            WorkStation,
            DoorLCZ,
            DoorHCZ,
            DoorEZ
        }

        public class PlayerEditorStatus
        {
            public bool editingMap { get; set; } = false;
            public string mapName { get; set; } = "";
            public bool isWorkstation { get; set; } = true;
            public GameObject selectedObject { get; set; } = null;
        }

        public class ObjectPosition
        {
            public float x { get; set; } = 0f;
            public float y { get; set; } = 0f;
            public float z { get; set; } = 0f;

            public void SetVector(Vector3 vec)
            {
                x = vec.x;
                y = vec.y;
                z = vec.z;
            }

            [YamlIgnore]
            public Vector3 Vector
            {
                get
                {
                    return new Vector3(x, y, z);
                }
            }
        }
    }
}
