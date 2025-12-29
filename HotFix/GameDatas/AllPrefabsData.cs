using System.Collections.Generic;
/*
* Author:W
* Excel表转换生成
* AllPrefabsData
*/
namespace HotFix
{
	public class AllPrefabsData
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

		public AllPrefabsData(int iD, string name, string path)
		{
			ID = iD;
			Name = name;
			Path = path;
		}
	}
}