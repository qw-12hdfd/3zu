using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix.Common.Utils
{
    public class HorseUtils
    {
        private static Material maneMaterial;
        private static Material bodyMaterial;
        private static Material tailMaterial;
        private static Material eyesMaterial;
        private static void SetHorseMane(Color color)
        {
            maneMaterial.SetColor("_BaseColor", color);
        }
        private static void SetHorseBody(Texture texture)
        {
            bodyMaterial.SetTexture("_BaseMap", texture);
        }
        private static void SetHorseTail(Color color)
        {
            tailMaterial.SetColor("_BaseColor", color);
        }
        private static void SetHorseEyes(Texture texture)
        {
            eyesMaterial.SetTexture("_BaseMap", texture);
        }

        public static void SetHorseTexture(Transform hrose,string str)
        {
            maneMaterial = hrose.Find("Meshes/Horse Mane").GetComponent<Renderer>().material;
            tailMaterial = hrose.Find("Meshes/Horse Tail").GetComponent<Renderer>().material;
            bodyMaterial = hrose.Find("Meshes/Horse Body").GetComponent<Renderer>().material;
            eyesMaterial = hrose.Find("Meshes/Horse Eyes").GetComponent<Renderer>().material;
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

