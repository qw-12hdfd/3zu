using HotFix.Common;
using HotFix.Common.Utils;
using LitJson;
using MalbersAnimations.HAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HotFix.RentOutItem;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace HotFix
{

    public enum CommonDataWindowType
    {
        LeaseHorse,
        QuotaList,
        HistoryBill,
        MyRentOut,
        HorseRentOut,
    }

    public class CommonDataWindow : Window
    {
        CommonDataWindowType type;

        Button returnBtn;
        Text quotaText;
        Button tipsBtn;
        Text otherName;
        GameObject nullImage;
        GameObject loadImage;

        Transform leaseList; //我租赁的马匹
        Transform leaseContent;
        Transform leaseItem;
        Button historyBtn;

        Transform quotaList; //配额列表 带tipsBtn
        Text quotaNum;
        Text allQuotaNum;
        Transform quotaContent;
        Transform quotaItem;

        Transform historyList; //历史订单
        Transform historyContent;
        Transform historyItem;

        Transform myRentOutList; //我的出租
        Button rentOutNowBtn;
        Button moneyBtn;
        Button myRentOutHistoryBtn;
        Button allRentOutBtn;
        Button rentingBtn;
        Button quotaingBtn;
        Transform rentOutContent;
        Transform rentOutItem;

        Transform horseRentOutList; //租赁市场
        Button rentOutAllBtn;
        Button chuangShiBtn;
        Button chuanQiBtn;
        Button guiZuBtn;
        Button jingYingBtn;
        Button moneyBtn2;
        Button rentOutMyHorseBtn;
        Button myQuotaBtn;
        Button sortBtn;
        Transform horseRentOutContent;
        Transform horseRentOutItem;


        Transform selectBtn;

        int horseType = 0;
        int rentOutStatus = 0;
        public int priceSort = 0;
        int quotaPageNum = 1;
        int count = 6;
        int quotaNowCount = 0;
        int horseRentOutPageNum = 1;
        int horseRentOutNowCount = 0;
        int rentOutPageNum = 1;
        int rentOutNowCount = 0;
        int leasePageNum = 1;
        int leaseNowCount = 0;
        int historyBillPageNum = 1;
        int historyBillNowCount = 0;
        int historyType = 0;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllBtnListener();
            quotaContent.parent.parent.GetComponent<ScrollRectRef>().top = GoQuotaTop;
            quotaContent.parent.parent.GetComponent<ScrollRectRef>().bottom = GoQuotaBottom;
            horseRentOutContent.parent.parent.GetComponent<ScrollRectRef>().top = GoHorseRentOutTop;
            horseRentOutContent.parent.parent.GetComponent<ScrollRectRef>().bottom = GoHorseRentOutBottom;
            rentOutContent.parent.parent.GetComponent<ScrollRectRef>().top = GoRentOutTop;
            rentOutContent.parent.parent.GetComponent<ScrollRectRef>().bottom = GoRentOutBottom;
            leaseContent.parent.parent.GetComponent<ScrollRectRef>().top = GoLeaseTop;
            leaseContent.parent.parent.GetComponent<ScrollRectRef>().bottom = GoLeaseBottom;
            historyContent.parent.parent.GetComponent<ScrollRectRef>().top = GoHistoryTop;
            historyContent.parent.parent.GetComponent<ScrollRectRef>().bottom = GoHistoryBottom;
        }

        private void GoHistoryTop()
        {
            if (historyBillPageNum <= 1)
            {
                historyBillPageNum = 1;
            }
            else
            {
                historyBillPageNum--;
            }
            Debug.Log(historyBillNowCount + "向上翻页" + historyBillPageNum);
            JsonData data = new JsonData();
            data["pageNum"] = historyBillPageNum;
            data["pageSize"] = count;
            data["type"] = historyType;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            historyContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myHistoryRecord, GetHistoryDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GoHistoryBottom()
        {
            if (historyBillNowCount > 0 && historyBillNowCount < count)
            {
                historyBillPageNum = historyBillPageNum;
            }
            else if (historyBillNowCount >= count)
            {
                historyBillPageNum++;
            }
            Debug.Log(historyBillNowCount + "向下翻页" + historyBillPageNum);
            JsonData data = new JsonData();
            data["pageNum"] = historyBillPageNum;
            data["pageSize"] = count;
            data["type"] = historyType;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            historyContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myHistoryRecord, GetHistoryDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GetHistoryDatas(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseRentOutData[] dataArr = JsonMapper.ToObject<HorseRentOutData[]>(jsonData["data"]["list"].ToJson());
                List<HorseRentOutData> list = new List<HorseRentOutData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetMyQuotaData " + list.Count);
                var historyList = new List<HorseRentOutData>();
                if (list.Count > 0)
                {
                    historyList = list;
                    quotaNowCount = list.Count;
                }
                else if (historyBillNowCount > 1)
                {
                    historyBillPageNum--;
                }
                RefreshHistoryList(historyList);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void GoQuotaTop()
        {
            if (quotaPageNum <= 1)
            {
                quotaPageNum = 1;
            }
            else
            {
                quotaPageNum--;
            }
            Debug.Log(quotaNowCount + "向上翻页" + quotaPageNum);
            JsonData data = new JsonData();
            data["pageNum"] = quotaPageNum;
            data["pageSize"] = count;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            quotaContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.shareGetRecord, GetQuotaDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GoQuotaBottom()
        {
            if (quotaNowCount > 0 && quotaNowCount < count)
            {
                quotaPageNum = quotaPageNum;
            }
            else if (quotaNowCount >= count)
            {
                quotaPageNum++;
            }
            Debug.Log(quotaNowCount + "向下翻页" + quotaPageNum);
            JsonData data = new JsonData();
            data["pageNum"] = quotaPageNum;
            data["pageSize"] = count;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            quotaContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.shareGetRecord, GetQuotaDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GetQuotaDatas(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                QuotaData[] dataArr = JsonMapper.ToObject<QuotaData[]>(jsonData["data"]["list"].ToJson());
                List<QuotaData> list = new List<QuotaData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetMyQuotaData " + list.Count);
                var quotalist = new List<QuotaData>();
                if (list.Count > 0)
                {
                    quotalist = list;
                    quotaNowCount = list.Count;
                }
                else if (quotaPageNum > 1)
                {
                    quotaPageNum--;
                }
                RefreshQuataList(quotalist);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void GoHorseRentOutTop()
        {
            if (horseRentOutPageNum <= 1)
            {
                horseRentOutPageNum = 1;
            }
            else
            {
                horseRentOutPageNum--;
            }
            Debug.Log(horseRentOutNowCount + "向上翻页" + horseRentOutPageNum);
            JsonData data = new JsonData();
            data["pageNum"] = horseRentOutPageNum;
            data["pageSize"] = count;
            data["priceSort"] = priceSort;
            if (horseType >= 0)
                data["horseType"] = horseType;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            horseRentOutContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, GetHorseRentOutDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GoHorseRentOutBottom()
        {
            if (horseRentOutNowCount > 0 && horseRentOutNowCount < count)
            {
                horseRentOutPageNum = horseRentOutPageNum;
            }
            else if (horseRentOutNowCount >= count)
            {
                horseRentOutPageNum++;
            }
            Debug.Log(horseRentOutNowCount + "向下翻页" + horseRentOutPageNum);
            JsonData data = new JsonData();
            data["pageNum"] = horseRentOutPageNum;
            data["pageSize"] = count;
            data["priceSort"] = priceSort;
            if (horseType >= 0)
                data["horseType"] = horseType;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            horseRentOutContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, GetHorseRentOutDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GetHorseRentOutDatas(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {

                HorseRentOutData[] dataArr = JsonMapper.ToObject<HorseRentOutData[]>(jsonData["data"]["list"].ToJson());
                List<HorseRentOutData> list = new List<HorseRentOutData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetHorseRentOutDatas " + list.Count);
                var horseRentOutList = new List<HorseRentOutData>();
                if (list.Count > 0)
                {
                    horseRentOutList = list;
                    horseRentOutNowCount = list.Count;
                }
                else if (horseRentOutPageNum > 1)
                {
                    horseRentOutPageNum--;
                }
                RefreshHorseRentOutList(horseRentOutList);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void GoRentOutTop()
        {
            if (rentOutPageNum <= 1)
            {
                rentOutPageNum = 1;
            }
            else
            {
                rentOutPageNum--;
            }
            Debug.Log(rentOutNowCount + "向上翻页" + rentOutPageNum);
            JsonData data = new JsonData();
            data["pageNum"] = rentOutPageNum;
            data["pageSize"] = count;
            if (rentOutStatus >= 0)
                data["status"] = rentOutStatus;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            horseRentOutContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseMy, GetRentOutDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GoRentOutBottom()
        {
            if (rentOutNowCount > 0 && rentOutNowCount < count)
            {
                rentOutPageNum = rentOutPageNum;
            }
            else if (rentOutNowCount >= count)
            {
                rentOutPageNum++;
            }
            Debug.Log(rentOutNowCount + "向下翻页" + rentOutPageNum);
            JsonData data = new JsonData();
            data["pageNum"] = rentOutPageNum;
            data["pageSize"] = count;
            if (rentOutStatus >= 0)
                data["status"] = rentOutStatus;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            horseRentOutContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseMy, GetRentOutDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GetRentOutDatas(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {

                HorseRentOutData[] dataArr = JsonMapper.ToObject<HorseRentOutData[]>(jsonData["data"]["list"].ToJson());
                List<HorseRentOutData> list = new List<HorseRentOutData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetRentOutDatas " + list.Count);
                var rentOutList = new List<HorseRentOutData>();
                if (list.Count > 0)
                {
                    rentOutList = list;
                    rentOutNowCount = list.Count;
                }
                else if (rentOutPageNum > 1)
                {
                    rentOutPageNum--;
                }
                RefreshRentOutList(rentOutList);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void GoLeaseTop()
        {
            if (leasePageNum <= 1)
            {
                leasePageNum = 1;
            }
            else
            {
                leasePageNum--;
            }
            Debug.Log(leaseNowCount + "向上翻页" + leasePageNum);
            JsonData data = new JsonData();
            data["pageNum"] = leasePageNum;
            data["pageSize"] = count;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            horseRentOutContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myRentHorse, GetLeaseDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GoLeaseBottom()
        {
            if (leaseNowCount > 0 && leaseNowCount < count)
            {
                leasePageNum = leasePageNum;
            }
            else if (leaseNowCount >= count)
            {
                leasePageNum++;
            }
            Debug.Log(leaseNowCount + "向下翻页" + leasePageNum);
            JsonData data = new JsonData();
            data["pageNum"] = leasePageNum;
            data["pageSize"] = count;
            string jsonStr = JsonMapper.ToJson(data);
            nullImage.SetActive(false);
            loadImage.SetActive(true);
            horseRentOutContent.gameObject.SetActive(false);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myRentHorse, GetLeaseDatas, true, jsonStr, RFrameWork.instance.token);
        }

        private void GetLeaseDatas(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {

                HorseRentOutData[] dataArr = JsonMapper.ToObject<HorseRentOutData[]>(jsonData["data"]["list"].ToJson());
                List<HorseRentOutData> list = new List<HorseRentOutData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetLeaseDatas " + list.Count);
                var leaseList = new List<HorseRentOutData>();
                if (list.Count > 0)
                {
                    leaseList = list;
                    leaseNowCount = list.Count;
                }
                else if (leasePageNum > 1)
                {
                    leasePageNum--;
                }
                RefreshLeaseList(leaseList);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            horseType = -1;
            priceSort = 0;
            rentOutStatus = -1;
            quotaPageNum = 1;
            count = 6;
            quotaNowCount = 0;
            horseRentOutPageNum = 1;
            horseRentOutNowCount = 0;
            rentOutPageNum = 1;
            rentOutNowCount = 0;
            leasePageNum = 1;
            leaseNowCount = 0;
            historyBillPageNum = 1;
            historyBillNowCount = 0;
            type = (CommonDataWindowType)param1;
            ShowPanelType(param2);
            InitAllBtns();
            if (type == CommonDataWindowType.HorseRentOut)
            {
                selectBtn = rentOutAllBtn.transform.GetChild(0);
                selectBtn.gameObject.SetActive(true);
            }
            else if (type == CommonDataWindowType.MyRentOut)
            {
                selectBtn = allRentOutBtn.transform.GetChild(0);
                selectBtn.gameObject.SetActive(true);
            }
            quotaPageNum = 1;
            count = 6;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshData, true, "{}", RFrameWork.instance.token);
        }

        private void InitAllBtns()
        {
            rentOutAllBtn.transform.GetChild(0).gameObject.SetActive(false);
            allRentOutBtn.transform.GetChild(0).gameObject.SetActive(false);
            rentingBtn.transform.GetChild(0).gameObject.SetActive(false);
            quotaingBtn.transform.GetChild(0).gameObject.SetActive(false);
            chuangShiBtn.transform.GetChild(0).gameObject.SetActive(false);
            chuanQiBtn.transform.GetChild(0).gameObject.SetActive(false);
            guiZuBtn.transform.GetChild(0).gameObject.SetActive(false);
            jingYingBtn.transform.GetChild(0).gameObject.SetActive(false);
        }

        private void RefreshData(string jsonStr)
        {
            Debug.Log("RefreshData:" + jsonStr);
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                //Convert.ToDouble("111.116").ToString("N2");
                string shareAmount = jsonData["data"]["shareAmount"].ToString();
                string milletAmount = jsonData["data"]["milletAmount"].ToString();
                string totalShareAmount = jsonData["data"]["totalShareAmount"].ToString();
                string hoserFeedNumber = jsonData["data"]["hoserFeedNumber"].ToString();
                string matchPrice = jsonData["data"]["matchPrice"].ToString();
                string horseMilletNote = jsonData["data"]["horseMilletNote"].ToString();
                UserInfoManager.hoserFeedNumber = (float)Math.Round(float.Parse(hoserFeedNumber), 2);
                UserInfoManager.peiENum = (float)Math.Round(float.Parse(shareAmount), 2);
                UserInfoManager.foodNum = (float)Math.Round(float.Parse(milletAmount), 2);
                UserInfoManager.allPeiENum = (float)Math.Round(float.Parse(totalShareAmount), 2);
                UserInfoManager.matchPrice = matchPrice;
                UserInfoManager.horseMilletNote = horseMilletNote;
                if (UIManager.instance.GetWndByName(FilesName.HORSEFEEDPANEL) != null && (UIManager.instance.GetWndByName(FilesName.HORSEFEEDPANEL) as HorseFeedWindow).desText != null)
                {
                    (UIManager.instance.GetWndByName(FilesName.HORSEFEEDPANEL) as HorseFeedWindow).desText.text = UserInfoManager.horseMilletNote;
                }
                moneyBtn.transform.GetChild(0).GetComponent<Text>().text = "         " + UserInfoManager.foodNum + "       ";
                moneyBtn2.transform.GetChild(0).GetComponent<Text>().text = "         " + UserInfoManager.foodNum + "       ";
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(moneyBtn.GetComponent<RectTransform>());
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(moneyBtn2.GetComponent<RectTransform>());
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void ShowPanelType(object param2)
        {
            quotaText.gameObject.SetActive(type == CommonDataWindowType.QuotaList);
            otherName.gameObject.SetActive(type != CommonDataWindowType.QuotaList);
            leaseList.gameObject.SetActive(type == CommonDataWindowType.LeaseHorse);
            quotaList.gameObject.SetActive(type == CommonDataWindowType.QuotaList);
            historyList.gameObject.SetActive(type == CommonDataWindowType.HistoryBill);
            myRentOutList.gameObject.SetActive(type == CommonDataWindowType.MyRentOut);
            horseRentOutList.gameObject.SetActive(type == CommonDataWindowType.HorseRentOut);
            switch (type)
            {
                case CommonDataWindowType.LeaseHorse:
                    otherName.text = "我租赁的马匹";
                    var leaseList = param2 as List<HorseRentOutData>;
                    leaseNowCount = leaseList.Count;
                    RefreshLeaseList(leaseList);
                    break;
                case CommonDataWindowType.QuotaList:
                    otherName.text = "配额列表";
                    var quotalist = param2 as List<QuotaData>;
                    quotaNowCount = quotalist.Count;
                    RefreshQuataList(quotalist);
                    break;
                case CommonDataWindowType.HistoryBill:
                    otherName.text = "历史订单";
                    var historyList = param2 as List<HorseRentOutData>;
                    historyBillNowCount = historyList.Count;
                    RefreshHistoryList(historyList);
                    break;
                case CommonDataWindowType.MyRentOut:
                    otherName.text = "我的出租";
                    var rentOut = param2 as List<HorseRentOutData>;
                    rentOutNowCount = rentOut.Count;
                    RefreshRentOutList(rentOut);
                    break;
                case CommonDataWindowType.HorseRentOut:
                    otherName.text = "租赁市场";
                    var horseRentOut = param2 as List<HorseRentOutData>;
                    horseRentOutNowCount = horseRentOut.Count;
                    RefreshHorseRentOutList(horseRentOut);
                    break;
                default:
                    break;
            }
        }

        private void RefreshHistoryList(List<HorseRentOutData> list)
        {
            Debug.Log("刷新租赁市场" + historyContent.name);
            for (int i = 0; i < historyContent.childCount; i++)
            {
                historyContent.GetChild(i).gameObject.SetActive(false);
            }
            int count = 0;
            foreach (var data in list)
            {
                if (count + 1 <= historyContent.childCount)
                {
                    HistoryBillItem item = new HistoryBillItem();
                    item.OnInit(historyContent.GetChild(count), data);
                    item = null;
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(this.historyItem.gameObject, historyContent).transform;
                    HistoryBillItem item = new HistoryBillItem();
                    item.OnInit(obj, data);
                    item = null;
                }
                count++;
            }
            nullImage.SetActive(historyBillPageNum <= 1 && historyBillNowCount <= 0);
            loadImage.SetActive(false);
            historyContent.gameObject.SetActive(true);
            historyContent.localPosition = new Vector3(historyContent.localPosition.x, -1, historyContent.localPosition.z);
        }

        private void RefreshHorseRentOutList(List<HorseRentOutData> list)
        {
            sortBtn.transform.GetChild(0).gameObject.SetActive(priceSort == 0);
            sortBtn.transform.GetChild(1).gameObject.SetActive(priceSort == 1);
            Debug.Log("刷新租赁市场" + horseRentOutContent.name);
            for (int i = 0; i < horseRentOutContent.childCount; i++)
            {
                horseRentOutContent.GetChild(i).gameObject.SetActive(false);
            }
            int count = 0;
            foreach (var data in list)
            {
                if (count + 1 <= horseRentOutContent.childCount)
                {
                    HorseRentOutItem item = new HorseRentOutItem();
                    item.OnInit(horseRentOutContent.GetChild(count), data);
                    item = null;
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(this.horseRentOutItem.gameObject, horseRentOutContent).transform;
                    HorseRentOutItem item = new HorseRentOutItem();
                    item.OnInit(obj, data);
                    item = null;
                }
                count++;
            }
            nullImage.SetActive(horseRentOutPageNum <= 1 && horseRentOutNowCount <= 0);
            loadImage.SetActive(false);
            horseRentOutContent.gameObject.SetActive(true);
            horseRentOutContent.localPosition = new Vector3(horseRentOutContent.localPosition.x, -1, horseRentOutContent.localPosition.z);
        }
        List<LeaseItem> leaseItemList = new List<LeaseItem>();
        private void RefreshLeaseList(List<HorseRentOutData> list)
        {
            Debug.Log("刷新我租赁的马匹" + leaseContent.name);
            for (int i = 0; i < leaseContent.childCount; i++)
            {
                leaseContent.GetChild(i).gameObject.SetActive(false);
            }
            IEnumeratorTool.instance.StopAllCoroutines();
            leaseItemList.Clear();
            leaseItemList = new List<LeaseItem>();
            int count = 0;
            foreach (var data in list)
            {
                if (count + 1 <= leaseContent.childCount)
                {
                    LeaseItem item = new LeaseItem();
                    item.OnInit(leaseContent.GetChild(count), data);
                    leaseItemList.Add(item);
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(this.leaseItem.gameObject, leaseContent).transform;
                    LeaseItem item = new LeaseItem();
                    item.OnInit(obj, data);
                    leaseItemList.Add(item);
                }
                count++;
            }
            nullImage.SetActive(leasePageNum <= 1 && leaseNowCount <= 0);
            loadImage.SetActive(false);
            leaseContent.gameObject.SetActive(true);
            leaseContent.localPosition = new Vector3(leaseContent.localPosition.x, -1, leaseContent.localPosition.z);
        }
        List<RentOutItem> rentOutItemList = new List<RentOutItem>();
        private void RefreshRentOutList(List<HorseRentOutData> list)
        {
            Debug.Log("刷新我的挂租" + rentOutContent.name);
            for (int i = 0; i < rentOutContent.childCount; i++)
            {
                rentOutContent.GetChild(i).gameObject.SetActive(false);
            }
            IEnumeratorTool.instance.StopAllCoroutines();
            rentOutItemList.Clear();
            rentOutItemList = new List<RentOutItem>();
            int count = 0;
            foreach (var data in list)
            {
                if (count + 1 <= rentOutContent.childCount)
                {
                    RentOutItem item = new RentOutItem();
                    item.OnInit(rentOutContent.GetChild(count), data);
                    rentOutItemList.Add(item);
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(this.rentOutItem.gameObject, rentOutContent).transform;
                    RentOutItem item = new RentOutItem();
                    item.OnInit(obj, data);
                    rentOutItemList.Add(item);
                }
                count++;
            }
            Debug.Log(rentOutPageNum + "     " + rentOutNowCount);
            nullImage.SetActive(rentOutPageNum <= 1 && rentOutNowCount <= 0);
            loadImage.SetActive(false);
            rentOutContent.gameObject.SetActive(true);
            rentOutContent.localPosition = new Vector3(rentOutContent.localPosition.x, -1, rentOutContent.localPosition.z);
        }

        private void RefreshQuataList(List<QuotaData> list)
        {
            quotaNum.text = "   剩余配额：" + UserInfoManager.peiENum + "   ";
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(quotaNum.transform.parent.GetComponent<RectTransform>());
            allQuotaNum.text = "   总获取配额：" + UserInfoManager.allPeiENum + "   ";
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(allQuotaNum.transform.parent.GetComponent<RectTransform>());
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(allQuotaNum.transform.parent.parent.GetComponent<RectTransform>());
            for (int i = 0; i < quotaContent.childCount; i++)
            {
                quotaContent.GetChild(i).gameObject.SetActive(false);
            }
            int count = 0;
            foreach (var data in list)
            {
                if (count + 1 <= quotaContent.childCount)
                {
                    QuotaItem item = new QuotaItem();
                    item.OnInit(quotaContent.GetChild(count), data);
                    item = null;
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(this.quotaItem.gameObject, quotaContent).transform;
                    QuotaItem item = new QuotaItem();
                    item.OnInit(obj, data);
                    item = null;
                }
                count++;
            }
            nullImage.SetActive(quotaPageNum <= 1 && quotaNowCount <= 0);
            loadImage.SetActive(false);
            quotaContent.gameObject.SetActive(true);
            quotaContent.localPosition = new Vector3(quotaContent.localPosition.x, -1, quotaContent.localPosition.z);
        }

        private void GetAllComponent()
        {
            returnBtn = m_Transform.Find("QuitBtn").GetComponent<Button>();
            tipsBtn = m_Transform.Find("QuitBtn/QuotaText/TipsBtn").GetComponent<Button>();
            quotaText = m_Transform.Find("QuitBtn/QuotaText").GetComponent<Text>();
            otherName = m_Transform.Find("QuitBtn/OtherText").GetComponent<Text>();
            nullImage = m_Transform.Find("Null").gameObject;
            loadImage = m_Transform.Find("Image").gameObject;

            leaseList = m_Transform.Find("LeaseHorseList");
            leaseContent = m_Transform.Find("LeaseHorseList/Viewport/Content");
            leaseItem = m_Transform.Find("LeaseHorseList/Viewport/Content/Item");
            historyBtn = m_Transform.Find("LeaseHorseList/HirtoryBtn").GetComponent<Button>();

            quotaList = m_Transform.Find("QuotaList");
            quotaNum = m_Transform.Find("QuotaList/Datas/QuotaNum/Text").GetComponent<Text>();
            allQuotaNum = m_Transform.Find("QuotaList/Datas/Num/Text").GetComponent<Text>();
            quotaContent = m_Transform.Find("QuotaList/Viewport/Content");
            quotaItem = m_Transform.Find("QuotaList/Viewport/Content/Item");

            historyList = m_Transform.Find("HistoryList");
            historyContent = m_Transform.Find("HistoryList/Viewport/Content");
            historyItem = m_Transform.Find("HistoryList/Viewport/Content/Item");

            myRentOutList = m_Transform.Find("MyRentOutList");
            rentOutNowBtn = m_Transform.Find("MyRentOutList/RightBtns/RentOutBtn").GetComponent<Button>();
            moneyBtn = m_Transform.Find("MyRentOutList/RightBtns/Money").GetComponent<Button>();
            myRentOutHistoryBtn = m_Transform.Find("MyRentOutList/HirtoryBtn").GetComponent<Button>();
            allRentOutBtn = m_Transform.Find("MyRentOutList/Btns/All").GetComponent<Button>();
            rentingBtn = m_Transform.Find("MyRentOutList/Btns/Doing").GetComponent<Button>();
            quotaingBtn = m_Transform.Find("MyRentOutList/Btns/Quota").GetComponent<Button>();
            rentOutContent = m_Transform.Find("MyRentOutList/Viewport/Content");
            rentOutItem = m_Transform.Find("MyRentOutList/Viewport/Content/Item");

            horseRentOutList = m_Transform.Find("HorseRentOutList");
            rentOutAllBtn = m_Transform.Find("HorseRentOutList/Btns/All").GetComponent<Button>();
            chuangShiBtn = m_Transform.Find("HorseRentOutList/Btns/ChuangShi").GetComponent<Button>();
            chuanQiBtn = m_Transform.Find("HorseRentOutList/Btns/ChuanQi").GetComponent<Button>();
            guiZuBtn = m_Transform.Find("HorseRentOutList/Btns/GuiZu").GetComponent<Button>();
            jingYingBtn = m_Transform.Find("HorseRentOutList/Btns/JingYing").GetComponent<Button>();
            moneyBtn2 = m_Transform.Find("HorseRentOutList/RightBtns/Money").GetComponent<Button>();
            rentOutMyHorseBtn = m_Transform.Find("HorseRentOutList/RightBtns/RentOutBtn").GetComponent<Button>();
            myQuotaBtn = m_Transform.Find("HorseRentOutList/RightBtns/RentOutHorseBtn").GetComponent<Button>();
            sortBtn = m_Transform.Find("HorseRentOutList/SortBtn").GetComponent<Button>();
            horseRentOutContent = m_Transform.Find("HorseRentOutList/Viewport/Content");
            horseRentOutItem = m_Transform.Find("HorseRentOutList/Viewport/Content/Item");
        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(returnBtn, ReturnFunc);
            AddButtonClickListener(tipsBtn, TipsFunc);
            AddButtonClickListener(historyBtn, () => { HistoryFunc(1); });
            AddButtonClickListener(rentOutNowBtn, RentOutNowFunc);
            AddButtonClickListener(moneyBtn, () => { MoneyFunc(moneyBtn.transform); });
            AddButtonClickListener(myRentOutHistoryBtn, () => { HistoryFunc(0); });
            AddButtonClickListener(allRentOutBtn, AllRentOutFunc);
            AddButtonClickListener(rentingBtn, RentingFunc);
            AddButtonClickListener(quotaingBtn, QuotaingFunc);
            AddButtonClickListener(rentOutAllBtn, RentOutAllFunc);
            AddButtonClickListener(chuangShiBtn, ChangeShiFunc);
            AddButtonClickListener(chuanQiBtn, ChuanQiFunc);
            AddButtonClickListener(guiZuBtn, GuiZuiFunc);
            AddButtonClickListener(jingYingBtn, JingYingFunc);
            AddButtonClickListener(moneyBtn2, () => { MoneyFunc(moneyBtn2.transform); });
            AddButtonClickListener(rentOutMyHorseBtn, RentOutMyHorseFunc);
            AddButtonClickListener(myQuotaBtn, MyQuotaFunc);
            AddButtonClickListener(sortBtn, SortFunc);
        }

        private void ReturnFunc()
        {
            if (otherName.text.Equals("租赁市场"))
            {
                if (UserInfoManager.noHorse)
                    RFrameWork.instance.OpenCommonConfirm("提示", "暂无马匹，是否退出元宇宙？", () => { ToolManager.ExitGame(); }, () => { });
                else
                    UIManager.instance.CloseWnd(this);
            }
            else
            {
                if (otherName.text.Equals("历史订单"))
                {
                    if(panelLayer == 1)
                    {
                        RentOutMyHorseFunc();
                    }
                    else
                    {
                        MyQuotaFunc();
                    }
                }
                else if (otherName.text.Equals("配额列表"))
                {
                    if(!UserInfoManager.noHorse)
                        UIManager.instance.CloseWnd(this);
                    else
                    {
                        JsonData json = new JsonData();
                        json["pageNum"] = 1;
                        json["pageSize"] = 6;
                        json["priceSort"] = priceSort;
                        WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, WebRequestFuncitons.GetRentHorseList, true, JsonMapper.ToJson(json), RFrameWork.instance.token);
                    }
                }
                else
                {
                    JsonData json = new JsonData();
                    json["pageNum"] = 1;
                    json["pageSize"] = 6;
                    json["priceSort"] = priceSort;
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, WebRequestFuncitons.GetRentHorseList, true, JsonMapper.ToJson(json), RFrameWork.instance.token);
                }
            }
        }

        private void TipsFunc()
        {
            //显示配额列表的配额说明
            ListFront data;
            data = new ListFront("horse_share_config");
            string jsonStr = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFront, WebRequestFuncitons.GetHorseShareConfigFront, true, jsonStr, RFrameWork.instance.token);
        }

        private void RentOutNowFunc()
        {
            UserInfoManager.detailPanelType = 9;
            JsonData data = new JsonData();
            data["isRent"] = 1;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.CanRentOut, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void HistoryFunc(int type)
        {
            historyType = type;
            Debug.Log("HistoryFunc  " + historyType);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = 6;
            data["type"] = historyType;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myHistoryRecord, WebRequestFuncitons.GetHistoryRecord, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void AllRentOutFunc()
        {
            rentOutPageNum = 1;
            rentOutNowCount = 0;
            rentOutStatus = -1;
            selectBtn.gameObject.SetActive(false);
            selectBtn = allRentOutBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = 6;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseMy, WebRequestFuncitons.GetMyRentOutHorseList, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void RentingFunc()
        {
            rentOutPageNum = 1;
            rentOutNowCount = 0;
            rentOutStatus = 0;
            selectBtn.gameObject.SetActive(false);
            selectBtn = rentingBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = 6;
            data["status"] = rentOutStatus;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseMy, GetRentOutDatas, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void QuotaingFunc()
        {
            rentOutPageNum = 1;
            rentOutNowCount = 0;
            rentOutStatus = 1;
            selectBtn.gameObject.SetActive(false);
            selectBtn = quotaingBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = 6;
            data["status"] = rentOutStatus;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseMy, GetRentOutDatas, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }



        private void RentOutAllFunc()
        {
            horseRentOutPageNum = 1;
            horseRentOutNowCount = 0;
            horseType = -1;
            selectBtn.gameObject.SetActive(false);
            selectBtn = rentOutAllBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = count;
            data["priceSort"] = priceSort;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, GetHorseRentOutDatas, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void ChangeShiFunc()
        {
            horseRentOutPageNum = 1;
            horseRentOutNowCount = 0;
            horseType = 0;
            selectBtn.gameObject.SetActive(false);
            selectBtn = chuangShiBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = count;
            data["priceSort"] = priceSort;
            data["horseType"] = horseType;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, GetHorseRentOutDatas, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void ChuanQiFunc()
        {
            horseRentOutPageNum = 1;
            horseRentOutNowCount = 0;
            horseType = 1;
            selectBtn.gameObject.SetActive(false);
            selectBtn = chuanQiBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = count;
            data["priceSort"] = priceSort;
            data["horseType"] = horseType;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, GetHorseRentOutDatas, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void GuiZuiFunc()
        {
            horseRentOutPageNum = 1;
            horseRentOutNowCount = 0;
            horseType = 2;
            selectBtn.gameObject.SetActive(false);
            selectBtn = guiZuBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = count;
            data["priceSort"] = priceSort;
            data["horseType"] = horseType;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, GetHorseRentOutDatas, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void JingYingFunc()
        {
            horseRentOutPageNum = 1;
            horseRentOutNowCount = 0;
            horseType = 3;
            selectBtn.gameObject.SetActive(false);
            selectBtn = jingYingBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = count;
            data["priceSort"] = priceSort;
            data["horseType"] = horseType;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, GetHorseRentOutDatas, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void MoneyFunc(Transform money)
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshData, true, "{}", RFrameWork.instance.token);
            UIManager.instance.PopUpWnd(FilesName.HORSEFEEDPANEL, true, false, money);
        }
        int panelLayer = 0;
        public void RentOutMyHorseFunc()
        {
            panelLayer = 1;
            selectBtn.gameObject.SetActive(false);
            selectBtn = allRentOutBtn.transform.GetChild(0);
            selectBtn.gameObject.SetActive(true);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = 6;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseMy, WebRequestFuncitons.GetMyRentOutHorseList, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        public void MyQuotaFunc()
        {
            panelLayer = 2;
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = 6;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myRentHorse, WebRequestFuncitons.GetMyRentHorseList, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void SortFunc()
        {
            if (priceSort == 0)
                priceSort = 1;
            else
                priceSort = 0;
            sortBtn.transform.GetChild(0).gameObject.SetActive(priceSort == 0);
            sortBtn.transform.GetChild(1).gameObject.SetActive(priceSort == 1);
            JsonData data = new JsonData();
            data["pageNum"] = horseRentOutPageNum;
            data["pageSize"] = count;
            data["priceSort"] = priceSort;
            if(horseType>=0)
            data["horseType"] = horseType;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, GetHorseRentOutDatas, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        public override void OnClose()
        {
        }
    }

    internal class QuotaItem
    {
        private Transform item;
        private Text name;
        private Text num;
        private RawImage icon;
        QuotaData data;
        public void OnInit(Transform itemTrans, QuotaData data)
        {
            item = itemTrans;
            name = item.Find("Title").GetComponent<Text>();
            num = item.Find("Des/DesNum").GetComponent<Text>();
            icon = item.Find("RawImage").GetComponent<RawImage>();
            item.gameObject.SetActive(true);
            this.data = data;
            UpdateUI();
        }

        private void UpdateUI()
        {
            name.text = data.name;
            num.text = NumUtils.FormatCurrency(float.Parse(data.amount), 2);
            WebRequestManager.instance.AsyncLoadUnityTexture(data.pic, (texture) =>
            {
                icon.texture = texture;
            });
        }
    }

    internal class HorseRentOutItem
    {
        private Transform item;
        private Text name;
        private Text num;
        private RawImage icon;
        private Button horseDetail;
        private Button rentOutBtn;
        HorseRentOutData data;
        public void OnInit(Transform itemTrans, HorseRentOutData data)
        {
            item = itemTrans;
            this.data = data;
            name = item.Find("Title").GetComponent<Text>();
            num = item.Find("Des/DesNum").GetComponent<Text>();
            icon = item.Find("RawImage").GetComponent<RawImage>();
            horseDetail = item.Find("Btn").GetComponent<Button>();
            rentOutBtn = item.Find("RentOut").GetComponent<Button>();
            horseDetail.onClick.RemoveAllListeners();
            rentOutBtn.onClick.RemoveAllListeners();
            horseDetail.onClick.AddListener(() =>
            {
                UserInfoManager.rentOutPrice = data.price;
                UserInfoManager.detailPanelType = 1;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + data.horseId, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            });
            rentOutBtn.onClick.AddListener(() =>
            {
                UserInfoManager.horseRentOutId = data.id;
                UserInfoManager.detailPanelType = 8;
                UserInfoManager.rentOutPrice = data.price;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + data.horseId, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            });
            item.gameObject.SetActive(true);
            UpdateUI();
        }

        private void UpdateUI()
        {
            name.text = data.horseName;
            num.text = NumUtils.FormatCurrency(float.Parse(data.price), 2);
            num.transform.GetChild(0).GetComponent<Text>().text = "马粟/" + UserInfoManager.RentOutTime + "小时";
            WebRequestManager.instance.AsyncLoadUnityTexture(data.pic, (texture) =>
            {
                icon.texture = texture;
            });
        }
    }

    internal class RentOutItem
    {
        private Transform item;
        private Text name;
        private Text num;
        private RawImage icon;
        private Button horseDetail;
        private Button changePrice;
        private Button putBtn;
        private Image back;
        private Text statuText;
        private Text updateText;
        HorseRentOutData data;
        public void OnInit(Transform itemTrans, HorseRentOutData data)
        {
            item = itemTrans;
            this.data = data;
            name = item.Find("Title").GetComponent<Text>();
            num = item.Find("Des/DesNum").GetComponent<Text>();
            icon = item.Find("RawImage").GetComponent<RawImage>();
            horseDetail = item.Find("Btn").GetComponent<Button>();
            changePrice = item.Find("ChangePrice").GetComponent<Button>();
            putBtn = item.Find("Return").GetComponent<Button>();
            back = item.Find("Back").GetComponent<Image>();
            statuText = item.Find("Back/Text").GetComponent<Text>();
            updateText = item.Find("RawImage/CountDownBack/CountDownText").GetComponent<Text>();
            horseDetail.onClick.RemoveAllListeners();
            changePrice.onClick.RemoveAllListeners();
            putBtn.onClick.RemoveAllListeners();
            horseDetail.onClick.AddListener(() =>
            {
                UserInfoManager.detailPanelType = 1;
                UserInfoManager.horseTexture = data.pic;
                UserInfoManager.horseRentOutName = data.horseName;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + data.horseId, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            });
            changePrice.onClick.AddListener(() =>
            {
                //修改价格
                //确认出租
                UserInfoManager.horseTexture = data.pic;
                UserInfoManager.horseRentOutName = data.horseName;
                UIManager.instance.PopUpWnd(FilesName.RENTPANEL, true, false, data.id, "修改租赁费");
            });
            putBtn.onClick.AddListener(() =>
            {
                //取回recycleHorse
                RFrameWork.instance.OpenCommonConfirm("提示", "是否撤租马匹？", () => {
                    JsonData jsondata = new JsonData();
                    jsondata["id"] = data.id;
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.recycleHorse, RecycleFunc, true, JsonMapper.ToJson(jsondata), RFrameWork.instance.token);
                }, () => { });
            });
            item.gameObject.SetActive(true);
            UpdateUI();
        }

        private void RecycleFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string status = jsonData["data"]["status"].ToString();
                string remark = jsonData["data"]["remark"].ToString();
                if (status.Equals("0"))
                {
                    RFrameWork.instance.OpenCommonConfirm("撤销失败", remark, () => { }, null);
                }
                else
                {
                    RFrameWork.instance.OpenCommonConfirm("撤销成功", "您可在喂马场查看马匹，或重新挂租。", () => {
                        UIManager.instance.CloseWnd(FilesName.COMMONDATAPANEL);
                        JsonData data = new JsonData();
                        data["pageNum"] = 1;
                        data["pageSize"] = 6;
                        WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseMy, WebRequestFuncitons.GetMyRentOutHorseList, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
                    }, null);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("撤销失败", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }
        string str;
        private void UpdateUI()
        {
            name.text = data.horseName;
            num.text = NumUtils.FormatCurrency(float.Parse(data.price), 2);
            num.transform.GetChild(0).GetComponent<Text>().text = "马粟/" + UserInfoManager.RentOutTime + "小时";
            WebRequestManager.instance.AsyncLoadUnityTexture(data.pic, (texture) =>
            {
                icon.texture = texture;
            });
            if (data.status.Equals("0")) //出租
            {
                back.color = new Color(0.9176471f, 0.8156863f, 0.6f, 0.33f);
                statuText.color = new Color(0.9176471f, 0.8156863f, 0.6f, 1);
                statuText.text = " 出租中 ";
                horseDetail.transform.localPosition = new Vector3(-48.55f, -84, 0);
                changePrice.gameObject.SetActive(true);
                putBtn.gameObject.SetActive(true);
                str = "距可撤销 ";

            }
            else if (data.status.Equals("1")) //租赁
            {
                back.color = new Color(0.9176471f, 0.8156863f, 0.6f, 1f);
                statuText.color = new Color(0.4509804f, 0.3137255f, 0.172549f, 1);
                statuText.text = " 租赁中 ";
                horseDetail.transform.localPosition = new Vector3(366.67f, -84, 0);
                changePrice.gameObject.SetActive(false);
                putBtn.gameObject.SetActive(false);
                str = "倒计时 ";
            }
            else if (data.status.Equals("2")) //已回收
            {

            }
            putBtn.GetComponent<Image>().color = string.IsNullOrEmpty(data.countdown) ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.33f);
            putBtn.enabled = string.IsNullOrEmpty(data.countdown) ? true : false;
            updateText.transform.parent.gameObject.SetActive(string.IsNullOrEmpty(data.countdown) ? false : true);
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(back.GetComponent<RectTransform>());
            if (!string.IsNullOrEmpty(data.countdown))
            {
                IEnumeratorTool.instance.StartCoroutineNew(CountDownFunc1());
            }
        }
        

        public IEnumerator CountDownFunc1()
        {
            DateTime startTime = DateTime.Now;
            startTime = startTime.AddSeconds(double.Parse(data.countdown) / 1000);
            while (true)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan span = nowTime.Subtract(startTime).Duration();
                updateText.text = str + (span.Hours > 0 ? span.Hours.ToString() : "0") + ":" + (span.Minutes > 0 ? span.Minutes.ToString() : "0") + ":" + (span.Seconds > 0 ? span.Seconds.ToString() : "0");
                if (TimeUtils.OnDiffSeconds(startTime, nowTime) > -0.1f)
                {
                    Debug.Log("倒计时结束了");
                    data.countdown = "";
                    putBtn.GetComponent<Image>().color = string.IsNullOrEmpty(data.countdown) ? new Color(0.9176471f, 0.8156863f, 0.6f, 1) : new Color(0.9176471f, 0.8156863f, 0.6f, 0.33f);
                    putBtn.enabled = string.IsNullOrEmpty(data.countdown) ? true : false;
                    updateText.transform.parent.gameObject.SetActive(string.IsNullOrEmpty(data.countdown) ? false : true);
                    //if (UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL) != null)
                    //    (UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL) as CommonDataWindow).RentOutMyHorseFunc();
                    break;
                }
                yield return new WaitForSecondsRealtime(1f);
            }
            yield break;
        }
    }

    internal class LeaseItem
    {
        private Transform item;
        private Text name;
        private Text num;
        private RawImage icon;
        private Button horseDetail;
        private Image back;
        private Text statuText;
        private Text updateText;
        HorseRentOutData data;
        public void OnInit(Transform itemTrans, HorseRentOutData data)
        {
            item = itemTrans;
            this.data = data;
            name = item.Find("Title").GetComponent<Text>();
            num = item.Find("Des/DesNum").GetComponent<Text>();
            icon = item.Find("RawImage").GetComponent<RawImage>();
            horseDetail = item.Find("Btn").GetComponent<Button>();
            back = item.Find("Back").GetComponent<Image>();
            statuText = item.Find("Back/Text").GetComponent<Text>();
            updateText = item.Find("RawImage/CountDownBack/CountDownText").GetComponent<Text>();
            horseDetail.onClick.RemoveAllListeners();
            horseDetail.onClick.AddListener(() =>
            {
                UserInfoManager.detailPanelType = 1;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + data.horseId, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            });
            item.gameObject.SetActive(true);
            UpdateUI();
        }

        private void UpdateUI()
        {
            name.text = data.horseName;
            num.text = NumUtils.FormatCurrency(float.Parse(data.price), 2);
            num.transform.GetChild(0).GetComponent<Text>().text = "马粟/" + UserInfoManager.RentOutTime + "小时";
            WebRequestManager.instance.AsyncLoadUnityTexture(data.pic, (texture) =>
            {
                icon.texture = texture;
            });
            if (data.status.Equals("0")) //出租
            {
                back.color = new Color(0.9176471f, 0.8156863f, 0.6f, 0.33f);
                statuText.color = new Color(0.9176471f, 0.8156863f, 0.6f, 1);

                statuText.text = " 出租中 ";

            }
            else if (data.status.Equals("1")) //租赁
            {
                back.color = new Color(0.9176471f, 0.8156863f, 0.6f, 1f);
                statuText.color = new Color(0.4509804f, 0.3137255f, 0.172549f, 1);
                statuText.text = " 租赁中 ";
            }
            else if (data.status.Equals("2")) //已回收
            {

            }
            updateText.transform.parent.gameObject.SetActive(string.IsNullOrEmpty(data.countdown) ? false : true);
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(back.GetComponent<RectTransform>());
            if (!string.IsNullOrEmpty(data.countdown))
            {
                IEnumeratorTool.instance.StartCoroutineNew(CountDownFunc());
            }
        }

        public IEnumerator CountDownFunc()
        {
            DateTime startTime = DateTime.Now;
            startTime = startTime.AddSeconds(double.Parse(data.countdown) / 1000);
            while (true)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan span = nowTime.Subtract(startTime).Duration();
                updateText.text = "倒计时 " + (span.Hours > 0 ? span.Hours.ToString() : "0") + ":" + (span.Minutes > 0 ? span.Minutes.ToString() : "0") + ":" + (span.Seconds > 0 ? span.Seconds.ToString() : "0");
                if (TimeUtils.OnDiffSeconds(startTime, nowTime) > -0.1f)
                {
                    Debug.Log("倒计时结束了");
                    //if (UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL)!=null)
                    //(UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL) as CommonDataWindow).MyQuotaFunc();
                    data.countdown = "";
                    updateText.transform.parent.gameObject.SetActive(string.IsNullOrEmpty(data.countdown) ? false : true);
                    break;
                }
                yield return new WaitForSecondsRealtime(1f);
            }
            yield break;
        }
    }

    internal class HistoryBillItem
    {
        private Transform item;
        private Text name;
        private Text num;
        private RawImage icon;
        private Button horseDetail;
        HorseRentOutData data;
        public void OnInit(Transform itemTrans, HorseRentOutData data)
        {
            item = itemTrans;
            this.data = data;
            name = item.Find("Title").GetComponent<Text>();
            num = item.Find("Des/DesNum").GetComponent<Text>();
            icon = item.Find("RawImage").GetComponent<RawImage>();
            horseDetail = item.Find("Btn").GetComponent<Button>();
            horseDetail.onClick.RemoveAllListeners();
            horseDetail.onClick.AddListener(() =>
            {
                UserInfoManager.detailPanelType = 1;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + data.horseId, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            });
            item.gameObject.SetActive(true);
            UpdateUI();
        }

        private void UpdateUI()
        {
            name.text = data.horseName;
            num.text = NumUtils.FormatCurrency(float.Parse(data.price), 2);
            num.transform.GetChild(0).GetComponent<Text>().text = "马粟/" + UserInfoManager.RentOutTime + "小时";
            WebRequestManager.instance.AsyncLoadUnityTexture(data.pic, (texture) =>
            {
                icon.texture = texture;
            });
        }
    }
}
