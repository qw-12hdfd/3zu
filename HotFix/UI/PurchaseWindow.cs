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

namespace HotFix
{
    public class PurchaseWindow : Window
    {
 
        private Text totalPriceText, payText;
        private Button purchaseBtn,closeBtn,agreementBtn;
        private Toggle[] toggles;
        private Toggle agreementToggle;
        private List<PayTypeData> payTypes=new List<PayTypeData>();
        private string totalPrice;
        private int count;
        private string payKey;
        private string content;
        public static Action<bool> AgreementAction;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
            totalPrice=param1.ToString();
            count = int.Parse(param2.ToString());
            UpdateUI();
            payTypes.Clear();
            PurchaseTypeData data = new PurchaseTypeData("pay_type");
            string jsonStr = JsonMapper.ToJson(data);
            AgreementData data2 = new AgreementData("horse_buy_textarea");
            string jsonStr2 = JsonMapper.ToJson(data2);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.purchaseTypeUrl, GetPurchseTypeCallBack, true, jsonStr, RFrameWork.instance.token);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFront, GetPurchseAgreementCallBack, true, jsonStr2, RFrameWork.instance.token);
            AgreementAction = AgreementActionCallBack;

        }
        private void GetAllComponents()
        {
            totalPriceText = m_Transform.Find("Bg/PayPanel/PayTotalPrice").GetComponent<Text>();
            payText = m_Transform.Find("Bg/PayPanel/PurchaseBtn/PayPriceText").GetComponent<Text>();
            agreementBtn = m_Transform.Find("Bg/PayPanel/AgreementToggle/AgreementBtn").GetComponent<Button>();
            purchaseBtn = m_Transform.Find("Bg/PayPanel/PurchaseBtn").GetComponent<Button>();
            closeBtn = m_Transform.Find("Bg/PayPanel/CloseBtn").GetComponent<Button>();
            toggles = m_Transform.Find("Bg/PayToggleGroup").GetComponentsInChildren<Toggle>();
            agreementToggle = m_Transform.Find("Bg/PayPanel/AgreementToggle").GetComponent<Toggle>();
            agreementToggle.isOn = false;
            for (int i = 0; i < toggles.Length; i++)
            {           
                toggles[i].onValueChanged.RemoveAllListeners();
                toggles[i].onValueChanged.AddListener(OnValueChanged); 
                OnValueChanged(toggles[i].isOn);
            }
        }
        private void GetPurchseTypeCallBack(string json)
        {
            Debug.Log("PurchaseWindow GetPurchaseTypeCallBack:"+json);
            JsonData jsonData = JsonMapper.ToObject(json);
            JsonData payData = jsonData["data"];
            Debug.Log("PurchaseWindow GetPurchaseTypeCallBack count:"+payData.Count);
            for (int i = 0; i < payData.Count; i++)
            {
                PayTypeData paydata = JsonMapper.ToObject<PayTypeData>(payData[i].ToJson());
                payTypes.Add(paydata);
               
            }
            for (int i = 0; i < toggles.Length; i++)
            {
                OnValueChanged(toggles[i].isOn);
            }
        }
        private void GetPurchseAgreementCallBack(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                JsonData data = jsonData["data"];
                content = jsonData["data"][0]["value"].ToString();
                Debug.Log("PurchaseWindow GetPurchseAgreementCallBack:" + content);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }

        }
        private void AddBtnClickListener()
        {
            AddButtonClickListener(purchaseBtn, OnPurchaseClicked);
            AddButtonClickListener(closeBtn, OnClosePanel);
            AddButtonClickListener(agreementBtn, OnAgreementClicked);
            
        }
        private void OnValueChanged(bool isOn)
        {
            if (isOn)
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i].isOn)
                    {
                        if(toggles[i].name.Equals("SMPayToggle"))
                        {
                            Debug.Log("商盟统统付");
                            payKey = GetPayKey("商盟统统付");
                        }
                        else if (toggles[i].name.Equals("SDPayToggle"))
                        {
                            Debug.Log("杉德支付");
                            payKey = GetPayKey("杉德支付");
                        }
                        else if (toggles[i].name.Equals("WeChatPayToggle"))
                        {
                            Debug.Log("微信支付");
                            payKey= GetPayKey("微信支付");
                        }
                        else if (toggles[i].name.Equals("ALiPayToggle"))
                        {
                            Debug.Log("支付宝支付");
                            payKey = GetPayKey("支付宝支付");
                        }
                        break;
                    }
                }
            }
        }
        private void WebRequestPay(string key)
        {
            if(key.Equals("5"))
            {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.registSDUrl, RegistSDCallBack, true, "{}", RFrameWork.instance.token);

            }
            else
            {
                StartPurchase(key);
            }
            
        }
        private void StartPurchase(string key)
        {
            if (UserInfoManager.payType == PayType.HorseFeed)
            {
                JsonData data = new JsonData();
                data["payType"] = key;
                data["wxAppId"] = "";
                data["amount"] = count;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.purchaseNewUrl, PurchaseCallBack, true, JsonMapper.ToJson(data), RFrameWork.instance.token);

            }
            else if (UserInfoManager.payType == PayType.BreedRoom)
            {
                JsonData jsonData = new JsonData();
                jsonData["horseId"] = UserInfoManager.selectHorseData.id;
                jsonData["price"] = UserInfoManager.fatherPrice;
                jsonData["payType"] = key;
                jsonData["wxAppId"] = "";
                string jsonStr = JsonMapper.ToJson(jsonData);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseBreadSiteRecordUrl, PurchaseCallBack, true, jsonStr, RFrameWork.instance.token);
            }
        }
        public void PurchaseCallBack(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            Debug.Log("payKey is:" + payKey);
            if (!code.Equals("200"))
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);

            }
            else
            {
                UserInfoManager.payOrderCode = jsonData["data"]["orderId"].ToString();
                //测试
                //JsonData data = new JsonData();
                //data["result"] = "succeed";
                //UserInfoManager.payOrderCode = jsonData["data"]["orderId"].ToString();
                //PurchaseManager.SetStateOfPayment(data.ToJson());
                //return;
                //测试
                if (payKey.Equals("1"))
                {
                    //支付宝支付
                    string signOrder = jsonData["data"]["alipayPayOrderRes"]["signOrder"].ToString();
                    PurchaseManager.CallALiPay(signOrder);
                    

                }
                else if (payKey.Equals("2"))
                {
                    //微信支付
                    WeChatData weChatData = JsonMapper.ToObject<WeChatData>(jsonData["data"]["wechatAppPayInfo"].ToJson());
                    PurchaseManager.CallWeChatPay(weChatData.appId,weChatData.merchantId,weChatData.prepayId,weChatData.wechatPackage,weChatData.nonceStr,weChatData.timeStamp,weChatData.paySign);
                   
                }
                else if (payKey.Equals("4")|| payKey.Equals("5"))
                {
                    //杉德支付//商盟统统付
                    string url = jsonData["data"]["html"].ToString();
                    PurchaseManager.CallH5Pay(url);
                }
                
                Debug.Log("payKey is:"+payKey);
            }
             

        }
        public void RegistSDCallBack(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();

            if (!code.Equals("200"))
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);

            }
            else
            {
                string status= jsonData["data"]["status"].ToString();
                if(status.Equals("0"))
                {
                    string url = jsonData["data"]["url"].ToString();
                    Application.OpenURL(url);
                }
                else
                {
                    StartPurchase(payKey);
                }


            }
        }
            private string GetPayKey(string name)
        {
            for(int i=0;i<payTypes.Count;i++)
            {
                Debug.Log("GetPayKey value is:"+ payTypes[i].value+ "  name:" + name);
                if (payTypes[i].value.Equals(name))
                {
                    Debug.Log("GetPayKey key is:" + payTypes[i].key);
                    return payTypes[i].key;
                }
            }
            return null;
        }
         
        private void OnPurchaseClicked()
        {
            if(agreementToggle.isOn)
            {
                WebRequestPay(payKey);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "需同意用户协议", () => { }, null);

                Debug.Log("需同意用户协议");
            }
        } 
        private void OnAgreementClicked()
        {
            Application.OpenURL(ToolManager.privacyUrl);
           // UIManager.instance.PopUpWnd(FilesName.USERAGREEMENTPANEL, true, false, "用户协议更新", content, "阅读并同意");
        }
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
        }
        private void UpdateUI()
        {
            totalPriceText.text = "<size=30>支付</size>"+ "<size=60>"+ totalPrice + "</size><size=30> 元</size>";
            payText.text = "立即支付"+totalPrice + "元";

        }
        private void AgreementActionCallBack(bool isAgree)
        {
            if(isAgree)
            {
                agreementToggle.isOn = true;
            }
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            //TODO接口请求
        }
    }
}
