using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GUIManager : MonoBehaviour {

	private GameObject player;
	//private CharacterController characterControllerScript;
	private MainChararacter_Controller characterControllerScript;
	private GameObject[] teleportPoint;
	private GameObject parentTeleportPoints;
	private PauseGUI pauseGUI; 
	private GameManager gameManager;
	private GameObject maxMapCamera;

	public RenderTexture miniMapTexture;
	public Material miniMapMaterial;

	public Texture playerPoint;
	public Vector2 playerPointIconSize;

	public Vector2 miniMapOffset;

	public Texture backgroundTexture;
	public RenderTexture maxMapTexture;

	public Vector2 maxMapSize; // % of screen range 0-1
	public Vector2 maxMapOffset;
	[HideInInspector]
	public bool maxMapShow;

	public Vector2 scrollViewOffset;

	public Material unSelectMatirial;
	public Material selectMatirial;
	
	public float buttonHeightSizeForMapScrollView = 0.05f;// % of screen range 0-1
	public Vector2 titleSizeForMap; // % of screen range 0-1
	public Vector2 titleForMapOffset;
	public float fontSizeButton = 0.01f; // % of screen range 0-1
	public float titlefontSize = 0.02f; // % of screen range 0-1

	//skins
	public GUISkin maxMapSkin;
	public GUISkin teleportButtonSkin;
	public GUISkin mainMenuSkin;
	public GUISkin tasksSkin;

	private Vector2 scrollPosition;

	//hold last button selected
	private int lastButtonSelect ;

	//set yes if scene use maxmap or minimap
	public bool useMaxMap;
	public bool useMiniMap;

	//for loading 
	public Texture2D emptyProgressBar; // for loading
	public Texture2D fullProgressBar; // for loading

	public Vector2 sizeLoadingBar;
	public Vector2 loadingBarOffset;

	private string title;
	public Vector2 titleSize; // % of screen range 0-1
	public Vector2 titleOffset;



	private AsyncOperation async = null;

	private bool loadLevel;

	private bool showTasks;

	public float tasksFontSize = 0.01f;

	//private bool showPauseMenu;

	// Use this for initialization
	void Start () {

		//showPauseMenu = false;
		if (useMaxMap) {
			teleportPoint = GameObject.FindGameObjectsWithTag (GameRepository.GetTeleportPointTag ()).OrderBy( go => go.name ).ToArray();
			parentTeleportPoints = GameObject.FindWithTag (GameRepository.GetParentTeleportPointsTag ());
			maxMapCamera = GameObject.FindWithTag (GameRepository.GetMapCameraTag ());
			SetMaxMapShow (false);

		}
		player = GameObject.FindWithTag (GameRepository.GetPlayerTag());
		//characterControllerScript = player.GetComponent<CharacterController> ();
		characterControllerScript = player.GetComponent<MainChararacter_Controller> ();
		pauseGUI = this.GetComponent<PauseGUI> ();
		gameManager = GameObject.FindWithTag (GameRepository.GetGameManagerTag()).GetComponent<GameManager>();

		loadLevel = false;
		showTasks = false;

	}

/*---------------------------------------------------------------------------------------------------------------*/	

	// Update is called once per frame
	void Update () 
	{


	}

/*---------------------------------------------------------------------------------------------------------------*/	
	
	void OnGUI ()
	{
		if (loadLevel == true) {
			LoadingProcess();
			return;
		}

		if ((maxMapShow == true) && (useMaxMap) )
		{
			DrawMaxMap ();
		} 
		else 
		{
			if (pauseGUI.GetShowPauseMenu() == false)
			{
				if(useMiniMap){
					DrawMinMap ();
				}

				if(showTasks){
					DrawShowTasks();
				}
			}
		}
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	private void DrawMinMap()
	{
		Rect miniMapTextureRect = new Rect (Screen.width - miniMapTexture.width - miniMapOffset.x, miniMapOffset.y, miniMapTexture.width, miniMapTexture.height);
		Graphics.DrawTexture (miniMapTextureRect, miniMapTexture, miniMapMaterial);  


		Vector2 playerPointSize;
		playerPointSize.x = miniMapTexture.width * playerPointIconSize.x;
		playerPointSize.y = miniMapTexture.height * playerPointIconSize.y;

		Vector2 playerPointPosition;
		playerPointPosition.x = Screen.width - miniMapTexture.width / 2.0f - miniMapOffset.x - playerPointSize.x / 2.0f;
		playerPointPosition.y = miniMapOffset.y + miniMapTexture.height / 2.0f - playerPointSize.y / 2.0f;

		Rect playerPointTextureRect = new Rect (playerPointPosition.x, playerPointPosition.y ,playerPointSize.x, playerPointSize.y );

		Matrix4x4 matrixBackup = GUI.matrix;
		Vector2 pivotPoint;
		pivotPoint.x = playerPointTextureRect.x + playerPointSize.x/2.0f;
		pivotPoint.y = playerPointTextureRect.y + playerPointSize.y/2.0f;
		GUIUtility.RotateAroundPivot(player.transform.eulerAngles.y, pivotPoint);
		//Debug.Log ((player.transform.eulerAngles.y));
		Graphics.DrawTexture (playerPointTextureRect, playerPoint, miniMapMaterial);  
		GUI.matrix = matrixBackup;
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	private void DrawMaxMap()
	{
		//set font size 
		maxMapSkin.button.fontSize = teleportButtonSkin.button.fontSize = Mathf.RoundToInt (Screen.width * fontSizeButton);

		GUI.skin = maxMapSkin;

		//draw background
		DrawBackground ();
	
		//draw title
		maxMapSkin.label.fontSize = Mathf.RoundToInt (Screen.width * titlefontSize);
		Vector2 titleSize = new Vector2 (Screen.width * titleSizeForMap.x, Screen.height * titleSizeForMap.y);
		Vector2 titlePosition = new Vector2 (((Screen.width/2)-(titleSize.x/2)) + titleForMapOffset.x, titleForMapOffset.y);
		Rect titleRect = new Rect(titlePosition,titleSize);
		GUI.Label (titleRect,"Map");

		//draw map
		Vector2 size;
		size.x = Screen.width * maxMapSize.x ;
		size.y = Screen.height * maxMapSize.y ;  
		Rect maxMapTextureRect = new Rect (maxMapOffset.x, Screen.height - size.y + maxMapOffset.y, size.x, size.y);
		Graphics.DrawTexture (maxMapTextureRect, maxMapTexture);  

		//draw buttons
		//scroll bar panel


		Rect positionScrollView = new Rect  (maxMapTextureRect.x + maxMapTextureRect.width + scrollViewOffset.x, maxMapTextureRect.y + scrollViewOffset.y,  Screen.width - maxMapTextureRect.x - maxMapTextureRect.width - scrollViewOffset.x , maxMapTextureRect.height - scrollViewOffset.y);
		maxMapSkin.button.fixedHeight = positionScrollView.height * buttonHeightSizeForMapScrollView;

		Rect viewRectScrollView = new Rect (0, 0, positionScrollView.width -16.0f, (maxMapSkin.button.fixedHeight + maxMapSkin.button.margin.top*2)*teleportPoint.Length);

		maxMapSkin.button.fixedWidth = positionScrollView.width;

		scrollPosition = GUI.BeginScrollView (positionScrollView, scrollPosition, viewRectScrollView);


		int i; 
		for (i=0; i<teleportPoint.Length; i++) 
		{
			GUI.enabled = true;

			if (lastButtonSelect == i)
			{
				GUI.enabled = false;
				teleportPoint[i].GetComponent<Renderer>().material = selectMatirial;
			}
			else
			{
				teleportPoint[i].GetComponent<Renderer>().material = unSelectMatirial;
			}

			if (GUILayout.Button(teleportPoint[i].name))
			{
				lastButtonSelect = i;
			}

		}

		GUI.EndScrollView ();

		GUI.enabled = true;

		//teleport button
		Vector2 positionTeleportButton;
		Vector2 sizeTeleportButton;

		sizeTeleportButton.x = maxMapSkin.button.fixedWidth / 2;
		sizeTeleportButton.y = maxMapSkin.button.fixedHeight;
		positionTeleportButton.x = Screen.width - sizeTeleportButton.x;
		positionTeleportButton.y = positionScrollView.y - sizeTeleportButton.y - 10.0f;

		GUI.skin = teleportButtonSkin;
		teleportButtonSkin.button.fixedWidth = sizeTeleportButton.x ;
		teleportButtonSkin.button.fixedHeight = sizeTeleportButton.y;

		if (DrawButton (positionTeleportButton,"Teleport",sizeTeleportButton)) 
		{
			characterControllerScript.teleport(teleportPoint[lastButtonSelect].transform.position);
			SetMaxMapShow(false);
			SetMaxMapCameraState(false);
			gameManager.UnPause();
		}

	}

/*---------------------------------------------------------------------------------------------------------------*/	

	private void DrawBackground()
	{
		//draw background
		Rect backgroundTextureRect = new Rect (0, 0, Screen.width, Screen.height);
		Graphics.DrawTexture (backgroundTextureRect, backgroundTexture); 
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	private bool DrawButton (Vector2 position, string name,Vector2 size)
	{
		Rect recForButton = new Rect (position.x,position.y, size.x, size.y);
		if(GUI.Button(recForButton,name))
		{
			return true;
		}
		return false ;
	}

/*---------------------------------------------------------------------------------------------------------------*/

	private void DrawShowTasks(){
		 
		GUI.skin = tasksSkin;

		tasksSkin.label.fontSize = Mathf.RoundToInt (Screen.width * tasksFontSize);

		List<SectionInfoStruct> si = DBInfo.GetSectionsInfo ();

		for (int i = 0; i < si.Count; i++) {

			int sn = si[i].serialNumber;
			int cn = DBInfo.GetCurrentSection();

			if (sn < cn){
				tasksSkin.label.normal.textColor = Color.green;
				tasksSkin.label.hover.textColor = Color.green;
				tasksSkin.label.fontStyle = FontStyle.Normal;
			}
			else if (sn > cn){
				tasksSkin.label.normal.textColor = Color.red;
				tasksSkin.label.hover.textColor = Color.red;
				tasksSkin.label.fontStyle = FontStyle.Normal;
			}
			else{
				tasksSkin.label.normal.textColor = Color.white;
				tasksSkin.label.hover.textColor = Color.white;
				tasksSkin.label.fontStyle = FontStyle.BoldAndItalic;
			}

			GUILayout.Label(si[i].serialNumber + ".  " + si[i].title);
		}
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public void SetMaxMapShow(bool value)
	{
		if (value == true) 
		{
			SetTeleportPointsState(true);
			SetMaxMapCameraState(true);
		} 
		else 
		{
			SetMaxMapCameraState(false);
			SetTeleportPointsState(false);
			lastButtonSelect = -1;
		}

		maxMapShow = value;
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public bool GetMaxMapShow()
	{
		return maxMapShow; 
	}

/*---------------------------------------------------------------------------------------------------------------*/	
	/*
	public void SetShowPauseMenu(bool value)
	{
		showPauseMenu = value;
		//if (showPauseMenu == true) 
		//{
			pauseGUI.SetShowPauseMenu (showPauseMenu);
		//} 
		//else
		//{
		//	pauseGUI.SetShowPauseMenu (false);
		//}
	}
	*/
/*---------------------------------------------------------------------------------------------------------------*/	
	/*
	public bool GetShowPauseMenu()
	{
		return showPauseMenu;
	}
	*/
/*---------------------------------------------------------------------------------------------------------------*/	

	public void SetMaxMapCameraState(bool value)
	{
		maxMapCamera.SetActive(value);
		
	}

/*---------------------------------------------------------------------------------------------------------------*/

	public void SetTeleportPointsState(bool value)
	{
		parentTeleportPoints.SetActive (value);
	}

/*---------------------------------------------------------------------------------------------------------------*/

	public void LoadingProcess(){
		title = "Loading...";

		GUI.skin = mainMenuSkin;
		mainMenuSkin.label.fontSize = Mathf.RoundToInt (Screen.width * titlefontSize);

		//draw title
		Vector2 caltitleSize = new Vector2 (Screen.width * titleSize.x, Screen.height * titleSize.y);
		Vector2 titlePosition = new Vector2 (((Screen.width / 2) - (caltitleSize.x / 2)) + titleOffset.x, titleOffset.y);
		Rect titleRect = new Rect (titlePosition, caltitleSize);
		GUI.Label (titleRect, title);

		Vector2 sizeLoadingTuxture;
		Vector2 positionLoadingTuxture;
		
		sizeLoadingTuxture.x = Screen.width * sizeLoadingBar.x ;
		sizeLoadingTuxture.y = Screen.height * sizeLoadingBar.y ;
		
		positionLoadingTuxture.x = (Screen.width / 2) - (sizeLoadingTuxture.x / 2);
		positionLoadingTuxture.y = (Screen.height / 2) - (sizeLoadingTuxture.y / 2);
		
		if (async != null) {
			GUI.DrawTexture(new Rect(positionLoadingTuxture.x, positionLoadingTuxture.y, sizeLoadingTuxture.x, sizeLoadingTuxture.y), emptyProgressBar);
			GUI.DrawTexture(new Rect(positionLoadingTuxture.x, positionLoadingTuxture.y, sizeLoadingTuxture.x * async.progress, sizeLoadingTuxture.y), fullProgressBar);
		}
	}

/*---------------------------------------------------------------------------------------------------------------*/

	public void LoadLavel(string name)
	{
		loadLevel = true;
		gameManager.Pause ();
		StartCoroutine (LoadScene (name));
	}


/*---------------------------------------------------------------------------------------------------------------*/
	
	public void SetShowTasks(bool value)
	{
		showTasks = value;
	}


/*---------------------------------------------------------------------------------------------------------------*/
	
	//function for load scene
	private IEnumerator LoadScene(string name)
	{
		async = Application.LoadLevelAsync(name);
		yield return async;
		
	}

}
