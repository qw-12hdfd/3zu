using System.Collections.Generic;
/*
* Author:W
* Excel表转换生成
* AllPlayerData
*/
namespace HotFix
{
	[System.Serializable]
	public class AllPlayerData
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

		public AllPlayerData(int iD, string name, string path)
		{
			ID = iD;
			Name = name;
			Path = path;
		}
	}
}