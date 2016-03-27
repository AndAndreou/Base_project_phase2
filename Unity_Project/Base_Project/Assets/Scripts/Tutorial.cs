using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour {

	private GameManager gameManager;
	private CameraController cameraController;
	//private CharacterController characterController;
	private MainChararacter_Controller characterController;
	private GameObject targetGameObject;


	public Texture backgroundTexture;

	public Texture[] textures;
	public string[] texts;

	public Vector2 tutorialSize; // % of screen range 0-1
	public Vector2 turorialOffset;

	public Vector2 textureSize; // % of screen range 0-1
	public Vector2 labelSize; // % of screen range 0-1

	public float fontSize; // % of screen range 0-1

	public GUISkin tutorialSkin;

	public KeyCode nextTutorial;
	public KeyCode prevTutorial;

	private int count;
	//private bool finishTutorial;

	private bool temp;
	// Use this for initialization
	void Start () {
	
		gameManager = GameObject.FindWithTag (GameRepository.GetGameManagerTag()).GetComponent<GameManager>();
		targetGameObject = GameObject.FindWithTag (GameRepository.GetPlayerTag ());
		//characterController = targetGameObject.GetComponent<CharacterController>();
		characterController = targetGameObject.GetComponent<MainChararacter_Controller>();
		cameraController = GameObject.FindWithTag (GameRepository.GetMainCameraTag()).GetComponent<CameraController>();



		temp = false;
		//finishTutorial = false;
		count = 0 ;

	}
	
	// Update is called once per frame
	void Update () {
	
		if (GameRepository.GetFinishTutorial() == false) {
			if (temp == false) {
				temp = true;
				gameManager.Pause ();
				characterController.SetDontRunUpdate(true);
				cameraController.SetDontRunUpdate(true);
			}

			if (Input.GetKeyDown (nextTutorial)) {
				count ++;
				if (count >= textures.Length) {
					GameRepository.SetFinishTutorial(true);
					gameManager.UnPause ();
					characterController.SetDontRunUpdate(false);
					cameraController.SetDontRunUpdate(false);

				}
			}

			if (Input.GetKeyDown (prevTutorial)) {
				count --;
				if (count < 0)
					count = 0;
			}
		}

	}

	void OnGUI (){
	
		if (GameRepository.GetFinishTutorial() == false) {
			if (count < textures.Length) { 

				DrawBackground();

				GUI.skin = tutorialSkin;
				tutorialSkin.label.fontSize = Mathf.RoundToInt (Screen.width * fontSize);
				//tutorialSkin.label.fontSize = Mathf.RoundToInt (Screen.width * labelSize );


				Vector2 groupTutorialSize;
				groupTutorialSize.x = Screen.width * tutorialSize.x;
				groupTutorialSize.y = Screen.height * tutorialSize.y;

				Vector2 groupTutorialPos;
				groupTutorialPos.x = (Screen.width / 2) - (groupTutorialSize.x / 2);
				groupTutorialPos.y = (Screen.height / 2) - (groupTutorialSize.y / 2);

				Rect groupTutorialRect = new Rect (groupTutorialPos, groupTutorialSize);

				Vector2 tS;
				tS.x = groupTutorialSize.x * textureSize.x;
				tS.y = groupTutorialSize.y * textureSize.y;
		
				Vector2 tP;
				tP.x = 0.0f;
				tP.y = 0.0f;

				Rect textureRect = new Rect (tP, tS);

				Vector2 lS;
				lS.x = groupTutorialSize.x * labelSize.x;
				lS.y = groupTutorialSize.y * labelSize.y;
		
				Vector2 lP;
				lP.x = tS.x;
				lP.y = 0.0f;

				Rect lableRect = new Rect (lP, lS);

				GUI.BeginGroup (groupTutorialRect);

				GUI.DrawTexture (textureRect, textures [count]);

				GUI.Label (lableRect, texts [count]);

				GUI.EndGroup ();
			}
		}
	}

	private void DrawBackground()
	{
		//draw background
		Rect backgroundTextureRect = new Rect (0, 0, Screen.width, Screen.height);
		Graphics.DrawTexture (backgroundTextureRect, backgroundTexture); 
	}

}
