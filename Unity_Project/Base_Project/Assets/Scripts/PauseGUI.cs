using UnityEngine;
using System.Collections;

public class PauseGUI : MonoBehaviour {

	//private GUIManager guiManager;
	private GameManager gameManager;

	public Vector2 titleSizeForPause; // % of screen range 0-1
	public Vector2 titleForPauseOffset;
	public Vector2 sizeButtonPause;

	public Vector2 sizeBoxSettings;
	public Vector2 sizeButtonSettings;

	public float fontSize ; //0.02
	public float titlefontSize ; //0.04

	private int numOfButtons = 6;
	private enum PauseMenuState
	{
		Pause,
		ResumeGame,
		SaveGame,
		MainMenu,
		Controls,
		Settings,
		Exit
	}

	private PauseMenuState pauseMenuState;
	private string title;

	public GUISkin pauseSkin;
	public GUISkin settingsSkin;

	public Texture backgroundTexture;

	public float volumeLevel ; //prepi na gini ena geniko volume gia ola

	public AudioClip buttonClickAudio;

	private bool showPauseMenu ;


	// Use this for initialization
	void Start () {
	
		showPauseMenu = false;
		pauseMenuState = PauseMenuState.Pause;
		title = "Pause";

		//guiManager =  GameObject.FindWithTag (GameRepository.GetGUIManagerTag()).GetComponent<GUIManager>();
		gameManager = GameObject.FindWithTag (GameRepository.GetGameManagerTag()).GetComponent<GameManager>();
		volumeLevel = GameRepository.GetVolumeLevel ();
	}

	void Update(){
		/*if (showPauseMenu == true) {
			Cursor.visible = true;
		}
		else {
			Cursor.visible = false; 
		}*/
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	void OnGUI ()
	{
		if (showPauseMenu) 
		{
			//draw background
			DrawBackground();

			//load-set skin
			GUI.skin = pauseSkin;
			pauseSkin.button.fontSize = settingsSkin.button.fontSize = settingsSkin.box.fontSize  = Mathf.RoundToInt (Screen.width * fontSize);
			pauseSkin.label.fontSize = Mathf.RoundToInt (Screen.width * titlefontSize);
			pauseSkin.button.fixedWidth = Screen.width * sizeButtonPause.x;
			pauseSkin.button.fixedHeight = Screen.height * sizeButtonPause.y;

			//draw title
			Vector2 titleSize = new Vector2 (Screen.width * titleSizeForPause.x, Screen.height * titleSizeForPause.y);
			Vector2 titlePosition = new Vector2 (((Screen.width/2)-(titleSize.x/2)) + titleForPauseOffset.x, titleForPauseOffset.y);
			Rect titleRect = new Rect(titlePosition,titleSize);
			GUI.Label (titleRect,title);

			if (pauseMenuState == PauseMenuState.Pause)
			{
				title = "Pause";

				Vector2 groupButtonSize = new Vector2 ( (pauseSkin.button.fixedWidth + (pauseSkin.button.margin.right*2)), (pauseSkin.button.fixedHeight + (pauseSkin.button.margin.top*2)) * numOfButtons );
				Vector2 groupButtonPosition = new Vector2 (((Screen.width/2) - (groupButtonSize.x/2)), ((Screen.height/2) - (groupButtonSize.y/2)));
				Rect groupButtonRect = new Rect(groupButtonPosition,groupButtonSize);

				GUI.BeginGroup (groupButtonRect);
					if (GUILayout.Button("Resume Game"))
					{
						pauseMenuState = PauseMenuState.ResumeGame;
						PlayButtonSfx();
					}
					
					if (GUILayout.Button("Save Game"))
					{
						pauseMenuState = PauseMenuState.SaveGame;
						PlayButtonSfx();
					}
					
					if (GUILayout.Button("Main Menu"))
					{
						pauseMenuState = PauseMenuState.MainMenu;
						PlayButtonSfx();
					}
					
					if (GUILayout.Button("Controls"))
					{
						pauseMenuState = PauseMenuState.Controls;
						PlayButtonSfx();
					}
					
					if (GUILayout.Button("Settings"))
					{
						pauseMenuState = PauseMenuState.Settings;
						PlayButtonSfx();
					}
					
					if (GUILayout.Button("Exit"))
					{
						pauseMenuState = PauseMenuState.Exit;
						PlayButtonSfx();
					}

				GUI.EndGroup();
			}
			else if (pauseMenuState == PauseMenuState.ResumeGame)
			{
				//close pause menu
				pauseMenuState = PauseMenuState.Pause;
				this.SetShowPauseMenu(false);
				gameManager.UnPause();

			}
			else if (pauseMenuState == PauseMenuState.SaveGame)
			{

				//save game
				GUI.skin = settingsSkin;
				if (GUI.Button(new Rect(0,0,Screen.width * sizeButtonSettings.x,Screen.height * sizeButtonSettings.y), "Back"))
				{
					pauseMenuState = PauseMenuState.Pause;
					PlayButtonSfx();
				}
			}
			else if (pauseMenuState == PauseMenuState.MainMenu)
			{
				//go to main menu scene
				Application.LoadLevel("menu(main)_scene");

			}
			else if (pauseMenuState == PauseMenuState.Controls)
			{
				GUI.skin = settingsSkin;
				title = "Controls";
				GUI.Box(new Rect(0,titleSize.y,Screen.width ,Screen.height-titleSize.y),
				        "WASD for navigate character" + 
				        "\n Mouse Movement for change character view point" + 
				        "\n Shift for character sprint" + 
				        "\n Space for character jumb" +
				        "\n Mouse Right hold for zoom" +
				        "\n Mouse scrollwheel for ZoomIn and ZoomOut" +
				        "\n V for change camera status (first/third person camera" +
				        "\n P for pause game" +
				        "\n M for show teleport map" +
				        "\n Esc for exit menu/game" 
				        );
				if (GUI.Button(new Rect(0,0,Screen.width * sizeButtonSettings.x,Screen.height * sizeButtonSettings.y), "Back"))
				{
					pauseMenuState = PauseMenuState.Pause;
					PlayButtonSfx();
				}

			}
			else if (pauseMenuState == PauseMenuState.Settings)
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
				Vector2 groupButtonPosition = new Vector2 (((Screen.width/2) - (groupButtonSize.x/2)), ((((Screen.height - titleSize.y - titleForPauseOffset.y)/2)+titleSize.y + titleForPauseOffset.y) - (groupButtonSize.y/2)));
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
					GameRepository.SetVolumeLevel(volumeLevel);
					volumeLevel = GUI.HorizontalSlider(new Rect(sizeBox.x,sizeBox.y * 3,sizeButton.x * 4,sizeButton.y), volumeLevel, 0.0F, 10.0F);

					GUI.Box(new Rect(0,sizeBox.y * 4,sizeButton.x * 4 + sizeBox.x,sizeBox.y * 4),
					        "Machine Information: \n GPU: " + SystemInfo.graphicsDeviceName +
					        "\n GPU Memory: " + SystemInfo.graphicsMemorySize + "MB" +
					        "\n CPU: " + SystemInfo.processorType +
					        "\n RAM: " + SystemInfo.systemMemorySize + "MB");

					if (GUI.Button(new Rect(0,sizeBox.y * 8,sizeButton.x *4 + sizeBox.x,sizeButton.y), "Back"))
					{
						pauseMenuState = PauseMenuState.Pause;
						PlayButtonSfx();
					}

				GUI.EndGroup();

			}
			else if (pauseMenuState == PauseMenuState.Exit)
			{
				Application.Quit();
			}
		}
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public void SetShowPauseMenu(bool value)
	{
		if (value == false) 
		{
			pauseMenuState = PauseMenuState.Pause;
		}
		showPauseMenu = value; 
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public bool GetShowPauseMenu()
	{
		return showPauseMenu;
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
}
