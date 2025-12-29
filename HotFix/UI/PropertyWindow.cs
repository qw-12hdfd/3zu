using HotFix.Common;
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
    internal class PropertyWindow:Window
    {
        Button returnBtn;
        Button sureBtn;
        Text Num;
        Text Des;
        Button moneyBtn;
        string id;
        int num;
        public static Action<string> RefreshData;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllButtonListener();
            RefreshData = RefreshNumData;
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            id = param1.ToString();
            num = int.Parse(param2.ToString());
            Num.text = num.ToString();
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
                moneyBtn.transform.GetChild(0).GetComponent<Text>().text = "         " + UserInfoManager.foodNum + "       ";
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(moneyBtn.transform.parent.GetComponent<RectTransform>());
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void GetAllComponent()
        {
            returnBtn = m_Transform.Find("Bg/CloseBtn").GetComponent<Button>();
            sureBtn = m_Transform.Find("Bg/SureBtn").GetComponent<Button>();
            moneyBtn = m_Transform.Find("Money").GetComponent<Button>();
            Num = m_Transform.Find("Bg/Num").GetComponent<Text>();
            Des = m_Transform.Find("Bg/Des").GetComponent<Text>();
        }

        private void AddAllButtonListener()
        {
            AddButtonClickListener(returnBtn, () => { UIManager.instance.CloseWnd(this); });
            AddButtonClickListener(sureBtn, SureFunc);
            AddButtonClickListener(moneyBtn, () => {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshNumData, true, "{}", RFrameWork.instance.token);
                UIManager.instance.PopUpWnd(FilesName.HORSEFEEDPANEL, true, false, moneyBtn.transform);
            });
        }

        private void SureFunc()
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
                    UIManager.instance.PopUpWnd(FilesName.PASSWORDINFOPANEL, true, false, PasswordType.RecordCreate,id); ;

                }
            }
        }
    }
}
