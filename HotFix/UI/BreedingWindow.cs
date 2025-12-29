using HotFix.GameDatas.ServerData.Response;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using HotFix.Common;
using HotFix.Common.Utils;

namespace HotFix
{
    public class BreedingWindow : Window
    {
        private Slider progressSlider, progressSlider2;
        private Text FirstText, SecondText, ThirdText;
        private Text countTimeText;
        private GameObject maleLogo, femaleLogo, waitHorse, showHorse, nameType;
        private Text costText, nameText, name1Text, name2Text, geneText;
        private RawImage iconImg;
        private string timeTitle, progress;
        private DateTime startTime;
        private Button closeBtn;
        private bool showWindow;
        private float totalProgress;
        private float nowProgress;
        private bool isCountDown = false;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {

            Debug.Log("BreedingWindow Awake");
            progressSlider = m_Transform.Find("Bg/BreedProgress/Slider/ProgressSlider").GetComponent<Slider>();
            progressSlider2 = m_Transform.Find("Bg/BreedProgress/Slider/ProgressSlider2").GetComponent<Slider>();
            FirstText = m_Transform.Find("Bg/BreedProgress/Title/FirstText").GetComponent<Text>();
            SecondText = m_Transform.Find("Bg/BreedProgress/Title/SecondText").GetComponent<Text>();
            ThirdText = m_Transform.Find("Bg/BreedProgress/Title/ThirdText").GetComponent<Text>();
            countTimeText = m_Transform.Find("Bg/BreedProgress/BreedTime/TimeText").GetComponent<Text>();
            maleLogo = m_Transform.Find("Bg/MatingObject/HorseProperty/MaleLogo").gameObject;
            femaleLogo = m_Transform.Find("Bg/MatingObject/HorseProperty/FemaleLogo").gameObject;
            costText = m_Transform.Find("Bg/MatingObject/Charge/Text").GetComponent<Text>();
            nameText = m_Transform.Find("Bg/MatingObject/HorseProperty/HorseNameText").GetComponent<Text>();
            name1Text = m_Transform.Find("Bg/MatingObject/HorseProperty/Types/XueTong/Text").GetComponent<Text>();
            name2Text = m_Transform.Find("Bg/MatingObject/HorseProperty/Types/Type/Text").GetComponent<Text>();
            geneText = m_Transform.Find("Bg/MatingObject/HorseProperty/Types/Num/Text").GetComponent<Text>();
            nameType = m_Transform.Find("Bg/MatingObject/HorseProperty/Types").gameObject;
            waitHorse = m_Transform.Find("Bg/MatingObject/WaitHorse").gameObject;
            showHorse = m_Transform.Find("Bg/MatingObject/HorseProperty").gameObject;
            iconImg = m_Transform.Find("Bg/MatingObject/HorseProperty/IconLogo").GetComponent<RawImage>();
            closeBtn = m_Transform.Find("Bg/CloseBtn").GetComponent<Button>();
            AddButtonClickListener(closeBtn, () =>
            {
                showWindow = false;
                UIManager.instance.CloseWnd(FilesName.BREEDINGPANEL);
            });
            HorseGrowUpData growData = (HorseGrowUpData)(param1);
            HorseDetail horseDetail = (HorseDetail)(param3);
            totalProgress = float.Parse(growData.totalPregnancyProgress);
            nowProgress = float.Parse(growData.alreadyPregnancyProgress);
            BreedingSexType sexType = ((string)param2).Equals("0") ? BreedingSexType.Female : BreedingSexType.Male;
            if (!string.IsNullOrEmpty(growData.objectPic))
            {
                UpdateUI(growData.objectPic);

            }
            if (!string.IsNullOrEmpty(growData.objectCode))
            {
                string gene = growData.objectCode;
                string[] type = gene.Split('-');
                string name1 = "  " + JsonConfigManager.GetHorseBloodData()[type[2]] + "  ";
                string name2 = "  " + JsonConfigManager.GetHorseTypeData()[type[1]] + "马  ";
                string geneName = "  基因：" + gene + "  ";
                UpdateUI(horseDetail.status, sexType, growData.remainPregnancyTime, growData.pairingPrice, name1, name2, geneName, growData.objectName);

            }
            else
            {
                UpdateUI(horseDetail.status, sexType, growData.remainPregnancyTime, growData.pairingPrice, "", "", "", "");

            }


            Debug.Log("原马horseDetail："+JsonMapper.ToJson(horseDetail));
            showWindow = true;
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
        }
        private void UpdateUI(string url)
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
        private void UpdateUI(string status, BreedingSexType type, string reaminSeconds, string cost, string name1, string name2, string name3, string name)
        {
            Debug.Log("母马：" + type + "  seconds:" + reaminSeconds + "  status:" + status + " bloodName:" + name1 + "===" + name2 + "====" + name3);
            if (type == BreedingSexType.Female)
            {
                FirstText.text = "配种阶段";
                SecondText.text = "孕育阶段";
                ThirdText.text = "诞生";
                if (status.Equals("11"))
                {
                    timeTitle = "距配种完成:";
                    ShowHorse(true, true, cost, name1, name2, name3, name);
                    progressSlider.value = nowProgress / totalProgress;
                    progressSlider2.value = 0;
                    
                }
                else if(status.Equals("6"))
                {
                    Debug.Log("slider:"+ (nowProgress / totalProgress));
                    timeTitle = "距小马诞生:";
                    progressSlider.value = 1;
                    progressSlider2.value = nowProgress / totalProgress;
                    ShowHorse(true, true, cost, name1, name2, name3, name);

                }
                 

            }
            else if (type == BreedingSexType.Male)
            {
                FirstText.text = "等待阶段";
                SecondText.text = "配种阶段";
                ThirdText.text = "完成";
                if (status.Equals("11"))
                {
                    timeTitle = "距配种完成:";
                    ShowHorse(true, false, cost, name1, name2, name3, name);
                    progressSlider.value = 1;
                    progressSlider2.value = nowProgress / totalProgress;
                }
                else if (status.Equals("10"))
                {
                    timeTitle = "房间剩余时间:";
                    progressSlider.value = nowProgress / totalProgress;
                    progressSlider2.value = 0;
                    ShowHorse(false, false, cost, name1, name2, name3, name);
                }
            }

            startTime = DateTime.Now;
            isCountDown = true;
            startTime = startTime.AddSeconds(double.Parse(reaminSeconds) / 1000);
        }
        private void ShowHorse(bool isShow, bool ismale, string cost, string name1, string name2, string gene, string name = null)
        {
            showHorse.gameObject.SetActive(isShow);
            waitHorse.gameObject.SetActive(!isShow);
            if (isShow)
            {
                maleLogo.gameObject.SetActive(ismale);
                femaleLogo.gameObject.SetActive(!ismale);
            }
            name1Text.text = name1;
            name2Text.text = name2;
            geneText.text = gene;
            nameText.text = name;
            IEnumeratorTool.instance.StartCoroutineNew(ShowNameType());
            if (!string.IsNullOrEmpty(cost))
            {
                costText.text = "配种费：" + cost + "马粟";
            }
            else
            {
                costText.text = "配种费不详";
            }

        }
        private IEnumerator ShowNameType()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            nameType.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.1f);
            nameType.gameObject.SetActive(true);
        }
        public override void OnUpdate()
        {
            if (showWindow&& isCountDown)
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
            countTimeText.text = timeTitle+(span.Hours > 0 ? span.Hours.ToString() : "0") + "时" + (span.Minutes > 0 ? span.Minutes.ToString() : "0") + "分"+(span.Seconds > 0 ? span.Seconds.ToString() : "0") + "秒";
            if (TimeUtils.OnDiffSeconds(startTime, nowTime) > -0.1f)
            {
                isCountDown = false;
                Debug.Log("倒计时结束了");
            }
        }

    }
}

