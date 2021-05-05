using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor
{
    public class MapEditorConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
     }
}
