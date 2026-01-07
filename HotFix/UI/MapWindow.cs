using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
  public  class MapWindow: Window
    {
        private   Button siYangChangBtn;
        private Button fanTuChangBtn;
        private Button saiMaChangBtn;
        private Button closeBtn;
        RawImage rawImage;
        Camera camera;
        RenderTexture showTest=new RenderTexture(512, 512, 16,RenderTextureFormat.ARGB32);
        public override void Awake(object param1 = null, object param2 = null, object param3 = null) { 
            GetALLComponent();
            AddALLBtnistener();
        }
        void AddALLBtnistener() { 
            AddButtonClickListener(closeBtn, CloseWndFunc);
            AddButtonClickListener(siYangChangBtn, GoToSiYangChang);
            AddButtonClickListener(fanTuChangBtn, GoToSaiMachang);
            AddButtonClickListener(saiMaChangBtn, GoToFanYunchang);

        }
        void CloseWndFunc()
        {
            UIManager.instance.CloseWnd(this);
        }
        void GoToSiYangChang() {
            RFrameWork.instance.OpenCommonConfirm("提示", "前往饲养厂?", () =>
            {
                MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(1));
            }, () => { });
        }
        void GoToSaiMachang()
        {
            RFrameWork.instance.OpenCommonConfirm("提示", "前往赛马厂?", () =>
            {
                MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(2));
            }, () => { });
        }
        void GoToFanYunchang()
        {
            RFrameWork.instance.OpenCommonConfirm("提示", "前往繁育厂?", () =>
            {
                MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(3));
            }, () => { });
           
        }
        void GetALLComponent() {
            siYangChangBtn=m_Transform.Find("MapBack/siyangchang").GetComponent<Button>();
            fanTuChangBtn = m_Transform.Find("MapBack/fanzhichang").GetComponent<Button>();
            saiMaChangBtn = m_Transform.Find("MapBack/saimachang").GetComponent<Button>();
            closeBtn=m_Transform.Find("MapBack/Close").GetComponent<Button>();
            rawImage = m_Transform.Find("MapBack/RawImage").GetComponent<RawImage>();
            camera = m_Transform.Find("Camera").GetComponent<Camera>();
        }
    }
}
