using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    internal class LeaseWindow: Window
    {
        public void LeasePanel()
        {
            Window RentPanel = UIManager.instance.PopUpWnd(FilesName.RENTPANEL, true, false);
            Transform transform = RentPanel.m_Transform.Find("BackImg/CloseBtn");
            Button startButton= transform.GetComponent<Button>();
            startButton.onClick.AddListener(() =>
            {
                UIManager.instance.CloseWnd(FilesName.RENTPANEL);
            });
        }
    }
}
