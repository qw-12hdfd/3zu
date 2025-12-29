using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* PlayerTypeData
*/
namespace HotFix
{
	[System.Serializable]
	public class PlayerTypeDataParSer
	{
		public List<PlayerTypeData> data = new List<PlayerTypeData>();
		public List<PlayerTypeData> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class PlayerTypeData:ExcelItem
	{
	/// <summary>
	/// 人物类型ID
	/// <summary>
	public int ID;
	/// <summary>
	/// 人物类型介绍
	/// <summary>
	public string Des;
	/// <summary>
	/// 男的人物ID列表
	/// <summary>
	public string ManIDList;
	/// <summary>
	/// 女的人物ID列表
	/// <summary>
	public string WoManIDList;
	/// <summary>
	/// 类型按钮图片路径
	/// <summary>
	public string ImgPath;
	/// <summary>
	/// 图片名称
	/// <summary>
	public string ImgName;

	}
}