using LitJson;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class ToPrepareWindow : Window
    {
        private Slider slider;
        private Text progressText;
        private Transform[] playerItems;
       /* private PlayerInfoData[] playerDatas;
        private RankInfoData[] rankDatas;*/
        /// <summary>
        /// 是否执行过弹出界面的方法
        /// </summary>
        bool isOpen = false;
        private bool canHidePanel = false;
        /// <summary>
        /// 加载界面关闭后要显示的界面
        /// </summary>
        private string m_TargetPanelName;
        private string sendData;

        private Action<Action> action = null;
        public static Action<string> LoadFinishProgressAction;

        private object[] objs = null;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            LoadFinishProgressAction = LoadFinishProgressResponse;
            //object[] params1 = (object[])param1;
            object[] params2 = (object[])param2;
            /*playerDatas = (PlayerInfoData[])params1[0];
            rankDatas = (RankInfoData[])params1[1];*/
            //sendData = params1[0].ToString();
            Debug.Log("ToPreparePanel Awake:");
            m_TargetPanelName = (string)params2[0];
            action = (Action<Action>)params2[1];
            isOpen = false;
            slider = m_Transform.Find("Slider").GetComponent<Slider>();
            progressText = m_Transform.Find("Slider/Text").GetComponent<Text>();
            Transform contents = m_Transform.Find("PlayerList/Viewport/Content");
            playerItems = new Transform[contents.childCount];
            for (int i = 0; i < contents.childCount; i++)
            {
                playerItems[i] = contents.GetChild(i);
            }
            for (int i = 0; i < playerItems.Length; i++)
            {
                playerItems[i].transform.Find("ItemInfo").gameObject.SetActive(false);
            }
            InitTextValue();
            //EnterRoom(playerDatas);


        }
        private void InitTextValue()
        {
            progressText.text = "加载中：0 %";
            slider.value = 0;
        }
       /* private void EnterRoom(PlayerInfoData[] infoDatas)
        {
            for (int i = 0; i < infoDatas.Length; i++)
            {
                PlayerItem item = new PlayerItem();
                item.Init(infoDatas[i], playerItems[i].transform);
            }

        }*/
        #region
        private void LoadFinishProgressResponse(string data)
        {

            Debug.Log("ToPrepareWindow LoadFinishProgressResponse 接收到的消息是：" + data);
            JsonData jsonData = JsonMapper.ToObject(data);
            float progress = float.Parse(jsonData["data"].ToString());
            slider.value = progress;
            Debug.Log("ToPrepareWindow LoadFinishProgressResponse 进度：" + progress);
            progressText.text = "加载中：" + progress + " %";
            slider.value = progress / 100f;
            if (jsonData["data"].ToString().Equals("100"))
            {
                LoadOtherScene();
            }


        }
        #endregion
        public override void OnUpdate()
        {
            if (GameMapManager.LoadingProgress >= 88 && isOpen == false)
            {
                isOpen = true;
                if (action != null)
                {

                    action.Invoke(LoadOtherScene);
                }
                else
                {
                    LoadOtherScene();
                }
            }
        }

        /// <summary>
        /// 加载对应场景第一个UI
        /// </summary>
        public void LoadOtherScene()
        {
            Debug.Log("关闭Preapre界面");
            UIManager.instance.CloseWnd(FilesName.TOPREPAREPANEL);
            string tarPanelName = m_TargetPanelName;
            Debug.Log("准备界面要打开的界面是：" + tarPanelName + "传送的数据：" + sendData);

            UIManager.instance.PopUpWnd(tarPanelName, true, false, sendData);
        }
    }
}

