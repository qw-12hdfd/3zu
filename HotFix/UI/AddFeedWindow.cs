
using HotFix.Common.Utils;
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
    internal class AddFeedWindow:Window
    {
        private Button returnBtn;
        private Button sureBtn;
        private Button addBtn;
        private Button minusBtn;
        private InputField inputNum;
        private InputField inputPhoneNum;
        private Text feedText;
        private int count = 1;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            FindAllComponent();
            AddAllBtnListener();
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            count = 1;
            inputNum.text = count.ToString();

            UpdateUI();
            inputNum.text = "1";
            inputPhoneNum.text = "";
        }

        private void FindAllComponent()
        {
            returnBtn = m_Transform.Find("Back/ReturnBtn").GetComponent<Button>();
            sureBtn = m_Transform.Find("Back/SureBtn").GetComponent<Button>();
            addBtn = m_Transform.Find("Back/InputBack/AddBtn").GetComponent<Button>();
            minusBtn = m_Transform.Find("Back/InputBack/MinusBtn").GetComponent<Button>();
            inputNum = m_Transform.Find("Back/InputBack/InputField").GetComponent<InputField>();
            inputPhoneNum = m_Transform.Find("Back/InputBack2/InputField").GetComponent<InputField>();
            feedText = m_Transform.Find("Back/InputBack/Des2").GetComponent<Text>();
        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(returnBtn, ReturnFunc);
            AddButtonClickListener(sureBtn, SureFunc);
            AddButtonClickListener(addBtn, AddFunc);
            AddButtonClickListener(minusBtn, MinusFunc);
            this.inputNum.onValueChanged.RemoveListener(OnValueChanged);
            this.inputNum.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(string arg0)
        {
            count = int.Parse(arg0);
        }

        private void UpdateUI()
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshNumData, true, "{}", RFrameWork.instance.token);
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
                feedText.text = "剩余" + UserInfoManager.foodNum + "马粟";
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

        private void SureFunc()
        {
            //确定赠送 TODO
            if (int.Parse(inputNum.text) <= UserInfoManager.foodNum)
            {
                if (string.IsNullOrEmpty(inputPhoneNum.text))
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "手机号输入为空", () => { }, null);
                }
                else
                {
                    JsonData data = new JsonData();
                    data["mobile"] = inputPhoneNum.text;
                    string jsonStr = JsonMapper.ToJson(data);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.getUserPhoto, GetUserName, true, jsonStr, RFrameWork.instance.token);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("马粟不足","您的马粟不足，无法赠送",() => { },null);
            }
        }

        private void GetUserName(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string name = jsonData["data"]["nickname"].ToString();
                UIManager.instance.PopUpWnd(FilesName.INFOCONFIRMPANEL, true, false, inputNum.text, inputPhoneNum.text, name);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void AddFunc()
        {
            count++;
            inputNum.text = count.ToString();
        }
        private void MinusFunc()
        {
            count--;
            if (count < 1)
            {
                count = 1;
            }
            inputNum.text = count.ToString();
        }
    }
}
