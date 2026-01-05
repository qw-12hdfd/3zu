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
    public class ExchangeHorseMilletWindow : Window
    {
        private InputField numText;
        private Text  milletCountText, explainText, remainMilletCountText, magicPointText, remainMagicPointText;
        private Button addBtn, reduceBtn, exchangeBtn, backBtn, closeBtn;
        private int count = 1;
        private int exchangeRatio = 1, magicRatio = 1;
        private int totalCount = 0;
        private int remainCount, remainMagicCount;
        public static Action<string> InputPasswordAction;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            Debug.Log("ExchangeHorseMilletWindow Awake");
            GetAllComponents();
            AddBtnClickListener();
            this.numText.onValueChanged.RemoveListener(OnValueChanged);
            this.numText.onValueChanged.AddListener(OnValueChanged);
            this.numText.keyboardType = TouchScreenKeyboardType.NumberPad;
            int[] param1s = (int[])param1;
            magicRatio = param1s[0];
            exchangeRatio = param1s[1];
            int[] counts = (int[])param3;
            remainCount = counts[0];
            remainMagicCount = counts[1];
            count = 1;
            UpdateUI();
            UpdateMagicPointUI();
            InputPasswordAction = InputPasswordCallBack;
        }
        private void GetAllComponents()
        {
            numText = m_Transform.Find("Bg/PayCount/NumText").GetComponent<InputField>();
            milletCountText = m_Transform.Find("Bg/TotalPurchase/milletCountText").GetComponent<Text>();
            explainText = m_Transform.Find("Bg/Detail/ExchangeInfoText").GetComponent<Text>();
            remainMilletCountText = m_Transform.Find("Bg/Detail/RemainCountText").GetComponent<Text>();
            magicPointText = m_Transform.Find("Bg/ExchangeBtn/ExchangeCountText").GetComponent<Text>();
            remainMagicPointText = m_Transform.Find("Bg/ExchangeBtn/RemainCountText").GetComponent<Text>();
            addBtn = m_Transform.Find("Bg/PayCount/AddBtn").GetComponent<Button>();
            reduceBtn = m_Transform.Find("Bg/PayCount/DecreaseBtn").GetComponent<Button>();
            exchangeBtn = m_Transform.Find("Bg/ExchangeBtn").GetComponent<Button>();
            backBtn = m_Transform.Find("Bg/BackBtn").GetComponent<Button>();
            closeBtn = m_Transform.Find("Bg/CloseBtn").GetComponent<Button>();



        }
        private void AddBtnClickListener()
        {
            AddButtonClickListener(addBtn, OnAddNumClicked);
            AddButtonClickListener(reduceBtn, OnReduceNumClicked);
            AddButtonClickListener(exchangeBtn, OnExchangeNowClicked);
            AddButtonClickListener(backBtn, OnBackPurchasePanel);
            AddButtonClickListener(closeBtn, OnClosePanel);
        }
        private void OnAddNumClicked()
        {
            count++;
            numText.text = count.ToString();
            UpdateUI();
            UpdateMagicPointUI();
        }
        private void OnReduceNumClicked()
        {
            count--;
            if (count <= 0)
            {
                count = 1;
            }
            numText.text = count.ToString();
            UpdateUI();
            UpdateMagicPointUI();

        }
        private void OnExchangeNowClicked()
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkPwdUrl, CheckPwdWebRequesetCallBack, true, "{}", RFrameWork.instance.token);


        }
        private void InputPasswordCallBack(string password)
        {
            PurchaseData jsonData = new PurchaseData("0", password, "", count);
            string jsonStr = JsonMapper.ToJson(jsonData);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.purchaseUrl, PurchaseGoodsCallBack, true, jsonStr, RFrameWork.instance.token);

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
                    UIManager.instance.PopUpWnd(FilesName.PASSWORDINFOPANEL, true, false, PasswordType.ExchangeHorseMillet); ;

                }
            }
        }
        private void PurchaseGoodsCallBack(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                Debug.Log("兑换成功");
                int expandMagic = magicRatio * count * -1;
                MainWindow.UpdateMagicAndMilletAmountAction(totalCount, expandMagic);
                OnClosePanel();
                UIManager.instance.CloseWnd(FilesName.PURCHASEHORSEMILLETPANEL, false);
                UIManager.instance.CloseWnd(FilesName.PASSWORDINFOPANEL, false);
                RFrameWork.instance.OpenCommonConfirm("提示", "兑换成功", () => { (UIManager.instance.GetWndByName(FilesName.PASSWORDINFOPANEL) as PasswordInfoWindow).isClick = true; }, null);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
            //"data":{"id":"345","userId":"491396688746127360","name":"肆个模样的赛马场","status":"0","number":1,"maxNumber":12,"roomUserResList":[{"homeOwnerFlag":"1","userId":"491396688746127360","homeOwnerId":"491396688746127360","nickname":"肆个模样","horsePhoto":"https://metat.oss-accelerate.aliyuncs.com/1662350491744.jpg","readyFlag":"1","gameStartFlag":null}]}}
        }
        private void OnBackPurchasePanel()
        {
            UIManager.instance.CloseWnd(this);

        }
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
        }
        private void UpdateUI()
        {

            totalCount = count * exchangeRatio;
            milletCountText.text = totalCount.ToString() + "马粟";
            remainMilletCountText.text = "剩余" + this.remainCount.ToString() + "马粟";
            numText.text = count.ToString();
        }




        private void UpdateMagicPointUI()
        {
            magicPointText.text = magicRatio * count + "元气值兑换";
            remainMagicPointText.text = "剩余" + this.remainMagicCount + "元气值";
            explainText.text = magicRatio + "元气值可兑换1份马粟，" + "1份=" + exchangeRatio + "马粟";


        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {

            numText.GetComponent<InputContentSizeWidth>().SetInputFieldValue(1000, 20, 88, new Vector2(88, 88));

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
            UpdateMagicPointUI();

        }



    }
}
