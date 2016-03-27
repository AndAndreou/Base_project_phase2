using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System;

//using on GetQandA - get all answers from one question
[Serializable]
public class AnswersDBTable
{
	public int answer_id;
	public string answer;
	public int iscorrect;
	public int questions_question_id;
}