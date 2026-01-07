using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
using LitJson;
using MalbersAnimations.HAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class OrderOpenHouseWindow : Window
    {

        private Text waitNumText, horseNameText,priceText,maxTimeText;
        private InputField breedPriceText;
        private Button okBtn,cancelBtn;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponents();
            AddBtnClickListener();
            breedPriceText.text = "";
        }
        
        private void GetAllComponents()
        {
            waitNumText = m_Transform.Find("Right/Title/WaitNumText").GetComponent<Text>();
            horseNameText = m_Transform.Find("HorseName/Text").GetComponent<Text>();
            priceText = m_Transform.Find("Right/BreedCost/OpenCost/Price").GetComponent<Text>();
            maxTimeText = m_Transform.Find("Right/BreedCost/OpenMaxTime/Time").GetComponent<Text>();
            breedPriceText = m_Transform.Find("Right/BreedCost/InputPriceField").GetComponent<InputField>();
            okBtn = m_Transform.Find("Right/BottomButton/OkButton").GetComponent<Button>();
            cancelBtn = m_Transform.Find("Right/BottomButton/CancelButton").GetComponent<Button>();
             
        }
        private void AddBtnClickListener()
        {
            AddButtonClickListener(okBtn, OnPurchaseClicked);
            AddButtonClickListener(cancelBtn, OnClosePanel);
        }
       
        
        
         
        private void OnPurchaseClicked()
        {
            if(!string.IsNullOrEmpty(breedPriceText.text))
            {
                OrderOpenHouseRequest();
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "输入内容不能为空", () => { }, null);

            }
        }
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
            UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
        }
        private void UpdateUI(string waitNum,string name,string time,string cost)
        {
            waitNumText.text = "当前排队人数：" + waitNum;
            horseNameText.text = name;
            maxTimeText.text = time + "小时"; 
            priceText.text = cost+ "元/次";

        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            HorseUtils.SetHorseTexture(m_Transform.Find("BG/HorsePos/Horse"), UserInfoManager.selectHorseData.code);
            //TODO接口请求
            UpdateUI(UserInfoManager.MyBreedSiteData.queueNumber.ToString(), UserInfoManager.selectHorseData.name, UserInfoManager.MyBreedSiteData.time.ToString(), UserInfoManager.MyBreedSiteData.price);
        }
        private void OrderOpenHouseRequest()
        {
            if(string.IsNullOrEmpty(breedPriceText.text))
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "输入内容不能为空", () => { }, null);
            }
            else
            {
                UserInfoManager.fatherPrice = int.Parse(breedPriceText.text);
                UserInfoManager.payType = PayType.BreedRoom;
                UIManager.instance.PopUpWnd(FilesName.PURCHASEPANEL, true, false, UserInfoManager.MyBreedSiteData.price, 1);
            }

        }
        private void WebRequestOrderOpenHouseResponse(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                int milletCount = int.Parse(jsonData["data"]["milletAmount"].ToString());
                

            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        public override void OnClose()
        {
            UserInfoManager.NowHorseList.Clear();
            UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
        }




    }
}
