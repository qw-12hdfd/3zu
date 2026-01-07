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
    public class PurchaseHorseMilletWindow : Window
    {
       private InputField  numText;
        private Text milletCountText, explainText, payText,moneyText;
        private Button addBtn,reduceBtn,closeBtn,purchaseBtn,moneyBtn;
        private int count = 1;
        private float price = 0;
        private float totalPrice = 0;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
            this.numText.onValueChanged.RemoveListener(OnValueChanged);
            this.numText.onValueChanged.AddListener(OnValueChanged);
        }
        private void GetAllComponents()
        {
            numText = m_Transform.Find("Bg/PayCount/NumText").GetComponent<InputField>();
            milletCountText = m_Transform.Find("Bg/TotalPurchase/MilletCountText").GetComponent<Text>();
            explainText = m_Transform.Find("Bg/Detail/ExchangeInfoText").GetComponent<Text>();
            payText = m_Transform.Find("Bg/PurchaseBtn/Text").GetComponent<Text>();
            addBtn = m_Transform.Find("Bg/PayCount/AddBtn").GetComponent<Button>();
            reduceBtn = m_Transform.Find("Bg/PayCount/DecreaseBtn").GetComponent<Button>();
            purchaseBtn = m_Transform.Find("Bg/PurchaseBtn").GetComponent<Button>();
            closeBtn = m_Transform.Find("Bg/CloseBtn").GetComponent<Button>();
            moneyBtn = m_Transform.Find("Money").GetComponent<Button>();
            moneyText = m_Transform.Find("Money/Text").GetComponent<Text>();
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            //milletAmount,shareAmount,milletPrice
            numText.text = "1";
            moneyText.text = "         " + param1.ToString() + "       ";
            explainText.text = "可使用配额：" + (int)float.Parse(param2.ToString());
            milletCountText.text = UserInfoManager.hoserFeedNumber + "马粟";
            payText.text = param3.ToString() + "元购买"; ;
            price = float.Parse(param3.ToString());
            totalPrice = price;
        }
        private void AddBtnClickListener()
        {
            AddButtonClickListener(addBtn, OnAddNumClicked);
            AddButtonClickListener(reduceBtn, OnReduceNumClicked);
            AddButtonClickListener(purchaseBtn, OnPurchaseNowClicked);
            AddButtonClickListener(closeBtn, OnClosePanel);
            AddButtonClickListener(moneyBtn, () =>
            {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
                UIManager.instance.PopUpWnd(FilesName.HORSEFEEDPANEL, true, false, moneyBtn.transform);
            });
        }

        
        private void OnAddNumClicked()
        {
            count++;
            numText.text = count.ToString();
            UpdateUI();
        }
        private void OnReduceNumClicked()
        {
            count--;
            if (count<=0)
            {
                count = 1;
            }
            numText.text = count.ToString();
            UpdateUI();

        }
        private void OnPurchaseNowClicked()
        {
            if (UserInfoManager.peiENum > 0)
            {
                UserInfoManager.payType = PayType.HorseFeed;
                UIManager.instance.PopUpWnd(FilesName.PURCHASEPANEL, true, false, totalPrice, count);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "配额不足，无法购买", () => { }, null);
            }
        } 
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
        }
        private void UpdateUI()
        {             
            milletCountText.text = count* UserInfoManager.hoserFeedNumber + "马粟";
            explainText.text = "可使用配额："+UserInfoManager.peiENum;
            totalPrice = ((price*1000) * count)/1000;
            //Debug.Log(price + "*" + count + "=" + totalPrice);
            payText.text = totalPrice + "元购买";
            numText.text = count.ToString();
        }

        public void OnValueChanged(string v)
        {
            if (!string.IsNullOrEmpty(numText.text))
            {
                int inputCount = int.Parse(numText.text);
                if (inputCount == 0)
                {
                    count = 1;
                    numText.text = count.ToString();
                }
                else
                {
                    count = inputCount;
                    numText.text = count.ToString();
                }
            }
            else
            {
                count = 1;
                numText.text = count.ToString();
            }
            UpdateUI();
        }


    }
}
