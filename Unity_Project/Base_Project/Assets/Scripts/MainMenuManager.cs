using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public Vector2 titleSizeForMainMenu; // % of screen range 0-1
	public Vector2 titleForMainMenuOffset;
	public Vector2 sizeButtonMainMenu;

	public Vector2 sizeLoadingBar;
	public Vector2 loadingBarOffset;

	//public Vector2 sizeBoxSettings; //for settings button
	//public Vector2 sizeButtonSettings; //for settings button

	public float fontSize; //0.02
	public float titlefontSize ; //0.04
	
	private int numOfButtons = 3;
	private enum MainMenuState
	{
		MainMenu,
		NewGame,
		LoadGame,
		//Controls, //for Controls button
		//Settings, //for settings button
		Exit,
		LoadScene
	}
	
	private MainMenuState mainMenuState;
	private string title;
	
	public GUISkin mainMenuSkin;
	//public GUISkin settingsSkin; //for settings button
	
	public Texture backgroundTexture;

	//public float volumeLevel = 10.0F; //prepi na gini ena geniko volume gia ola //for settings button

	public Texture2D emptyProgressBar; // for loading
	public Texture2D fullProgressBar; // for loading

	public AudioClip buttonClickAudio;

	private AsyncOperation async = null;
	
	// Use this for initialization
	void Start () {
	
		mainMenuState = MainMenuState.MainMenu;
		title = "Main Menu";

		Time.timeScale = 1;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI ()
	{
		//draw background
		DrawBackground ();
			
		//load-set skin
		GUI.skin = mainMenuSkin;
		mainMenuSkin.button.fontSize = Mathf.RoundToInt (Screen.width * fontSize);
		mainMenuSkin.label.fontSize = Mathf.RoundToInt (Screen.width * titlefontSize);
		mainMenuSkin.button.fixedWidth = Screen.width * sizeButtonMainMenu.x;
		mainMenuSkin.button.fixedHeight = Screen.height * sizeButtonMainMenu.y;
			
		//draw title
		Vector2 titleSize = new Vector2 (Screen.width * titleSizeForMainMenu.x, Screen.height * titleSizeForMainMenu.y);
		Vector2 titlePosition = new Vector2 (((Screen.width / 2) - (titleSize.x / 2)) + titleForMainMenuOffset.x, titleForMainMenuOffset.y);
		Rect titleRect = new Rect (titlePosition, titleSize);
		GUI.Label (titleRect, title);
			
		if (mainMenuState == MainMenuState.MainMenu) {
			title = "Main Menu";
				
			Vector2 groupButtonSize = new Vector2 ((mainMenuSkin.button.fixedWidth + (mainMenuSkin.button.margin.right * 2)), (mainMenuSkin.button.fixedHeight + (mainMenuSkin.button.margin.top * 2)) * numOfButtons);
			Vector2 groupButtonPosition = new Vector2 (((Screen.width / 2) - (groupButtonSize.x / 2)), ((Screen.height / 2) - (groupButtonSize.y / 2)));
			Rect groupButtonRect = new Rect (groupButtonPosition, groupButtonSize);
				
			GUI.BeginGroup (groupButtonRect);
			if (GUILayout.Button ("New Game")) {
				mainMenuState = MainMenuState.NewGame;
				PlayButtonSfx ();
			}
				
			if (GUILayout.Button ("Load Game")) {
				mainMenuState = MainMenuState.LoadGame;
				PlayButtonSfx ();
			}

			//for controls and settings button
			/*	
			if (GUILayout.Button ("Controls")) {
				mainMenuState = MainMenuState.Controls;
				PlayButtonSfx ();
			}
				
			if (GUILayout.Button ("Settings")) {
				mainMenuState = MainMenuState.Settings;
				PlayButtonSfx ();
			}
			*/

			if (GUILayout.Button ("Exit")) {
				mainMenuState = MainMenuState.Exit;
				PlayButtonSfx ();
			}
				
			GUI.EndGroup ();
		} 
		else if (mainMenuState == MainMenuState.NewGame) 
		{

			mainMenuState = MainMenuState.LoadScene;
			StartCoroutine (LoadScene ("main_scene"));

		} 
		else if (mainMenuState == MainMenuState.LoadGame) 
		{
			//go to main menu scene
			title = "Load Game";

			if (GUI.Button (new Rect (0, 0, 10, 10), "Back")) {
				mainMenuState = MainMenuState.MainMenu;
				PlayButtonSfx ();
			}
		}
		/*
		else if (mainMenuState == MainMenuState.Controls)
		{
			title = "Controls";
			if (GUI.Button(new Rect(0,0,10,10), "Back"))
			{
				mainMenuState = MainMenuState.MainMenu;
				PlayButtonSfx();
			}
			
		}
		else if (mainMenuState == MainMenuState.Settings)
		{
			GUI.skin = settingsSkin;
			
			title = "Settings";
			
			Vector2 sizeButton;
			sizeButton.x = Screen.width * sizeButtonSettings.x;
			sizeButton.y = Screen.height * sizeButtonSettings.y;
			
			Vector2 sizeBox;
			sizeBox.x = Screen.width * sizeBoxSettings.x;
			sizeBox.y = Screen.height * sizeBoxSettings.y;
			
			//settingsSkin.button.fixedWidth = sizeButton.x;
			
			Vector2 groupButtonSize = new Vector2 ( sizeBox.x + sizeButton.x*4, sizeBox.y*9 );
			Vector2 groupButtonPosition = new Vector2 (((Screen.width/2) - (groupButtonSize.x/2)), ((((Screen.height - titleSize.y - titleForMainMenuOffset.y)/2)+titleSize.y + titleForMainMenuOffset.y) - (groupButtonSize.y/2)));
			Rect groupButtonRect = new Rect(groupButtonPosition,groupButtonSize);
			
			GUI.BeginGroup (groupButtonRect);
			
			GUI.Box(new Rect(0, 0, sizeBox.x, sizeBox.y), "Quality Level: " + QualitySettings.GetQualityLevel());
			if (GUI.Button(new Rect(sizeBox.x,0,sizeButton.x,sizeButton.y), "<"))
			{
				QualitySettings.DecreaseLevel(true);
				PlayButtonSfx();
			}
			if (GUI.Button(new Rect(sizeBox.x + sizeButton.x ,0,sizeButton.x,sizeButton.y), ">"))
			{
				QualitySettings.IncreaseLevel(true);
				PlayButtonSfx();
			}
			
			GUI.Box(new Rect(0,sizeBox.y,sizeBox.x,sizeBox.y), "Anti-Aliasing Level: " + QualitySettings.antiAliasing);
			if (GUI.Button(new Rect(sizeBox.x,sizeBox.y,sizeButton.x,sizeButton.y), "X0"))
			{
				QualitySettings.antiAliasing = 0;
				PlayButtonSfx();
			}
			if (GUI.Button(new Rect(sizeBox.x + sizeButton.x,sizeBox.y,sizeButton.x,sizeButton.y), "X2"))
			{
				QualitySettings.antiAliasing = 2;
				PlayButtonSfx();
			}
			if (GUI.Button(new Rect(sizeBox.x + sizeButton.x*2,sizeBox.y,sizeButton.x,sizeButton.y), "X4"))
			{
				QualitySettings.antiAliasing = 4;
				PlayButtonSfx();
			}
			if (GUI.Button(new Rect(sizeBox.x + sizeButton.x*3,sizeBox.y,sizeButton.x,sizeButton.y), "X8"))
			{
				QualitySettings.antiAliasing = 8;
				PlayButtonSfx();
			}
			
			GUI.Box(new Rect(0,sizeBox.y * 2,sizeBox.x,sizeBox.y), "Anisotropic Level: " + QualitySettings.anisotropicFiltering);
			if (GUI.Button(new Rect(sizeBox.x ,sizeBox.y * 2,sizeButton.x,sizeButton.y), "Disable"))
			{
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
				PlayButtonSfx();
			}
			if (GUI.Button(new Rect(sizeBox.x + sizeButton.x,sizeBox.y * 2,sizeButton.x,sizeButton.y), "Enable"))
			{
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
				PlayButtonSfx();
			}
			if (GUI.Button(new Rect(sizeBox.x + sizeButton.x*2,sizeBox.y * 2,sizeButton.x,sizeButton.y), "ForceEnable"))
			{
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
				PlayButtonSfx();
			}
			
			GUI.Box(new Rect(0,sizeBox.y * 3,sizeBox.x,sizeBox.y), "Vol: " + Mathf.Round(volumeLevel));
			AudioListener.volume = volumeLevel / 10.0F;
			volumeLevel = GUI.HorizontalSlider(new Rect(sizeBox.x,sizeBox.y * 3,sizeButton.x * 4,sizeButton.y), volumeLevel, 0.0F, 10.0F);
			
			GUI.Box(new Rect(0,sizeBox.y * 4,sizeButton.x * 4 + sizeBox.x,sizeBox.y * 4),
			        "Machine Information: \n GPU: " + SystemInfo.graphicsDeviceName +
			        "\n GPU Memory: " + SystemInfo.graphicsMemorySize + "MB" +
			        "\n CPU: " + SystemInfo.processorType +
			        "\n RAM: " + SystemInfo.systemMemorySize + "MB");
			
			if (GUI.Button(new Rect(0,sizeBox.y * 8,sizeButton.x *4 + sizeBox.x,sizeButton.y), "Back"))
			{
				mainMenuState = MainMenuState.MainMenu;
				PlayButtonSfx();
			}
			
			GUI.EndGroup();
			
		}
		*/
		else if (mainMenuState == MainMenuState.Exit) 
		{
			Application.Quit ();
		} 
		else if (mainMenuState == MainMenuState.LoadScene) 
		{
			title = "Loading...";

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


	}

/*---------------------------------------------------------------------------------------------------------------*/

	private void DrawBackground()
	{
		//draw background
		Rect backgroundTextureRect = new Rect (0, 0, Screen.width, Screen.height);
		Graphics.DrawTexture (backgroundTextureRect, backgroundTexture); 
	}

/*---------------------------------------------------------------------------------------------------------------*/

	private void PlayButtonSfx()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonClickAudio);
	}

/*---------------------------------------------------------------------------------------------------------------*/

	//function for load scene
	private IEnumerator LoadScene(string name)
	{
		async = Application.LoadLevelAsync(name);
		yield return async;

	}
}
