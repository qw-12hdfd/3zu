using HotFix.Common.Utils;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    internal class QuestionsWindow:Window
    {
        Text titleText;
        Text desStr;
        Button triggerBtn;
        GameObject triggerImage;
        Text questionText;
        Text countDownText;
        Transform content;
        Text desText;
        Button sureBtn;
        Button closeBtn;
        private bool isShowTime = false, isCountDown = false;
        private DateTime startTime;
        string answer = "";
        string answerNow = "";
        bool isAn = false;
        Dictionary<string, Transform> dic = new Dictionary<string, Transform>();
        string nowId;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllBtnListener();
            dic.Clear();
            dic["A"] = content.GetChild(0);
            dic["B"] = content.GetChild(1);
            dic["C"] = content.GetChild(2);
            dic["D"] = content.GetChild(3);
        }

        private void GetAllComponent()
        {
            titleText = m_Transform.Find("Title").GetComponent<Text>();
            desStr = m_Transform.Find("Title/Des").GetComponent<Text>();
            triggerBtn = m_Transform.Find("Title/Trigger").GetComponent<Button>();
            triggerImage = m_Transform.Find("Title/Trigger/back/Image").gameObject;
            questionText = m_Transform.Find("Back/TopicText").GetComponent<Text>();
            countDownText = m_Transform.Find("Back/CountDownText").GetComponent<Text>();
            content = m_Transform.Find("Back/Items");
            desText = m_Transform.Find("Back/DesText").GetComponent<Text>();
            sureBtn = m_Transform.Find("SureBtn").GetComponent<Button>();
            closeBtn = m_Transform.Find("CloseBtn").GetComponent<Button>();
        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(triggerBtn, TriggerEventFunc);
            AddButtonClickListener(sureBtn, SureFunc);
            AddButtonClickListener(closeBtn, CloseWnd);
        }

        private void TriggerEventFunc()
        {
            triggerImage.SetActive(!triggerImage.active);
        }

        private void SureFunc()
        {
            if (!isAn)
            {
                isCountDown = false;
                countDownText.gameObject.SetActive(false);
                desText.gameObject.SetActive(true);
                desText.text = "正确答案：" + answer;
                if (answerNow.Equals(answer))
                {
                    titleText.text = "恭喜您答对了";
                    desStr.text = "马粟+1";
                    foreach (var item in dic.Values)
                    {
                        item.Find("SelectImg").gameObject.SetActive(false);
                        item.GetComponent<Button>().enabled = false;
                    }
                    dic[answerNow].Find("SelectImg").gameObject.SetActive(true);
                    dic[answerNow].Find("SelectImg").GetComponent<Image>().color = new Color(0.2588235f, 0.854902f, 0.372549f);
                    dic[answerNow].Find("Text").GetComponent<Text>().color = new Color(0.2588235f, 0.854902f, 0.372549f);
                    dic[answerNow].Find("Tips/True").gameObject.SetActive(true);
                }
                else
                {
                    titleText.text = "很可惜答错了";
                    desStr.text = "再接再厉哦～";
                    foreach (var item in dic.Values)
                    {
                        item.Find("SelectImg").gameObject.SetActive(false);
                        item.GetComponent<Button>().enabled = false;
                    }
                    //dic[answerNow].Find("SelectImg").GetComponent<Image>().color = new Color(1, 0.3098039f, 0.3098039f);
                    dic[answerNow].Find("Text").GetComponent<Text>().color = new Color(1, 0.3098039f, 0.3098039f);
                    dic[answerNow].Find("Tips/False").gameObject.SetActive(true);
                    dic[answerNow].Find("SelectImg").gameObject.SetActive(true);
                    dic[answerNow].Find("SelectImg").GetComponent<Image>().color = new Color(1, 0.3098039f, 0.3098039f);
                    dic[answer].Find("SelectImg").GetComponent<Image>().color = new Color(0.2588235f, 0.854902f, 0.372549f);
                    dic[answer].Find("Tips/True").gameObject.SetActive(true);
                    dic[answer].Find("Text").GetComponent<Text>().color = new Color(0.2588235f, 0.854902f, 0.372549f);
                }
                isAn = true;
            }
            else
            {
                CloseWnd();
            }
        }

        private void CloseWnd()
        {
            if (isAn)
            {
                JsonData data = new JsonData();
                data["answer"] = answerNow;
                data["flag"] = triggerImage.active ? 1 : 0;
                data["horseId"] = UserInfoManager.mountHorseID;
                data["questionId"] = nowId;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.questionRecord, WebRequestFuncitons.QuestionRecordFunc, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
            }
            UIManager.instance.CloseWnd(this);
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            foreach (var item in dic.Values)
            {
                item.GetComponent<Button>().enabled = true;
            }
            int status = int.Parse(param1.ToString());
            object[] objs = param2 as object[]; 
            triggerImage.SetActive(false);
            //objects = new object[] { "知识问答题", question, answerType, num1, num2, num3, };
            if (status == 0)
            {
                isAn = false;
                string title = objs[0].ToString();
                string question = objs[1].ToString();
                answer = objs[2].ToString();
                float num1 = float.Parse(objs[3].ToString());
                float num2 = float.Parse(objs[4].ToString());
                float num3 = float.Parse(objs[5].ToString());
                QuestionData[] questionsArr = objs[6] as QuestionData[];
                nowId = objs[7].ToString();
                titleText.text = title;
                triggerBtn.gameObject.SetActive(num1 <= 0);
                desStr.gameObject.SetActive(num1 > 0);
                desStr.text = "答对将会有" + num1 + "马粟奖励，每日最高" + num2 + "马粟（奖池剩余：" + num3 + "）";
                questionText.text = "题目："+question;
                for (int i = 0; i < questionsArr.Length; i++)
                {
                    dic[questionsArr[i].type].GetComponent<Button>().onClick.RemoveAllListeners();
                    string type = questionsArr[i].type;
                    dic[questionsArr[i].type].GetComponent<Button>().onClick.AddListener(() =>
                    {
                        foreach(var item in dic.Values)
                        {
                            item.Find("SelectImg").gameObject.SetActive(false);
                        }
                        answerNow = type;
                        sureBtn.gameObject.SetActive(true);
                        dic[answerNow].Find("SelectImg").gameObject.SetActive(true);
                        dic[answerNow].Find("SelectImg").GetComponent<Image>().color = new Color(0.7098039f, 0.5921569f, 0.4941177f);
                    });
                    dic[questionsArr[i].type].Find("Text").GetComponent<Text>().text = questionsArr[i].type +"："+ questionsArr[i].content;
                    dic[questionsArr[i].type].Find("SelectImg").gameObject.SetActive(false);
                    dic[questionsArr[i].type].Find("Tips/True").gameObject.SetActive(false);
                    dic[questionsArr[i].type].Find("Tips/False").gameObject.SetActive(false);
                }
                desText.gameObject.SetActive(false);
                sureBtn.gameObject.SetActive(false);
                
                string countDownTime = "60";
                isShowTime = true;
                countDownText.gameObject.SetActive(true);
                if (isShowTime)
                {
                    startTime = DateTime.Now;
                    startTime = startTime.AddSeconds(double.Parse(countDownTime));
                    isCountDown = true;
                }
            }



        }
        public override void OnUpdate()
        {
            if (isShowTime && isCountDown)
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
            countDownText.text = "倒计时：" + (span.Seconds > 0 ? span.Seconds.ToString() : "0") + "秒";
            if (TimeUtils.OnDiffSeconds(startTime, nowTime) > -0.1f)
            {
                isCountDown = false;
                Debug.Log("倒计时结束了");
                isCountDown = false;
                countDownText.gameObject.SetActive(false);
                desText.gameObject.SetActive(true);
                desText.text = "正确答案：" + answer;
                titleText.text = "很可惜超时了";
                desStr.text = "再接再厉哦～";
                foreach (var item in dic.Values)
                {
                    item.Find("SelectImg").gameObject.SetActive(false);
                    item.GetComponent<Button>().enabled = false;
                }
                dic[answer].Find("Text").GetComponent<Text>().color = new Color(0.2588235f, 0.854902f, 0.372549f);
                dic[answer].Find("Tips/True").gameObject.SetActive(true);
                isAn = true;
                sureBtn.gameObject.SetActive(true);
            }
        }

        public override void OnClose()
        {
            isShowTime = false; 
            isCountDown = false;
            foreach (var item in dic.Values)
            {
                item.Find("Text").GetComponent<Text>().color = Color.white;
            }
            answerNow = "";
            answer = "";
        }
    }
}
