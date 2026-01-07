using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using LitJson;
using HotFix.Common.Utils;
using HotFix.Common;

namespace HotFix
{
    class MainWindow : Window
    {
        /// <summary>
        /// 退出游戏按钮
        /// </summary>
        private Button Quit;

        private Button Map;

        private Button Jump;
        private Button MountHorseBtn;
        private Button GetDownHorseBtn;
        private Button HorseDataBtn;
        private Button PutFoodBtn;
        private Button money;
        private Button leaseBtn;
        private Button changeBtn;
        private Button shareBtn;
        private Text FoodNumText;
        private Text moneyText;

        private GameObject player;
        private CameraController camCtrler;
        private JointedArm leftJointedArm;
        private JointedArm rightJointedArm;

        public static Action<bool,bool> MountHorse;//上马
        public static Action<bool> GetDownHorse;//下马
        public static Action<bool> SetBtnGray;//按钮置灰
        public static Action<bool, int, int> PutFood;//喂食
        public static Action<float> UpdateFoodWebRequestAction;//刷新喂食按钮上的数据
        public static Action< int, float, double, double> UpdateFeedPriceAndAmountWebRequestAction;//饲料马粟元气值等
        public static Action<int, int> UpdateMagicAndMilletAmountAction;//更新马粟元气值等
        public static Action<string> RefreshNumData;
        public static int horseIndex = 0;
        public static int horseID = 0;
        /// <summary>
        /// 每份饲料元气值
        /// </summary>
        public int perFeedElement;
        /// <summary>
        /// 每份饲料数量
        /// </summary>
        public int feedNumber;
        /// <summary>
        /// 每份饲料单价
        /// </summary>
        public float feedPrice;
        /// <summary>
        /// 饲料数量
        /// </summary>
        public double milletAmount;
        /// <summary>
        /// 元气值数量
        /// </summary>
        public double accountAmount;

        public bool isClick1 = false;
        public bool isClick2 = false;
        public bool isClick3 = false;


        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            MountHorse = ShowMountHorseBtn;
            GetDownHorse = ShowGetDownHorseBtn;
            PutFood = ShowPutFoodBtn;
            SetBtnGray = SetBtnGrayFunc;
            RefreshNumData = RefreshData;
            UpdateFeedPriceAndAmountWebRequestAction = UpdateFeedPriceAndAmount;
            UpdateMagicAndMilletAmountAction = UpdateMilletAmountAndMagicAmount;
            RFrameWork.instance.SetBackAudio("SingleSounds/MainBack");
            GetAllComponent();
            AddAllButtonClickListener();
            AgreementData data2 = new AgreementData("horse_buy_textarea");
            string jsonStr2 = JsonMapper.ToJson(data2);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFront, AgreementWebRequestResponse, true, jsonStr2, RFrameWork.instance.token);
            AgreementData data3 = new AgreementData("horse_rent_time");
            string jsonStr3 = JsonMapper.ToJson(data3);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFront, GetTimeConfig, true, jsonStr3, RFrameWork.instance.token);
            //WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseBreadPayResultsUrl, WebRequestFuncitons.HorseBreedSuccess, true, jsonText, RFrameWork.instance.token);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseBirthListUrl, BirthWebRequestResponse, true, "{}", RFrameWork.instance.token);
            ListFront listFront;
            listFront = new ListFront("horse_rent_config");
            string jsonStr = JsonMapper.ToJson(listFront);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFront, GetConfig, true, jsonStr, RFrameWork.instance.token);

        }

        private void GetTimeConfig(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                JsonData data = jsonData["data"];
                Debug.Log("MainWindow AgreementWebRequestResponse GetTimeConfig 接收到的消息是：" + jsonData["data"][0]["value"].ToString());
                UserInfoManager.RentOutTime = jsonData["data"][0]["value"].ToString();
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void GetConfig(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Clear();
                foreach (JsonData data in jsonData["data"])
                {
                    string key = data["key"].ToString();
                    string value = data["value"].ToString();
                    dic.Add(key, value);
                }
                UserInfoManager.rentOutDataStr = dic["horse_rent_note"];
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void SetBtnGrayFunc(bool obj)
        {
            changeBtn.interactable = obj;
        }

        private void AddAllButtonClickListener()
        {
            AddButtonClickListener(Quit, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("", "确认退出马术元宇宙？", () => { ToolManager.ExitGame(); }, () => { });
            });
            AddButtonClickListener(Map, OpenMap);
            AddButtonClickListener(Jump, JumpFunc);
            AddButtonClickListener(MountHorseBtn, MountHorseFunc);
            AddButtonClickListener(GetDownHorseBtn, GetDownHorseFunc);
            AddButtonClickListener(HorseDataBtn, ShowHorseData);
            AddButtonClickListener(PutFoodBtn, FeedHorseClicked);
            AddButtonClickListener(money, ShowMoneyPanel);
            AddButtonClickListener(leaseBtn, LeaseHorsePanel);
            AddButtonClickListener(changeBtn, ChangePlayer);
            AddButtonClickListener(shareBtn, SharePicFunc);
        }

        private void ShowMoneyPanel()
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshData, true, "{}", RFrameWork.instance.token);
            UIManager.instance.PopUpWnd(FilesName.HORSEFEEDPANEL, true, false,money.transform);
        }

        private void LeaseHorsePanel()
        {
            JsonData data = new JsonData();
            data["pageNum"] = 1;
            data["pageSize"] = 6;
            data["priceSort"] = 0;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, WebRequestFuncitons.GetRentHorseList, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
        }

        private void ChangePlayer()
        {
            UIManager.instance.PopUpWnd(FilesName.SELECTPLAYERPANEL, true, false);
        }

        private void SharePicFunc()
        {
            //ToolManager.ShareMsgToApp(m_Transform.GetComponent<RectTransform>());
            UserInfoManager.rankStr = "邀请您下载元年app体验「马术元宇宙」";
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myInvite, WebRequestFuncitons.ShareFunc, true, "{}", RFrameWork.instance.token);
        }

        private void GetAllComponent()
        {
            money = m_Transform.Find("Btns/Money").GetComponent<Button>();
            leaseBtn = m_Transform.Find("Btns/LeaseHorse").GetComponent<Button>();
            changeBtn = m_Transform.Find("Btns/ChangePlayer").GetComponent<Button>();
            shareBtn = m_Transform.Find("Btns/SharePic").GetComponent<Button>();
            Quit = m_Transform.Find("Tip/QuitBtn").GetComponent<Button>();
            Map = m_Transform.Find("Map").GetComponent<Button>();
            Jump = m_Transform.Find("Jump").GetComponent<Button>();
            MountHorseBtn = m_Transform.Find("HorseButtons/MountHorseBtn").GetComponent<Button>();
            HorseDataBtn = m_Transform.Find("HorseButtons/HorseDataBtn").GetComponent<Button>();
            GetDownHorseBtn = m_Transform.Find("HorseButtons/GetDownHorseBtn").GetComponent<Button>();
            PutFoodBtn = m_Transform.Find("HorseButtons/FoodBtn").GetComponent<Button>();
            FoodNumText = m_Transform.Find("HorseButtons/FoodBtn/Num").GetComponent<Text>();
            moneyText = m_Transform.Find("Btns/Money/Text").GetComponent<Text>();
            MountHorseBtn.gameObject.SetActive(false);
            GetDownHorseBtn.gameObject.SetActive(false);
            HorseDataBtn.gameObject.SetActive(false);
            PutFoodBtn.gameObject.SetActive(false);
            player = UserInfoManager.player;
            camCtrler = UserInfoManager.camCtrler;
            leftJointedArm = m_Transform.Find("SetComponent/LeftArm").GetComponent<JointedArm>();
            rightJointedArm = m_Transform.Find("SetComponent/RightArm").GetComponent<JointedArm>();
        }

        private void ShowHorseData()
        {
            UserInfoManager.detailPanelType = 1;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + horseID, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
        }

        private void FeedHorseClicked()
        {
            if (isClick1)
                return;
            isClick1 = true;
            Debug.Log("MainWindow FeedHorseClicked");
            if (UserInfoManager.foodNum > 0)
            {
                HorseIdData data = new HorseIdData(horseID);
                string jsonStr = JsonMapper.ToJson(data);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.feedHorseUrl, FeedHorseResult, true, jsonStr, RFrameWork.instance.token);
            }
            else
            {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, GetMilletDetail, true, "{}", RFrameWork.instance.token);
            }
        }
        internal void GetMilletDetail(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string totalShareAmount = jsonData["data"]["totalShareAmount"].ToString();
                string milletAmount = (jsonData["data"]["milletAmount"].ToString());
                string shareAmount = (jsonData["data"]["shareAmount"].ToString());
                string milletPrice = (jsonData["data"]["milletPrice"].ToString());
                string hoserFeedNumber = jsonData["data"]["hoserFeedNumber"].ToString();
                UserInfoManager.hoserFeedNumber = (float)Math.Round(float.Parse(hoserFeedNumber), 2);
                UserInfoManager.peiENum = (float)Math.Round(float.Parse(shareAmount), 2);
                UserInfoManager.foodNum = (float)Math.Round(float.Parse(milletAmount), 2);
                UserInfoManager.allPeiENum = (float)Math.Round(float.Parse(totalShareAmount), 2);
                UIManager.instance.PopUpWnd(FilesName.PURCHASEHORSEMILLETPANEL, true, false, milletAmount, shareAmount, milletPrice);
                isClick1 = false;
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
                isClick1 = false;
            }
            //"data":{"id":"345","userId":"491396688746127360","name":"肆个模样的赛马场","status":"0","number":1,"maxNumber":12,"roomUserResList":[{"homeOwnerFlag":"1","userId":"491396688746127360","homeOwnerId":"491396688746127360","nickname":"肆个模样","horsePhoto":"https://metat.oss-accelerate.aliyuncs.com/1662350491744.jpg","readyFlag":"1","gameStartFlag":null}]}}
        }


        internal void FeedHorseResult(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                //RFrameWork.instance.OpenCommonConfirm("提示", "投喂成功", () => { }, null);
                float milletCount = float.Parse(jsonData["data"]["milletAmount"].ToString());
                string status = jsonData["data"]["status"].ToString();

                if (status.Equals("3"))
                {
                    //UpdateFoodWebRequest(milletCount);
                    //WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletDetailUrl, GetMilletDetailOnly, true, "{}", RFrameWork.instance.token);
                    RFrameWork.instance.OpenCommonConfirm("提示", "投喂成功", () =>
                    {
                    }, null);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
                }
                else if (status.Equals("0"))
                {
                    string remark = jsonData["data"]["remark"].ToString();
                    RFrameWork.instance.OpenCommonConfirm("提示", remark, () =>
                    {

                        WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, GetMilletDetail, true, "{}", RFrameWork.instance.token);


                    }, null);


                }
                else if (status.Equals("1"))
                {
                    string remark = jsonData["data"]["remark"].ToString();
                    RFrameWork.instance.OpenCommonConfirm("提示", remark, () => { }, null);

                }
                isClick1 = false;
            }

            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
                isClick1 = false;
            }
            //"data":{"id":"345","userId":"491396688746127360","name":"肆个模样的赛马场","status":"0","number":1,"maxNumber":12,"roomUserResList":[{"homeOwnerFlag":"1","userId":"491396688746127360","homeOwnerId":"491396688746127360","nickname":"肆个模样","horsePhoto":"https://metat.oss-accelerate.aliyuncs.com/1662350491744.jpg","readyFlag":"1","gameStartFlag":null}]}}
        }

        private void ShowPutFoodBtn(bool bl, int index, int id)
        {
            if (UserInfoManager.maCaoTransform != null)
            {
                Debug.Log("关闭高亮的模型" + bl);
                UserInfoManager.maCaoTransform.Find("HighLight").gameObject.SetActive(bl);
                if (bl)
                {
                    ObjectsManager.instance.SetHighLight(UserInfoManager.maCaoTransform.Find("ShiCaoObject").gameObject);
                }
                else
                {
                    ObjectsManager.instance.SetHighLightClose(UserInfoManager.maCaoTransform.Find("ShiCaoObject").gameObject);
                }
            }
            PutFoodBtn.gameObject.SetActive(bl);
            horseIndex = index;
            UserInfoManager.nowHorseIndex = horseIndex;
            horseID = id;
            FoodNumText.text = "一次" + UserInfoManager.hoserFeedNumber+"马粟";
        }
        private void UpdateFoodWebRequest(float num)
        {
            var fodders = UserInfoManager.maCaoTransform.Find("fodders");
            for (int i = 0; i < fodders.GetChildCount(); i++)
            {
                if (fodders.GetChild(i).gameObject.active == false)
                {
                    fodders.GetChild(i).gameObject.SetActive(true);
                    var ani = UserInfoManager.maCaoTransform.Find("HorsePos").GetChild(0).GetComponent<Animator>();
                    if (ani.GetBool("Eat") == false && !UserInfoManager.playerCtrl.mount)
                        ani.SetBool("Eat", true);
                    break;
                }
            }
        }
        private void UpdateFeedPriceAndAmount(int fnPrice, float fpPrice, double amount, double magicAmount)
        {
            this.feedNumber = fnPrice;
            this.feedPrice = fpPrice;
            this.milletAmount = amount;
            this.accountAmount = magicAmount;
            Debug.Log("更新价格  feedNumber:" + this.feedNumber + "  feedPrice价格:" + this.feedPrice +" 马粟数量："+ this.milletAmount +"  元气值数量："+ this.accountAmount);


        }
        private void UpdateMilletAmountAndMagicAmount(int milletAmount, int magicAmount)
        {
            this.milletAmount += milletAmount;
            this.accountAmount += magicAmount;
            UserInfoManager.foodNum = milletAmount;
            FoodNumText.text = "一次" + UserInfoManager.hoserFeedNumber + "马粟";
        }
        private void ShowGetDownHorseBtn(bool bl)
        {
            GetDownHorseBtn.gameObject.SetActive(bl);
        }
        private void GetDownHorseFunc()
        {
            if (isClick3)
                return;
            isClick3 = true;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd, WalkHorseEndFunc, true, JsonMapper.ToJson(new HorseIdData(UserInfoManager.mountHorseID)), RFrameWork.instance.token);
        }

        internal void WalkHorseEndFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                isClick3 = false;
                PlayerController.GetDownHorseAction();
                UserInfoManager.playerCtrl.horse.transform.ResetLocal();
                UserInfoManager.playerCtrl.horse.transform.parent.rotation = UserInfoManager.horseRotate;
                UserInfoManager.playerCtrl.horse = null;
                UserInfoManager.mountHorseID = 0;
                //if (UserInfoManager.mountBreedHorse)
                //{
                //    UserInfoManager.mountBreedHorse = false;
                //    Debug.Log("开房间 上马成功");
                //    UserInfoManager.playerCtrl.horse = UserInfoManager.mountBreedHorseObject;
                //    PlayerController.MountHorseAction();
                //}
            }
            else
            {
                isClick3 = false;
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void MountHorseFunc()
        {
            if (isClick2)
                return;
            isClick2 = true;
            UserInfoManager.mountHorseID = horseID;
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorse, WalkHorseFunc, true, JsonMapper.ToJson(new HorseIdData(horseID)), RFrameWork.instance.token);
        }

        internal void WalkHorseFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                Debug.Log(UserInfoManager.MyHorseList.Count);
                UserInfoManager.playerCtrl.horse = UserInfoManager.MyHorseList[UserInfoManager.nowHorseIndex].horse;
                UserInfoManager.horseRotate = UserInfoManager.playerCtrl.horse.transform.parent.rotation;
                UserInfoManager.playerCtrl.horse.transform.parent.rotation = Quaternion.Euler(Vector3.zero);
                PlayerController.GoToPosition("siyangchang");
                PlayerController.MountHorseAction();
                isClick2 = false;
                RFrameWork.instance.OpenCommonConfirm("提示", "马匹送回马厩需返回养马场才可下马", () => {
                }, null);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
                isClick2 = false;
            }
        }

        private void ShowMountHorseBtn(bool bl,bool bl2)
        {
            MountHorseBtn.gameObject.SetActive(bl2 && horseIndex != 0);
            HorseDataBtn.gameObject.SetActive(bl && horseIndex != 0);
        }
        private void BirthWebRequestResponse(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                JsonData data = jsonData["data"];
                 Debug.Log("MainWindow BirthWebRequestResponse 接收到的消息是：" + jsonData["data"].ToJson());
                if (data.Count > 0)
                {
                     foreach(JsonData item in data)
                    {
                         if (item["type"].ToString().Contains("1")|| item["type"].ToString().Contains("3") || item["type"].ToString().Contains("4"))
                        {
                            BirthHorseData horseData = JsonMapper.ToObject<BirthHorseData>(item["horseDataRes"].ToJson());
                            string picUrl = item["pic"].ToString();
                            string caoPos = item["horseStableNo"].ToString();
                            string bornDatetime = item["bornDatetime"].ToString();
                            object[] objects = { picUrl, caoPos, bornDatetime, item["remark"].ToString() };
                            UIManager.instance.PopUpWnd(FilesName.DOUBLETOASTPANEL, true, false, item["type"].ToString(), objects, horseData);
                        }else if (item["type"].ToString().Equals("5"))
                            {
                            RFrameWork.instance.OpenCommonConfirm("马匹取名", "马匹取名审核通过", () => { }, null);

                        }
                        else
                        {
                            UIManager.instance.PopUpWnd(FilesName.SINGLETOASTPANEL, true, false, 4, item["remainTime"].ToString());
                        }
                    }

                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }
        private void AgreementWebRequestResponse(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                JsonData data = jsonData["data"];
                Debug.Log("MainWindow AgreementWebRequestResponse horse_buy_textarea 接收到的消息是：" + jsonData["data"][0]["value"].ToJson());
                 

            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            isClick1 = false;
            isClick2 = false;
            isClick3 = false;
            // 左摇杆 -------------------------------------------
            leftJointedArm.onDragCb = (direction) =>
            {
                var realDirect = UserInfoManager.camera.transform.localToWorldMatrix * new Vector3(direction.x, 0, direction.y);
                realDirect.y = 0;
                realDirect = realDirect.normalized;
                PlayerController.moveAction(realDirect);
            };
            leftJointedArm.onStopCb = () => { PlayerController.standAction(); };
            // 右摇杆 ------------------------------------------
            rightJointedArm.onDragCb2 = (direction) =>
            {
                camCtrler.RotateCam(direction);
            };
            rightJointedArm.onStopCb = () => { camCtrler.StopRotate(); };
            NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
            PutFoodBtn.gameObject.SetActive(UserInfoManager.maCaoTransform != null && !UserInfoManager.playerCtrl.mount);
            UserInfoManager.mount = false;
            JsonData data = new JsonData();
            data["currency"] = "MILLET";
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailByUser, WebRequestFuncitons.GetMyFeedNum, true, data.ToJson(), RFrameWork.instance.token); ;
            if(UserInfoManager.noHorse)
            {
                JsonData json = new JsonData();
                json["pageNum"] = 1;
                json["pageSize"] = 6;
                json["priceSort"] = 0;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.rentHorseList, WebRequestFuncitons.GetRentHorseList, true, JsonMapper.ToJson(json), RFrameWork.instance.token);
            }
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, RefreshData, true, "{}", RFrameWork.instance.token);
        }

        private void RefreshData(string jsonStr)
        {
            Debug.Log("RefreshData:" + jsonStr);
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                //Convert.ToDouble("111.116").ToString("N2");
                string shareAmount = jsonData["data"]["shareAmount"].ToString();
                string milletAmount = jsonData["data"]["milletAmount"].ToString();
                string totalShareAmount = jsonData["data"]["totalShareAmount"].ToString();
                string hoserFeedNumber = jsonData["data"]["hoserFeedNumber"].ToString();
                string matchPrice = jsonData["data"]["matchPrice"].ToString();
                string horseMilletNote = jsonData["data"]["horseMilletNote"].ToString();
                UserInfoManager.hoserFeedNumber = (float)Math.Round(float.Parse(hoserFeedNumber), 2);
                UserInfoManager.peiENum = (float)Math.Round(float.Parse(shareAmount), 2);
                UserInfoManager.foodNum = (float)Math.Round(float.Parse(milletAmount), 2);
                UserInfoManager.allPeiENum = (float)Math.Round(float.Parse(totalShareAmount), 2);
                UserInfoManager.matchPrice = matchPrice;
                UserInfoManager.horseMilletNote = horseMilletNote;
                if (UIManager.instance.GetWndByName(FilesName.HORSEFEEDPANEL) != null && (UIManager.instance.GetWndByName(FilesName.HORSEFEEDPANEL) as HorseFeedWindow).desText != null)
                {
                    (UIManager.instance.GetWndByName(FilesName.HORSEFEEDPANEL) as HorseFeedWindow).desText.text = UserInfoManager.horseMilletNote;
                }
                moneyText.text = "         " + UserInfoManager.foodNum + "       ";
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(money.GetComponent<RectTransform>());
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private void JumpFunc()
        {
            if(UserInfoManager.playerCtrl.jumping==false&& UserInfoManager.playerCtrl.isGround)
                PlayerController.jumpAction();
        }

        private void OpenMap()
        {
            UIManager.instance.PopUpWnd(FilesName.MAPPANEL, true, false);
        }

        public override void OnUpdate()
        {
            PlayerController.walkAction(leftJointedArm.Run);
        }
    }
}
