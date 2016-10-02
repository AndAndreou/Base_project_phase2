using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Do_Question : MonoBehaviour {

	private GameManager gameManager;
	private DBManager dbManager;
	private CameraController cameraController;
	//private CharacterController characterController;
	private MainChararacter_Controller characterController;

	public float distanceFromChar;
	public float distanceForUpdate;

	private GameObject targetGameObject;
	private Transform targetTransform;

	//[HideInInspector]
	private bool qAndaGUIShow;
	private bool lastTimeqAndaGUIShow;

	//private bool speak;

	private bool flagDistance;
	private bool flagPressButton;

	private bool init = false;

	private Animator animator;

	private Rect interactTextRect;
	public string interactText = "Press F To Talk";
	public GUIStyle InteractTextStyle;

    //new
    public string extraTextGreek = "";
    public string extraTextEnglish = "";
    public string sectionSerialNumberGreekText = "Θα πρέπει να συμπληρώσετε τις προηγούμενες ενότητες, προκειμένου να ξεκινήσει αυτή την ενότητα";
    public string sectionSerialNumberEnglishText = "You have to complete the previous sections in order to start this section";

    private string extraText = "" ;
	private string sectionSerialNumberText = "" ;

	//section no for take questions from db
	public int sectionNo;

	//skins
	public GUISkin qAndaSkin;
	public float answerfontSize = 0.01f;
	public float questionfontSize = 0.015f ;

	public GUISkin qAndaNextButtonSkin;
	public float nextButtonfontSize = 0.01f ;
	
	public GUISkin successRateSkin;
	public float successRatefontSize = 0.01f;
	public Vector2 successRateLabelSise  ;
	public Vector2 successRateButtonSise ;


	private int selGridInt = -1;
	//num of question for wait answer
	private int currentQuestion;
	private int noUpdateQuestionCount;

	//list of questions and answer
	private List<Q_AStruct> q_a;
	//list of user answers (save by id answers)
	private int[] userAnswers;

	private float timeExtraMsg = 0 ;
	public float displayTimeExtraMsg = 5.0f;
	private bool showExtraMsg;

	private bool haveChange;
	private int lastUpdateQuestion;


	private bool checkForResult; //bool for call function to search success rate
	private bool showSuccessRate; // bool for show the success rate
	private float successRate;
	private bool showPlayerGameRoundRank;
	private int[] playerGameRoundRank;

	private int sectionSerialNumber;

	// Use this for initialization
	void Start () {
	
		//speak = false;
		flagDistance = false;
		flagPressButton = false;
		animator = GetComponent<Animator> ();
		targetGameObject = GameObject.FindWithTag (GameRepository.GetPlayerTag ());
		targetTransform = targetGameObject.transform;
		gameManager = GameObject.FindWithTag (GameRepository.GetGameManagerTag()).GetComponent<GameManager>();
		dbManager = GameObject.FindWithTag (GameRepository.GetDBManagerTag()).GetComponent<DBManager>();
		//characterController = targetGameObject.GetComponent<CharacterController>();
		characterController = targetGameObject.GetComponent<MainChararacter_Controller>();
		cameraController = GameObject.FindWithTag (GameRepository.GetMainCameraTag()).GetComponent<CameraController>();

		//add new lines in string
		extraText = extraText.Replace("@", System.Environment.NewLine);

		//Init Interact text Rect
		Vector2 textSize = InteractTextStyle.CalcSize(new GUIContent(interactText));
		interactTextRect = new Rect(Screen.width / 2 - textSize.x / 2, Screen.height - (textSize.y + 5), textSize.x, textSize.y);

		if (sectionNo >= 0) {
			q_a = dbManager.GetQandA (sectionNo);
			userAnswers = new int[q_a.Count];

			//get the serial number from db
			sectionSerialNumber = dbManager.GetSectionSerialNumber (sectionNo);
		}


		lastTimeqAndaGUIShow = false;
		qAndaGUIShow = false;

		showExtraMsg = true;

		haveChange = false;

		checkForResult = false;
		showSuccessRate = false;
		successRate = -3.0f;

		showPlayerGameRoundRank = false;
		playerGameRoundRank = new int[2];
		playerGameRoundRank [0] = -1;
		playerGameRoundRank [1] = -1;
	
		currentQuestion = 0;

		if (sectionNo >= 0) {
			for (int i=0; i < q_a.Count; i++) {
				//Debug.Log ("!!!!!!!!!");
				if (q_a [i].question.qno == DBInfo.GetCurrentQuestion ()) {
					currentQuestion = i;
					break;
				}
			}
		}

		lastUpdateQuestion = currentQuestion;
		noUpdateQuestionCount = 0;

        //new
        if (DBInfo.GetInEnglish() == true) {
            extraText = extraTextEnglish;
            sectionSerialNumberText = sectionSerialNumberEnglishText;
        }
        else
        {
            extraText = extraTextGreek;
            sectionSerialNumberText = sectionSerialNumberGreekText;
        }

    init = true;

	}

/*---------------------------------------------------------------------------------------------------------------*/

	// Update is called once per frame
	void Update () {
		if (gameManager.GetIsPause () == false) {
			if (Mathf.Abs (Vector3.Distance (targetTransform.position, this.transform.position)) <= distanceFromChar) {
				flagDistance = true;
				if (Input.GetKeyDown (KeyCode.F)) {
					flagPressButton = true;
					SetQandAGUIShow(true);
					timeExtraMsg = Time.deltaTime;
				}
			} else {
				flagDistance = false;
				SetQandAGUIShow(false);
			}

			if (animator != null)
				animator.SetBool ("Speak", flagPressButton);

			//close Q&A GUI
			if(qAndaGUIShow == true){
				if(Input.GetKeyDown(KeyCode.Escape)){
					flagPressButton = false;
					SetQandAGUIShow(false);
					showSuccessRate = false ;
				}
			}

			if(showSuccessRate == true){
				if(Input.GetKeyDown(KeyCode.Escape)){
					showSuccessRate = false ;
					Cursor.visible = false;
				}
			}


			//run if change show or not the qAnda GUI
			if(lastTimeqAndaGUIShow != qAndaGUIShow){
				lastTimeqAndaGUIShow =qAndaGUIShow;
				if(qAndaGUIShow == true){
					Cursor.visible = true;
					characterController.SetDontRunUpdate(true);
					cameraController.SetDontRunUpdate(true);
				}
				else{
					Cursor.visible = false; 
					characterController.SetDontRunUpdate(false);
					cameraController.SetDontRunUpdate(false);
				}
			}

			//set timer
			if ((timeExtraMsg != 0) && (showExtraMsg == true)){
				timeExtraMsg += Time.deltaTime;
			}

			//if display time pass msg disable
			if(showExtraMsg == true){

				if (timeExtraMsg > displayTimeExtraMsg) {
					showExtraMsg = false;
					if(sectionNo < 0 ){
						flagPressButton = false;
						qAndaGUIShow = false;
						timeExtraMsg = 0 ;
						showExtraMsg = true;
					}
				}
			}

			//Debug.Log(Mathf.Abs (Vector3.Distance (targetTransform.position, this.transform.position)) >= distanceForUpdate);
			//Debug.Log("havechange : " + haveChange);
			if (DBInfo.GetID() != -1){
				if (sectionNo>=0){
					if (Mathf.Abs (Vector3.Distance (targetTransform.position, this.transform.position)) >= distanceForUpdate) {
						UpdateDB();
					}
				}
			}

			//find the successrate for this round
			if (checkForResult == true){
				checkForResult = false;
				showSuccessRate = true;
				Cursor.visible = true;
				characterController.SetDontRunUpdate(true);
				cameraController.SetDontRunUpdate(true);
				UpdateDB();
				successRate = GetSuccessRateForSection();


				if(sectionSerialNumber != DBInfo.GetLastSectionSerialNumber()){
					DBInfo.SetCurrentSection(sectionSerialNumber +1);
					DBInfo.SetCurrentQuestion(dbManager.GetFirstQuestionIdFromNextSection(sectionSerialNumber));
				}
				//start new round and get round rank
				else{
					playerGameRoundRank = dbManager.GetPlayerRank(DBInfo.GetID(),DBInfo.GetGameRoundId());
					showPlayerGameRoundRank = true;
					//playerGameRoundRank = dbManager.GetPlayerRank(DBInfo.GetID(),DBInfo.GetGameRoundId());
					DBInfo.SetCurrentSection(0);
					DBInfo.SetCurrentQuestion(1);

					//add new round
					dbManager.SetGameRound(DBInfo.GetID());
					DBInfo.SetGameRoundId(dbManager.FindGameRound(DBInfo.GetID()));
				}

			}


		}
	}

/*---------------------------------------------------------------------------------------------------------------*/


	void OnGUI(){
		if (gameManager.GetIsPause () == false) {

			if (showSuccessRate == true){

				GUI.skin = successRateSkin;

				successRateSkin.label.fontSize = Mathf.RoundToInt (Screen.width * successRatefontSize);


				string msg;
				if (successRate == -3.0f){
					msg = "wait";
				}
				else{
					//Debug.Log ("Success Rate is : " + successRate);
					msg = "Success Rate is : " + successRate;
				}

				Vector2 sizeLabel;
				sizeLabel.x = Screen.width * successRateLabelSise.x;
				sizeLabel.y = Screen.height * successRateLabelSise.y;

				Vector2 positionLabel;
				positionLabel.x = Screen.width / 2 - sizeLabel.x / 2;
				positionLabel.y = Screen.height / 2 - sizeLabel.y / 2;

				GUI.Label(new Rect(positionLabel.x, positionLabel.y, sizeLabel.x, sizeLabel.y),msg);

				Vector2 sizeButton;
				sizeButton.x = Screen.width * successRateButtonSise.x;
				sizeButton.y = Screen.height * successRateButtonSise.y;
				
				Vector2 positionButton;
				positionButton.x = Screen.width / 2 + sizeLabel.x / 2 - sizeButton.x ;
				positionButton.y = Screen.height / 2 + sizeLabel.y / 2;

				if(GUI.Button(new Rect(positionButton.x, positionButton.y, sizeButton.x, sizeButton.y),"Ok")){
					showSuccessRate = false ;
					Cursor.visible = false;
					characterController.SetDontRunUpdate(false);
					cameraController.SetDontRunUpdate(false);
				}

				return;
			}

			//show game round rank
			if (showPlayerGameRoundRank == true){

				Cursor.visible = true;

				GUI.skin = successRateSkin;
				
				successRateSkin.label.fontSize = Mathf.RoundToInt (Screen.width * successRatefontSize);
				
				
				string msg;
				if (playerGameRoundRank[0] == -1){
					msg = "wait";
				}
				else{
					msg = "Your position in : " + playerGameRoundRank[0] + " and your total rate is : " + playerGameRoundRank[1] + "/100" ;
				}
				
				Vector2 sizeLabel;
				sizeLabel.x = Screen.width * successRateLabelSise.x;
				sizeLabel.y = Screen.height * successRateLabelSise.y;
				
				Vector2 positionLabel;
				positionLabel.x = Screen.width / 2 - sizeLabel.x / 2;
				positionLabel.y = Screen.height / 2 - sizeLabel.y / 2;
				
				GUI.Label(new Rect(positionLabel.x, positionLabel.y, sizeLabel.x, sizeLabel.y),msg);
				
				Vector2 sizeButton;
				sizeButton.x = Screen.width * successRateButtonSise.x;
				sizeButton.y = Screen.height * successRateButtonSise.y;
				
				Vector2 positionButton;
				positionButton.x = Screen.width / 2 + sizeLabel.x / 2 - sizeButton.x ;
				positionButton.y = Screen.height / 2 + sizeLabel.y / 2;
				
				if(GUI.Button(new Rect(positionButton.x, positionButton.y, sizeButton.x, sizeButton.y),"Ok")){
					showPlayerGameRoundRank = false ;
					Cursor.visible = false;
					characterController.SetDontRunUpdate(false);
					cameraController.SetDontRunUpdate(false);
				}
				
				return;
			}


			if (/*!init ||*/ !flagDistance){
				return;
			}

			//show text if player is close
			if (!flagPressButton) {
				//Init Interact text Rect
				Vector2 textSize = InteractTextStyle.CalcSize (new GUIContent (interactText));
				interactTextRect = new Rect (Screen.width / 2 - textSize.x / 2, Screen.height - (textSize.y + 5), textSize.x, textSize.y);
				GUI.Label (interactTextRect, interactText, InteractTextStyle);
			}

			//if this seqtion != current section
			if ((flagPressButton) && (sectionNo>=0)){
				if(DBInfo.GetCurrentSection() != sectionSerialNumber){
					GUI.skin = successRateSkin;
					DrawSectionSerialNumberText();
					if (GUILayout.Button ("OK")) {
						flagPressButton = false;
						SetQandAGUIShow(false);
						showSuccessRate = false ;
						//Cursor.visible = false; 
					}
					return;
				}
			}


			//show question and answer panel
			if (qAndaGUIShow == true) 
			{
				//show extra msg
				if((currentQuestion == 0) && (extraText!="") && (showExtraMsg == true)){
					DrawExtraText();
				}
				else{
					if (sectionNo >= 0){
						DrawQandAGUI();
					}
				}
			}
		}

	}

	/*---------------------------------------------------------------------------------------------------------------*/
	
	public void SetQandAGUIShow(bool value)
	{
		qAndaGUIShow = value;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public void DrawQandAGUI(){
		GUI.skin = qAndaSkin;
		qAndaSkin.button.fontSize  = Mathf.RoundToInt (Screen.width * answerfontSize);
		qAndaSkin.label.fontSize = Mathf.RoundToInt (Screen.width * questionfontSize);

		string[] selStrings = new string[q_a[currentQuestion].answer.Count];
		for (int i=0; i < q_a[currentQuestion].answer.Count;i++){
			selStrings[i]=q_a[currentQuestion].answer[i].answer;
		}

		string question = q_a [currentQuestion].question.question;
		//string buttonText;
		//if(currentQuestion == q_a.Count - 1){
		//	buttonText = "Finish";
		//}
		//else{
		//	buttonText = "Next";
		//}


		//GUILayout.BeginArea(new Rect(0,0,qAndaSkin.label.fixedWidth,qAndaSkin.label.fixedHeight));
		GUILayout.Label (question);
		GUILayout.BeginVertical("Box");
			selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 1);
		GUILayout.EndVertical();

		GUI.skin = qAndaNextButtonSkin;
		//qAndaNextButtonSkin.button.fontSize  = Mathf.RoundToInt (Screen.width * nextButtonfontSize);

		//if (GUILayout.Button (buttonText)) {
		if ((Input.GetMouseButtonUp(0)) && (selGridInt>=0)){
		//set user answers
			if(q_a[currentQuestion].answer.Count != 0 )
				userAnswers[currentQuestion] = q_a[currentQuestion].answer[selGridInt].ano;
			//Debug.Log(selGridInt + "  " + q_a[currentQuestion].answer[selGridInt].ano);
			currentQuestion ++;
			noUpdateQuestionCount ++;
			haveChange = true;
			if(currentQuestion > q_a.Count-1){
				currentQuestion = 0;
				flagPressButton = false;
				qAndaGUIShow = false;
				checkForResult =true;
				}
				
			selGridInt=-1;
		}

	}

/*---------------------------------------------------------------------------------------------------------------*/

	private void DrawExtraText(){
		GUI.skin = qAndaNextButtonSkin;
		qAndaNextButtonSkin.label.fontSize = Mathf.RoundToInt (Screen.width * questionfontSize);
		GUILayout.Label (extraText);
	}

/*---------------------------------------------------------------------------------------------------------------*/
	
	private void DrawSectionSerialNumberText(){
		GUI.skin = qAndaNextButtonSkin;
		qAndaNextButtonSkin.label.fontSize = Mathf.RoundToInt (Screen.width * questionfontSize);
		GUILayout.Label (sectionSerialNumberText);
	}
/*---------------------------------------------------------------------------------------------------------------*/

	private float GetSuccessRateForSection(){

		return (dbManager.GetSuccessRateForSection(DBInfo.GetGameRoundId(),sectionNo));

	}

/*---------------------------------------------------------------------------------------------------------------*/

	private void UpdateDB(){
		if(haveChange == true){
			//Debug.Log("++++++++++++++++++++++++++++++++++++++++++++++++++");
			List<TwoInt> updateAnswerUser = new List<TwoInt>();
			//for(int i=0; i < currentQuestion ; i++){
			for(int i=0; i < noUpdateQuestionCount ; i++){
				Debug.Log(">>>>>>>>>>>>>");
				if (i> q_a.Count-1){
					break;
				}
				//updateAnswerUser.Add(new TwoInt(q_a[i].question.qno,userAnswers[i]));
				updateAnswerUser.Add(new TwoInt(q_a[(i+lastUpdateQuestion)%(q_a.Count)].question.qno,userAnswers[(i+lastUpdateQuestion)%(q_a.Count)]));
			}
			lastUpdateQuestion = currentQuestion;
			noUpdateQuestionCount = 0 ;
			haveChange = false;
			
			string returnMsg = dbManager.AddUserAnswers(updateAnswerUser);
			Debug.Log ("returnMsg from AddUserAnswers: " + returnMsg);
		}
	}

/*---------------------------------------------------------------------------------------------------------------*/

}
