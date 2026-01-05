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
    public class PlayWindow_Old : Window
    {
        private Text totalTimeText;
        private Transform[] playerItems;
        private Transform selfItem;
        private List<RankItem> rankItemList;
        private List<HorseMoveAI> horseMoveAIList;
        private Vector3[] birthPosArray;
        public static Action<string> GameStartResponseAction;
        public static Action<string> GameCountdownResponseAction;
        public static Action GameOverAction;
        private Dictionary<int, List<HorseMoveData>> houseMoveDataDic;
        private DateTime startTime;
        private GameObject countDown3, countDown2, countDown1, countDown0;
        private HorseCamera horseCamera;
        private Camera mainCamera;
        private Vector3 cameraDir;
        private Transform camera;
        private float moveSpeed = 0;
        List<RankInfoData> rankList;
        private GameObject terrain;
        private Transform contents;
        private Animator[] doorAnims;
        private AnimatorStateInfo animatorInfo;
        private Animator animator;
        private bool startMove;
        public static Action<PlayerInfoData> JoinRoomResponseAction;
        public static Action<PlayerInfoData> UpdateRoomResponseAction;
        public static Action<PlayerInfoData> UserReadyResponseAction;
        private Transform horseStateParent;
        private float cameraMoveTime;
        private List<PlayStateItem> playStateItemsList;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            Debug.Log("PlayerWindow Awake2222");
            JoinRoomResponseAction = JoinRoomResponse;
            UpdateRoomResponseAction = UpdateRoomResponse;
            UserReadyResponseAction = UserReadyResponseUI;
            RFrameWork.instance.SetBackAudio("SingleSounds/GameBack1");
            Debug.Log("PlayerWindow Awake333");
            mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
            animator=mainCamera.GetComponent<Animator>();
            camera = mainCamera.transform;
            mainCamera.transform.position = new Vector3(582, 5f, 329);
            mainCamera.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            //cameraDir = mainCamera.transform.position - 位置为1的马的位置;
            houseMoveDataDic = new Dictionary<int, List<HorseMoveData>>();
            rankList = new List<RankInfoData>();
            playStateItemsList=new List<PlayStateItem>();
            Debug.Log("PlayerWindow Awake");
            GameStartResponseAction = GameStartResponse;
            GameCountdownResponseAction = GameCountdownResponse;
            GameOverAction = GameOver;
            birthPosArray = new Vector3[12];
            rankItemList = new List<RankItem>();
            horseMoveAIList = new List<HorseMoveAI>();
            terrain = GameObject.Find("Terrain2(Clone)");
            horseStateParent = terrain.transform.Find("Runway/court/courtDisplay");
            countDown3 = m_GameObject.transform.Find("Countdown/Three").gameObject;
            countDown2 = m_GameObject.transform.Find("Countdown/Two").gameObject;
            countDown1 = m_GameObject.transform.Find("Countdown/One").gameObject;
            countDown0 = m_GameObject.transform.Find("Countdown/Go").gameObject;
            totalTimeText = m_GameObject.transform.Find("Right/Time/Bg/TimeText").GetComponent<Text>();
            contents = m_GameObject.transform.Find("Right/PlayerList/Viewport/Content");
            playerItems = new Transform[contents.childCount - 1];
            selfItem = contents.transform.Find("SelfItem");
            selfItem.gameObject.SetActive(false);
            selfItem.SetAsFirstSibling();
            Transform birthParent = GameObject.Find("BirthPositions").transform;
            for (int i = 0; i < birthParent.childCount; i++)
            {
                birthPosArray[i] = birthParent.GetChild(i).position;
            }
            Transform doorParent = terrain.transform.Find("Runway/court/sluice_gate");
            doorAnims = new Animator[doorParent.childCount];
             for (int i = 0; i < doorParent.childCount; i++)
            {
                doorAnims[i] = doorParent.GetChild(i).Find("Door").GetComponent<Animator>();

            }
             for (int i = 1; i < contents.childCount; i++)
            {
                playerItems[i - 1] = contents.GetChild(i);
            }
            HidePlayerUI();
            cameraMoveTime = AnimatorUtils.GetClipLength(animator, "move");
            Debug.Log("PlayerWindow Awake finish time:"+ cameraMoveTime);





        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            
        }
        private void StartGame(object param1)
        {
            InitUI();
            rankList.Clear();
            RankInfoData[] datas = (RankInfoData[])param1;
            Debug.Log("PlayerWindow Awake OnShow:" + datas.Length);
            for (int i = 0; i < datas.Length; i++)
            {
                rankList.Add(datas[i]);
            }
            OnStartGamePlay(rankList);
        }
        private void InitUI()
        {
            totalTimeText.text = "00:00:00";
            HideAllItems();
        }
        private void HideAllItems()
        {
            for (int i = 0; i < playerItems.Length; i++)
            {
                playerItems[i].gameObject.SetActive(false);
            }
        }
        private void ShowRankItemList(bool show)
        {
            contents.parent.parent.parent.gameObject.SetActive(show);
        }
        private void OnStartGamePlay(List<RankInfoData> datas)
        {
            ShowRankItemList(true);
            Debug.Log("PlayWindow OnStartGamePlay datas count:" + datas.Count);
            RankInfoData mine = null;
            for (int i = 0; i < datas.Count; i++)
            {
                RankInfoData data = datas[i];
                if (data.userId.Equals(UserInfoManager.userID))
                {
                    mine = data;
                    datas.Remove(data);
                    break;
                }
            }
            Debug.Log("PlayWindow OnStartGamePlay datas remove mine count:" + datas.Count);
            EnterRoom(mine, datas);

        }
        private void TestPlay()
        {
            RankInfoData[] datas = new RankInfoData[9];
            RankInfoData mine = null;
            for (int i = 1; i < 11; i++)
            {

                Debug.Log("i:" + i);
                RankInfoData data = new RankInfoData(i.ToString(), "马" + i.ToString(), i - 1, 0);
                if (i == 1)
                {
                    mine = data;
                }
                else
                {
                    datas[i - 2] = data;
                }

            }
            // EnterRoom(mine, datas);

        }
        /// <summary>
        /// 加入房间生成马
        /// </summary>
        /// <param name="mineData"></param>
        /// <param name="otherDatas"></param>
        private void JoinRoomResponse(PlayerInfoData data)
        {
            ShowRankItemList(false);
            RankItem mineItem = new RankItem();
            RankInfoData rankInfoData = new RankInfoData(data.userId, data.nickname, data.track, 0);
            mineItem.Init(rankInfoData, this.selfItem.transform);
            SpawnHorse(data.userId, data.track-1, mineItem, data.horseName,data.horseCode,data.readyFlag,data.countdown);
            Debug.Log("进入房间克隆所有马");


        }
        /// <summary>
        /// 更新房间若已经生成过马则返回
        /// </summary>
        /// <param name="mineData"></param>
        private void UpdateRoomResponse(PlayerInfoData data)
        {
            Debug.Log("房间中所有的马："+ horseMoveAIList.Count+"  当前更新的马"+data.track);
            if(horseMoveAIList.Count<data.track)
            {
                RankItem mineItem = new RankItem();
                RankInfoData rankInfoData = new RankInfoData(data.userId, data.nickname, data.track, 0);
                mineItem.Init(rankInfoData, this.selfItem.transform);
                SpawnHorse(data.userId, data.track - 1, mineItem, data.horseName, data.horseCode, data.readyFlag, data.countdown);
                Debug.Log("其他玩家进入房间克隆所有马");
            }
            


        }
        #region 接收服务端消息
        private void GameStartResponse(string data)
        {

            JsonData jsonData = JsonMapper.ToObject(data);
            JsonData roomData = jsonData["data"]["gameRankResList"];
            for (int i = 0; i < roomData.Count; i++)
            {
                int time = int.Parse(roomData[i]["time"].ToString());
                JsonData speedData = roomData[i]["speedResList"];
                List<HorseMoveData> moveDatas = new List<HorseMoveData>();
                for (int j = 0; j < speedData.Count; j++)
                {
                    HorseMoveData horseData = JsonMapper.ToObject<HorseMoveData>(speedData[j].ToJson());
                    moveDatas.Add(horseData);
                    Debug.Log("sasa 游戏开始马的信息：" + horseData.userId);
                }
                houseMoveDataDic.Add(time, moveDatas);
            }
            //TODO更新马的初始速度
           // var dic = houseMoveDataDic.ElementAt(0);   此方法手机不能使用        
            StartGame();



        }
        private void GameCountdownResponse(string data)
        {
            JsonData jsonData = JsonMapper.ToObject(data);
            string time = jsonData["data"].ToString();
            Debug.Log("倒计时是：" + time);
            HideCountTime();
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
        private void EnterRoom(RankInfoData mineData, List<RankInfoData> otherDatas)
        {
            ClearAllData();
            RankItem mineItem = new RankItem();
            mineItem.Init(mineData, this.selfItem.transform);
            this.selfItem.gameObject.SetActive(true);
            SpawnHorse(mineData.userId, mineData.track, mineItem, mineData.nickname,"gene");
            rankItemList.Add(mineItem);
            for (int i = 0; i < otherDatas.Count; i++)
            {

                RankItem item = new RankItem();
                item.Init(otherDatas[i], playerItems[i].transform);
                rankItemList.Add(item);
                SpawnHorse(otherDatas[i].userId, otherDatas[i].track, item, otherDatas[i].nickname, "gene");

            }
            Debug.Log("进入房间克隆所有马");



        }
        /*  private void CreateHorseCamera()
          {
              horseCamera = ObjectsManager.instance.AddObject(mainCamera.gameObject, "Camera", "HorseCamera") as HorseCamera;
              ChangeCameraSpeed();


          }
          private void ChangeCameraSpeed()
          {
              Debug.Log("ChangeCameraSpeed 11111:");
              Debug.Log("ChangeCameraSpeed 2222:"+ horseMoveAIList.Count);
              horseMoveAIList.Sort((horse1, horse2) => { return horse1.moveSpeed < horse2.moveSpeed ? -1 : 1; });
              float maxSpeed = horseMoveAIList[0].moveSpeed;
              Debug.Log("ChangeCameraSpeed 3333:" + maxSpeed);
              Debug.Log("ChangeCameraSpeed 333:" + (horseCamera==null));
              horseCamera.MoveSpeed = maxSpeed;
              Debug.Log("ChangeCameraSpeed 4444:" + maxSpeed);
              Debug.Log("相机速度更改了："+ maxSpeed.ToString());

          }*/

        private void ChangeCameraSpeed()
        {
            Debug.Log("相机速度更改了 ChangeCameraSpeed 1111:" + horseMoveAIList.Count);
            if (horseMoveAIList.Count > 0)
            {
                horseMoveAIList.Sort((horse1, horse2) => { return horse1.moveSpeed < horse2.moveSpeed ? -1 : 1; });
                moveSpeed = horseMoveAIList[0].moveSpeed;
            }

            Debug.Log("相机速度更改了 ChangeCameraSpeed 3333:" + moveSpeed);
        }
        private void SpawnHorse(string id, int pos, RankItem rankItem, string name,string gene,string state="",int time=0)
        {
            if (UserInfoManager.horseClone != null)
            {
                GameObject horse = GameObject.Instantiate(UserInfoManager.horseClone);
                Debug.Log("开始克隆马了名字：" + name);
                //horse.GetComponent<Animator>().enabled = false;
                object[] names = new object[2] { name, gene };
                HorseMoveAI horseMoveAI = ObjectsManager.instance.AddObject(horse, id, "HorseMoveAI", id, names, rankItem) as HorseMoveAI;
                horseMoveAI.pos = pos;
                horse.transform.position = birthPosArray[pos];
                horse.gameObject.SetActive(true);
                horse.transform.localScale = Vector3.one;
                horseMoveAIList.Add(horseMoveAI);
                AddPlayStateItem(pos,name,state,id,time);
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
        private void AddPlayStateItem(int pos,string name,string state,string id,int time)
        {
            Debug.Log("位置："+pos+""+name+ "  state:" + state+ "  time:" + time);
            
            Transform child = horseStateParent.GetChild(pos);
            child.gameObject.SetActive(true);
            object[] params2 = new object[3] { name, state,id };
            PlayStateItem stateItem = ObjectsManager.instance.AddObject(child.gameObject, child.name, "PlayStateItem", pos, params2, time) as PlayStateItem;
            playStateItemsList.Add(stateItem);
        }
        private void UserReadyResponseUI(PlayerInfoData data)
        {
            if(playStateItemsList.Count > 0)
            {
                for(int i=0;i<playStateItemsList.Count;i++)
                {
                    PlayStateItem item = playStateItemsList[i];
                    if(data.userId.Equals(item.userId))
                    {
                        item.UpdateUI(data.horseName,data.readyFlag);
                    }
                }
            }
        }
        private void HidePlayerUI()
        {
            
            for(int i=0;i<horseStateParent.childCount;i++)
            {
                horseStateParent.GetChild(i).gameObject.SetActive(false);
            }
                
                    
         }
        private void StartGame()
        {
            startTime = DateTime.Now;
            OpenDoor();
            GameManager.cameraMoveEnd = false;
            for (int i = 0; i < horseMoveAIList.Count; i++)
            {
                horseMoveAIList[i].StartGame();
            }
            countDown0.gameObject.SetActive(false);
            UserInfoManager.startGame = true;
            GameManager.gameEnd = false;
        }

        private void OpenDoor()
        {
           for(int i = 0; i < doorAnims.Length; i++)
            {
                //yield return new WaitForSecondsRealtime(0.1f);
                doorAnims[i].SetTrigger("open");
            }
        }
        private void CloseDoor()
        {
            for (int i = 0; i < doorAnims.Length; i++)
            {
                //yield return new WaitForSecondsRealtime(0.1f);
                doorAnims[i].SetTrigger("close");
            }
        }
        /// <summary>
        /// TODO待调用播放相机动画
        /// </summary>
        private void StartPlayCameraMoveAnim()
        {
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
            if ((animatorInfo.normalizedTime > 1.0f) && (animatorInfo.IsName("Idle")))//normalizedTime：0-1在播放、0开始、1结束 MyPlay为状态机动画的名字
            {
                if (!startMove)
                {
                    animator.enabled = false;
                }
            }
        }
        private void GameOver()
        {
            Debug.Log("PlayWindow 游戏结束了");
            UserInfoManager.startGame = false;
            GameManager.gameEnd = true;
            for (int i = 0; i < horseMoveAIList.Count; i++)
            {
                horseMoveAIList[i].m_GameObject.SetActive(false);
            }
             
            horseMoveAIList.Clear();
            rankList.Clear();
            playStateItemsList.Clear();


        }
    
    private void ChangeHorseSpeed(int time)
    {
        List<HorseMoveData> moveDatas;
        houseMoveDataDic.TryGetValue(time, out moveDatas);
        if (moveDatas != null)
        {
            for (int i = 0; i < moveDatas.Count; i++)
            {
                HorseMoveData moveData = moveDatas[i];
                for (int j = 0; j < horseMoveAIList.Count; j++)
                {
                    HorseMoveAI moveAI = horseMoveAIList[j];
                    if (moveAI.userId.Equals(moveData.userId))
                    {
                        moveAI.ChangeSpeed(moveData.speed);
                        Debug.Log(moveAI.Name + "马速度更改了：" + moveData.speed);
                    }

                }

            }
            houseMoveDataDic.Remove(time);
            ChangeCameraSpeed();
        }
    }
    public override void OnUpdate()
    {
        if (UserInfoManager.startGame)
        {
            OnRankHorsePosition();
            CountDownTime();
        }
        if (UserInfoManager.startGame && moveSpeed != 0 && !GameManager.cameraMoveEnd)
        {
            this.camera.Translate(moveSpeed * Time.deltaTime, 0, 0, Space.World);

        }

    }

    private void CountDownTime()
    {
        string lastTime = startTime.ToString();
        DateTime nowTime = DateTime.Now;
        TimeSpan span = nowTime.Subtract(DateTime.Parse(lastTime)).Duration();
        string second = span.Seconds < 10 ? ("0" + span.Seconds) : span.Seconds.ToString();
        int totalTime = (int)span.TotalSeconds;
        if (houseMoveDataDic.ContainsKey((int)span.TotalSeconds))
        {
            ChangeHorseSpeed(totalTime);
        }
        totalTimeText.text = (span.Hours < 10 ? ("0" + span.Hours) : span.Hours.ToString()) + ":" + (span.Minutes < 10 ? ("0" + span.Minutes) : span.Minutes.ToString()) + ":" + (span.Seconds < 10 ? ("0" + span.Seconds) : span.Seconds.ToString());

    }
    private void ChangeSpeed(int time)
    {

    }
    public void OnRankHorsePosition()
    {
        if (Time.frameCount % 60 == 0)
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
            rankItemList.Sort((item1, item2) => { return item1.distance < item2.distance ? -1 : 1; });

        }
        for (int i = 0; i < rankItemList.Count; i++)
        {
            RankItem item = rankItemList[i];
            item.rank = i + 1;
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
        if (houseMoveDataDic.Count > 0)
        {
            houseMoveDataDic.Clear();
        }
            if (playStateItemsList.Count > 0)
            {
                playStateItemsList.Clear();
            }
            HideAllItems();

    }
}
}

