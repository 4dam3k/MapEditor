using Exiled.API.Features;
using MapEditor.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace MapEditor
{
    public class Map
    {
        public string MapName { get; set; } = "None";
        public List<ushort> LoadOnStart { get; set; } = new List<ushort>();
        public Dictionary<int, MapClassSpawn> CustomSpawnpoints { get; set; } = new Dictionary<int, MapClassSpawn>();
        public Dictionary<int, MapDoor> CustomDoors { get; set; } = new Dictionary<int, MapDoor>();
        public Dictionary<int, MapItemSpawn> CustomItemSpawns { get; set; } = new Dictionary<int, MapItemSpawn>();
        public Dictionary<int, MapWorkstation> CustomWorkstations { get; set; } = new Dictionary<int, MapWorkstation>();
        public Dictionary<int, MapCustomObject> CustomObjects { get; set; } = new Dictionary<int, MapCustomObject>();

        public void Load()
        {
            foreach(var csp in CustomSpawnpoints)
            {
                var gm = new GameObject("CSP");
                var mp = gm.AddComponent<MapObject>();
                mp.Init(this, csp.Key, csp.Value, csp.Value.Position.Vector, csp.Value.Scale.Vector, Quaternion.Euler(csp.Value.Rotation.Vector));
            }
            foreach (var csp in CustomDoors)
            {
                var gm = new GameObject("CD");
                var mp = gm.AddComponent<MapObject>();
                mp.Init(this, csp.Key, csp.Value, csp.Value.Position.Vector, csp.Value.Scale.Vector, Quaternion.Euler(csp.Value.Rotation.Vector));
            }
            foreach (var csp in CustomItemSpawns)
            {
                var gm = new GameObject("CIP");
                var mp = gm.AddComponent<MapObject>();
                mp.Init(this, csp.Key, csp.Value, csp.Value.Position.Vector, csp.Value.Scale.Vector, Quaternion.Euler(csp.Value.Rotation.Vector));
            }
            foreach (var csp in CustomWorkstations)
            {
                var gm = new GameObject("CW");
                var mp = gm.AddComponent<MapObject>();
                mp.Init(this, csp.Key, csp.Value, csp.Value.Position.Vector, csp.Value.Scale.Vector, Quaternion.Euler(csp.Value.Rotation.Vector));
            }
            foreach (var csp in CustomObjects)
            {
                var gm = new GameObject("CO");
                var mp = gm.AddComponent<MapObject>();
                mp.Init(this, csp.Key, csp.Value, csp.Value.Position.Vector, csp.Value.Scale.Vector, Quaternion.Euler(csp.Value.Rotation.Vector));
            }
            Log.Info($"Map {MapName} loaded.");
        }

        public void Unload()
        {
            foreach(var obj in UnityEngine.Object.FindObjectsOfType<MapObject>())
            {
                if (obj.Map == null)
                    continue;
                if (obj.Map.MapName == MapName)
                    UnityEngine.Object.Destroy(obj);
            }
            Log.Info($"Map {MapName} unloaded.");
        }
    }
}
