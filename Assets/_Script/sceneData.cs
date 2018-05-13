using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneData : MonoBehaviour {



    public TextAsset data;
    public Dictionary<string,gameScene> sceData;
    private sceInfo sce;
  
	// Use this for initialization
    public void loadScene()
    {
        sceData = new Dictionary<string,gameScene>();
        sce = JsonUtility.FromJson<sceInfo>(data.text);
        for(int i = 0; i < sce.sceneList.Count;i++){
            gameScene sc = sce.sceneList[i];
            string key = sc.sceneCode;
            sceData.Add(key,sc);
        }
    }
}

[System.Serializable]
public class sceInfo
{
    public List<gameScene> sceneList;
}

[System.Serializable]
public class gameScene{
    public string sceneCode;
    public string sceneLabel;
    public List<string> scripts;
}


