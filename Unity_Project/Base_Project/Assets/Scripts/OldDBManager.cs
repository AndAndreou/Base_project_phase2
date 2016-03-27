using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;



public class OldDBManager : MonoBehaviour {

	/*
	 * LoginTable (UserID (int), UserName (char30), UserEmail (char50), Password (char20), Login_Timestamp (timestamp), Counter (int))
	 * 
	 */
	private MySqlConnection con = null;
	//private MySqlDataReader reader = null;
	private string server;
	private string database;
	private string uid;
	private string password;
	private string connectionString;

	//private string playerUserName;
	//private string playerPassword;
	//private int playerUserID;

	private AsyncOperation async = null;

	// Use this for initialization
	void Awake () {

		/*
		//cs.ucy DB Server (need vpn)
		server = "dbserver.in.cs.ucy.ac.cy";
		database = "basedb";
		uid = "basedb";
		password = "FMZBQbGpus";
		*/

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
	}

	/*---------------------------------------------------------------------------------------------------------------*/
	 
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown(dbConnect)) {
			string str = @"server=dbserver.in.cs.ucy.ac.cy;database=basedb;userid=basedb;password=FMZBQbGpus;";
			MySqlConnection con = null;
			MySqlDataReader reader = null;
			try
			{
				con = new MySqlConnection(str);
				con.Open(); //open the connection
				Debug.Log("Connect in DB");

				string cmdText1 = "INSERT INTO LoginTable (UserName, UserEmail, Password, Counter) VALUES('unityinsert', 'unity3d@test', '1234', 0)";
				MySqlCommand cmd1 = new MySqlCommand(cmdText1,con);
				cmd1.ExecuteNonQuery();

				string cmdText = "SELECT * FROM LoginTable";
				MySqlCommand cmd = new MySqlCommand(cmdText,con);
				reader = cmd.ExecuteReader(); //execure the reader
				//The Read() method points to the next record It return false if there are no more records else returns true.
				while (reader.Read())
				{
					//reader.GetString(0) will get the value of the first column of the table myTable because we selected all columns using SELECT * (all); the first loop of the while loop is the first row; the next loop will be the second row and so on...
					Debug.Log(reader.GetString(0) + " " + reader.GetString(1) + " " + reader.GetString(2) + " " + reader.GetString(3) + " " + reader.GetString(4) + " " +  reader.GetString(5));
				}
			}
			catch (MySqlException err) //We will capture and display any MySql errors that will occur
			{
				Debug.Log("Error: " + err.ToString());
			}
		}*/
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	private bool OpenConnection()
	{
		try
		{
			con.Open();
			return true;
		}
		catch (MySqlException ex)
		{
			//When handling errors, you can your application's response based 
			//on the error number.
			//The two most common error numbers when connecting are as follows:
			//0: Cannot connect to server.
			//1045: Invalid user name and/or password.
			switch (ex.Number)
			{
			case 0:
				Debug.Log("Cannot connect to server.  Contact administrator");
				break;
				
			case 1045:
				Debug.Log("Invalid username/password, please try again");
				break;

			default:
				Debug.Log("Error Number : " + ex.Number);
				break;
			}
			return false;
		}
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//Close connection
	private bool CloseConnection()
	{
		try
		{
			con.Close();
			return true;
		}
		catch (MySqlException ex)
		{
			Debug.Log(ex.ToString());
			return false;
		}
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//Insert statement
	public void Insert(string query)
	//public IEnumerator Insert(string query)
	{
		//string query = "INSERT INTO tableinfo (name, age) VALUES('John Smith', '33')";
		//Debug.Log ("test");
		//open connection
		if (this.OpenConnection() == true)
		{
			try
			{
				//create command and assign the query and connection from the constructor
				MySqlCommand cmd = new MySqlCommand(query, con);
				//Execute command
				cmd.ExecuteNonQuery();
				//close connection
				this.CloseConnection();
			}
			catch (MySqlException ex)
			{
				Debug.Log(ex.ToString());
				//close connection
				this.CloseConnection();
			}

			//yield return async;
		}
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//Update statement
	public void Update(string query)
	{
		//string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
		
		//Open connection
		if (this.OpenConnection() == true)
		{
			//create mysql command
			MySqlCommand cmd = new MySqlCommand();
			//Assign the query using CommandText
			cmd.CommandText = query;
			//Assign the connection using Connection
			cmd.Connection = con;
			
			//Execute query
			cmd.ExecuteNonQuery();
			
			//close connection
			this.CloseConnection();
		}
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//Delete statement
	public void Delete(string query)
	{
		//string query = "DELETE FROM tableinfo WHERE name='John Smith'";
		
		if (this.OpenConnection() == true)
		{
			MySqlCommand cmd = new MySqlCommand(query, con);
			cmd.ExecuteNonQuery();
			this.CloseConnection();
		}
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	//Count statement
	public int Count(string tb)
	{
		string query = "SELECT Count(*) FROM " + tb ;
		int Count = -1;
		
		//Open Connection
		if (this.OpenConnection() == true)
		{
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand(query, con);
			
			//ExecuteScalar will return one value
			Count = int.Parse(cmd.ExecuteScalar()+"");

			//Debug.Log(Count);

			//close Connection
			this.CloseConnection();
			
			return Count;
		}
		else
		{
			return Count;
		}
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public string CheckLogin(string username, string pass){

		string cmdText = "SELECT * FROM users ur WHERE ur.username = '" + username + "' AND ur.password = '" + pass + "'";

		//Open Connection
		if (this.OpenConnection() == true)
		{
			string returnMsg;
			MySqlDataReader reader = null;


			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand(cmdText,con);
			
			//execure the reader
			reader = cmd.ExecuteReader(); 


			if (reader.Read())
			{
				int playerUserID = int.Parse(reader.GetString(3));
				string playerUserName = username;
				string playerPassword = pass;
				returnMsg = "OK";

				DBInfo.SetUsername(playerUserName);
				DBInfo.SetPassword(playerPassword);
				DBInfo.SetID(playerUserID);

				reader.Close();
				this.CloseConnection();

				//find game round
				int gameRound = FindGameRound(playerUserID);
				Debug.Log("gameRound : " + gameRound);

				if ( gameRound >=0){
					DBInfo.SetGameRoundId(gameRound);
				}
				else{
					//set new game rount
					SetGameRound(playerUserID);
				}

				//find current section
				FindCurrentSection(gameRound);


				DBInfo.SetLastSectionSerialNumber(GetLastSectionSerialNumber());

				Debug.Log("last sections : " + DBInfo.GetLastSectionSerialNumber());

				//get info from db for all sections (serial num, title, description)
				DBInfo.SetSectionsInfo(GetAllSectionsInfo(1));

				//print list
				/*
				List<SectionInfoStruct> sis = DBInfo.GetSectionsInfo();
				for (int i = 0; i< sis.Count ; i++){
					Debug.Log(sis[i].serialNumber +"  "+ sis[i].title +"  "+ sis[i].description);
				}
				*/

			}
			else
			{
				this.CloseConnection();
				returnMsg = "UserName or Password in not correct";
			}

			return returnMsg;
		
		}
		else
		{
			this.CloseConnection();
			return ("DataBase can not connect");
		}
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public int FindCurrentSection (int gameRound){

		int currentSection = -1;
		int lastQuestion = -1;
		int nextQuestion = -1;

		//current section
		string cmdText = "SELECT MAX(s.serial_number) as temp FROM game_round_answers ga " +
			"INNER JOIN questions q ON q.question_ID = ga.questions_question_id " +
			"INNER JOIN section s ON s.section_id = q.section_section_id " +
				"WHERE game_round_game_round_id = '" + gameRound + "' " ; //+
			//"GROUP BY q.section_section_id";
			

		//Open Connection
		if (this.OpenConnection () == true) {
			string returnMsg;
			MySqlDataReader reader = null;
			
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand (cmdText, con);
			
			//execure the reader
			reader = cmd.ExecuteReader (); 


			if (reader.Read ()) {
				if (!reader.IsDBNull (reader.GetOrdinal ("temp"))) {
					currentSection = int.Parse (reader.GetString (0));
				} else {
					currentSection=-1;
				}
			}

		} 
		else {
			currentSection = -2;
		}

		this.CloseConnection();

		Debug.Log ("currentSection : " + currentSection);


		if (currentSection >= 0) {

			DBInfo.SetCurrentSection (currentSection);
			//last question
			cmdText = "SELECT MAX(q.question_id) as temp FROM game_round_answers ga " +
				"INNER JOIN questions q ON q.question_ID = ga.questions_question_id " +
				"INNER JOIN section s ON s.section_id = q.section_section_id " +
				"WHERE game_round_game_round_id = '" + gameRound + "' AND serial_number = '" + currentSection + "' ";

			//Open Connection
			if (this.OpenConnection () == true) {
				string returnMsg;
				MySqlDataReader reader = null;
			
			
				//Create Mysql Command
				MySqlCommand cmd = new MySqlCommand (cmdText, con);
			
				//execure the reader
				reader = cmd.ExecuteReader (); 

				if (reader.Read ()) {
					if (!reader.IsDBNull (reader.GetOrdinal ("temp"))) {
						lastQuestion = int.Parse (reader.GetString (0));
					} else {
						lastQuestion = -1;
					}
				}

			} else {
				lastQuestion = -2;
			}
	
			this.CloseConnection ();

			Debug.Log ("lastQuestion : " + lastQuestion);

			//next question
			cmdText = "SELECT MIN(q.question_id) as temp FROM questions q " +
				"INNER JOIN section s ON s.section_id = q.section_section_id " +
					"WHERE serial_number = '" + currentSection + "' AND question_id > '" + lastQuestion + "' ";
		
			//Open Connection
			if (this.OpenConnection () == true) {
				string returnMsg;
				MySqlDataReader reader = null;
			
			
				//Create Mysql Command
				MySqlCommand cmd = new MySqlCommand (cmdText, con);
			
				//execure the reader
				reader = cmd.ExecuteReader (); 

				if (reader.Read ()) {
					if (!reader.IsDBNull (reader.GetOrdinal ("temp"))) {
						nextQuestion = int.Parse (reader.GetString (0));
					} else {
						nextQuestion = -1;
					}
				}

			} else {
				nextQuestion = -2;
			}

			this.CloseConnection ();

			Debug.Log ("nextQuestion : " + nextQuestion);

			if(nextQuestion == -1){
				DBInfo.SetCurrentSection(DBInfo.GetCurrentSection()+1); //kanonika prepi na vrisko  pio ine to epomeno section
				//DBInfo.SetCurrentQuestion(-1); //kanonika prepi na vrisko pia ine i proti erotisi tou section
				DBInfo.SetCurrentQuestion(GetFirstQuestionIdFromNextSection(DBInfo.GetCurrentSection()));
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

		Debug.Log ("last - currentSection : " + DBInfo.GetCurrentSection());
		Debug.Log ("last - currentquestion : " + DBInfo.GetCurrentQuestion());

		return 0;

	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public int FindGameRound(int playerID){
		
		string cmdText = "SELECT MAX(gr.game_round_id) as temp FROM game_round gr WHERE gr.users_user_id = '" + playerID + "'";
		int gameRound = -1;
		
		//Open Connection
		if (this.OpenConnection () == true) {
			string returnMsg;
			MySqlDataReader reader = null;

			//Debug.Log("test1");
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand (cmdText, con);
			
			//execure the reader
			reader = cmd.ExecuteReader (); 

			if (reader.Read ()) {
				if (!reader.IsDBNull (reader.GetOrdinal ("temp"))) {
					gameRound = int.Parse (reader.GetString (0));
				} else {
					gameRound=-1;
				}
			}


			this.CloseConnection();
			return gameRound;
		} 
		else {
			this.CloseConnection();
			return -2;
		}
		
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

	public List<Q_AStruct> GetQandA(int seqno){

		//Dictionary<QStruct,List<AStruct>> QandA = new Dictionary<QStruct,List<AStruct>>();
		List<Q_AStruct> QandA = new List<Q_AStruct> ();
		List<QStruct> qstruct = new List<QStruct> ();


		string cmdText = "SELECT * FROM questions qs WHERE qs.section_section_id = '" + seqno + "'";
		
		//Open Connection
		if (this.OpenConnection () == true) {
			//string returnMsg;
			MySqlDataReader reader = null;
			
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand (cmdText, con);
			
			//execure the reader
			reader = cmd.ExecuteReader (); 

			while (reader.Read()) {
				//reader.GetString(0) will get the value of the first column of the table myTable because we selected all columns using SELECT * (all); the first loop of the while loop is the first row; the next loop will be the second row and so on...
				//Debug.Log (reader.GetString (0) + " " + reader.GetString (1) + " " + reader.GetString (3)); 

				qstruct.Add(new QStruct (reader.GetString (1),int.Parse(reader.GetString (0))));

			}
		
			this.CloseConnection ();
		}

		//foreach (QStruct q in qstruct) {
		for (int i=0; i< qstruct.Count;i++){
			List<AStruct> astruct = new List<AStruct> ();
			//Open Connection
			if (this.OpenConnection () == true) {
				string cmdText2 = "SELECT * FROM answers aw WHERE aw.questions_question_id = '" + qstruct[i].qno + "'";
				MySqlDataReader reader2 = null;
				MySqlCommand cmd2 = new MySqlCommand (cmdText2, con);
				reader2 = cmd2.ExecuteReader ();
				while (reader2.Read()) {
					//Debug.Log ("-----> " + reader2.GetString (0) + " " + reader2.GetString (1));
					astruct.Add(new AStruct(reader2.GetString (1),int.Parse(reader2.GetString (0))));
				}
				this.CloseConnection ();
			}
			QandA.Add(new Q_AStruct(qstruct[i],astruct));
		}

		return QandA;
	}
	
	/*---------------------------------------------------------------------------------------------------------------*/

	public void AddUserAnswers(List<TwoInt> userAnswers){
		for (int i=0; i<userAnswers.Count; i++) {
			//Debug.Log(DBInfo.GetID());
			string query = "INSERT INTO game_round_answers (game_round_game_round_id, questions_question_id, answers_answer_id) VALUES('" + DBInfo.GetGameRoundId() + "', '" + userAnswers[i].questionNo + "', '" + userAnswers[i].answerNo + "')";
			try
			{
				//Debug.Log(":::::::::::::::::::::::::::::::::::::");
				Insert(query);
				//StartCoroutine (Insert(query));
			}
			catch (MySqlException ex)
			{
				Debug.Log("Error Insert");
				this.CloseConnection();
				
			}
		}

	}
	
	/*---------------------------------------------------------------------------------------------------------------*/


	public float GetSuccessRateForSection (int gameRound, int sectionNo){

		string cmdText = "SELECT round(sum(iscorrect)/count(*),2) " +
			"FROM game_round_answers " +
			"INNER JOIN questions ON questions.question_id = questions_question_id  " +
			"INNER JOIN answers ON answers.answer_id = answers_answer_id  " +
			"WHERE game_round_answers.game_round_game_round_id = '" + gameRound + "' AND questions.section_section_id = '" + sectionNo + "'";
		
		//Open Connection
		if (this.OpenConnection() == true)
		{
			float successRate;
			MySqlDataReader reader = null;
			
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand(cmdText,con);
			
			//execure the reader
			reader = cmd.ExecuteReader(); 

			if (reader.Read())
			{
				successRate = float.Parse(reader.GetString(0));
				
			}
			else
			{
				this.CloseConnection();
				successRate = (-1f);
			}

			this.CloseConnection();
			return successRate;

		}
		else
		{
			this.CloseConnection();
			return (-2f);//("can not open connection");
		}
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public int GetSectionSerialNumber (int sectionId){
		
		string cmdText = "SELECT serial_number " +
				"FROM section " +
				"WHERE section_id = '" + sectionId + "'";
		
		//Open Connection
		if (this.OpenConnection() == true)
		{
			int sectionSerialNumber;
			MySqlDataReader reader = null;
			
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand(cmdText,con);
			
			//execure the reader
			reader = cmd.ExecuteReader(); 
			
			if (reader.Read())
			{
				sectionSerialNumber = int.Parse(reader.GetString(0));
				
			}
			else
			{
				//this.CloseConnection();
				sectionSerialNumber = (-1);
			}
			
			this.CloseConnection();
			return sectionSerialNumber;
			
		}
		else
		{
			this.CloseConnection();
			return (-2);//("can not open connection");
		}
	}

	/*---------------------------------------------------------------------------------------------------------------*/
	
	public int GetLastSectionSerialNumber (){
		
		string cmdText = "SELECT MAX(serial_number) " +
			"FROM section ";
		
		//Open Connection
		if (this.OpenConnection() == true)
		{
			int lastSectionSerialNumber;
			MySqlDataReader reader = null;
			
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand(cmdText,con);
			
			//execure the reader
			reader = cmd.ExecuteReader(); 
			
			if (reader.Read())
			{
				lastSectionSerialNumber = int.Parse(reader.GetString(0));
				
			}
			else
			{
				//this.CloseConnection();
				lastSectionSerialNumber = (-1);
			}
			
			this.CloseConnection();
			return lastSectionSerialNumber;
			
		}
		else
		{
			this.CloseConnection();
			return (-2);//("can not open connection");
		}
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public int GetFirstQuestionIdFromNextSection (int currentSectionSerialNumber){

		string cmdText = "SELECT MIN(question_id) " +
			"FROM questions " + 
			"INNER JOIN section s ON s.section_id = section_section_id " +
			"WHERE s.serial_number = '" + (currentSectionSerialNumber +1) + "'";
		
		//Open Connection
		if (this.OpenConnection() == true)
		{
			int firstQuestionIdFromNextSection;
			MySqlDataReader reader = null;
			
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand(cmdText,con);
			
			//execure the reader
			reader = cmd.ExecuteReader(); 
			
			if (reader.Read())
			{
				firstQuestionIdFromNextSection = int.Parse(reader.GetString(0));
				
			}
			else
			{
				//this.CloseConnection();
				firstQuestionIdFromNextSection = (-1);
			}
			
			this.CloseConnection();
			return firstQuestionIdFromNextSection;
			
		}
		else
		{
			this.CloseConnection();
			return (-2);//("can not open connection");
		}
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public int[] GetPlayerRank(int playerId, int gameRoundId){
		string cmdText = "SELECT user_id, game_round_game_round_id, round(sum(iscorrect)/count(*),2) as success_rate " +
			"FROM game_round_answers " + 
				"INNER JOIN (SELECT user_id, game_round_id, username FROM game_round INNER JOIN users ON users.user_id = users_user_id) as us ON us.game_round_id = game_round_game_round_id " +
				"INNER JOIN answers ON answers.answer_id = answers_answer_id " +
				"GROUP BY user_id, game_round_game_round_id " +
				"ORDER BY success_rate DESC" ;

		//Open Connection
		if (this.OpenConnection() == true)
		{
			int[] playerRank = new int[2]; //0 - position, 1 - points 
			playerRank[0] = -1;
			playerRank[1] = -1;

			MySqlDataReader reader = null;
			
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand(cmdText,con);
			
			//execure the reader
			reader = cmd.ExecuteReader(); 

			int i=1;
			float prevPlayerRank = -1f;
			while (reader.Read())
			{

				if ((int.Parse(reader.GetString(0)) == playerId) || (int.Parse(reader.GetString(1)) == gameRoundId)){
					playerRank[0] = i;
					playerRank[1] = Mathf.RoundToInt(float.Parse(reader.GetString(2)) * 100);
				}

				if (prevPlayerRank != float.Parse(reader.GetString(2))){
					i++;
				}
				prevPlayerRank =float.Parse(reader.GetString(2));
			}
			/*else
			{
				//this.CloseConnection();
				lastSectionSerialNumber = (-1);
			}*/
			
			this.CloseConnection();
			return playerRank;
			
		}
		else
		{
			this.CloseConnection();
			int[] temp = new int[2];
			temp[0] = -2;
			temp[1] = -2;
			return (temp);//("can not open connection");
		}
	}

	/*---------------------------------------------------------------------------------------------------------------*/

	public List<SectionInfoStruct> GetAllSectionsInfo (int gameId){
		
		string cmdText = "SELECT serial_number as temp, title, description " +
						 "FROM section " + 
						 "WHERE game_game_id = '" + gameId + "' " +
						 "ORDER BY serial_number ASC" ;

		List<SectionInfoStruct> sectionsInfo = new List<SectionInfoStruct> ();
		//Open Connection
		if (this.OpenConnection() == true)
		{
			//SectionInfoStruct sectionInfoStruct;
			MySqlDataReader reader = null;
			
			
			//Create Mysql Command
			MySqlCommand cmd = new MySqlCommand(cmdText,con);
			
			//execure the reader
			reader = cmd.ExecuteReader(); 
			
			while (reader.Read())
			{
				if (!reader.IsDBNull (reader.GetOrdinal ("temp"))){

					SectionInfoStruct sectionInfoStruct = new SectionInfoStruct(
															int.Parse(reader.GetString(0)),
															reader.GetString(1),
															""
														  );
					if (!reader.IsDBNull (reader.GetOrdinal ("description"))){
						sectionInfoStruct.description = reader.GetString(2);
					}

					sectionsInfo.Add(sectionInfoStruct);
				}
				
			}

			
			this.CloseConnection();

			return sectionsInfo;
			
		}
		else
		{
			this.CloseConnection();

			/*sectionInfoStruct.serialNumber = -1;
			sectionInfoStruct.title = "";
			sectionInfoStruct.description = "";*/

			return sectionsInfo;;//("can not open connection");
		}
	}


}
