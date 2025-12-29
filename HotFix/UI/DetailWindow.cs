using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.Manager;
using LitJson;
using MalbersAnimations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HotFix
{
    internal class DetailWindow : Window
    {
        HorseDetail horseData;
        /// <summary>
        /// 界面类型 1 马槽里面点击马匹 2 选择马匹开始游戏 3 加入房间 4 选择公马繁育 5 选择母马繁育 6 选择公马进行配种 7 对局内点开马匹详情页 8 租赁马匹 9 选择马匹出租
        /// </summary>
        int panelType;
        int horseCaoIndex = 1;
        public static Action<int> SetDetailPanelActive;
        public static Action<int> SetRankActive;
        public static Action<string> startGame;
        public static Action<string> joinGame;
        public bool isShowTime = false, isCountDown=false;
        int num = 1;
        int count = 6;
        int nowCount = 0;
        DateTime time;
        #region 基本组件
        private Button returnBtn;
        private Text returnText;
        private Button jiBenBtn;
        private Button jingSaiBtn;
        private Button biSaiBtn;
        private Button isDoingBtn;
        private Button isJoinBtn;
        private Button isPlayBtn;
        private Button sureBtn;
        private GameObject isRentOut;
        private GameObject isGirl;
        private Text name;
        private Text howDays;
        private Slider foodSlider;
        private Text foodText;
        private Text foodDesText;
        private GameObject selectBack;
        private GameObject nullImage;
        private GameObject loadImage;
        private Button selectLeftBtn;
        private Button selectRightBtn;
        private Text selectNumText;
        private Text createBabyText;
        private Button createBabyBtn;
        private Button rentOutBtn;
        private Text rentOutText;
        private Button selectFHorseBtn;
        private Button selectMHorseBtn;
        public Transform rankTransform;
        private int isCreate;
        private int Sex=0;
        #endregion
        #region 基本界面
        private GameObject jiBenPanel;
        private Button chengZhangTips;
        private Button xueTongTips;
        private Text oldTime;
        private Slider oldSlider;
        private GameObject isBaby;
        private GameObject isAdult;
        private GameObject isOld;
        private Text xueTongText;
        private Text typeText;
        private Text horseNumText;
        private string horseBlood;
        private string horseName;
        private string horseGene;
        #endregion
        #region 竞赛界面
        private GameObject jingSaiPanel;
        private Transform backImg;
        private Slider qiPaoSlider;
        private Slider suDuSlider;
        private Slider naiLiSlider;
        private Slider zhiHuiSlider;
        private Slider piLaoDuSlider;
        private Text qiPaoNum;
        private Text suDuNum;
        private Text naiLiNum;
        private Text zhiHuiNum;
        private Text piLaoDuNum;
        private Button qiPaoBtn;
        private Button suDuBtn;
        private Button naiLiBtn;
        private Button zhiHuiBtn;
        private Button piLaoDuBtn;
        private Transform myBackImg;
        private Slider qiPaoSlider2;
        private Slider suDuSlider2;
        private Slider naiLiSlider2;
        private Slider zhiHuiSlider2;
        private Slider piLaoDuSlider2;
        private Text qiPaoNum2;
        private Text suDuNum2;
        private Text naiLiNum2;
        private Text zhiHuiNum2;
        private Text piLaoDuNum2;
        private Button qiPaoBtn2;
        private Button suDuBtn2;
        private Button naiLiBtn2;
        private Button zhiHuiBtn2;
        private Button piLaoDuBtn2;
        private Button buyData;
        #endregion
        #region 比赛记录界面
        private GameObject biSaiPanel;
        private Text biSaiText;
        private Text biSaiNumText;
        private Transform content;
        private Transform item;
        #endregion
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            FindAllComponent();
            AddButtonsClick();
            startGame = StartGame;
            joinGame = JoinGame;
            SetDetailPanelActive = SetPanelActive;
            SetRankActive = SetThisRankActive;
            horseCaoIndex = 1;
            if(UserInfoManager.NowHorseList.ContainsKey(horseCaoIndex))
            UserInfoManager.horseID = UserInfoManager.NowHorseList[horseCaoIndex].id;
            content.parent.parent.GetComponent<ScrollRectRef>().top = GoTop;
            content.parent.parent.GetComponent<ScrollRectRef>().bottom = GoBottom;
        }

        private void SetThisRankActive(int num)
        {
            rankTransform.GetChild(0).gameObject.SetActive(num == 1);
            rankTransform.GetChild(1).gameObject.SetActive(num == 2);
            rankTransform.GetChild(2).gameObject.SetActive(num == 3);
            rankTransform.GetChild(3).gameObject.SetActive(num >3);
            rankTransform.GetChild(3).GetChild(1).GetComponent<Text>().text = num.ToString();
            
        }
        bool isSettlement;
        bool isOrderOpen;
        bool isCommon;
        bool isCommonConfirm;
        bool isMap;
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            isSettlement = false;
            isOrderOpen = false;
            isCommon = false;
            isCommonConfirm = false;
            isMap = false;
            UserInfoManager.horseID = UserInfoManager.NowHorseList.Count>0? UserInfoManager.NowHorseList[horseCaoIndex].id:"0";
            UserInfoManager.enterGame = false;
            if(GameMapManager.instance.CurrentMapName == ConStr.MAINSCENE)
                UIManager.instance.CloseWnd(FilesName.MAINPANEL);
            if (UIManager.instance.m_WndRoot.Find(FilesName.SETTLEMENTPANEL.Replace(".prefab", "(Clone)")) && UIManager.instance.m_WndRoot.Find(FilesName.SETTLEMENTPANEL.Replace(".prefab", "(Clone)")).gameObject.activeInHierarchy)
            {
                isSettlement = true;
                UIManager.instance.m_WndRoot.Find(FilesName.SETTLEMENTPANEL.Replace(".prefab", "(Clone)")).gameObject.SetActive(false);
            }
            if (UIManager.instance.m_WndRoot.Find(FilesName.ORDEROPENHOUSEPANEL.Replace(".prefab", "(Clone)")) && UIManager.instance.m_WndRoot.Find(FilesName.ORDEROPENHOUSEPANEL.Replace(".prefab", "(Clone)")).gameObject.activeInHierarchy)
            {
                isOrderOpen = true;
                UIManager.instance.m_WndRoot.Find(FilesName.ORDEROPENHOUSEPANEL.Replace(".prefab", "(Clone)")).gameObject.SetActive(false);
            }
            if (UIManager.instance.m_WndRoot.Find(FilesName.COMMONDATAPANEL.Replace(".prefab", "(Clone)")) && UIManager.instance.m_WndRoot.Find(FilesName.COMMONDATAPANEL.Replace(".prefab", "(Clone)")).gameObject.activeInHierarchy)
            {
                isCommon = true;
                UIManager.instance.m_WndRoot.Find(FilesName.COMMONDATAPANEL.Replace(".prefab", "(Clone)")).gameObject.SetActive(false);
            }
            if (UIManager.instance.m_WndRoot.Find(FilesName.MAPPANEL.Replace(".prefab", "(Clone)")) && UIManager.instance.m_WndRoot.Find(FilesName.MAPPANEL.Replace(".prefab", "(Clone)")).gameObject.activeInHierarchy)
            {
                isMap = true;
                UIManager.instance.m_WndRoot.Find(FilesName.MAPPANEL.Replace(".prefab", "(Clone)")).gameObject.SetActive(false);
            }
            if (UIManager.instance.m_WndRoot.Find("CommonConfirm(Clone)") && UIManager.instance.m_WndRoot.Find("CommonConfirm(Clone)").gameObject.activeInHierarchy)
            {
                isCommonConfirm = true;
                UIManager.instance.m_WndRoot.Find("CommonConfirm(Clone)").gameObject.SetActive(false);
            }
            horseData = param1 as HorseDetail;

            UserInfoManager.horseTexture = horseData.pic;
            UserInfoManager.horseRentOutName = horseData.name;
            panelType = (int)param2;
            SetAllComponentData();
            SetPanelActive(2);
            m_Transform.Find("BG/HorsePos/Horse").localScale = horseData.stage == "0" ? new Vector3(0.7f, 0.7f, 0.7f) :new Vector3(1,1,1);
            HorseUtils.SetHorseTexture(m_Transform.Find("BG/HorsePos/Horse"), horseData.code);
        }

        private void SetAllComponentData()
        {
            string[] type = horseData.code.Split('-');
            bool isdoing = false;
            if (type[0] == "F")
            {
                isDoingBtn.gameObject.SetActive(horseData.status == "5" || horseData.status == "6" || horseData.status == "10" || horseData.status == "11");
                isdoing = (horseData.status == "5" || horseData.status == "6" || horseData.status == "10" || horseData.status == "11");
            }
            else
            {
                isDoingBtn.gameObject.SetActive(horseData.status == "5" || horseData.status == "10" || horseData.status == "11");
                isdoing = (horseData.status == "5" || horseData.status == "10" || horseData.status == "11");
            }
            isPlayBtn.gameObject.SetActive(panelType == 2 && !isdoing);
            rankTransform.gameObject.SetActive(panelType == 7);
            bool ishave = false;
            foreach (var item in UserInfoManager.NowHorseList)
            {
                if (item.Value.id == horseData.id)
                    ishave = true;
            }
            myBackImg.gameObject.SetActive(ishave);
            backImg.gameObject.SetActive(!ishave);
            isPlayBtn.interactable = (panelType == 2 && !isdoing && (horseData.status == "2" || horseData.status == "9") && horseData.stage == "1");
            isJoinBtn.gameObject.SetActive(panelType == 3 && !isdoing);
            isJoinBtn.interactable = (panelType == 3 && !isdoing && (horseData.status == "2" || horseData.status == "9") && horseData.stage == "1");
            if(horseData.status == "7")
            {
                isPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "待取名";
                isJoinBtn.transform.GetChild(0).GetComponent<Text>().text = "待取名";
            }
            else if(horseData.status == "8")
            {
                isPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "名字待审核";
                isJoinBtn.transform.GetChild(0).GetComponent<Text>().text = "名字待审核";
            }
            else if (horseData.stage == "2")
            {
                isPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "老年马";
                isJoinBtn.transform.GetChild(0).GetComponent<Text>().text = "老年马";
            }
            else if (horseData.stage == "0")
            {
                isPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "未成年";
                isJoinBtn.transform.GetChild(0).GetComponent<Text>().text = "未成年";
            }
            else
            {
                isPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "确认发起";
                isJoinBtn.transform.GetChild(0).GetComponent<Text>().text = "立即参赛";
            }
            if (!(panelType == 2 || panelType == 3))
            {
                isDoingBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(0.5019608f, 0.3137255f, 0.2039216f);
            }
            else
            {
                isDoingBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(0.7058824f, 0.5607843f, 0.3686275f);
            }
            rentOutText.text = "租赁费:" + UserInfoManager.rentOutPrice + "马粟/"+UserInfoManager.RentOutTime+"H";
            isRentOut.SetActive(horseData.isRent.Equals("1"));
            selectBack.SetActive(panelType != 1 && panelType != 6&& panelType != 7&&panelType!=8);
            createBabyBtn.transform.parent.gameObject.SetActive(panelType == 6);
            rentOutBtn.transform.parent.gameObject.SetActive(panelType == 8);
            sureBtn.gameObject.SetActive(panelType == 9);
            createBabyText.text = "配种费:" + UserInfoManager.breedPrice + "马粟";
            selectFHorseBtn.gameObject.SetActive(panelType == 5);
            selectMHorseBtn.gameObject.SetActive(panelType == 4);
            if (panelType == 4)
                returnText.text = "选择一匹公马";
            else if (panelType == 5)
                returnText.text = "选择一匹母马";
            else
                returnText.text = "";
            name.text = horseData.name;
            howDays.text = horseData.age;
            Sex = type[0] == "F" ? 0 : 1;
            isGirl.SetActive(type[0] == "F");
            foodSlider.gameObject.SetActive(panelType == 1 && horseData.remainFeedProgress != "0");
            isShowTime = (panelType == 1 && horseData.remainFeedProgress != "0");
            long timestamp = long.Parse(horseData.remainFeedTime);
            System.DateTime startTime = DateTime.Now;//当地时区
            time = startTime.AddSeconds(timestamp/1000);
            isCountDown = true;
            foodText.text = time.Hour + "时" + time.Minute + "分"+time.Second+"秒"; // dt.ToString("yyyy/MM/dd HH:mm:ss");//转化为日期时间
            foodDesText.text = horseData.remainFeedProgress + "/" + horseData.totalFeedProgress;
            foodSlider.value = float.Parse(horseData.remainFeedProgress) / float.Parse(horseData.totalFeedProgress);
            selectNumText.text = horseCaoIndex + "/" + UserInfoManager.NowHorseList.Count;
            SetJiBenPanelData(type);
            SetJingSaiPanelData();
            SetBiSaiPanelData();
        }
        public override void OnUpdate()
        {
            if (isShowTime&& isCountDown)
            {
                if (Time.frameCount % 10 == 0)
                {
                    CountDownTime();
                }
            }

        }
        private void CountDownTime()
        {
            DateTime nowTime = DateTime.Now;
            TimeSpan span = nowTime.Subtract(time).Duration();
            foodText.text = (span.Hours > 0 ? span.Hours.ToString() : "0") + "时" + (span.Minutes > 0 ? span.Minutes.ToString() : "0") + "分" + (span.Seconds > 0 ? span.Seconds.ToString() : "0") + "秒"; // dt.ToString("yyyy/MM/dd HH:mm:ss");//转化为日期时间
            if (TimeUtils.OnDiffSeconds(time, nowTime) > -0.1f)
            {
                isCountDown = false;
                Debug.Log("倒计时结束了");
                foodSlider.gameObject.SetActive(false);
            }
        }

        private void SetPanelActive(int showType)
        {
            string[] type = horseData.code.Split('-');
            jiBenPanel.SetActive(showType == 1);
            jingSaiPanel.SetActive(showType == 2);
            biSaiPanel.SetActive(showType == 3);
            if (showType == 3)
                RefreshHistoryData(UserInfoManager.horseHistoryData);
        }

        private void RefreshHistoryData(List<HorseHistoryData> horseHistoryData)
        {
            nowCount = UserInfoManager.horseHistoryData.Count;
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
            int count = 0;
            foreach (var data in UserInfoManager.horseHistoryData)
            {
                if (count + 1 <= content.childCount)
                {
                    HistoryItem history = new HistoryItem();
                    history.Init(content.GetChild(count), data);
                    history = null;
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(item.gameObject, content).transform;
                    HistoryItem history = new HistoryItem();
                    history.Init(item, data);
                    history = null;
                }
                count++;
            }
            loadImage.SetActive(false);
            content.gameObject.SetActive(true);
            content.localPosition = new Vector3(content.localPosition.x, -1, content.localPosition.z);
        }

        private void SetBiSaiPanelData()
        {
            biSaiText.text = "总共参与" + horseData.matchNumebr + "场比赛";
            biSaiNumText.text = "胜率：" + Math.Round(float.Parse(horseData.winRate) * 100.0f) + "%";
            biSaiNumText.gameObject.SetActive(horseData.matchNumebr > 0);
            nullImage.SetActive(horseData.matchNumebr <= 0);
        }

        private void SetJingSaiPanelData()
        {
            qiPaoNum.text = horseData.startSpeed;
            suDuNum.text = horseData.speed;
            naiLiNum.text = horseData.endurance;
            zhiHuiNum.text = horseData.wisdom;
            piLaoDuNum.text = horseData.fatigue;
            qiPaoSlider.value = float.Parse(horseData.startSpeed) / float.Parse(horseData.startSpeedMax);
            suDuSlider.value = float.Parse(horseData.speed) / float.Parse(horseData.speedMax);
            naiLiSlider.value = float.Parse(horseData.endurance) / float.Parse(horseData.enduranceMax);
            zhiHuiSlider.value = float.Parse(horseData.wisdom) / float.Parse(horseData.wisdomMax);
            piLaoDuSlider.value = float.Parse(horseData.fatigue) / float.Parse(horseData.fatigueMax);
            qiPaoNum2.text = horseData.startSpeed;
            suDuNum2.text = horseData.speed;
            naiLiNum2.text = horseData.endurance;
            zhiHuiNum2.text = horseData.wisdom;
            piLaoDuNum2.text = horseData.fatigue;
            qiPaoSlider2.value = float.Parse(horseData.startSpeed) / float.Parse(horseData.startSpeedMax);
            suDuSlider2.value = float.Parse(horseData.speed) / float.Parse(horseData.speedMax);
            naiLiSlider2.value = float.Parse(horseData.endurance) / float.Parse(horseData.enduranceMax);
            zhiHuiSlider2.value = float.Parse(horseData.wisdom) / float.Parse(horseData.wisdomMax);
            piLaoDuSlider2.value = float.Parse(horseData.fatigue) / float.Parse(horseData.fatigueMax);
        }
        
        private void SetJiBenPanelData(string[] type)
        {
            xueTongText.text = "  " + JsonConfigManager.GetHorseBloodData()[type[2]] + "  ";
            typeText.text = "  " + JsonConfigManager.GetHorseTypeData()[type[1]] + "马  ";
            horseNumText.text = "  基因：" + horseData.code + "  ";
            horseBlood = xueTongText.text;
            horseName = typeText.text;
            horseGene = horseNumText.text;
            if (horseData.stage == "0")
            {
                oldSlider.value = float.Parse(horseData.alreadyGrowUpProgress) / float.Parse(horseData.totalGrowUpProgress) / 2.0f;
                oldTime.text = "距成年马还需" + Math.Round(float.Parse(horseData.remainGrowUpNumber)) + "份马粟";
            }
            else if(horseData.stage == "1")
            {
                oldSlider.value = float.Parse(horseData.alreadyGrowUpProgress) / float.Parse(horseData.totalGrowUpProgress) / 2.0f + 0.5f;
                oldTime.text = "距老年马还剩余" + horseData.remainGrowUpTime + "天";
            }
            else
            {
                oldSlider.value = 1;
                oldTime.text = "";
            }
            isBaby.SetActive(true);
            isAdult.SetActive(horseData.stage != "0");
            isOld.SetActive(horseData.stage == "2");
        }

        private void FindAllComponent()
        {
            returnBtn = m_Transform.Find("Return").GetComponent<Button>();
            returnText = m_Transform.Find("Return/ReturnText").GetComponent<Text>();
            Transform back = m_Transform.Find("Back");
            Transform btns = m_Transform.Find("Back/Btns");
            jiBenBtn = btns.Find("JiBen").GetComponent<Button>();
            jingSaiBtn = btns.Find("JingSai").GetComponent<Button>();
            biSaiBtn = btns.Find("BiSai").GetComponent<Button>();
            isDoingBtn = btns.Find("Doing").GetComponent<Button>();
            isPlayBtn = btns.Find("Play").GetComponent<Button>();
            sureBtn = btns.Find("SureBtn").GetComponent<Button>();
            isJoinBtn = btns.Find("Join").GetComponent<Button>();
            isRentOut = btns.Find("RentOut").gameObject;
            selectFHorseBtn = btns.Find("SelectFHorse").GetComponent<Button>();
            selectMHorseBtn = btns.Find("SelectMHorse").GetComponent<Button>();
            createBabyBtn = m_Transform.Find("CreateBaby/CreateBabyBtn").GetComponent<Button>();
            rentOutBtn = m_Transform.Find("RentOutHorse/CreateBabyBtn").GetComponent<Button>();
            createBabyText = m_Transform.Find("CreateBaby/Img/Num").GetComponent<Text>();
            rentOutText = m_Transform.Find("RentOutHorse/Img/Num").GetComponent<Text>();
            rankTransform = m_Transform.Find("RankNum");
            isGirl = btns.Find("Sex/Image").gameObject;
            name = btns.Find("Name").GetComponent<Text>();
            howDays = btns.Find("Name/Time/Text").GetComponent<Text>();
            foodSlider = m_Transform.Find("FoodLoad").GetComponent<Slider>();
            foodText = m_Transform.Find("FoodLoad/Time").GetComponent<Text>();
            foodDesText = m_Transform.Find("FoodLoad/Num").GetComponent<Text>();
            foodDesText = m_Transform.Find("FoodLoad/Num").GetComponent<Text>();
            nullImage = m_Transform.Find("Back/BiSaiBack/Back/Scroll View/Viewport/Null").gameObject;
            loadImage = m_Transform.Find("Back/BiSaiBack/Back/Scroll View/Viewport/Image").gameObject;
            selectBack = m_Transform.Find("SelectBack").gameObject;
            selectLeftBtn = selectBack.transform.Find("LeftBtn").GetComponent<Button>();
            selectRightBtn = selectBack.transform.Find("RightBtn").GetComponent<Button>();
            selectNumText = selectBack.transform.Find("Num").GetComponent<Text>();
            jiBenPanel = back.Find("JiBenBack").gameObject;
            chengZhangTips = jiBenPanel.transform.Find("Back1/Tip/Image").GetComponent<Button>();
            xueTongTips = jiBenPanel.transform.Find("Back2/Tip/Image").GetComponent<Button>();
            oldTime = jiBenPanel.transform.Find("Back1/Time").GetComponent<Text>();
            oldSlider = jiBenPanel.transform.Find("Back1/LoadingBg").GetComponent<Slider>();
            isBaby = jiBenPanel.transform.Find("Back1/LoadingBg/baby/Image").gameObject;
            isAdult = jiBenPanel.transform.Find("Back1/LoadingBg/adult/Image").gameObject;
            isOld = jiBenPanel.transform.Find("Back1/LoadingBg/old/Image").gameObject;
            xueTongText = jiBenPanel.transform.Find("Back2/Types/XueTong/Text").GetComponent<Text>();
            typeText = jiBenPanel.transform.Find("Back2/Types/Type/Text").GetComponent<Text>();
            horseNumText = jiBenPanel.transform.Find("Back2/Types/Num/Text").GetComponent<Text>();
            jingSaiPanel = back.Find("JingSaiBack").gameObject;
            backImg = jingSaiPanel.transform.Find("BackImg");
            qiPaoBtn = jingSaiPanel.transform.Find("BackImg/QiPao/Tips").GetComponent<Button>();
            suDuBtn = jingSaiPanel.transform.Find("BackImg/SuDu/Tips").GetComponent<Button>();
            naiLiBtn = jingSaiPanel.transform.Find("BackImg/NaiLi/Tips").GetComponent<Button>();
            zhiHuiBtn = jingSaiPanel.transform.Find("BackImg/ZhiHui/Tips").GetComponent<Button>();
            piLaoDuBtn = jingSaiPanel.transform.Find("BackImg/PiLaoDu/Tips").GetComponent<Button>();
            qiPaoSlider = jingSaiPanel.transform.Find("BackImg/QiPao/SliderBack").GetComponent<Slider>();
            suDuSlider = jingSaiPanel.transform.Find("BackImg/SuDu/SliderBack").GetComponent<Slider>();
            naiLiSlider = jingSaiPanel.transform.Find("BackImg/NaiLi/SliderBack").GetComponent<Slider>();
            zhiHuiSlider = jingSaiPanel.transform.Find("BackImg/ZhiHui/SliderBack").GetComponent<Slider>();
            piLaoDuSlider = jingSaiPanel.transform.Find("BackImg/PiLaoDu/SliderBack").GetComponent<Slider>();
            qiPaoNum = jingSaiPanel.transform.Find("BackImg/QiPao/SliderBack/Des").GetComponent<Text>();
            suDuNum = jingSaiPanel.transform.Find("BackImg/SuDu/SliderBack/Des").GetComponent<Text>();
            naiLiNum = jingSaiPanel.transform.Find("BackImg/NaiLi/SliderBack/Des").GetComponent<Text>();
            zhiHuiNum = jingSaiPanel.transform.Find("BackImg/ZhiHui/SliderBack/Des").GetComponent<Text>();
            piLaoDuNum = jingSaiPanel.transform.Find("BackImg/PiLaoDu/SliderBack/Des").GetComponent<Text>();
            myBackImg = jingSaiPanel.transform.Find("MyBackImg");
            qiPaoBtn2 = jingSaiPanel.transform.Find("MyBackImg/QiPao/Tips").GetComponent<Button>();
            suDuBtn2 = jingSaiPanel.transform.Find("MyBackImg/SuDu/Tips").GetComponent<Button>();
            naiLiBtn2 = jingSaiPanel.transform.Find("MyBackImg/NaiLi/Tips").GetComponent<Button>();
            zhiHuiBtn2 = jingSaiPanel.transform.Find("MyBackImg/ZhiHui/Tips").GetComponent<Button>();
            piLaoDuBtn2 = jingSaiPanel.transform.Find("MyBackImg/PiLaoDu/Tips").GetComponent<Button>();
            qiPaoSlider2 = jingSaiPanel.transform.Find("MyBackImg/QiPao/SliderBack").GetComponent<Slider>();
            suDuSlider2 = jingSaiPanel.transform.Find("MyBackImg/SuDu/SliderBack").GetComponent<Slider>();
            naiLiSlider2 = jingSaiPanel.transform.Find("MyBackImg/NaiLi/SliderBack").GetComponent<Slider>();
            zhiHuiSlider2 = jingSaiPanel.transform.Find("MyBackImg/ZhiHui/SliderBack").GetComponent<Slider>();
            piLaoDuSlider2 = jingSaiPanel.transform.Find("MyBackImg/PiLaoDu/SliderBack").GetComponent<Slider>();
            qiPaoNum2 = jingSaiPanel.transform.Find("MyBackImg/QiPao/SliderBack/Des").GetComponent<Text>();
            suDuNum2 = jingSaiPanel.transform.Find("MyBackImg/SuDu/SliderBack/Des").GetComponent<Text>();
            naiLiNum2 = jingSaiPanel.transform.Find("MyBackImg/NaiLi/SliderBack/Des").GetComponent<Text>();
            zhiHuiNum2 = jingSaiPanel.transform.Find("MyBackImg/ZhiHui/SliderBack/Des").GetComponent<Text>();
            piLaoDuNum2 = jingSaiPanel.transform.Find("MyBackImg/PiLaoDu/SliderBack/Des").GetComponent<Text>();
            buyData = m_Transform.transform.Find("Back/JingSaiBack/MyBackImg/AddDataBtn").GetComponent<Button>();
            biSaiPanel = back.Find("BiSaiBack").gameObject;
            biSaiText = biSaiPanel.transform.Find("Back/DesBack/Des").GetComponent<Text>();
            biSaiNumText = biSaiPanel.transform.Find("Back/DesBack/Num").GetComponent<Text>();
            content = biSaiPanel.transform.Find("Back/Scroll View/Viewport/Content");
            item = biSaiPanel.transform.Find("Back/Scroll View/Viewport/Content/Item");
        }

        private void AddButtonsClick()
        {
            AddButtonClickListener(qiPaoBtn, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("起跑", "增加：每天投喂马匹，起跑增加（有上限）。\r\n衰减：24小时内不投喂马匹起跑减少（有下限）。\r\n", () => { }, null);
            });
            AddButtonClickListener(suDuBtn, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("速度", "增加：每天投喂马匹，速度增加（有上限）。\r\n衰减：24小时内不投喂马匹速度降低（有下限）。", () => { }, null);
            });
            AddButtonClickListener(naiLiBtn, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("耐力", "增加：随遛马时长增加耐力增加（有上限）。\r\n衰减：24小时内不遛马耐力减少（有下限）。", () => { }, null);
            });
            AddButtonClickListener(zhiHuiBtn, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("智慧", "公马衰减：繁育场排队开房、交配成功后，一段时间内公马智慧将减少。\r\n母马衰减：孕育出小马后一段时间内母马智慧将减少，后代增多智慧将减少。\r\n", () => { }, null);
            });
            AddButtonClickListener(piLaoDuBtn, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("元气值", "1.周期内比赛场次增多元气值减少，元气值为0时，不可参加比赛 \r\n2.元气值不满时不可放入租赁市场", () => { }, null);
            });
            AddButtonClickListener(qiPaoBtn2, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("起跑", "增加：每天投喂马匹，起跑增加（有上限）。\r\n衰减：24小时内不投喂马匹起跑减少（有下限）。\r\n", () => { }, null);
            });
            AddButtonClickListener(suDuBtn2, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("速度", "增加：每天投喂马匹，速度增加（有上限）。\r\n衰减：24小时内不投喂马匹速度降低（有下限）。", () => { }, null);
            });
            AddButtonClickListener(naiLiBtn2, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("耐力", "增加：随遛马时长增加耐力增加（有上限）。\r\n衰减：24小时内不遛马耐力减少（有下限）。", () => { }, null);
            });
            AddButtonClickListener(zhiHuiBtn2, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("智慧", "公马衰减：繁育场排队开房、交配成功后，一段时间内公马智慧将减少。\r\n母马衰减：孕育出小马后一段时间内母马智慧将减少，后代增多智慧将减少。\r\n", () => { }, null);
            });
            AddButtonClickListener(piLaoDuBtn2, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("元气值", "1.周期内比赛场次增多元气值减少，元气值为0时，不可参加比赛 \r\n2.元气值不满时不可放入租赁市场", () => { }, null);
            });
            AddButtonClickListener(jiBenBtn, () => {
                SetPanelActive(1);
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(xueTongText.transform.parent.parent.GetComponent<RectTransform>());
            });
            AddButtonClickListener(jingSaiBtn, () => { SetPanelActive(2); });
            AddButtonClickListener(createBabyBtn, CreateBabyFunc);
            AddButtonClickListener(selectMHorseBtn, () =>
            {
                if (horseData.stage == "0" || horseData.stage == "2")
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "请选择其他成年马匹", () => { }, null);
                }
                else
                {
                    if (horseData.status == "9")
                    {
                        RFrameWork.instance.OpenCommonConfirm("提示", "此马匹当前正在排队中，请选择其他成年马匹吧", () => { }, null);
                    }
                    else
                    {
                        UIManager.instance.CloseWnd(this);
                        UserInfoManager.selectHorseData = horseData;
                        UIManager.instance.PopUpWnd(FilesName.ORDEROPENHOUSEPANEL, true, false);
                    }
                }

                //WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchCreateUrl, WebRequestFuncitons.CreateGameRoom, true, "{}", RFrameWork.instance.token);
            });
            AddButtonClickListener(selectFHorseBtn, () =>
            {
                if (horseData.stage == "0" || horseData.stage == "2")
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "请选择其他成年马匹", () => { }, null);
                }
                else
                {
                    UserInfoManager.selectHorseData = horseData;
                    UIManager.instance.PopUpWnd(FilesName.PURCHASEBREEDCHARGEPANEL, true, false, UserInfoManager.breedPrice, UserInfoManager.foodNum, PasswordType.BreedingCharge);
                }
            });
            AddButtonClickListener(isPlayBtn, () =>
            {
                if(horseData.stage == "0")
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "请选择其他成年马匹", () => { }, null);
                }
                else
                {
                    if (!NetManager.instance.HasConnected())
                    {
                        NetManager.instance.OnStartReconnect();
                        return;
                    }
                    RFrameWork.instance.OpenCommonConfirm("", "确认消耗"+UserInfoManager.matchPrice + "马粟参与匹配赛？", () =>
                    {
                        WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkPwdUrl, CheckPwdWebRequesetCallBack, true, "{}", RFrameWork.instance.token);
                    }, () =>
                    {

                    });
                }


            });
            AddButtonClickListener(isJoinBtn, () =>
            {
                if (horseData.stage == "0")
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "请选择其他成年马匹", () => { }, null);
                }
                else
                {
                    if (!NetManager.instance.HasConnected())
                    {
                        NetManager.instance.OnStartReconnect();
                        return;
                    }
                    RFrameWork.instance.OpenCommonConfirm("", "确认消耗"+UserInfoManager.matchPrice + "马粟参与匹配赛？", () =>
                    {
                        WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkPwdUrl, CheckPwdWebRequesetCallBack2, true, "{}", RFrameWork.instance.token);
                    }, () =>
                    {

                    });
                }
            });
            AddButtonClickListener(biSaiBtn, () =>
            {
                num = 1;
                nowCount = 0;
                JsonData data = new JsonData();
                data["pageNum"] = 1;
                data["pageSize"] = count;
                data["horseId"] = horseData.id;
                string jsonStr = JsonMapper.ToJson(data);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.pageFront, WebRequestFuncitons.ShowHorsePageFront, true, jsonStr, RFrameWork.instance.token);
            });
            AddButtonClickListener(chengZhangTips, ShowHorseGrowUpPanel);
            AddButtonClickListener(returnBtn, CloseWnd);
            AddButtonClickListener(selectLeftBtn, SelectLeftFunc);
            AddButtonClickListener(selectRightBtn, SelectRightFunc);
            AddButtonClickListener(isDoingBtn, ShowGrowUpDetailPanel);
            AddButtonClickListener(buyData, () =>
            {
                JsonData jsondata = new JsonData();
                jsondata["horseId"] = horseData.id;
                string jsonStr = JsonMapper.ToJson(jsondata);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.getProperty, GetPropertyNum, true, jsonStr, RFrameWork.instance.token);
            });
            AddButtonClickListener(sureBtn, () =>
            {
                //确认出租
                UIManager.instance.PopUpWnd(FilesName.RENTPANEL, true, false,horseData.id,"填写租赁费");
            });
            AddButtonClickListener(rentOutBtn, () =>
            {
                //确认租赁
                UIManager.instance.PopUpWnd(FilesName.PURCHASEBREEDCHARGEPANEL, true, false, UserInfoManager.rentOutPrice, UserInfoManager.foodNum, PasswordType.RentOutHorse);
            });
        }

        private void GoTop()
        {
            if (num <= 1)
            {
                num = 1;
            }
            else
            {
                num--;
            }
            Debug.Log(nowCount + "向上翻页" + num);
            JsonData data = new JsonData();
            data["pageNum"] = num;
            data["pageSize"] = count;
            data["horseId"] = horseData.id;
            string jsonStr = JsonMapper.ToJson(data);
            loadImage.SetActive(true);
            content.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.pageFront, WebRequestFuncitons.ShowHorsePageFront, true, jsonStr, RFrameWork.instance.token);
        }

        private void GoBottom()
        {
            if (nowCount > 0 && nowCount < count)
            {
                num = num;
            }
            else if (nowCount >= count)
            {
                num++;
            }
            Debug.Log(nowCount + "向下翻页" + num);
            JsonData data = new JsonData();
            data["pageNum"] = num;
            data["pageSize"] = count;
            data["horseId"] = horseData.id;
            string jsonStr = JsonMapper.ToJson(data);
            loadImage.SetActive(true);
            content.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.pageFront, WebRequestFuncitons.ShowHorsePageFront, true, jsonStr, RFrameWork.instance.token);
        }
        string passWord = "";
        private void StartGame(string pwd)
        {
            passWord = pwd;
            UserInfoManager.enterGame = true;
            NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
            if (UIManager.instance.GetWndByName(FilesName.PASSWORDINFOPANEL) != null)
                (UIManager.instance.GetWndByName(FilesName.PASSWORDINFOPANEL) as PasswordInfoWindow).isClick = true;
            JsonData jsondata = new JsonData();
            jsondata["id"] = UserInfoManager.mountHorseID;
            string jsonStr = JsonMapper.ToJson(jsondata);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd, WebRequestFuncitons.NullFunc, true, jsonStr, RFrameWork.instance.token);
            GameMapManager.instance.LoadGameScene(ConStr.GAMESCENE, FilesName.PLAYPANEL, StartLoadTerrain);
            UserInfoManager.CloseMainScenesObjectScript(); //离开主场景调用
            isCreate = 0;
        }

        private void JoinGame(string pwd)
        {
            passWord = pwd;
            UserInfoManager.enterGame = true;
            NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
            if (UIManager.instance.GetWndByName(FilesName.PASSWORDINFOPANEL) != null)
                (UIManager.instance.GetWndByName(FilesName.PASSWORDINFOPANEL) as PasswordInfoWindow).isClick = true;
            JsonData jsondata = new JsonData();
            jsondata["id"] = UserInfoManager.mountHorseID;
            string jsonStr = JsonMapper.ToJson(jsondata);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd, WebRequestFuncitons.NullFunc, true, jsonStr, RFrameWork.instance.token);
            GameMapManager.instance.LoadGameScene(ConStr.GAMESCENE, FilesName.PLAYPANEL, StartLoadTerrain);
            UserInfoManager.CloseMainScenesObjectScript(); //离开主场景调用
            isCreate = 1;
        }
        private void CheckPwdWebRequesetCallBack(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string status = jsonData["data"]["status"].ToString();
                if (status.Equals("0"))
                {
                    string remark = jsonData["data"]["remark"].ToString();
                    if (!string.IsNullOrEmpty(remark))
                    {
                        RFrameWork.instance.OpenCommonConfirm("提示", remark, () =>
                        {
                            ToolManager.ExitGame();
                        }, () =>
                        {
                            UIManager.instance.CloseWnd(this);
                        });
                    }
                }
                else
                {
                    UIManager.instance.PopUpWnd(FilesName.PASSWORDINFOPANEL, true, false, PasswordType.StartGame);
                }
            }
        }
        private void CheckPwdWebRequesetCallBack2(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string status = jsonData["data"]["status"].ToString();
                if (status.Equals("0"))
                {
                    string remark = jsonData["data"]["remark"].ToString();
                    if (!string.IsNullOrEmpty(remark))
                    {
                        RFrameWork.instance.OpenCommonConfirm("提示", remark, () =>
                        {
                            ToolManager.ExitGame();
                        }, () =>
                        {
                            UIManager.instance.CloseWnd(this);
                        });
                    }
                }
                else
                {
                    UIManager.instance.PopUpWnd(FilesName.PASSWORDINFOPANEL, true, false, PasswordType.JoinRoom);
                }
            }
        }

        private void GetPropertyNum(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                if (jsonData["data"]["status"].ToString().Equals("1"))
                {
                    int num = (int)float.Parse(jsonData["data"]["amount"].ToString());
                    UIManager.instance.PopUpWnd(FilesName.PROPERTYPANEL, true, false, horseData.id, num);
                }
                else
                {
                    object[] objs = new object[] { "无法提高", jsonData["data"]["remark"].ToString(), 3 ,"确定",""};
                    UIManager.instance.PopUpWnd(FilesName.IFSUCCEEDPANEL, true, false, objs);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void CloseWnd()
        {
            UIManager.instance.CloseWnd(this);
            if (isSettlement)
            {
                UIManager.instance.m_WndRoot.Find(FilesName.SETTLEMENTPANEL.Replace(".prefab", "(Clone)")).gameObject.SetActive(true);
            }
            if (isOrderOpen)
            {
                UIManager.instance.m_WndRoot.Find(FilesName.ORDEROPENHOUSEPANEL.Replace(".prefab", "(Clone)")).gameObject.SetActive(true);
            }
            if (isCommon)
            {
                UIManager.instance.m_WndRoot.Find(FilesName.COMMONDATAPANEL.Replace(".prefab", "(Clone)")).gameObject.SetActive(true);
            }
            if (isCommonConfirm)
            {
                UIManager.instance.m_WndRoot.Find("CommonConfirm(Clone)").gameObject.SetActive(true);
            }
            if (isMap)
            {
                UIManager.instance.m_WndRoot.Find(FilesName.MAPPANEL.Replace(".prefab", "(Clone)")).gameObject.SetActive(true);
            }
            if (GameMapManager.instance.CurrentMapName == ConStr.MAINSCENE)
                UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.GetMyHorsesData, true, "{}", RFrameWork.instance.token);

        }

        private void CreateBabyFunc()
        {
            CloseWnd();
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.GetMyFHorseData, true, JsonMapper.ToJson(new HorseSex(1,2,1,1)), RFrameWork.instance.token);
        }

        private void SelectRightFunc()
        {
            if (UserInfoManager.NowHorseList.ContainsKey(horseCaoIndex + 1))
            {
                horseCaoIndex++;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[horseCaoIndex].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            }
            else
            {
                horseCaoIndex = 1;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[1].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            }
            selectNumText.text = horseCaoIndex + "/" + UserInfoManager.NowHorseList.Count;
            UserInfoManager.horseID = UserInfoManager.NowHorseList[horseCaoIndex].id;
        }

        private void SelectLeftFunc()
        {
            if (UserInfoManager.NowHorseList.ContainsKey(horseCaoIndex - 1))
            {
                horseCaoIndex--;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[horseCaoIndex].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            }
            else
            {
                horseCaoIndex = UserInfoManager.NowHorseList.Count;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[UserInfoManager.NowHorseList.Count].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            }
            selectNumText.text = horseCaoIndex + "/" + UserInfoManager.NowHorseList.Count;
            UserInfoManager.horseID = UserInfoManager.NowHorseList[horseCaoIndex].id;
        }

        public int GetHorseCaoIndex()
        {
            return horseCaoIndex;
        }

        private void ShowHorseGrowUpPanel()
        {
            ListFront data;
            if (typeText.text.Contains("创世"))
            {
                Debug.Log("这是一匹创世马匹");
                data = new ListFront("horse_grow_up_vip_config");
                UserInfoManager.isChuangShi = true;
            }
            else
            {
                Debug.Log("这不是一匹创世马匹");
                data = new ListFront("horse_grow_up_config");
                UserInfoManager.isChuangShi = false;
            }
            string jsonStr = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFront, WebRequestFuncitons.GetGrowUpFront, true, jsonStr, RFrameWork.instance.token);
        }
        private void ShowGrowUpDetailPanel()
        {
            //UserInfoManager.horseID 错误
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseGrowUpUrl + "/" + horseData.id, GrowUpDetailWebRequestResponse, true, "{}", RFrameWork.instance.token);

        }
        private  void GrowUpDetailWebRequestResponse(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                 
                Debug.Log("DetailWindow GrowUpDetailWebRequestResponse 接收到的消息是：" + jsonData["data"].ToJson());
                HorseGrowUpData horseGrowData = JsonMapper.ToObject<HorseGrowUpData>(jsonData["data"].ToJson());
                UIManager.instance.PopUpWnd(FilesName.BREEDINGPANEL,true,false, horseGrowData, Sex.ToString(), horseData);
                
                 
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }
        private void StartLoadTerrain(Action action)
        {
            ObjectManager.instance.InstantiateObjectAsync("Assets/GameData/Prefabs/Building/Racecourse/Terrain2.prefab", (path, go/*GameObject*/, param1, param2, param3) =>
            {
                // GameMapManager.instance.LoadScene();

                Debug.Log("StartLoadTerrain finish" + go.name);
                //NetManager.instance.Send(new MsgBase(RequestCode.LoadFinish.ToString(), JsonUtility.ToJson(new JoinScene(UserInfoManager.RoomId.ToString(), UserInfoManager.userID))));
                GameObject instantiateGo = go as GameObject;
                instantiateGo.SetActive(true);
                instantiateGo.transform.localPosition = new Vector3(-142f, -18.6f, 0);
                GameObject horseClone = ObjectManager.instance.InstantiateObject("Assets/GameData/Prefabs/Animals/HorseLow.prefab", false, true);
                horseClone.name = "HorseClone";
                horseClone.gameObject.SetActive(false);
                UserInfoManager.horseClone = horseClone;
                UIManager.instance.CloseWnd(FilesName.TOPREPAREPANEL);
                UIManager.instance.PopUpWnd(FilesName.ROOMPANEL, true, false);
                if (isCreate == 0)
                {
                    JsonData jsondata = new JsonData();
                    jsondata["horseId"] = horseData.id;
                    jsondata["pwd"] = passWord;
                    string jsonStr = JsonMapper.ToJson(jsondata);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchCreateUrl, WebRequestFuncitons.CreateGameRoom, true, jsonStr, RFrameWork.instance.token);

                }
                else if(isCreate == 1)
                {
                    JoinRoomData roomData = new JoinRoomData(UserInfoManager.RoomId, int.Parse(horseData.id),passWord);
                    string jsonStr = JsonMapper.ToJson(roomData);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchJoinUrl, WebRequestFuncitons.JoinGameRoom, true, jsonStr, RFrameWork.instance.token);

                }
                EcsManager.ClearAllPlayer();
                Debug.Log("ActionInvokeDetailWindow");
                action?.Invoke();

            }, LoadResPriority.RES_HIGHT, false, null, null, null, true);
        }
        public override void OnClose()
        {
            isShowTime = false;
            UserInfoManager.enterGame = false;
            UserInfoManager.NowHorseList.Clear();
            UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
        }


    }
}
