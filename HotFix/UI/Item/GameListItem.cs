using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using LitJson;
using HotFix.Common.Utils;
using HotFix.Common;
using System.Xml.Linq;
using HotFix.Manager;

namespace HotFix
{
    internal class GameListItem
    {
        private Text NameText;
        private Button Join;
        private Button Look;
        private Button LookGame;
        private Slider slider;
        private Text sliderText;
        private Text timeText;
        private Transform statusWait;
        private Transform statusDoing;
        private Transform statusOver;
        private Transform Item;
        GameData data;

        public void Init(Transform item,GameData m_Data)
        {
            this.data = m_Data;
            Item = item;
            NameText = item.Find("GameName/RoomId").GetComponent<Text>();
            Join = item.Find("Btns/Join").GetComponent<Button>();
            Look = item.Find("Btns/Look").GetComponent<Button>();
            LookGame = item.Find("Btns/LookGame").GetComponent<Button>();
            slider = item.Find("Slider").GetComponent<Slider>();
            sliderText = item.Find("Slider/Des").GetComponent<Text>();
            timeText = item.Find("TimeText").GetComponent<Text>();
            statusWait = item.Find("GameName/RoomId/Status/WaitImg");
            statusDoing = item.Find("GameName/RoomId/Status/DoingImg");
            statusOver = item.Find("GameName/RoomId/Status/OverImg");
            item.gameObject.SetActive(true);
            Join.onClick.RemoveAllListeners();
            Look.onClick.RemoveAllListeners();
            LookGame.onClick.RemoveAllListeners();
            Join.onClick.AddListener(() =>
            {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkMatch, WebRequestFuncitons.CheckMatch, true, "{}", RFrameWork.instance.token);
                UserInfoManager.returnAct = ReturnAct;
            });
            Look.onClick.AddListener(() =>
            {
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseMatchDetail + "/" + m_Data.id, WebRequestFuncitons.GetHorseMatchDetail, true, "{}", RFrameWork.instance.token);
            });
            LookGame.onClick.AddListener(() =>
            {
                UserInfoManager.isLookGame = 1;
                if (!NetManager.instance.HasConnected())
                {
                    NetManager.instance.OnStartReconnect();
                    return;
                }
                UserInfoManager.enterGame = true;
                NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
                NetManager.instance.Send(new MsgBase(RequestCode.JoinScene.ToString(), JsonUtility.ToJson(new JoinScene("1", UserInfoManager.userID))));
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd, WebRequestFuncitons.NullFunc, true, JsonMapper.ToJson(new HorseIdData(UserInfoManager.mountHorseID)), RFrameWork.instance.token);
                GameMapManager.instance.LoadGameScene(ConStr.GAMESCENE, FilesName.PLAYPANEL, StartLoadTerrain);
                UserInfoManager.CloseMainScenesObjectScript(); //离开主场景调用
            });
            Refresh(m_Data);
        }
        public void ReturnAct()
        {
            UserInfoManager.isLookGame = 0;
            UserInfoManager.detailPanelType = 3;
            UIManager.instance.CloseWnd(FilesName.GAMELISTPANEL);
            string[] arr = new string[2] { "2", "9" };
            HorseListType type = new HorseListType(arr);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.CanPlayGame, true, JsonMapper.ToJson(type), RFrameWork.instance.token);
            UserInfoManager.RoomId = int.Parse(this.data.id);
        }

        private void StartLoadTerrain(Action obj)
        {
            ObjectManager.instance.InstantiateObjectAsync("Assets/GameData/Prefabs/Building/Racecourse/Terrain2.prefab", (path, go/*GameObject*/, param1, param2, param3) =>
            {
                // GameMapManager.instance.LoadScene();

                Debug.Log("StartLoadTerrain finish" + go.name);
                GameObject instantiateGo = go as GameObject;
                instantiateGo.SetActive(true);
                instantiateGo.transform.localPosition = new Vector3(-142f, -18.6f, 0);
                GameObject horseClone = ObjectManager.instance.InstantiateObject("Assets/GameData/Prefabs/Animals/HorseLow.prefab", false, true);
                horseClone.name = "HorseClone";
                horseClone.gameObject.SetActive(false);
                UserInfoManager.horseClone = horseClone;
                UIManager.instance.CloseWnd(FilesName.TOPREPAREPANEL);
                UIManager.instance.PopUpWnd(FilesName.ROOMPANEL, true, false);

                NetManager.instance.Send(new MsgBase(RequestCode.WatchBattle.ToString(), JsonUtility.ToJson(new JoinScene(this.data.id, UserInfoManager.userID))));
                EcsManager.ClearAllPlayer();

            }, LoadResPriority.RES_HIGHT, false, null, null, null, true);
        }

        public void Refresh(GameData data)
        {
            NameText.text = "(房间ID：" + data.roomNumber + ")";
            statusWait.gameObject.SetActive(data.status == "0");
            statusDoing.gameObject.SetActive(data.status == "1");
            statusOver.gameObject.SetActive(data.status == "2");
            float v = (float)data.number / (float)data.maxNumber;
            sliderText.text = data.number + "/" + data.maxNumber;
            slider.value = v;
            Join.gameObject.SetActive(data.status == "0");
            LookGame.gameObject.SetActive(data.status == "1");
            Look.gameObject.SetActive(data.status == "2");
            timeText.gameObject.SetActive(data.status == "2");
            slider.gameObject.SetActive(data.status != "2");
            timeText.text = data.endDatetime; // dt.ToString("yyyy/MM/dd HH:mm:ss");//转化为日期时间
        }
    }
}
