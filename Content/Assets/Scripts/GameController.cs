using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DragController : MonoBehaviour
{
    public float playerDepth;
    protected Vector3 offset;
    public Vector3 force;

    public bool dragging;
    public bool WasDragging;

    public abstract void OnMouseDragStart();
    public abstract void OnMouseDragEnd();
    public abstract void OnMouseDragging();

    public void Start(){
        //translate the player position from the world to Screen Point
        playerDepth = Camera.main.WorldToScreenPoint(transform.localPosition).z;
        //calculate any difference between the cubes world position and the mouses Screen position converted to a world point  
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, playerDepth));
    }
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

    public float forceMultiplier = 1000;
    public GameObject forceArrowPrefab;
    GameObject forceArrow;
    Rigidbody rb;

    new void Start()
    {
        base.Start();
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
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
        force = transform.position - curPosition;
        forceArrow.transform.localScale = force;
    }
}