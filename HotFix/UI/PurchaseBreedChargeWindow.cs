using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
using LitJson;
using MalbersAnimations.HAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class PurchaseBreedChargeWindow : Window
    {
 
        private Text totalPriceText, payText,remainMilletText,titleText;
        private Button purchaseBtn,closeBtn;
        private float remainCount;
        private float totalPrice;
        public static Action<string> InputPasswordAction;
        PasswordType type;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
            totalPrice=float.Parse( param1.ToString());
            remainCount = float.Parse(param2.ToString());
            type = (PasswordType)param3;
            UpdateUI(totalPrice,remainCount, type);
            InputPasswordAction = InputPasswordCallBack;
        }
        private void GetAllComponents()
        {
            totalPriceText = m_Transform.Find("Bg/PayPanel/PayTotalPrice").GetComponent<Text>();
            payText = m_Transform.Find("Bg/PayPanel/PurchaseBtn/PayPriceText").GetComponent<Text>();            
            remainMilletText = m_Transform.Find("Bg/PayPanel/RemainMillet/Text").GetComponent<Text>();
            purchaseBtn = m_Transform.Find("Bg/PayPanel/PurchaseBtn").GetComponent<Button>();
            closeBtn = m_Transform.Find("Bg/PayPanel/CloseBtn").GetComponent<Button>();
            titleText = m_Transform.Find("Bg/PayPanel/Title/Text (Legacy)").GetComponent<Text>();
        }
        private void AddBtnClickListener()
        {
            AddButtonClickListener(purchaseBtn, OnPurchaseClicked);
            AddButtonClickListener(closeBtn, OnClosePanel);
        }
       
        
        
         /// <summary>
         /// TODO 此处有bug，需要先判断是否是自己的马匹不能配对，若是则不再执行余额不足购买马粟，弹出支付弹窗
         /// </summary>
        private void OnPurchaseClicked()
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkPwdUrl, CheckPwdWebRequesetCallBack, true, "{}", RFrameWork.instance.token);
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
                    if (remainCount >= totalPrice)
                    {
                        UIManager.instance.PopUpWnd(FilesName.PASSWORDINFOPANEL, true, false, type);
                    }
                    else
                    {
                        OnClosePanel();
                        WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, WebRequestFuncitons.GetMilletDetail, true, "{}", RFrameWork.instance.token);
                    }

                }
            }
        }
        private void InputPasswordCallBack(string password)
        {
            JsonData data = new JsonData();
            data["id"] = UserInfoManager.breedRoomId;
            data["motherId"] = UserInfoManager.selectHorseData.id;
            data["tradePwd"] = password;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.bookBreedSiteMaternalUrl, WebRequestFuncitons.bookBreedSiteMaternal, true, data.ToJson(), RFrameWork.instance.token);

        }
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
        }
        private void UpdateUI(float totalPrice,float remainCount,PasswordType type)
        {
            if(type == PasswordType.BreedingCharge)
            {
                titleText.text = "支付配种费";
                totalPriceText.text = "<size=30>配种费</size>" + "<size=90>" + totalPrice + "</size><size=30> 马粟</size>";
            }
            else
            {
                titleText.text = "支付租赁费";
                totalPriceText.text = "<size=30>租赁费</size>" + "<size=90>" + UserInfoManager.rentOutPrice + "</size><size=30> 马粟</size>";
            }
            remainMilletText.text = "马粟余额："+remainCount;
            if (remainCount >= totalPrice)
            {
                payText.text = "立即支付" + totalPrice + "马粟";
            }
            else
            {
                payText.text = "余额不足，立即购买";
            }

        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            //TODO接口请求
        }
    }
}
