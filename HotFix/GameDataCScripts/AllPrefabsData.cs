using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* AllPrefabsData
*/
namespace HotFix
{
	[System.Serializable]
	public class AllPrefabsDataParSer
	{
		public List<AllPrefabsData> data = new List<AllPrefabsData>();
		public List<AllPrefabsData> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class AllPrefabsData:ExcelItem
	{
	/// <summary>
	/// 预制体id
	/// <summary>
	public int ID;
	/// <summary>
	/// 预制体名称
	/// <summary>
	public string Name;
	/// <summary>
	/// 预制体路径
	/// <summary>
	public string Path;

	}
}