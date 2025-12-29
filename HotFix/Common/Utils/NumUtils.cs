using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class NumUtils
    {
        //一般是单独配一个单位表 读表获取
        static string[] unitList = new string[] { "", "万", "亿","万亿" };

        /// <summary>
        /// 格式化货币
        /// </summary>
        /// digit:保留几位小数
        public static string FormatCurrency(float num, int digit = 1)
        {
            float tempNum = num;
            long v = 10000;//几位一个单位
            int unitIndex = 0;
            while (tempNum >= v)
            {
                unitIndex++;
                tempNum /= v;
            }

            string str = "";
            if (unitIndex >= unitList.Length)
            {
                Debug.LogError("超出单位表中的最大单位");
                str = num.ToString();
            }
            else
            {
                tempNum = Round(tempNum, digit);
                str = $"{tempNum}{unitList[unitIndex]}";
            }
            return str;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// digits:保留几位小数
        public static float Round(float value, int digits = 1)
        {
            float multiple = Mathf.Pow(10, digits);
            float tempValue = value * multiple + 0.5f;
            tempValue = Mathf.FloorToInt(tempValue);
            return tempValue / multiple;
        }
    }
}
