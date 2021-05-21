using Assets;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Schematic : MonoBehaviour
{
    public string SchematicName;
    public string SchematicAuthor;

    public static SchematicInit init = new SchematicInitEditor();


    public void Awake()
    {
        SaveDataObjectList listOfObjectsToSave = new SaveDataObjectList();
        listOfObjectsToSave.blocks = new List<SchematicData>();
        SchematicData block;
        Transform[] objChildren = this.GetComponentsInChildren<Transform>();
        listOfObjectsToSave.topLevelInstanceID = this.GetInstanceID();
        listOfObjectsToSave.SchematicAuthor = SchematicAuthor;
        listOfObjectsToSave.SchematicName = SchematicName;
        foreach (Transform obj in objChildren)
        {
            block = new SchematicData();
            if (obj.gameObject.TryGetComponent<ObjectRef>(out ObjectRef ob))
            {
                block.ObjectType = ob.objectType;
                switch (ob.objectType)
                {
                    case ObjectType.Item:
                        block.CustomData = JsonConvert.SerializeObject(new SchematicItemData()
                        {
                            ItemID = ob.ItemID,
                            EventName = ob.eventName,
                            ExecuteEventOnPickup = ob.executeEventOnPickup
                        });
                        break;
                    case ObjectType.Collider:
                        break;
                }
            }
            else if (obj.gameObject.TryGetComponent<AnimationRef>(out AnimationRef oba))
            {
                block.ObjectType = ObjectType.Animation;
                block.CustomData = JsonConvert.SerializeObject(new SchematicAnimationData()
                {
                    rotateAnimation = oba.rotateAnimation,
                    rotateAnimationSpeed = oba.rotateAnimationSpeed
                });
            }
            else
            {
                block.ObjectType = ObjectType.Empty;
            }
            block.Name = obj.name;
            block.Position = obj.transform.position.GetJsonVector();
            block.Rotation = obj.transform.rotation.eulerAngles.GetJsonVector();
            block.Scale = obj.transform.localScale.GetJsonVector();
            block.DataID = obj.transform.GetInstanceID();
            block.ParentID = obj.transform.parent == null ? -1 : obj.transform.parent.GetInstanceID();
            listOfObjectsToSave.blocks.Add(block);
        }
        File.WriteAllText("Assets/Schematics/schematic-" + SchematicName + ".json", JsonConvert.SerializeObject(listOfObjectsToSave, Formatting.Indented));
    }
}



