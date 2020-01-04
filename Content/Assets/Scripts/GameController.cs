using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
	private GameObject playerPrefab;
	private GameObject impactParticlePrefab;
	private Vector3 lastPlayerPosition;
	private Dictionary<string, GameObject> players;
	private string _playerName;
	private Vector3 _startPoint;
	private GameObject _player;

    public string playerName {
		get { return _playerName; }
		set { _playerName = value; }
	}

    public GameObject player {
		get { return _player; }
		set {
			_player = value;
			_startPoint = _player.transform.localPosition;
			Camera.main.GetComponent<CameraFollow2D>().target = _player;
			NetworkManager.Instance.Init();
		}
	}
	
	void Start()
	{
		playerPrefab = Resources.Load<GameObject>("Prefabs/Sphere") as GameObject;
		impactParticlePrefab = Resources.Load<GameObject>("Parcticles/Splash") as GameObject;
	}

	public void addImpactMarkerAt(Vector3 collisionPoint, bool networkTransfert=false)
	{
		if(networkTransfert) NetworkManager.Instance.newImpact(collisionPoint);
		Instantiate(impactParticlePrefab, collisionPoint, Quaternion.identity);
	}

	public void killThePlayer()
	{
		_player.GetComponent<PlayerController>().GoTo(_startPoint);
	}

	public void playerMoved(Vector3 playerPos)
	{
        if(lastPlayerPosition != playerPos)
		{
			NetworkManager.Instance.playerMoved(playerPos);
			lastPlayerPosition = playerPos;
		}
	}

	public void addOtherPlayer(Request r)
	{
		GameObject other = Instantiate(playerPrefab, _startPoint, Quaternion.identity);
		other.name = r.userName;
		other.GetComponent<PlayerController>().convertToNonPlayer();
		players.Add(other.name, other);
	}

	public void moveOtherPlayer(Request r) {
		players[r.userName].transform.localPosition = r.position;
	}

	public bool isOtherPlayerExists(string name)
	{
		return players.ContainsKey(name);
	}
}