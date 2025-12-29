using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class QuotaData
    {
        public string id;
        public string code;
        public string name;
        public string pic;
        public string amount;
        public double createDatetime;

        public QuotaData(string id, string code, string name, string pic, string amount, double createDatetime)
        {
            this.id = id;
            this.code = code;
            this.name = name;
            this.pic = pic;
            this.amount = amount;
            this.createDatetime = createDatetime;
        }
    }
    public class HorseRentOutData
    {
        public string id;
        public string horseId;
        public string horseName;
        public string horseCode;
        public string horseType;
        public string price;
        public string status;
        public string pic;
        public string countdown;

        public HorseRentOutData(string id, string horseId, string horseName, string horseCode, string horseType, string price, string status, string pic)
        {
            this.id = id;
            this.horseId = horseId;
            this.horseName = horseName;
            this.horseCode = horseCode;
            this.horseType = horseType;
            this.price = price;
            this.status = status;
            this.pic = pic;
        }
    }



    public class QuestionData
    {
        public string id;
        public string type;
        public string content;

        public QuestionData(string id, string type, string content)
        {
            this.id = id;
            this.type = type;
            this.content = content;
        }
    }
}
