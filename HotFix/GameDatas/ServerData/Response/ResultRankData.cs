using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix.GameDatas.ServerData.Response
{
    public class ResultRankData
    {
        
        public string nickname;
        public int rank;
        public string rewardAmount;
        public string useTime;
        public string userId;
        public string horseCode;

        public ResultRankData(string nickname, int rank, string rewardAmount, string useTime, string userId, string horseCode)
        {
            this.nickname = nickname;
            this.rank = rank;
            this.rewardAmount = rewardAmount;
            this.useTime = useTime;
            this.userId = userId;
            this.horseCode = horseCode;
        }
    }
}
