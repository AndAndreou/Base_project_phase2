using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Q_AStruct
{
	public QStruct question ;
	public List<AStruct> answer;
		
	public Q_AStruct(QStruct q, List<AStruct> a)
	{
		question = q;
		answer = a;
			
	}
}
