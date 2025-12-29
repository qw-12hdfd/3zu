
using HotFix.Common;
using HotFix.Common.Utils;
using LitJson;
using MalbersAnimations.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

namespace HotFix
{
    public class OtherPlayerController : ObjectParent
    {

        public TriggerEvent triggerEvent;
        public TriggerEvent horseTrigger;
        public GameObject otherPlayer;
        public TextMesh name;
        private bool moving;
        private bool jumping;
        private Vector3 moveDirection = Vector3.zero;
        private CharacterController playerCtl;
        private Animator animator;
        private float speed = 2f;
        private float turnSpeed = 20f;
        private float gravity = 1.8f;//模拟重力
        private float jumpSpeed = 0.8f;//起跳速度
        private float jumpTime = 0.4f;//跳跃时间
        private float jumpTimeFlag = 0;//累计跳跃时间用来判断是否结束跳跃
        private string walkState = "Walk";
        public GameObject horse;
        private Animator horseAni;
        private bool mount = false;
        private bool mount2;
        private bool isGround;
        PlayerMoveData playerMoveData;

        internal void SetSex(string roleType)
        {
            if (sex == roleType)
                return;
            if(otherPlayer == null)
            {
                string name = "RiderBoy";
                if (sex == "1")
                    name = "RiderGirl";
                var playerObj = ObjectManager.instance.InstantiateObject("Assets/GameData/Prefabs/Player/" + name + ".prefab", false, true);
                otherPlayer = playerObj;
            }
            otherPlayer.transform.localPosition = m_Transform.localPosition;
            otherPlayer.transform.localRotation = m_Transform.localRotation;
            var obj = otherPlayer;
            otherPlayer = m_GameObject;
            otherPlayer.SetActive(false);
            m_GameObject = obj;
            m_Transform = obj.transform;
            m_GameObject.SetActive(true);
            sex = roleType;
            playerMoveData.roleType = roleType;
            Awake(playerMoveData);
            otherPlayer.GetComponent<CharacterController>().enabled = false;
            //sex = roleType;
            //if (roleType == "1")
            //{
            //    m_Transform.Find("CowBoy").GetComponent<Renderer>().material.SetTexture("_BaseMap", ResourceManager.instance.LoadResources<Texture>("Assets/GameData/Materials/Animals/Textures/Rider_Textures/CowBoyDiffuse.png"));
            //    m_Transform.localScale = Vector3.one;
            //    m_Transform.Find("R_CG/R_Pelvis/R_Spine/R_Spine1/R_Spine2/R_Neck/R_Head/Mesh2").gameObject.SetActive(false);
            //}
            //else
            //{
            //    m_Transform.Find("CowBoy").GetComponent<Renderer>().material.SetTexture("_BaseMap", ResourceManager.instance.LoadResources<Texture>("Assets/GameData/Materials/Animals/Textures/Rider_Textures/CowGrilDiffuse.png"));
            //    m_Transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            //    m_Transform.Find("R_CG/R_Pelvis/R_Spine/R_Spine1/R_Spine2/R_Neck/R_Head/Mesh2").gameObject.SetActive(true);
            //}
        }
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            m_GameObject.SetLayer(10);
            playerMoveData = param1 as PlayerMoveData;
            m_Transform.position = new Vector3(float.Parse(playerMoveData.nowPosX), float.Parse(playerMoveData.nowPosY), float.Parse(playerMoveData.nowPosZ));
            m_Transform.rotation = Quaternion.Euler(new Vector3(Mathf.Rad2Deg * 0, Mathf.Rad2Deg * float.Parse(playerMoveData.rotateY), 0));
            playerCtl = m_Transform.GetComponent<CharacterController>();
            playerCtl.enabled = true;
            animator = m_Transform.GetComponent<Animator>();
            triggerEvent = m_Transform.GetComponent<TriggerEvent>();
            triggerEvent.TriggerEnter = SetTriggerEnter;
            triggerEvent.TriggerExit = SetHorseExit;
            UserInfoManager.camera.GetComponent<Animator>().enabled = false;
            name = m_Transform.Find("Name").GetComponent<TextMesh>();
            name.text = playerMoveData.userName.Length > 5 ? playerMoveData.userName.Substring(0,5) + "...": playerMoveData.userName;
            name.gameObject.SetActive(true);
            GameObject.Destroy(m_Transform.Find("Quad").gameObject);
            if (string.IsNullOrEmpty(playerMoveData.horseCode))
            {
                GetDownHorse();
            }
            else
            {
                MountHorseFunc(playerMoveData.horseCode);
            }
        }

        private void PlayerGoToPos(string name)
        {
            Transform playerObj = null;
            if (horse != null)
                playerObj = horse.transform;
            else
                playerObj = m_Transform;
            var posTrans = UserInfoManager.transferPoint.Find(name);
            playerCtl.enabled = false;
            playerObj.position = posTrans.position;
            playerObj.rotation = posTrans.rotation;
            if (!mount)
                playerCtl.enabled = true;
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            triggerEvent.TriggerEnter = SetTriggerEnter;
            triggerEvent.TriggerExit = SetHorseExit;
        }

        private void SetTriggerEnter(Collider collider)
        {
            switch (collider.gameObject.tag)
            {
                case "Door":
                    collider.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("open");
                    break;
                default:
                    break;
            }
        }

        private void SetHorseExit(Collider collider)
        {
            switch (collider.gameObject.tag)
            {
                case "Door":
                    collider.transform.GetChild(0).GetComponent<Animator>().SetTrigger("close");
                    break;
                default:
                    break;
            }
        }

        public void GetDownHorse()
        {
            Debug.Log("玩家下马咯");
            mount2 = false;
            if (walkState == "Walk")
                speed = 2f;
            else
                speed = 4f;
        }

        public void MountHorseFunc(string code)
        {
            Debug.Log("玩家上马咯" + code);
            mount2 = true;
            if (walkState == "Walk")
                speed = 4f;
            else
                speed = 8f;
        }
        int id = 0;
        int horseid = 0;
        internal string sex;

        public override void OnUpdate()
        {
            name.transform.LookAt(UserInfoManager.camera.transform);
            if (moving)
            {
                if (mount)
                {
                    var horseMoveData = moveDirection * Time.deltaTime * speed;
                    horse.transform.localPosition += new Vector3(horseMoveData.x, 0, horseMoveData.z);
                    horse.transform.forward = Vector3.Lerp(horse.transform.forward, moveDirection, turnSpeed * Time.deltaTime);
                    if (isGround)
                    {
                        if (!horseAni.GetBool(walkState))
                            horseAni.SetBool(walkState, true);
                    }
                    MainWindow.GetDownHorse(false);
                }
                else
                {

                    if (playerCtl.enabled == true)
                    {
                        m_Transform.forward = Vector3.Lerp(m_Transform.forward, moveDirection, turnSpeed * Time.deltaTime);
                    }
                    if (isGround)
                    {
                        if (!animator.GetBool(walkState))
                            animator.SetBool(walkState, true);
                    }
                    else
                    {
                        animator.SetBool(walkState, false);
                        animator.SetInteger("Jump", 2);
                    }
                    if (animator.GetBool(walkState))
                        playerCtl.Move(moveDirection * Time.deltaTime * speed);
                }
            }
            if (jumping)
            {
                if (mount)
                {
                    horseAni.SetTrigger("Jump");
                    jumping = false;
                    MainWindow.GetDownHorse(false);
                }
                else
                {
                    if (jumpTimeFlag < jumpTime)
                    {
                        animator.SetInteger("Jump", 4);
                        playerCtl.Move(m_Transform.up * jumpSpeed * Time.deltaTime);
                        jumpTimeFlag += Time.deltaTime;
                    }
                    else if (jumpTime <= jumpTimeFlag)
                    {
                        animator.SetInteger("Jump", 2);
                        if (!isGround)
                        {
                        }
                        else
                        {
                            jumping = false;
                            jumpTimeFlag = 0;
                        }
                    }
                }
            }
            if (jumping == false)
            {
                animator.SetInteger("Jump", -1);
                if (!isGround)
                {
                    if (mount)
                    {
                        horseAni.SetBool("Fall", true);
                    }
                    else
                    {
                        animator.SetInteger("Jump", 2);
                    }
                }
                if (isGround && mount)
                {
                    if (horseAni.GetBool("Fall"))
                        horseAni.SetBool("Fall", false);
                }
            }
        }

        public override void OnFixUpdate()
        {
            if (!mount)
            {
                var pointBottom = m_Transform.position + m_Transform.up * 0.2f - m_Transform.up * 0.23f;
                var pointTop = m_Transform.position + m_Transform.up * 1.7f - m_Transform.up * 0.2f;
                LayerMask ignoreMask = 1 << 7 | 1 << 6 | 1 << 10;
                LayerMask ignoreMask2 = 1 << 7;

                var colliders = Physics.OverlapCapsule(pointBottom, pointTop, 0.2f, ignoreMask);
                var colliders2 = Physics.OverlapCapsule(pointBottom, pointTop, 0.2f, ignoreMask2);
                Debug.DrawLine(pointBottom, pointTop, Color.yellow);
                if (colliders2.Length != 0)
                {
                    isGround = true;
                    playerCtl.enabled = true;
                }
                else
                {
                    isGround = false;
                    playerCtl.Move(m_Transform.up * Time.deltaTime * -gravity);
                }
            }
            else
            {
                var pointBottom = horse.transform.position + new Vector3(0, 0, 0.5f) + horse.transform.up * 0.2f - horse.transform.up * 1f;
                var pointTop = horse.transform.position + new Vector3(0, 0, 0.5f) + horse.transform.up * 1.7f - horse.transform.up * 0.2f;
                LayerMask ignoreMask = 1 << 7 | 1 << 6 | 1 << 10;
                LayerMask ignoreMask2 = 1 << 7;

                var colliders = Physics.OverlapCapsule(pointBottom, pointTop, 0.2f, ignoreMask);
                var colliders2 = Physics.OverlapCapsule(pointBottom, pointTop, 0.2f, ignoreMask2);
                Debug.DrawLine(pointBottom, pointTop, Color.yellow);
                if (colliders2.Length != 0)
                {
                    isGround = true;
                    animator.SetBool("Fall", false);
                }
                else
                {
                    isGround = false;
                }
            }
        }

        public void Move(Vector3 direction)
        {
            moveDirection = direction;
            moving = true;
        }

        public void Stand()
        {
            moving = false;
            if (mount)
            {
                horseAni.SetBool(walkState, false);
            }
            else
            {
                Debug.Log("animator.SetBool(walkState, false);");
                animator.SetBool(walkState, false);
            }
        }

        public void Jump()
        {
            if (isGround && jumping == false)
            {
                jumping = true;
            }
        }

        public void RunOrWalk(bool isRun)
        {
            if(isRun!=(walkState == "Run"))
            {
                if (mount)
                {
                    horseAni.SetBool(walkState, false);
                }
                else
                {
                    Debug.Log("animator.SetBool(walkState, false);");
                    animator.SetBool(walkState, false);
                }
            }
            if (isRun == true)
            {
                walkState = "Run";
                if (mount2)
                    speed = 6f;
                else
                    speed = 4f;
            }
            else
            {
                walkState = "Walk";
                if (mount2)
                    speed = 4f;
                else
                    speed = 2f;
            }
        }

        /// <summary>
        /// 闪现
        /// </summary>
        /// <param name="vector3"></param>
        public void SetPosition(Vector3 vector3, string rotateY)
        {
            if (Vector3.Distance(new Vector3(vector3.x, 0, vector3.z), new Vector3(m_Transform.position.x, 0, m_Transform.position.z)) > (mount2?1.5:1))
            {
                playerCtl.enabled = false;
                m_Transform.position = new Vector3(vector3.x, m_Transform.position.y, vector3.z);
                m_Transform.rotation = Quaternion.Euler(new Vector3(Mathf.Rad2Deg * 0, Mathf.Rad2Deg * float.Parse(playerMoveData.rotateY), 0));
                playerCtl.enabled = true;
            }
        }
    }
}
