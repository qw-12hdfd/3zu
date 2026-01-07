using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    internal class HorseFeedWindow:Window
    {
        private Button closeBtn;
        private Button moneyBtn;
        private Button buyBtn;
        private Button addBtn;
        private Button detailBtn;
        public Text desText;
        private Text moneyText;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllButtonListener();
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            Transform parent = param1 as Transform;
            m_Transform.position = parent.position;
            m_Transform.SetAsLastSibling();
            UpdateUI();
        }

        private void UpdateUI()
        {
            moneyText.text = UserInfoManager.peiENum.ToString();
            desText.text = UserInfoManager.horseMilletNote;
        }

        private void GetAllComponent()
        {
            closeBtn = m_Transform.Find("CloseBtn").GetComponent<Button>();
            moneyBtn = m_Transform.Find("BackImg/Btns/Money").GetComponent<Button>();
            buyBtn = m_Transform.Find("BackImg/Btns/Buy").GetComponent<Button>();
            addBtn = m_Transform.Find("BackImg/Btns/Add").GetComponent<Button>();
            detailBtn = m_Transform.Find("BackImg/Btns/Detail").GetComponent<Button>();
            desText = m_Transform.Find("BackImg/Des").GetComponent<Text>();
            moneyText = m_Transform.Find("BackImg/Btns/Money/Des").GetComponent<Text>();
        }

        private void AddAllButtonListener()
        {
            AddButtonClickListener(closeBtn, ClosePanel);
            AddButtonClickListener(moneyBtn, QuotaPanel);
            AddButtonClickListener(buyBtn, BuyHorseFeed);
            AddButtonClickListener(addBtn, AddHorseFeed);
            AddButtonClickListener(detailBtn, DetailBill);
        }

        private void ClosePanel()
        {
            UIManager.instance.CloseWnd(this);
        }

        private void QuotaPanel()
        {
            //配额界面 TODO
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshData, true, "{}", RFrameWork.instance.token);
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
                UserInfoManager.hoserFeedNumber = (float)Math.Round(float.Parse(hoserFeedNumber), 2);
                UserInfoManager.peiENum = (float)Math.Round(float.Parse(shareAmount), 2);
                UserInfoManager.foodNum = (float)Math.Round(float.Parse(milletAmount), 2);
                UserInfoManager.allPeiENum = (float)Math.Round(float.Parse(totalShareAmount), 2);
                JsonData data = new JsonData();
                data["pageNum"] = 1;
                data["pageSize"] = 6;
                string str = JsonMapper.ToJson(data);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.shareGetRecord, WebRequestFuncitons.GetQuotaDatas, true, str, RFrameWork.instance.token);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void BuyHorseFeed()
        {
            //买马粟
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, WebRequestFuncitons.GetMilletDetail, true, "{}", RFrameWork.instance.token);
        }

        private void AddHorseFeed()
        {
            //赠送马粟
            UIManager.instance.PopUpWnd(FilesName.ADDFEEDPANEL, true, false);
        }

        private void DetailBill()
        {
            //明细界面
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount,MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = 6;
            data["type"] =1;
            string jsonStr = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.jourMyPage, WebRequestFuncitons.GetMyBillData, true, jsonStr, RFrameWork.instance.token);
        }
    }
}
