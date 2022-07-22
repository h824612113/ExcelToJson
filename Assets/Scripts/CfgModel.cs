using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameMain.Hotfix;
using UnityEngine;

public class CfgModel : BaseManager<CfgModel>
{
    public void initJson()
    {
        string json = File.ReadAllText(Application.streamingAssetsPath+"/cfg_data.json");
        CfgData.GetInstance().InitCfg_v3(json);   
    }
    
}
