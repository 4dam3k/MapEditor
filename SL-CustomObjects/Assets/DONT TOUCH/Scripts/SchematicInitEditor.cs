using Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SchematicInitEditor : SchematicInit
{
    public override GameObject CreateObject(SchematicData data)
    {
        switch (data.ObjectType)
        {
            case ObjectType.Item:
                var dat = JsonConvert.DeserializeObject<SchematicItemData>(data.CustomData);
                GameObject pkp = null;
                foreach (var ob in UnityEngine.Object.FindObjectsOfType<ObjectRef>())
                {
                    if (ob.ItemID == dat.ItemID)
                    {
                        pkp = ob.gameObject;
                        break;
                    }
                }

                return UnityEngine.Object.Instantiate(pkp);
            case ObjectType.Collider:
                var gmb = new GameObject(data.Name);
                gmb.AddComponent<BoxCollider>();
                return gmb;
            default:
                return new GameObject(data.Name);
        }
    }
}

