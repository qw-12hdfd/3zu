using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* AllGateData
*/
namespace HotFix
{
	[System.Serializable]
	public class AllGateDataParSer
	{
		public List<AllGateData> data = new List<AllGateData>();
		public List<AllGateData> Data
		{
			get
			{
				return data;
			}
		}	}
	[System.Serializable]
	public class AllGateData : ExcelItem
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
		public string Pos;
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
		public string ImgPos;
		/// <summary>
		/// 传送点所属于的地图
		/// <summary>
		public int Parent;

	}
}