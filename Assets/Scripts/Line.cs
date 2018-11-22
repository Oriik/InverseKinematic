using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour {

    [HideInInspector] public Node start;
    public Node end;
    private LineRenderer line;

    private Vector3 startPos;
    private Vector3 endPos;
    // Use this for initialization
    void Start () {
        line = GetComponent<LineRenderer>();
        if (end != null)
        {
            start = GetComponentInParent<Node>();
            startPos = start.transform.position;
            endPos = end.transform.position;
            Vector3[] tab = new Vector3[] { startPos, endPos };

            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.SetPositions(tab);
        }
          

    }

    public void Update()
    {
        if (end != null)
        {
            if(startPos != start.transform.position || endPos != end.transform.position)
            {
                startPos = start.transform.position;
                endPos = end.transform.position;
                Vector3[] tab = new Vector3[] { startPos, endPos };
                line.SetPositions(tab);
            }
        }
    }

	
	
}
