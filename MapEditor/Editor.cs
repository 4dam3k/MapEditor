using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using MapEditor.Models;
using MapEditor.Models.Schematic;
using MapEditor.Objects;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MapEditor
{
    public class Editor
    {
        public static Dictionary<string, Map> maps = new Dictionary<string, Map>();
        public static Dictionary<string, PlayerEditorStatus> playerEditors = new Dictionary<string, PlayerEditorStatus>();
    }
}
