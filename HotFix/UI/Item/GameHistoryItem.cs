using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace HotFix
{
    internal class GameHistoryItem
    {
        private RawImage icon;
        private Text horseName;
        private Text gameName;
        private Text time;
        private Text rankNum;
        private Transform horseNum;

        public void Init( Transform item, HistoryDatas m_Data)
        {
            icon = ToolManager.FindBehaviour<RawImage>(item, "Icon");
            gameName = ToolManager.FindBehaviour<Text>(item, "GameName");
            time = ToolManager.FindBehaviour<Text>(item, "Time");
            rankNum = ToolManager.FindBehaviour<Text>(item, "RankNum");
            horseName = ToolManager.FindBehaviour<Text>(item, "Name");
            horseNum = item.Find("RankNum/IsWinner");
            item.gameObject.SetActive(true);
            Refresh(m_Data);
        }

        public void Refresh(HistoryDatas data)
        {
            WebRequestManager.instance.AsyncLoadUnityTexture(data.horsePic, (texture) =>
            {
                icon.texture = texture;
            });
            gameName.text = "(房间ID：" + data.roomNumber + ")";
            time.text = data.createDatetimeString; // dt.ToString("yyyy/MM/dd HH:mm:ss");//转化为日期时间
            rankNum.text = "第" + data.rank + "名";

            horseName.text = data.horseName.Length>5? data.horseName.Substring(0,5)+"...": data.horseName;
            horseNum.GetChild(0).gameObject.SetActive(data.rank == 1);
            horseNum.GetChild(1).gameObject.SetActive(data.rank == 2);
            horseNum.GetChild(2).gameObject.SetActive(data.rank == 3);

        }
    }
}
