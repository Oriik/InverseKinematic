using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Chain : MonoBehaviour
{
    #region Variables
    public static Chain singleton;

    [SerializeField] private List<Node> m_nodes = new List<Node>();
    [SerializeField] private float m_constraintAngleDegree = 30;

    private Dictionary<Node, Vector3> m_fixedNodesPositions;
    private List<float> m_distances;
    private float m_constraintAngleRadiant;
    private LineRenderer m_line;
    #endregion

    private void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("More than one instance");
            Destroy(singleton);
        }
        singleton = this;
    }

    void Start()
    {
        m_fixedNodesPositions = new Dictionary<Node, Vector3>();
        SetRadiantAngle();
        m_line = GetComponent<LineRenderer>();
        m_line.startWidth = 0.1f;
        m_line.endWidth = 0.1f;
        SetInitDistance();
        DrawLine();
    }

    public void OnNodeMovableChange(Node node)
    {
        if (m_fixedNodesPositions.ContainsKey(node))
        {
            m_fixedNodesPositions.Remove(node);
        }
        else
        {
            m_fixedNodesPositions.Add(node, node.transform.position);
        }
    }

    public void SetRadiantAngle()
    {
        m_constraintAngleRadiant = (Mathf.PI / 180) * m_constraintAngleDegree;
    }

    private void SetInitDistance()
    {
        m_distances = new List<float>();
        for (int i = 0; i < m_nodes.Count - 1; i++)
        {
            m_distances.Add(Vector3.Distance(m_nodes[i].transform.position, m_nodes[i + 1].transform.position));
        }
    }

    public void Move(Node node)
    {
        List<Node> nodesToMove = MovableChainFromNode(node);

        //FABRIK INVERSE KINEMATIC
        //Forward
        foreach (Node n in nodesToMove)
        {
            MoveNode(n);
        }
        nodesToMove.Reverse();
        m_nodes.Reverse();
        m_distances.Reverse();

        //Backward
        foreach (Node n in nodesToMove)
        {
            MoveNode(n);
        }
        m_nodes.Reverse();
        m_distances.Reverse();
        DrawLine();
    }

    private void MoveNode(Node node)
    {
        int i = m_nodes.IndexOf(node);
        if (i > 1)
        {
            Transform previousNode = m_nodes[i - 1].transform;
            Transform currentNode = m_nodes[i].transform;
            Ray ray = new Ray(previousNode.position, currentNode.position - previousNode.position);
            currentNode.position = ray.GetPoint(m_distances[i - 1]);

            Vector3 B = Vector3.Normalize(currentNode.position - previousNode.position);
            Vector3 N = Vector3.Normalize(previousNode.position - m_nodes[i - 2].transform.position);

            if (!(Vector3.Dot(N, B) > Mathf.Cos(m_constraintAngleRadiant)))
            {
                float theta = Mathf.Atan2(N.y, N.x);
                float thetaP = theta + m_constraintAngleRadiant;
                float thetaM = theta - m_constraintAngleRadiant;

                Vector3 normalP = new Vector3(Mathf.Cos(thetaP), Mathf.Sin(thetaP), 0);
                Vector3 normalM = new Vector3(Mathf.Cos(thetaM), Mathf.Sin(thetaM), 0);

                Vector3 p1 = previousNode.position + normalP * m_distances[i - 1];
                Vector3 p2 = previousNode.position + normalM * m_distances[i - 1];

                if (Vector3.Distance(currentNode.position, p1) < Vector3.Distance(currentNode.position, p2))
                {
                    currentNode.position = p1;
                }
                else
                {
                    currentNode.position = p2;
                }
            }
        }
        else if (i == 1)
        {
            Ray ray = new Ray(m_nodes[i - 1].transform.position, m_nodes[i].transform.position - m_nodes[i - 1].transform.position);
            m_nodes[i].transform.position = ray.GetPoint(m_distances[i - 1]);
        }
    }

    public void DrawLine()
    {
        Vector3[] tabPos = new Vector3[m_nodes.Count];
        for (int i = 0; i < m_nodes.Count; i++)
        {
            tabPos[i] = m_nodes[i].transform.position;
        }
        m_line.positionCount = tabPos.Length;
        m_line.SetPositions(tabPos);
    }

    private List<Node> MovableChainFromNode(Node node)
    {
        if (!m_nodes.Contains(node))
        {
            return null;
        }
        List<Node> result = new List<Node>();
        bool findNode = false;
        foreach (Node n in m_nodes)
        {
            if (n == node)
            {
                findNode = true;
            }
            if (n.Movable)
            {
                result.Add(n);
            }
            else
            {
                if (findNode)
                {
                    break;
                }
                else
                {
                    result.Clear();
                }
            }
        }

        return result;
    }
}
