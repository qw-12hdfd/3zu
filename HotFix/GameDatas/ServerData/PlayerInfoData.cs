using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class PlayerInfoData
    {
        public string homeOwnerId;
        public string horseCode;
        public string horseName;
        public string nickname;
        public int number;
        public string readyFlag;
        public int readyNumber;
        public int track;
        public string userId;
        public int countdown;
        public string distance;
        public string speed;

        /// <param name="homeOwnerId">房间id</param>
        /// <param name="horseCode">马匹code</param>
        /// <param name="horseName">马匹名称</param>
        /// <param name="nickname">马匹名称</param>
        /// <param name="number">第几个</param>
        /// <param name="readyFlag">是否准备</param>
        /// <param name="readyNumber"></param>
        /// <param name="track"></param>
        /// <param name="userId">马匹id</param>
        public PlayerInfoData(string homeOwnerId, string horseCode, string horseName, string nickname, int number, string readyFlag, int readyNumber, int track, string userId, int countdown, string distance, string speed)
        {
            this.homeOwnerId = homeOwnerId;
            this.horseCode = horseCode;
            this.horseName = horseName;
            this.nickname = nickname;
            this.number = number;
            this.readyFlag = readyFlag;
            this.readyNumber = readyNumber;
            this.track = track;
            this.userId = userId;
            this.countdown = countdown;
            this.distance = distance;
            this.speed = speed;
        }
    }
}
