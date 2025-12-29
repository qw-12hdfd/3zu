using HotFix.Common;
using HotFix.Common.Utils;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotFix
{
    public class HorseMove : ObjectParent
    {
        //private BoxCollider
        private AudioSource audioSource;//TODO添加引用
        private Animation anim;
        public static Action<Vector3> MoveAct;
        public static Action StandAct;

        // 是否在移动
        private bool moving = false;
        // 移动向量
        private Vector3 moveDirection = Vector3.zero;
        // 是否可移动
        private bool canMove = true;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            this.m_Transform.GetComponent<BoxCollider>().isTrigger = true;
            this.anim = this.m_GameObject.GetComponent<Animation>();
            this.audioSource = this.m_GameObject.GetComponent<AudioSource>();
            this.anim.enabled = false;
            MoveAct = Move;
            StandAct = Stand;
        }
        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            PlayRun(false);
        }

        public override void OnUpdate()
        {
            if (canMove && moving)
            {
                PlayRun(true);

                m_Transform.position += moveDirection * 10 * Time.deltaTime;
                m_Transform.forward = Vector3.Lerp(m_Transform.forward, moveDirection, 5 * Time.deltaTime);
            }
            else
            {
                PlayRun(false);
            }
        }

        public void Move(Vector3 direction)
        {
            Debug.Log(direction+"位置");
            moveDirection = direction;
            moving = true;
        }

        private void PlayRun(bool bl)
        {
            anim.enabled = bl;
            this.anim.Play("Run");
        }

        internal void Stand()
        {
            moving = false;
        }
    }
}
