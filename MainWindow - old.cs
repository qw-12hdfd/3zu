using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using LitJson;
using HotFix.Common.Utils;

namespace HotFix
{
    class MainWindow:Window
    {
        /// <summary>
        /// 开始游戏按钮
        /// </summary>
        private Button startGame;
        /// <summary>
        /// 开始游戏按钮
        /// </summary>
        private Button MyHourse;
        /// <summary>
        /// 开始游戏按钮
        /// </summary>
        private Button History;
        /// <summary>
        /// 退出游戏按钮
        /// </summary>
        private Button Quit;

        private Text name;

        private Image Icon;

        private static Transform Item;
        private static Dictionary<int, RoomDatas> roomDatas;
        private static Dictionary<int, MainItem> itemDic = new Dictionary<int, MainItem>();

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            RFrameWork.instance.SetBackAudio("SingleSounds/MainBack");
            Debug.Log("MainWindow Awake 222:");
            string myName = (string)param1;
            Texture tx = (Texture)param2;
            string id = (string)param3;
            UserInfoManager.userID = id;
            Debug.Log("MainWindow Awake 获取玩家ID了:"+ id);
            startGame = m_GameObject.transform.Find("StartGame").GetComponent<Button>();
            MyHourse = m_GameObject.transform.Find("Tip/BagBtn").GetComponent<Button>();
            History = m_GameObject.transform.Find("Tip/HistoryBtn").GetComponent<Button>();
            Quit = m_GameObject.transform.Find("Tip/QuitBtn").GetComponent<Button>();
            name = m_GameObject.transform.Find("Tip/Name").GetComponent<Text>();
            Icon = m_GameObject.transform.Find("Tip/Icon/IconBack/MyIcon").GetComponent<Image>();
            Item = m_GameObject.transform.Find("GameList/Viewport/Content/GameItem");
            Item.gameObject.SetActive(false);
            roomDatas = new Dictionary<int, RoomDatas>();
            roomDatas = ServerRequestManager.GetJoinHall();
            name.text = myName;
            Sprite sprite = Sprite.Create((Texture2D)tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
            Icon.sprite = sprite;
            AddButtonClickListener(startGame, () =>
            {
                UserInfoManager.SetIsCreate(true);
                NumberOfPages data = new NumberOfPages(1, 30);
                string jsonStr = JsonMapper.ToJson(data);
                UserInfoManager.selectOrSelectAndCreate = true;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseOwnerUrl, WebRequestFuncitons.GetHorseData, true, jsonStr, RFrameWork.instance.token);
            });
            AddButtonClickListener(MyHourse, () =>
            {
                NumberOfPages data = new NumberOfPages(1, 30);
                string jsonStr = JsonMapper.ToJson(data);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseOwnerUrl, WebRequestFuncitons.GetHorseData1, true, jsonStr, RFrameWork.instance.token);
            });
            AddButtonClickListener(History, () =>
            {
                NumberOfPages data = new NumberOfPages(1, 30);
                string jsonStr = JsonMapper.ToJson(data);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseRecordUrl, WebRequestFuncitons.GetRecordData, true, jsonStr, RFrameWork.instance.token);
            });
            AddButtonClickListener(Quit, () =>
            {
                RFrameWork.instance.OpenCommonConfirm("", "确认退出赛马元宇宙？", QuitGame, () => { } );
            });
            foreach (var item in roomDatas)
            {
                SetItems(item);
            }
            MessageCenter.instance.AddListener((int)MessageCenterEventID.RefreshMainItem, RefreshItem);
            MessageCenter.instance.AddListener((int)MessageCenterEventID.RefreshAllMainItem, RefreshAllItem);

        }

        private void QuitGame()
        {
            JsonData data = new JsonData();
            MsgBase close = new MsgBase("Close", data.ToJson());
            NetManager.instance.Send(close) ;
            NetManager.instance.Close();
            RFrameWork.instance.QuitTheGame();
        }

        private void RefreshAllItem(Notification obj)
        {
            Dictionary<int, RoomDatas> roomDatasRefresh = new Dictionary<int, RoomDatas>();
            roomDatasRefresh = (Dictionary<int, RoomDatas>)obj.content[0];
            if (itemDic.Count > 0)
            {
                itemDic.Clear();
                itemDic = new Dictionary<int, MainItem>();
            }
            for (int i = 1; i < Item.parent.GetChildCount(); i++)
            {
                GameObject.DestroyObject(Item.parent.GetChild(i).gameObject);
            }
            foreach (var item in roomDatasRefresh)
            {
                SetItems(item);
            }
        }

        private void RefreshItem(Notification obj)
        {
            Dictionary<int,RoomDatas> roomDatasRefresh = new Dictionary<int,RoomDatas>();
            roomDatasRefresh = (Dictionary<int, RoomDatas>)obj.content[0];
            foreach (var item in roomDatasRefresh)
            {
                SetItems(item);
            }
        }

        public static void SetItems(KeyValuePair<int, RoomDatas> item)
        {
            if (itemDic.ContainsKey(item.Key))
            {
                itemDic[item.Key].Refresh(item.Value);
            }
            else
            {
                itemDic[item.Key] = new MainItem();
                itemDic[item.Key].Init(item.Value, Item);
            }
        }
    }
}
