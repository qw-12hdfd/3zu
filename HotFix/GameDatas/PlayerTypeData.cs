using System.Collections.Generic;
/*
* Author:W
* Excel表转换生成
* PlayerTypeData
*/
namespace HotFix
{
	[System.Serializable]
	public class PlayerTypeData
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
		public int[] ManIDList;
		/// <summary>
		/// 女的人物ID列表
		/// <summary>
		public int[] WoManIDList;
		/// <summary>
		/// 图片路径
		/// </summary>
		public string ImgPath;
		/// <summary>
		/// 图片名称
		/// </summary>
		public string ImgName;

		public PlayerTypeData(int iD, string des, string manIDList, string woManIDList, string imgPath,string imgName)
		{
			ID = iD;
			Des = des;
			string[] man = manIDList.Split('|');
			string[] women = woManIDList.Split('|');
			ManIDList = new int[man.Length];
			WoManIDList = new int[women.Length];
			ImgPath = imgPath;
			ImgName = imgName;

			for (int i = 0; i < women.Length; i++)
			{
				WoManIDList[i] = int.Parse(women[i]);
			}
			for (int i = 0; i < man.Length; i++)
			{
				ManIDList[i] = int.Parse(man[i]);
			}
		}
	}
}