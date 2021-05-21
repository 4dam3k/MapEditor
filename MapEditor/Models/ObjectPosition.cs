using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace MapEditor.Models
{
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
        public Vector3 _vector;

        [YamlIgnore]
        public Vector3 Vector
        {
            get
            {
                if (_vector == null)
                    _vector = new Vector3(x, y, z);
                if (_vector.x != x)
                    _vector.x = x;
                if (_vector.y != y)
                    _vector.y = y;
                if (_vector.z != z)
                    _vector.z = z;
                return _vector;
            }
        }
    }
}
