/*
* Author:W
* Excel表转换生成
* AllGateData
*/
using System.Collections.Generic;

namespace HotFix
{
    [System.Serializable]
    public class AllGateData
	{
		/// <summary>
		/// 传送点ID
		/// <summary>
		public int ID;
		/// <summary>
		/// 传送点描述
		/// <summary>
		public string Des;
		/// <summary>
		/// 传送点位置
		/// <summary>
		public float[] Pos;
		/// <summary>
		/// 图集名称
		/// <summary>
		public string ScriptAtlasName;
		/// <summary>
		/// 图片名称
		/// <summary>
		public string ScriptName;
		/// <summary>
		/// 传送点按钮在地图中的位置
		/// <summary>
		public float[] ImgPos;
		/// <summary>
		/// 传送点所属于的地图
		/// <summary>
		public int Parent;

        public AllGateData(int iD, string des, string pos, string scriptAtlasName, string scriptName, string imgPos, int parent)
        {
            ID = iD;
            Des = des; 
			string[] m_Pos = pos.Split('|');
			Pos = new float[m_Pos.Length];
			for (int i = 0; i < m_Pos.Length; i++)
            {
				Pos[i] = float.Parse(m_Pos[i]);
            }
            ScriptAtlasName = scriptAtlasName;
            ScriptName = scriptName;
			string[] m_ImgPos = imgPos.Split('|');
			ImgPos = new float[m_ImgPos.Length];
			for (int i = 0; i < m_ImgPos.Length; i++)
			{
				ImgPos[i] = float.Parse(m_ImgPos[i]);
			}
            Parent = parent;
        }
    }
}