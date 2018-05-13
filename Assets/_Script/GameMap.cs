using System.Collections;
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
		UnityEngine.UI.Image img = chara.GetComponent<UnityEngine.UI.Image>();
		msg.gameObject.SetActive(true);
		msg.dialText.gameObject.SetActive(true);
		msg.narraText.gameObject.SetActive(false);
		msg.charaIcon.gameObject.SetActive(true);
		msg.charaIcon.sprite = img.sprite;
		msg.dialText.text = content;
	}

	void showNarritive(string content){
		msg.gameObject.SetActive(true);
		msg.dialText.gameObject.SetActive(false);
		msg.charaIcon.gameObject.SetActive(false);
		msg.narraText.gameObject.SetActive(true);
		msg.narraText.text = content;
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
		string msg = "Nothing Special!";
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
					msg = "Found clue: " + allClue.clueData[pool[index]].content;
					if (allClue.clueData[pool[index]].isOneTime == 1){
						currentchara.currentPlace.clues.Remove(pool[index]);
					}
				}
			}
		states = 2;
		showNarritive(msg);
		}


	}

	void movePlayer(Character chara){
		Vector3 pos = chara.transform.position;
		pos.x += 40;
		currentchara.transform.position = pos;
		currentchara.currentPlace = chara.currentPlace;
	}


	public void ask(Character chara){
		string msg = chara.cInfo.charaType + " does not seem know anything important";
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
					msg = "Found clue: " + allClue.clueData[cPool[index]].content;
				}
			}
			else{
				msg = chara.cInfo.charaType + " refuse to tell you anything.";
			}
			states = 2;
			showNarritive(msg);
		}
		
	}

	public void showPanel(){
		selectionPanel.SetActive(true);
		states = 3;
	}

	public void closePanel(){
		selectionPanel.SetActive(false);
		states = 0;
	}
	public void accuse(Character chara){

	}

}
