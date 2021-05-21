using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Models.Schematic
{
    public class SchematicItemData
    {
        public int ItemID { get; set; }
        public bool ExecuteEventOnPickup { get; set; }
        public string EventName { get; set; }
    }
}
