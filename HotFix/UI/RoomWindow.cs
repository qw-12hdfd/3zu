using Cinemachine;
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
    internal class RoomWindow : Window
    {
        private Button shareBtn;
        private Text horseFeedNum;
        private Button quitBtn;
        private Button cancelBtn;
        private Text cancelText;
        public static Action<string> JoinRoomResponseAction;
        public static Action<string> UpdateRoomResponseAction;
        
        public static Action<string> DissolveRoomResponseAction;
        public static Action<string> ReadyResponseAction;
        public static Action<string> RemindReadyResponseAction;
        public static Action MandatoryExitResponseAction;
        public static Action EnterRoomFailedResponseAction;
        public static Action<string> LeaveRoomResponseAction;
        private Animator cameraAnim;
        private int totalNum;
        private int curNum;
        private int totalTime;
        private int remainTime;
        private bool isCountDown;
        private DateTime startTime;
        public bool countDownTime = false;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            RFrameWork.instance.SetBackAudio("SingleSounds/RoomWait");
            Debug.Log("RoomWindow Awake:" + m_GameObject.name);
            JoinRoomResponseAction = JoinRoomRequestResponse;
            UpdateRoomResponseAction = UpdateRoomRequestResponse;
             DissolveRoomResponseAction = DissolveRoomResponse;
            ReadyResponseAction = ReadyRequestResponse;
            LeaveRoomResponseAction = LeaveRoomResponse;
            RemindReadyResponseAction = RemindReadyResponse;
            MandatoryExitResponseAction = MandatoryExitResponse;
            EnterRoomFailedResponseAction = BackMainScene;
            isCountDown = false;
            GetAllComponents();
            UIManager.instance.PopUpWnd(FilesName.PLAYPANEL, true, false);
            Debug.Log("RoomWindow Awake finish"+ JoinRoomResponseAction);
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            startTime = DateTime.Now;
            MainWindow.SetBtnGray(true);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshNumData, true, "{}", RFrameWork.instance.token);
            countDownTime = true;
        }

        private void RefreshNumData(string jsonStr)
        {
            Debug.Log("RefreshData:" + jsonStr);
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string shareAmount = jsonData["data"]["shareAmount"].ToString();
                string milletAmount = jsonData["data"]["milletAmount"].ToString();
                string totalShareAmount = jsonData["data"]["totalShareAmount"].ToString();
                string hoserFeedNumber = jsonData["data"]["hoserFeedNumber"].ToString();
                UserInfoManager.hoserFeedNumber = (float)Math.Round(float.Parse(hoserFeedNumber), 2);
                UserInfoManager.peiENum = (float)Math.Round(float.Parse(shareAmount), 2);
                UserInfoManager.foodNum = (float)Math.Round(float.Parse(milletAmount), 2);
                UserInfoManager.allPeiENum = (float)Math.Round(float.Parse(totalShareAmount), 2);
                horseFeedNum.text = "         " + UserInfoManager.foodNum + "  ";
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(horseFeedNum.transform.parent.GetComponent<RectTransform>());
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }
        private void GetAllComponents()
        {
            shareBtn = m_Transform.Find("Btns/SharePic").GetComponent<Button>();
            horseFeedNum = m_Transform.Find("Btns/Money/Text").GetComponent<Text>();
            quitBtn = m_Transform.Find("QuitBtn").GetComponent<Button>();
            cancelBtn = m_Transform.Find("CancelBtn").GetComponent<Button>();
            cancelText = m_Transform.Find("CancelBtn/TimeText").GetComponent<Text>();

            AddButtonClickListener(shareBtn, ShareFunc);
            AddButtonClickListener(quitBtn, QuitFunc);
            AddButtonClickListener(cancelBtn, CancelFunc);
            cameraAnim = GameObject.Find("Camera").GetComponent<Animator>();
            ResetCamera();
            cancelBtn.gameObject.SetActive(UserInfoManager.isLookGame == 0);
            quitBtn.gameObject.SetActive(UserInfoManager.isLookGame == 1);
        }

        private void ShareFunc()
        {
            UserInfoManager.rankStr = "邀请您下载元年app体验「马术元宇宙」";
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myInvite, WebRequestFuncitons.ShareFunc, true, "{}", RFrameWork.instance.token);
        }

        private void QuitFunc()
        {
            RFrameWork.instance.OpenCommonConfirm("提示", "您是否要退出观战？", () =>
            {
                Debug.Log("LeaveRoomBtnClicked  离开房间 JoinScene");
                NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("0", UserInfoManager.userID))));
                UIManager.instance.CloseWnd(this);
                BackMainScene();
            }, () =>
            {

            });
        }

        private void JoinSceneFunc()
        {
            Debug.Log("LeaveRoomBtnClicked  离开房间 JoinScene");
            NetManager.instance.Send(new MsgBase(RequestCode.LeaveRoom.ToString(), JsonUtility.ToJson(new JoinScene(UserInfoManager.RoomId.ToString(), UserInfoManager.userID))));
            UIManager.instance.CloseWnd(this);
            Debug.Log("RoomWindows BackBtnClicked");
        }

        private void CancelFunc()
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkMatch, CheckMatch, true, "{}", RFrameWork.instance.token);
        }

        internal void CheckMatch(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string waitTime = jsonData["data"]["waitTime"].ToString();//二维码
                Action joinScene = JoinSceneFunc;
                object[] objs = new object[] { "确认退出？", "若退出" + waitTime + "分钟内不可再次参与比赛，马粟原路返回。", 3, "确定", "取消"};
                UIManager.instance.PopUpWnd(FilesName.IFSUCCEEDPANEL, true, false, objs, joinScene, null);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }


        #region 接收服务端消息回调

        private void WebRequestResponse(string jsonInfo)
        {
            Debug.Log("接收到的消息是：" + jsonInfo);
        }

        private void JoinRoomRequestResponse(string data)
        {
            Debug.Log("RoomWindow JoinRoomRequestResponse 接收到的消息是：" + data);
            JsonData jsonData = JsonMapper.ToObject(data);
            totalNum = int.Parse(jsonData["data"]["number"].ToString());
            JsonData roomData = jsonData["data"]["roomUserResList"];
            string roomName = jsonData["data"]["roomNumber"].ToString();
            string nowTime = jsonData["data"]["nowTime"].ToString();
            PlayWindow.SetNowTime(nowTime);
            if (GameObject.Find("HouseNameText"))
            {
                GameObject.Find("HouseNameText").GetComponent<TextMesh>().text = "匹配赛(房间号:"+roomName+")";
            }

            PlayerInfoData[] datas = new PlayerInfoData[roomData.Count];
            Debug.Log("joinroomrequestResponse roomDataCount  " + roomData.Count);
            for (int i = 0; i < roomData.Count; i++)
            {
                Debug.Log(roomData[i].ToJson());
                PlayerInfoData playerInfoData = JsonMapper.ToObject<PlayerInfoData>(roomData[i].ToJson());
                Debug.Log("playerinfoData" + i + "   " + playerInfoData.userId);
                PlayWindow.JoinRoomResponseAction(playerInfoData);
                curNum = playerInfoData.readyNumber;
                Debug.Log("curNum" + i + "   " + curNum);
                datas[i] = playerInfoData;
                Debug.Log("datas[i]" + i + "   " + datas[i]);

            }

        }

        private void UpdateRoomRequestResponse(string data)
        {
            Debug.Log("RoomWindow UpdateRoomRequestResponse 接收到的消息是：" + data);
            //TODO 已经准备的玩家数和倒计时
            JsonData jsonData = JsonMapper.ToObject(data);
            PlayerInfoData playerInfoData = JsonMapper.ToObject<PlayerInfoData>(jsonData["data"].ToJson());
            PlayWindow.UpdateRoomResponseAction(playerInfoData);
            totalNum = playerInfoData.number;
            curNum = playerInfoData.readyNumber;
            
        }

        private void LeaveRoomResponse(string data)
        {
            Debug.Log("RoomWindow LeaveRoomResponse 接收到的消息是：" + data);
            //TODO 已经准备的玩家数和倒计时
            JsonData jsonData = JsonMapper.ToObject(data);
            totalNum = int.Parse(jsonData["data"]["number"].ToString()); 
            curNum =int.Parse(jsonData["data"]["readlyNumber"].ToString());
            string userId = jsonData["data"]["userId"].ToString();
            if (userId.Equals(UserInfoManager.userID))
            {
                Debug.Log("QuitRoomPanel_4444444444444444444444444444444444444");
                BackMainScene();
            }
            


        }

        private void DissolveRoomResponse(string data)
        {
                    UIManager.instance.CloseWnd(FilesName.ROOMPANEL);
                    Debug.Log("DissolveRoom  解散房间2 JoinScene");

                    NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
            Debug.Log("QuitRoomPanel_555555555555555555555555555555555555555555555555");
            BackMainScene();
        }

        private void MandatoryExitResponse()
        {
            RFrameWork.instance.OpenCommonConfirm("提示", "您被踢出房间了", () => {
                UIManager.instance.CloseWnd(FilesName.ROOMPANEL);
                Debug.Log("MandatoryExitResponse  踢出房间 JoinScene");

                NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
                BackMainScene();
            }, null);
        }
        
        private void ReadyRequestResponse(string data)
        {
            JsonData jsonData = JsonMapper.ToObject(data);
            Debug.Log("RoomWindow ReadyRequestResponse 接收到的消息是：" + data);
            PlayerInfoData playerInfoData = JsonMapper.ToObject<PlayerInfoData>(jsonData["data"].ToJson());         
            curNum = playerInfoData.readyNumber;
            PlayWindow.UserReadyResponseAction(playerInfoData);
            
         

        }

        private void RemindReadyResponse(string data)
        {
            Debug.Log("RoomWindow RemindReadyResponse 接收到的消息是：" + data);
            JsonData jsonData = JsonMapper.ToObject(data);
            string toast = jsonData["data"]["remark"].ToString();
            string houseOwnerId= jsonData["data"]["userId"].ToString();
            RFrameWork.instance.OpenCommonConfirm("提示", toast, ConfirmClicked, null);
           

        }

        private void ResetCamera()
        {
            Debug.Log("重置摄像机");
            cameraAnim.enabled=true;
            cameraAnim.transform.GetComponent<CinemachineBrain>().enabled=false;
            cameraAnim.transform.GetComponent<Camera>().fieldOfView = 48;
        }
        #endregion
        private void ConfirmClicked()
        {
            Debug.Log("ConfirmClicked");
        }
        public void DissolveRoom(string owner,string toast)
        {
            if(owner.Equals("1"))
            {
                //TODO退出房间
                BackMainScene();
                UIManager.instance.CloseWnd(FilesName.ROOMPANEL);
                Debug.Log("DissolveRoom  解散房间 JoinScene");

                NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));

               }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", toast, () => {
                    UIManager.instance.CloseWnd(FilesName.ROOMPANEL);
                    Debug.Log("DissolveRoom  解散房间2 JoinScene");

                    NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
                    BackMainScene();
                }, null);
            }
        }

        private void BackMainScene()
        {
            Debug.Log("RoomWindow BackMainScene");
            UserInfoManager.enterGame=false;
            cameraAnim.enabled = false;
            cameraAnim.transform.GetComponent<CinemachineBrain>().enabled = true;
            UserInfoManager.isGoToSiyangchang = false;
            GameMapManager.instance.LoadScene(ConStr.MAINSCENE, FilesName.MAINPANEL, HouseManager.LoadMainScene);
            UIManager.instance.CloseWnd(FilesName.ROOMPANEL,true);
            UIManager.instance.CloseWnd(FilesName.PLAYPANEL, true);
        }

        public override void OnUpdate()
        {
            if (countDownTime)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan span = nowTime.Subtract(startTime).Duration();
                cancelText.text = (span.Minutes < 10 ? ("0" + span.Minutes) : span.Minutes.ToString()) + "分" + (span.Seconds < 10 ? ("0" + span.Seconds) : span.Seconds.ToString()) + "秒";
            }
        }

        public override void OnClose()
        {
            UIManager.instance.CloseWnd(FilesName.SHAREPANEL);
            countDownTime = false;
        }

    }
}
