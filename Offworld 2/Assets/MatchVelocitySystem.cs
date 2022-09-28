using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchVelocitySystem : MonoBehaviour
{
    public Transform player;
    public float velocityMatchDistance;
    public float rotationMatchDistance;

    // Update is called once per frame
    void Update()
    {
        float distance = (player.position - transform.position).magnitude;
        if(distance < velocityMatchDistance){
            if(distance < rotationMatchDistance){
                player.parent = transform;
            }else{
                player.parent = transform.parent;
            }
        }else{
            if(player.parent != null){
                player.parent = null;
            }
        }
    }
}
