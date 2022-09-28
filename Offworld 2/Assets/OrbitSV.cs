using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitSV : MonoBehaviour
{
    public Transform orbittingBody;
    public float orbitalVelocity;
    public float altitude;

    void Start(){
        if(orbittingBody != null){
            float currentVSDesired = altitude - (orbittingBody.position - transform.position).magnitude;
            orbittingBody.position = orbittingBody.position + ((orbittingBody.position - transform.position).normalized * currentVSDesired);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(orbitalVelocity * Time.deltaTime, 0, 0);
        if(orbittingBody != null){
            orbittingBody.Rotate(-orbitalVelocity * Time.deltaTime, 0, 0);
        }
    }
}
