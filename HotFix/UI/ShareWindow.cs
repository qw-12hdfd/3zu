using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    internal class ShareWindow : Window
    {
        public void ShareStartPanel()
        {
            Window startPanel = UIManager.instance.PopUpWnd(FilesName.SHAREPANEL, true, false);
            Transform buttonTtansform = startPanel.m_Transform.Find("Back / ReturnBtn");
            Button startButton = buttonTtansform.GetComponent<Button>();
            startButton.onClick.AddListener(() => { UIManager.instance.CloseWnd(startPanel); });

        }
    }
}