using UnityEngine;
using System.Collections;

public class MinimapCameraMovement : MonoBehaviour {

	//target is main char
	private GameObject targetGameObject;
	public Transform targetTransform;

	// Use this for initialization
	void Start () {
	
		targetGameObject = GameObject.FindWithTag (GameRepository.GetPlayerTag ());
		targetTransform = targetGameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.position = new Vector3 (targetTransform.position.x, transform.position.y, targetTransform.position.z);

	}
}
