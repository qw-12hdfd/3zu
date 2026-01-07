using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace HotFix
{
    public class ResultRankItem
    {
        public Transform rank1, rank2, rank3;
        public Text rankText;
        public Text horseName;
        public Text timeText;
        public Text goodsText;

        public void Init(Transform item, ResultRankData data,string horseId) 
        {
            rank1 = item.Find("BackImages/First");
            rank2 = item.Find("BackImages/Second");
            rank3 = item.Find("BackImages/Third");
            rankText = item.Find("Num").GetComponent<Text>();
            horseName = item.Find("Name").GetComponent<Text>();
            timeText = item.Find("Time").GetComponent<Text>();
            goodsText = item.Find("HorseFeedNum").GetComponent<Text>();
            item.gameObject.SetActive(true);
            if(horseId == data.userId)
                SettlementWindow.SetHorseCode(data.horseCode);
            UpdateUI(data);
            item.GetComponent<Button>().onClick.RemoveAllListeners();
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                UserInfoManager.detailPanelType = 7;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + data.userId, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
                DetailWindow.SetRankActive(data.rank);
            });
        }

        private void UpdateUI(ResultRankData data)
        {
            rank1.gameObject.SetActive(data.rank == 1);
            rank2.gameObject.SetActive(data.rank == 2);
            rank3.gameObject.SetActive(data.rank == 3);
            rankText.text = data.rank.ToString();
            horseName.text = data.nickname;
             var totalMilSeconds = int.Parse(data.useTime);
            int totalSecond = totalMilSeconds / 1000;
            int mile = totalMilSeconds % 1000;
            int minute = totalSecond / 60;
            int second = totalSecond % 60;
            Debug.Log("分：" + minute + " 秒：" + second + " 毫秒：" + mile + "  总秒数：" + totalSecond);
            string time = TimeUtils.CombineTime(minute) + ":" + TimeUtils.CombineTime(second) + ":" + TimeUtils.CombineTimeDouble(mile);
            timeText.text = time;
            goodsText.gameObject.SetActive(data.rank < 7);
            goodsText.text = "+"+((int)float.Parse(data.rewardAmount)).ToString();
        }
    }
}
