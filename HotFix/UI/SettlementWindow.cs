using Cinemachine;
using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HotFix
{
    public class SettlementWindow : Window
    {
        public Transform firstRank;
        public Transform secondRank;
        public Transform thirdRank;
        public Transform otherRank;
        public Text rankText;
        public Button homeBtn;
        public Button homeBtn2;
        public Button backBtn;
        public Text goodsText;
        public Text roomId;
        public Text timeText;
        public Button shareBtn;
        public Transform content;
        public Transform item;
        ResultRankData[] datas;
        public static Action<string, string, string, int,string,int> SetPanelData;
        public static Action<string> SetHorseCode;
        private bool isClick = false;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllBtnListener();
            SetPanelData = SetThisPanelData;
            SetHorseCode = SetThisHorseCode;
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            isClick = false;
            datas = param1 as ResultRankData[];
        }

        private void UpdateUI(string horseId)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
            int count = 0;
            foreach (var data in datas)
            {
                if (count + 1 <= content.childCount)
                {
                    ResultRankItem rankItem = new ResultRankItem();
                    rankItem.Init(content.GetChild(count), data,horseId);
                    rankItem = null;
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(item.gameObject, content).transform;
                    ResultRankItem rankItem = new ResultRankItem();
                    rankItem.Init(obj, data, horseId);
                    rankItem = null;
                }
                count++;
            }
        }

        private void GetAllComponent()
        {
            firstRank = m_Transform.Find("RankNum/First");
            secondRank = m_Transform.Find("RankNum/Second");
            thirdRank = m_Transform.Find("RankNum/Third");
            otherRank = m_Transform.Find("RankNum/Other");
            rankText = m_Transform.Find("RankNum/Other/Num").GetComponent<Text>();
            homeBtn = m_Transform.Find("HomeBack/HomeBtn").GetComponent<Button>();
            homeBtn2 = m_Transform.Find("OhterHomeBtn").GetComponent<Button>();
            goodsText = m_Transform.Find("HomeBack/Text/Image/AwardNum").GetComponent<Text>();
            roomId = m_Transform.Find("Bg/Right/Title/RoomId").GetComponent<Text>();
            timeText = m_Transform.Find("Bg/Right/Title/Time").GetComponent<Text>();
            shareBtn = m_Transform.Find("Bg/Right/ShareBtn").GetComponent<Button>();
            backBtn = m_Transform.Find("CloseBtn").GetComponent<Button>();
            content = m_Transform.Find("Bg/Right/RankList/Viewport/Content");
            item = m_Transform.Find("Bg/Right/RankList/Viewport/Content/RankItem");
        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(homeBtn, BackHomeFunc);
            AddButtonClickListener(homeBtn2, BackHomeFunc);
            AddButtonClickListener(shareBtn, ShareFunc);
            AddButtonClickListener(backBtn, () =>
            {
                UIManager.instance.CloseWnd(this);
                UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
            });
        }

        private void BackHomeFunc()
        {
            if (isClick)
                return;
            UserInfoManager.camera.GetComponent<Animator>().enabled = false;
            UserInfoManager.camera.transform.GetComponent<CinemachineBrain>().enabled = true;
            UserInfoManager.isGoToSiyangchang = false;
            UserInfoManager.enterGame = false;
            GameMapManager.instance.LoadScene(ConStr.MAINSCENE, FilesName.MAINPANEL, HouseManager.LoadMainScene);
            isClick = true;
        }

        private void ShareFunc()
        {
            if(UserInfoManager.playerRank>0 && UserInfoManager.isLookGame == 0)
                UserInfoManager.rankStr = "我在赛马比赛中获得了「第" + UserInfoManager.playerRank + "名」，你也来试试吧～";
            else
                UserInfoManager.rankStr = "邀请您下载元年app体验「马术元宇宙」";
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myInvite, WebRequestFuncitons.ShareFunc, true, "{}", RFrameWork.instance.token);
        }

        public void SetThisPanelData(string roomId, string horseId, string startTime, int rank,string goodsStr,int bl = 1)
        {
            this.roomId.text = "(房间ID：" + roomId + ")";
            timeText.text = startTime;
            firstRank.gameObject.SetActive(rank == 1);
            secondRank.gameObject.SetActive(rank == 2);
            thirdRank.gameObject.SetActive(rank == 3);
            otherRank.gameObject.SetActive(rank > 3);
            UserInfoManager.playerRank = rank;
            rankText.text = rank.ToString();
            goodsText.text = "+"+goodsStr;
            firstRank.parent.gameObject.SetActive(bl==1);
            homeBtn.transform.parent.gameObject.SetActive(bl == 1 && rank<7&&rank!=0);
            homeBtn2.transform.gameObject.SetActive((bl == 1 && rank>6)||rank == 0);
            shareBtn.gameObject.SetActive(bl == 1);
            backBtn.gameObject.SetActive(bl == 0&&rank!=0);

            UpdateUI(horseId);
            if (bl == 0)
            {
                SetThisHorseCode(datas[0].horseCode);
                UIManager.instance.CloseWnd(FilesName.GAMELISTPANEL);
            }
        }

        private void SetThisHorseCode(string horseCode)
        {
            HorseUtils.SetHorseTexture(m_Transform.Find("BG/HorsePos/Horse"),horseCode);
        }
    }
}
