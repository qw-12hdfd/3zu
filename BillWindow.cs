using HotFix.Common.Utils;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HotFix
{
    internal class BillWindow : Window
    {
        private Button returnBtn;
        private Button feedBtn;
        private Text FeedNum;
        private Transform content;
        private Transform item;
        List<BillData> list = new List<BillData>();
        int num = 1;
        int count = 6;
        int nowCount = 0;
        GameObject loadImg;
        GameObject nullImg;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllBtnListener();
            content.parent.parent.GetComponent<ScrollRectRef>().top = GoTop;
            content.parent.parent.GetComponent<ScrollRectRef>().bottom = GoBottom;
        }

        private void GoTop()
        {
            if(num <= 1)
            {
                num = 1;
            }
            else
            {
                num--;
            }
            Debug.Log(nowCount + "向上翻页" + num);
            IEnumeratorTool.instance.StartCoroutineNew(RefreshItems());
        }

        private IEnumerator RefreshItems()
        {
            loadImg.SetActive(true);
            nullImg.SetActive(false);
            content.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            JsonData data = new JsonData();
            data["pageNum"] = num;
            data["pageSize"] = count;
            string jsonStr = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.jourMyPage, GetMyBillData, true, jsonStr, RFrameWork.instance.token);
        }

        private void GoBottom()
        {
            if (nowCount>0&&nowCount< count)
            {
                num = num;
            }
            else if(nowCount>=count)
            {
                num++;
            }
            Debug.Log(nowCount+"向下翻页" + num);
            IEnumeratorTool.instance.StartCoroutineNew(RefreshItems());
        }

        private void GetMyBillData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                BillData[] dataArr = JsonMapper.ToObject<BillData[]>(jsonData["data"]["list"].ToJson());
                List<BillData> list = new List<BillData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetMyBillData " + list.Count);
                if (list.Count > 0)
                {
                    this.list = list;
                    nowCount = list.Count;
                }
                else if(num>1)
                {
                    num--;
                }
                
                UpdateUI();
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
            loadImg.SetActive(false);
            content.gameObject.SetActive(true);
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            list = param1 as List<BillData>;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshNumData, true, "{}", RFrameWork.instance.token);
            num = 1;
            count = 6;
            nowCount = list.Count;
            UpdateUI();
        }

        private void GetAllComponent()
        {
            returnBtn = m_Transform.Find("ReturnBtn").GetComponent<Button>();
            FeedNum = m_Transform.Find("Money/Text").GetComponent<Text>();
            feedBtn = m_Transform.Find("Money").GetComponent<Button>();
            content = m_Transform.Find("Scroll View/Viewport/Content");
            item = m_Transform.Find("Scroll View/Viewport/Content/Item");
            loadImg = m_Transform.Find("Scroll View/Viewport/Image").gameObject;
            nullImg = m_Transform.Find("Scroll View/Viewport/Null").gameObject;
        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(returnBtn, ReturnFunc);
            AddButtonClickListener(feedBtn, YuBeiFunc);
        }



        private void UpdateUI()
        {
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
            int count = 0;
            foreach (var data in list)
            {
                if (count + 1 <= content.childCount)
                {
                    BillItem item = new BillItem();
                    item.OnInit(content.GetChild(count), data);
                    item = null;
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(this.item.gameObject, content).transform;
                    BillItem item = new BillItem();
                    item.OnInit(obj, data);
                    item = null;
                }
                count++;
            }
            content.localPosition = new Vector3(content.localPosition.x, -1, content.localPosition.z);
            nullImg.SetActive(list.Count <= 0 && num <= 1);
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
                FeedNum.text = "         " + UserInfoManager.foodNum + "       ";
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(FeedNum.transform.parent.GetComponent<RectTransform>());
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void ReturnFunc()
        {
            UIManager.instance.CloseWnd(this);
        }

        private void YuBeiFunc()
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshNumData, true, "{}", RFrameWork.instance.token);
            UIManager.instance.PopUpWnd(FilesName.HORSEFEEDPANEL, true, false, feedBtn.transform);
        }
    }
}
