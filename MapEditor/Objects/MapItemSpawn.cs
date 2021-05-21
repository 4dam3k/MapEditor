using Exiled.API.Enums;
using MapEditor.Models;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace MapEditor.Objects
{
    public class MapItemSpawn : IMapObject
    {
        public ItemType Item { get; set; } = ItemType.Coin;
        public RoomType Room { get; set; } = RoomType.Unknown;
        public ObjectPosition Position { get; set; } = new ObjectPosition();
        public ObjectPosition Rotation { get; set; } = new ObjectPosition();
        public ObjectPosition Scale { get; set; } = new ObjectPosition();
        [YamlIgnore]
        public bool IsEditMode { get; set; } = false;
        [YamlIgnore]
        public MapObject LinkedMapObject { get; set; } = null;
        public GameObject GetOrginalObject()
        {
            return UnityEngine.Object.Instantiate<GameObject>(ReferenceHub.HostHub.inventory.pickupPrefab);
        }

        [YamlIgnore]
        public GameObject fakeItem;

        public void ShowEdit()
        {
            if (LinkedMapObject == null)
                return;
            if (fakeItem != null)
                NetworkServer.Destroy(fakeItem);
            fakeItem = GetOrginalObject();
            fakeItem.transform.parent = LinkedMapObject.transform;
            fakeItem.GetComponent<Rigidbody>().isKinematic = true;
            fakeItem.AddComponent<DisablePickupInteract>();
            fakeItem.GetComponent<Pickup>().SetupPickup(Item, -1f, ReferenceHub.HostHub.gameObject, new Pickup.WeaponModifiers(false, 0, 0, 0), LinkedMapObject.transform.position, LinkedMapObject.transform.rotation);
            NetworkServer.Spawn(fakeItem);
            IsEditMode = true;
        }

        public void UnShowEdit()
        {
            if (fakeItem != null)
            {
                NetworkServer.Destroy(fakeItem);
                IsEditMode = false;
            }
        }
    }
}
