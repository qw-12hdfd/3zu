using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class UserAgreementWindow : Window
    {
 
        private Text contentText, titleText,btnText;
        private Button closeBtn, AgreeBtn;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
            string title=(string)param1;
            string content=(string)param2;
            string btn=(string)param3;
            UpdateUI(title,content,btn);
            
           
        }
        private void GetAllComponents()
        {
            contentText = m_Transform.Find("Bg/PayPanel/Scroll View/Viewport/Content").GetComponent<Text>();
            titleText = m_Transform.Find("Bg/PayPanel/Title/Text").GetComponent<Text>();
            btnText = m_Transform.Find("Bg/PayPanel/AgreeBtn/Text").GetComponent<Text>();

            closeBtn = m_Transform.Find("Bg/PayPanel/CloseBtn").GetComponent<Button>();
            AgreeBtn = m_Transform.Find("Bg/PayPanel/AgreeBtn").GetComponent<Button>();

        }
        private void AddBtnClickListener()
        {
            AddButtonClickListener(closeBtn, OnClosePanel);
            AddButtonClickListener(AgreeBtn, OnAgreementClicked);
        }
        
       
        
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
        }
        public void UpdateUI(string title, string content, string btnInfo)
        {
            titleText.text = title;
            btnText.text = btnInfo;
            content = content.Replace("<p>", "").Replace("</p>", "").Replace("<ol>", "\n").Replace("</ol>", "\n").Replace("<strong>&ldquo;", "\"").Replace("&rdquo;</strong>", "\"").Replace("&ldquo;", "\"").Replace("&rdquo;", "\"");
            string[] array22 = Regex.Split(content, "</li>");  
            string finishStr = "";
            for (int i = 0; i < array22.Length; i++)
            {
                string str = array22[i].Replace("<li>", "\u00A0\u00A0\u00A0" + (i + 1).ToString() + ".");
                Debug.Log("str：" + str);
                finishStr += str;
            }
            Debug.Log("长度：" + array22.Length);
            contentText.text = finishStr + "\n";

        }
      private void OnAgreementClicked()
      {
            PurchaseWindow.AgreementAction(true);
            UIManager.instance.CloseWnd(this);
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            //TODO接口请求
        }

        
       
        
        
    }
}
