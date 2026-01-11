using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* TypeTable
*/
namespace HotFix
{
	[System.Serializable]
	public class TypeTableParSer
	{
		public List<TypeTable> data = new List<TypeTable>();
		public List<TypeTable> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class TypeTable:ExcelItem
	{
	/// <summary>
	/// 类型编号
	/// <summary>
	public string TypeNum;
	/// <summary>
	/// 类型名称
	/// <summary>
	public string TypeName;

	}
}