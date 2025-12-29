using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using W.GameFrameWork.ExcelTool;
using UnityEngine;
using LitJson;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace HotFix
{
    public static class JsonConfigManager
    {
        private static List<AllPrefabsData> allPrefabsData = null;
        private static Dictionary<int, AllPrefabsData> allPrefabsDataDic = new Dictionary<int, AllPrefabsData>();
        private static List<BuildingData> allBuildingData = null;
        private static Dictionary<int, BuildingData> buildingDataDic = new Dictionary<int, BuildingData>();
        private static Dictionary<string, string> HorseBloodDataDic = new Dictionary<string, string>();
        private static Dictionary<string, string> HorseTypeDataDic = new Dictionary<string, string>();
        private static string parentPath = "Assets/GameData/Data/Json/";
        public static List<string> buildingDataList = new List<string>()
        {
            "SceneData",
        };

        /// <summary>
        /// 通过文件名称获取jsonData 然后回调
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        private static void AnalyzeJson(string path, Action<JsonData> callback)
        {

            string jsonPath = parentPath + path + ".json";
            Debug.Log(jsonPath + "  ==  jsonpath");
            string jsonStr = ResourceManager.instance.LoadResources<TextAsset>(jsonPath).ToString();
            
            Debug.Log(jsonStr + "  ==  jsonStr");
            JsonData temp = JsonMapper.ToObject(jsonStr);


            callback(temp);

        }

        /// <summary>
        /// 获取allBuildingData的方法
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<int, BuildingData> GetBuildingData(string tableName)
        {
            allBuildingData = new List<BuildingData>();
            AnalyzeJson(tableName, (JsonData temp) =>
            {
                foreach (JsonData item in temp["data"])
                {

                    //Debug.Log("allBuildingData itemStr==" + item.ToJson());
                    BuildingData t = new BuildingData((int)item["ID"], (int)item["PrefabID"], item["Name"].ToString(), (int)item["Parent"], item["Position"].ToString(), item["Rotation"].ToString(), item["Scale"].ToString());
                    Debug.LogError(t);
                    allBuildingData.Add(t);
                    //Debug.Log("allBuildingData.count==" + allBuildingData.Count);
                }
            });
            buildingDataDic.Clear();
            foreach (var item in allBuildingData)
            {
                buildingDataDic.Add(item.ID, item);
            }
            return buildingDataDic;
        }

        /// <summary>
        /// 获取allPrefabsData的方法
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<int, AllPrefabsData> GetPrefabsData()
        {
            if (allPrefabsData == null)
            {
                allPrefabsData = new List<AllPrefabsData>();
                AnalyzeJson("AllPrefabsData", (JsonData temp) =>
                {
                    foreach (JsonData item in temp["data"])
                    {

                        Debug.Log("AllPrefabsData itemStr==" + item.ToJson());
                        AllPrefabsData t = new AllPrefabsData((int)item["ID"], item["Name"].ToString(), item["Path"].ToString());
                        allPrefabsData.Add(t);
                        Debug.Log("allPrefabsData.count==" + allPrefabsData.Count);
                    }
                });
            }
            allPrefabsDataDic.Clear();
            foreach (var item in allPrefabsData)
            {
                Debug.Log("allPrefabData" + item.ID);
                allPrefabsDataDic.Add(item.ID, item);
            }
            return allPrefabsDataDic;
        }

        /// <summary>
        /// 获取马匹血统的方法
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, string> GetHorseBloodData()
        {
            if (HorseBloodDataDic.Count<=0)
            {
                HorseBloodDataDic = new Dictionary<string, string>();
                AnalyzeJson("BloodTable", (JsonData temp) =>
                {
                    foreach (JsonData item in temp["data"])
                    {
                        HorseBlood t = JsonMapper.ToObject<HorseBlood>(item.ToJson());
                        HorseBloodDataDic.Add(t.BloodNum, t.BloodName);
                    }
                });
            }
            return HorseBloodDataDic;
        }

        /// <summary>
        /// 获取马匹类型的方法
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, string> GetHorseTypeData()
        {
            if (HorseTypeDataDic.Count<=0)
            {
                HorseTypeDataDic = new Dictionary<string, string>();
                AnalyzeJson("TypeTable", (JsonData temp) =>
                {
                    foreach (JsonData item in temp["data"])
                    {
                        HorseType t = JsonMapper.ToObject<HorseType>(item.ToJson());
                        HorseTypeDataDic.Add(t.TypeNum, t.TypeName);
                    }
                });
            }
            return HorseTypeDataDic;
        }
    }
}
