using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class SaveDataObjectList
    {
        public string SchematicName;
        public string SchematicAuthor;
        public int topLevelInstanceID;
        public List<SchematicData> blocks;
        public SchematicData BlockWithinstanceID(int id)
        {
            SchematicData block = blocks.Find(c => c.DataID == id);
            return block;
        }

        public List<SchematicData> BlocksWithParentID(int id)
        {
            List<SchematicData> newList = blocks.FindAll(c => c.ParentID == id);
            return newList;
        }
    }

    public class SchematicItemData
    {
        public int ItemID { get; set; }
        public bool ExecuteEventOnPickup { get; set; }
        public string EventName { get; set; }
    }

    public class SchematicAnimationData
    {
        public bool rotateAnimation { get; set; } = false;
        public float rotateAnimationSpeed { get; set; } = 3f;
        public bool blinkAnimation { get; set; } = false;
        public float blinkAnimationSpeed { get; set; } = 3f;
    }


    public abstract class SchematicInit
    {
        public abstract GameObject CreateObject(SchematicData data);
    }


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

    public enum ObjectType
    {
        Workstation,
        Door,
        Ragdoll,
        Item,
        Animation,
        Empty,
        Collider,
        InitScheamtic
    }

    public class JsonVector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}
