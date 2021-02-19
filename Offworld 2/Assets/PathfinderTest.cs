using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderTest : MonoBehaviour
{
    public GameObject[] foundNodes;
    public Transform model;
    public List<GameObject> nodePath = new List<GameObject>();
    public LayerMask sightMask;
    public bool pathFind;
    public bool pathFound;
    public int currentNode;

    public Transform Target;
    // Start is called before the first frame update
    void Start()
    {
        foundNodes = GameObject.FindGameObjectsWithTag("Node");
    }

    // Update is called once per frame
    void Update()
    {


        if (pathFind)
        {
            nodePath.Clear();
            GetNodes(foundNodes, Target.position, transform.position);
            //nodePath.Add(GetNode(foundNodes, Target.position, transform.position));
        }

        if (pathFound)
        {
            Vector3 hitPos = new Vector3();
            if (Physics.Raycast(transform.position, Target.position - transform.position, out RaycastHit hit, (Target.position - transform.position).magnitude, sightMask))
            {
                Debug.Log(hitPos);
                hitPos = hit.point;
            }
            Debug.Log(hitPos);
            if (hitPos == Vector3.zero)
            {
                Debug.Log("Resume Normal Operation!");
                model.LookAt(Target.position);
                pathFound = false;
            }
            transform.Translate((nodePath[currentNode].transform.position - transform.position).normalized / 10);
            model.LookAt(nodePath[currentNode].transform.position);
            if((nodePath[currentNode].transform.position - transform.position).magnitude < 1)
            {
                currentNode -= 1;
            }
        }
    }

    //GameObject GetNode(GameObject[] nodes, Vector3 StartPosition, Vector3 TargetPosition)
    //{
    //    RaycastHit hit;
    //    Vector3 hitPos = new Vector3();
    //    int ClosestStartNode = 0;
    //    GameObject[] newNodes = FindAvailableNodes(foundNodes, StartPosition);
    //    ClosestStartNode = FindShortestDistance(newNodes, TargetPosition);
    //    Debug.DrawLine(StartPosition, TargetPosition, Color.blue);
    //    if (Physics.Raycast(StartPosition, TargetPosition - StartPosition, out hit, (TargetPosition - StartPosition).magnitude, sightMask))
    //    {

    //        hitPos = hit.point;
    //    }
    //    if(hitPos == Vector3.zero)
    //    {
    //        Debug.Log("Target Clear!");
    //        return newNodes[ClosestStartNode];
    //    }
    //    else
    //    {
    //        Debug.Log("Find Next Node!");
    //        nodePath.Add(GetNode(foundNodes, newNodes[ClosestStartNode].transform.position, TargetPosition));
    //    }
    //    return newNodes[0];
    //}

    void GetNodes(GameObject[] nodes, Vector3 StartPosition, Vector3 TargetPosition)
    {
        Vector3 hitPos = new Vector3();
        Vector3 nextNode = StartPosition;
        int ClosestStartNode = 0;
        //while (!pathFound)
        //{
        for (int i = 0; i < 20; i++)
        {
            hitPos = Vector3.zero;
            Debug.DrawLine(nextNode, TargetPosition, Color.blue);
            if (Physics.Raycast(nextNode, TargetPosition - nextNode, out RaycastHit hit, (TargetPosition - nextNode).magnitude, sightMask))
            {
                Debug.Log(hitPos);
                hitPos = hit.point;
            }
            Debug.Log(hitPos);
            if (hitPos != Vector3.zero)
            {
                Debug.Log("Find Next Node!");
                GameObject[] newNodes = FindAvailableNodes(foundNodes, nextNode);
                ClosestStartNode = FindShortestDistance(newNodes, TargetPosition);
                nextNode = newNodes[ClosestStartNode].transform.position;
                nodePath.Add(newNodes[ClosestStartNode]);
            }
            else
            {
                Debug.Log("Target Clear!");
                pathFound = true;
                pathFind = false;
                currentNode = nodePath.Count - 1;
                return;
            }
        }
        pathFind = false;
        //}
    }

    GameObject[] FindAvailableNodes(GameObject[] Nodes, Vector3 StartPosition)
    {
        List<GameObject> availableNodes = new List<GameObject>();
        for (int n = 0; n < Nodes.Length; n++)
        {
            Vector3 CurrentNode = Nodes[n].transform.position - StartPosition;
            Vector3 hitPos = new Vector3();
            if (Physics.Raycast(StartPosition, CurrentNode, out RaycastHit hit, CurrentNode.magnitude, sightMask))
            {
                hitPos = hit.point;
            }
            if (hitPos == Vector3.zero)
            {
                availableNodes.Add(Nodes[n]);
            }
        }
        GameObject[] newNodes = new GameObject[availableNodes.Count];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = availableNodes[n];
        }
        return newNodes;
    }

    int FindShortestDistance(GameObject[] Nodes, Vector3 StartPosition)
    {
        int ClosestNodeIndex = 0;
        for (int n = 0; n < Nodes.Length; n++)
        {
            Vector3 CurrentNode = StartPosition - Nodes[n].transform.position;
            Vector3 ClosestNode = StartPosition - Nodes[ClosestNodeIndex].transform.position;
            if (CurrentNode.magnitude < ClosestNode.magnitude)
            {
                bool cancel = false;
                foreach (GameObject node in nodePath) {
                    if (Nodes[n] == node)
                    {
                        cancel = true;
                    }
                }
                if (!cancel)
                {
                    ClosestNodeIndex = n;
                }
            }
        }

        return ClosestNodeIndex;
    }
}
