using HotFix.Common;
using LitJson;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HotFix
{
    /// <summary>
    /// 用户信息管理类
    /// </summary>
    public static class UserInfoManager
    {
        public static bool startGame;
        public static GameObject horseClone;
        public static string userID;
        public static string horseID;
        public static bool selectOrSelectAndCreate;
        public static int Sex = 0;
        public static int totalNum;
        public static string geneID;
        public static bool enterGame;
        public static int isLookGame; //0 比赛 1 观战 

        public static GameObject player;
        public static CameraController camCtrler;
        public static Transform transferPoint;
        /// <summary>
        /// 是否可以进入元宇宙
        /// </summary>
        private static bool IsCreate = false;
        internal static float LoadNum;
        public static Dictionary<int, HorseData> MyHorseList = new Dictionary<int, HorseData>();
        public static Dictionary<int, HorseData> NowHorseList = new Dictionary<int, HorseData>();
        internal static float foodNum = 0;
        internal static GameObject cmCamera = RFrameWork.instance.transform.Find("CM Player").gameObject;
        internal static GameObject camera = RFrameWork.instance.transform.Find("Camera").gameObject;

        public static List<HorseHistoryData> horseHistoryData;

        public static Vector3 horseCameraPos = new Vector3(457.76f, 13.21f, 582.96f);
        public static Quaternion horseCameraRotate = Quaternion.Euler(new Vector3(359.6f, 190.4f, 0));
        internal static PlayerController playerCtrl;
        public static int nowHorseIndex;
        public static GameObject horseCantPutDown;
        public static int detailPanelType = 1;
        internal static Quaternion horseRotate = Quaternion.Euler(Vector3.one);
        internal static bool mount;
        internal static BreedSiteData MyBreedSiteData;
        internal static HorseDetail selectHorseData;
        internal static PayType payType;
        internal static string payOrderCode;
        internal static Transform maCaoTransform;
        internal static float breedPrice = 0;
        internal static string breedRoomId;
        internal static int fatherPrice;
        internal static Transform doorParent;
        internal static Transform nowHorseBreedRoom;
        internal static int mountHorseID;
        internal static bool mountBreedHorse;
        internal static GameObject mountBreedHorseObject;
        internal static bool isGoToSiyangchang = true;
        internal static bool isChuangShi;

        /// <summary>
        /// 保存是否可以进入元宇宙
        /// </summary>
        /// <param name="isOpenFlag"></param>
        public static void SetIsCreate(bool isCreate)
        {
            IsCreate = isCreate;
        }

        public static bool GetIsCreate()
        {
            return IsCreate;
        }

        internal static void CloseMainScenesObjectScript()
        {
            ObjectsManager.instance.OnRemove(playerCtrl);
            ObjectsManager.instance.OnRemove(camCtrler);
            foreach (var item in MyHorseList.Values)
            {
                ObjectsManager.instance.OnRemove(item.horseCtrl);
                item.horse = null;
                item.horseCtrl = null;
            }
        }

        internal static int RoomId;
        internal static bool noHorse;
        internal static float peiENum;
        internal static float allPeiENum;
        internal static float hoserFeedNumber;
        internal static int playerRank=1;
        internal static string matchPrice;
        internal static string horseMilletNote;
        internal static Action returnAct;
        internal static string userName;
        internal static string rankStr;
        internal static string rentOutPrice;
        internal static string recycleId;
        internal static string horseRentOutId;
        internal static string rentOutDataStr;
        internal static string horseTexture;
        internal static string horseRentOutName;
        internal static string RentOutTime;

        public static void Init()
        {
            startGame = false;
            horseClone = null;
            userID = null;
            horseID = null;
            selectOrSelectAndCreate = false;
            Sex = 0;
            totalNum = 0;
            geneID = null;
            player = null;
            camCtrler = null;
            transferPoint = null;
            IsCreate = false;
            LoadNum = 0;
            if (MyHorseList.Count > 0 && MyHorseList != null)
                MyHorseList.Clear();
            MyHorseList = new Dictionary<int, HorseData>();
            if (NowHorseList.Count > 0 && NowHorseList != null)
                NowHorseList.Clear();
            NowHorseList = new Dictionary<int, HorseData>();
            foodNum = 0;
            ObjectsManager.instance.OnRemove(playerCtrl);
            nowHorseIndex = 0;
            if (horseCantPutDown != null)
                GameObject.Destroy(horseCantPutDown);
            detailPanelType = 1;
            horseRotate = Quaternion.Euler(Vector3.one);
            mount = false;
            if (MyBreedSiteData!=null && MyBreedSiteData.list.Count > 0 && MyBreedSiteData.list != null)
                MyBreedSiteData.list.Clear();
            MyBreedSiteData = null;
            selectHorseData = null;
            payType = PayType.BreedFeed;
            payOrderCode = null;
            maCaoTransform = null;
            breedPrice = 0;
            breedRoomId = null;
            fatherPrice = 0;
            doorParent = null;
            nowHorseBreedRoom = null;
            mountHorseID = 0;
            mountBreedHorse = false;
            if (mountBreedHorseObject != null)
                GameObject.Destroy(mountBreedHorseObject);
            isGoToSiyangchang = false;
            isChuangShi = false;
        }
    }
}
