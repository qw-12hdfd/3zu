using HotFix.Common;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace HotFix
{
    internal class RentWindow:Window
    {
        public InputField inputField;
        public Text desText;
        public Button sureBtn;
        public Button closeBtn;
        public Text titleText;
        public static Action<string> RentOutHorse;
        string id;
        string title;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            //RentOutHorse = RentOutHorseFunc;
            GetAllComponent();
            AddAllBtnListener();
            desText.text = UserInfoManager.rentOutDataStr;
        }

        private void RentOutHorseFunc()
        {
            if (titleText.text.Contains("填写"))
            {
                UIManager.instance.PopUpWnd(FilesName.PRICEAFFIRMPANEL, true, false, "填写", id, inputField.text);
            }
            else
            {
                UIManager.instance.PopUpWnd(FilesName.PRICEAFFIRMPANEL, true, false, "修改", id, inputField.text);
            }
        }

        private void GetAllComponent()
        {
            inputField = m_Transform.Find("BackImg/InputField").GetComponent<InputField>();
            desText = m_Transform.Find("BackImg/DesTitle/DesText").GetComponent<Text>();
            titleText = m_Transform.Find("BackImg/TitleImg/TitleText").GetComponent<Text>();
            sureBtn = m_Transform.Find("BackImg/SureBtn").GetComponent<Button>();
            closeBtn = m_Transform.Find("BackImg/CloseBtn").GetComponent<Button>();

        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(sureBtn, () =>
            {
                if (string.IsNullOrEmpty(inputField.text))
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "租赁费不可为空", () =>
                    {
                    });
                }
                else
                {
                    if (float.Parse(inputField.text) > 0)
                    {
                        RentOutHorseFunc();
                    }
                    else
                        RFrameWork.instance.OpenCommonConfirm("提示", "租赁费不可为0", () => { });
                }
            });
            AddButtonClickListener(closeBtn, () =>
            {
                UIManager.instance.CloseWnd(this);
            });
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            id = param1.ToString();
            title = param2.ToString();
            titleText.text = title;
            inputField.text = "";
        }
    }
}
