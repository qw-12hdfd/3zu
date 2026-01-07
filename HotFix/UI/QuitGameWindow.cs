using HotFix.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace HotFix
{
    public class QuitGameWindow:Window
    {
        private Button quitBtn;
        private Button sureBtn;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            quitBtn = m_Transform.Find("Bg/QuitBtn").GetComponent<Button>();
            sureBtn = m_Transform.Find("Bg/SureBtn").GetComponent<Button>();
            AddButtonClickListener(quitBtn,QuitFunc);
            AddButtonClickListener(sureBtn,SureFunc);
        }

        private void SureFunc()
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkRole, WebRequestFuncitons.CheckRole, true, "{}", RFrameWork.instance.token);
            UserInfoManager.isGoToSiyangchang = true;
            UserInfoManager.enterGame = false;
            UserInfoManager.noHorse = true;
            GameMapManager.instance.LoadScene(ConStr.MAINSCENE, FilesName.MAINPANEL, HouseManager.LoadMainScene);
        }

        private void QuitFunc()
        {
            ToolManager.StartExitGame();
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {

        }
    }
}
