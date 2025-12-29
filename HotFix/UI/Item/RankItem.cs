using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
 
namespace HotFix
{
    public class RankItem
    {
        private Text nameText,rankText;
        private Transform item;
        public string id;
        private string name;
        public int  rank;
        public float distance;
       
        public void Init(RankInfoData data,Transform item)
        {
            id = data.userId;
            this.item = item;
            Debug.Log("显示的排名UI:" + this.item.gameObject.name);
            this.item.gameObject.SetActive(true);
            this.nameText = this.item.Find("Name").GetComponent<Text>();
            this.rankText = this.item.Find("Num").GetComponent<Text>();
            UpdateUI(data.userId, data.nickname, data.track+1,data.distance);
        }

        public void UpdateUI(string id,string name,int rank,float dis)
        {
            nameText.text = name;
            Debug.Log("SetRankItemName +" + name);
            rankText.text = rank.ToString();
            if(id == UserInfoManager.horseID)
            {
                nameText.fontStyle = FontStyle.Bold;
                nameText.fontSize = 48;
                this.item.Find("Bg").gameObject.SetActive(true);
            }
            else
            {
                nameText.fontStyle = FontStyle.Normal;
                nameText.fontSize = 46;
                this.item.Find("Bg").gameObject.SetActive(false);
            }
            this.item.SetSiblingIndex(rank);
        }
        public void UpdateUI(int rank)
        {
            rankText.text = rank.ToString();
            this.item.SetSiblingIndex(rank);
        }
        public void HideUI()
        {
            this.item.gameObject.SetActive(false);
        }
    }
}
