using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Models.Schematic
{
    public class SchematicAnimationData
    {
        public bool rotateAnimation { get; set; } = false;
        public float rotateAnimationSpeed { get; set; } = 3f;
        public bool blinkAnimation { get; set; } = false;
        public float blinkAnimationSpeed { get; set; } = 3f;
    }
}
