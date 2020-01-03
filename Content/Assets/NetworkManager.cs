﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;

[Serializable]
public class Player
{
	public static List<Player> players = new List<Player>();
	public string name;
	public Vector3 position;
	public Player(string name)
	{
		this.name = name;
	}
	public Player(string name, Vector3 position)
	{
		this.name = name;
		this.position = position;
	}
}

public class NetworkManager : MonoBehaviour
{
	private WebSocket ws;

	public GameObject playerPrefab;
	public GameObject startPoint;
	public Dictionary<string, GameObject> players;
	public GameObject me;

	public string serverAddress = "ws://OOGRE.fritz.box:1337";
	public string localName;

	void Start()
    {
		players = new Dictionary<string, GameObject>();
		ws = new WebSocket(serverAddress);
		ws.OnOpen += (sender, e) => ws.Send(JsonUtility.ToJson(new Player(localName)));
		ws.OnMessage += Websocket_MessageReceived;
		ws.OnError += (sender, e) => print(e.Message);
		ws.Connect();
	}

	// Update is called once per frame
	void Update()
    {
		for (int i = 0;  i < Player.players.Count; i++) {
			if (!players.ContainsKey(Player.players[i].name))
			{
				GameObject other = Instantiate(playerPrefab, startPoint.transform.localPosition, Quaternion.identity);
				Destroy(other.GetComponent<Rigidbody>());
				Destroy(other.GetComponent<GameController>());

				players.Add(Player.players[i].name, other);
			}
			print(Player.players[i].position);
			players[Player.players[i].name].transform.localPosition = Player.players[i].position;
		}
		Player.players.Clear();
		ws.Send(JsonUtility.ToJson(new Player(localName, me.transform.localPosition)));
	}
	
	private void Websocket_MessageReceived(object sender, MessageEventArgs e)
	{
		Player.players.Add(JsonUtility.FromJson<Player>(e.Data));
	}
}
