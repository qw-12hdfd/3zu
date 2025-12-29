using Cinemachine;
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
    public class GainHorseWindow : Window
    {
        private Text name1Text, name2Text, geneText;
        private Text startRunText, speedText, durabilityText, wisdomText, tireText;
        private InputField horseNameInputText;
        private Button saveBtn;
        private Slider startRunSlider, speedSlider, durabilitySlider, wisdomSlider, tireSlider;
        private GameObject camera;
        BirthHorseData horseData;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
            horseData = (BirthHorseData)param1;
            string[] type = horseData.code.Split('-');
            string birthPos = (string)param2;
            SetCameraPos(birthPos);
            string name1 = "  " + JsonConfigManager.GetHorseBloodData()[type[2]] + "  ";
            string name2 = "  " + JsonConfigManager.GetHorseTypeData()[type[1]] + "马  ";
            string gene = "  基因：" + horseData.code + "  ";
            UpdateUI(name1, name2, gene, horseData.startSpeed, horseData.speed, horseData.endurance, horseData.wisdom, horseData.fatigue, horseData.startSpeedMax, horseData.speedMax, horseData.enduranceMax, horseData.wisdomMax, horseData.fatigueMax);


        }
        private void GetAllComponents()
        {
            name1Text = m_Transform.Find("Right/HorseProperty/Name/Types/XueTong/Text").GetComponent<Text>();
            name2Text = m_Transform.Find("Right/HorseProperty/Name/Types/Type/Text").GetComponent<Text>();
            geneText = m_Transform.Find("Right/HorseProperty/Name/Types/Num/Text").GetComponent<Text>();
            startRunText = m_Transform.Find("Right/HorseProperty/StartRun/NumText").GetComponent<Text>();
            speedText = m_Transform.Find("Right/HorseProperty/Speed/NumText").GetComponent<Text>();
            durabilityText = m_Transform.Find("Right/HorseProperty/Durability/NumText").GetComponent<Text>();
            wisdomText = m_Transform.Find("Right/HorseProperty/Wisdom/NumText").GetComponent<Text>();
            tireText = m_Transform.Find("Right/HorseProperty/Tire/NumText").GetComponent<Text>();

            startRunSlider = m_Transform.Find("Right/HorseProperty/StartRun/Slider").GetComponent<Slider>();
            speedSlider = m_Transform.Find("Right/HorseProperty/Speed/Slider").GetComponent<Slider>();
            durabilitySlider = m_Transform.Find("Right/HorseProperty/Durability/Slider").GetComponent<Slider>();
            wisdomSlider = m_Transform.Find("Right/HorseProperty/Wisdom/Slider").GetComponent<Slider>();
            tireSlider = m_Transform.Find("Right/HorseProperty/Tire/Slider").GetComponent<Slider>();
            horseNameInputText = m_Transform.Find("Right/HorseName/InputNameField").GetComponent<InputField>();
            saveBtn = m_Transform.Find("Right/BottomButton/SaveButton").GetComponent<Button>();




        }
        private void AddBtnClickListener()
        {
            AddButtonClickListener(saveBtn, OnSaveHorseDataClicked);

        }




        private void OnSaveHorseDataClicked()
        {

            if (!string.IsNullOrEmpty(horseNameInputText.text))
            {
                BirthHorseNameData data = new BirthHorseNameData(horseData.id, horseNameInputText.text);
                string jsonStr = JsonMapper.ToJson(data);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseUserNameUrl, SetNameWebRequestResponse, true, jsonStr, RFrameWork.instance.token);

            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "输入内容不能为空", () => { }, null);

            }

        }

        private void UpdateUI(string name1, string name2, string gene, string startSpeed, string speed, string dur, string wis, string tire, string startSpeedMax, string speedMax, string durMax, string wisMax, string tirMax)
        {
            Debug.Log("name1:" + name1);
            name1Text.text = name1;
            name2Text.text = name2;
            geneText.text = gene;
            startRunText.text = startSpeed;
            speedText.text = speed;
            durabilityText.text = dur;
            wisdomText.text = wis;
            tireText.text = tire;
            float startValue = float.Parse(startSpeed) / float.Parse(startSpeedMax);
            float speedValue = float.Parse(speed) / float.Parse(speedMax);
            float durVlaue = float.Parse(dur) / float.Parse(durMax);
            float wisVlaue = float.Parse(wis) / float.Parse(wisMax);
            float tireMax = float.Parse(tire) / float.Parse(tirMax);
            startRunSlider.value = startValue;
            speedSlider.value = speedValue;
            durabilitySlider.value = durVlaue;
            wisdomSlider.value = wisVlaue;
            tireSlider.value = tireMax;
        }
        private void SetCameraPos(string index)
        {
            string pos = "ShiCao" + index;
            if (GameObject.Find(pos).transform.Find("CameraPos"))
            {
                Transform moveTrans = GameObject.Find(pos).transform.Find("CameraPos");
                camera = GameObject.Find("Camera");
                camera.GetComponent<Camera>().fieldOfView = 60;
                camera.GetComponent<CinemachineBrain>().enabled = false;
                camera.transform.position = moveTrans.position;
                camera.transform.rotation = moveTrans.rotation;
            }
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            UIManager.instance.CloseWnd(FilesName.MAINPANEL);
        }

        private void SetNameWebRequestResponse(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "您的小马取名成功，请耐心等待管理员审核", () => {
                    Debug.Log("您的小马取名成功了");
                    camera.GetComponent<CinemachineBrain>().enabled = true;
                    UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                    UIManager.instance.CloseWnd(FilesName.GAINHORSEPANEL);
                }, null);

            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => {

                    camera.GetComponent<CinemachineBrain>().enabled = true;
                    UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                    UIManager.instance.CloseWnd(FilesName.GAINHORSEPANEL);

                }, null);

            }
        }



    }
}
