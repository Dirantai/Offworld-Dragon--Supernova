using UnityEngine;
using System.Collections;

public class HitMarkerSystem : MonoBehaviour {

    public bool Hit;
    public float HitShownTime; //Time the hit marker is shown for
    public GameObject HitMarker; //The object
    float HitTime; //Current time it has appeared for

	// Use this for initialization
	void Start () {
        HitMarker.SetActive(true);
        Hit = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Hit) //Check if the player has hit
        {
            HitMarker.SetActive(true); //Show the object
            Hit = false; //set it to false
            HitTime = HitShownTime; //Set the Current time to the max shown time.
        }
        if(HitTime > 0) //Check if the time > 0
        {
            HitTime -= Time.deltaTime; //Decrease the timer
        }
        else //Once <= 0
        {
            HitMarker.SetActive(false); //hide object
            HitTime = 0; //reset for clarity
        }

	}
}
