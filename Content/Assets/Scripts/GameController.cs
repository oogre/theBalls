using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DragController : MonoBehaviour
{
    public bool dragging;
    public abstract void OnMouseDragStart();
    public abstract void OnMouseDragEnd();
    public abstract void OnMouseDragging();

    void OnMouseDrag()
    {
        if (!dragging) {
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

public abstract class ColliderController : DragController
{

	public abstract void OnWallImpact(Collision collision);
	public abstract void OnSawImpact(Collision collision);
	public abstract void OnLavaImpact(Collision collision);
	public abstract void OnBonusImpact(Collision collision);

	public void OnCollisionEnter(Collision collision)
    {
		switch (collision.gameObject.tag) {
			case "wall": OnWallImpact(collision); break;
			case "speed":break;
			case "lava": break;
			case "saw": OnSawImpact(collision); break;
		}
	}
}

public class GameController : ColliderController
{
	private Rigidbody rb;
	private float playerDepth;
	private GameObject forceArrow;

	private List<GameObject> impactParticles;
	private GameObject quakyParticle;
	private LineRenderer lineRenderer;
	private Vector3 startPoint;

	public Vector3 force;
	public float forceMultiplier = 1000;
	public GameObject forceArrowPrefab;
	public GameObject impactParticlePrefab;
	public GameObject quakyParticlePrefab;
	public float quakyMagnitudeTreshold;
	public float impactMagnitudeTreshold;
	void Start()
	{
		playerDepth = Camera.main.WorldToScreenPoint(transform.localPosition).z;
		rb = gameObject.GetComponent<Rigidbody>();
		startPoint = transform.localPosition;
	}

	public override void OnMouseDragStart()
	{
		rb.constraints = RigidbodyConstraints.FreezeAll;
		forceArrow = Instantiate(forceArrowPrefab, transform.position, Quaternion.identity);
		lineRenderer = forceArrow.GetComponent<LineRenderer>();
	}

	public override void OnMouseDragEnd()
	{
		DestroyImmediate(forceArrow);
		rb.constraints = RigidbodyConstraints.FreezePositionZ;
		rb.AddForce(force * forceMultiplier);
	}

	public override void OnMouseDragging()
	{
		Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, playerDepth);
		//convert the screen mouse position to world point and adjust with offset
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace);
		curPosition.z = transform.localPosition.z;

		force = transform.localPosition - curPosition;

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
			Instantiate(impactParticlePrefab, collisionPoint, Quaternion.identity);
		}
		if (impactMagnitude > quakyMagnitudeTreshold)
		{
			Instantiate(quakyParticlePrefab, transform);
		}
	}

	public override void OnSawImpact(Collision collision)
	{
		transform.localPosition = startPoint;
		foreach (Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}
		rb.constraints = RigidbodyConstraints.FreezeAll;
		rb.constraints = RigidbodyConstraints.FreezePositionZ;
	}
	public override void OnLavaImpact(Collision collision)
	{
	}
	public override void OnBonusImpact(Collision collision)
	{
	}
}