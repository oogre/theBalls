using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
	[Range(-10, 10)]
	public float xOffset = 0;
	public float FollowSpeed = 2f;
	public Transform Target;

	private void Start()
	{
		transform.position = getCamNewPos();
	}

	private void Update()
	{
		transform.position = Vector3.Slerp(transform.position, getCamNewPos(), FollowSpeed * Time.deltaTime);
	}

	private Vector3 getCamNewPos() {
		Vector3 newPosition = Target.position;
		newPosition.z -= 10;
		newPosition.x += xOffset;
		return newPosition;
	}
}