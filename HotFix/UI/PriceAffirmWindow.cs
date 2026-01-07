using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace HotFix
{
    internal class PriceAffirmWindow : Window
    {
        private RawImage icon;
        private Text name;
        private Text des;
        private Text title;
        private Text sure;
        private Button returnBtn;
        private Button sureBtn;
        private bool isClick = false;
        string id;
        string price;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            FindAllComponent();
            AddAllButtonListener();
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            isClick = false;
            title.text = "价格" + param1.ToString() + "确认";
            name.text = UserInfoManager.horseRentOutName;
            string titleStr = param1.ToString();
            if (titleStr.Equals("填写"))
            {
                sure.text = "确认发布";
            }
            else
                sure.text = "立即" + titleStr;
            des.text = "租赁费：" + param3.ToString() + "马粟";
            WebRequestManager.instance.AsyncLoadUnityTexture(UserInfoManager.horseTexture, (texture) =>
            {
                icon.texture = texture;
            });
            id = param2.ToString();
            price = param3.ToString();

        }

        private void FindAllComponent()
        {
            icon = m_Transform.Find("Back/IconBack/Icon").GetComponent<RawImage>();
            name = m_Transform.Find("Back/Name").GetComponent<Text>();
            des = m_Transform.Find("Back/Des").GetComponent<Text>();
            title = m_Transform.Find("Back/TitleImg/TitleText").GetComponent<Text>();
            sure = m_Transform.Find("Back/SureBtn/Text").GetComponent<Text>();
            returnBtn = m_Transform.Find("Back/ReturnBtn").GetComponent<Button>();
            sureBtn = m_Transform.Find("Back/SureBtn").GetComponent<Button>();
        }

        private void AddAllButtonListener()
        {
            AddButtonClickListener(returnBtn, ReturnFunc);
            AddButtonClickListener(sureBtn, SureFunc);
        }

        private void ReturnFunc()
        {
            UIManager.instance.CloseWnd(this);
        }

        private void SureFunc()
        {
            if (isClick)
                return;
            if (title.text.Contains("填写"))
            {
                if (UserInfoManager.playerCtrl.mount)
                {
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd, WebRequestFuncitons.WalkHorseEndFunc, true, JsonMapper.ToJson(new HorseIdData(UserInfoManager.mountHorseID)), RFrameWork.instance.token);
                }
                JsonData jsondata = new JsonData();
                jsondata["horseId"] = this.id;
                jsondata["price"] = this.price;
                string jsonStr = JsonMapper.ToJson(jsondata);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorse, SetRentHorsePrice, true, jsonStr, RFrameWork.instance.token);
            }
            else
            {
                JsonData jsondata = new JsonData();
                jsondata["id"] = this.id;
                jsondata["price"] = this.price;
                string jsonStr = JsonMapper.ToJson(jsondata);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.changeRentHorse, SetRentHorsePrice, true, jsonStr, RFrameWork.instance.token);
            }
        }

        private void SetRentHorsePrice(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                isClick = true;
                if (title.text.Contains("填写"))
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "马匹出租成功", () => {
                        UIManager.instance.CloseAllWnd();
                        UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                        (UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL) as CommonDataWindow).RentOutMyHorseFunc();
                    }, null);
                }
                else
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "修改租赁费成功", () => {
                        UIManager.instance.CloseAllWnd();
                        UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                        (UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL) as CommonDataWindow).RentOutMyHorseFunc();
                    }, null);
                };
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => {
                    UIManager.instance.CloseAllWnd();
                    UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                    (UIManager.instance.GetWndByName(FilesName.COMMONDATAPANEL) as CommonDataWindow).RentOutMyHorseFunc();
                }, null);
            }
        }
    }
}
