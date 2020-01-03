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

public class GameController : DragController
{
    private Rigidbody rb;
    private float playerDepth;
    private GameObject forceArrow;

    public Vector3 force;
    public float forceMultiplier = 1000;
    public GameObject forceArrowPrefab;
    
    void Start()
    {
        playerDepth = Camera.main.WorldToScreenPoint(transform.localPosition).z;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public override void OnMouseDragStart() {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        forceArrow = Instantiate(forceArrowPrefab, transform.position, Quaternion.identity);
    }

    public override void OnMouseDragEnd() {
        DestroyImmediate(forceArrow);
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.AddForce(force * forceMultiplier);
    }

    public override void OnMouseDragging() {
        Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, playerDepth);
        //convert the screen mouse position to world point and adjust with offset
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace);
        force = transform.position - curPosition;
        forceArrow.transform.localScale = force;
        forceArrow.transform.localPosition = transform.position + .5f * force;
    }
}