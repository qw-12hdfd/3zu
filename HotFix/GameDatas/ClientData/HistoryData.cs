using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class HistoryData
    {
        public int ID;
        public string HorsePic;
        public string HorseName;
        public string MatchName;
        public int Number;
        public string UserID;
        public int Rank;
        public string CreateDatetime;

        public HistoryData(int iD, string horsePic, string horseName, string matchName, int number, string userID, int rank, string createDatetime)
        {
            ID = iD;
            HorsePic = horsePic;
            HorseName = horseName;
            MatchName = matchName;
            Number = number;
            UserID = userID;
            Rank = rank;
            CreateDatetime = createDatetime;
        }
    }
    public class HorseHistoryData
    {
        public string id;
        public string horseName;
        public string horsePic;
        public string matchName;
        public int number;
        public string userId;
        public int rank;
        public string createDatetimeString;
        public string rewardAmount;
    }
}
