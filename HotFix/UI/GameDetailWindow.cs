using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HotFix
{
    internal class GameDetailWindow:Window
    {
        public Text price;
        public Text peopleNum;
        public Text cancel;
        public Text award;
        public Transform back;
        public Button closeBtn;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            GetAllComponent();
            AddAllBtnListener();
        }

        private void GetAllComponent()
        {
            price = m_Transform.Find("BackImg/Scroll View/Viewport/Content/Price").GetComponent<Text>();
            peopleNum = m_Transform.Find("BackImg/Scroll View/Viewport/Content/PeopleNum").GetComponent<Text>();
            cancel = m_Transform.Find("BackImg/Scroll View/Viewport/Content/Cancel").GetComponent<Text>();
            award = m_Transform.Find("BackImg/Scroll View/Viewport/Content/Award").GetComponent<Text>();
            back = m_Transform.Find("BackImg/Scroll View/Viewport/Content/Back");
            closeBtn = m_Transform.Find("BackImg/CloseBtn").GetComponent<Button>();
        }

        private void AddAllBtnListener()
        {
            AddButtonClickListener(closeBtn, () => { UIManager.instance.CloseWnd(this); });
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            string[] strs = param1 as string[];
            Dictionary<string,string> dic = param2 as Dictionary<string,string>;
            UpdateUI(strs, dic);
        }

        private void UpdateUI(string[] strs, Dictionary<string, string> dic)
        {
            price.text = strs[0];
            peopleNum.text = strs[1];
            cancel.text = strs[2];
            award.text = strs[3];
            for (int i = 0; i < back.parent.childCount; i++)
            {
                if (back.parent.GetChild(i).gameObject.name.Contains("Back"))
                    back.parent.GetChild(i).gameObject.SetActive(false);
            }
            int count = 4;
            int nowCount = 0;
            foreach (var item in dic)
            {
                if (count + 1 <= back.parent.childCount)
                {
                    var trans = back.parent.GetChild(count);
                    trans.GetChild(0).GetChild(nowCount).GetComponent<Text>().text = item.Key;
                    trans.GetChild(0).GetChild(nowCount+3).GetComponent<Text>().text = item.Value;
                    trans.gameObject.SetActive(true);
                }
                else
                {
                    Transform obj = Object.Instantiate<GameObject>(back.gameObject, back.parent).transform;
                    var trans = back.parent.GetChild(count);
                    trans.GetChild(0).GetChild(nowCount).GetComponent<Text>().text = item.Key;
                    trans.GetChild(0).GetChild(nowCount + 3).GetComponent<Text>().text = item.Value;
                    trans.gameObject.SetActive(true);
                }
                nowCount++;
                if(nowCount >= 3)
                {
                    count++;
                    nowCount = 0;
                }
            }
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(back.parent.GetComponent<RectTransform>());
        }
    }
}
