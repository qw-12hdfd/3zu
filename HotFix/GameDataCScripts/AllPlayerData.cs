using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* AllPlayerData
*/
namespace HotFix
{
	[System.Serializable]
	public class AllPlayerDataParSer
	{
		public List<AllPlayerData> data = new List<AllPlayerData>();
		public List<AllPlayerData> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class AllPlayerData:ExcelItem
	{
	/// <summary>
	/// 人物ID
	/// <summary>
	public int ID;
	/// <summary>
	/// 人物名称
	/// <summary>
	public string Name;
	/// <summary>
	/// 人物路径
	/// <summary>
	public string Path;

	}
}