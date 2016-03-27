using UnityEngine;
using System.Collections;


public class GetJsonDataScript : MonoBehaviour
{
	// Use this for initialization
	string Url;
	string s = "+++++++++++++";

	void Start()
	{
		//Url = "http://localhost/slim/index.php/GetRank";
		//WWW www = new WWW(Url);
		//StartCoroutine("GetdataEnumerator", www);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log ("run test");
			test();
		}
	}
	IEnumerator GetdataEnumerator(WWW www)
	{
		//Wait for request to complete
		//yield return www;
		while(!www.isDone){}
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

			for(int i=0; i < rows.Length; i++) {
				Debug.Log(rows[i]) ;
			}

			MyClass[] dbTable = new MyClass[rows.Length];
			for(int i=0; i < rows.Length; i++) {
				dbTable[i] = JsonUtility.FromJson<MyClass>(rows[i]) ;
			}

			Debug.Log("--not null--");
			Debug.Log(dbTable[1].user_id);
			//Debug.Log(serviceData.temp);
		}
		else
		{
			Debug.Log("--is null--");
			Debug.Log(www.error);
		}

		s = ">>>>>>>>>>>>>>>>>>>>";
		yield return www;
	}

	void test(){
		Url = "http://localhost/slim/index.php/GetRank";
		WWW www = new WWW(Url);
		StartCoroutine("GetdataEnumerator", www);
		Debug.Log (s);
	}


}