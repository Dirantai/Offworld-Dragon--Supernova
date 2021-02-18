using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSystem : MonoBehaviour
{

    public MeshFilter meshFilter;
    public Mesh mesh;
    public class NodeData
    {
        public Vector3 nodePosition;
        public Vector3[] connectedNodes;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
