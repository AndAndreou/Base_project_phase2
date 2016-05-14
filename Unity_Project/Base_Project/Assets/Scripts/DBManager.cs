using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;



public class DBManager : MonoBehaviour {

	/*
	 * LoginTable (UserID (int), UserName (char30), UserEmail (char50), Password (char20), Login_Timestamp (timestamp), Counter (int))
	 * 
	 */
	private MySqlConnection con = null;
	private string server;
	private string database;
	private string uid;
	private string password;
	private string connectionString;

	private AsyncOperation async = null;

	//Api Return Values
	//string hostLink = "http://localhost";
	string hostLink = "http://baseproject.eu";

	private UsersDBTable[] checkLoginApiDBTable;
	private string checkLoginApiMsg="";

	private OneValueDBTable[] currentSectionDBTable;
	private string currentSectionApiMsg="";

	private OneValueDBTable[] lastQuestionDBTable;
	private string lastQuestionApiMsg="";

	private OneValueDBTable[] nextQuestionDBTable;
	private string nextQuestionApiMsg="";

	private OneValueDBTable[] gameRoundDBTable;
	private string gameRoundApiMsg="";

	private QuestionsDBTable[] questionsDBTable;
	private string questionsApiMsg="";

	private AnswersDBTable[] answersDBTable;
	private string answersApiMsg="";

	private OneFloatValueDBTable[] successRateForSectionDBTable;
	private string successRateForSectionApiMsg="";

	private OneValueDBTable[] sectionSerialNoDBTable;
	private string sectionSerialNoApiMsg="";

	private OneValueDBTable[] lastSectionSerialNoDBTable;
	private string lastSectionSerialNoApiMsg="";

	private OneValueDBTable[] firtsQuestionIdNextLevelDBTable;
	private string firtsQuestionIdNextLevelApiMsg="";

	private PlayerRankDBTable[] playerRankDBTable;
	private string playerRankApiMsg="";

	private SectionsInfoDBTable[] allSectionsInfoDBTable;
	private string allSectionsInfoApiMsg="";

	private string insertMsg="";
	//

	// Use this for initialization
	void Awake () {

		/*
		//cs.ucy DB Server (need vpn)
		server = "dbserver.in.cs.ucy.ac.cy";
		database = "basedb";
		uid = "basedb";
		password = "FMZBQbGpus";
		*/

		/*
		//Haris DB Server
		server = "192.185.119.216";
		database = "wwwbasep_unitydb";
		uid = "wwwbaseproject";
		password = "baseproject321#";

		connectionString = "SERVER=" + server + ";" + 
			"DATABASE=" + database + ";" + 
				"UID=" + uid + ";" + 
				"PASSWORD=" + password + ";";
		
		con = new MySqlConnection(connectionString);
		*/
	}

	/*---------------------------------------------------------------------------------------------------------------*/
	 
	// Update is called once per frame
	void Update () {
		
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	private bool OpenConnection()
	{
		return true;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//Close connection
	private bool CloseConnection()
	{
		return true;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//Send_Query Api call
	IEnumerator PostApiSend_Query(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			Debug.Log("insert ok");
			insertMsg = "insert ok";

		}
		else
		{
			Debug.Log("--is null--PostApiSend_Query");
			Debug.Log(www.error);
			insertMsg = ("insert error : " + www.error);
		}

		yield return www;
	}


	/*---------------------------------------------------------------------------------------------------------------*/

	//Insert statement
	public void Insert(string query)
	{
		insertMsg = "";
		string Url = hostLink +"/slim/index.php/Send_Query";

		WWWForm form = new WWWForm ();
		form.AddField ("query", query);
		WWW www = new WWW(Url,form);
		StartCoroutine("PostApiSend_Query", www);

	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//checkLogin Api call
	IEnumerator GetApiCheckLogin(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		UsersDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new UsersDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<UsersDBTable>(rows[i]) ;
				checkLoginApiMsg = "ok";
				Debug.Log("CheckLogin ok");
				checkLoginApiDBTable = new UsersDBTable[dbTable.Length]; //se ola afta ta dio na vgoun ektos tou for
				checkLoginApiDBTable = dbTable;
			}

		}
		else
		{
			checkLoginApiMsg = www.error;
			Debug.Log("--is null--CheckLogin");
			Debug.Log(www.error);
		}


		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public string CheckLogin(string username, string pass){

		string Url = hostLink +"/slim/index.php/CheckLogin/" + username + "/" + pass;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiCheckLogin", www);

		string returnMsg;
		if (checkLoginApiMsg == "ok") {
			int playerUserID = checkLoginApiDBTable[0].user_id;
			string playerUserName = username;
			string playerPassword = pass;
			returnMsg = "OK";

			DBInfo.SetUsername(playerUserName);
			DBInfo.SetPassword(playerPassword);
			DBInfo.SetID(playerUserID);

			Debug.Log("PlayerUserID : " + DBInfo.GetID());

			//find game round
			int gameRound = FindGameRound(playerUserID);


			if ( gameRound >0){
				DBInfo.SetGameRoundId(gameRound);
			}
			else{
				//set new game rount
				SetGameRound(playerUserID);
			}

			Debug.Log("gameRound : " + DBInfo.GetGameRoundId());

			//find current section
			FindCurrentSection(gameRound);


			DBInfo.SetLastSectionSerialNumber(GetLastSectionSerialNumber());

			Debug.Log("last sections : " + DBInfo.GetLastSectionSerialNumber());

			//get info from db for all sections (serial num, title, description)
			DBInfo.SetSectionsInfo(GetAllSectionsInfo(1));

			return returnMsg;
		} 
		else if (checkLoginApiMsg == "{\"error\":{\"text\":No records found.}}") {
			return returnMsg = "UserName or Password in not correct";
		}
		else {
			returnMsg =  "DataBase can not connect";
			return returnMsg;
		}
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//currentSection Api call
	IEnumerator GetApiCurrentSection(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		OneValueDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new OneValueDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<OneValueDBTable>(rows[i]) ;
				currentSectionApiMsg = "ok";
				Debug.Log("GetApiCurrentSection ok");
				currentSectionDBTable = new OneValueDBTable[dbTable.Length];
				currentSectionDBTable =  dbTable;
			}

		}
		else
		{
			currentSectionApiMsg = www.error;
			Debug.Log("--is null--GetApiCurrentSection");
			Debug.Log(www.error);
		}
			
		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//LastQuestion Api call
	IEnumerator GetApiLastQuestion(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		OneValueDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new OneValueDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<OneValueDBTable>(rows[i]) ;
				lastQuestionApiMsg = "ok";
				Debug.Log("GetApiLastQuestion ok");
				lastQuestionDBTable = new OneValueDBTable[dbTable.Length];
				lastQuestionDBTable = dbTable ;
			}

		}
		else
		{
			lastQuestionApiMsg = www.error;
			Debug.Log("--is null--GetApiLastQuestion");
			Debug.Log(www.error);
		}
			
		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//NextQuestion Api call
	IEnumerator GetApiNextQuestion(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		OneValueDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new OneValueDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				//Debug.Log ("++++++" + (JsonUtility.FromJson<OneValueDBTable>(rows[i])).temp);
				dbTable[i] = JsonUtility.FromJson<OneValueDBTable>(rows[i]) ;
				//Debug.Log (">>>>>>" + dbTable[i].temp);
				nextQuestionApiMsg = "ok";
				Debug.Log("GetApiNextQuestion ok");
				nextQuestionDBTable = new OneValueDBTable[dbTable.Length];
				nextQuestionDBTable = dbTable ;
			}

		}
		else
		{
			nextQuestionApiMsg = www.error;
			Debug.Log("--is null--GetApiNextQuestion");
			Debug.Log(www.error);
		}
			
		yield return www;
	}


	/*---------------------------------------------------------------------------------------------------------------*/
	public int FindCurrentSection (int gameRound){

		int currentSection = -1;
		int lastQuestion = -1;
		int nextQuestion = -1;
		string returnMsg;

		//find current section
		string Url = hostLink +"/slim/index.php/FindCurrentSection/" + gameRound;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiCurrentSection", www);

		if (currentSectionApiMsg == "ok") {
			if (currentSectionDBTable[0].temp != null) {
				currentSection = currentSectionDBTable[0].temp;
			}
			else {
				currentSection = -1;
			}
				
		} 
		else if (currentSectionApiMsg == "{\"error\":{\"text\":No records found.}}") {
			currentSection = -1;
		}
		else {
			currentSection = -2;
		}
			
		Debug.Log ("currentSection : " + currentSection);


		if (currentSection >= 0) {

			DBInfo.SetCurrentSection (currentSection);

			//find last question
			string Url2 = hostLink +"/slim/index.php/LastQuestion/" + gameRound + "/" + currentSection ;
			WWW www2 = new WWW(Url2);
			StartCoroutine("GetApiLastQuestion", www2);

			if (lastQuestionApiMsg == "ok") {
				if (lastQuestionDBTable[0].temp != null) {
					lastQuestion = lastQuestionDBTable[0].temp;
				}
				else {
					lastQuestion = -1;
				}

			} 
			else if (lastQuestionApiMsg == "{\"error\":{\"text\":No records found.}}") {
				lastQuestion = -1;
			}
			else {
				lastQuestion = -2;
			}

			Debug.Log ("lastQuestion : " + lastQuestion);

			//find next question
			string Url3 = hostLink +"/slim/index.php/NextQuestion/" + currentSection + "/" + lastQuestion ;
			WWW www3 = new WWW(Url3);
			StartCoroutine("GetApiNextQuestion", www3);

			if (nextQuestionApiMsg == "ok") {
				if (nextQuestionDBTable[0].temp != 0) {
					nextQuestion = nextQuestionDBTable[0].temp;
					//Debug.Log ("-->nextQuestion : " + nextQuestion);
				}
				else {
					nextQuestion = 0;
				}

			} 
			else if (nextQuestionApiMsg == "{\"error\":{\"text\":No records found.}}") {
				nextQuestion = -1;
			}
			else {
				nextQuestion = -2;
			}
				

			Debug.Log ("nextQuestion : " + nextQuestion);

			if(nextQuestion == 0){
				DBInfo.SetCurrentQuestion(GetFirstQuestionIdFromNextSection(DBInfo.GetCurrentSection()));
				DBInfo.SetCurrentSection(DBInfo.GetCurrentSection()+1); //kanonika prepi na vrisko  pio ine to epomeno section
				//DBInfo.SetCurrentQuestion(-1); //kanonika prepi na vrisko pia ine i proti erotisi tou section
			}
			else{
				DBInfo.SetCurrentQuestion(nextQuestion);
			}

		} 
		else {
			if(currentSection == -1){
				DBInfo.SetCurrentSection(0); //kanonika prepi na vrisko  pio ine to proto section
				DBInfo.SetCurrentQuestion(1); //kanonika prepi na vrisko pia ine i proti erotisi tou section
			}
		}

		Debug.Log ("after processing - currentSection : " + DBInfo.GetCurrentSection());
		Debug.Log ("after processing - currentquestion : " + DBInfo.GetCurrentQuestion());

		return 0;

	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//FindGameRound Api call
	IEnumerator GetApiGameRound(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		OneValueDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new OneValueDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<OneValueDBTable>(rows[i]) ;
				gameRoundApiMsg = "ok";
				Debug.Log("GetApiGameRound ok");
				gameRoundDBTable = new OneValueDBTable[dbTable.Length];
				gameRoundDBTable = dbTable ;
			}

		}
		else
		{
			gameRoundApiMsg = www.error;
			Debug.Log("--is null--GetApiGameRound");
			Debug.Log(www.error);
		}

		yield return www;
	}


	/*---------------------------------------------------------------------------------------------------------------*/

	public int FindGameRound(int playerID){
		
		int gameRound = -1;

		string Url = hostLink +"/slim/index.php/FindGameRound/" + playerID ;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiGameRound", www);

		if (gameRoundApiMsg == "ok") {
			if (gameRoundDBTable[0].temp != null) {
				gameRound = gameRoundDBTable[0].temp;
			}
			else {
				gameRound = -1;
			}

		} 
		else if (gameRoundApiMsg == "{\"error\":{\"text\":No records found.}}") {
			gameRound = -1;
		}
		else {
			gameRound = -2;
		}

		return gameRound;
		
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public void SetGameRound(int playerUserID){

		//insert gameround
		string query = "INSERT INTO game_round (users_user_id) VALUES('" + playerUserID + "')";
		try
		{
			Insert (query);
			int gameRound = FindGameRound(playerUserID);
			//if (gameRound  < 0 ){
				DBInfo.SetGameRoundId(gameRound);
				//Debug.Log(DBInfo.GetGameRoundId());
			//}
		}
		catch (MySqlException ex)
		{
			this.CloseConnection();
		}


	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public string SignUp(string username, string pass, string year_of_birth, string country){

		string query = "INSERT INTO users (username, password, year_of_birth, country) VALUES('" + username + "', '" + pass + "', '" + year_of_birth + "', '" + country + "')";
		try
		{
			Insert (query);

			this.CloseConnection();

			if (insertMsg != "insert ok"){
				return insertMsg;
			}

			CheckLogin(username,pass);

			return "OK";
		}
		catch (MySqlException ex)
		{
			this.CloseConnection();
			return "UserName or Password not invalid";
		}
		//return "";
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//GetQuestionsfromSection Api call
	IEnumerator GetApiQuestionsFromSection(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		QuestionsDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new QuestionsDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<QuestionsDBTable>(rows[i]) ;
				questionsApiMsg = "ok";
				Debug.Log("GetApiQuestionsFromSection ok");
				questionsDBTable = new QuestionsDBTable[dbTable.Length];
				questionsDBTable = dbTable ;
			}

		}
		else
		{
			questionsApiMsg = www.error;
			Debug.Log("--is null--GetApiQuestionsFromSection");
			Debug.Log(www.error);
		}

		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//GetAnswersfromQuestion Api call
	IEnumerator GetApiAnswersFromQuestion(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		AnswersDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new AnswersDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<AnswersDBTable>(rows[i]) ;
				answersApiMsg = "ok";
				Debug.Log("GetApiAnswersFromQuestion ok");
				answersDBTable = new AnswersDBTable[dbTable.Length];
				answersDBTable = dbTable ;
			}

		}
		else
		{
			answersApiMsg = www.error;
			Debug.Log("--is null--GetApiAnswersFromQuestion");
			Debug.Log(www.error);
		}

		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public List<Q_AStruct> GetQandA(int seqno){

		//Dictionary<QStruct,List<AStruct>> QandA = new Dictionary<QStruct,List<AStruct>>();
		List<Q_AStruct> QandA = new List<Q_AStruct> ();
		List<QStruct> qstruct = new List<QStruct> ();


		string Url = hostLink +"/slim/index.php/GetQuestionsFromSection/" + seqno ;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiQuestionsFromSection", www);

		if (questionsApiMsg == "ok") {

			for (int i = 0; i < questionsDBTable.Length; i++) {
				qstruct.Add(new QStruct (questionsDBTable[i].question,questionsDBTable[i].question_id));
			}


			//foreach (QStruct q in qstruct) {
			for (int i=0; i< qstruct.Count;i++){
				List<AStruct> astruct = new List<AStruct> ();

				string Url2 = hostLink +"/slim/index.php/GetAnswersFromQuestion/" + qstruct[i].qno ;
				WWW www2 = new WWW(Url2);
				StartCoroutine("GetApiAnswersFromQuestion", www2);

				if (answersApiMsg == "ok") {
					for (int j = 0; j < answersDBTable.Length; j++) {
						astruct.Add(new AStruct (answersDBTable[j].answer,answersDBTable[j].answer_id));
					}
				}
				QandA.Add(new Q_AStruct(qstruct[i],astruct));
			}

		} 
		else if (questionsApiMsg == "{\"error\":{\"text\":No records found.}}") {
			QandA = null;
		}
		else {
			QandA = null;
		}

		return QandA;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public string AddUserAnswers(List<TwoInt> userAnswers){
		string msg = "--Error--AddUserAnswers-Empty arg";
		for (int i=0; i<userAnswers.Count; i++) {
			//Debug.Log(DBInfo.GetID());
			string query = "INSERT INTO game_round_answers (game_round_game_round_id, questions_question_id, answers_answer_id) VALUES('" + DBInfo.GetGameRoundId() + "', '" + userAnswers[i].questionNo + "', '" + userAnswers[i].answerNo + "')";
			try
			{
				//Debug.Log(":::::::::::::::::::::::::::::::::::::");
				Insert(query);
				//StartCoroutine (Insert(query));
				msg = "AddUserAnswers ok";
			}
			catch (MySqlException ex)
			{
				Debug.Log("Error Insert");
				this.CloseConnection();
				msg = "--Error--AddUserAnswers-MySqlException";
			}
		}
		return msg;

	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//GetSuccessRateForSection Api call
	IEnumerator GetApiSuccessRateForSection(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		OneFloatValueDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new OneFloatValueDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<OneFloatValueDBTable>(rows[i]) ;
				successRateForSectionApiMsg = "ok";
				Debug.Log("GetApiSuccessRateForSection ok");
				successRateForSectionDBTable = new OneFloatValueDBTable[dbTable.Length];
				successRateForSectionDBTable = dbTable ;
			}

		}
		else
		{
			successRateForSectionApiMsg = www.error;
			Debug.Log("--is null--GetApiSuccessRateForSection");
			Debug.Log(www.error);
		}

		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/


	public float GetSuccessRateForSection (int gameRound, int sectionNo){

		float successRate;

		string Url = hostLink +"/slim/index.php/GetSuccessRateForSection/" + gameRound + "/" + sectionNo ;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiSuccessRateForSection", www);

		if (successRateForSectionApiMsg == "ok") {
			successRate = successRateForSectionDBTable [0].temp;
		} 
		else if (successRateForSectionApiMsg == "{\"error\":{\"text\":No records found.}}") {
			successRate = -1;
		}
		else {
			successRate = -2;
		}

		return successRate;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//GetSectionSerialNumber Api call
	IEnumerator GetApiSectionSerialNumber(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		OneValueDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new OneValueDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<OneValueDBTable>(rows[i]) ;
				sectionSerialNoApiMsg = "ok";
				Debug.Log("GetApiSectionSerialNumber ok");
				sectionSerialNoDBTable = new OneValueDBTable[dbTable.Length];
				sectionSerialNoDBTable = dbTable ;
			}

		}
		else
		{
			sectionSerialNoApiMsg = www.error;
			Debug.Log("--is null--GetApiSectionSerialNumber");
			Debug.Log(www.error);
		}

		yield return www;
	}


	/*---------------------------------------------------------------------------------------------------------------*/

	public int GetSectionSerialNumber (int sectionId){

		int sectionSerialNumber;

		string Url = hostLink +"/slim/index.php/GetSectionSerialNumber/" + sectionId  ;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiSectionSerialNumber", www);

		if (sectionSerialNoApiMsg == "ok") {
			sectionSerialNumber = sectionSerialNoDBTable [0].temp;
		} 
		else if (sectionSerialNoApiMsg == "{\"error\":{\"text\":No records found.}}") {
			sectionSerialNumber = -1;
		}
		else {
			sectionSerialNumber = -2;
		}

		return sectionSerialNumber;

	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//GetLastSectionSerialNumber Api call
	IEnumerator GetApiLastSectionSerialNumber(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		OneValueDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new OneValueDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<OneValueDBTable>(rows[i]) ;
				lastSectionSerialNoApiMsg = "ok";
				Debug.Log("GetApiLastSectionSerialNumber ok");
				lastSectionSerialNoDBTable = new OneValueDBTable[dbTable.Length];
				lastSectionSerialNoDBTable  = dbTable ;
			}

		}
		else
		{
			lastSectionSerialNoApiMsg = www.error;
			Debug.Log("--is null--GetApiLastSectionSerialNumber");
			Debug.Log(www.error);
		}

		yield return www;
	}
		
	/*---------------------------------------------------------------------------------------------------------------*/
	
	public int GetLastSectionSerialNumber (){

		int lastSectionSerialNumber;
		string Url = hostLink +"/slim/index.php/GetLastSectionSerialNumber"  ;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiLastSectionSerialNumber", www);

		if (lastSectionSerialNoApiMsg == "ok") {
			lastSectionSerialNumber = lastSectionSerialNoDBTable [0].temp;
		} 		
		else if (lastSectionSerialNoApiMsg == "{\"error\":{\"text\":No records found.}}") {
			lastSectionSerialNumber = -1;
		}
		else {
			lastSectionSerialNumber = -2;
		}

		return lastSectionSerialNumber;

	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//GetFirstQuestionIdFromNextSection Api call
	IEnumerator GetApiFirstQuestionIdFromNextSection(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		OneValueDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = "[" + serviceData + "]";
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new OneValueDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<OneValueDBTable>(rows[i]) ;
				firtsQuestionIdNextLevelApiMsg = "ok";
				Debug.Log("GetApiFirstQuestionIdFromNextSection ok");
				firtsQuestionIdNextLevelDBTable = new OneValueDBTable[dbTable.Length];
				firtsQuestionIdNextLevelDBTable  = dbTable ;
			}

		}
		else
		{
			firtsQuestionIdNextLevelApiMsg = www.error;
			Debug.Log("--is null--GetApiFirstQuestionIdFromNextSection");
			Debug.Log(www.error);
		}

		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public int GetFirstQuestionIdFromNextSection (int currentSectionSerialNumber){

		int firstQuestionIdFromNextSection;

		string Url = hostLink +"/slim/index.php/GetFirstQuestionIdFromNextSection/" + currentSectionSerialNumber  ;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiFirstQuestionIdFromNextSection", www);

		if (firtsQuestionIdNextLevelApiMsg == "ok") {
			firstQuestionIdFromNextSection = firtsQuestionIdNextLevelDBTable [0].temp;
		} 
		else if (firtsQuestionIdNextLevelApiMsg == "{\"error\":{\"text\":No records found.}}") {
			firstQuestionIdFromNextSection = -1;
		}
		else {
			firstQuestionIdFromNextSection = -2;
		}

		return firstQuestionIdFromNextSection;

	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//GetGetRank Api call
	IEnumerator GetApiGetRank(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		PlayerRankDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new PlayerRankDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<PlayerRankDBTable>(rows[i]) ;
				playerRankApiMsg = "ok";
				Debug.Log("GetApiGetRank ok");
				playerRankDBTable = new PlayerRankDBTable[dbTable.Length];
				playerRankDBTable  = dbTable ;
			}

		}
		else
		{
			playerRankApiMsg = www.error;
			Debug.Log("--is null--GetApiGetRank");
			Debug.Log(www.error);
		}

		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public int[] GetPlayerRank(int playerId, int gameRoundId){

		int[] playerRank = new int[2]; //0 - position, 1 - points 
		playerRank[0] = -1;
		playerRank[1] = -1;

		string Url = hostLink +"/slim/index.php/GetRank"  ;
		WWW www = new WWW(Url);
		StartCoroutine("GetApiGetRank", www);

		if (playerRankApiMsg == "ok") {

			int i=1;
			float prevPlayerRank = -1f;

			for (int j = 1; j < playerRankDBTable.Length; j++) {

				if ((playerRankDBTable[j].user_id == playerId) || (playerRankDBTable[j].game_round_game_round_id == gameRoundId)) {
					playerRank [0] = i;
					playerRank [1] = Mathf.RoundToInt (playerRankDBTable[j].success_rate * 100);
				}

				if (prevPlayerRank != playerRankDBTable[j].success_rate) {
					i++;
				}
				prevPlayerRank = playerRankDBTable[j].success_rate;
			}

		} 
		else if (playerRankApiMsg == "{\"error\":{\"text\":No records found.}}") {
			playerRank[0] = -1;
			playerRank[1] = -1;
		}
		else {
			playerRank[0] = -2;
			playerRank[1] = -2;
		}

		return playerRank;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	//GetAllSectionsInfo Api call
	IEnumerator GetApiAllSectionsInfo(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}

		SectionsInfoDBTable[] dbTable;
		if (www.error != "Null")
		{
			//string serviceData = www.text;
			//MyClass serviceData = JsonUtility.FromJson<MyClass>(www.text) ;
			//Data is in json format, we need to parse the Json.
			string serviceData = www.text;

			//spazo to string kai perno kathe grami ksexorista
			serviceData = serviceData.Replace("[{", "");
			serviceData = serviceData.Replace("}]", "");
			serviceData = serviceData.Replace(",{", "");
			string[] rows = serviceData.Split(new char[]{'}'},System.StringSplitOptions.None);

			//ksanaferno kathe grami se morfi json
			for(int i=0; i < rows.Length; i++) {
				rows[i] = "{" + rows[i] + "}";
			}

			dbTable = new SectionsInfoDBTable[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<SectionsInfoDBTable>(rows[i]) ;

			}
			allSectionsInfoDBTable = new SectionsInfoDBTable[dbTable.Length];
			allSectionsInfoDBTable  = dbTable ;
			allSectionsInfoApiMsg = "ok";
			Debug.Log("GetApiAllSectionsInfo ok");

		}
		else
		{
			allSectionsInfoApiMsg = www.error;
			Debug.Log("--is null--GetApiAllSectionsInfo");
			Debug.Log(www.error);
		}

		yield return www;
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public List<SectionInfoStruct> GetAllSectionsInfo (int gameId){

		List<SectionInfoStruct> sectionsInfo = new List<SectionInfoStruct> ();

		string Url = hostLink +"/slim/index.php/GetAllSectionsInfo/" + gameId;
		WWW www = new WWW (Url);
		StartCoroutine ("GetApiAllSectionsInfo", www);

		if (allSectionsInfoApiMsg == "ok") {
			for (int i = 0; i < allSectionsInfoDBTable.Length; i++) {
				if (allSectionsInfoDBTable [i].serial_number != "") {
					Debug.Log(allSectionsInfoDBTable [i].serial_number + " | " + allSectionsInfoDBTable [i].title);
					SectionInfoStruct sectionInfoStruct = new SectionInfoStruct (int.Parse(allSectionsInfoDBTable [i].serial_number), allSectionsInfoDBTable [i].title, "");
					if (allSectionsInfoDBTable [i].description != null) {
						sectionInfoStruct.description = allSectionsInfoDBTable [i].description;
					}

					sectionsInfo.Add (sectionInfoStruct);
				}
			}
		} else if (allSectionsInfoApiMsg == "{\"error\":{\"text\":No records found.}}") {
			
		} else {
			
		}

		return sectionsInfo;
	}
}
