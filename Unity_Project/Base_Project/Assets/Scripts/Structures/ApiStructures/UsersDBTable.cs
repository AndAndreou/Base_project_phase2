using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System;

//using on CheckLogin
[Serializable]
public class UsersDBTable
{
	public string username;
	public string email;
	public string password;
	public int user_id;
	public int roles_role_id;
	public int year_of_birth;
	public string country;
	public string create_time;
	public string name;
	public string surname;
	public int islive;
}
