using HotFix.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
 
namespace HotFix
{
    public class PlayStateItem : ObjectParent
    {
        private GameObject lightObj, userNameObj, stateObj;
        private MeshRenderer lightMatRender;
        private Material redMat,greenMat;
        private TextMesh horseNameText,prepareStateText;
        private Animator animator;
        //private Color greenColor = new Color(0.196f,0.643f,0.545f,1f);
        //private Color redColor = new Color(0.643f, 0.196f, 0.196f, 1f);
        public int pos;
        public string userId;
        private string nickName;
        public string countDownTime;
        private bool isCountDown;
        private double timeValue;//时间戳 
        private DateTime startTime;       
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            Debug.Log("PlayStateItem p1:"+param1+"  p2:"+param2);
            this.m_Transform.gameObject.SetActive(true);
            lightObj = this.m_Transform.Find("Light").gameObject;
            userNameObj = this.m_Transform.Find("UserName").gameObject;
            stateObj = this.m_Transform.Find("State").gameObject;
            ShowUI();
            this.pos = (int)param1;
            Debug.Log("PlayStateItem p333:"+ m_Transform.parent.parent.name);
            this.lightMatRender= m_Transform.Find("Light").GetComponent<MeshRenderer>();
            redMat = m_Transform.parent.parent.Find("RedLightMaterial").GetComponent<MeshRenderer>().material;
            greenMat = m_Transform.parent.parent.Find("GreenLightMaterial").GetComponent<MeshRenderer>().material;
            this.horseNameText = this.m_Transform.Find("UserName/Text").GetComponent<TextMesh>();
            this.prepareStateText = this.m_Transform.Find("State/Text").GetComponent<TextMesh>();       
            animator = this.m_Transform.Find("DoorRoot/Door").GetComponent<Animator>();
            object[] params2= (object[])param2;
            Debug.Log("PlayStateItem p2222");
            timeValue = (int)param3/1000;
            userId = (string)params2[2];
            UpdateUI((string)params2[0], (string)params2[1]);
        }

        public void UpdateUI(string name,string state)
        {
            this.nickName = name;
            if(state.Equals("1"))
            {
                lightMatRender.material = greenMat;
                prepareStateText.text = "Ready";
                isCountDown = false;
                timeValue = 0;
            }
            else
            {
                StartCountDown();
                lightMatRender.material = redMat;
                prepareStateText.text = countDownTime;
               
                
            }
            horseNameText.text=this.nickName;

           
        }
        public void ShowUI()
        {
            lightObj.gameObject.SetActive(true);
            userNameObj.gameObject.SetActive(true);
           // stateObj.gameObject.SetActive(true);取消倒计时
        }
        public void HideUI()
        {
            lightObj.gameObject.SetActive(false);
            userNameObj.gameObject.SetActive(false);
            stateObj.gameObject.SetActive(false);
        }
        private void StartCountDown()
        {
            startTime = DateTime.Now;
            Debug.Log("倒计时："+timeValue);
            startTime = startTime.AddSeconds(timeValue);
            // isCountDown = true;取消倒计时
        }
        public void OpenDoor()
        {
            animator.SetTrigger("open");
        }
        public void CloseDoor()
        {
            animator.SetTrigger("close");
        }
        /// <summary>
        /// 开启倒计时
        /// </summary>
        public override void OnFixUpdate()
        {
            if(isCountDown)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan span = nowTime.Subtract(startTime).Duration();               
                prepareStateText.text = (span.Hours < 10 ? ("0" + span.Hours) : span.Hours.ToString()) + ":" + (span.Minutes < 10 ? ("0" + span.Minutes) : span.Minutes.ToString()) + ":" + (span.Seconds < 10 ? ("0" + span.Seconds) : span.Seconds.ToString());
                if (TimeUtils.OnDiffSeconds(startTime,nowTime) >-0.1f)
                {
                    isCountDown = false;
                    Debug.Log("倒计时结束了");
                }
            }
        }

    }
}
