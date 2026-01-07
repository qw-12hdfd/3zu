using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    internal class HistoryItem
    {
        private Text name;
        private Text time;
        private Text rankNum;
        private Text feedNum;
        private Transform isWinner;
        public void Init(Transform item, HorseHistoryData data)
        {
            Debug.Log(data.horseName);
            item.gameObject.SetActive(true);
            feedNum = ToolManager.FindBehaviour<Text>(item, "HorseNum");
            name = ToolManager.FindBehaviour<Text>(item, "Name");
            time = ToolManager.FindBehaviour<Text>(item, "Time");
            rankNum = ToolManager.FindBehaviour<Text>(item, "Num");
            isWinner = item.Find("IsWinner");
            RefreshItem(data);
        }

        private void RefreshItem(HorseHistoryData data)
        {
            feedNum.transform.parent.gameObject.SetActive(!((int)float.Parse(data.rewardAmount) <= 0));
            feedNum.text = "马粟+" + (int)float.Parse(data.rewardAmount);
            time.text = data.createDatetimeString; // dt.ToString("yyyy/MM/dd HH:mm:ss");//转化为日期时间
            rankNum.text = "第" + data.rank.ToString() + "名";
            isWinner.gameObject.SetActive(data.rank <= 3);
            isWinner.GetChild(0).gameObject.SetActive(data.rank == 1);
            isWinner.GetChild(1).gameObject.SetActive(data.rank == 2);
            isWinner.GetChild(2).gameObject.SetActive(data.rank == 3);
        }
    }
}
