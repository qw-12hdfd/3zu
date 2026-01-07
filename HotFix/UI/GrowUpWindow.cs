using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    internal class GrowUpWindow:Window
    {
        private Text titleText;
        private Text title1;
        private Text title2;
        private Text title3;
        private Text des1;
        private Text des2;
        private Text des3;
        private Text desText1;
        private Text desText2;
        private Text desText3;
        private Button closeBtn;
        private Dictionary<string, string> dic;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            titleText = m_Transform.Find("Back/TitleBack/TitleText").GetComponent<Text>();
            title1 = m_Transform.Find("Back/GameObject/baby/TipsBack/TitleImage/TypeNameText").GetComponent<Text>();
            title2 = m_Transform.Find("Back/GameObject/adult/TipsBack/TitleImage/TypeNameText").GetComponent<Text>();
            title3 = m_Transform.Find("Back/GameObject/old/TipsBack/TitleImage/TypeNameText").GetComponent<Text>();
            des1 = m_Transform.Find("Back/GameObject/baby/TipsBack/Des").GetComponent<Text>();
            des2 = m_Transform.Find("Back/GameObject/adult/TipsBack/Des").GetComponent<Text>();
            des3 = m_Transform.Find("Back/GameObject/old/TipsBack/Des").GetComponent<Text>();
            desText1 = m_Transform.Find("Back/GameObject/baby/TipsBack/DesText").GetComponent<Text>();
            desText2 = m_Transform.Find("Back/GameObject/adult/TipsBack/DesText").GetComponent<Text>();
            desText3 = m_Transform.Find("Back/GameObject/old/TipsBack/DesText").GetComponent<Text>();
            closeBtn = m_Transform.Find("Back/Close").GetComponent<Button>();
            AddButtonClickListener(closeBtn, () =>
            {
                UIManager.instance.CloseWnd(this);
            });
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            dic = param1 as Dictionary<string, string>;
            int type = int.Parse(param2.ToString());
            if(type== 1)
            {
                titleText.text = "成长阶段";
                title1.text = "   小马   ";
                title2.text = "   成年马   ";
                title3.text = "   老年马   ";
                des1.text = dic["hoser_grow_up_child_time_note"];
                des2.text = dic["hoser_grow_up_young_time_note"];
                des3.text = "";
                desText1.text = dic["hoser_grow_up_child_action_two"];
                desText2.text = dic["hoser_grow_up_young_action_note"];
                desText3.text = dic["hoser_grow_up_old_action_note"];
            }
            else
            {// 配额
                titleText.text = "配额说明";
                title1.text = "   配额说明   ";
                title2.text = "   获取方法   ";
                title3.text = "   特殊说明   ";
                des1.text = "";
                des2.text = "";
                des3.text = "";
                desText1.text = dic["horse_share_note"];
                desText2.text = dic["horse_share_get_method"];
                desText3.text = dic["horse_share_special_note"];
            }
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(m_Transform.GetComponent<RectTransform>());
        }
    }
}
