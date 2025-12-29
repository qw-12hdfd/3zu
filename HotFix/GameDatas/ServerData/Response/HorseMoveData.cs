using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix.GameDatas.ServerData.Response
{
    public class HorseMoveData
    {
        public string speed;
        public string userId;
        public string distance;
        public string rank;

        public HorseMoveData(string speed, string userId, string distance, string rank)
        {
            this.speed = speed;
            this.userId = userId;
            this.distance = distance;
            this.rank = rank;
        }
    }
}
