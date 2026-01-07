using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace HotFix.Manager
{
    public class PlayerEntity
    {
    }
    internal class EcsManager
    {
        // 实体字典
        private static Dictionary<string, OtherPlayerController> entities = new Dictionary<string, OtherPlayerController>();

        public static void PlayerMove(PlayerMoveData playerMoveData)
        {
            if (GameMapManager.instance.CurrentMapName != ConStr.MAINSCENE)
                return;
            if (!entities.ContainsKey(playerMoveData.userId))
            {
                if (UserInfoManager.playerCtrl != null && UserInfoManager.playerCtrl.m_GameObject != null)
                {
                    string playerName = "RiderGirl";
                    if(playerMoveData.roleType == "1")
                        playerName = "RiderBoy";
                    GameObject player = ObjectManager.instance.InstantiateObject("Assets/GameData/Prefabs/Player/" + playerName + ".prefab", false, true);
                    OtherPlayerController otherPlayerController = ObjectsManager.instance.AddObject(player, "player"+playerMoveData.userId, "OtherPlayerController",playerMoveData) as OtherPlayerController;
                    otherPlayerController.sex = playerMoveData.roleType;
                    entities[playerMoveData.userId] = otherPlayerController;
                }
            }
            else
            {
                if(playerMoveData.posX!="0"|| playerMoveData.posY != "0" || playerMoveData.posZ != "0")
                {
                    entities[playerMoveData.userId].Move(new Vector3(float.Parse(playerMoveData.posX), float.Parse(playerMoveData.posY), float.Parse(playerMoveData.posZ)));
                }
                else
                    entities[playerMoveData.userId].Stand();
                entities[playerMoveData.userId].name.text = playerMoveData.userName.Length > 5 ? playerMoveData.userName.Substring(0, 5) + "..." : playerMoveData.userName;

                entities[playerMoveData.userId].SetPosition(new Vector3(float.Parse(playerMoveData.nowPosX), float.Parse(playerMoveData.nowPosY), float.Parse(playerMoveData.nowPosZ)),playerMoveData.rotateY);
                entities[playerMoveData.userId].RunOrWalk(playerMoveData.type == "1");
                entities[playerMoveData.userId].SetSex(playerMoveData.roleType);
                if (string.IsNullOrEmpty(playerMoveData.horseCode))
                {
                    entities[playerMoveData.userId].GetDownHorse();
                }
                else
                {
                    entities[playerMoveData.userId].MountHorseFunc(playerMoveData.horseCode);
                }
            }
        }

        internal static void ClearAllPlayer()
        {
            foreach(var item in entities)
            {
                if (entities.ContainsKey(item.Key))
                {
                    Debug.Log("Remove this player" + item.Key);
                    entities[item.Key].GetDownHorse();
                    ObjectManager.instance.RealaseObject(entities[item.Key].m_GameObject);
                    ObjectsManager.instance.OnRemove(entities[item.Key]);
                    entities[item.Key] = null;
                    entities.Remove(item.Key);
                }
            }
        }

        internal static void OffLinePlayer(string userId)
        {
            if (GameMapManager.instance.CurrentMapName != ConStr.MAINSCENE)
                return;
            if (entities.ContainsKey(userId))
            {
                Debug.Log("Remove this player" + userId);
                entities[userId].GetDownHorse();
                ObjectManager.instance.RealaseObject(entities[userId].m_GameObject);
                ObjectsManager.instance.OnRemove(entities[userId]);
                entities[userId] = null;
                entities.Remove(userId);
            }
        }
    }
}
