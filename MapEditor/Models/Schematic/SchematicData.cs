using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Models.Schematic
{
    public class SchematicData
    {
        public int DataID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public JsonVector3 Position { get; set; }
        public JsonVector3 Rotation { get; set; }
        public JsonVector3 Scale { get; set; }
        public ObjectType ObjectType { get; set; }
        public string CustomData { get; set; }
    }
}
