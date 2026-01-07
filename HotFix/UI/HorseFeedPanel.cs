using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
     class HorseFeedWindow : Window
    {
        public void HorseStartPanel()
        {
            Window horseFeedPanel = UIManager.instance.PopUpWnd(FilesName.HORSEFEEDPANEL, true, false);

        }
    }
}
