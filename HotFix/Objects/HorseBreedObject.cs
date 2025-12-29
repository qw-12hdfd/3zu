using Cinemachine;
using HotFix.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class HorseBreedObject:ObjectParent
    {
        GameObject horseMane;
        GameObject horseBody;
        GameObject horseTail;
        GameObject horseEyes;
        Material maneMaterial;
        Material bodyMaterial;
        Material tailMaterial;
        Material eyesMaterial;
        string datastr = "F-00-RR-CA-G1-W2-DP00";
        HorseData data;
        TriggerEvent horseEvent;
        public int price = 0;
        public string roomId;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            horseMane = m_Transform.Find("Meshes/Horse Mane").gameObject;
            horseTail = m_Transform.Find("Meshes/Horse Tail").gameObject;
            horseBody = m_Transform.Find("Meshes/Horse Body").gameObject;
            horseEyes = m_Transform.Find("Meshes/Horse Eyes").gameObject;
            maneMaterial = horseMane.GetComponent<Renderer>().material;
            bodyMaterial = horseBody.GetComponent<Renderer>().material;
            tailMaterial = horseTail.GetComponent<Renderer>().material;
            eyesMaterial = horseEyes.GetComponent<Renderer>().material;
            horseEvent = m_Transform.GetComponent<TriggerEvent>();
            horseEvent.ClickAct = ClickHorseAct;
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            data = ((HorseData)param1);
            price = int.Parse(param2.ToString());
            roomId = param3.ToString();
            SetHorseTexture(data.code);
        }

        private void ClickHorseAct(object[] obj)
        {
            if (!UIManager.instance.IsSignWindowOpen(FilesName.MAINPANEL))
                return;
            foreach(var item in UserInfoManager.MyHorseList)
            {
                if(item.Value.id == data.id)
                {
                    RFrameWork.instance.OpenCommonConfirm("提示", "不能和自己的公马繁育，去别的房间看看吧", () => {}, null);
                    return;
                }
            }
            var array = data.code.ToCharArray();
            if (array[0] == 'M')
            {
                UserInfoManager.detailPanelType = 6;
                UserInfoManager.breedPrice = price;
                UserInfoManager.breedRoomId = roomId;
                WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.detailFront + "/" + data.id, WebRequestFuncitons.GetHorseDetailData, true, "{}", RFrameWork.instance.token);
            }
        }

        public void SetHorseMane(Color color)
        {
            maneMaterial.SetColor("_BaseColor", color);
        }
        public void SetHorseBody(Texture texture)
        {
            bodyMaterial.SetTexture("_BaseMap", texture);
        }
        public void SetHorseTail(Color color)
        {
            tailMaterial.SetColor("_BaseColor", color);
        }
        public void SetHorseEyes(Texture texture)
        {
            eyesMaterial.SetTexture("_BaseMap", texture);
        }

        public void SetHorseTexture(string str)
        {
            string[] array = str.Split('-');
            string bodyColor = "Red";
            switch (array[2])
            {
                case "RR":
                    bodyColor = "Red";
                    break;
                case "WW":
                    bodyColor = "White";
                    break;
                case "BB":
                    bodyColor = "Black";
                    break;
                case "YY":
                    bodyColor = "Brown";
                    break;
                case "GG":
                    bodyColor = "Gray";
                    break;
                default:
                    break;
            }
            string eyesColor = array[3];
            Color color1 = new Color(1, 1, 1);
            switch (array[4])
            {
                case "W1":
                    color1 = new Color(1, 1, 1);
                    break;
                case "G1":
                    color1 = new Color(0.6f, 0.6f, 0.6f);
                    break;
                case "R1":
                    color1 = new Color(0.7f, 0.5f, 0.3f);
                    break;
                case "B1":
                    color1 = new Color(0.3f, 0.3f, 0.3f);
                    break;
                case "Y1":
                    color1 = new Color(1f, 0.95f, 0.47f);
                    break;
                case "O1":
                    color1 = new Color(1f, 0.75f, 0.47f);
                    break;
                default:
                    break;
            }
            Color color2 = new Color(0, 0, 0);
            switch (array[5])
            {
                case "W2":
                    color2 = new Color(1, 1, 1);
                    break;
                case "G2":
                    color2 = new Color(0.7f, 0.7f, 0.7f);
                    break;
                case "R2":
                    color2 = new Color(0.7f, 0.5f, 0.3f);
                    break;
                case "B2":
                    color2 = new Color(0.3f, 0.3f, 0.3f);
                    break;
                case "Y2":
                    color2 = new Color(1f, 0.95f, 0.47f);
                    break;
                case "O2":
                    color2 = new Color(1f, 0.75f, 0.47f);
                    break;
                default:
                    break;
            }
            string bodyColorCopy = bodyColor;
            bodyColor = bodyColor + "_" + array[6];
            var bodyTexture = ResourceManager.instance.LoadResources<Texture>("Assets/GameData/Materials/Animals/Textures/Horse_Textures/HorseBody/HorseBody" + bodyColor + ".png");
            if (bodyTexture != null)
            {
                SetHorseBody(bodyTexture);
            }
            else
            {
                SetHorseBody(ResourceManager.instance.LoadResources<Texture>("Assets/GameData/Materials/Animals/Textures/Horse_Textures/HorseBody/HorseBody" + bodyColorCopy + "_DP00.png"));
            }
            SetHorseEyes(ResourceManager.instance.LoadResources<Texture>("Assets/GameData/Materials/Animals/Textures/Horse_Textures/Horse_Eye/Horse_Eye_" + eyesColor + ".png"));
            SetHorseMane(color1);
            SetHorseTail(color2);
        }
    }
}
