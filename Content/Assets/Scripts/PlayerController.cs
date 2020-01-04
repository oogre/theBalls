using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ColliderController
{
	private Rigidbody rigidbody;

	private LineRenderer lineRenderer;

	private Vector3 force;
	private GameObject quakyParticlePrefab;

	public string localName;

	public float forceMultiplier = 1000;
	public float impactMagnitudeTreshold;
	public float quakyMagnitudeTreshold;

	void Start()
	{
		GameController.Instance.player = this.gameObject;
		GameController.Instance.playerName = localName;

		rigidbody = this.GetComponent<Rigidbody>();
		lineRenderer = this.GetComponent<LineRenderer>();
		quakyParticlePrefab = Resources.Load<GameObject>("Parcticles/Splash") as GameObject;
		lineRenderer.SetPosition(0, Vector3.zero);
		lineRenderer.SetPosition(1, Vector3.zero);
	}

	void Update() {
		GameController.Instance.playerMoved(this.transform.localPosition);
	}

	public void convertToNonPlayer(){
        Destroy(this.GetComponent<Rigidbody>());
        Destroy(this.GetComponent<SphereCollider>());
		Destroy(this.GetComponent<PlayerController>());
	}

    public void GoTo(Vector3 position) {
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.transform.localPosition = position;
		rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
	}






	public override void OnMouseDragStart()
	{
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;	
	}






    public override void OnMouseDragEnd()
	{
		rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
		rigidbody.AddForce(force * forceMultiplier);
		lineRenderer.SetPosition(0, Vector3.zero);
		lineRenderer.SetPosition(1, Vector3.zero);
	}







	public override void OnMouseDragging()
	{
		Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		//convert the screen mouse position to world point and adjust with offset
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace);
		curPosition.z = transform.localPosition.z;

		force = transform.localPosition - curPosition;
		force.z = 0;

		float curve = Mathf.Min(Mathf.Max(force.magnitude / 3.0f, 0.0f), 1.0f);
		curve = Mathf.Pow(curve, 2f);
		lineRenderer.endWidth = Mathf.Lerp(1.0f, 0.0f, curve);
		lineRenderer.SetPosition(0, transform.localPosition);
		lineRenderer.SetPosition(1, curPosition);
	}




	public override void OnWallImpact(Collision collision)
	{
		float impactMagnitude = collision.relativeVelocity.magnitude;
		Vector3 collisionPoint = collision.contacts[0].point;
		if (impactMagnitude > impactMagnitudeTreshold)
		{
			GameController.Instance.addImpactMarkerAt(collisionPoint, true);
		}
		if (impactMagnitude > quakyMagnitudeTreshold)
		{
			Instantiate(quakyParticlePrefab, this.transform);
		}
	}




	public override void OnSawImpact(Collision collision)
	{
		foreach (Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}
		GameController.Instance.killThePlayer();
	}
}













public abstract class ColliderController : DragController
{
	public abstract void OnWallImpact(Collision collision);
	public abstract void OnSawImpact(Collision collision);

	public void OnCollisionEnter(Collision collision)
	{
		switch (collision.gameObject.tag)
		{
			case "wall": OnWallImpact(collision); break;
			case "speed": break;
			case "lava": break;
			case "saw": OnSawImpact(collision); break;
		}
	}
}

public abstract class DragController : MonoBehaviour
{
	private bool dragging;
	public abstract void OnMouseDragStart();
	public abstract void OnMouseDragEnd();
	public abstract void OnMouseDragging();

	void OnMouseDrag()
	{
		if (!dragging)
		{
			OnMouseDragStart();
		}
		OnMouseDragging();
		dragging = true;
	}

	void OnMouseUp()
	{
		if (dragging)
		{
			OnMouseDragEnd();
		}
		dragging = false;
	}
}