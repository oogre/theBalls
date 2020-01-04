using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
	[Range(-10, 10)]
	public float xOffset = 0;
	[Range(-10, 10)]
	public float yOffset = 0;
	[Range(0, 10)]
	public float FollowXSpeed = 2f;
	[Range(0, 10)]
	public float FollowYSpeed = 2f;

	private GameObject _target;
	public GameObject target{
		set { _target = value; }
		get { return _target; }
	}

	private void Start()
	{
		float x = _target.transform.position.x + xOffset;
		float y = _target.transform.position.y + yOffset;
		float z = -10.0f;
		transform.position = new Vector3(x, y, z);
	}

	private void Update()
	{
		float x = Mathf.Lerp(transform.position.x, _target.transform.position.x + xOffset, FollowXSpeed * Time.deltaTime);
		float y = Mathf.Lerp(transform.position.y, _target.transform.position.y + yOffset, FollowYSpeed * Time.deltaTime);
		float z = transform.position.z;
		transform.position = new Vector3(x, y, z);
	}
}