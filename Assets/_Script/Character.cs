using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour {
    public TextAsset data;
    private CharaInfo cInfo;
  

    void Start()
    {
        cInfo = JsonUtility.FromJson<CharaInfo>(data.text);
        print(cInfo.charaName);
        print(cInfo.Descrption);
    }



}

[System.Serializable]
public class CharaInfo
{
    public string charaType;
    public string charaName;
    public int Constitution;
    public int Intellegence;
    public int Charisma;
    public int Observation;
    public string Skill1;
    public string Skill2;
    public string Descrption;
}
