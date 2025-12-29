using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class RoomDatas
    {//{"data":[{"id":21,"maxNumber":12,"name":"偏执的赛马场","number":1,"photo":"https://metaculture.oss-accelerate.aliyuncs.com/IOS_1647081192957624_1125_1125.jpg","status":"0"}]}
        public int ID;
        public int MaxNumber;
        public int Number;
        public string Name;
        public string Photo;
        public string State;// HORSE_MATCH_STATUS_0("0", "待开始"), HORSE_MATCH_STATUS_1("1", "进行中"),  HORSE_MATCH_STATUS_2("2", "已结束"), HORSE_MATCH_STATUS_3("3", "已废弃"),

        public RoomDatas(int iD, int maxNumber, int number, string name, string photo, string state)
        {
            ID = iD;
            MaxNumber = maxNumber;
            Number = number;
            Name = name;
            Photo = photo;
            State = state;
        }
    }
    public class WeChatData
    {
        public string merchantId;
        public string prepayId;
        public string payCode;
        public string appId;
        public string timeStamp;
        public string nonceStr;
        public string wechatPackage;
        public string signType;
        public string paySign;   
        public WeChatData(string merchantId, string prepayId, string payCode, string appId, string timeStamp, string nonceStr, string wechatPackage, string signType, string paySign)
        {
            this.merchantId = merchantId;
            this.prepayId = prepayId;
            this.payCode = payCode;
            this.appId = appId;
            this.timeStamp = timeStamp;
            this.nonceStr = nonceStr;
            this.wechatPackage = wechatPackage;
            this.signType = signType;
            this.paySign = paySign;
        }
    }
    public class GameData
    {
        public string id;
        public int number;
        public string roomNumber;
        public int maxNumber;
        public string status;
        public string endDatetime;
    }

    public class HistoryDatas
    {
        //    "id": 0,
        //"horseName": "",
        //"horsePic": "",
        //"matchName": "",
        //"number": 0,
        //"userId": 0,
        //"rank": 0,
        //"createDatetime": ""
        public string id;
        public string userId;
        public string roomNumber;
        public string horsePic;
        public string horseName;
        public int number;
        public int rank;
        public string rewardAmount;
        public string createDatetimeString;
    }
}
