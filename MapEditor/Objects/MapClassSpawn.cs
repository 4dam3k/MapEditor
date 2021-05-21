using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditor.Models;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace MapEditor.Objects
{
    public class MapClassSpawn : IMapObject
    {
        public RoleType Class { get; set; } = RoleType.ClassD;
        public RoomType Room { get; set; } = RoomType.Unknown;
        public ObjectPosition Position { get; set; } = new ObjectPosition();
        public ObjectPosition Rotation { get; set; } = new ObjectPosition();
        public ObjectPosition Scale { get; set; } = new ObjectPosition();
        [YamlIgnore]
        public bool IsEditMode { get; set; } = false;
        [YamlIgnore]
        public MapObject LinkedMapObject { get; set; } = null;


        [YamlIgnore]
        public GameObject fakePlayer;

        public GameObject GetOrginalObject()
        {
            return null;
        }

        public void ShowEdit()
        {
            if (LinkedMapObject == null)
                return;
            if (fakePlayer != null)
                NetworkServer.Destroy(fakePlayer);
            fakePlayer = CreateFakePlayer(Class);
            fakePlayer.transform.parent = LinkedMapObject.transform;
            IsEditMode = true;
        }

        public void UnShowEdit()
        {
            if (fakePlayer != null)
            {
                NetworkServer.Destroy(fakePlayer);
                IsEditMode = false;
            }
        }

        public GameObject CreateFakePlayer( RoleType RoleType)
        {
            try
            {
                GameObject obj =
                    UnityEngine.Object.Instantiate(
                        NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
                CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
                if (ccm == null)
                    return null;
                ccm.NetworkCurClass = RoleType;
                ccm._privUserId = "retard@steam";
                ccm.GodMode = true;
                obj.GetComponent<NicknameSync>().Network_myNickSync = $"SPAWN POINT";
                var qp = obj.GetComponent<QueryProcessor>();
                var newId = QueryProcessor._idIterator++;
                qp.PlayerId = newId;
                qp.NetworkPlayerId = newId;
                qp._ipAddress = "127.0.0.WAN";
                obj.transform.position = LinkedMapObject.transform.position;
                obj.transform.eulerAngles = LinkedMapObject.transform.eulerAngles;
                NetworkServer.Spawn(obj);
                return obj;

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return null;
        }

    }
}
