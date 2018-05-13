using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clues : MonoBehaviour {



    public TextAsset data;
    public Dictionary<string,Clue> clueData;
    public clueInfo cls;
  
    public void loadClue()
    {
        clueData = new Dictionary<string,Clue>();
        cls = JsonUtility.FromJson<clueInfo>(data.text);
        for(int i = 0; i < cls.allClues.Count;i++){
            Clue c = cls.allClues[i];
            string key = c.code;
            clueData.Add(key,c);
        }
    }

    public bool checkPrereq(Character chara, string code){
        List<string> prereq = clueData[code].prereq;
        int check = prereq.Count;
        foreach (string toCheck in prereq){
            if(!toCheck.Contains("+")){
                int stateCheck = chara.clueState[toCheck];
                if(stateCheck == 1){
                    check -= 1;
                }
            }
            else{
                string[] toCheckList = toCheck.Split('+');
                foreach(string checkClue in toCheckList){
                    int stateCheck = chara.clueState[checkClue];
                    if(stateCheck == 1){
                        check -= 1;
                        break;
                    }
                }
            }
        }
        if(check == 0){
            return true;
        }
        else{
            return false;
        }

    }

}

[System.Serializable]
public class clueInfo
{
    public List<Clue> allClues;
}

[System.Serializable]
public class Clue{
    public string code;
    public string content;
    public List<string> prereq;
    public List<string> initialOwner;
    public List<int> states;
    public int isOneTime;
    public List<string> scene;
}


