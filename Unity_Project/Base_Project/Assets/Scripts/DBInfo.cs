using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DBInfo : MonoBehaviour {
	private static readonly DBInfo instance = new DBInfo();
	
	//static names
	private string username = null;
	private string pass = null;
	private int id = -1;
	private int gameRounId ;
	private int currentSection ;
	private int currentQuestion ;
	private int lastSection;
	private List<SectionInfoStruct> sectionsInfo;
	private Vector3 playerFirstPositionForMainScene ;
	//private int currentLevel=1;
	
	// Explicit static constructor to tell C# compiler
	// not to mark type as beforefieldinit
	static DBInfo() 
	{
		
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/	
	
	private DBInfo () { }
	
	public static DBInfo Instance {
		get {
			return instance;
		}
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/	

	public static string GetUsername() {
		return instance.username;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public static string SetUsername(string un) {
		 instance.username = un;
		return "";
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public static string GetPassword() {
		return instance.pass;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static string SetPassword(string ps) {
		instance.pass = ps;
		return "";
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public static int GetID() {
		return instance.id;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static string SetID(int id) {
		instance.id = id;
		return "";
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public static int GetGameRoundId() {
		return instance.gameRounId;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public static string SetGameRoundId(int gameRoundId) {
		instance.gameRounId = gameRoundId;
		return "";
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	public static int GetCurrentSection() {
		return instance.currentSection;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static string SetCurrentSection( int currentSection) {
		instance.currentSection = currentSection;
		return "";
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static int GetLastSectionSerialNumber() {
		return instance.lastSection;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static string SetLastSectionSerialNumber( int lastSection) {
		instance.lastSection = lastSection;
		return "";
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static int GetCurrentQuestion() {
		return instance.currentQuestion;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static string SetCurrentQuestion( int currentQuestion) {
		instance.currentQuestion = currentQuestion;
		return "";
	}

	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static List<SectionInfoStruct> GetSectionsInfo() {
		return instance.sectionsInfo;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static string SetSectionsInfo( List<SectionInfoStruct> si) {
		instance.sectionsInfo = si;
		return "";
	}

	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static Vector3 GetPlayerFirstPositionForMainScene() {
		Debug.Log (instance.playerFirstPositionForMainScene);
		return instance.playerFirstPositionForMainScene;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public static string SetPlayerFirstPositionForMainScene( Vector3 pos) {
		instance.playerFirstPositionForMainScene = pos;
		Debug.Log (instance.playerFirstPositionForMainScene);
		return "";
	}
}
