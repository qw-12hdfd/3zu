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
    public class SingleToastWindow : Window
    {
 
        private Text titleText, contentText,updateText;
        private string updateData;
        private Button okBtn;
        private bool isShowTime=false, isCountDown=false;
        private DateTime startTime;
        private string waitPlayerNum="0"; //0 预约成功 1 开房间成功 2母马配种成功 3 房间已满母马配种失败 4 倒计时结束 公马开房间成功 5 倒计时结束才能进入比赛 6 租赁成功 7 养马场已满，不能租赁
        int type;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            type = int.Parse(param1.ToString());
            string time = param2.ToString();
            updateData = param2.ToString();
            if (param3 != null)
            {
                waitPlayerNum = param3.ToString();
            }
            UpdateUI(type, time);
        }
        private void GetAllComponents()
        {
            titleText = m_Transform.Find("Bg/TitleText").GetComponent<Text>();
            contentText = m_Transform.Find("Bg/ToastInfoText").GetComponent<Text>();
            updateText = m_Transform.Find("Bg/Update/Text").GetComponent<Text>();
            okBtn = m_Transform.Find("Bg/OkBtn").GetComponent<Button>();
        }
        private void AddBtnClickListener()
        {
              AddButtonClickListener(okBtn, OnClosePanel);
        }
        
         
         
        private void OnClosePanel()
        {
            isShowTime=false;
            if (type == 1)
            {
                PlayerController.GetDownHorseAction();
                UserInfoManager.playerCtrl.horse = null;
            }
            else if(type == 2)
            {
                PlayerController.setPosition(UserInfoManager.nowHorseBreedRoom.transform.Find("playerPos").position);
            }
            else if(type == 6)
            {
                UIManager.instance.CloseWnd(FilesName.PASSWORDINFOPANEL);
                UIManager.instance.CloseWnd(FilesName.PURCHASEBREEDCHARGEPANEL);
                UIManager.instance.CloseWnd(FilesName.DETAILPANEL);
                UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                JsonData json = new JsonData();
                json["pageNum"] = 1;
                json["pageSize"] = 6;
                json["priceSort"] = (UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL) as CommonDataWindow).priceSort;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, WebRequestFuncitons.GetRentHorseList, true, JsonMapper.ToJson(json), RFrameWork.instance.token);
            }else if(type == 7)
            {
                UIManager.instance.CloseWnd(FilesName.PASSWORDINFOPANEL);
                UIManager.instance.CloseWnd(FilesName.PURCHASEBREEDCHARGEPANEL);
                UIManager.instance.CloseWnd(FilesName.DETAILPANEL);
                UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                JsonData json = new JsonData();
                json["pageNum"] = 1;
                json["pageSize"] = 6;
                json["priceSort"] = (UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL) as CommonDataWindow).priceSort;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, WebRequestFuncitons.GetRentHorseList, true, JsonMapper.ToJson(json), RFrameWork.instance.token);
            }
            UIManager.instance.CloseWnd(this);
        }
        private void UpdateUI(int type, string reaminSeconds)
        {
            updateText.transform.parent.gameObject.SetActive(true);
            if (type == 1)
            {
                titleText.text = "开房成功！";
                contentText.text = "恭喜您开房成功，可在剩余时间内等待母马前来配种";
                updateData = "剩余时长:";
                isShowTime = true;
            }
            else if (type == 0)
            {
                titleText.text = "预约成功！";
                contentText.text = "已为您预约成功，房间空余后，系统将安排您的马匹进入房间等待母马前来配种";
                updateData = "当前排队人数:"+ reaminSeconds;
            }
            else if (type == 2)
            {
                titleText.text = "配种成功！";
                contentText.text = "恭喜您配种成功";
                updateData = "剩余配种时长:";
                isShowTime = true;
            }
            else if(type == 3)
            {
                titleText.text = "喂养场已满";
                contentText.text = "您的喂养场暂无空闲马厩，无法进行繁育，请在有空闲马厩后在尝试繁育吧～";
                updateText.transform.parent.gameObject.SetActive(false);
            }
            else if(type == 4)
            {
                titleText.text = "开房成功！";
                contentText.text = "系统已安排您的公马进入繁育场，可在剩余时间内等待母马前来配种～";
                updateData = "剩余时长:";
                isShowTime = true;
            }
            else if (type == 5)
            {
                titleText.text = "发起比赛失败";
                contentText.text = "距您上次退出比赛未满" + waitPlayerNum + "分钟，待倒计时结束后即可比赛～";
                updateData = "剩余时长:";
                isShowTime = true;
            }
            else if (type == 6)
            {
                titleText.text = waitPlayerNum.Split('|')[0];
                contentText.text = waitPlayerNum.Split('|')[1];
                updateData = "剩余租赁时长:";
                isShowTime = true;
            }
            else if (type == 7)
            {
                titleText.text = waitPlayerNum.Split('|')[0];
                contentText.text = waitPlayerNum.Split('|')[1];
                updateText.transform.parent.gameObject.SetActive(false);
            }
            updateText.text = updateData;
            if(isShowTime)
            {
                startTime = DateTime.Now;
                startTime = startTime.AddSeconds(double.Parse(reaminSeconds) / 1000);
                isCountDown = true;

            }

        }
        public override void OnUpdate()
        {
            if (isShowTime&& isCountDown)
            {
                if (Time.frameCount % 10 == 0)
                {
                    CountDownTime();
                }
            }

        }
        private void CountDownTime()
        {
            DateTime nowTime = DateTime.Now;
            TimeSpan span = nowTime.Subtract(startTime).Duration();
            updateText.text = updateData + (span.Hours > 0 ? span.Hours.ToString() : "0") + "时" + (span.Minutes > 0 ? span.Minutes.ToString() : "0") + "分" + (span.Seconds > 0 ? span.Seconds.ToString() : "0") + "秒";
            if (TimeUtils.OnDiffSeconds(startTime, nowTime) > -0.1f)
            {
                isCountDown = false;
                Debug.Log("倒计时结束了");
            }
        }
    }
}
