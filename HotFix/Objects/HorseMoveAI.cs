using HotFix.Common;
using HotFix.Common.Utils;
using HotFix.GameDatas.ServerData.Response;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    public class HorseMoveAI : ObjectParent
    {
        private AudioSource audioSource;
        public RankItem rankItem;
        private Animator anim;
        private AnimatorStateInfo animatorInfo;
        public string runAnimName, idleAnimName;
        public float moveSpeed = 0;
        public bool crossFade = false;
        public float distance;
        public int rank;
        public int pos;
        private TextMesh rankText;
        public string userId;
        public Transform endTrans;
        private bool moveEnd = false;
        private string nickName;
        private LODGroup loadGroup;
        private Vector3 aimPos;
        private GameObject mountTrigger;
        public float posZ = 0;
        GameObject horseMane;
        GameObject horseBody;
        GameObject horseTail;
        GameObject horseEyes;
        Material maneMaterial;
        Material bodyMaterial;
        Material tailMaterial;
        Material eyesMaterial;
        private HorseAnimSpeed horseAnimSpeed;
        string gene = "F-00-RR-CA-G1-W2-DP00";
        private DateTime startTime;
        private float lerpDistance = 0;
        private float startPosX;
        private float maxSpeed = 0;
        private float framSpeed;
        public float speed;
        private float animSpeed;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            if (m_GameObject.GetComponent<Rigidbody>())
            {
                MonoBehaviour.DestroyImmediate(m_GameObject.GetComponent<Rigidbody>());
            }
            horseMane = m_Transform.Find("Meshes/Horse Mane").gameObject;
            horseTail = m_Transform.Find("Meshes/Horse Tail").gameObject;
            horseBody = m_Transform.Find("Meshes/Horse Body").gameObject;
            horseEyes = m_Transform.Find("Meshes/Horse Eyes").gameObject;
            maneMaterial = horseMane.GetComponent<Renderer>().material;
            bodyMaterial = horseBody.GetComponent<Renderer>().material;
            tailMaterial = horseTail.GetComponent<Renderer>().material;
            eyesMaterial = horseEyes.GetComponent<Renderer>().material;
            horseAnimSpeed = m_GameObject.GetComponent<HorseAnimSpeed>();
            userId = param1.ToString();
            object[] params2 = (object[])param2;
            nickName = (string)params2[0];
            gene = (string)params2[1];
            maxSpeed = (float)params2[2];
            float speed222 = (Math.Abs(moveSpeed) / maxSpeed) * 1.5f;
            Debug.Log("动画的最大速度：" + maxSpeed + "速度：" + moveSpeed);
            if (userId.Equals(UserInfoManager.userID))
            {
                UserInfoManager.geneID = gene;
            }
            HorseUtils.SetHorseTexture(this.m_Transform, gene);
            this.rankItem = (RankItem)param3;
            Debug.Log("HorseMoveAI Awake:" + param1 + " 22:" + param2 + " speed:" + maxSpeed);
            this.m_Transform.name = "HorseClone" + userId;
            this.m_Transform.localEulerAngles = new Vector3(0, -90f, 0);
            // this.m_Transform.GetComponent<BoxCollider>().isTrigger = true;
            runAnimName = "Run";
            this.anim = this.m_GameObject.GetComponent<Animator>();
            this.audioSource = this.m_GameObject.GetComponent<AudioSource>();
            endTrans = GameObject.Find("GameEnd").transform;
            rankText = this.m_Transform.Find("position").GetComponent<TextMesh>();
            rankText.text = nickName;
            rankText.gameObject.SetActive(false);
            loadGroup = this.m_Transform.Find("Meshes").GetComponent<LODGroup>();
            loadGroup.enabled = false;
            startPosX = this.m_Transform.position.x;
            lerpDistance = 0;
            Debug.Log("克隆马：");
            Debug.Log("HorseMoveAI Awake 88888");

        }
        float thisPos;
        public void SetAimPos(float pos,float sp)
        {

            thisPos = sp;
            this.aimPos = new Vector3(-pos, this.m_Transform.localPosition.y, posZ);
            speed = Math.Abs((this.m_Transform.localPosition.x + pos) / 0.2f);
            animSpeed = speed / 12f;
            if (animSpeed < 1)
            {
                animSpeed = 1;
            }
            if (animSpeed > 1.5f)
            {
                animSpeed = 1.5f;
            }
            horseAnimSpeed.SetAniSpeed("H_Gallop_IP", 0, animSpeed);
            Debug.Log(this.m_GameObject.name + "SetAimPos pos：" + aimPos + " speed:" + speed + " animSpeed:" + animSpeed);
        }
        public void StartGame()
        {
            lerpDistance = 0;
            startPosX = this.m_Transform.position.x;
            audioSource.Play();
            PlayRun();
            startTime = DateTime.Now;
            moveEnd = false;
            rankText.gameObject.SetActive(true);

            Debug.Log("HorseMoveAI StartGame:" + moveSpeed);

        }
        private void PlayRun()
        {
            this.anim.SetBool("RunPowerful", true);

        }

        public override void OnLateUpdate()
        {

            rankText.transform.LookAt(UserInfoManager.camera.transform);
            if (UserInfoManager.startGame && !GameManager.gameEnd && this.m_GameObject != null && !moveEnd)
            {

                this.m_Transform.localPosition = Vector3.MoveTowards(this.m_Transform.localPosition, aimPos, speed * Time.deltaTime);
                //this.m_Transform.localPosition = Vector3.Lerp(this.m_Transform.localPosition, this.aimPos, 0.2f * Time.deltaTime);
                if (Time.frameCount % 120 == 0)
                {
                    Debug.Log("name:" + this.m_GameObject.name + "this.m_Transform.position:" + m_Transform.position);
                }
                distance = m_Transform.position.x;
                if ((this.m_Transform.position.x - endTrans.position.x) <= 0 && !moveEnd)
                {
                    moveEnd = true;
                    audioSource.Stop();
                    //this.anim.SetBool("RunPowerful", false);
                    //this.m_Transform.localPosition = Vector3.MoveTowards(this.m_Transform.localPosition, this.m_Transform.localPosition + new Vector3(-100, 0, 0), speed * Time.deltaTime);
                    horseAnimSpeed.ResetSpeed();
                    if (userId.Equals(UserInfoManager.userID))
                    {
                        NetManager.instance.Send(new MsgBase(RequestCode.MatchEnd.ToString(), JsonUtility.ToJson(new JoinScene(UserInfoManager.RoomId.ToString(), userId, TimeUtils.CurTimeToTimestamp().ToString()))));
                        Debug.Log("我的马跑到了终点：" + this.m_GameObject.name);
                        //m_Transform.Translate(new Vector3(1, 0, 0));
                    }
                }
                ChangeSpeedByDistance();
            }
            if (moveEnd && this.m_Transform.localPosition.x>-1100)
            {
                Vector3 v3 = this.m_Transform.localPosition + new Vector3(-thisPos, 0, 0);
                this.m_Transform.localPosition = Vector3.Lerp(this.m_Transform.localPosition, v3, Time.deltaTime);
            }else if (this.m_Transform.localPosition.x <= -1100)
            {
                this.anim.SetBool("RunPowerful", false);
            }
        }

        private void ChangeSpeedByDistance()
        {
            lerpDistance = Math.Abs(this.m_Transform.position.x - startPosX);
        }
    }
}
