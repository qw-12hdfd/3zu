using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{

    public class BillData
    {
        public string bizCategory;
        public string bizCategoryNote; //名字1
        public string remark;  //名字2
        public string bizType;
        public string createDatetimeStr;
        public string id;
        public string transAmount; //变动金额

        public BillData(string bizCategory, string bizCategoryNote, string bizNote, string bizType, string createDatetimeStr, string id, string transAmount)
        {
            this.bizCategory = bizCategory;
            this.bizCategoryNote = bizCategoryNote;
            this.remark = bizNote;
            this.bizType = bizType;
            this.createDatetimeStr = createDatetimeStr;
            this.id = id;
            this.transAmount = transAmount;
        }
    }
}
