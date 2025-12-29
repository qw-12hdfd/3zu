using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class HorseGrowUpData
    {

        public int queueNumber;
        public string id;
        public string objectCode;
        public string objectLineage;
        public string objectName;
        public string objectType;
        public string pairingPrice;
        public string alreadyPregnancyProgress;
        public string objectPic;
        public string objectSex;
        public string remainPregnancyTime;
        public string totalPregnancyProgress;
        public HorseGrowUpData(int queueNumber = 0, string id = null, string objectCode = null, string objectLineage = null, string objectName = null, string objectType = null, string pairingPrice = null, string alreadyPregnancyProgress = null, string objectPic = null, string objectSex = null, string remainPregnancyTime = null, string totalPregnancyProgress = null)
        {
            this.queueNumber = queueNumber;
            this.id = id;
            this.objectCode = objectCode;
            this.objectLineage = objectLineage;
            this.objectName = objectName;
            this.objectType = objectType;
            this.pairingPrice = pairingPrice;
            this.alreadyPregnancyProgress = alreadyPregnancyProgress;
            this.objectPic = objectPic;
            this.objectSex = objectSex;
            this.remainPregnancyTime = remainPregnancyTime;
            this.totalPregnancyProgress = totalPregnancyProgress;
        }
    }
    public class NumberOfPages
    {
        public int pageNum;
        public int pageSize;

        public NumberOfPages(int pageNum, int pageSize)
        {
            this.pageNum = pageNum;
            this.pageSize = pageSize;
        }
    }
    public class MyHorseData
    {
        //{ "id":"10","userId":"503356083155509248","horseName":"汗血宝马","horseId":"4","pic":"https://metat.oss-accelerate.aliyuncs.com/1660901998873.jpeg","status":"0","joinNumer":0,"winRate":"0","updateDatetime":1670208191000}
        public int ID;
        public string UserID;
        public string Name;
        public int HourseID;
        public string IconUrl;
        public int Status;
        public int JoinNumber;
        public float WinRate;
        public string UpdateDataTime;

        public MyHorseData(int iD, string userID, string name, int hourseID, string iconUrl, int status, int joinNumber, float winRate, string updateDataTime)
        {
            ID = iD;
            UserID = userID;
            Name = name;
            HourseID = hourseID;
            IconUrl = iconUrl;
            Status = status;
            JoinNumber = joinNumber;
            WinRate = winRate;
            UpdateDataTime = updateDataTime;
        }
    }
    public class HorseData
    {
        public string id;
        public string status;                   //状态
        public string stage;                    //阶段
        public string code;
        public int horseStableNo;
        public int feedNember;
        public GameObject horse;
        public HorseObject horseCtrl;
        public HorseBreedObject horseBreedCtrl;

        public HorseData(string id, string code, int horseStableNo, int feedNember)
        {
            this.id = id;
            this.code = code;
            this.horseStableNo = horseStableNo;
            this.feedNember = feedNember;
        }
    }
    public class HorseDetail
    {
        public string id;                       //马匹id
        public string code;                     //马匹编码
        public string name;                     //名称
        public string age;                      //年龄 几个月零几天
        public string endurance;                //耐力
        public string startSpeed;               //起跑
        public string speed;                    //速度
        public string wisdom;                   //智慧
        public string fatigue;                  //疲劳度
        public string enduranceMax;             //耐力
        public string startSpeedMax;            //起跑
        public string speedMax;                 //速度
        public string wisdomMax;                //智慧
        public string fatigueMax;               //疲劳度
        public string status;                   //状态
        public string stage;                    //阶段
        public string isRent;                   //是否租赁中
        public string totalFeedProgress;        //总投喂长度
        public string remainFeedProgress;       //剩余投喂长度
        public string remainFeedTime;           //剩余投喂时间
        public string totalGrowUpProgress;      //总成长时间
        public string alreadyGrowUpProgress;    //已成长时间
        public string remainGrowUpNumber;       //剩余成年马粟
        public int remainGrowUpTime;            //剩余成长时间
        public int matchNumebr;                 //参加场次
        public string winRate;                  //胜率
        public string pic;                      //头像
    }
    public class BirthHorseData
    {
      
        public string id;
        public string code;
        public string type;
        public string sex;
        public string lineage;
        public string endurance;
        public string startSpeed;
        public string speed;
        public string wisdom;
        public string fatigue;
        public string enduranceMax;
        public string startSpeedMax;
        public string speedMax;
        public string wisdomMax;
        public string fatigueMax;
        public BirthHorseData(string id, string code, string type, string sex, string lineage, string endurance, string startSpeed, string speed, string wisdom, string fatigue, string enduranceMax, string startSpeedMax, string speedMax, string wisdomMax, string fatigueMax)
        {
            this.id = id;
            this.code = code;
            this.type = type;
            this.sex = sex;
            this.lineage = lineage;
            this.endurance = endurance;
            this.startSpeed = startSpeed;
            this.speed = speed;
            this.wisdom = wisdom;
            this.fatigue = fatigue;
            this.enduranceMax = enduranceMax;
            this.startSpeedMax = startSpeedMax;
            this.speedMax = speedMax;
            this.wisdomMax = wisdomMax;
            this.fatigueMax = fatigueMax;
        }
    }
    public class BirthHorseNameData
    {

        public string id;
        public string name;
        public BirthHorseNameData(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
        public class HorseID
    {
        public int horseId;

        public HorseID(int horseId)
        {
            this.horseId = horseId;
        }
    }

    public class JoinRoomData
    {
        public int id;
        public int horseId;
        public string pwd;

        public JoinRoomData(int id, int horseId, string pwd)
        {
            this.id = id;
            this.horseId = horseId;
            this.pwd = pwd;
        }
    }
    public class PurchaseTypeData
    {
        public string parentKey;

        public PurchaseTypeData(string parentKey)
        {
            this.parentKey = parentKey;
        }
    }
    public class AgreementData
    {
        public string key;

        public AgreementData(string key)
        {
            this.key = key;
        }
    }
    public class BreedHouseListData
    {
       
        public string id;
        public int roomNumber;
        public string useStatus;
        public BreedHouseListData(string id, int roomNumber, string useStatus)
        {
            this.id = id;
            this.roomNumber = roomNumber;
            this.useStatus = useStatus;
        }
    }
    public class OrderOpenHouseData
    {

        public string payType;
        public int horseId;
        public float price;
        public string wxAppId;
        public OrderOpenHouseData(int horseId, float price, string payType = "", string wxAppId="")
        {
            this.payType = payType;
            this.horseId = horseId;
            this.price = price;
            this.wxAppId = wxAppId;
        }
    }
    public class PayTypeData
    {
        public int id;
        public string type;
        public string parentKey;
        public string key;
        public string value;
        public string updater="";
        public double updateDatetime=0;
        public PayTypeData(int id, string type, string parentKey, string key, string value, string updater="", double updateDatetime=0)
        {
            this.id = id;
            this.type = type;
            this.parentKey = parentKey;
            this.key = key;
            this.value = value;
            this.updater = updater;
            this.updateDatetime = updateDatetime;
        }
    }
    public class HorseIdData
    {
        public int id;

        public HorseIdData(int horseID)
        {
            this.id = horseID;
        }
    }

    public class HorseSex
    {
        public int sex;
        public int status;
        public int stage;
        public int isBreed;
        public HorseSex(int sex, int status, int stage, int isBreed)
        {
            this.sex = sex;
            this.status = status;
            this.stage = stage;
            this.isBreed = isBreed;
        }
    }

    public class HorseListType
    {
        public string[] statusList;

        public HorseListType(string[] statusList)
        {
            this.statusList = statusList;
        }
    }
    public class PurchaseData
    {
        public string payType;
        public string pwd;
        public string wxAppId;
        public int quantity;
        public PurchaseData(string payType, string pwd, string wxAppId, int quantity)
        {
            this.payType = payType;
            this.pwd = pwd;
            this.wxAppId = wxAppId;
            this.quantity = quantity;
        }
    }
    public class JoinRoom
    {
        public int id;
        public int horseId;

        public JoinRoom(int id,int horseId)
        {
            this.id = id;
            this.horseId = horseId;
        }
    }
}
