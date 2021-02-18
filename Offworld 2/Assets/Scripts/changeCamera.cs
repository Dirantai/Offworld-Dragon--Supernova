using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeCamera : MonoBehaviour
{

    public Transform thirdPersonTrans;
    public Transform firstPersonTrans;
    public Transform cameraRig;
    public ShipSystem2 playerShip;
    private bool firstPerson;

    // Start is called before the first frame update
    void Start()
    {
        cameraRig.parent = thirdPersonTrans;
        cameraRig.localPosition = Vector3.zero;
        playerShip.mutedVisuals = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            firstPerson = !firstPerson;
        }

        if (firstPerson)
        {
            //playerShip.mutedVisuals = true;
        }
        else
        {
            //playerShip.mutedVisuals = false;
        }
    }
}
