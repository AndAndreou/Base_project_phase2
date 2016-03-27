using UnityEngine;
using System.Collections;

public class SimpleDoorTrigger : MonoBehaviour {
	public Transform Door;
	public float OpenAngleAmount ;
	public float SmoothRotation;	
	public string interactText = "Press F To Interact";
	public GUIStyle InteractTextStyle;
		
	private bool init = false;
	private bool hasEntered = false;
	private bool doorOpen = false;
	private Vector3 startAngle;
	private Vector3 openAngle;	
	private Rect interactTextRect;

	//
	private GameObject targetGameObject;
	private Transform targetTransform;
	private float distanceCheck = 2.0f ;
	//
		
	void Start () {
		//Check if Door Game Object is properly assigned
		if(Door == null){
			Debug.LogError (this + " :: Door Object Not Defined!");
		}

		//
		targetGameObject = GameObject.FindWithTag (GameRepository.GetPlayerTag ());
		targetTransform = targetGameObject.transform;
		//

		//Init Start and Open door angles
		startAngle = Door.eulerAngles;
		openAngle = new Vector3(startAngle.x, startAngle.y + OpenAngleAmount, startAngle.z);
		
		//Init Interact text Rect
		Vector2 textSize = InteractTextStyle.CalcSize(new GUIContent(interactText));
		interactTextRect = new Rect(Screen.width / 2 - textSize.x / 2, Screen.height - (textSize.y + 5), textSize.x, textSize.y);
		
		init = true;
	}
		
	void Update () {
		if(!init)
			return;

		//
		//Debug.Log (Mathf.Abs (Vector3.Distance (targetTransform.position, this.transform.position)) );
		if (Mathf.Abs (Vector3.Distance (targetTransform.position, this.transform.position)) <= distanceCheck) {
			hasEntered = true;
		} 
		else 
		{
			hasEntered = false;
		}
		//

		HandleDoorRotation();
		HandleUserInput();	
	}

	/*
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			hasEntered = true;
		}
	}
	
	void OnTriggerExit(Collider other){
		hasEntered = false;
	}
	*/

	void OnGUI(){
		if(!init || !hasEntered)
			return;

		//Init Interact text Rect
		Vector2 textSize = InteractTextStyle.CalcSize(new GUIContent(interactText));
		interactTextRect = new Rect(Screen.width / 2 - textSize.x / 2, Screen.height - (textSize.y + 5), textSize.x, textSize.y);
		GUI.Label(interactTextRect, interactText, InteractTextStyle);
	}
	
	void HandleDoorRotation(){
		if(!doorOpen)
			Door.rotation = Quaternion.Euler(Vector3.Slerp(Door.eulerAngles, startAngle, Time.deltaTime * SmoothRotation));
		else
			Door.rotation = Quaternion.Euler(Vector3.Slerp(Door.eulerAngles, openAngle, Time.deltaTime * SmoothRotation));
	}
	
	void HandleUserInput(){
		if(Input.GetKeyDown(KeyCode.F) && hasEntered){
			doorOpen = !doorOpen;
		}			
	}
}