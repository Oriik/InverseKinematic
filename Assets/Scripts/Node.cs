using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class Node : MonoBehaviour
{

    public bool canMove = true;

    private bool lastCanMove;
    private Chain chain;

    private bool one_click = false;
    private float timer_for_double_click;


    void Start()
    {
        chain = Chain.singleton;
        lastCanMove = canMove;
    }

    void Update()
    {
        if (canMove != lastCanMove)
        {
            GetComponent<Renderer>().material.color = (canMove) ? Color.green : Color.red;
            chain.NodeMovementChange(this);
            lastCanMove = canMove;
        }
        if (one_click)
        {
            if ((Time.time - timer_for_double_click) > 0.33)
            {
                one_click = false;
            }
        }
    }

    void OnMouseDown()
    {
        if (!one_click)
        {
            one_click = true;
            timer_for_double_click = Time.time;
        }
        else
        {
            one_click = false;
            canMove = !canMove;
        }


    }

    void OnMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;

        chain.Move(this);
    }
}
