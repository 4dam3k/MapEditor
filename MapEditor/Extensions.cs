using MapEditor.Models.Schematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapEditor
{
    public static class Extensions
    {
        public static Vector3 GetJsonVector(this JsonVector3 vc)
        {
            return new Vector3() { x = vc.x, y = vc.y, z = vc.z };
        }
    }
}
