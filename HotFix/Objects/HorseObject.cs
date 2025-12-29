using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class HorseObject:ObjectParent
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
        Animator ani;
        public bool isMount = false;
        // Start is called before the first frame update
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            horseMane = m_Transform.Find("Meshes/Horse Mane").gameObject;
            horseTail = m_Transform.Find("Meshes/Horse Tail").gameObject;
            horseBody = m_Transform.Find("Meshes/Horse Body").gameObject;
            horseEyes = m_Transform.Find("Meshes/Horse Eyes").gameObject;
            Debug.Log("查找马匹身上组件");
            maneMaterial = horseMane.GetComponent<Renderer>().material;
            bodyMaterial = horseBody.GetComponent<Renderer>().material;
            tailMaterial = horseTail.GetComponent<Renderer>().material;
            eyesMaterial = horseEyes.GetComponent<Renderer>().material;
            Debug.Log("查找马匹身上材质");
            ani = m_Transform.GetComponent<Animator>();
            Debug.Log("获取动画");
            horseEvent = m_Transform.GetComponent<TriggerEvent>();
            horseEvent.ClickAct = ClickHorseAct;
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            if (param1 != null)
                data = ((HorseData)param1);
            else
                data = new HorseData("", datastr, 0, 0);
            SetHorseTexture(String.IsNullOrEmpty(data.code)? datastr:data.code);
            m_GameObject.name = data.id;
            if (data.feedNember > 0)
            {
                ani.SetBool("Eat", true);
            }
        }

        private void ClickHorseAct(object[] obj)
        {
            if (UserInfoManager.mount == false && UIManager.instance.IsSignWindowOpen(FilesName.MAINPANEL) && data.id == UserInfoManager.maCaoTransform.Find("HorsePos").GetChild(0).name)
            {
                MainWindow.MountHorse(true,isMount);
                if(ani.GetBool("Eat") == true)
                    ani.SetBool("Eat", false);
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
            if (array[0] == "F")
            {
                isMount = !(data.status == "5" || data.status == "11" || data.stage == "0");
            }
            else
            {
                isMount = !(data.status == "5" || data.status == "11" || data.stage == "0");
            }
            m_Transform.Find("CG").localScale = data.stage == "0" ? new Vector3(0.6F, 0.6F, 0.6F) : new Vector3(1, 1, 1);
            m_Transform.Find("- Cloth Colliders - ").localScale = data.stage == "0" ? new Vector3(0.6F, 0.6F, 0.6F) : new Vector3(1, 1, 1);
            m_Transform.localPosition = data.stage == "0" ? new Vector3(m_Transform.localPosition.x, m_Transform.localPosition.y, 0.5F) : m_Transform.localPosition;

            Debug.Log("SetHorseTexture 马匹设置是否可以遛马"+isMount);
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
            Debug.Log("SetHorseTexture 马匹设置body颜色" + bodyColor);
            string eyesColor = array[3];
            Debug.Log("SetHorseTexture 马匹设置眼睛颜色" + eyesColor);
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
            Debug.Log("SetHorseTexture 马匹设置鬃毛颜色" + array[4]);
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
            Debug.Log("SetHorseTexture 马匹设置尾巴颜色" + array[5]);
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
