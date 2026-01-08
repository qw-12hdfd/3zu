using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HotFix
{
    internal class CommonDataWindow : Window
    {
        // 5个界面面板（对应5种状态）
        GameObject panel1; // 租赁马匹（参数1）
        GameObject panel2; // 马匹配额（参数2）
        GameObject panel3; // 租赁记录（参数3）
        GameObject panel4; // 我的租赁（参数4）
        GameObject panel5; // 所有马匹（参数5）

        // 功能按钮
        Button myXiangqingBtn; // 所有马匹-马匹详情
        Button myPeieBtn;     // 所有马匹-马匹配额
        Button myChuzuBtn;  // 所有马匹-我的出租
        Button myZulinBtn;  // 所有马匹-我租赁的马匹
        Button quitBtn;     // 退出按钮
        Text quotaTitleText;// 标题文本

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            base.Awake(param1, param2, param3);
            FindAllComponent();
            AddAllBtnListtener();
            HideAllPanels();
        }

        /// <summary>
        /// 查找所有控件（直接按路径获取，不做存在性判断）
        /// </summary>
        private void FindAllComponent()
        {
            // 面板
            panel1 = m_Transform.Find("LeaseHorseList").gameObject;
            panel2 = m_Transform.Find("QuotaList").gameObject;
            panel3 = m_Transform.Find("HistoryList").gameObject;
            panel4 = m_Transform.Find("MyRentOutList").gameObject;
            panel5 = m_Transform.Find("HorseRentOutList").gameObject;

            // 功能按钮
            myXiangqingBtn = m_Transform.Find("HorseRentOutList/Viewport/Content/Item/Btn").GetComponent<Button>();
            myPeieBtn = m_Transform.Find("LeaseHorseList/HirtoryBtn").GetComponent<Button>();
            myChuzuBtn = m_Transform.Find("HorseRentOutList/RightBtns/RentOutBtn").GetComponent<Button>();
            myZulinBtn = m_Transform.Find("HorseRentOutList/RightBtns/RentOutHorseBtn").GetComponent<Button>();
            quitBtn = m_Transform.Find("QuitBtn").GetComponent<Button>();

            quotaTitleText = quitBtn.transform.Find("QuotaText").GetComponent<Text>();

        }

        /// <summary>
        /// 绑定所有按钮事件（按需求实现核心交互）
        /// </summary>
        private void AddAllBtnListtener()
        {
            //马匹详情
            AddButtonClickListener(myXiangqingBtn, () =>
            {
                UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, 2, null, null);
            });
            AddButtonClickListener(myPeieBtn, () =>
            {
                UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, 3, null, null);
            });



            // 我的出租 → 我的租赁（参数4）
            AddButtonClickListener(myChuzuBtn, () =>
            {
                UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, 4, null, null);
            });

            // 我租赁的马匹 → 租赁马匹（参数1）
            AddButtonClickListener(myZulinBtn, () =>
            {
                UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, 1, null, null);
            });

            // 退出按钮 → 关闭窗口
            AddButtonClickListener(quitBtn, () =>
            {
                UIManager.instance.CloseWnd(FilesName.COMMONDATAPANEL);
            });

        }

        /// <summary>
        /// 隐藏所有面板
        /// </summary>
        private void HideAllPanels()
        {
            panel1.SetActive(false);
            panel2.SetActive(false);
            panel3.SetActive(false);
            panel4.SetActive(false);
            panel5.SetActive(false);
        }

        /// <summary>
        /// 按参数显示对应界面（核心逻辑）
        /// </summary>
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            // 参数为空默认显示所有马匹（主界面租赁按钮触发）
            int showType = param1 == null ? 5 : Convert.ToInt32(param1);

            HideAllPanels();

            // 显示对应面板
            panel1.SetActive(showType == 1);
            panel2.SetActive(showType == 2);
            panel3.SetActive(showType == 3);
            panel4.SetActive(showType == 4);
            panel5.SetActive(showType == 5);

            // 更新标题
            UpdateTitle(showType);

            // 我的租赁界面显示倒计时（按策划案固定文本）
            if (showType == 4)
            {
                Text countDown = panel4.transform.Find("Viewport/Content/Item/CountDown").GetComponent<Text>();
                countDown.text = "距可撤销23:50:60";
            }
        }

        /// <summary>
        /// 更新界面标题
        /// </summary>
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

    // 主场景租赁按钮逻辑
    internal class MainLeaseBtn : MonoBehaviour
    {
        private Button leaseBtn;

        private void Awake()
        {
            leaseBtn = GetComponent<Button>();
            AddButtonClickListener(leaseBtn, () =>
            {
                // 打开所有马匹界面（参数5）
                UIManager.instance.PopUpWnd(FilesName.COMMONDATAPANEL, true, false, 5, null, null);
            });
        }

        private void AddButtonClickListener(Button btn, Action action)
        {
            btn.onClick.AddListener(new UnityAction(action));
        }
    }
}