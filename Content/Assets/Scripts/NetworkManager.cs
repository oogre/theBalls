using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


public class NetworkManager : Singleton<NetworkManager>
{
	private WebSocket ws;
	public string serverAddress = "ws://OOGRE.local:1337";
	
	public void Init()
    {
		ws = new WebSocket(serverAddress);
		ws.OnOpen += (sender, e) => ws.Send(JsonUtility.ToJson(new Request("player", GameController.Instance.playerName)));
		ws.OnMessage += (sender, e) => Request.requests.Add(JsonUtility.FromJson<Request>(e.Data));
		ws.OnError += (sender, e) => print(e.Message);
		ws.Connect();
	}

	void Update()
    {
		for (int i = 0;  i < Request.requests.Count; i++) {
			switch(Request.requests[i].type){
				case "player": {
					if (!GameController.Instance.isOtherPlayerExists(Request.requests[i].userName))
					{
						GameController.Instance.addOtherPlayer(Request.requests[i]);
					}
					GameController.Instance.moveOtherPlayer(Request.requests[i]);
				} 
				break;
				case "impact": {
					GameController.Instance.addImpactMarkerAt(Request.requests[i].position);
				}
				break;
			}
		}
		Request.requests.Clear();
	}

	public void newImpact(Vector3 position) {
		ws.Send(JsonUtility.ToJson(new Request("impact", GameController.Instance.playerName, position)));
	}

	public void playerMoved(Vector3 position) {
		ws.Send(JsonUtility.ToJson(new Request("player", GameController.Instance.playerName, position)));
	}

}
