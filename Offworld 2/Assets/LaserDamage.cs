using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDamage : MonoBehaviour
{

    public LayerMask laserMask;
    public float laserDamage;
    public float damageInterval;
    private GameObject target;
    private float timer;
    private float decayTimer;
    

    private void Start()
    {
        Destroy(gameObject, 3);
    }

    

    public void SetTarget(GameObject currentTarget)
    {
        target = currentTarget;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hitPos = Vector3.zero;
        float widthMultiplier = 1;

        if (decayTimer > 2.5f)
        {
            widthMultiplier -= (decayTimer - 2.5f) / 0.5f;
            transform.GetChild(0).localScale = new Vector3(15 * widthMultiplier, 3000, 15 * widthMultiplier);
        }

        if (Physics.SphereCast(transform.position, 1 * widthMultiplier, transform.forward, out RaycastHit rayHit, 9000, laserMask))
        {
            hitPos = rayHit.point;
            //target = rayHit.collider.gameObject;
        }

        decayTimer += Time.deltaTime;



        if(hitPos != Vector3.zero)
        {
            if (timer <= 0)
            {
                timer = damageInterval;
                target.GetComponent<ShipSystem2>().OnDamage(laserDamage);
            }
        }
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
}
