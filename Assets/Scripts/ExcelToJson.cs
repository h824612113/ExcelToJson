using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using OfficeOpenXml;
using LitJson;
using UnityEditor;

public class ExcelToJson : MonoBehaviour
{
    //所有的excel名字
    public static List<string> m_AllExcelName = new List<string>();

    //所有的表的类型和属性名
    public static Dictionary<string, Dictionary<string, string>> m_AllPropertyDic =
        new Dictionary<string, Dictionary<string, string>>();

    public static JsonData m_FinalJsonData = new JsonData();

    public static string filePath = "";

    public static string codePath = Application.dataPath+ "/Scripts/JsonCode";//生成对应结构的的地址
    public static JsonData m_data = new JsonData(); //中间转换的类型

    public static bool isClientMode = true;//只生成_c的配置

    [MenuItem("Tools/ExcelToJson")]
    public static void ExcelToJsonFun()
    {
        filePath = Application.dataPath+ "/../Excel/Test"; //excel的地址
        //生成Json
        GenerateJson();
        //生成类结构
        GenerateDataCode();
        AssetDatabase.Refresh();
        string json = JsonMapper.ToJson(m_FinalJsonData);
        json = Regex.Unescape(json);
        print("json-----"+json);
    }

    public static void GenerateDataCode()
    {
        string destJson = Application.streamingAssetsPath+"/cfg_data.json";
        if (!Directory.Exists(codePath))
            Directory.CreateDirectory(codePath);
        string destcs = codePath + "/CfgData.cs";
        //分析json并写入cs脚本文件
        //StreamWriter类，以流的方式写入
        StreamWriter file = new StreamWriter(destcs, false);
        file.WriteLine("using System;");
        file.WriteLine("using LitJson;");
        file.WriteLine("using System.Collections.Generic;");
        file.WriteLine("namespace GameMain.Hotfix");
        file.WriteLine("{");
        file.WriteLine("//Ooooops:This cshap script is automatic make!Do not modify! ~mzc");
        
        string json = File.ReadAllText(destJson, System.Text.Encoding.UTF8);
        foreach (var item in m_AllPropertyDic)
        {
            file.Write("\tpublic class  "+item.Key+"\n\t{\n");
            foreach (var sheetItem in item.Value)
            {
                string type = getRealType(sheetItem.Value);
                file.Write("\t\tpublic "+type+" "+sheetItem.Key+";\n");
            }
            file.Write("\t}\n");
        }
        //所有结构的集合

        //cfgdata类
        
        //cfgdata初始化
        file.WriteLine("\tpublic class CfgData : BaseManager<CfgData>");
        file.WriteLine("\t{");
        foreach (var item in m_AllPropertyDic)
        {
            file.WriteLine(string.Format("\t\tpublic List<{0}> {0}s = new List<{0}>();", item.Key));
        }

        file.WriteLine("");
        file.WriteLine(string.Format("\t\tpublic void InitCfg_v3(string json)"));
        file.WriteLine("\t\t{");
        // file.WriteLine("\t\t\tvar data = UnityEngine.JsonUtility.FromJson<TestCFG>(json);");
        file.WriteLine("\t\t\tvar data = JsonMapper.ToObject<TestCFG>(json);");
        file.WriteLine("\t\t\tvar items = data.Items;");
        foreach (var item in m_AllPropertyDic)
        {
            file.WriteLine($"\t\t\t{item.Key}s = items.{item.Key};");
        }
        file.WriteLine("\t\t}");
        file.WriteLine("");
        // file.WriteLine("\t\t[System.Serializable]");
        file.WriteLine("\t\tpublic class TestCFG {");
        // file.WriteLine("\t\t\tpublic string Ver;");
        file.WriteLine("\t\t\tpublic TestItem Items;");
        file.WriteLine("\t\t}");
        file.WriteLine("\t\tpublic class TestItem {");
        foreach (var item in m_AllPropertyDic)
        {
            file.WriteLine($"\t\t\tpublic List<{item.Key}> {item.Key};");
        }
        file.WriteLine("\t\t}");
        file.WriteLine("\t}");
        file.WriteLine("}");
        file.Flush();
        file.Close();
        
    }
    public static void GenerateJson()
    {
        GetAllExcel(filePath);
        SolveAllSheets();
        m_data = new JsonData();
        JsonMgr.Instance.SaveData(m_FinalJsonData,"cfg_data");
    }

    public static string getRealType(string type)
    {
        string str = "";
        if (type == "int")
        {
            str = "int";
        }else if (type == "int64")
        {
            str = "Int64";
        }else if (type == "string")
        {
            str = "string";
        }else if (type == "bool")
        {
            str = "bool";
        }else if (type == "float")
        {
            str = "float";
        }else if (type.Contains("list"))
        {
            string subStr = type.Substring(4, type.Length-4);
            str = "List"+subStr;
        }else if (type == "double")
        {
            str = "double";
        }
        return str;
    }

    static bool CheckElementType(JsonData jsonData, string type)
    {
        switch (type)
        {
            case "bool":
                return jsonData.IsBoolean;
            case "string":
                return jsonData.IsString;
            case "int":
                return jsonData.IsInt;
            case "double":
                return jsonData.IsDouble;
            case "int64":
            case "long":
                return jsonData.IsLong;
        }

        return false;
    }
    
    public static JsonData ParseValue(string type,string textStr)
    {
        if (type.ToLower() == "int")
        {
            return int.Parse(textStr);
        }
        else if (type.ToLower() == "int64")
        {
            return Int64.Parse(textStr);
        }
        else if (type.ToLower() == "string")
        {
            return textStr;
        }
        else if (type.ToLower().Contains("list"))
        {
            var data = JsonMapper.ToObject(textStr);
            if (!data.IsArray)
                throw new Exception("not an array "+textStr);
            string elementType = type.Substring(type.IndexOf('<') + 1, type.IndexOf('>') - type.IndexOf('<')-1);
            foreach (var element in data)
            {
                if (!CheckElementType((JsonData) element, elementType))
                    throw new Exception("invalid element type in array field " + textStr);
            }
            return data;
        }
        else if (type.ToLower() == "dictionary")
        {
            var data = JsonMapper.ToObject(textStr);
            return data;
        }
        else if (type.ToLower() == "bool")
        {
            return int.Parse(textStr)>0?true:false;
        }
        else if (type.ToLower() == "float")
        {
            return float.Parse(textStr);
        }
        else if (type.ToLower() == "double")
        {
            return double.Parse(textStr);
        }
        return null;
    }
    //处理所有的excel表
    public static void SolveAllSheets()
    {
        m_FinalJsonData["Items"] = new JsonData();
        for (int i = 0; i < m_AllExcelName.Count; i++)
        {
            string excelPath = filePath+"/" + m_AllExcelName[i];
            FileInfo fileInfo = new FileInfo(excelPath);
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                int sheetCount = excelPackage.Workbook.Worksheets.Count;
                Dictionary<string, string> tempJson = new Dictionary<string, string>();//当前json的名字和类型
                // for (int j = 0; j < sheetCount; j++)
                // {
                //     ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[j];
                //     
                // }
                ExcelWorksheets worksheets = excelPackage.Workbook.Worksheets;
                // string allJsonData = "{/n";
                // JsonData allJsonData = new JsonData();
                // string rowData = "";
                foreach (var worksheet in worksheets)
                {
                    if (!worksheet.Name.Contains("!"))
                    {
                        continue;
                    }
                    else
                    {
                        // string[] sep = worksheet.Name.Split('!');
                        // worksheet.Name = sep[1];
                    }
                    int colCount = worksheet.Dimension.End.Column;
                    int rowCount = worksheet.Dimension.End.Row;
                    // Debug.Log(""+"-666-表名是---"+worksheet.Name+"--在--"+excelPath+"--下面row==="+rowCount+"---col---"+colCount);
                    if (rowCount < 4)
                    {
                        Debug.Log(""+"--表名是---"+worksheet.Name+"--在--"+excelPath+"--下面的行少于四行");
                    }
                    
                    JsonData rowJson = new JsonData();
                    string rowData = "";
                    rowJson.SetJsonType(LitJson.JsonType.Array);
                    List<JsonData> perSheetData = new List<JsonData>();
                    perSheetData.Clear();
                    Dictionary<string, string> tempDic = new Dictionary<string, string>();
                    tempDic.Clear();
                    for (int row = 4; row <= rowCount; row++) //行
                    {
                        rowData = "{";
                        JsonData perRowData = new JsonData();
                        for (int col = 1; col <= colCount; col++) //列
                        {
                            // ----------------------------------------------
                            string key = worksheet.Cells[1, col].Text;//json的键值
                            string type = worksheet.Cells[2, col].Text;//json的类型
                            string text = worksheet.Cells[row, col].Text;

                            //吧一个个数据存到对应的结构中

                            if (text == string.Empty)
                            {
                                Debug.Log(""+"-666-表名是-空空空--"+worksheet.Name+"--在--"+excelPath+"--下面row==="+row+"---col---"+col);
                            }
                            else
                            {
                                if (text.Contains("|"))
                                {
                                    text = text.Replace('|', ',');
                                    text = "[" + text + "]";
                                    Debug.Log("-----|||||-----text--------------"+text);
                                }
                                bool isClientPro = false;
                                if (col != 1 && type.ToLower().Contains("_c"))
                                {
                                    //客户端专用的字段
                                    isClientPro = true;
                                    type = type.Substring(0, type.Length - 2);
                                }else if (col == 1)
                                {
                                    isClientPro = true;
                                }
                                if (col < colCount)
                                {
                                    if (isClientMode)
                                    {
                                        if (isClientPro)
                                        {
                                            perRowData[key] =  ParseValue(type, text);   
                                        }
                                    }
                                    else
                                    {
                                        perRowData[key] =  ParseValue(type, text);    
                                    }
                                    
                                    Debug.Log("当前的type是"+type+"--内容是"+text);
                                }
                                else
                                {
                                    Debug.Log("当前的type是"+type+"--内容是"+text);
                                    if (isClientMode)
                                    {
                                        if (isClientPro)
                                        {
                                            perRowData[key] =  ParseValue(type, text);   
                                        }
                                    }
                                    else
                                    {
                                        perRowData[key] =  ParseValue(type, text);   
                                    }
                                    Debug.Log("rowData-------------------sss"+rowData);
                                    perSheetData.Add(perRowData);
                                }
                            
                                
                                tempDic[key] = type.ToLower();
                            }
                        }
                    }

                    Debug.Log("当前的rowJson----"+rowJson);
                    if (worksheet.Name.Contains("!"))
                    {
                        string[] sep = worksheet.Name.Split('!');
                        worksheet.Name = sep[1];
                        m_FinalJsonData["Items"][worksheet.Name] = new JsonData();
                        // m_FinalJsonData[worksheet.Name].SetJsonType(LitJson.JsonType.Array);
                        for (int j = 0; j < perSheetData.Count; j++)
                        {
                            // allJsonData[worksheet.Name].Add(perSheetData[j]);
                            m_FinalJsonData["Items"][worksheet.Name].Add(perSheetData[j]);
                            Debug.Log("persheetdata----"+j+"-------------"+perSheetData[j]);
                        }
                        
                        m_AllPropertyDic[worksheet.Name] = tempDic;
                        
                    }
                    else
                    {
                        Debug.Log("当前表命名不规范,没有 ! "+"--表名是---"+worksheet.Name+"--在--"+excelPath+"--下面");
                    }
                }
            }
        }
    }
    public static List<string> GetAllExcel(string path)
    {
        m_AllExcelName.Clear();
        if (System.IO.Directory.Exists(path))
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] files = directory.GetFiles("*.xlsx", SearchOption.AllDirectories);
            Debug.Log("excel文件的个数为----"+files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log("当前的excel的名字是-----------"+files[i].Name);
                m_AllExcelName.Add(files[i].Name);
            }

            return m_AllExcelName;
        }
        else
        {
            Debug.LogError(path+"--当前文件夹下为空");
            return null;
        }
    }
    
    //1.读取excel
    //2.生成excel的json;
    //3.生成excel的结构类;
    //4.初始化 赋值 
}
