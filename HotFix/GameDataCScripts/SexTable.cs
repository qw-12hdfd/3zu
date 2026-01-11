using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* SexTable
*/
namespace HotFix
{
	[System.Serializable]
	public class SexTableParSer
	{
		public List<SexTable> data = new List<SexTable>();
		public List<SexTable> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class SexTable:ExcelItem
	{
	/// <summary>
	/// 性别编号
	/// <summary>
	public string SexType;
	/// <summary>
	/// 性别名称
	/// <summary>
	public string SexName;

	}
}