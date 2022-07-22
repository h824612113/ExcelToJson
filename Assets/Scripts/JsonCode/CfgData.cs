using System;
using LitJson;
using System.Collections.Generic;
namespace GameMain.Hotfix
{
//Ooooops:This cshap script is automatic make!Do not modify! ~mzc
	public class  Test_Excel
	{
		public int Id;
		public int FishId;
		public int FishType;
		public string FishName;
		public int FishValue;
		public int RefreshPro;
		public List<int> SceneId;
		public int FishGroup;
		public int GroupNum;
		public List<string> PathId;
		public float FishDelay;
		public double FishShape;
		public List<string> MinFishCounthahhaha;
		public Int64 MaxFishCount;
		public bool IsDynamic;
	}
	public class  Test_999
	{
		public int Id;
		public int FishId;
		public int FishType;
		public string FishName;
		public int FishValue;
		public int RefreshPro;
		public List<int> SceneId;
		public int FishGroup;
		public int GroupNum;
	}
	public class  Test_Excel_Copy
	{
		public int Id;
		public int FishId;
		public int FishType;
		public string FishName;
		public int FishValue;
		public int RefreshPro;
		public List<int> SceneId;
		public int FishGroup;
		public int GroupNum;
		public List<string> PathId;
		public float FishDelay;
		public double FishShape;
		public List<string> MinFishCount;
		public Int64 MaxFishCount;
		public bool IsDynamic;
	}
	public class  Test_999_Copy
	{
		public int Id;
		public int FishId;
		public int FishType;
		public string FishName;
		public int FishValue;
		public int RefreshPro;
		public List<int> SceneId;
		public int FishGroup;
		public int GroupNum;
	}
	public class CfgData : BaseManager<CfgData>
	{
		public List<Test_Excel> Test_Excels = new List<Test_Excel>();
		public List<Test_999> Test_999s = new List<Test_999>();
		public List<Test_Excel_Copy> Test_Excel_Copys = new List<Test_Excel_Copy>();
		public List<Test_999_Copy> Test_999_Copys = new List<Test_999_Copy>();

		public void InitCfg_v3(string json)
		{
			var data = JsonMapper.ToObject<TestCFG>(json);
			var items = data.Items;
			Test_Excels = items.Test_Excel;
			Test_999s = items.Test_999;
			Test_Excel_Copys = items.Test_Excel_Copy;
			Test_999_Copys = items.Test_999_Copy;
		}

		public class TestCFG {
			public TestItem Items;
		}
		public class TestItem {
			public List<Test_Excel> Test_Excel;
			public List<Test_999> Test_999;
			public List<Test_Excel_Copy> Test_Excel_Copy;
			public List<Test_999_Copy> Test_999_Copy;
		}
	}
}
