using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Request;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

namespace HotFix
{
    internal class RoomWindowOld : Window
    {
        private Button startBtn, hintBtn, readyBtn, cancelBtn, backBtn, selectBtn;

        private Text horseFarmName;
        private Dictionary<string, PlayerItem> playerItemDic;
        private Transform[] playerItems;
        public static Action<string> JoinRoomResponseAction;
        public static Action<string> UpdateRoomResponseAction;
        public static Action<string> LeaveUpdateRoomResponseAction;
        public static Action<string> DissolveRoomResponseAction;
        public static Action<string> ReadyResponseAction;
        public static Action<string> RemindReadyResponseAction;
        public static Action<string> EnterRoomResponseAction;
        

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            RFrameWork.instance.SetBackAudio("SingleSounds/RoomWait");
            Debug.Log("RoomWindow Awake:" + m_GameObject.name);
            JoinRoomResponseAction = JoinRoomRequestResponse;
            UpdateRoomResponseAction = UpdateRoomRequestResponse;
            LeaveUpdateRoomResponseAction = LeaveUpdateRoomResponse;
            DissolveRoomResponseAction = DissolveRoomResponse;
            ReadyResponseAction = ReadyRequestResponse;
            RemindReadyResponseAction = RemindReadyResponse;
            EnterRoomResponseAction = EnterRoomResponse;
            playerItemDic = new Dictionary<string, PlayerItem>();
            GetAllComponents();
            HideAllBtn();
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
          /*  JoinRoom data = new JoinRoom(UserInfoManager.RoomId, UserInfoManager.userID);
            string jsonStr = JsonMapper.ToJson(data);
            Debug.Log("JoinRoom OnShow json:" + jsonStr);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.joinRoomUrl, WebRequestResponse, true, jsonStr, RFrameWork.instance.token);

            UIManager.instance.CloseWnd(FilesName.BAGORSELECTPANEL);*/
        }
        private void GetAllComponents()
        {
            backBtn = m_GameObject.transform.Find("BackBtn").GetComponent<Button>();
            startBtn = m_GameObject.transform.Find("BottomBtns/StartBtn").GetComponent<Button>();
            hintBtn = m_GameObject.transform.Find("BottomBtns/HintBtn").GetComponent<Button>();
            readyBtn = m_GameObject.transform.Find("BottomBtns/ReadyBtn").GetComponent<Button>();
            cancelBtn = m_GameObject.transform.Find("BottomBtns/CancelReadyBtn").GetComponent<Button>();
            selectBtn = m_GameObject.transform.Find("SelectHorseBtn").GetComponent<Button>();
            horseFarmName = m_GameObject.transform.Find("Title/Text").GetComponent<Text>();
            Transform contents = m_GameObject.transform.Find("PlayerList/Viewport/Content");
            playerItems = new Transform[contents.childCount];
            for (int i = 0; i < contents.childCount; i++)
            {
                playerItems[i] = contents.GetChild(i);
                contents.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            AddButtonClickListener(startBtn, StartGameClicked);
            AddButtonClickListener(hintBtn, HintReadyClicked);
            AddButtonClickListener(readyBtn, () => { ReadyStateClicked(true); });
            AddButtonClickListener(cancelBtn, () => { ReadyStateClicked(false); });
            AddButtonClickListener(backBtn, BackBtnClicked);
            AddButtonClickListener(selectBtn, SelectHorseClicked);

        }

        private void SelectHorseClicked()
        {
            NumberOfPages data = new NumberOfPages(1, 30);
            string jsonStr = JsonMapper.ToJson(data);
            UserInfoManager.selectOrSelectAndCreate = false;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseOwnerUrl, WebRequestFuncitons.GetHorseData, true, jsonStr, RFrameWork.instance.token);
        }
        #region 接收服务端消息回调

        private void WebRequestResponse(string jsonInfo)
        {
            Debug.Log("接收到的消息是：" + jsonInfo);
        }
        private void JoinRoomRequestResponse(string data)
        {
            Debug.Log("RoomWindow JoinRoomRequestResponse 接收到的消息是：" + data);
            ClearAllData();
            JsonData jsonData = JsonMapper.ToObject(data);
            horseFarmName.text = jsonData["data"]["name"].ToString();
            JsonData roomData = jsonData["data"]["roomUserResList"];
            PlayerInfoData[] datas = new PlayerInfoData[roomData.Count];
            for (int i = 0; i < roomData.Count; i++)
            {
                Debug.Log(roomData[i].ToJson());
                PlayerInfoData playerInfoData = JsonMapper.ToObject<PlayerInfoData>(roomData[i].ToJson());
                UpdateUI(playerInfoData);
                datas[i] = playerInfoData;

            }
            EnterRoom(datas);
        }
        private void UpdateRoomRequestResponse(string data)
        {
            PlayerInfoData playerInfoData = JsonMapper.ToObject<PlayerInfoData>(data);
            UpdateRoom(playerInfoData);
        }
        private void LeaveUpdateRoomResponse(string data)
        {
            JsonData jsonData = JsonMapper.ToObject(data);
            string userId= jsonData["data"]["userId"].ToString();
            LeaveRoom(userId);
        }
        private void DissolveRoomResponse(string data)
        {
            JsonData jsonData = JsonMapper.ToObject(data);
            PlayerInfoData playerInfoData = JsonMapper.ToObject<PlayerInfoData>(jsonData["data"].ToJson());
            string toast = jsonData["data"]["remark"].ToString();
            string flag = jsonData["data"]["homeOwnerFlag"].ToString();
            DissolveRoom(flag,toast);
        }
        
        private void ReadyRequestResponse(string data)
        {
            Debug.Log("RoomWindow ReadyRequestResponse 接收到的消息是：" + data);
            JsonData jsonData = JsonMapper.ToObject(data);          
            PlayerInfoData playerInfoData = JsonMapper.ToObject<PlayerInfoData>(jsonData["data"].ToJson());
            UpdateRoom(playerInfoData);

        }
        private void RemindReadyResponse(string data)
        {
            Debug.Log("RoomWindow RemindReadyResponse 接收到的消息是：" + data);
            JsonData jsonData = JsonMapper.ToObject(data);
            string toast = jsonData["data"]["remark"].ToString();
            string houseOwnerId= jsonData["data"]["userId"].ToString();
            RFrameWork.instance.OpenCommonConfirm("提示", toast, ConfirmClicked, null);
            /*if (!UserInfoManager.UserId.Equals(houseOwnerId))
            {
                PlayerItem mineItem = null;
                playerItemDic.TryGetValue(UserInfoManager.UserId,out mineItem);
                if(mineItem!=null)
                {
                    if(mineItem.data.readyFlag.Equals("0"))
                    {
                        RFrameWork.instance.OpenCommonConfirm("提示", toast, ConfirmClicked, null);
                    }
                }
                
            }*/

        }
        private void EnterRoomResponse(string data)
        {
            Debug.Log("RoomWindow EnterRoomResponse 接收到的消息是：" + data+" 长度："+ playerItemDic.Count);
            JsonData jsonData = JsonMapper.ToObject(data);
            JsonData roomData = jsonData["data"]["startListResList"];
            RankInfoData[] rankDatas = new RankInfoData[roomData.Count];
            for (int i = 0; i < roomData.Count; i++)
            {
                Debug.Log(roomData[i].ToJson());
                RankInfoData infoData = JsonMapper.ToObject<RankInfoData>(roomData[i].ToJson());              
                rankDatas[i] = infoData;
            }
            #region 保存此页面本玩家的数据传送个准备页面
            PlayerInfoData[] playerInfoDatas=new PlayerInfoData[playerItemDic.Count];
            PlayerItem[] items= playerItemDic.Values.ToArray<PlayerItem>();
            Debug.Log("RoomWindow items length：" + items.Length);
            for (int i=0;i< items.Length;i++)
            {
                playerInfoDatas[i] = items[i].data;
                Debug.Log("传送的玩家ID:" + playerInfoDatas[i].nickname);
            }
            if(playerInfoDatas==null)
            {
                Debug.LogError("playerinfodatas is null");
                return;
            }
            Debug.Log("RoomWindow items length：" + playerInfoDatas.Length);
            object[] params2 = new object[2];
            params2[0] = playerInfoDatas;
            params2[1] = rankDatas;
            Debug.Log("RoomWindow传送给ToPrepareWindow 玩家的信息："+ rankDatas.Length);
            #endregion
            GameMapManager.instance.LoadGameScene(ConStr.GAMESCENE, FilesName.PLAYPANEL, StartLoadTerrain, params2);
        }

        private void StartLoadTerrain(Action action)
        {
            ObjectManager.instance.InstantiateObjectAsync("Assets/GameData/Prefabs/Building/Racecourse/Terrain.prefab", (path, go/*GameObject*/, param1, param2, param3) =>
            {
                // GameMapManager.instance.LoadScene();

                Debug.Log("StartLoadTerrain finish" + go.name);
                NetManager.instance.Send(new MsgBase(RequestCode.LoadFinish.ToString(), JsonUtility.ToJson(new JoinScene(UserInfoManager.RoomId.ToString(), UserInfoManager.userID))));
                GameObject instantiateGo = go as GameObject;
                instantiateGo.SetActive(true);
                instantiateGo.transform.localPosition = Vector3.zero;
                 GameObject horseClone = ObjectManager.instance.InstantiateObject("Assets/GameData/Prefabs/Animals/Horse.prefab", false, true);
                horseClone.name = "HorseClone";
                 horseClone.gameObject.SetActive(false);
                 UserInfoManager.horseClone = horseClone;
                 
                //action?.Invoke();

            }, LoadResPriority.RES_HIGHT, false, null, null, null, true);
        }
        #endregion
        private void ConfirmClicked()
        {
            Debug.Log("ConfirmClicked");
        }
        
        private void HideAllBtn()
        {
            startBtn.gameObject.SetActive(false);
            hintBtn.gameObject.SetActive(false);
            cancelBtn.gameObject.SetActive(false);
            readyBtn.gameObject.SetActive(false);
        }
        private void HideAllItem()
        {
            for (int i = 0; i < playerItems.Length; i++)
            {
                playerItems[i].GetChild(0).gameObject.SetActive(false);
                
            }
        }
    

        private void EnterRoom(PlayerInfoData[] infoDatas)
        {
            for (int i = 0; i < infoDatas.Length; i++)
            {
                PlayerItem item = new PlayerItem();
                item.Init(infoDatas[i], playerItems[i].transform);
                playerItemDic.Add(infoDatas[i].userId.ToString(), item);
            }

        }
        public void UpdateRoom(PlayerInfoData data)
        {
            PlayerItem item = null;
            Debug.Log("进入游戏的玩家id:"+ data.userId+" 昵称："+data.nickname);
            playerItemDic.TryGetValue(data.userId.ToString(), out item);
            if (item != null)
            {
                Debug.Log("进入游戏的玩家已经存在了");

                item.UpdateUI(data);
                
            }
            else
            {
                
                item = new PlayerItem();
                Debug.Log("进入游戏的玩家不存在："+ playerItemDic.Count);
                item.Init(data, playerItems[playerItemDic.Count].transform);
                playerItemDic.Add(data.userId.ToString(), item);
            }
            UpdateUI(data);
        }
        /// <summary>
        /// 其他玩家离开房间
        /// </summary>
        /// <param name="playerId"></param>
        public void LeaveRoom(string playerId)
        {
            PlayerItem item = null;
            playerItemDic.TryGetValue(playerId, out item);
            if(item!=null)
            {
                item.HideItem();
                playerItemDic.Remove(playerId);
            }
            OnResetRoomUI();


        }
        /// <summary>
        /// 玩家离开更新房间UI ,按顺序排列
        /// </summary>
        private void OnResetRoomUI()
        {
            Debug.Log("RoomWindow OnResetRoomUI");
            HideAllItem();
            try
            {
                int index = 0;
                foreach (var playerItem in playerItemDic)
                {
                    Debug.Log("RoomWindow ResetUI item:" + playerItem.Key+ " index:" + index );
                    PlayerItem item = playerItem.Value;
                    item.Init(item.data, playerItems[index].transform);
                    item.UpdateUI(item.data);
                    UpdateUI(item.data);
                    index++;
                }
            }catch(Exception e)
            {
                Debug.Log("RoomWindow ResetUI error:" + e.Message);
            }
           
             
             
           
             
           
        }
            
            
            


        
        public void DissolveRoom(string owner,string toast)
        {
            if(owner.Equals("1"))
            {
                
                UIManager.instance.CloseWnd(FilesName.ROOMPANEL);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", toast, () => {
                    UIManager.instance.CloseWnd(FilesName.ROOMPANEL);                    
                    NetManager.instance.Send(new MsgBase("JoinScene", JsonUtility.ToJson(new JoinScene("0", UserInfoManager.userID))));
                }, null);
            }
           

            
        }

        private void UpdateUI(PlayerInfoData data)
        {
            if (!string.IsNullOrEmpty(data.homeOwnerId))
            {
                if (data.homeOwnerId.Equals(UserInfoManager.userID))
                {
                    HideAllBtn();
                    Debug.Log("我是房主开始游戏：" + data.gameStartFlag);
                    if (data.gameStartFlag)
                    {

                        startBtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        hintBtn.gameObject.SetActive(true);
                    }

                }
                else
                {
                    UpdateLocalPlayerUI(data);
                }

            }
            else
            {

                UpdateLocalPlayerUI(data);

            }
           

        }
    private void UpdateLocalPlayerUI(PlayerInfoData data)
    {
            if (data.userId.Equals(UserInfoManager.userID))
            {

                Debug.Log("我的id：" + UserInfoManager.userID + "  更新的玩家ID:" + data.userId + "  是否房主：" + data.homeOwnerFlag);
                if (data.homeOwnerFlag.Equals("1"))
                {
                    HideAllBtn();
                    Debug.Log("我是房主开始游戏22：" + data.gameStartFlag);
                    if (data.gameStartFlag)
                    {

                        startBtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        hintBtn.gameObject.SetActive(true);
                    }

                }
                else
                {
                    if (data.readyFlag.Equals("1"))
                    {
                        HideAllBtn();
                        cancelBtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        HideAllBtn();
                        readyBtn.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void StartGameClicked()
        {
          Debug.Log("RoomWindows StartGameClicked");
          WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.enterRoomUrl, WebRequestResponse, true, "{}", RFrameWork.instance.token);

           
        }
        private void HintReadyClicked()
        {
            Debug.Log("RoomWindows HintReadyClicked");
            NetManager.instance.Send(new MsgBase(RequestCode.RemindReady.ToString(), JsonUtility.ToJson(new JoinScene(UserInfoManager.RoomId.ToString(), UserInfoManager.userID))));

        }
        private void ReadyStateClicked(bool isReady)
        {
            Debug.Log("RoomWindows ReadyStateClicked ：" + isReady);
            if(isReady)
            {
                NetManager.instance.Send(new MsgBase(RequestCode.UserReady.ToString(), JsonUtility.ToJson(new JoinScene(UserInfoManager.RoomId.ToString(), UserInfoManager.userID))));

            }
            else
            {
                NetManager.instance.Send(new MsgBase(RequestCode.CancelReady.ToString(), JsonUtility.ToJson(new JoinScene(UserInfoManager.RoomId.ToString(), UserInfoManager.userID))));

            }
        }
        private void BackBtnClicked()
        {
            NetManager.instance.Send(new MsgBase(RequestCode.LeaveRoom.ToString(), JsonUtility.ToJson(new JoinScene(UserInfoManager.RoomId.ToString(), UserInfoManager.userID))));
            NetManager.instance.Send(new MsgBase("JoinScene", JsonUtility.ToJson(new JoinScene("0", UserInfoManager.userID))));
            UIManager.instance.CloseWnd(this);
            Debug.Log("RoomWindows BackBtnClicked");
        }
        private void ClearAllData()
        {
            Debug.Log("RoomWindow 清空数据重置UI");
            if(playerItemDic!=null&&playerItemDic.Count>0)
            {
                playerItemDic.Clear();
            }
            HideAllBtn();
            HideAllItem();
        }
    }
}
