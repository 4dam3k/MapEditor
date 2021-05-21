using Assets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Schematic;

public class SchematicLoad : MonoBehaviour
{
    public string schematicPath;

    private void Awake()
    {
        Load();
    }

    public void Load()
    {
        SaveDataObjectList blocks = (SaveDataObjectList)JsonConvert.DeserializeObject<SaveDataObjectList>(File.ReadAllText(schematicPath)); //
        GameObject ob = new GameObject(blocks.SchematicName);
        CreateRecursiveFromID(blocks.blocks[0].DataID, blocks, ob.transform); // neeed to do the parent as GetComponentsInChildren also saves the game object at that level.
    }

    public void CreateRecursiveFromID(int id, SaveDataObjectList blocks, Transform parentGameObject)
    {
        Transform childGameObjectTransform = CreateGameObjectFromGameObjectSaveData(blocks.BlockWithinstanceID(id), parentGameObject); // Create the object first before creating children.
        foreach (var block in blocks.BlocksWithParentID(id))
        {
            CreateRecursiveFromID(block.DataID, blocks, childGameObjectTransform); // The child now becomes the parent
        }
    }
    public Transform CreateGameObjectFromGameObjectSaveData(SchematicData block, Transform parentGameObject)
    {
        GameObject newGameObject = init.CreateObject(block);
        newGameObject.transform.parent = parentGameObject;
        newGameObject.transform.position = block.Position.GetJsonVector();
        newGameObject.transform.rotation = Quaternion.Euler(block.Rotation.GetJsonVector());
        newGameObject.transform.localScale = block.Scale.GetJsonVector();

        newGameObject.name = block.Name;
        return newGameObject.transform;
    }
}
