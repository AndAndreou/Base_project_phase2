using UnityEngine;
using System.Collections;

public class GoToLevel : MonoBehaviour {
	
	private GameManager gameManager;
	private GUIManager guiManager;
	
	public float distanceFromChar = 1.5f;
	private bool flagDistance;
	private bool flagPressButton;
	
	private bool init = false;
	private GameObject targetGameObject;
	private Transform targetTransform;
	
	private Rect interactTextRect;
	public string interactText = "Press F To Exit";
	public GUIStyle InteractTextStyle;
	
	public string levelNameToGo;
	
	// Use this for initialization
	void Start () {
		
		flagDistance = false;
		flagPressButton = false;
		targetGameObject = GameObject.FindWithTag (GameRepository.GetPlayerTag ());
		targetTransform = targetGameObject.transform;
		gameManager = GameObject.FindWithTag (GameRepository.GetGameManagerTag()).GetComponent<GameManager>();
		guiManager = GameObject.FindWithTag (GameRepository.GetGUIManagerTag ()).GetComponent<GUIManager> ();
		
		
		//Init Interact text Rect
		Vector2 textSize = InteractTextStyle.CalcSize(new GUIContent(interactText));
		interactTextRect = new Rect(Screen.width / 2 - textSize.x / 2, Screen.height - (textSize.y + 5), textSize.x, textSize.y);
		
		init = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (gameManager.GetIsPause () == false) {
			if (Mathf.Abs (Vector3.Distance (targetTransform.position, this.transform.position)) <= distanceFromChar) {
				flagDistance = true;
				if (Input.GetKeyDown (KeyCode.F)) {
					if (Application.loadedLevelName == "main_scene") {
						Debug.Log("test");
						DBInfo.SetPlayerFirstPositionForMainScene(targetTransform.position);
					}
					guiManager.LoadLavel(levelNameToGo);
				}
			} else {
				flagDistance = false;
			}
		}
	}
	
	void OnGUI(){
		if (gameManager.GetIsPause () == false) {
			
			if (!init || !flagDistance)
				return;
			
			if (!flagPressButton) {
				//Init Interact text Rect
				Vector2 textSize = InteractTextStyle.CalcSize (new GUIContent (interactText));
				interactTextRect = new Rect (Screen.width / 2 - textSize.x / 2, Screen.height - (textSize.y + 5), textSize.x, textSize.y);
				GUI.Label (interactTextRect, interactText, InteractTextStyle);
			}
		}
	}
}
