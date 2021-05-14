using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
d
namespace MapEditor
{
    public class Map
    {
        public string MapName { get; set; } = "None";
        public List<ushort> LoadOnStart { get; set; } = new List<ushort>();
        public List<MapObject> objects { get; set; } =
        new List<MapObject>();
    }
}
