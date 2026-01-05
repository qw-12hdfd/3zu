using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
using LitJson;
using MalbersAnimations.HAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class WebRequestFuncitons
    {
        internal static void GetMilletDetail(string jsonStr)
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
                UIManager.instance.PopUpWnd(FilesName.PURCHASEHORSEMILLETPANEL, true, false,milletAmount,shareAmount,milletPrice);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
            //"data":{"id":"345","userId":"491396688746127360","name":"肆个模样的赛马场","status":"0","number":1,"maxNumber":12,"roomUserResList":[{"homeOwnerFlag":"1","userId":"491396688746127360","homeOwnerId":"491396688746127360","nickname":"肆个模样","horsePhoto":"https://metat.oss-accelerate.aliyuncs.com/1662350491744.jpg","readyFlag":"1","gameStartFlag":null}]}}
        }

        
        internal static void ShowHorsePageFront(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                List<HorseHistoryData> historyData = new List<HorseHistoryData>();
                foreach (JsonData item in jsonData["data"]["list"])
                {
                    HorseHistoryData data = JsonMapper.ToObject<HorseHistoryData>(item.ToJson());
                    historyData.Add(data);
                }
                UserInfoManager.horseHistoryData = historyData;
                //DetailWindow.SetDetailPanelActive(3);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void SendToken(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                StartWindow.setText("------正在检验------");
                string id = jsonData["data"]["id"].ToString();
                string name = jsonData["data"]["nickname"].ToString();
                string photoUrl = jsonData["data"]["photo"].ToString();
                UserInfoManager.userID = id;
                UserInfoManager.userName = name;
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetMyHorsesList(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                StartWindow.setText("------正在获取马匹信息------");
                UserInfoManager.MyHorseList.Clear();
                UserInfoManager.MyHorseList = new Dictionary<int, HorseData>();
                HorseData[] list = JsonMapper.ToObject<HorseData[]>(jsonData["data"].ToJson());
                int index = 1;
                foreach (HorseData item in list)
                {
                    int id = int.Parse(item.id);
                    Debug.Log(id + "   " + item.code);
                    UserInfoManager.MyHorseList.Add(index, item);
                    index++;
                }
                UserInfoManager.NowHorseList.Clear();
                UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
                Debug.Log("进入游戏");
                if (UserInfoManager.noHorse)
                    return;
                if(UserInfoManager.Sex == 0)
                {
                    UIManager.instance.PopUpWnd(FilesName.SELECTPLAYERPANEL, true, false);
                }
                else
                {
                    GameMapManager.instance.LoadScene(ConStr.MAINSCENE, FilesName.MAINPANEL, HouseManager.LoadMainScene);
                }
                UIManager.instance.CloseWnd(FilesName.STARTPANEL);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetMyHorsesData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                StartWindow.setText("------正在获取马匹信息------");
                HorseData[] list = JsonMapper.ToObject<HorseData[]>(jsonData["data"].ToJson());
                HouseManager.SetFeedNum(list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetHorseDetailData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseDetail data = JsonMapper.ToObject<HorseDetail>(jsonData["data"].ToJson());
                UIManager.instance.PopUpWnd(FilesName.DETAILPANEL, true, false, data, UserInfoManager.detailPanelType/*详情界面 2 房主选择马匹 3 加入房间选择马匹*/);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetGrowUpFront(string jsonStr)
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
                UIManager.instance.PopUpWnd(FilesName.GROWUPPANEL, true, false, dic,1);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void WalkHorseEndFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                PlayerController.GetDownHorseAction();
                UserInfoManager.playerCtrl.horse.transform.ResetLocal();
                UserInfoManager.playerCtrl.horse.transform.parent.rotation = UserInfoManager.horseRotate;
                UserInfoManager.playerCtrl.horse = null;
                UserInfoManager.mountHorseID = 0;
                if (UserInfoManager.mountBreedHorse)
                {
                    UserInfoManager.mountBreedHorse = false;
                    Debug.Log("开房间 上马成功");
                    UserInfoManager.playerCtrl.horse = UserInfoManager.mountBreedHorseObject;
                    PlayerController.MountHorseAction();
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetGameListData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                List<GameData> list = new List<GameData>();
                foreach (JsonData item in jsonData["data"]["list"])
                {
                    GameData data = JsonMapper.ToObject<GameData>(item.ToJson());
                    list.Add(data);
                }
                //GameListWindow.SetGameListData(list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetGameHistoryData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                List<HistoryDatas> list = new List<HistoryDatas>();
                foreach (JsonData item in jsonData["data"]["list"])
                {
                    HistoryDatas data = JsonMapper.ToObject<HistoryDatas>(item.ToJson());
                    list.Add(data);
                    Debug.Log(data.id + "   " + data.roomNumber);
                }
                //GameListWindow.SetHistoryListData(list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void CreateGameRoom(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (!code.Equals("200"))
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => {
                    // if (RoomWindow.EnterRoomFailedResponseAction != null)
                    // {
                    //     RoomWindow.EnterRoomFailedResponseAction();
                    // }
                }, null);

            }

        }

        internal static void JoinGameRoom(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (!code.Equals("200"))
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => {
                    // if (RoomWindow.EnterRoomFailedResponseAction != null)
                    // {
                    //     RoomWindow.EnterRoomFailedResponseAction();
                    // }
                }, null);

            }
        }
        internal static void GetMyFHorseData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseData[] list = JsonMapper.ToObject<HorseData[]>(jsonData["data"].ToJson());
                UserInfoManager.NowHorseList.Clear();
                UserInfoManager.NowHorseList = new Dictionary<int, HorseData>();
                int index = 1;
                foreach (HorseData item in list)
                {
                    int id = int.Parse(item.id);
                    Debug.Log(id + "   " + item.code);
                    UserInfoManager.NowHorseList.Add(index, item);
                    index++;
                }
                if (index > 1)
                {
                    UserInfoManager.detailPanelType = 5;
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[1].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
                }
                else
                {
                    UserInfoManager.NowHorseList.Clear();
                    UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
                    RFrameWork.instance.OpenCommonConfirm("提示", "您暂无可繁育的母马", () => { }, null);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetMyMHorsesList(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                UserInfoManager.NowHorseList.Clear();
                UserInfoManager.NowHorseList = new Dictionary<int, HorseData>();
                HorseData[] list = JsonMapper.ToObject<HorseData[]>(jsonData["data"].ToJson());
                int index = 1;
                foreach (HorseData item in list)
                {
                    int id = int.Parse(item.id);
                    Debug.Log(id + "   " + item.code);
                    UserInfoManager.NowHorseList.Add(index, item);
                    index++;
                }
                if (index > 1)
                {
                    UserInfoManager.detailPanelType = 4;
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[1].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
                }
                else
                {
                    UserInfoManager.NowHorseList.Clear();
                    UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
                    RFrameWork.instance.OpenCommonConfirm("提示", "您暂无可繁育的公马", () => { }, null);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetBreedSiteConfig(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                StartWindow.setText("------正在获取繁育场信息状态------");
                List<BreedSiteList> list = new List<BreedSiteList>();
                foreach(JsonData item in jsonData["data"]["breedSiteListResList"])
                {
                    list.Add(JsonMapper.ToObject<BreedSiteList>(item.ToJson()));
                }
                UserInfoManager.MyBreedSiteData = new BreedSiteData(jsonData["data"]["accountAvailableAmount"].ToString(),
                    int.Parse(jsonData["data"]["queueNumber"].ToString()),
                    jsonData["data"]["price"].ToString(),
                    int.Parse(jsonData["data"]["time"].ToString()),
                    list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        /// <summary>
        /// 公马开房成功之后的回调
        /// </summary>
        /// <param name="jsonStr"></param>
        internal static void HorseBreedSuccess(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                if (jsonData["data"]["status"].ToString() == "1")
                {
                    //公马开房成功 骑马开房逻辑
                    long timestamp = long.Parse(jsonData["data"]["remainTime"].ToString());
                    string roomId = jsonData["data"]["roomId"].ToString();

                    Debug.Log("开房间 数据获取");
                    var horse = ObjectManager.instance.InstantiateObject("Assets/GameData/Prefabs/Animals/Horse.prefab", true, true);
                    horse.name = UserInfoManager.selectHorseData.id;
                    Debug.Log("开房间 马匹生成");
                    UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).GetComponent<BoxCollider>().enabled = (true);
                    UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).GetChild(0).Find("HighLight").gameObject.SetActive(true);
                    if(UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).Find("Father").childCount > 0)
                    {
                        GameObject.Destroy(horse.gameObject);
                        horse = UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).Find("Father").GetChild(0).gameObject;
                    }
                    else
                    {
                        horse.transform.SetParent(UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).Find("Father"));
                    }
                    horse.transform.ResetLocal();
                    Debug.Log("开房间 设置位置");
                    var horseData = new HorseData(UserInfoManager.selectHorseData.id, UserInfoManager.selectHorseData.code, 2, (int)UserInfoManager.foodNum);
                    int price = 0;
                    HorseBreedObject obj = ObjectsManager.instance.AddObject(horse, "HorseFather", "HorseBreedObject", horseData, price, roomId) as HorseBreedObject;
                    UserInfoManager.MyBreedSiteData.list[int.Parse(roomId) - 1].horseFatherData = horseData;
                    UserInfoManager.MyBreedSiteData.list[int.Parse(roomId) - 1].horseFatherData.horse = horse;
                    UserInfoManager.MyBreedSiteData.list[int.Parse(roomId) - 1].horseFatherData.horseBreedCtrl = obj;
                    foreach (var item in UserInfoManager.MyHorseList)
                    {
                        if (item.Value.id == horseData.id)
                        {
                            item.Value.horseCtrl.isMount = false;
                        }
                    }
                    Debug.Log("开房间 马匹赋值");
                    UserInfoManager.mountBreedHorseObject = horse;
                    UserInfoManager.mountBreedHorse = true;
                    Debug.Log("开房间 上马");
                    PlayerController.MountHorseAction();
                    UIManager.instance.CloseAllWnd();
                    UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                    UIManager.instance.PopUpWnd(FilesName.SINGLETOASTPANEL, true, false, 1, timestamp);
                }
                else
                {
                    //公马预约成功
                    string num = jsonData["data"]["queueNumber"].ToString();
                    UIManager.instance.CloseAllWnd();
                    UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                    UIManager.instance.PopUpWnd(FilesName.SINGLETOASTPANEL, true, false, 0, num);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        public static void GetDownHorse()
        {
            PlayerController.GetDownHorseAction();
            UserInfoManager.playerCtrl.horse = null;
        }

        internal static void bookBreedSiteMaternal(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string status = jsonData["data"]["status"].ToString(); 
                // (UIManager.instance.GetWndByName(FilesName.PASSWORDINFOPANEL) as PasswordInfoWindow).isClick = true;
                if (status == "1")
                {
                    //母马配种成功 传送逻辑
                    string roomId = jsonData["data"]["roomId"].ToString();
                    long timestamp = long.Parse(jsonData["data"]["remainTime"].ToString());
                    var horse = ObjectManager.instance.InstantiateObject("Assets/GameData/Prefabs/Animals/Horse.prefab", true, true);
                    horse.name = UserInfoManager.selectHorseData.id;
                    UserInfoManager.nowHorseBreedRoom = UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1);
                    UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).GetComponent<BoxCollider>().enabled = (false);
                    UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).GetChild(0).Find("HighLight").gameObject.SetActive(false);
                    if (UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).Find("Mother").childCount > 0)
                    {
                        GameObject.Destroy(horse.gameObject);
                        horse = UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).Find("Mother").GetChild(0).gameObject;
                    }
                    else
                    {
                        horse.transform.SetParent(UserInfoManager.doorParent.GetChild(int.Parse(roomId) - 1).Find("Mother"));
                    }
                    horse.transform.ResetLocal();
                    var horseData = new HorseData(UserInfoManager.selectHorseData.id, UserInfoManager.selectHorseData.code, 2, (int)UserInfoManager.foodNum);
                    int price = 0;
                    HorseBreedObject obj = ObjectsManager.instance.AddObject(horse, "HorseMother", "HorseBreedObject", horseData, price, roomId) as HorseBreedObject;
                    UserInfoManager.MyBreedSiteData.list[int.Parse(roomId) - 1].horseMotherData = horseData;
                    UserInfoManager.MyBreedSiteData.list[int.Parse(roomId) - 1].horseMotherData.horse = horse;
                    UserInfoManager.MyBreedSiteData.list[int.Parse(roomId) - 1].horseMotherData.horseBreedCtrl = obj;
                    foreach(var item in UserInfoManager.MyHorseList)
                    {
                        if(item.Value.id == horseData.id)
                        {
                            item.Value.horseCtrl.isMount = false;
                        }
                    }
                    UIManager.instance.CloseAllWnd();
                    UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                    UIManager.instance.PopUpWnd(FilesName.SINGLETOASTPANEL, true, false, 2, timestamp);
                }
                else
                {
                    UIManager.instance.CloseAllWnd();
                    UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                    UIManager.instance.PopUpWnd(FilesName.SINGLETOASTPANEL, true, false, 3, 0);
                }
                //WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.breedSiteConfigUrl, WebRequestFuncitons.SetBreedSiteConfig, true, "{}", RFrameWork.instance.token);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        private static void SetBreedSiteConfig(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                List<BreedSiteList> list = new List<BreedSiteList>();
                foreach (JsonData item in jsonData["data"]["breedSiteListResList"])
                {
                    list.Add(JsonMapper.ToObject<BreedSiteList>(item.ToJson()));
                }
                UserInfoManager.MyBreedSiteData = new BreedSiteData(jsonData["data"]["accountAvailableAmount"].ToString(),
                    int.Parse(jsonData["data"]["queueNumber"].ToString()),
                    jsonData["data"]["price"].ToString(),
                    int.Parse(jsonData["data"]["time"].ToString()),
                    list);
                Debug.Log("通过web刷新繁殖场");
                HouseManager.RefreshBreedData();
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void CheckHorseNum(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                int num = int.Parse(jsonData["data"]["horseNumber"].ToString());
                if (num > 0)
                {
                    StartWindow.setText("------正在请求用户马匹数量------");
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkRole, WebRequestFuncitons.CheckRole, true, "{}", RFrameWork.instance.token);
                }
                else
                {
                    UIManager.instance.PopUpWnd(FilesName.QUITGAMEPANEL, true, false);
                    //RFrameWork.instance.OpenCommonConfirm("提示", "您暂无马匹，无法进入元宇宙", () => { ToolManager.StartExitGame(); }, null);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => {
                    if(UIManager.instance.GetWndByName(FilesName.STARTPANEL)!=null&& (UIManager.instance.GetWndByName(FilesName.STARTPANEL) as StartWindow).startGame != null)
                    {
                        (UIManager.instance.GetWndByName(FilesName.STARTPANEL) as StartWindow).startGame.interactable = true;
                    }
                }, null);
            }
        }

        internal static void CheckRole(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                UserInfoManager.Sex = int.Parse(jsonData["data"]["status"].ToString());
                if (jsonData["data"]["status"].ToString().Equals("0"))
                {
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.breedSiteConfigUrl, WebRequestFuncitons.GetBreedSiteConfig, true, "{}", RFrameWork.instance.token);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myPhotoUrl, WebRequestFuncitons.SendToken, true, "{}", RFrameWork.instance.token);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.GetMyHorsesList, true, "{}", RFrameWork.instance.token);
                }
                else
                {
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.breedSiteConfigUrl, WebRequestFuncitons.GetBreedSiteConfig, true, "{}", RFrameWork.instance.token);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.myPhotoUrl, WebRequestFuncitons.SendToken, true, "{}", RFrameWork.instance.token);
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.GetMyHorsesList, true, "{}", RFrameWork.instance.token);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void NullFunc(string obj)
        {
        }

        internal static void GetMyFeedNum(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                UserInfoManager.foodNum = float.Parse(jsonData["data"]["availableAmount"].ToString());
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void CanPlayGame(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseData[] list = JsonMapper.ToObject<HorseData[]>(jsonData["data"].ToJson());
                UserInfoManager.NowHorseList.Clear();
                UserInfoManager.NowHorseList = new Dictionary<int, HorseData>();
                int index = 1;
                foreach (HorseData item in list)
                {
                    int id = int.Parse(item.id);
                    Debug.Log(id + "   " + item.code);
                    UserInfoManager.NowHorseList.Add(index, item);
                    index++;
                }
                if (index > 1)
                {
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[1].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
                }
                else
                {
                    UserInfoManager.NowHorseList.Clear();
                    UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
                    RFrameWork.instance.OpenCommonConfirm("提示", "您暂无可参赛的马匹", () => { UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false); }, null);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void CanRentOut(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseData[] list = JsonMapper.ToObject<HorseData[]>(jsonData["data"].ToJson());
                UserInfoManager.NowHorseList.Clear();
                UserInfoManager.NowHorseList = new Dictionary<int, HorseData>();
                int index = 1;
                foreach (HorseData item in list)
                {
                    int id = int.Parse(item.id);
                    Debug.Log(id + "   " + item.code);
                    UserInfoManager.NowHorseList.Add(index, item);
                    index++;
                }
                if (index > 1)
                {
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[1].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
                }
                else
                {
                    UserInfoManager.NowHorseList.Clear();
                    UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
                    RFrameWork.instance.OpenCommonConfirm("提示", "您暂无可出租的马匹", () => { }, null);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void CanRentOutData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseData[] list = JsonMapper.ToObject<HorseData[]>(jsonData["data"].ToJson());
                UserInfoManager.NowHorseList.Clear();
                UserInfoManager.NowHorseList = new Dictionary<int, HorseData>();
                int index = 1;
                foreach (HorseData item in list)
                {
                    int id = int.Parse(item.id);
                    Debug.Log(id + "   " + item.code);
                    UserInfoManager.NowHorseList.Add(index, item);
                    index++;
                }
                if (index > 1)
                {
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + UserInfoManager.NowHorseList[1].id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
                }
                else
                {
                    UserInfoManager.NowHorseList.Clear();
                    UserInfoManager.NowHorseList = new Dictionary<int, HorseData>(UserInfoManager.MyHorseList);
                    RFrameWork.instance.OpenCommonConfirm("提示", "您暂无可出租的马匹", () => { }, null);
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetMyBillData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                BillData[] dataArr = JsonMapper.ToObject<BillData[]>(jsonData["data"]["list"].ToJson());
                List<BillData> list = new List<BillData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetMyBillData " + list.Count);
                UIManager.instance.PopUpWnd(FilesName.BILLPANEL, true, false, list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetQuotaDatas(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                QuotaData[] dataArr = JsonMapper.ToObject<QuotaData[]>(jsonData["data"]["list"].ToJson());
                List<QuotaData> list = new List<QuotaData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetMyQuotaData " + list.Count);
                // UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, CommonDataWindowType.QuotaList,list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void LookGameDetail(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string horseMatchPrice = jsonData["data"]["horseMatchPrice"].ToString();
                string horseMatchNumber = jsonData["data"]["horseMatchNumber"].ToString();
                string horseMatchCancel = jsonData["data"]["horseMatchCancel"].ToString();
                string horseMatchReward = jsonData["data"]["horseMatchReward"].ToString();
                string[] strs = new string[] { horseMatchPrice, horseMatchNumber, horseMatchCancel, horseMatchReward };
                Dictionary<string, string> dic = JsonMapper.ToObject<Dictionary<string, string>>(jsonData["data"]["rewardMap"].ToJson());
                UIManager.instance.PopUpWnd(FilesName.GAMEDETAILPANEL, true, false, strs,dic);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetHorseMatchDetail(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                JsonData roomData = jsonData["data"]["matchRecordListRes"];
                string roomId = jsonData["data"]["roomNumber"].ToString();
                string horseId = "0";
                string gameStartDatetime = jsonData["data"]["endDatetime"].ToString();
                int rank = 1;
                ResultRankData[] datas = new ResultRankData[roomData.Count];
                for (int i = 0; i < roomData.Count; i++)
                {
                    ResultRankData horseData = JsonMapper.ToObject<ResultRankData>(roomData[i].ToJson());
                    datas[i] = horseData;
                    Debug.Log("马排名的信息：" + horseData.rank);
                }
                UIManager.instance.PopUpWnd(FilesName.SETTLEMENTPANEL, true, false, datas);
                // SettlementWindow.SetPanelData(roomId, horseId, gameStartDatetime, rank, ((int)float.Parse(datas[rank - 1].rewardAmount)).ToString(),0);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetHorseShareConfigFront(string jsonStr)
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
                UIManager.instance.PopUpWnd(FilesName.GROWUPPANEL, true, false, dic, 2);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void ShareFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string id = jsonData["data"]["id"].ToString();//编号
                string photo = jsonData["data"]["photo"].ToString();//头像图片
                string nickname = jsonData["data"]["nickname"].ToString();//名称
                string inviteCode = jsonData["data"]["inviteCode"].ToString();//邀请码
                string inviteUrl = jsonData["data"]["inviteUrl"].ToString();//邀请链接
                string inviteUrlPhoto = jsonData["data"]["inviteUrlPhoto"].ToString();//二维码
                string milletQuantity = jsonData["data"]["milletQuantity"].ToString();//二维码

                Debug.Log("ShareWindow_id:"+id + ",photo:" + photo + ",nickname:" + nickname + ",inviteCode:" + inviteCode + ",inviteUrl:" + inviteUrl + ",inviteUrlPhoto:" + inviteUrlPhoto);
                ToolManager.ShareMsgToApp(id,photo,nickname,inviteCode,inviteUrl,inviteUrlPhoto, milletQuantity);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void CheckMatch(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string countdown = jsonData["data"]["countdown"].ToString();//二维码
                string waitTime = jsonData["data"]["waitTime"].ToString();//二维码
                float time = float.Parse(countdown);
                if(time>0)
                UIManager.instance.PopUpWnd(FilesName.SINGLETOASTPANEL, true, false, 5, countdown,waitTime);
                else
                {
                    if (UserInfoManager.returnAct != null)
                        UserInfoManager.returnAct.Invoke();
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }
        public static object[] objects;

        internal static void GetQuestionFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                string id = jsonData["data"]["id"].ToString();
                string isAnswer = jsonData["data"]["isAnswer"].ToString();
                string question = jsonData["data"]["title"].ToString();
                string answerType = jsonData["data"]["answerType"].ToString();
                string num1 = jsonData["data"]["amount"].ToString(); 
                string num2 = jsonData["data"]["questionRewardAmountPersonal"].ToString();
                string num3 = jsonData["data"]["questionRewardRemainingAmount"].ToString();
                if (isAnswer.Equals("1"))
                {
                    if (UIManager.instance.IsSignWindowOpen(FilesName.MAINPANEL))
                    {
                        QuestionData[] dataArr = JsonMapper.ToObject<QuestionData[]>(jsonData["data"]["horseQuestionOptionList"].ToJson());
                        objects = new object[] { "知识问答题", question, answerType, num1, num2, num3, dataArr, id };
                        Action QuestionFunc = WebRequestFuncitons.QuestionFunc;
                        object[] objs = new object[] { "恭喜您触发知识问答", "答对可获得马粟奖励哦，快来参与吧～", 3, "立即答题", "稍后答题" };
                        UIManager.instance.PopUpWnd(FilesName.IFSUCCEEDPANEL, true, false, objs, QuestionFunc, null);
                    }
                }
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        /// <summary>
        /// getQusetions
        /// </summary>
        public static void QuestionFunc()
        {
            UIManager.instance.PopUpWnd(FilesName.QUESTIONSPANEL, true, false, 0,objects);
        }

        internal static void GetRentHorseList(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseRentOutData[] dataArr = JsonMapper.ToObject<HorseRentOutData[]>(jsonData["data"]["list"].ToJson());
                List<HorseRentOutData> list = new List<HorseRentOutData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetRentHorseList " + list.Count);
                // UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, CommonDataWindowType.HorseRentOut, list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetMyRentOutHorseList(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseRentOutData[] dataArr = JsonMapper.ToObject<HorseRentOutData[]>(jsonData["data"]["list"].ToJson());
                List<HorseRentOutData> list = new List<HorseRentOutData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetMyRentOutHorseList " + list.Count);
                // UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, CommonDataWindowType.MyRentOut, list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetHistoryRecord(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseRentOutData[] dataArr = JsonMapper.ToObject<HorseRentOutData[]>(jsonData["data"]["list"].ToJson());
                List<HorseRentOutData> list = new List<HorseRentOutData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetHistoryRecord " + list.Count);
                // UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, CommonDataWindowType.HistoryBill, list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void GetMyRentHorseList(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                HorseRentOutData[] dataArr = JsonMapper.ToObject<HorseRentOutData[]>(jsonData["data"]["list"].ToJson());
                List<HorseRentOutData> list = new List<HorseRentOutData>();
                foreach (var item in dataArr)
                {
                    list.Add(item);
                }
                Debug.Log("GetMyRentHorseList " + list.Count);
                // UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, CommonDataWindowType.LeaseHorse, list);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        internal static void QuestionRecordFunc(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
        }
    }
}
