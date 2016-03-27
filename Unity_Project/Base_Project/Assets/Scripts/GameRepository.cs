using UnityEngine;
using System.Collections;

public class GameRepository : MonoBehaviour {

	private static readonly GameRepository instance = new GameRepository();
	
	//static names
	private string gameManagerTag = "GameManager";
	private string GUIManagerTag = "GUIManager";
	private string teleportPointTag = "TeleportPoint";
	private string playerTag = "Player1";
	private string parentTeleportPointsTag = "ParentTeleportPoints";
	private string mapCameraTag = "MapCamera";
	private string mainCameraTag = "MainCamera";
	private string DBManagerTag = "DBManager";

	public float volumeLevel = 10.0f; 

	public bool finishTutorial = false;


	//private int currentLevel=1;
	
	// Explicit static constructor to tell C# compiler
	// not to mark type as beforefieldinit
	static GameRepository() 
	{
		
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	private GameRepository () { }
	
	public static GameRepository Instance {
		get {
			return instance;
		}
	}

/*---------------------------------------------------------------------------------------------------------------*/	
	
	// Use this for initialization
	void Start () 
	{
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	// Update is called once per frame
	void Update () {
		
	}

/*---------------------------------------------------------------------------------------------------------------*/	
	
	public static string GetGameManagerTag() {
		return instance.gameManagerTag;
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public static string GetGUIManagerTag() {
		return instance.GUIManagerTag;
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public static string GetTeleportPointTag() {
		return instance.teleportPointTag;
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public static string GetPlayerTag() {
		return instance.playerTag;
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public static string GetParentTeleportPointsTag() {
		return instance.parentTeleportPointsTag;
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public static string GetMapCameraTag() {
		return instance.mapCameraTag;
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public static string GetMainCameraTag() {
		return instance.mainCameraTag;
	}
	
/*---------------------------------------------------------------------------------------------------------------*/	
	
	public static string GetDBManagerTag() {
		return instance.DBManagerTag;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public static float GetVolumeLevel(){
		return instance.volumeLevel;
	}
		
	/*---------------------------------------------------------------------------------------------------------------*/

	public static float SetVolumeLevel(float vl){
		instance.volumeLevel = vl;
		return instance.volumeLevel;
	}

	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static bool GetFinishTutorial(){
		return instance.finishTutorial;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static bool SetFinishTutorial(bool value){
		instance.finishTutorial = value;
		return instance.finishTutorial;
	}
}
