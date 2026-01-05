using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.Events;
using System.Data;
using HotFix.Common.Utils;

namespace HotFix
{

    class SelectPlayerWindow : Window
    {
        Button startGame; //开始游戏按钮
        Button exitGame; //退出游戏按钮
        Button selectLeft; 
        Button selectRight;
        Transform boy;
        Transform girl;
        Text text;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            //获取组件
            startGame = m_Transform.Find("BackGround/StartOrReturn").GetComponent<Button>();
            exitGame = m_Transform.Find("BackGround/Exit").GetComponent<Button>();
            boy = m_Transform.Find("Camera/Pos/RiderBoy");
            girl = m_Transform.Find("Camera/Pos/RiderGirl");
            selectLeft = m_Transform.Find("BackGround/ChangeBtn/LeftButton").GetComponent<Button>();
            selectRight = m_Transform.Find("BackGround/ChangeBtn/RightButton").GetComponent<Button>();
            //绑定事件
            AddButtonClickListener(startGame, StartGame);
            AddButtonClickListener(exitGame, ExitGame);
            AddButtonClickListener(selectLeft, SelectPeople);
            AddButtonClickListener(selectRight, SelectPeople);
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            boy.gameObject.SetActive(false);
            girl.gameObject.SetActive(false);
            if (GameMapManager.instance.CurrentMapName == ConStr.MAINSCENE)
            {
                exitGame.gameObject.SetActive(false);
                startGame.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                exitGame.gameObject.SetActive(true);
                startGame.transform.GetChild(0).gameObject.SetActive(false);
            }
            SelectPeople();
        }

        /// <summary>
        /// 选择女性人物的方法
        /// </summary>
        private void SelectPeople()
        {
            UserInfoManager.Sex = UserInfoManager.Sex == 1?2:1;
            if (UserInfoManager.Sex == 1)
            {
                boy.gameObject.SetActive(true);
                girl.gameObject.SetActive(false);
            }
            else
            {
                boy.gameObject.SetActive(false);
                girl.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 选择完角色进入游戏
        /// </summary>
        public void StartGame()
        {
            if(GameMapManager.instance.CurrentMapName == ConStr.MAINSCENE)
            {
                UserInfoManager.playerCtrl.SetSex(UserInfoManager.Sex.ToString());
                UIManager.instance.CloseWnd(this);
            }
            else
            {
                UIManager.instance.CloseWnd(this, true);
                UserInfoManager.isGoToSiyangchang = true;
                UserInfoManager.enterGame = false;
                GameMapManager.instance.LoadScene(ConStr.MAINSCENE, FilesName.MAINPANEL, HouseManager.LoadMainScene);
            }
            JsonData data = new JsonData();
            data["metaRoleId"] = UserInfoManager.Sex;
            string str = JsonMapper.ToJson(data);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.enterTheGame, EnterTheGame, true, str, RFrameWork.instance.token);
        }

        private void EnterTheGame(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            string code = jsonData["code"].ToString();
            if (code.Equals("200"))
            {

            }
            else
            {
                RFrameWork.instance.OpenCommonConfirm("提示", jsonData["errorMsg"].ToString(), () => { }, null);
            }
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void ExitGame()
        {
            RFrameWork.instance.OpenCommonConfirm("", "确认退出马术元宇宙？", () => { ToolManager.ExitGame(); }, () => { });
        }
    }
}
