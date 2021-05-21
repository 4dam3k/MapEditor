using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapEditor
{
    public class AnimationRotate : MonoBehaviour
    {
        public float speed = 3f;
        public void Update()
        {
            this.transform.Rotate(Vector3.left, Time.deltaTime * speed);
        }
    }
}
