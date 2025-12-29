using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class HorseCamera : ObjectParent
    {
        private float moveSpeed=0;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            Debug.Log("HorseCamera Awake");
            Debug.Log("HorseCamera obj:"+this.m_GameObject.name);
            m_Transform.GetComponent<TriggerEvent>().BeginDrag = BeginDragFunc;
            m_Transform.GetComponent<TriggerEvent>().Drag = DragFunc;
            m_Transform.GetComponent<TriggerEvent>().EndDrag = EndDragFunc;
            m_Transform.GetComponent<TriggerEvent>().ClickAct = ClickFunc;
        }

        private void ClickFunc(object[] obj)
        {
            Debug.Log("Click");
        }
        public float MoveSpeed
        {
            private get { return moveSpeed; }
            set { moveSpeed = value; }
        }
        public override void OnFixUpdate()
        {
            if (UserInfoManager.startGame&&moveSpeed != 0)
            {
                this.m_Transform.Translate(moveSpeed * Time.deltaTime, 0, 0, Space.World);
               
            }
        }

        private void EndDragFunc(object[] obj)
        {
            Debug.Log("EndDrag");
        }

        private void DragFunc(object[] obj)
        {
            Debug.Log("Drag");
        }

        private void BeginDragFunc(object[] obj)
        {
            Debug.Log("BeginDrag");
        }
    }
}
