using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class PlayWindow : Window
    {
        private Text totalTimeText;
        private Vector3[] birthPosArray;
        public static Action<string> GameStartResponseAction;
        public static Action<string> GamingResponseAction;
        
        public static Action<string> GameCountdownResponseAction;
        public static Action GameOverAction;
        private DateTime startTime;
        private GameObject countDown3, countDown2, countDown1, countDown0;
        private Transform horseParent;
        private Camera mainCamera;
        private Transform camera;
        private GameObject terrain;
        private Transform contents;
        private AnimatorStateInfo animatorInfo;
        private Animator animator;
        private bool startMove;
        public static Action<PlayerInfoData> JoinRoomResponseAction;
        public static Action<PlayerInfoData> UpdateRoomResponseAction;
        public static Action<PlayerInfoData> UserReadyResponseAction;
        public static Action<string> SetNowTime;
        public static Action<string> LeaveUpdateRoomResponseAction;
        public static Action<string> EnterRoomResponseAction;
        private Transform horseStateParent;
        private float cameraMoveTime;
        private Transform[] playerItems;
        /// <summary>
        /// 玩家的状态倒计时，昵称
        /// </summary>
        private List<PlayStateItem> playStateItemsList;
        /// <summary>
        /// UI玩家排名
        /// </summary>
        private List<RankItem> rankItemList;
        /// <summary>
        /// 保存所有移动的马
        /// </summary>
        private List<HorseMoveAI> horseMoveAIList;
        HorseMoveAI myhorse;
        /// <summary>
        /// 保存服务端马速度，位移和变速的信息
        /// </summary>
        //private Dictionary<string, List<HorseMoveData>> houseMoveDataDic;
        private float maxMoveSpeed;
        private float moveSpeed = -15.1f;
        int distance = -600;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            Debug.unityLogger.logEnabled = false;
            Debug.Log("PlayerWindow Awake2222");
            JoinRoomResponseAction = JoinRoomResponse;
            UpdateRoomResponseAction = UpdateRoomResponse;
            UserReadyResponseAction = UserReadyResponseUI;
            EnterRoomResponseAction = EnterRoomResponse;
            LeaveUpdateRoomResponseAction = LeaveUpdateRoomResponse;
            SetNowTime = SetThisNowTime;
            RFrameWork.instance.SetBackAudio("SingleSounds/GameBack1");
            Debug.Log("PlayerWindow Awake333");
            mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
            animator = mainCamera.GetComponent<Animator>();
            camera = mainCamera.transform;
            mainCamera.transform.position = new Vector3(582, 5f, 329);
            mainCamera.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            playStateItemsList = new List<PlayStateItem>();
            Debug.Log("PlayerWindow Awake");
            GameStartResponseAction = GameStartResponse;
            GamingResponseAction = GamingResponse;
            GameCountdownResponseAction = GameCountdownResponse;
            GameOverAction = GameOver;
            birthPosArray = new Vector3[12];
            rankItemList = new List<RankItem>();
            horseMoveAIList = new List<HorseMoveAI>();
            terrain = GameObject.Find("Terrain2(Clone)");
            Debug.Log("playwindow awake 获取Terrain2成功");
            horseStateParent = terrain.transform.Find("Runway/court/courtDisplay");
            countDown3 = m_GameObject.transform.Find("Countdown/Three").gameObject;
            countDown2 = m_GameObject.transform.Find("Countdown/Two").gameObject;
            countDown1 = m_GameObject.transform.Find("Countdown/One").gameObject;
            countDown0 = m_GameObject.transform.Find("Countdown/Go").gameObject;
            totalTimeText = m_GameObject.transform.Find("Right/Time/Bg/TimeText").GetComponent<Text>();
            contents = m_GameObject.transform.Find("Right/PlayerList/Viewport/Content");
            horseParent = GameObject.Find("HorseParent").transform;
            Debug.Log("playwindow awake 获取倒计时image成功");
            playerItems = new Transform[contents.childCount];
            for(int i=0;i<contents.childCount;i++)
            {
                playerItems[i]=contents.GetChild(i);
                playerItems[i].gameObject.SetActive(false);
            }
            Transform birthParent = GameObject.Find("BirthPositions").transform;
            Debug.Log("playwindow awake 获取BirthPostions成功");
            ShowRankItemList(false);
            for (int i = 0; i < birthParent.childCount; i++)
            {
                birthPosArray[i] = birthParent.GetChild(i).localPosition ;
            }
            Debug.Log("playwindow awake 12345成功");
            HideAllPlayerUI();
            Debug.Log("playwindow awake 54321成功");
            HideAllItems();
            Debug.Log("playwindow awake 45678成功");
            cameraMoveTime = AnimatorUtils.GetClipLength(animator, "move");
            Debug.Log("PlayerWindow Awake finish time:" + cameraMoveTime);
        }

        private void SetThisNowTime(string nowTime)
        {
            float timestamp = float.Parse(nowTime);
            System.DateTime startTime = DateTime.Now;//当地时区
            this.startTime = startTime.AddSeconds(timestamp);
            Debug.Log("PlayWindow SetThisNowTime" + this.startTime);
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {

        }

        private void HideAllItems()
        {
            for (int i = 0; i < contents.childCount; i++)
            {
                contents.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void ShowRankItemList(bool show)
        {
            contents.parent.parent.parent.gameObject.SetActive(show);
        }

        /// <summary>
        /// 加入房间生成马
        /// </summary>
        /// <param name="mineData"></param>
        /// <param name="otherDatas"></param>
        private void JoinRoomResponse(PlayerInfoData data)
        {
            Debug.Log("JoinRoomResponse"+ horseMoveAIList.Count+"   "+ data.userId);
            if (horseMoveAIList.Count > 0)
            {
                for (int i = 0; i < horseMoveAIList.Count; i++)
                {
                    if (horseMoveAIList[i].userId.Equals(data.userId)) return;
                }
            }
            ShowRankItemList(false);
            RankItem mineItem = new RankItem();
            RankInfoData rankInfoData = new RankInfoData(data.userId, data.nickname, data.track, 0);
            int pos = data.track - 1;
            mineItem.Init(rankInfoData, playerItems[pos]);
            Debug.Log("进入房间玩家位置：" + data.track + "  pos:" + pos);
            SpawnHorse(data.userId, data.track - 1, mineItem, data.nickname, data.horseCode, data.readyFlag, data.countdown, float.Parse(data.distance));
            rankItemList.Add(mineItem);
            Debug.Log("进入房间克隆所有马");


        }

        /// <summary>
        /// 更新房间,若已经存在此Id的马则返回
        /// </summary>
        /// <param name="mineData"></param>
        private void UpdateRoomResponse(PlayerInfoData data)
        {
            Debug.Log("房间中所有的马：" + horseMoveAIList.Count + "  当前更新的马" + data.track);
            for(int i = 0; i < horseMoveAIList.Count; i++)
            {
                if (horseMoveAIList[i].userId.Equals(data.userId)) return;
            }
            RankItem mineItem = new RankItem();
            RankInfoData rankInfoData = new RankInfoData(data.userId, data.nickname, data.track, 0);
            int pos = data.track - 1;
            mineItem.Init(rankInfoData, playerItems[pos]);
            Debug.Log("更新房间玩家位置：" + data.track + "  pos:" + pos+"   name:"+data.nickname);
            SpawnHorse(data.userId, data.track - 1, mineItem, data.nickname, data.horseCode, data.readyFlag, data.countdown,float.Parse( data.distance));
            rankItemList.Add(mineItem);
            Debug.Log("其他玩家进入房间克隆所有马");

        }

        /// <summary>
        /// 将房间内的马匹移除
        /// </summary>
        /// <param name="data"></param>
        private void LeaveUpdateRoomResponse(string data)
        {
            
            LeaveRoomResponse(data);
        }

        private void LeaveRoomResponse(string data)
        {
            
            JsonData jsonData = JsonMapper.ToObject(data);
            string userId = jsonData["data"]["userId"].ToString();
            for (int i=0;i<rankItemList.Count;i++)
            {
                RankItem item = rankItemList[i];
                Debug.Log("PlayWindow rankItemList:" + item.id);
                if (item.id.Equals(userId))
                {
                    Debug.Log("PlayWindow rankItemList:" + userId);
                    item.HideUI();
                    rankItemList.Remove(item);
                    break;
                }

            }
            for (int i = 0; i < horseMoveAIList.Count; i++)
            {
                HorseMoveAI horseAI = horseMoveAIList[i];
                Debug.Log("PlayWindow horseMoveAIList:" + horseAI.userId);
                if (horseAI.userId.Equals(userId))
                {
                    Debug.Log("PlayWindow horseMoveAIList:" + userId);
                    MonoBehaviour.DestroyImmediate(horseAI.m_GameObject);
                    horseMoveAIList.Remove(horseAI);
                    break;
                }

            }
            for (int i=0;i<playStateItemsList.Count;i++)
            {
                Debug.Log("PlayWindow playStateItemsList:" + playStateItemsList[i].userId);
                if (playStateItemsList[i].userId.Equals(userId))
                {
                    Debug.Log("PlayWindow playStateItemsList:" + userId);
                    PlayStateItem item = playStateItemsList[i];
                    playStateItemsList.Remove(item);
                    RemovePlayStateItem(item);
                    break;
                }
            }
            //houseMoveDataDic.Remove(userId);
            RoomWindow.LeaveRoomResponseAction(data);



        }
        #region 接收服务端消息
        /// <summary>
        /// 倒计时结束开始游戏
        /// </summary>
        /// <param name="data"></param>
        private void GameStartResponse(string data)
        {
            Debug.Log("马的最大速度：" + maxMoveSpeed);
            StartGame();
        }

       private void GamingResponse(string data)
        {
            JsonData jsonData = JsonMapper.ToObject(data);
          
            HorseMoveData moveData = JsonMapper.ToObject<HorseMoveData>(jsonData["data"].ToJson());
            Debug.Log("GameStartResponse :" + data + "  userId:" + moveData.userId);

            for (int i = 0; i < horseMoveAIList.Count; i++)
            {

                HorseMoveAI horseAI = horseMoveAIList[i];
                Debug.Log("horseAI id :" + horseAI.userId );
                if (horseAI.userId.Equals(moveData.userId))
                {
                    horseAI.rankItem.rank = int.Parse(moveData.rank);
                    horseAI.SetAimPos(float.Parse(moveData.distance), float.Parse(moveData.speed));
                }
            }
        }

        private void StartGame()
        {
            this.startTime = DateTime.Now;
            HideAllPlayerUI();
            OpenDoor();
            ShowRankItemList(true);
            animator.enabled = false;
            GameManager.cameraMoveEnd = false;
            for (int i = 0; i < horseMoveAIList.Count; i++)
            {
                horseMoveAIList[i].StartGame();
            }
            countDown0.gameObject.SetActive(false);
            UserInfoManager.startGame = true;
            GameManager.gameEnd = false;
        }

        private void GameCountdownResponse(string data)
        {
            JsonData jsonData = JsonMapper.ToObject(data);
            string time = jsonData["data"].ToString();
            Debug.Log("倒计时是：" + time);
            HideCountTime();
            UIManager.instance.CloseWnd(FilesName.SHAREPANEL);
            switch (time)
            {
                case "3":
                    RFrameWork.instance.SetOtherAudio("SingleSounds/Countdown");
                    countDown3.gameObject.SetActive(true);
                    break;
                case "2":
                    countDown2.gameObject.SetActive(true);
                    break;
                case "1":
                    countDown1.gameObject.SetActive(true);
                    break;
                case "0":
                    countDown0.gameObject.SetActive(true);
                    break;
                default:
                    Debug.LogError("倒计时错误：" + time);
                    break;
            }

        }

        private void HideCountTime()
        {
            countDown0.gameObject.SetActive(false);
            countDown1.gameObject.SetActive(false);
            countDown2.gameObject.SetActive(false);
            countDown3.gameObject.SetActive(false);
        }
        #endregion
        bool have = false;
        private void SpawnHorse(string id, int pos, RankItem rankItem, string name, string gene, string state = "", int time = 0,float posX = 0)
        {
            have = false;
            if (UserInfoManager.horseClone != null)
            {
                GameObject horse = GameObject.Instantiate(UserInfoManager.horseClone);
                Debug.Log("开始克隆马了名字：" + name);
                //horse.GetComponent<Animator>().enabled = false;
                object[] names = new object[3] { name, gene,maxMoveSpeed };
                HorseMoveAI horseMoveAI = ObjectsManager.instance.AddObject(horse, id, "HorseMoveAI", id, names, rankItem) as HorseMoveAI;
                horse.transform.parent = horseParent;
                horseMoveAI.pos = pos;
                horse.transform.localPosition = new Vector3(-posX, birthPosArray[pos].y, birthPosArray[pos].z);
                horseMoveAI.posZ = birthPosArray[pos].z;
                if (posX != 0 && posX<1000)
                {
                    animator.enabled = false;
                    camera.localPosition = new Vector3(camera.localPosition.x, -13.77f, 348.12f);
                    camera.localRotation = Quaternion.Euler(new Vector3(0, 123.137f,0));
                    GameManager.cameraMoveEnd = false;
                    countDown0.gameObject.SetActive(false);
                    UserInfoManager.startGame = true;
                    GameManager.gameEnd = false;
                    ShowRankItemList(true);
                    horseMoveAI.StartGame();
                    if(UserInfoManager.isLookGame!=1)
                        UIManager.instance.CloseWnd(FilesName.ROOMPANEL);
                }
                horse.gameObject.SetActive(true);
                horse.transform.localScale = Vector3.one*1.5f;
                horseMoveAIList.Add(horseMoveAI);
                AddPlayStateItem(pos, name, state, id, time);
                if(id == UserInfoManager.horseID)
                {
                    have = true;
                    myhorse = horseMoveAI;
                }
            }
            else
            {
                Debug.LogError("SpawnHorse is null");
            }
        }

        /// <summary>
        /// 克隆马的时候添加跑道状态
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        private void AddPlayStateItem(int pos, string name, string state, string id, int time)
        {
            Debug.Log("位置：" + pos + "" + name + "  state:" + state + "  time:" + time);
            Transform child = horseStateParent.GetChild(pos);
            child.gameObject.SetActive(true);
            object[] params2 = new object[3] { name, state, id };
            PlayStateItem stateItem = ObjectsManager.instance.AddObject(child.gameObject, child.name, "PlayStateItem", pos, params2, time) as PlayStateItem;
            playStateItemsList.Add(stateItem);
        }

        private void RemovePlayStateItem(PlayStateItem item)
        {
            if(item!=null)
            {
                if(item.m_GameObject!=null)
                {
                    item.HideUI();
                    ObjectsManager.instance.OnRemove(item);
                }
                
            }
            
            
        }

        private void UserReadyResponseUI(PlayerInfoData data)
        {
            Debug.Log("UserReadyResponseUI data:" + data.userId + "  readyFlag:" + data.readyFlag);
            if (playStateItemsList.Count > 0)
            {
                for (int i = 0; i < playStateItemsList.Count; i++)
                {

                    PlayStateItem item = playStateItemsList[i];
                    Debug.Log("UserReadyResponseUI item:" + (item == null));
                    if (data.userId.Equals(item.userId))
                    {
                        item.UpdateUI(data.nickname, data.readyFlag);
                    }
                }
            }
        }

        private void OpenDoor()
        {
            for(int i=0;i<playStateItemsList.Count;i++)
            {
                PlayStateItem item= playStateItemsList[i];
                item.OpenDoor();
            }
        }

        private void HideAllPlayerUI()
        {
            for (int i = 0; i < horseStateParent.childCount; i++)
            {
                horseStateParent.GetChild(i).Find("Light").gameObject.SetActive(false);
                horseStateParent.GetChild(i).Find("UserName").gameObject.SetActive(false);
                horseStateParent.GetChild(i).Find("State").gameObject.SetActive(false);
            }
        }

        private void EnterRoomResponse(string data)
        {
            Debug.Log("RoomWindow EnterRoomResponse 接收到的消息是：" + data);
            UIManager.instance.CloseWnd(FilesName.ROOMPANEL);
            StartPlayCameraMoveAnim();

        }
       
        /// <summary>
        /// TODO待调用播放相机动画
        /// </summary>
        private void StartPlayCameraMoveAnim()
        {
            Debug.Log("StartPlayCameraMoveAnim :" + startMove);
            if (!startMove)
            {
                startMove = true;
                animator.enabled = true;
                animator.SetTrigger("move");

            }

        }

        private void UpdateCameraAnimState()
        {
            animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
            if ((animatorInfo.normalizedTime > 1.0f) && (animatorInfo.IsName("move")))//normalizedTime：0-1在播放、0开始、1结束 MyPlay为状态机动画的名字
            {
                if (startMove)
                {
                    animator.SetTrigger("endmove");
                    startMove = false;

                }
            }
        }

        private void GameOver()
        {
            Debug.Log("PlayWindow 游戏结束了");
            IEnumeratorTool.instance.StartCoroutineNew(WaitOver());
            ClearAllData();


        }

        System.Collections.IEnumerator WaitOver()
        {
            yield return new WaitForSecondsRealtime(1f);
            Debug.Log("PlayWindow 游戏结束了等待了2秒钟");
            GameManager.cameraMoveEnd = true;
            UserInfoManager.startGame = false;
            GameManager.gameEnd = true;

        }
        Vector3 cameraPos = new Vector3(384.7324f,-13.77f,348.12f);
        public override void OnLateUpdate()
        {
            if(have == false&&horseMoveAIList.Count>0)
            {
                myhorse = horseMoveAIList[0];
            }
            if (UserInfoManager.startGame && moveSpeed != 0 && !GameManager.cameraMoveEnd&&horseMoveAIList.Count>0)
            {
                if(this.camera.transform.position.x<=-581.5f)
                {
                    moveSpeed = 0;
                    GameManager.cameraMoveEnd = true;
                }
                if (-myhorse.m_Transform.localPosition.x / 300 > 1)
                {
                    cameraPos = new Vector3(395.7896f, 3.634026f, 340.873f);
                    this.camera.localRotation = Quaternion.Euler(new Vector3(45.6f, 98.1f,  0));
                }
                if (-myhorse.m_Transform.localPosition.x / 300 > 2)
                {
                    cameraPos = new Vector3(386.616f, -14.24213f, 316.825f);
                    this.camera.localRotation = Quaternion.Euler(new Vector3(4.572f,  45.55f,  0));
                }
                if (-myhorse.m_Transform.localPosition.x / 300 > 3)
                {
                    cameraPos = new Vector3(381.2188f, -14.66f, 322.2f);
                    this.camera.localRotation = Quaternion.Euler(new Vector3(4.67f, 62.3f, 0));
                }
                if (-myhorse.m_Transform.localPosition.x / 985 >= 1)
                {
                    cameraPos = new Vector3(-563.4386f, -3.342211f, 304.806f);
                    this.camera.localRotation = Quaternion.Euler(new Vector3(29.323f, 0.172f, 0));
                    this.camera.localPosition = Vector3.Lerp(this.camera.localPosition, new Vector3(cameraPos.x, cameraPos.y, cameraPos.z), 20 * Time.deltaTime);
                }
                else
                {
                    this.camera.localPosition = Vector3.Lerp(this.camera.localPosition, new Vector3(myhorse.m_Transform.localPosition.x + 425, cameraPos.y, cameraPos.z), 4.5f * Time.deltaTime);
                }
            }
        }

        public override void OnUpdate()
        {
            if (UserInfoManager.startGame)
            {
                OnRankHorsePosition();
                CountDownTime();

            }

            UpdateCameraAnimState();
        }

        /// <summary>
        /// 倒计时
        /// </summary>
        private void CountDownTime()
        {
            DateTime nowTime = DateTime.Now;
            TimeSpan span = nowTime.Subtract(startTime).Duration();
            totalTimeText.text = (span.Hours < 10 ? ("0" + span.Hours) : span.Hours.ToString()) + ":" + (span.Minutes < 10 ? ("0" + span.Minutes) : span.Minutes.ToString()) + ":" + (span.Seconds < 10 ? ("0" + span.Seconds) : span.Seconds.ToString());

        }

        public void OnRankHorsePosition()
        {
            if (Time.frameCount % 30 == 0)
            {
                if (horseMoveAIList.Count > 0)
                {

                    horseMoveAIList.Sort((horse1, horse2) => { return horse1.distance < horse2.distance ? -1 : 1; });


                }
                for (int i = 0; i < horseMoveAIList.Count; i++)
                {
                    horseMoveAIList[i].rank = i + 1;
                }
                RankUI();
            }
        }

        public void RankUI()
        {
            if (rankItemList != null && rankItemList.Count > 0)
            {
                rankItemList.Sort((item1, item2) => {
                    return item1.rank < item2.rank ? -1 : 1;
                });

            }
            for (int i = 0; i < rankItemList.Count; i++)
            {
                RankItem item = rankItemList[i];
                rankItemList[i].UpdateUI(item.rank);
            }

        }

        private void ClearAllData()
        {
            Debug.Log("PlayWindow 清空数据");
            if (rankItemList.Count > 0)
            {
                rankItemList.Clear();
            }
            if (horseMoveAIList.Count > 0)
            {
                horseMoveAIList.Clear();
            }
            if (playStateItemsList.Count > 0)
            {
                playStateItemsList.Clear();
            }
            maxMoveSpeed = 0;
            moveSpeed = 0;
            HideAllItems();

        }
    }
}

