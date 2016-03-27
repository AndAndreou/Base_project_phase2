using UnityEngine;
using System.Collections;

public class MainChararacter_Controller : MonoBehaviour {

	private GameManager gameManager;

	public float speedNormal = 1.0f;
	public float speedFast   = 4.0f;
	
	public float mouseSensitivityX = 5.0f;
	public float mouseSensitivityY = 5.0f;

	private bool zoom;
	private bool sprint;

	private bool dontRunUpdate;
	
	//float rotY = 0.0f;


	private Animator animator;
	// Use this for initialization
	void Start () {
	
		if (GetComponent<Rigidbody> ()) 
		{
			GetComponent<Rigidbody> ().freezeRotation = true;
		}

		gameManager = GameObject.FindWithTag (GameRepository.GetGameManagerTag()).GetComponent<GameManager>();

		animator = GetComponent<Animator> ();

		zoom = false;
		sprint = false;
		dontRunUpdate = false;

		//if the scene is the main scene then transfor the player in the last position in main scene
		if (Application.loadedLevelName == "main_scene") {
			Vector3 dbtransform = DBInfo.GetPlayerFirstPositionForMainScene();
			if (dbtransform != Vector3.zero){
				this.transform.position = dbtransform;
			}
			
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (gameManager.GetIsPause () == false) {
			if (dontRunUpdate == false) {
				float forward = Input.GetAxis ("Vertical");
				float strafe = Input.GetAxis ("Horizontal");
				//Debug.Log ("forward : " + forward + "  strafe : " + strafe);

				float run = forward * (Input.GetKey (KeyCode.LeftShift) ? speedFast : speedNormal);
				animator.SetFloat ("Speed", forward);
				animator.SetFloat ("Strafe", strafe);
				animator.SetFloat ("Run", run);
			

				//zoom
				if (Input.GetKey (KeyCode.Mouse1)) {
					zoom = true;
				} else {
					zoom = false;
				}


				float rotX = transform.localEulerAngles.y + Input.GetAxis ("Mouse X") * mouseSensitivityX;

				transform.localEulerAngles = new Vector3 (0.0f, rotX, 0.0f);

		

				// move forwards/backwards
				if (forward != 0.0f) {
					float speed = Input.GetKey (KeyCode.LeftShift) ? speedFast : speedNormal;
					Vector3 trans = new Vector3 (0.0f, 0.0f, forward * speed * Time.deltaTime);
					gameObject.transform.localPosition += gameObject.transform.localRotation * trans;
				}
		
				// strafe left/right
				if (strafe != 0.0f) {
					float speed = Input.GetKey (KeyCode.LeftShift) ? speedFast : speedNormal;
					Vector3 trans = new Vector3 (strafe * speed * Time.deltaTime, 0.0f, 0.0f);
					gameObject.transform.localPosition += gameObject.transform.localRotation * trans;
				}
			}
		}

	}

	public void teleport(Vector3 destination)
	{
		this.transform.position = destination /*+ new Vector3(0,  this.GetComponent<CapsuleCollider>().height/2.0f)*/;
	}

	public bool IsZooming()
	{
		return zoom;
	}
	
	public bool IsSprinting()
	{
		return sprint ;
	}

	public void SetDontRunUpdate(bool value){
		dontRunUpdate = value;
		//animator.SetBool ("Aiming", false);
		animator.SetFloat ("Speed", 0f);
	}

	public bool GetDontRunUpdate(){
		return dontRunUpdate;
	}
}
