using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace HotFix
{
    public class IfSucceedWindow:Window
    {
        private Transform succeedImg;
        private Transform unSucceedImg;
        private Transform tipsImg;
        private Text title;
        private Text des;
        private Button sureBtn;
        private Text sureBtnText;
        private Button cancelBtn;
        private Text cancelBtnText;
        private Action sureAct;
        private Action cancelAct;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllBtnListener();
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            object[] objs = param1 as object[];
            sureAct = param2 as Action;
            cancelAct = param3 as Action;
            UpdateUI(objs[0].ToString(), objs[1].ToString(), int.Parse(objs[2].ToString()), objs[3].ToString(), objs[4].ToString());
        }

        private void UpdateUI(string titleStr, string desStr, int ifSucceed,string sureText,string cancelText)
        {
            succeedImg.gameObject.SetActive(ifSucceed == 1);
            unSucceedImg.gameObject.SetActive(ifSucceed == 2);
            tipsImg.gameObject.SetActive(ifSucceed == 3);
            title.text = titleStr;
            des.text = desStr;
            sureBtnText.text = sureText;
            sureBtn.gameObject.SetActive(!string.IsNullOrEmpty(sureText));
            cancelBtnText.text = cancelText;
            cancelBtn.gameObject.SetActive(!string.IsNullOrEmpty(cancelText));
        }

        private void GetAllComponent()
        {
            succeedImg = m_Transform.Find("Bg/TitleImg/True");
            unSucceedImg = m_Transform.Find("Bg/TitleImg/False");
            tipsImg = m_Transform.Find("Bg/TitleImg/Tip");
            title = m_Transform.Find("Bg/Title").GetComponent<Text>();
            des = m_Transform.Find("Bg/Des").GetComponent<Text>();
            sureBtn = m_Transform.Find("Bg/GameObject/SureBtn").GetComponent<Button>();
            sureBtnText = m_Transform.Find("Bg/GameObject/SureBtn/Text (Legacy)").GetComponent<Text>();
            cancelBtn = m_Transform.Find("Bg/GameObject/CancelBtn").GetComponent<Button>();
            cancelBtnText = m_Transform.Find("Bg/GameObject/CancelBtn/Text (Legacy)").GetComponent<Text>();
        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(sureBtn, SureFunc);
            AddButtonClickListener(cancelBtn, CancelFunc);
        }

        private void CancelFunc()
        {
            UIManager.instance.CloseWnd(this);
            if (cancelAct != null)
                cancelAct.Invoke();
        }

        private void SureFunc()
        {
            UIManager.instance.CloseWnd(this);
            if (sureAct != null)
                sureAct.Invoke();
        }
    }
}
