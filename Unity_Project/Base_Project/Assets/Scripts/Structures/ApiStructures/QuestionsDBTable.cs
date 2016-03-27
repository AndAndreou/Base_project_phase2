using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System;

//using on GetQandA - get all questions from one section
[Serializable]
public class QuestionsDBTable
{
	public int question_id;
	public string question;
	public int section_section_id;
}