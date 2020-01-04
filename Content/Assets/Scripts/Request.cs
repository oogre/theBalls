using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Request
{
	public static List<Request> requests = new List<Request>();
	public string type;
	public string userName;
	public Vector3 position;
	public Request(string name)
	{
		this.type = "player";
		this.userName = name;
		this.position = Vector3.zero;
	}
	public Request(string type, string name)
	{
		this.type = type;
		this.userName = name;
		this.position = Vector3.zero;
	}
	public Request(string type, string name, Vector3 position)
	{
		this.type = type;
		this.userName = name;
		this.position = position;
	}
}