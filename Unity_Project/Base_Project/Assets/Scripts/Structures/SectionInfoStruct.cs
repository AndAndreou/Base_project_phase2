using UnityEngine;
using System.Collections;

public class SectionInfoStruct {

	public int serialNumber;
	public string title;
	public string description;
	
	public SectionInfoStruct(int sn, string tl, string ds)
	{
		serialNumber = sn;
		title = tl;
		description = ds;
		
	}
}
