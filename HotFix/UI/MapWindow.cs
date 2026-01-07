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
        private Button fanYuChangBtn;
        private Button saiMaChangBtn;
        private Button closeBtn;

        RawImage rapImage;

        Camera camera;
        RenderTexture rt = new RenderTexture(520, 520, 20, RenderTextureFormat.ARGB32);

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GatAllComponent();
            AddAllBtnListener();
            camera.targetTexture = rt;
            rapImage.texture = rt;
        }

        void AddAllBtnListener()
        {
            AddButtonClickListener(closeBtn, () => { UIManager.instance.CloseWnd(this); });
            AddButtonClickListener(siYangChangBtn, GoToSiYangChang);
            AddButtonClickListener(fanYuChangBtn, GoToSaiYangChang);
            AddButtonClickListener(saiMaChangBtn, GoToFanYangChang);


        }

        private void GoToFanYangChang()
        {
            Debug.Log("点击赛马场按钮");
            RFrameWork.instance.OpenCommonConfirm("提示", "是否传送赛马场？", () =>
            {
                MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(2));

            }, () => { });
        }
        //hotfix中
        //弹窗逻辑
        //网络 hotfix中所有webrequestmanager替换为//webrequestmananger  把所有web网络相关内容全部注释掉
        //serrequestmanager中所有添加的事件全部注释掉

        //asset里面的脚本 rframework
        //rframework中 netmananger相关内容全部注释掉
        //赛马场
        private void GoToSaiYangChang()
        {
            Debug.Log("点击赛马场按钮");
            RFrameWork.instance.OpenCommonConfirm("提示", "是否传送繁育场？", () =>
            {
                MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(3));

            }, () => { });
            // 

        }

        void GoToSiYangChang()
        {
            Debug.Log("点击赛马场按钮");
            RFrameWork.instance.OpenCommonConfirm("提示", "是否传送饲养场？", () =>
            {
                MessageCenter.instance.Dispatch(MessageCenterEventID.PlayerChangePosition, new Notification(1));

            }, () => { });
        }


        void CloseWndFunc()
        {
            UIManager.instance.CloseWnd(this);
        }

        private void GatAllComponent()
        {
            siYangChangBtn = m_Transform.Find("MapBack/siyangchang").GetComponent<Button>();
            fanYuChangBtn = m_Transform.Find("MapBack/fanzhichang").GetComponent<Button>();
            saiMaChangBtn = m_Transform.Find("MapBack/saimachang").GetComponent<Button>();
            closeBtn = m_Transform.Find("MapBack/Close").GetComponent<Button>();
            rapImage = m_Transform.Find("MapBack/RawImage").GetComponent<RawImage>();
            camera = m_Transform.Find("Camera").GetComponent<Camera>();
        }
    }
}
