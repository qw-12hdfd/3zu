using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class BreedSiteList
    {
        public string id;
        public HorseData horseFatherData;
        public HorseData horseMotherData;
        public int roomNumber;
        public string useStatus;
        public string status;
        public string price;
    }

    public class BreedSiteData
    {
        public string accountAvailableAmount;
        public int queueNumber;
        public string price;
        public int time;
        public List<BreedSiteList> list;

        public BreedSiteData(string accountAvailableAmount, int queueNumber, string price, int time, List<BreedSiteList> list)
        {
            this.accountAvailableAmount = accountAvailableAmount;
            this.queueNumber = queueNumber;
            this.price = price;
            this.time = time;
            this.list = list;
        }
    }
}
