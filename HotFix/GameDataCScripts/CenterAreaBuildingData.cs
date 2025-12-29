using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* CenterAreaBuildingData
*/
namespace HotFix
{
	[System.Serializable]
	public class CenterAreaBuildingDataParSer
	{
		public List<CenterAreaBuildingData> data = new List<CenterAreaBuildingData>();
		public List<CenterAreaBuildingData> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class CenterAreaBuildingData:ExcelItem
	{
	/// <summary>
	/// 区域内唯一ID
	/// <summary>
	public int ID;
	/// <summary>
	/// 预制体id
	/// <summary>
	public int PrefabID;
	/// <summary>
	/// 预制体名称
	/// <summary>
	public string Name;
	/// <summary>
	/// 父节点ID（如果为空为-1）
	/// <summary>
	public int Parent;
	/// <summary>
	/// 坐标(x|y|z)
	/// <summary>
	public string Position;
	/// <summary>
	/// 旋转(x|y|z)
	/// <summary>
	public string Rotation;
	/// <summary>
	/// 大小(x|y|z)
	/// <summary>
	public string Scale;

	}
}