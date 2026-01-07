using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    class HorseFeedPanel : Window
    {
        public void HorseStartPanel()
        {
            Window horsePanel = UIManager.instance.PopUpWnd(FilesName.HORSEFEEDPANEL, true, false);

        }
    }
}
