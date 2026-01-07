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
    public class DoubleToastWindow : Window
    {
 
        private Text titleText, contentText;
        private Button okBtn,cancelBtn;
        private RawImage iconImg;
        private string changeStr;
        private string birthPos;
        private BirthHorseData horseData;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
            string type=(string)param1;
            object[] objects = (object[])param2;
            string url = (string)objects[0];
            birthPos = (string)objects[1];
            string time = (string)objects[2];
            string remark = (string)objects[3];
            horseData =(BirthHorseData)param3;
            UpdateUI(type,url,time, remark);
        }
        private void GetAllComponents()
        {
            titleText = m_Transform.Find("Bg/TitleText").GetComponent<Text>();
            contentText = m_Transform.Find("Bg/ToastInfoText").GetComponent<Text>();
            okBtn = m_Transform.Find("Bg/OkBtn").GetComponent<Button>();
            cancelBtn = m_Transform.Find("Bg/CancelBtn").GetComponent<Button>();
            iconImg = m_Transform.Find("Bg/Icon").GetComponent<RawImage>();
            

        }
        private void AddBtnClickListener()
        {
              AddButtonClickListener(cancelBtn, OnClosePanel);
              AddButtonClickListener(okBtn, OnCheckHorse);
        }        
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
            
        }
        private void OnCheckHorse()
        {
            UIManager.instance.CloseWnd(this);
            UIManager.instance.PopUpWnd(FilesName.GAINHORSEPANEL, true, false,horseData,birthPos);

        }
        private void UpdateUI(string type, string url=null, string time=null, string remark = null)
        {
            string content="";
            if(type.Equals("1"))
            {
                titleText.text = "小马诞生";
                content = "您的小马于" + TimeUtils.MilSecondsTimestampToTime(time) + "诞生，快去养马场看看吧";

            }
            else if (type.Equals("3"))
            {
                titleText.text = "小马取名失败";
                content = remark;

            }
            else if (type.Equals("5"))
            {
                titleText.text = "小马取名成功";
                content = "小马取名成功，快去养马场看看吧";

            }
            if (!string.IsNullOrEmpty(time))
            {
               
            }
            contentText.text = content;
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.Contains("https"))
                {
                    url = "https://yuzhouyuan.oss-accelerate.aliyuncs.com/" + url;
                }
                WebRequestManager.instance.AsyncLoadUnityTexture(url, (texture) =>
                {
                    iconImg.texture = texture;
                });
            }
            
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
             
        }     
    }
}
