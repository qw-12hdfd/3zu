using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    class AllHorseItem
    {
        private Transform m_transform;
        private HorseDetail m_Data;
        private Text m_Name;
        private Text m_Price;
        private Button m_DetileBtn;
        private Button m_LeaseBtn;

        public void init(GameObject item, HorseDetail data)
        {
            item.gameObject.SetActive(true);
            m_transform = item.GetComponent<Transform>();
            m_Data = data;
            m_Name = m_transform.Find("Title").GetComponent<Text>();
            m_Price = m_transform.Find("Des/DesNum").GetComponent<Text>();
            m_DetileBtn = m_transform.Find("Btn").GetComponent<Button>();
            m_LeaseBtn = m_transform.Find("RentOut").GetComponent<Button>();
         SetData(data);
        }
        public void SetData(HorseDetail data) {
            m_Data = data;
            m_Name.text = m_Data.name;
            m_Price.text = m_Data.price.ToString();
            m_DetileBtn.onClick.RemoveAllListeners();
            m_DetileBtn.onClick.AddListener(() => {

            });

            m_LeaseBtn.onClick.RemoveAllListeners();
            m_LeaseBtn.onClick.AddListener(() => {

            });

        }
    }
   
}
