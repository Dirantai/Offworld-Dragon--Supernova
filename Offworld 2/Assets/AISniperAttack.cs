using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISniperAttack : MonoBehaviour
{

    public bool charging;
    public bool prime;
    public float charge;
    public float chargeRate;
    public Transform barrel;
    public GameObject laserWarningEffect;
    public GameObject laserFire;
    public ShipSystem2 shipController;
    Vector3 aimPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(charge < 100)
        {
            charge += Time.deltaTime * chargeRate;
        }
        else
        {
            charge = 0;
            charging = false;
            prime = false;
        }
        

        if (!prime)
        {
            aimPoint = shipController.shipTarget.position + (shipController.shipTarget.GetComponent<Rigidbody>().velocity);
        }

        if (charging)
        {
            laserWarningEffect.SetActive(true);
            laserWarningEffect.transform.LookAt(aimPoint);
        }
        else
        {
            laserWarningEffect.SetActive(false);
        }

        Debug.DrawLine(barrel.position, aimPoint, Color.yellow);
        if (charge > 50)
        {
            charging = true;
        }

        if (charge > 95)
        {
            prime = true;
        }
    }
}
