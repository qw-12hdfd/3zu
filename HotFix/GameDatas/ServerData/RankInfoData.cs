using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class RankInfoData
    {
       
        public string userId;
        public string nickname;
        public int track;
        public float distance;
        public RankInfoData(string userId, string nickname, int track, float distance=-1)
        {
            this.userId = userId;
            this.nickname = nickname;
            this.track = track;
            this.distance = distance;
        }
    }
}
