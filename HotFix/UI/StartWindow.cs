using UnityEngine.UI;
using LitJson;
using UnityEngine;
using System;
using System.Collections.Generic;
using HotFix.Common.Utils;

namespace HotFix
{
    /// <summary>
    /// 游戏初始动画界面
    /// </summary>
    class StartWindow : Window
    {
        /// <summary>
        /// 开始游戏按钮
        /// </summary>
        public Button startGame;
        private Text text;

        public static Action<string> setText;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            startGame = m_Transform.Find("StartGame").GetComponent<Button>();
            startGame.interactable = true;
            text = m_Transform.Find("StartGame/Text").GetComponent<Text>();
            Debug.Log("开始游戏拉");
            setText = setTextData;
            AddButtonClickListener(startGame, () =>
            {
                Debug.Log(RFrameWork.instance.token);
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkUserUrl, WebRequestFuncitons.CheckHorseNum, true, "{}", RFrameWork.instance.token);
                startGame.interactable = false;
                text.text = "------开始加载------";
            });
        }

        private void setTextData(string obj)
        {
            text.text = obj;
        }
    }
}
