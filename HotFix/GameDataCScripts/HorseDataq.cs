using System.Collections;
using System.Collections.Generic;
using W.GameFrameWork.ExcelTool;
/*
* Author:W
* Excel表转换生成
* HorseDataq
*/
namespace HotFix
{
	[System.Serializable]
	public class HorseDataqParSer
	{
		public List<HorseDataq> data = new List<HorseDataq>();
		public List<HorseDataq> Data
		{
			get
			{
				return data;
			} 
		}	} 
	[System.Serializable]
	public class HorseDataq:ExcelItem
	{
	/// <summary>
	/// 马匹id
	/// <summary>
	public string id;
	/// <summary>
	/// 马匹编号
	/// <summary>
	public string code;
	/// <summary>
	/// 名称
	/// <summary>
	public string name;
	/// <summary>
	/// 年龄
	/// <summary>
	public string age;
	/// <summary>
	/// 耐力
	/// <summary>
	public string endurance;
	/// <summary>
	/// 起跑
	/// <summary>
	public string startSpeed;
	/// <summary>
	/// 速度
	/// <summary>
	public string speed;
	/// <summary>
	/// 智慧
	/// <summary>
	public string wisdom;
	/// <summary>
	/// 疲劳度
	/// <summary>
	public string fatigue;
	/// <summary>
	/// 耐力
	/// <summary>
	public string enduranceMax;
	/// <summary>
	/// 起跑
	/// <summary>
	public string startSpeedMax;
	/// <summary>
	/// 速度
	/// <summary>
	public string speedMax;
	/// <summary>
	/// 智慧
	/// <summary>
	public string wisdomMax;
	/// <summary>
	/// 疲劳度
	/// <summary>
	public string fatigueMax;
	/// <summary>
	/// 状态
	/// <summary>
	public string status;
	/// <summary>
	/// 阶段
	/// <summary>
	public string stage;
	/// <summary>
	/// 是否租赁中
	/// <summary>
	public string isRent;
	/// <summary>
	/// 总投喂长度
	/// <summary>
	public string totalFeedProgress;
	/// <summary>
	/// 剩余投喂长度
	/// <summary>
	public string remainFeedProgress;
	/// <summary>
	/// 剩余投喂时间
	/// <summary>
	public string remainFeedTime;
	/// <summary>
	/// 总成长时间
	/// <summary>
	public string totalGrowUpProgress;
	/// <summary>
	/// 已成长时间
	/// <summary>
	public string alreadyGrowUpProgress;
	/// <summary>
	/// 剩余成年马粟
	/// <summary>
	public string remainGrowUpNumber;
	/// <summary>
	/// 剩余成长时间
	/// <summary>
	public int remainGrowUpTime;
	/// <summary>
	/// 参加场次
	/// <summary>
	public int matchNumebr;
	/// <summary>
	/// 胜率
	/// <summary>
	public string winRate;
	/// <summary>
	/// 头像
	/// <summary>
	public string pic;
	/// <summary>
	/// 租赁费
	/// <summary>
	public string price;
	/// <summary>
	/// 时间戳 -1代表空闲状态
	/// <summary>
	public string time;

	}
}