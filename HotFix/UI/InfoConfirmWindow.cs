using HotFix.Common;
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
    public class InfoConfirmWindow:Window
    {
        private Text des1;
        private Text des2;
        private Text name;
        private Button closeBtn;
        private Button sureBtn;
        private string num;
        private string phoneNum;
        public static Action<string> addFeedFunc;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllBtnListener();
            addFeedFunc = AddFeedFunc;
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            //inputNum.text, inputPhoneNum.text,name
            UpdateUI(param1.ToString(), param2.ToString(), param3.ToString());
        }

        private void UpdateUI(string des1, string des2, string name)
        {
            this.des1.text = "赠送：" + des1 + "马粟";
            this.des2.text = "接收账号：" + des2;
            char[] arr = name.ToCharArray();
            this.name.text = "昵称：" + arr[0] + "**" + arr[arr.Length - 1];
            num = des1;
            phoneNum = des2;
        }

        private void GetAllComponent()
        {
            des1 = m_Transform.Find("Back/Back/AddDes").GetComponent<Text>();
            des2 = m_Transform.Find("Back/Back/User").GetComponent<Text>();
            name = m_Transform.Find("Back/Back/UserName").GetComponent<Text>();
            closeBtn = m_Transform.Find("Back/ReturnBtn").GetComponent<Button>();
            sureBtn = m_Transform.Find("Back/SureBtn").GetComponent<Button>();
        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(closeBtn, CloseFunc);
            AddButtonClickListener(sureBtn, SureFunc);
        }

        private void CloseFunc()
        {
            UIManager.instance.CloseWnd(this);
        }

        private void SureFunc()
        {
            //立即赠送 TODO
            PassWordFunc();
        }

        private void AddFeedFunc(string str)
        {
            JsonData data = new JsonData();
            data["toMobile"] = phoneNum;
            data["amount"] = num;
            data["tradePwd"] = str;
            string jsonStr = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletTransfer, AddFishServerFunc, true, jsonStr, RFrameWork.instance.token);
        }

        private void AddFishServerFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                if (UIManager.instance.GetWndByName(FilesName.PASSWORDINFOPANEL) != null)
                    (UIManager.instance.GetWndByName(FilesName.PASSWORDINFOPANEL) as PasswordInfoWindow).isClick = true;
                string remark = jsonData["data"]["remark"].ToString();
                string status = jsonData["data"]["status"].ToString();
                if (status.Equals("1"))
                {
                    object[] objs = new object[] { "赠送成功", "共向尾号「" + phoneNum.Substring(phoneNum.Length - 4, 4) + "」用户赠送" + num + "马粟", 1, "确定", "" };
                    UIManager.instance.PopUpWnd(FilesName.IFSUCCEEDPANEL, true, false, objs);
                }
                else
                {
                    object[] objs = new object[] { "赠送失败", remark, 2, "确定", "" };
                    UIManager.instance.PopUpWnd(FilesName.IFSUCCEEDPANEL, true, false, objs);
                }
                UIManager.instance.CloseWnd(FilesName.ADDFEEDPANEL);
                UIManager.instance.CloseWnd(FilesName.INFOCONFIRMPANEL);
                UIManager.instance.CloseWnd(FilesName.PASSWORDINFOPANEL);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
        }

        private void PassWordFunc()
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
                    UIManager.instance.PopUpWnd(FilesName.PASSWORDINFOPANEL, true, false, PasswordType.AddFeed); ;

                }
            }
        }
    }
}
