using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    [System.Serializable]
    public class HorseBlood
    {
        public string BloodNum;
        public string BloodName;

        public HorseBlood(string bloodNum, string bloodName)
        {
            BloodNum = bloodNum;
            BloodName = bloodName;
        }
    }
    [System.Serializable]
    public class HorseTypeParSer
    {
        public List<HorseType> data = new List<HorseType>();
        public List<HorseType> Data
        {
            get
            {
                return data;
            }
        }
    }
    [System.Serializable]
    public class HorseType
    {
        public string TypeNum;
        public string TypeName;

        public HorseType(string typeNum, string typeName)
        {
            TypeNum = typeNum;
            TypeName = typeName;
        }
    }

    public class ListFront
    {
        public string type;

        public ListFront(string type)
        {
            this.type = type;
        }
    }
}
