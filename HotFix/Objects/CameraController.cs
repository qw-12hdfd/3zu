using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class CameraController:ObjectParent
    {
        // 摄像机Transform
        public static Transform camTransform;
        public CinemachineFreeLook vcam;
        public GameObject player;
        // 旋转速度
        public float rotateSpeed = 0.006f;


        private bool rotating;
        private bool stoprotate;
        private Vector2 rotateDelta;
        public static Action<Vector2> RotateCamAct;
        public static Action StopRotateAct;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            player = param1 as GameObject;
            RotateCamAct = RotateCam;
            StopRotateAct = StopRotate;
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            camTransform = m_Transform;
            vcam = camTransform.GetComponent<CinemachineFreeLook>();
            if(UserInfoManager.Sex == 1)
            {
                vcam.LookAt = player.transform.Find("R_CG/R_Pelvis/R_Spine/R_Spine1/R_Spine2/R_Neck/R_Head");
                vcam.Follow = player.transform.Find("R_CG/R_Pelvis");
            }
            else
            {
                vcam.LookAt = player.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
                vcam.Follow = player.transform.Find("Bip01/Bip01 Pelvis");
            }
        }

        public override void OnClose()
        {

        }

        public override void OnUpdate()
        {
            if (rotating)
            {
                vcam.m_XAxis.m_InputAxisValue = -rotateDelta.x;
                vcam.m_YAxis.m_InputAxisValue = -rotateDelta.y;
            }
            else if(stoprotate)
            {
                vcam.m_XAxis.m_InputAxisValue = rotateDelta.x;
                vcam.m_YAxis.m_InputAxisValue = rotateDelta.y;
                stoprotate = false;
            }
        }

        public void RotateCam(Vector2 delta)
        {
            rotateDelta = delta * rotateSpeed;
            rotating = true;
            stoprotate = false;
        }

        public void StopRotate()
        {
            rotateDelta = Vector2.zero;
            rotating = false;
            stoprotate = true;
        }
    }
}
