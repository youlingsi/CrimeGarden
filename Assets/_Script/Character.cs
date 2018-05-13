using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour {
    public TextAsset data;
    public CharaInfo cInfo;
    public Place currentPlace;
    public Dictionary<string,int> clueState;

    public void loadCharacter()
    {
        clueState = new Dictionary<string,int>();
        cInfo = JsonUtility.FromJson<CharaInfo>(data.text);
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
    public string keyClue;
    public string Descrption;
}
