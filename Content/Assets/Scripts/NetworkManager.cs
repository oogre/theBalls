using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
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
 
/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Check to see if we're about to be destroyed.
	private static bool m_ShuttingDown = false;
	private static object m_Lock = new object();
	private static T m_Instance;

	/// <summary>
	/// Access singleton instance through this propriety.
	/// </summary>
	public static T Instance
	{
		get
		{
			if (m_ShuttingDown)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
					"' already destroyed. Returning null.");
				return null;
			}

			lock (m_Lock)
			{
				if (m_Instance == null)
				{
					// Search for existing instance.
					m_Instance = (T)FindObjectOfType(typeof(T));

					// Create new instance if one doesn't already exist.
					if (m_Instance == null)
					{
						// Need to create a new GameObject to attach the singleton to.
						var singletonObject = new GameObject();
						m_Instance = singletonObject.AddComponent<T>();
						singletonObject.name = typeof(T).ToString() + " (Singleton)";

						// Make instance persistent.
						DontDestroyOnLoad(singletonObject);
					}
				}

				return m_Instance;
			}
		}
	}


	private void OnApplicationQuit()
	{
		m_ShuttingDown = true;
	}


	private void OnDestroy()
	{
		m_ShuttingDown = true;
	}
}


public class NetworkManager : Singleton<NetworkManager>
{
	private WebSocket ws;
	private Vector3 startPoint;
	private Vector3 position;
	private ColorManager colorManager;
	public GameObject me;
	public string localName;
	public GameObject playerPrefab;
	public GameObject impactParticlePrefab;
	public Dictionary<string, GameObject> players;
	public string serverAddress = "ws://OOGRE.fritz.box:1337";
	
	void Start()
    {
        startPoint = me.transform.localPosition;
		players = new Dictionary<string, GameObject>();
		ws = new WebSocket(serverAddress);
		ws.OnOpen += (sender, e) => ws.Send(JsonUtility.ToJson(new Request("player", localName)));
		ws.OnMessage += Websocket_MessageReceived;
		ws.OnError += (sender, e) => print(e.Message);
		ws.Connect();


		colorManager = Camera.main.GetComponent<ColorManager>();
}

	private void Websocket_MessageReceived(object sender, MessageEventArgs e)
	{
		Request.requests.Add(JsonUtility.FromJson<Request>(e.Data));
	}

	void Update()
    {
		for (int i = 0;  i < Request.requests.Count; i++) {
			if (Request.requests[i].type == "player")
			{
				if (!players.ContainsKey(Request.requests[i].userName))
				{
					GameObject other = Instantiate(playerPrefab, startPoint, Quaternion.identity);
					colorManager.setColorTo(other);
					other.name = Request.requests[i].userName;
					Destroy(other.GetComponent<Rigidbody>());
					Destroy(other.GetComponent<GameController>());
					Destroy(other.GetComponent<SphereCollider>());
					players.Add(other.name, other);
				}
				players[Request.requests[i].userName].transform.localPosition = Request.requests[i].position;
			}
			else if (Request.requests[i].type == "impact")
			{
				Instantiate(impactParticlePrefab, Request.requests[i].position, Quaternion.identity);
			}
		}
		Request.requests.Clear();

        if(position != me.transform.localPosition) { 
			ws.Send(JsonUtility.ToJson(new Request("player", localName, me.transform.localPosition)));
            position = me.transform.localPosition;
        }
	}

	public void newImpact(Vector3 position) {
		ws.Send(JsonUtility.ToJson(new Request("impact", localName, position)));
	}
}
