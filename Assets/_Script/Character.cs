using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour {
    public TextAsset data;
    //Json data file
    
    public CharaInfo cInfo;
    //converted data
    
    public Place currentPlace;
    //the room where the character currently stays
    
    public Dictionary<string,int> clueState;
    //state of the clues relating to the character
    //key is the string indicte the type of the character
    //Values:
    ////-2-not available to the character
    ////-1-locked, requiring prerequsuit clues
    ////0--not achieved by the character
    ////1--achieved by the character

    public void loadCharacter()
    // initiate the character instanse
    {
        clueState = new Dictionary<string,int>();
        cInfo = JsonUtility.FromJson<CharaInfo>(data.text);
    }

}

[System.Serializable]
public class CharaInfo
{
    public string charaType;//the type of the character(Assistant, Butler, Friend, Maid, Son or Wife)
    public string charaName;//The name of the character(not currently used)
    public int Constitution;//properties(not currently used)
    public int Intellegence;//properties(not currently used)
    public int Charisma;//properties(not currently used)
    public int Observation;//properties(not currently used)
    public string Skill1;//skill name(not currently used)
    public string Skill2;//skill name(not currently used)
    public string keyClue;//the key clue required to accuse the character
    public string Descrption;//the bacground story of the character
}
