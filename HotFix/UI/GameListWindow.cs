using HotFix.Common;
using HotFix.Common.Utils;
using LitJson;
using MalbersAnimations.HAP;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.TouchScreenKeyboard;
using Object = UnityEngine.Object;

namespace HotFix
{
    internal class GameListWindow:Window
    {
        private Button gameListBtn;
        private Button myHistoryBtn;
        private Button startGameBtn;
        private Button returnBtn;
        private Button moneyBtn;
        private Button allBtn;
        private Button waitBtn;
        private Button doingBtn;
        private Button overBtn;
        private Button detailBtn;
        private Text gamePrice;
        private GameObject gameListPanel;
        private GameObject myHistoryPanel;
        private GameObject nullImage;
        private Transform gameListContent;
        private Transform historyListContent;
        private Transform gameItem;
        private Transform historyItem;
        private Transform selectBtn;
        public static Action<List<GameData>> SetGameListData;
        public static Action<List<HistoryDatas>> SetHistoryListData;
        public static Action addItemToList;
        public static GameObject loadImg;

        int gameNum = 1;
        int gameCount = 6;
        int gameNowCount = 0;

        int historyNum = 1;
        int historyCount = 6;
        int historyNowCount = 0;

        string status = "all";
        int statusNum = -1;
       
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            gameListBtn = m_Transform.Find("Back/Line/GameListBtn").GetComponent<Button>();
            myHistoryBtn = m_Transform.Find("Back/Line/HistoryBtn").GetComponent<Button>();
            startGameBtn = m_Transform.Find("Back/Line/StartGame").GetComponent<Button>();
            moneyBtn = m_Transform.Find("Money").GetComponent<Button>();
            allBtn = m_Transform.Find("Back/GameList/Btns/All").GetComponent<Button>();
            waitBtn = m_Transform.Find("Back/GameList/Btns/Wait").GetComponent<Button>();
            doingBtn = m_Transform.Find("Back/GameList/Btns/Doing").GetComponent<Button>();
            overBtn = m_Transform.Find("Back/GameList/Btns/Over").GetComponent<Button>();
            detailBtn = m_Transform.Find("DetailBtn").GetComponent<Button>();
            returnBtn = m_Transform.Find("ReturnBtn").GetComponent<Button>();
            gamePrice = m_Transform.Find("Back/GameList/GamePrice").GetComponent<Text>();
            gameListPanel = m_Transform.Find("Back/GameList").gameObject;
            myHistoryPanel = m_Transform.Find("Back/HistoryList").gameObject;
            nullImage = m_Transform.Find("Back/Null").gameObject;
            loadImg = m_Transform.Find("Back/Image").gameObject;
            gameItem = m_Transform.Find("Back/GameList/Viewport/Content/Item");
            gameListContent = m_Transform.Find("Back/GameList/Viewport/Content");
            gameItem.gameObject.SetActive(false);
            historyItem = m_Transform.Find("Back/HistoryList/Viewport/Content/Item");
            historyListContent = m_Transform.Find("Back/HistoryList/Viewport/Content");
            selectBtn = allBtn.transform.GetChild(0);
            historyItem.gameObject.SetActive(false);
            SetGameListData = RefreshGameList;
            SetHistoryListData = RefreshHistoryList;
            addItemToList = AddItemToList;
            AddButtonClickListener(returnBtn, () => {
                UIManager.instance.CloseWnd(this);
                UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
            });
            AddButtonClickListener(gameListBtn, ShowGameList);
            AddButtonClickListener(myHistoryBtn, ShowHistoryBtn);
            AddButtonClickListener(startGameBtn, StartGame);
            AddButtonClickListener(moneyBtn, () => {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshNumData, true, "{}", RFrameWork.instance.token);
                UIManager.instance.PopUpWnd(FilesName.HORSEFEEDPANEL, true, false, moneyBtn.transform);
            });
            AddButtonClickListener(allBtn, AllItemsFunc);
            AddButtonClickListener(waitBtn, WaitItemsFunc);
            AddButtonClickListener(doingBtn, DoingItemsFunc);
            AddButtonClickListener(overBtn, OverItemsFunc);
            AddButtonClickListener(detailBtn, () =>
            {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.gameDetail, WebRequestFuncitons.LookGameDetail, true,"{}", RFrameWork.instance.token);
            });
            gameListContent.parent.parent.GetComponent<ScrollRectRef>().top = GoGameListTop;
            gameListContent.parent.parent.GetComponent<ScrollRectRef>().bottom = GoGameListBottom;
            historyListContent.parent.parent.GetComponent<ScrollRectRef>().top = GoHistoryListTop;
            historyListContent.parent.parent.GetComponent<ScrollRectRef>().bottom = GoHistoryListBottom;
        }

        private void GoGameListTop()
        {
            if (gameNum <= 1)
            {
                gameNum = 1;
            }
            else
            {
                gameNum--;
            }
            Debug.Log(gameNowCount + "向上翻页" + gameNum);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = gameCount;
            if(statusNum>=0)
                data["status"] = statusNum;
            loadImg.SetActive(true);
            nullImage.SetActive(false);
            gameListContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void GoGameListBottom()
        {
            if (gameNowCount > 0 && gameNowCount < gameCount)
            {
                gameNum = gameNum;
            }
            else if (gameNowCount >= gameCount)
            {
                gameNum++;
            }
            Debug.Log(gameNowCount + "向下翻页" + gameNum);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = gameCount;
            if (statusNum >= 0)
                data["status"] = statusNum;
            loadImg.SetActive(true);
            nullImage.SetActive(false);
            gameListContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void GoHistoryListTop()
        {
            if (historyNum <= 1)
            {
                historyNum = 1;
            }
            else
            {
                historyNum--;
            }
            Debug.Log(historyNowCount + "向上翻页" + historyNum);
            JsonData data = new JsonData();
            data["pageNum"] = historyNum;
            data["pageSize"] = historyCount;
            string jsonStr = JsonMapper.ToJson(data);
            loadImg.SetActive(true);
            nullImage.SetActive(false);
            historyListContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchRecordUrl, WebRequestFuncitons.GetGameHistoryData, true, jsonStr, RFrameWork.instance.token);
        }

        private void GoHistoryListBottom()
        {
            if (historyNowCount > 0 && historyNowCount < historyCount)
            {
                historyNum = historyNum;
            }
            else if (historyNowCount >= historyCount)
            {
                historyNum++;
            }
            Debug.Log(historyNowCount + "向下翻页" + historyNum);
            JsonData data = new JsonData();
            data["pageNum"] = historyNum;
            data["pageSize"] = historyCount;
            string jsonStr = JsonMapper.ToJson(data);
            loadImg.SetActive(true);
            nullImage.SetActive(false);
            historyListContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchRecordUrl, WebRequestFuncitons.GetGameHistoryData, true, jsonStr, RFrameWork.instance.token);
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
                string matchPrice = jsonData["data"]["matchPrice"].ToString();
                string hoserFeedNumber = jsonData["data"]["hoserFeedNumber"].ToString();
                UserInfoManager.hoserFeedNumber = (float)Math.Round(float.Parse(hoserFeedNumber), 2);
                UserInfoManager.peiENum = (float)Math.Round(float.Parse(shareAmount), 2);
                UserInfoManager.foodNum = (float)Math.Round(float.Parse(milletAmount), 2);
                UserInfoManager.allPeiENum = (float)Math.Round(float.Parse(totalShareAmount), 2);
                UserInfoManager.matchPrice = matchPrice;
                gamePrice.text = "比赛入场费：" + matchPrice;
                moneyBtn.transform.GetChild(0).GetComponent<Text>().text = "         " + UserInfoManager.foodNum + "       ";
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(moneyBtn.GetComponent<RectTransform>());
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            UIManager.instance.CloseWnd(FilesName.MAINPANEL);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshNumData, true, "{}", RFrameWork.instance.token);
            ShowGameList();
            AllItemsFunc();
            NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("0", UserInfoManager.userID))));
        }

        private void AllItemsFunc()
        {
            status = "all";
            statusNum = -1;
            selectBtn.gameObject.SetActive(false);
            selectBtn = allBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            NumberOfPages data = new NumberOfPages(1, gameCount);
            string jsonStr = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, jsonStr, RFrameWork.instance.token);
        }

        private void WaitItemsFunc()
        {
            status = "wait";
            statusNum = 0;
            selectBtn.gameObject.SetActive(false);
            selectBtn = waitBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = gameCount;
            data["status"] = 0;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void DoingItemsFunc()
        {
            status = "doing";
            statusNum = 1;
            selectBtn.gameObject.SetActive(false);
            selectBtn = doingBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = gameCount;
            data["status"] = 1;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void OverItemsFunc()
        {
            status = "over";
            statusNum = 2;
            selectBtn.gameObject.SetActive(false);
            selectBtn = overBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = gameCount;
            data["status"] = 2;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void RefreshGameList(List<GameData> gameList)
        {
            gameNowCount = gameList.Count;
            for (int i = 0; i < gameListContent.childCount; i++)
            {
                gameListContent.GetChild(i).gameObject.SetActive(false);
            }
            int count = 0;
            foreach (var data in gameList)
            {
                if (count + 1 <= gameListContent.childCount)
                {
                    GameListItem gameListData = new GameListItem();
                    gameListData.Init(gameListContent.GetChild(count), data);
                    gameListData = null;
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(gameItem.gameObject, gameListContent).transform;
                    GameListItem gameListData = new GameListItem();
                    gameListData.Init(obj, data);
                    gameListData = null;
                }
                count++;
            }
            gameListContent.gameObject.SetActive(true);
            loadImg.SetActive(false);
            nullImage.SetActive(gameNum <= 1 && gameNowCount <= 0);
            gameListContent.localPosition = new Vector3(gameListContent.localPosition.x, -1, gameListContent.localPosition.z);
        }

        private void RefreshHistoryList(List<HistoryDatas> historyList)
        {
            historyNowCount = historyList.Count;
            for (int i = 0; i < historyListContent.childCount; i++)
            {
                historyListContent.GetChild(i).gameObject.SetActive(false);
            }
            int count = 0;
            foreach (var data in historyList)
            {
                if (count + 1 <= historyListContent.childCount)
                {
                    GameHistoryItem gameHistoryItem = new GameHistoryItem();
                    gameHistoryItem.Init(historyListContent.GetChild(count), data);
                    gameHistoryItem = null;
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(historyItem.gameObject, historyListContent).transform;
                    GameHistoryItem gameHistoryItem = new GameHistoryItem();
                    gameHistoryItem.Init(obj, data);
                    gameHistoryItem = null;
                }
                count++;
            }
            historyListContent.gameObject.SetActive(true);
            loadImg.SetActive(false);
            nullImage.SetActive(historyNum <= 1 && historyNowCount <= 0);
            historyListContent.localPosition = new Vector3(historyListContent.localPosition.x, -1, historyListContent.localPosition.z);
        }

        private void StartGame()
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkMatch, WebRequestFuncitons.CheckMatch, true,"{}", RFrameWork.instance.token);
            UserInfoManager.returnAct = ReturnAct;
        }
        public void ReturnAct()
        {
            UserInfoManager.isLookGame = 0;
            string[] arr = new string[2] { "2", "9" };
            HorseListType type = new HorseListType(arr);
            UserInfoManager.detailPanelType = 2;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.CanPlayGame, true, JsonMapper.ToJson(type), RFrameWork.instance.token);
            UIManager.instance.CloseWnd(this);
        }

        private void ShowHistoryBtn()
        {
            gameListPanel.SetActive(false);
            myHistoryPanel.SetActive(true);
            gameListBtn.transform.GetChild(0).gameObject.SetActive(false);
            gameListBtn.GetComponent<Text>().color = new Color(0.5188679f, 0.4671568f, 0.2667764f);
            myHistoryBtn.transform.GetChild(0).gameObject.SetActive(true);
            myHistoryBtn.GetComponent<Text>().color = new Color(0.9137256f, 0.8196079f, 0.454902f);
            myHistoryBtn.GetComponent<Text>().color = new Color(0.9137256f, 0.8196079f, 0.454902f);
            NumberOfPages data = new NumberOfPages(1, 6);
            string jsonStr = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchRecordUrl, WebRequestFuncitons.GetGameHistoryData,true, jsonStr, RFrameWork.instance.token);
        }

        private void ShowGameList()
        {
            selectBtn.gameObject.SetActive(false);
            selectBtn = allBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            gameListPanel.SetActive(true);
            myHistoryPanel.SetActive(false);
            gameListBtn.transform.GetChild(0).gameObject.SetActive(true);
            gameListBtn.GetComponent<Text>().color = new Color(0.9137256f, 0.8196079f, 0.454902f);
            myHistoryBtn.transform.GetChild(0).gameObject.SetActive(false);
            myHistoryBtn.GetComponent<Text>().color = new Color(0.5188679f, 0.4671568f, 0.2667764f);
            NumberOfPages data = new NumberOfPages(1, gameCount);
            string jsonStr = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, jsonStr, RFrameWork.instance.token);
        }

        public override void OnClose()
        {
            selectBtn.gameObject.SetActive(false);
            selectBtn = allBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
        }

        public void AddItemToList()
        {
            if(status == "all")
            {
                NumberOfPages data = new NumberOfPages(1, gameCount);
                string jsonStr = JsonMapper.ToJson(data);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, jsonStr, RFrameWork.instance.token);
            }
            else if (status == "wait")
            {
                JsonData data = new JsonData();
                data["pageNum"] = 1;
                data["pageSize"] = gameCount;
                data["status"] = 0;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
            }
            else if (status == "doing")
            {
                JsonData data = new JsonData();
                data["pageNum"] = 1;
                data["pageSize"] = gameCount;
                data["status"] = 1;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
            }
            else if (status == "over")
            {
                JsonData data = new JsonData();
                data["pageNum"] = 1;
                data["pageSize"] = gameCount;
                data["status"] = 2;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchFrontUrl, WebRequestFuncitons.GetGameListData, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
            }

        }
    }
}
