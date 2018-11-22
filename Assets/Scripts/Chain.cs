using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Chain : MonoBehaviour
{
    public static Chain singleton;

    public List<Node> nodes = new List<Node>(); //liste de tous les nodes
    private Dictionary<Node, Vector3> fixedNodesPositions; // positions des nodes fixes
    private List<float> distances;

    private LineRenderer line;

    public float contrainteAngleDegree = 30;
    private float m_contrainteAngleDegree;
    private float contrainteAngleRadiant;


    private void Awake()
    {
        if (singleton != null)
        {
            Debug.Log("Plus d'une instance de Chain");
        }
        singleton = this;
    }

    public void NodeMovementChange(Node node)
    {
        if (fixedNodesPositions.ContainsKey(node))
        {
            fixedNodesPositions.Remove(node);
        }
        else
        {
            fixedNodesPositions.Add(node, node.transform.position);
        }
    }

    void Start()
    {
        fixedNodesPositions = new Dictionary<Node, Vector3>();

        m_contrainteAngleDegree = contrainteAngleDegree;
        contrainteAngleRadiant = (Mathf.PI / 180) * contrainteAngleDegree;      

        line = GetComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        initDistance();

        DrawLine();

    }

    void Update()
    {
        if(m_contrainteAngleDegree != contrainteAngleDegree)
        {
            m_contrainteAngleDegree = contrainteAngleDegree;
            contrainteAngleRadiant = (Mathf.PI / 180) * contrainteAngleDegree;
        }
    }
    private void initDistance()
    {
        distances = new List<float>();
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            distances.Add(Vector3.Distance(nodes[i].transform.position, nodes[i + 1].transform.position));
        }
    }


    public void Move(Node node)
    {
        //FABRIK INVERSE KINEMATIC


        //Déplacement aller
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] != node && nodes[i].canMove)
            {               
                MoveNodes(i);
            }
        }

        nodes.Reverse();
        distances.Reverse();
      
        //Déplacement retour
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].canMove)
            {
                MoveNodes(i);
            }
        }
        nodes.Reverse();
        distances.Reverse();

        DrawLine();

    }

    private void MoveNodes(int i)
    {        
        if (i > 1)
        {
            Ray ray = new Ray(nodes[i - 1].transform.position, nodes[i].transform.position - nodes[i - 1].transform.position);
            nodes[i].transform.position = ray.GetPoint(distances[i - 1]);
            Vector3 B = Vector3.Normalize(nodes[i].transform.position - nodes[i - 1].transform.position);
            Vector3 N = Vector3.Normalize(nodes[i - 1].transform.position - nodes[i - 2].transform.position);

            if (!(Vector3.Dot(N, B) > Mathf.Cos(contrainteAngleRadiant)))
            {
                float theta = Mathf.Atan2(N.y, N.x);
                float thetaP = theta + contrainteAngleRadiant;
                float thetaM = theta - contrainteAngleRadiant;

                Vector3 normalP = new Vector3(Mathf.Cos(thetaP), Mathf.Sin(thetaP), 0);
                Vector3 normalM = new Vector3(Mathf.Cos(thetaM), Mathf.Sin(thetaM), 0);

                Vector3 p1 = nodes[i - 1].transform.position + normalP * distances[i - 1];
                Vector3 p2 = nodes[i - 1].transform.position + normalM * distances[i - 1];

                if (Vector3.Distance(nodes[i].transform.position, p1) < Vector3.Distance(nodes[i].transform.position, p2))
                {
                    nodes[i].transform.position = p1;
                }
                else
                {
                    nodes[i].transform.position = p2;
                }
            }

        }
        else if (i==1)
        {
            Ray ray = new Ray(nodes[i - 1].transform.position, nodes[i].transform.position - nodes[i - 1].transform.position);
            nodes[i].transform.position = ray.GetPoint(distances[i - 1]);
        }
    }


    public void DrawLine()
    {

        Vector3[] tabPos = new Vector3[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            tabPos[i] = nodes[i].transform.position;
        }

        line.positionCount = tabPos.Length;
        line.SetPositions(tabPos);


    }

}
