using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameMain.Hotfix;
using UnityEngine;

public class CfgModel : BaseManager<CfgModel>
{
    // Start is called before the first frame update
    void Start()
    {
        // var json = JsonMgr.Instance.LoadData<CfgData.TestCFG>("cfg_data");
        // initJson();
    }

    public void initJson()
    {
        string json = File.ReadAllText(Application.streamingAssetsPath+"/cfg_data.json");
        CfgData.GetInstance().InitCfg_v3(json);   
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
