using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapEditor.Models
{
    public class PlayerEditorStatus
    {
        public bool editingMap { get; set; } = false;
        public string mapName { get; set; } = "";
        public MapObject selectedObject { get; set; } = null;
    }
}
