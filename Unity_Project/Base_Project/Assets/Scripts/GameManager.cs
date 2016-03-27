using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private GUIManager guiManager;
	//private GameObject maxMapCamera;
	private PauseGUI pauseGUI;
	private CameraController cameraController;

	//keys
	public KeyCode closeTab;
	public KeyCode mapKey;
	public KeyCode pauseKey;
	public KeyCode changeCameraView;
	public KeyCode showTasks;

	//set yes if scene use maxmap or minimap
	public bool useMaxMap;


	public AudioClip backgroundAudio;

	private bool isPause;

	// Use this for initialization
	void Start () {
	
		//isPause = false;
		UnPause();
		guiManager =  GameObject.FindWithTag (GameRepository.GetGUIManagerTag()).GetComponent<GUIManager>();
		pauseGUI = GameObject.FindWithTag (GameRepository.GetGUIManagerTag()).GetComponent<PauseGUI>();
		//maxMapCamera = GameObject.FindWithTag (GameRepository.GetMapCameraTag ());
		cameraController = GameObject.FindWithTag (GameRepository.GetMainCameraTag()).GetComponent<CameraController>();

		GetComponent<AudioSource> ().clip = backgroundAudio;
		AudioListener.volume = GameRepository.GetVolumeLevel() / 10.0F;
		PlayBackgroundSfx ();

		//Time.timeScale = 1;
		//Cursor.visible = false; 



	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(closeTab) ) {
			if(guiManager.GetMaxMapShow())
			{
				guiManager.SetMaxMapShow(false);
				//SetMaxMapCameraState(false);
				UnPause();
			}
			else if (isPause)
			{
				pauseGUI.SetShowPauseMenu (false);
				UnPause();
			}
			//else
			//{
			//	Application.Quit();
			//}
		}

		if ((Input.GetKeyDown(mapKey) ) && (useMaxMap)) {
			if ((!guiManager.GetMaxMapShow()) && !isPause)
			{
				guiManager.SetMaxMapShow(true);
				//SetMaxMapCameraState(true);
				Pause ();
				//Screen.showCursor = true;
			}
			else if (guiManager.GetMaxMapShow())
			{
				guiManager.SetMaxMapShow(false);
				//SetMaxMapCameraState(false);
				UnPause();
			}
		}

		if (Input.GetKeyDown(pauseKey) ) {
			if (guiManager.GetMaxMapShow() == false) {
				if (!isPause)
				{
					Pause();
					//Screen.showCursor = true;
					pauseGUI.SetShowPauseMenu (true);
				}
				else
				{
					UnPause();
					pauseGUI.SetShowPauseMenu (false);
				}
			}
		}

		if (!isPause) 
		{
			if (Input.GetKeyDown(changeCameraView) ) 
			{
				cameraController.ChangeCameraState();
			}
		}

		if (!isPause) {
			guiManager.SetShowTasks( Input.GetKey(showTasks));
		}


	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public void Pause()
	{
			Time.timeScale = 0 ;
			isPause = true ;
			Cursor.visible = true;
			Debug.Log ("pause");
	}

/*---------------------------------------------------------------------------------------------------------------*/

	public void UnPause()
	{
		Time.timeScale = 1 ;
		isPause = false ;
		Cursor.visible = false;
		Debug.Log ("unpause");
	}

/*---------------------------------------------------------------------------------------------------------------*/	

	public bool GetIsPause()
	{
		return isPause;
	}

/*---------------------------------------------------------------------------------------------------------------*/



/*---------------------------------------------------------------------------------------------------------------*/

	private void PlayBackgroundSfx()
	{
		GetComponent<AudioSource> ().Play ();
	}
}
