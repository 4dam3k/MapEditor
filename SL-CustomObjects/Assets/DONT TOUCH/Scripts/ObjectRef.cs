using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRef : MonoBehaviour
{
    public ObjectType objectType;
    public int ItemID;
    public int ClassID;
    public bool executeEventOnPickup = false;
    public string eventName = "onPickup1";
}
