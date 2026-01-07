using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace HotFix
{
    internal class PuppyMillWindow : Window
    {
        private Button openRoomBtn;
        private Button closeBtn;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            openRoomBtn = m_Transform.Find("openRoomBtn").GetComponent<Button>();
            AddButtonClickListener(openRoomBtn, OpenDetailPanel);
            AddButtonClickListener(closeBtn, OnClosePanel);
            
        }
        private void OnClosePanel()
        {
            UIManager.instance.CloseWnd(this);
        }
        //TODO打开详情页面选择马匹
        private void OpenDetailPanel()
        {
           //UIManager.instance.PopUpWnd(FilesName.)
        }

      
    }
}
