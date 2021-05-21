using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ObjectRef;

namespace Assets
{
    public static class Extensions
    {
        public static JsonVector3 GetJsonVector(this Vector3 vc)
        {
            return new JsonVector3() { x = vc.x, y = vc.y, z = vc.z };
        }
        public static string GetPath(this Component component)
        {
            return string.Join("/", component.gameObject.GetComponentsInParent<Transform>().Select(t => t.gameObject.name).Reverse().ToArray(), component.gameObject);
        }
        public static Vector3 GetJsonVector(this JsonVector3 vc)
        {
            return new Vector3() { x = vc.x, y = vc.y, z = vc.z };
        }
    }
}
