using UnityEngine;
using System.Collections.Generic;

public class NodeData : MonoBehaviour {
    public NodeData[] NextNodes;

    private void Start()
    {
        //transform.GetComponent<MeshRenderer>().enabled = false;
        NodeData[] FoundNodes = FindObjectsOfType<NodeData>();
        List<NodeData> NextNodesList = new List<NodeData>();
        for (int i = 0; i < FoundNodes.Length; i++)
        {
            if (FoundNodes[i] != this)
            {
                Transform HitObject = null;
                Vector3 CurrentNodeToNextNode = FoundNodes[i].transform.position - transform.position;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, CurrentNodeToNextNode, out hit))
                {
                    HitObject = hit.transform;
                }
                if (HitObject == FoundNodes[i].transform)
                {
                    NextNodesList.Add(FoundNodes[i]);
                }
            }
        }
        NextNodes = new NodeData[NextNodesList.Count];
        for(int n = 0;n< NextNodesList.Count; n++)
        {
            NextNodes[n] = NextNodesList[n];
        }
    }

    void Update()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < NextNodes.Length; i++)
        {
            Gizmos.DrawLine(transform.position, NextNodes[i].transform.position);
        }
    }

}
