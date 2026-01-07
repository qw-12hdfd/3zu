using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class MapWindow : Window
    {
        private Button siYangChangBtn;
        private Button fanTuChangBtn;
        private Button saiMaChangBtn;
        private Button closeBtn;
        RawImage rawImage;
        Camera camera;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetALLComponent();
            AddALLBtnistener();
        }

        void AddALLBtnistener()
        {
            AddButtonClickListener(closeBtn, CloseWndFunc);
            AddButtonClickListener(siYangChangBtn, GoToSiYangChang);
            AddButtonClickListener(fanTuChangBtn, GoToSaiMaChang);
            AddButtonClickListener(saiMaChangBtn, GoToFanYuChang);
        }

        void CloseWndFunc()
        {
            UIManager.instance.CloseWnd(this);
        }

        private void GoToSiYangChang()
        {
            Debug.Log("点击赛马场按钮");
            RFrameWork.instance.OpenCommonConfirm("提示", "是否传送繁育场？", () =>
            {
                MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(3));

            }, () => { });
            // 

        }

        void GoToSaiMaChang()
        {
            MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(2));
        }

        void GoToFanYuChang()
        {
            RFrameWork.instance.OpenCommonConfirm("提示", "前往繁育场？",()=>{
                MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(1));
            }, () => { });
           
        }

        void GetALLComponent()
        {
            siYangChangBtn = m_Transform.Find("MapBack/siyangchang").GetComponent<Button>();
            fanTuChangBtn = m_Transform.Find("MapBack/fanzhichang").GetComponent<Button>();
            saiMaChangBtn = m_Transform.Find("MapBack/saimachang").GetComponent<Button>();
            closeBtn = m_Transform.Find("MapBack/Close").GetComponent<Button>();
            rawImage = m_Transform.Find("MapBack/RawImage").GetComponent<RawImage>();
            camera = m_Transform.Find("Camera").GetComponent<Camera>();
        }
    }
}