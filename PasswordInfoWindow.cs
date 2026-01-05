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
    public class PasswordInfoWindow : Window
    {

        private Button closeBtn;
        private Button sureBtn;
        private Button[] inputBtns;
        private Text[] showTxts;
        private Button deleteBtn;
        private PasswordType payType;
        string password = "";
        string reconFirm = "";
        object thisParam2;
        public bool isClick = false;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
            payType = (PasswordType)(param1);
            Debug.Log("PasswordInfoWindow param1:" + payType);



        }
        private void GetAllComponents()
        {
            closeBtn = m_Transform.Find("Bg/CloseButton").GetComponent<Button>();
            sureBtn = m_Transform.Find("Bg/SureButton").GetComponent<Button>();
            deleteBtn = m_Transform.Find("Bg/DeleteBtn").GetComponent<Button>();
            inputBtns = m_Transform.Find("Bg/InputNumber").GetComponentsInChildren<Button>();
            showTxts = m_Transform.Find("Bg/ShowNumber").GetComponentsInChildren<Text>();
            Debug.Log("InputBtn length:"+inputBtns.Length);
            Debug.Log("showTxts length:" + showTxts.Length);
        }

        private void AddBtnClickListener()
        {
            ClearValue();
            AddButtonClickListener(closeBtn, OnClosePanel);
            AddButtonClickListener(deleteBtn, DeleteTxtValue);
            AddButtonClickListener(sureBtn, ConfirmBtnClicked);
            AddButtonClickListener(inputBtns[0],()=> {
                NumBtnClicked(1);
            } );
            AddButtonClickListener(inputBtns[1], () => {
                NumBtnClicked(2);
            });
            AddButtonClickListener(inputBtns[2], () => {
                NumBtnClicked(3);
            });
            AddButtonClickListener(inputBtns[3], () => {
                NumBtnClicked(4);
            });
            AddButtonClickListener(inputBtns[4], () => {
                NumBtnClicked(5);
            });
            AddButtonClickListener(inputBtns[5], () => {
                NumBtnClicked(6);
            });
            AddButtonClickListener(inputBtns[6], () => {
                NumBtnClicked(7);
            });
            AddButtonClickListener(inputBtns[7], () => {
                NumBtnClicked(8);
            });
            AddButtonClickListener(inputBtns[8], () => {
                NumBtnClicked(9);
            });
            AddButtonClickListener(inputBtns[9], () => {
                NumBtnClicked(0);
            });


        }




        private void NumBtnClicked(int num)
        {
            ClearValue();
             
            password += num;
            SetTxtValue();
            Debug.Log("NumBtnClicked num:"+ num+" password:" + password);
            
        }
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
        }


        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            isClick = false;
            ClearValue();
            password = "";
            reconFirm = "";
            thisParam2 = param2;
        }

        private void SetTxtValue()
        {
            if (password.Length <= 6)
            {
                reconFirm = password;
                Debug.Log("SetTxtValue ReconFirm:" + reconFirm);
            }
            if (password != null && password.Length > 0)
            {
                for (int i = 0; i < reconFirm.Length; i++)
                {
                    showTxts[i].text = "";
                    showTxts[i].text = password[i].ToString();
                    showTxts[i].text = "*";
                }
            }
        }
        private void MinusPassword()
        {
            password = reconFirm.Substring(0, reconFirm.Length - 1);
            Debug.Log(password);
        }
        private void ClearValue()
        {
            for (int i = 0; i < showTxts.Length; i++)
            {
                showTxts[i].text = "";
            }
        }
        private void DeleteTxtValue()
        {
            MinusPassword(); 
            ClearValue();
            Debug.Log("PasswordInfoPanel" + password);
            SetTxtValue();

        }
        private void ConfirmBtnClicked()
        {
            Debug.Log("ConfirmBtnClicked password:" + reconFirm);
            if (reconFirm.Length == 6 && reconFirm != null)
            {
                if (isClick)
                    return;
                if (payType == PasswordType.ExchangeHorseMillet)
                {
                    ExchangeHorseMilletWindow.InputPasswordAction(reconFirm);

                }
                else if (payType == PasswordType.BreedingCharge)
                {
                    PurchaseBreedChargeWindow.InputPasswordAction(reconFirm);
                }
                else if (payType == PasswordType.AddFeed)
                {
                    InfoConfirmWindow.addFeedFunc(reconFirm);
                }
                else if (payType == PasswordType.RecordCreate)
                {
                    JsonData jsondata = new JsonData();
                    jsondata["horseId"] = thisParam2.ToString();
                    jsondata["pwd"] = reconFirm;
                    string jsonStr = JsonMapper.ToJson(jsondata);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.payRecordCreate, PayRecordCreate, true, jsonStr, RFrameWork.instance.token);
                }
                else if (payType == PasswordType.StartGame)
                {
                    DetailWindow.startGame(reconFirm);
                }
                else if (payType == PasswordType.JoinRoom)
                {
                    DetailWindow.joinGame(reconFirm);
                }
                else if (payType == PasswordType.RentOutHorse)
                {
                    JsonData jsondata = new JsonData();
                    jsondata["rentId"] = UserInfoManager.horseRentOutId;
                    jsondata["tradePwd"] = reconFirm;
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseRecord, RentOutHorseFunc, true, JsonMapper.ToJson(jsondata), RFrameWork.instance.token);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "请输入正确的密码", () => { }, null);


            }
        }

        private void RentOutHorseFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                isClick = true;
                string countdown = jsonData["data"]["countdown"].ToString();//二维码
                string waitTime = jsonData["data"]["remark"].ToString();//二维码
                UIManager.instance.PopUpWnd(FilesName.SINGLETOASTPANEL, true, false, 6, countdown, waitTime);
            }
            else
            {
                UIManager.instance.PopUpWnd(FilesName.SINGLETOASTPANEL, true, false, 7, 0, jsonData["errorMsg"].ToString());
            }
        }

        private void PayRecordCreate(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                isClick = true;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount,PropertyWindow.RefreshData, true, "{}", RFrameWork.instance.token);
                string remark = jsonData["data"]["remark"].ToString();
                string status = jsonData["data"]["status"].ToString();
                if (status.Equals("1"))
                {
                    object[] objs = new object[] { "提高成功！", remark, 1, "确定", "" };
                    UIManager.instance.PopUpWnd(FilesName.IFSUCCEEDPANEL, true, false, objs);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + thisParam2.ToString(), WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);

                }
                else
                {
                    object[] objs = new object[] { "无法提高", remark, 3, "确定", "" };
                    UIManager.instance.PopUpWnd(FilesName.IFSUCCEEDPANEL, true, false, objs);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
            UIManager.instance.CloseWnd(FilesName.PASSWORDINFOPANEL);
            UIManager.instance.CloseWnd(FilesName.PROPERTYPANEL);
        }
    }
}
