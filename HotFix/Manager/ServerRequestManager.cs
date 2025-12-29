using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Request;
using HotFix.GameDatas.ServerData.Response;
using HotFix.Manager;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class ServerRequestManager
    {
        public void Start(string url,string token)
        {
            NetManager.instance.AddListener(NetEvent.ConnectFail, OnConnectFailed);
            NetManager.instance.AddListener(NetEvent.ConnectSucc, OnConnectSuccess);
            NetManager.instance.AddMsgListener(RequestCode.QueueUpdate.ToString(), JoinSceneFunc);
            NetManager.instance.AddMsgListener(RequestCode.JoinHall.ToString(), JoinHall);
            NetManager.instance.AddMsgListener(RequestCode.UpdateRoomInHall.ToString(), UpdateRoomInHallResponse);
            NetManager.instance.AddMsgListener(RequestCode.CreateRoomInHall.ToString(), CreateRoomInHallResponse);
            NetManager.instance.AddMsgListener(RequestCode.JoinRoom.ToString(), JoinRoomResponse);//TODO用枚举监听
            NetManager.instance.AddMsgListener(RequestCode.UpdateRoom.ToString(), UpdateRoomResponse);
            NetManager.instance.AddMsgListener(RequestCode.LeaveRoom.ToString(), LeaveRoomResponse);
            NetManager.instance.AddMsgListener(RequestCode.CloseRoom.ToString(), DissolveRoomResponse);
            NetManager.instance.AddMsgListener(RequestCode.UserReady.ToString(), UserReadyResponse);
            NetManager.instance.AddMsgListener(RequestCode.CancelReady.ToString(), UserReadyResponse);
            NetManager.instance.AddMsgListener(RequestCode.RemindReady.ToString(), RemindReadyResponse);
            NetManager.instance.AddMsgListener(RequestCode.RoomStart.ToString(), EnterRoomResponse);
             NetManager.instance.AddMsgListener(RequestCode.LoadFinish.ToString(), LoadFinishProgressResponse);
            NetManager.instance.AddMsgListener(RequestCode.GameStart.ToString(), GameStartResponse);
            NetManager.instance.AddMsgListener(RequestCode.GameCountdown.ToString(), GameCountdownResponse);
            NetManager.instance.AddMsgListener(RequestCode.GameEnd.ToString(), GameOverResponse);
            NetManager.instance.AddMsgListener(RequestCode.MandatoryExit.ToString(), MandatoryExitResponse);
            NetManager.instance.AddMsgListener(RequestCode.InGame.ToString(), GamingResponse);
            NetManager.instance.AddMsgListener(RequestCode.Move.ToString(), PlayerMoveResponse);
            NetManager.instance.AddMsgListener(RequestCode.OffLine.ToString(), PlayerOffLineResponse);
            NetManager.instance.AddMsgListener(RequestCode.Transmit.ToString(), PlayerTransmitResponse);
            NetManager.instance.AddMsgListener(RequestCode.HorseListUpdate.ToString(), RefreshHorseData);
            NetManager.instance.AddMsgListener(RequestCode.WalkHorseQuestion.ToString(), WalkHorseQuestionFunc);
            NetManager.instance.AddMsgListener(RequestCode.WalkHorseEnd.ToString(), WalkEndHorseFunc);
            WebRequestUtils.InitUrl(url,token);//http://47.96.110.234:1818/api/core/v1/
            Debug.Log("ServerRequestManager Start");
        }

        private void WalkEndHorseFunc(MsgBase msg)
        {
            Debug.Log(" WalkEndHorseFunc data服务器发送的消息:" + msg.data);
            JsonData jsonData = JsonMapper.ToObject(msg.data);
            if (GameMapManager.instance.CurrentMapName != ConStr.MAINSCENE) return;
            if (UserInfoManager.mountHorseID <= 0)
                return;
            JsonData jsondata = new JsonData();
            jsondata["id"] = UserInfoManager.mountHorseID;
            string jsonStr = JsonMapper.ToJson(jsondata);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd2, NullFunc, true, jsonStr, RFrameWork.instance.token);
            PlayerController.GetDownHorseAction();
            UserInfoManager.playerCtrl.horse.transform.ResetLocal();
            UserInfoManager.playerCtrl.horse.transform.parent.rotation = UserInfoManager.horseRotate;
            UserInfoManager.playerCtrl.horse = null;
            UserInfoManager.mountHorseID = 0;
        }

        private static void NullFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private static void NullFunc2(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                PlayerController.GetDownHorseAction();
                UserInfoManager.playerCtrl.horse.transform.ResetLocal();
                UserInfoManager.playerCtrl.horse.transform.parent.rotation = UserInfoManager.horseRotate;
                UserInfoManager.playerCtrl.horse = null;
                UserInfoManager.mountHorseID = 0;
                HouseManager.RefreshHorse();
                RFrameWork.instance.OpenCommonConfirm("提示", "您的马厩刷新了，快去看看吧～", () => { }, null);
            }
        }

        private void WalkHorseQuestionFunc(MsgBase msg)
        {
            Debug.Log(" WalkHorseQuestionFunc data服务器发送的消息:" + msg.data);
            UserInfoManager.playerCtrl.GetQuestionFunc();
        }

        private void RefreshHorseData(MsgBase msg)
        {
            Debug.Log(" RefreshHorseData data服务器发送的消息:" + msg.data);
            JsonData jsonData = JsonMapper.ToObject(msg.data);
            StartWindow.setText("------正在获取马匹信息------");
            UserInfoManager.MyHorseList.Clear();
            UserInfoManager.MyHorseList = new Dictionary<int, HorseData>();
            HorseData[] list = JsonMapper.ToObject<HorseData[]>(jsonData["data"].ToJson());
            int index = 1;
            foreach (HorseData item in list)
            {
                int id = int.Parse(item.id);
                Debug.Log(id + "   " + item.code);
                UserInfoManager.MyHorseList.Add(index, item);
                index++;
            }
            if (UserInfoManager.MyHorseList.Count > 0)
            {
                UserInfoManager.noHorse = false;
            }
            UserInfoManager.NowHorseList.Clear();
            UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
            if (GameMapManager.instance.CurrentMapName != ConStr.MAINSCENE) return;
            if (UserInfoManager.playerCtrl.mount)
            {
                JsonData jsondata = new JsonData();
                jsondata["id"] = UserInfoManager.mountHorseID;
                string jsonStr = JsonMapper.ToJson(jsondata);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd, NullFunc2, true, jsonStr, RFrameWork.instance.token);
            }
            else
                HouseManager.RefreshHorse();
        }

        private void PlayerOffLineResponse(MsgBase msg)
        {
            Debug.Log(" PlayerOffLineResponse data服务器发送的消息:" + msg.data);
            string jsonData = msg.data;
            JsonData temp = JsonMapper.ToObject(jsonData)["data"];
            EcsManager.OffLinePlayer(temp["userId"].ToString());
        }

        private void PlayerTransmitResponse(MsgBase msg)
        {
            Debug.Log(" PlayerTransmitResponse data服务器发送的消息:" + msg.data);
        }
        
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="msg"></param>
        private void PlayerMoveResponse(MsgBase msg)
        {
            Debug.Log(" PlayerMoveResponse data服务器发送的消息:" + msg.data);
            string jsonData = msg.data;
            JsonData temp = JsonMapper.ToObject(jsonData)["data"];
            EcsManager.PlayerMove(JsonMapper.ToObject<PlayerMoveData>(temp.ToJson()));
        }

        private void GamingResponse(MsgBase msg)
        {
            Debug.Log(" GamingResponse data服务器发送的消息:" + msg.data);
            string data = msg.data;
            PlayWindow.GamingResponseAction(data);

        }
        private void OnConnectFailed(string str)
        {
            Debug.Log("ServerRequestManager OnConnectFailed:");
            NetManager.instance.OnStartReconnect();

        }
        private void OnConnectSuccess(string str)
        {
            Debug.Log("OnConnectSuccess 连接服务器成功："+str);
            if (UIManager.instance.CommonConfirm)
            {
                Debug.Log("OnConnectSuccess 连接服务器成功：关闭提示弹窗");
                UIManager.instance.CommonConfirm.GetComponent<CommonConfirm>().CloseSelf();
            }
            Debug.Log("OnConnectSuccess 连接服务器成功：" + GameMapManager.instance.CurrentMapName);
            if (string.IsNullOrEmpty(GameMapManager.instance.CurrentMapName))
                return;
            Debug.Log("ServerRequestManager OnConnectSuccess");
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.OutLineConnectAgain, ConnectFunc, true, "{}", RFrameWork.instance.token);
        }

        private void ConnectFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string status = jsonData["data"]["status"].ToString();
                if (status.Equals("1"))
                {
                    if (GameMapManager.instance.CurrentMapName.Equals("Main"))
                    {
                        Debug.Log("OnConnectSuccess 连接服务器成功 JoinScene 1");
                        NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));

                    }
                    if (GameMapManager.instance.CurrentMapName.Equals("Game"))
                    {
                        Debug.Log("ServerRequestManager OnConnectSuccess PlayGameNow 1");
                        NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
                    }

                }
                else if (status.Equals("2"))
                {
                    if (GameMapManager.instance.CurrentMapName.Equals("Main"))
                    {
                        Debug.Log("OnConnectSuccess 连接服务器成功 JoinScene 2");
                        NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));

                    }
                    if (GameMapManager.instance.CurrentMapName.Equals("Game"))
                    {
                        Debug.Log("ServerRequestManager OnConnectSuccess PlayGameNow 2");
                        NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
                    }
                }
                else
                {
                    if (GameMapManager.instance.CurrentMapName.Equals("Main"))
                    {
                        Debug.Log("OnConnectSuccess 连接服务器成功 JoinScene 0");
                        NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));

                    }
                    if (GameMapManager.instance.CurrentMapName.Equals("Game"))
                    {
                        Debug.Log("ServerRequestManager OnConnectSuccess PlayGameNow  0");
                        GameMapManager.instance.LoadScene(ConStr.MAINSCENE, FilesName.MAINPANEL, HouseManager.LoadMainScene);
                    }
                }
                //    if (GameMapManager.instance.CurrentMapName.Equals("Main"))
                //{
                //    Debug.Log("OnConnectSuccess 连接服务器成功 JoinScene");
                //    NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));

                //}
                //if (GameMapManager.instance.CurrentMapName.Equals("Game"))
                //{
                //    Debug.Log("ServerRequestManager OnConnectSuccess PlayGameNow");
                //    NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
                //}
                //Debug.Log("ServerRequestManager OnConnectSuccess");
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void JoinSceneFunc(MsgBase msg)
        {
            Debug.Log("刷新繁殖场咯:"+UserInfoManager.enterGame);
            string jsonData = msg.data;
            JsonData temp = JsonMapper.ToObject(jsonData)["data"];
            List<BreedSiteList> list = new List<BreedSiteList>();
            foreach (JsonData item in temp["breedSiteListResList"])
            {
                list.Add(JsonMapper.ToObject<BreedSiteList>(item.ToJson()));
            }
            UserInfoManager.MyBreedSiteData = new BreedSiteData("0",
                int.Parse(temp["queueNumber"].ToString()),
                temp["price"].ToString(),
                int.Parse(temp["time"].ToString()),
                list);
            if (GameMapManager.instance.CurrentMapName!=ConStr.MAINSCENE) return;
            //UserInfoManager.playerCtrl.name.text = temp["userName"].ToString().Length > 5 ? temp["userName"].ToString().Substring(0, 5) + "..." : temp["userName"].ToString(); //playerMoveData.userName; temp["userName"].ToString().Substring(0,7)+"...";
            Debug.Log("通过socket刷新繁殖场");
            HouseManager.RefreshBreedData();
        }

        private void CreateRoomInHallResponse(MsgBase msg)
        {
            if(UIManager.instance.GetWndByName(FilesName.GAMELISTPANEL)!=null)
            GameListWindow.addItemToList();
        }

        private void UpdateRoomInHallResponse(MsgBase msg)
        {
            if (UIManager.instance.GetWndByName(FilesName.GAMELISTPANEL) != null)
                GameListWindow.addItemToList();
        }
        
       
        private void JoinRoomResponse(MsgBase msg)
        {
            Debug.Log("JoinRoomResponse   joinroomresponse 111");
            if (string.IsNullOrEmpty(GameMapManager.instance.CurrentMapName))
                return;
            if (GameMapManager.instance.CurrentMapName.Equals("Main"))
            {
                Debug.Log("OnConnectSuccess 连接服务器成功 JoinRoom");
                UserInfoManager.enterGame = true;
                //NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd, WebRequestFuncitons.NullFunc, true, JsonMapper.ToJson(new HorseIdData(UserInfoManager.mountHorseID)), RFrameWork.instance.token);
                GameMapManager.instance.LoadGameScene(ConStr.GAMESCENE, FilesName.PLAYPANEL, (Action obj) =>
                {
                    ObjectManager.instance.InstantiateObjectAsync("Assets/GameData/Prefabs/Building/Racecourse/Terrain2.prefab", (path, go/*GameObject*/, param1, param2, param3) =>
                    {
                        // GameMapManager.instance.LoadScene();

                        Debug.Log("StartLoadTerrain finish" + go.name);
                        GameObject instantiateGo = go as GameObject;
                        instantiateGo.SetActive(true);
                        instantiateGo.transform.localPosition = new Vector3(-142f, -18.6f, 0);
                        GameObject horseClone = ObjectManager.instance.InstantiateObject("Assets/GameData/Prefabs/Animals/HorseLow.prefab", false, true);
                        horseClone.name = "HorseClone";
                        horseClone.gameObject.SetActive(false);
                        UserInfoManager.horseClone = horseClone;
                        UIManager.instance.CloseWnd(FilesName.TOPREPAREPANEL);
                        UIManager.instance.PopUpWnd(FilesName.ROOMPANEL, true, false);
                        Debug.Log("ServerRequestManager OnConnectSuccess11111111111");
                        JsonData jsonData = JsonMapper.ToObject(msg.data);
                        UserInfoManager.RoomId = int.Parse(jsonData["data"]["id"].ToString());
                        Debug.Log(" JoinRoomResponse data服务器发送的消息:" + msg.data + "  roomId:" + UserInfoManager.RoomId);
                        string data = msg.data;
                        Debug.Log("加入房间的action是否为空" + RoomWindow.JoinRoomResponseAction);
                        RoomWindow.JoinRoomResponseAction(data);
                        EcsManager.ClearAllPlayer();

                    }, LoadResPriority.RES_HIGHT, false, null, null, null, true);
                });
                UserInfoManager.CloseMainScenesObjectScript(); //离开主场景调用
            }
            else if (GameMapManager.instance.CurrentMapName.Equals("Game"))
            {
                Debug.Log("ServerRequestManager OnConnectSuccess");
                JsonData jsonData = JsonMapper.ToObject(msg.data);
                UserInfoManager.RoomId = int.Parse(jsonData["data"]["id"].ToString());
                Debug.Log(" JoinRoomResponse data服务器发送的消息:" + msg.data + "  roomId:" + UserInfoManager.RoomId);
                string data = msg.data;
                Debug.Log("加入房间的action是否为空" + RoomWindow.JoinRoomResponseAction);
                RoomWindow.JoinRoomResponseAction(data);
            }

        }
        private void UpdateRoomResponse(MsgBase msg)
        {
            Debug.Log(" UpdateRoomResponse data服务器发送的消息:" + msg.data);
            string data = msg.data;
            RoomWindow.UpdateRoomResponseAction(data);
        }
        private void LeaveRoomResponse(MsgBase msg)
        {
            Debug.Log(" LeaveRoomResponse data服务器发送的消息:" + msg.data);
            string data = msg.data;
            PlayWindow.LeaveUpdateRoomResponseAction(data);

        }
        private void DissolveRoomResponse(MsgBase msg)
        {
            Debug.Log(" DissolveRoomResponse data服务器发送的消息:" + msg.data);
            string data = msg.data;
            RoomWindow.DissolveRoomResponseAction(data);

        }

        private void UserReadyResponse(MsgBase msg)
        {
            string data = msg.data;
            Debug.Log(" UserReadyResponse data服务器发送的消息:" + data);
            RoomWindow.ReadyResponseAction(data);

        }
        private void RemindReadyResponse(MsgBase msg)
        {
            string data = msg.data;
            Debug.Log(" RemindReadyResponse data服务器发送的消息:" + data);
            RoomWindow.RemindReadyResponseAction(data);

        }
        private void EnterRoomResponse(MsgBase msg)
        {
            string data = msg.data;
            Debug.Log(" EnterRoomResponse data服务器发送的消息:" + data);
            PlayWindow.EnterRoomResponseAction(data);

        }
        private void LoadFinishProgressResponse(MsgBase msg)
        {
            Debug.Log(" LoadFinishProgress data服务器发送的消息:" + msg.data);
            string data = msg.data;
            ToPrepareWindow.LoadFinishProgressAction(data);

        }
        private void GameStartResponse(MsgBase msg)
        {
            Debug.Log(" GameStartResponse data服务器发送的消息:" + msg.data);
            string data = msg.data;
            PlayWindow.GameStartResponseAction(data);

        }
        private void GameCountdownResponse(MsgBase msg)
        {
            Debug.Log(" GameCountdownResponse data服务器发送的消息:" + msg.data);
            string data = msg.data;
            PlayWindow.GameCountdownResponseAction(data);
        }
        private void GameOverResponse(MsgBase msg)
        {
            Debug.Log(" GameOverResponse data服务器发送的消息:" + msg.data);
            string data = msg.data;
            JsonData jsonData = JsonMapper.ToObject(data);
            JsonData roomData = jsonData["data"]["matchEndListResList"];
            string roomId = jsonData["data"]["roomNumber"].ToString();
            string horseId = jsonData["data"]["horseId"].ToString();
            string gameStartDatetime = jsonData["data"]["gameStartDatetime"].ToString();
            int rank = int.Parse(jsonData["data"]["rank"].ToString());
            string startTime = TimeUtils.MilSecondsTimestampToTime(gameStartDatetime);
            ResultRankData[] datas = new ResultRankData[roomData.Count];
            for (int i = 0; i < roomData.Count; i++)
            {
                ResultRankData horseData = JsonMapper.ToObject<ResultRankData>(roomData[i].ToJson());
                datas[i] = horseData;
                Debug.Log("马排名的信息：" + horseData.rank);
            }
            if(PlayWindow.GameOverAction!=null)
                PlayWindow.GameOverAction();
            UIManager.instance.CloseWnd(FilesName.PLAYPANEL);
            UIManager.instance.PopUpWnd(FilesName.SETTLEMENTPANEL, true, false,datas);
            SettlementWindow.SetPanelData(roomId, horseId, startTime, rank, rank!=0?((int)float.Parse(datas[rank - 1].rewardAmount)).ToString():"0",1);

        }
        private void MandatoryExitResponse(MsgBase msg)
        {
            Debug.Log(" GameOverResponse data服务器发送的消息:" + msg.data);
            string data = msg.data;
            RoomWindow.MandatoryExitResponseAction();
        }
        
        public static Dictionary<int, RoomDatas> roomDatas = new Dictionary<int, RoomDatas>();

        private void JoinHall(MsgBase msg)  
        {

        }
    }
}
