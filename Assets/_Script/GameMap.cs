﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour {
	public Clues allClue;
	public sceneData allScene;
	public Dictionary<string,Place> allRoom;
	public Character[] chara;
	public Place[] plcs;
	public int turn;
	public int states;//0-idle, 1-sceneplaying, 2-Aquiring clue, 3-accusing

	public int sceFrame;
	public string currentSce;
	public Character currentchara;
	public List<string> charaList;
	public GameObject frame;
	public msgBox msg;
	public GameObject selectionPanel;
	public GameObject notesPanel;
	public UnityEngine.UI.Text notesContent;
	public string murder;

	// Use this for initialization
	void Start () {
		turn = 0;
		charaList = new List<string>{
			"Assistant",
			"Butler",
			"Friend",
			"Maid",
			"Son",
			"Wife"
			};
		List<string> roomList = new List<string>{
			"rmAssistant", 
			"rmButler",
			"rmCourtyard",
			"rmDin",
			"rmFriend",
			"rmGarden",
			"rmLibrary",
			"rmLord",
			"rmMaid",
			"rmRestroom",
			"rmSon",
			"rmWife"
			};
		murder = "Assistant"; //randomly generate later.
		allRoom = new Dictionary<string, Place>();
		for(int i = 0; i < roomList.Count;i++){
			plcs[i].roomName = roomList[i];
			allRoom.Add(roomList[i],plcs[i]);
		}
		init();
		initScene("sc01");
	}
	
	// Update is called once per frame
	void Update () {
		if(states == 0){
			currentchara = chara[turn%6];
			frame.transform.position = currentchara.transform.position;	
		}
		else if (states == 1){
			showFrame(currentSce, sceFrame);
			if (Input.GetButtonUp("Jump")){
				sceFrame += 1;
			}
			if (sceFrame == allScene.sceData[currentSce].scripts.Count){
				disableMsg();
			}
		}
		else if (states == 2){
			if (Input.GetButtonUp("Jump")){
				disableMsg();
				turn += 1;
			}
		}

	}
	void init(){
		allClue.loadClue();
		allScene.loadScene();
		for(int i = 0; i < charaList.Count; i++){
			chara[i].loadCharacter();
			chara[i].currentPlace = allRoom["rmCourtyard"];
		}
		foreach(KeyValuePair<string,Clue> c in allClue.clueData){
			for(int i = 0; i < charaList.Count; i++){
				string k = c.Key;
				int state = c.Value.states[i];
				chara[i].clueState.Add(k,state);
			}
			List<string> initOwner = c.Value.initialOwner;
			foreach(string owner in initOwner){
				if (owner.StartsWith("rm")){
					allRoom[owner].clues.Add(c.Value.code);
				}
			}
		}
	}

	void showDialog(Character chara, string content){
		string suffix = "....(Press Space...)";
		UnityEngine.UI.Image img = chara.GetComponent<UnityEngine.UI.Image>();
		msg.gameObject.SetActive(true);
		msg.dialText.gameObject.SetActive(true);
		msg.narraText.gameObject.SetActive(false);
		msg.charaIcon.gameObject.SetActive(true);
		msg.charaIcon.sprite = img.sprite;
		msg.dialText.text = content + suffix;
	}

	void showNarritive(string content){
		string suffix = "....(Press Space...)";
		msg.gameObject.SetActive(true);
		msg.dialText.gameObject.SetActive(false);
		msg.charaIcon.gameObject.SetActive(false);
		msg.narraText.gameObject.SetActive(true);
		msg.narraText.text = content + suffix;
	}

	void disableMsg(){
		msg.charaIcon.gameObject.SetActive(false);
		msg.gameObject.SetActive(false);
		msg.narraText.gameObject.SetActive(false);
		states = 0;
		sceFrame = 0;
	}
	void showFrame(string sceName,int frame){
		string[] line = allScene.sceData[sceName].scripts[frame].Split('+');
		if (line[0] == "n"){
			showNarritive(line[1]);
		}
		else if (line[0]=="d"){
			Character c = chara[charaList.IndexOf(line[1])];
			showDialog(c,line[2]);
		}
	}

	void initScene(string scene){
		states = 1;
		sceFrame = 0;
		currentSce = scene;
	}
	public void enterRoom(Place room){
		if(states == 0){
			currentchara.currentPlace = room;
			currentchara.transform.position = Input.mousePosition;
		}
	}

	public void searchRoom(){
		string msgContent = "Nothing Special!";
		if(states == 0){
			List<string> pClue = currentchara.currentPlace.clues;
			List<string> pool = new List<string>();
			if (pClue.Count != 0){
				foreach(string clueCode in pClue){
					int state = currentchara.clueState[clueCode];
					if(state == 0){
						pool.Add(clueCode);
					}
					else if (state == -1){
						if (allClue.checkPrereq(currentchara,clueCode)){
							pool.Add(clueCode);
						}
					}
				}
				if (pool.Count != 0){
					int index = Random.Range(0,pool.Count-1);
					currentchara.clueState[pool[index]] = 1;
					msgContent = "Found clue: " + allClue.clueData[pool[index]].content;
					if (allClue.clueData[pool[index]].isOneTime == 1){
						currentchara.currentPlace.clues.Remove(pool[index]);
					}
				}
			}
		states = 2;
		showNarritive(msgContent);
		}


	}

	void movePlayer(Character chara){
		Vector3 pos = chara.transform.position;
		pos.x += 40;
		currentchara.transform.position = pos;
		currentchara.currentPlace = chara.currentPlace;
	}


	public void ask(Character chara){
		string msgContent = chara.cInfo.charaType + " does not seem know anything important";
		if (states == 0 && currentchara.cInfo.charaType != chara.cInfo.charaType){
			if(chara.currentPlace != currentchara.currentPlace){
				movePlayer(chara);
			}
			int dice = Random.Range(0, 100);

			if(dice <= 60){
				List<string> cClue = new List<string>();
				List<string> cPool = new List<string>();
				foreach(KeyValuePair<string,int> c in chara.clueState){
					if(c.Value == 1){
						cClue.Add(c.Key);
					}
				}
				foreach(string cCode in cClue){
					if(allClue.checkPrereq(currentchara,cCode)){
						if (currentchara.clueState[cCode] != 1){
							cPool.Add(cCode);
						}						
					}
				}
				if (cPool.Count != 0){
					int index = Random.Range(0,cPool.Count-1);
					currentchara.clueState[cPool[index]] = 1;
					msgContent = "Found clue: " + allClue.clueData[cPool[index]].content;
				}
			}
			else{
				msgContent = chara.cInfo.charaType + " refuse to tell you anything.";
			}
			states = 2;
			showNarritive(msgContent);
		}
		
	}

	public void showPanel(GameObject obj){
		if(states == 0){
			obj.SetActive(true);
			states = 3;
		}
	}

	public void closePanel(GameObject obj){
		obj.SetActive(false);
		states = 0;
	}
	public void accuse(Character chara){
		string kClue = chara.cInfo.keyClue;
		closePanel(selectionPanel);
		if(currentchara.cInfo.charaType != chara.cInfo.charaType){
			if(currentchara.clueState[kClue] == 1){
				if(chara.cInfo.charaType == murder){
					states = -1;
					showNarritive("You found the murder! \n\n Game Ended.");
				}
				else{
					states = -1;
					showNarritive(chara.cInfo.charaType + "was arrested, but the real murder is still free from punishment.\n\n Game End!");
				}
			}
			else{
				states = 2;
				showNarritive("The evidences are not enough.");
			}
		}
		else{
			if(chara.cInfo.charaType == murder){
				states = -1;
				showNarritive("You confessed your crime. \n\n Game Ended.");
			}
		}
	}

	public void clueNotes(){
		if(states == 0){
			showPanel(notesPanel);
			string notes = "";
			foreach(KeyValuePair<string, int> c in currentchara.clueState){
				if (c.Value == 1){
					string txt = "clue"+ c.Key + ": " + allClue.clueData[c.Key].content + "\n\n";
					notes += txt;
				}
			}
			notesContent.text = notes;
		}
	}

	public void characterInfo(){
		if(states == 0){
			showPanel(notesPanel);
			string info = currentchara.cInfo.Descrption;
			notesContent.text = info;
			
		}
	}

}
