using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapEditor.Models.Schematic
{
    public abstract class SchematicInit
    {
        public abstract GameObject CreateObject(SchematicData data, string schematicName, ItemType force, ref bool isNetwork);
    }
}
