using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HotFix
{
    internal class CommonDataWindow : Window
    {

        GameObject panel1; // 租赁马匹
        GameObject panel2; // 马匹配额
        GameObject panel3; // 租赁记录
        GameObject panel4; // 我的租赁
        GameObject panel5; // 所有马匹

        Button myPeieBtn;   //马匹配额
        Button myChuzuBtn;  // 我的出租
        Button myZulinBtn;  // 我租赁的马匹
        Button quitBtn;     // 退出
        Button allBtn;     // 全部
        Button chuZuBtn;     // 出租中
        Button zuLinBtn;     // 租赁中
        Text quotaTitleText;// 标题



        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            base.Awake(param1, param2, param3);
            FindAllComponent();
            AddAllBtnListtener();
        }


        //查找所有组件

        private void FindAllComponent()
        {

            panel1 = m_Transform.Find("LeaseHorseList").gameObject;
            panel2 = m_Transform.Find("QuotaList").gameObject;
            panel3 = m_Transform.Find("HistoryList").gameObject;
            panel4 = m_Transform.Find("MyRentOutList").gameObject;
            panel5 = m_Transform.Find("HorseRentOutList").gameObject;
            myPeieBtn = m_Transform.Find("LeaseHorseList/HirtoryBtn").GetComponent<Button>();
            myChuzuBtn = m_Transform.Find("HorseRentOutList/RightBtns/RentOutBtn").GetComponent<Button>();
            myZulinBtn = m_Transform.Find("HorseRentOutList/RightBtns/RentOutHorseBtn").GetComponent<Button>();
            quitBtn = m_Transform.Find("QuitBtn").GetComponent<Button>();
            allBtn = m_Transform.Find("MyRentOutList/Btns/All").GetComponent<Button>();
            chuZuBtn = m_Transform.Find("MyRentOutList/Btns/Doing").GetComponent<Button>();
            zuLinBtn = m_Transform.Find("MyRentOutList/Btns/Quota").GetComponent<Button>();
            quotaTitleText = quitBtn.transform.Find("QuotaText").GetComponent<Text>();

        }

        //绑定所有按钮事件
        private void AddAllBtnListtener()
        {
            // 马匹配额
            AddButtonClickListener(myPeieBtn, () =>
            {
                UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, new object[] { 3 }, null, null);
            });

            // 我的出租 → 我的租赁
            AddButtonClickListener(myChuzuBtn, () =>
            {
                UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, new object[] { 4, 1 }, null, null);

            });

            // 我租赁的马匹 → 租赁马匹
            AddButtonClickListener(myZulinBtn, () =>
            {
                UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, new object[] { 4, 2 }, null, null);
            });

            // 退出按钮 → 关闭窗口
            AddButtonClickListener(quitBtn, () =>
            {
                UIManager.instance.CloseWnd(FilesName.COMMONDATAPANEL);
            });
        }

        // 显示对应界面

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {

            int showType = param1 == null ? 5 : (int)(param1 as object[])[0];

            // 显示对应面板x
            panel1.SetActive(showType == 1);
            panel2.SetActive(showType == 2);
            panel3.SetActive(showType == 3);
            panel4.SetActive(showType == 4);
            panel5.SetActive(showType == 5);

            if (showType == 4)
            {
                int showType2 = param1 == null ? 5 : (int)(param1 as object[])[1];

                allBtn.transform.GetChild(0).gameObject.SetActive(showType2 == 3);
                chuZuBtn.transform.GetChild(0).gameObject.SetActive(showType2 == 1);
                zuLinBtn.transform.GetChild(0).gameObject.SetActive(showType2 == 2);
            }


            // 更新标题
            UpdateTitle(showType);
        }


        private void UpdateTitle(int showType)
        {
            switch (showType)
            {
                case 1: quotaTitleText.text = "租赁马匹"; break;
                case 2: quotaTitleText.text = "马匹配额"; break;
                case 3: quotaTitleText.text = "租赁记录"; break;
                case 4: quotaTitleText.text = "我的租赁"; break;
                case 5: quotaTitleText.text = "所有马匹"; break;
                default: quotaTitleText.text = "所有马匹"; break;
            }
        }
    }
}