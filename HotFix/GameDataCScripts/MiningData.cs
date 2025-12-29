using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* MiningData
*/
namespace HotFix
{
	[System.Serializable]
	public class MiningDataParSer
	{
		public List<MiningData> data = new List<MiningData>();
		public List<MiningData> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class MiningData:ExcelItem
	{
	/// <summary>
	/// 位置ID
	/// <summary>
	public int ID;
	/// <summary>
	/// 坐标(x|y|z)
	/// <summary>
	public string Position;

	}
}