using System;
using UnityEngine.UI;

namespace HotFix
{
    internal class HorseFeedWindow : Window
    {
        private Button closeBtn;



        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GatAllComponent();
            AddAllBtnListener();
        }
        void AddAllBtnListener()
        {

            AddButtonClickListener(closeBtn, ClosePanel);

        }
        private void ClosePanel()
        {
            // 确保正确关闭当前窗口
            UIManager.instance.CloseWnd(FilesName.HORSEFEEDPANEL);
        }
        private void GatAllComponent()
        {
            closeBtn = m_Transform.Find("CloseBtn").GetComponent<Button>();
        }
    }
}