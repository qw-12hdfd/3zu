using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix.GameDatas.ServerData.Request
{
    public class JoinRoomPlayerData
    {
        public string homeOwnerFlag;
        public string horsePhoto;
        public string nickname;
        public string readyFlag;
        public double userId;

        public JoinRoomPlayerData() { }

        public JoinRoomPlayerData(string homeOwner,string horsePhoto,string nickname,string readyFlag, double userId)
        {
            this.homeOwnerFlag = homeOwner;
            this.horsePhoto = horsePhoto;
            this.nickname = nickname;
            this.readyFlag = readyFlag;
            this.userId = userId;
        }
    }
}
