using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJson : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // CfgModel.GetInstance().initJson();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CfgModel.GetInstance().initJson();
        }
    }
}
