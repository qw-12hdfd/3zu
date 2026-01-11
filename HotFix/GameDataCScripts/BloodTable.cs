using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* BloodTable
*/
namespace HotFix
{
	[System.Serializable]
	public class BloodTableParSer
	{
		public List<BloodTable> data = new List<BloodTable>();
		public List<BloodTable> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class BloodTable:ExcelItem
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