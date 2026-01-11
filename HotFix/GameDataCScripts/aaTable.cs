using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* aaTable
*/
namespace HotFix
{
	[System.Serializable]
	public class aaTableParSer
	{
		public List<aaTable> data = new List<aaTable>();
		public List<aaTable> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class aaTable:ExcelItem
	{
	/// <summary>
	/// 血统编号
	/// <summary>
	public string BloodNum;
	/// <summary>
	/// 血统名称
	/// <summary>
	public string BloodName;

	}
}