
using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
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
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

namespace HotFix
{
    public class PlayerMoveData
    {
        public string type;
        public string posX;
        public string posY;
        public string posZ;
        public string horseCode;
        public string userId;
        public string roleType;
        public string nowPosX;
        public string nowPosY;
        public string nowPosZ;
        public string rotateY;
        public string userName;

        public PlayerMoveData(string type, string posX, string posY, string posZ, string horseCode, string userId, string roleType, string nowPosX, string nowPosY, string nowPosZ, string rotateZ)
        {
            this.type = type;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.horseCode = horseCode;
            this.userId = userId;
            this.roleType = roleType;
            this.nowPosX = nowPosX;
            this.nowPosY = nowPosY;
            this.nowPosZ = nowPosZ;
            this.rotateY = rotateZ;
        }
    }
    public class PlayerController : ObjectParent
    {
        public static Action<Vector3> moveAction;
        public static Action standAction;
        public static Action jumpAction;
        public static Action<bool> walkAction;
        public static Action MountHorseAction;
        public static Action GetDownHorseAction;
        public static Action<string> GoToPosition;
        public static Action<Vector3> setPosition;
        public GameObject otherPlayer;

        public TriggerEvent triggerEvent;
        public TriggerEvent horseTrigger;

        private bool moving;
        public bool jumping;
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
        public bool mount;
        public bool isGround;
        private bool getDownHorse;
        private bool openDoor = false;
        public TextMesh name;
        public string sex;
        public void SetSex(string roleType)
        {
            if (sex == roleType)
                return;
            if (otherPlayer == null)
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
            Awake();
            UserInfoManager.camCtrler.player = m_GameObject;
            UserInfoManager.camCtrler.OnShow();
            otherPlayer.GetComponent<CharacterController>().enabled = false;
        }
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            name = m_Transform.Find("Name").GetComponent<TextMesh>();
            name.text = UserInfoManager.userName.Length > 5 ? UserInfoManager.userName.Substring(0, 5) + "..." : UserInfoManager.userName;
            name.gameObject.SetActive(true);
            name.color = Color.green;
            moveAction = Move;
            standAction = Stand;
            jumpAction = Jump;
            walkAction = RunOrWalk;
            setPosition = SetPosition;
            MountHorseAction = MountHorse;
            GetDownHorseAction = GetDownHorse;
            GoToPosition = PlayerGoToPos;
            playerCtl = m_Transform.GetComponent<CharacterController>();
            playerCtl.enabled = true;
            animator = m_Transform.GetComponent<Animator>();
            triggerEvent = m_Transform.GetComponent<TriggerEvent>();
            triggerEvent.TriggerEnter = SetTriggerEnter;
            triggerEvent.TriggerExit = SetHorseExit;
            UserInfoManager.camera.GetComponent<Animator>().enabled = false;

        }

        private void PlayerGoToPos(string name)
        {
            UserInfoManager.cmCamera.gameObject.SetActive(false);
            Transform playerObj = null;
            if (horse!=null)
                playerObj = horse.transform;
            else
                playerObj = m_Transform;
            var posTrans = UserInfoManager.transferPoint.Find(name);
            playerCtl.enabled = false;
            switch (name)
            {
                case "saimachang":
                    UserInfoManager.cmCamera.transform.rotation = Quaternion.Euler(new Vector3(6, -103, 0));
                    UserInfoManager.cmCamera.transform.position = new Vector3(666, 1.6f, 184);
                    UserInfoManager.camera.transform.rotation = Quaternion.Euler(new Vector3(6, -103, 0));
                    UserInfoManager.camera.transform.position = new Vector3(666, 1.6f, 184);
                    break;
                case "fanzhichang":
                    UserInfoManager.cmCamera.transform.rotation = Quaternion.Euler(new Vector3(5, 178, 0));
                    UserInfoManager.cmCamera.transform.position = new Vector3(646, 1.4f, 890);
                    UserInfoManager.camera.transform.rotation = Quaternion.Euler(new Vector3(5, 178, 0));
                    UserInfoManager.camera.transform.position = new Vector3(646, 1.4f, 890);
                    break;
                case "siyangchang":
                    UserInfoManager.cmCamera.transform.rotation = Quaternion.Euler(new Vector3(2, 176, 0));
                    UserInfoManager.cmCamera.transform.position = new Vector3(483, 11, 534);
                    UserInfoManager.camera.transform.rotation = Quaternion.Euler(new Vector3(2, 176, 0));
                    UserInfoManager.camera.transform.position = new Vector3(483, 11, 534);
                    break;
                default:
                    break;
            }
            playerObj.position = posTrans.position;
            playerObj.rotation = posTrans.rotation;
            NetManager.instance.Send(new MsgBase(RequestCode.Move.ToString(), JsonMapper.ToJson(new PlayerMoveData(
                walkState == "Walk" ? "0" : "1",    // 行为类型 0:走,1:跑,2:骑马走,3:骑马跑,4:传送,5:退出
                "0",         // x坐标
                "0",         // y坐标
                "0",         // z坐标
                "",                                 // 马匹code
                UserInfoManager.userID,             // 用户id
                UserInfoManager.Sex.ToString(),     // 用户性别
                m_Transform.position.x.ToString(),         // x坐标
                m_Transform.position.y.ToString(),         // y坐标
                m_Transform.position.z.ToString(),        // z坐标
                m_Transform.eulerAngles.y.ToString()         // 角度
                ))));
            if (!mount)
                playerCtl.enabled = true;
            IEnumeratorTool.instance.StartCoroutineNew(showCM());
        }
        private IEnumerator showCM()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            UserInfoManager.cmCamera.gameObject.SetActive(true);
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
                case "MaCao":
                    if (collider.transform.parent.Find("HorsePos").childCount > 0)
                    {
                        UserInfoManager.maCaoTransform = collider.transform.parent;
                        //WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletDetailUrl, WebRequestFuncitons.GetMilletDetailOnly, true, "{}", RFrameWork.instance.token);
                        id = int.Parse(collider.transform.parent.name.Split('o')[1]);
                        horseid = int.Parse(collider.transform.parent.Find("HorsePos").GetChild(0).name);
                        MainWindow.PutFood(true, id, horseid);
                    }
                    break;
                case "Door":
                    collider.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("open");
                    if (collider.transform.GetChild(0).Find("HighLight") != null)
                    {
                        collider.transform.GetChild(0).Find("HighLight").gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
        }
        

        private void SetTriggerEnter2(Collider collider)
        {
            switch (collider.gameObject.tag)
            {
                case "yangmachang":
                    GetQuestionFunc();
                    break;
                case "saimachang":
                    GetQuestionFunc();
                    break;
                case "fanyuchang":
                    GetQuestionFunc();
                    break;
                default:
                    break;
            }
        }

        public void GetQuestionFunc()
        {
            if (mount&&UIManager.instance.IsSignWindowOpen(FilesName.MAINPANEL))
            {
                JsonData data = new JsonData();
                data["horseId"] = UserInfoManager.mountHorseID;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.getQuestion, WebRequestFuncitons.GetQuestionFunc, true, JsonMapper.ToJson(data), RFrameWork.instance.token);
            }
        }

        private void GetDownHorse()
        {
            m_Transform.SetParent(ObjectManager.instance.SceneTrs); //人物跟随马匹身上的父节点
            animator.SetTrigger("GetDownHorse");
            mount = false; //设置是否上马
            UserInfoManager.maCaoTransform = null;
            MainWindow.SetBtnGray(true);
            UserInfoManager.mount = false;
            MainWindow.GetDownHorse(false);//关闭上马按钮
            UserInfoManager.horseCantPutDown.SetActive(false);
            horseAni = null; //关闭 否则无法移动位置
            horseTrigger = null;
            m_Transform.position += new Vector3(1.5f, 0, 1.5f);
            playerCtl.enabled = true;
            if (walkState == "Walk")
                speed = 2f;
            else
                speed = 4f;
            NetManager.instance.Send(new MsgBase(RequestCode.Move.ToString(), JsonMapper.ToJson(new PlayerMoveData(
                walkState == "Walk" ? "0" : "1",    // 行为类型 0:走,1:跑,2:骑马走,3:骑马跑,4:传送,5:退出
                "0".ToString(),         // x坐标
                "0".ToString(),         // y坐标
                "0".ToString(),         // z坐标
                "",                                 // 马匹code
                UserInfoManager.userID,             // 用户id
                UserInfoManager.Sex.ToString(),      // 用户性别
                m_Transform.position.x.ToString(),         // x坐标
                m_Transform.position.y.ToString(),         // y坐标
                m_Transform.position.z.ToString(),        // z坐标
                m_Transform.eulerAngles.y.ToString()         // 角度
                ))));
        }

        /// <summary>
        /// 上马方法
        /// </summary>
        private void MountHorse()
        {
            if (horse != null)
            {
                if (UserInfoManager.mountBreedHorse && mount)
                {
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.walkHorseEnd, WebRequestFuncitons.WalkHorseEndFunc, true, JsonMapper.ToJson(new HorseIdData(UserInfoManager.mountHorseID)), RFrameWork.instance.token);
                }
                else
                {
                    MountHorseFunc();
                }
            }
            else
            {

                if (UserInfoManager.mountBreedHorse)
                {
                    horse = UserInfoManager.mountBreedHorseObject;
                }
                MountHorseFunc();
            }
        }

        private void MountHorseFunc()
        {
            mount = true; //设置是否上马
            MainWindow.SetBtnGray(false);
            UserInfoManager.mount = true;
            m_Transform.SetParent(horse.transform.Find("CG/Pelvis/Spine/PlayerPos")); //人物跟随马匹身上的父节点
            if(UserInfoManager.Sex == 1)
            {
                horse.transform.Find("CG/Pelvis/Spine/PlayerPos").localPosition = new Vector3(-0.5053826f, 1.195526f, 0.5479326f);
            }
            else
            {
                horse.transform.Find("CG/Pelvis/Spine/PlayerPos").localPosition = new Vector3(-0.5053826f, 1.268f, 0.5479326f);
            }
            playerCtl.enabled = false; //将人物控制器关闭 否则无法移动位置
            m_Transform.ResetLocal();//数据归零
            animator.SetTrigger("MountHorse"); //播放上马动画
            MainWindow.MountHorse(false,false);//关闭上马按钮
            MainWindow.PutFood(false, id, horseid);
            horseAni = horse.GetComponent<Animator>();
            horseTrigger = horse.GetComponent<TriggerEvent>();
            horseTrigger.TriggerStay = SetHorseStay;
            horseTrigger.TriggerEnter = SetTriggerEnter2;
            UserInfoManager.horseCantPutDown.SetActive(true);
            if (walkState == "Walk")
                speed = 4f;
            else
                speed = 8f;
            NetManager.instance.Send(new MsgBase(RequestCode.Move.ToString(), JsonMapper.ToJson(new PlayerMoveData(
                walkState == "Walk" ? "0" : "1",    // 行为类型 0:走,1:跑,2:骑马走,3:骑马跑,4:传送,5:退出
                "0".ToString(),         // x坐标
                "0".ToString(),         // y坐标
                "0".ToString(),         // z坐标
                "",                                 // 马匹code
                UserInfoManager.userID,             // 用户id
                UserInfoManager.Sex.ToString(),      // 用户性别
                m_Transform.position.x.ToString(),         // x坐标
                m_Transform.position.y.ToString(),         // y坐标
                m_Transform.position.z.ToString(),        // z坐标
                m_Transform.eulerAngles.y.ToString()         // 角度
                ))));
        }

        private void SetHorseExit(Collider collider)
        {
            switch (collider.gameObject.tag)
            {
                case "MaCao":
                    MainWindow.MountHorse(false,false );
                    MainWindow.PutFood(false, 0,0);
                    if(UserInfoManager.maCaoTransform&& UserInfoManager.maCaoTransform.Find("HorsePos").childCount>0)
                    {
                        Animator ani = UserInfoManager.maCaoTransform.Find("HorsePos").GetChild(0).GetComponent<Animator>();
                        if (!ani.GetBool("Eat") && UserInfoManager.maCaoTransform.Find("fodders").GetChild(0).gameObject.activeSelf && !mount)
                            ani.SetBool("Eat", true);
                    }
                    
                    UserInfoManager.maCaoTransform = null;
                    break;
                case "Door":
                    //if (openDoor)
                    //{
                    //    openDoor = false;
                    //}
                    collider.transform.GetChild(0).GetComponent<Animator>().SetTrigger("close");
                    if (collider.transform.GetChild(0).Find("HighLight") != null)
                    {
                        collider.transform.GetChild(0).Find("HighLight").gameObject.SetActive(true);
                    }
                    //collider.gameObject.transform.parent.rotation = Quaternion.Euler(new Vector3(collider.gameObject.transform.parent.rotation.x, 90, collider.gameObject.transform.parent.rotation.z));
                    break;
                default:
                    break;
            }
        }
        int id = 0;
        int horseid = 0;
        private void SetHorseStay(Collider collider)
        {
            switch (collider.gameObject.tag)
            {
                case "GetDownHorse":
                    if (getDownHorse && mount)
                        MainWindow.GetDownHorse(true);
                    else
                        MainWindow.GetDownHorse(false);
                    break;
                default:
                    break;
            }
        }
        int timeNum = 0;

        public override void OnUpdate()
        {
            if (mount&&horseAni!=null&&horseAni.GetBool("Eat"))
            {
                horseAni.SetBool("Eat", false);
            }
            name.transform.LookAt(UserInfoManager.camera.transform);
            if (m_Transform.position.y < -5)
            {
                if (UserInfoManager.isGoToSiyangchang)
                    PlayerController.GoToPosition("siyangchang");
                else
                    PlayerController.GoToPosition("saimachang");
            }
            if (Input.GetMouseButtonDown(0))
            {
                //如果点击鼠标左键，从主相机发射一条射线，射向鼠标点击的位置
                Ray ray = UserInfoManager.camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                //定义一个RaycastHit变量用来保存被撞物体的信息；
                RaycastHit hit;

                //如果碰撞到了物体，hit里面就包含该物体的相关信息；
                if (Physics.Raycast(ray, out hit))
                {
                    switch (hit.transform.tag)
                    {
                        case "GameList":
                            if (UIManager.instance.IsSignWindowOpen(FilesName.MAINPANEL))
                                UIManager.instance.PopUpWnd(FilesName.GAMELISTPANEL, true, false);
                            break;
                        case "StartGame":
                            if (UIManager.instance.IsSignWindowOpen(FilesName.MAINPANEL))
                            {
                                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.checkMatch, WebRequestFuncitons.CheckMatch, true, "{}", RFrameWork.instance.token);
                                UserInfoManager.returnAct = ReturnAct;
                            }
                            break;
                        case "CreateRoom":
                            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.GetMyMHorsesList, true, JsonMapper.ToJson(new HorseSex(0, 2,1,1)), RFrameWork.instance.token);
                            break;
                        case "FZCDoor":
                            if (hit.transform.parent.Find("Room_tips").Find("red").gameObject.active)
                            {
                                RFrameWork.instance.OpenCommonConfirm("提示", "此房已满，换间房试试吧", () => { }, null);
                            }
                            if (hit.transform.parent.Find("Room_tips").Find("Yellow").gameObject.active)
                            {
                                RFrameWork.instance.OpenCommonConfirm("提示", "暂未开房，可前往繁育场门口提示牌点击预约开房进行预约哦", () => { }, null);
                            }
                            if (hit.transform.parent.Find("Room_tips").gameObject.active == false)
                            {
                                RFrameWork.instance.OpenCommonConfirm("提示", "此房间已下架，换间房试试吧", () => { }, null);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            if (moving)
            {
                if (mount)
                {
                    var horseMoveData = moveDirection * Time.deltaTime * speed;
                    horse.transform.localPosition += new Vector3(horseMoveData.x, 0, horseMoveData.z);
                    horse.transform.forward = Vector3.Lerp(horse.transform.forward, moveDirection, turnSpeed * Time.deltaTime);
                    if (isGround)
                    {
                        horseAni.SetBool(walkState, true);
                    }
                    MainWindow.GetDownHorse(false);
                    getDownHorse = false;
                }
                else
                {

                    if (playerCtl.enabled == true)
                    {
                        m_Transform.forward = Vector3.Lerp(m_Transform.forward, moveDirection, turnSpeed * Time.deltaTime);
                    }
                    if (isGround)
                    {
                        animator.SetBool(walkState, true);
                    }
                    else
                    {
                        animator.SetBool(walkState, false);
                        animator.SetInteger("Jump", 2);
                    }
                    if(animator.GetBool(walkState))
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
                    getDownHorse = false;
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
                    if(horseAni.GetBool("Fall"))
                    horseAni.SetBool("Fall", false);
                    if(!moving)
                    getDownHorse = true;
                }
            }

            if (UnityEngine.Input.GetKey(KeyCode.Space))
            {
                if(jumping == false && isGround)
                {
                    Jump();
                }
            }
        }
        public void ReturnAct()
        {
            UserInfoManager.isLookGame = 0;
            UserInfoManager.detailPanelType = 2;
            string[] arr = new string[2] { "2", "9" };
            HorseListType type = new HorseListType(arr);
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.listFrontUrl, WebRequestFuncitons.CanPlayGame, true, JsonMapper.ToJson(type), RFrameWork.instance.token);
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
                if (colliders2.Length!=0)
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
            timeNum++;
            if (timeNum > 10)
            {
                timeNum = 0;
                if (moveDirection != Vector3.zero)
                {
                    NetManager.instance.Send(new MsgBase(RequestCode.Move.ToString(), JsonMapper.ToJson(new PlayerMoveData(
                        walkState == "Walk" ? "0" : "1",    // 行为类型 0:走,1:跑,2:骑马走,3:骑马跑,4:传送,5:退出
                        moveDirection.x.ToString(),         // x坐标
                        moveDirection.y.ToString(),         // y坐标
                        moveDirection.z.ToString(),         // z坐标
                        "",                                 // 马匹code
                        UserInfoManager.userID,             // 用户id
                        UserInfoManager.Sex.ToString(),      // 用户性别
                        m_Transform.position.x.ToString(),         // x坐标
                        m_Transform.position.y.ToString(),         // y坐标
                        m_Transform.position.z.ToString(),        // z坐标
                m_Transform.eulerAngles.y.ToString()         // 角度
                        ))));
                }
            }
            moveDirection = direction;
            moving = true;
        }

        public void Stand()
        {
            NetManager.instance.Send(new MsgBase(RequestCode.Move.ToString(), JsonMapper.ToJson(new PlayerMoveData(
                walkState == "Walk" ? "0" : "1",    // 行为类型 0:走,1:跑,2:骑马走,3:骑马跑,4:传送,5:退出
                "0",         // x坐标
                "0",         // y坐标
                "0",         // z坐标
                "",                                 // 马匹code
                UserInfoManager.userID,             // 用户id
                UserInfoManager.Sex.ToString(),     // 用户性别
                m_Transform.position.x.ToString(),         // x坐标
                m_Transform.position.y.ToString(),         // y坐标
                m_Transform.position.z.ToString(),        // z坐标
                m_Transform.eulerAngles.y.ToString()         // 角度
                ))));
            moving = false;
            if (mount)
            {
                horseAni.SetBool(walkState, false);
            }
            else
            {
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
            if (mount)
            {
                if (horseAni == null)
                    return;
                horseAni.SetBool(walkState, false);
            }
            else
            {
                if (animator == null)
                    return;
                animator.SetBool(walkState, false);
            }
            if (isRun == true)
            {
                walkState = "Run";
                if (mount)
                    speed = 6f;
                else
                    speed = 4f;
            }
            else
            {
                walkState = "Walk";
                if (mount)
                    speed = 4f;
                else
                    speed = 2f;
            }
        }

        public void SetPosition(Vector3 vector3)
        {
            NetManager.instance.Send(new MsgBase(RequestCode.Move.ToString(), JsonMapper.ToJson(new PlayerMoveData(
                walkState == "Walk" ? "0" : "1",    // 行为类型 0:走,1:跑,2:骑马走,3:骑马跑,4:传送,5:退出
                "0",         // x坐标
                "0",         // y坐标
                "0",         // z坐标
                "",                                 // 马匹code
                UserInfoManager.userID,             // 用户id
                UserInfoManager.Sex.ToString(),     // 用户性别
                m_Transform.position.x.ToString(),         // x坐标
                m_Transform.position.y.ToString(),         // y坐标
                m_Transform.position.z.ToString(),        // z坐标
                m_Transform.eulerAngles.y.ToString()         // 角度
                ))));
            playerCtl.enabled = false;
            m_Transform.position = vector3;
            playerCtl.enabled = true;
        }
    }
}
