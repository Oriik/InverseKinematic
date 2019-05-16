using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class Node : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool m_movable = true;

    private Chain chain;

    public bool Movable { get => m_movable;}
    #endregion

    private void Start()
    {
        chain = Chain.singleton;
    }

    public void OnMovableChanged()
    {
        GetComponent<Renderer>().material.color = (Movable) ? Color.green : Color.red;
        chain.OnNodeMovableChange(this);
    }

    private void OnMouseDrag()
    {
        if (!Movable)
        {
            return;
        }
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;

        chain.Move(this);
    }
}
