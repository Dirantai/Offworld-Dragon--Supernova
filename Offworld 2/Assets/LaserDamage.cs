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
    public float decay;
    

    private void Start()
    {
        Destroy(gameObject, decay);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hitPos = Vector3.zero;
        float widthMultiplier = 1;

        if (decayTimer > decay - 0.5f)
        {
            widthMultiplier -= (decayTimer - (decay - 0.5f)) / 0.5f;
            transform.GetChild(0).localScale = new Vector3(15 * widthMultiplier, 3000, 15 * widthMultiplier);
        }

        if (Physics.SphereCast(transform.position, 0.5f * widthMultiplier, transform.forward, out RaycastHit rayHit, 1000, laserMask))
        {
            hitPos = rayHit.point;
            target = rayHit.collider.gameObject;
        }

        decayTimer += Time.deltaTime;



        if(hitPos != Vector3.zero)
        {
            ShipSystem2 temp = target.gameObject.GetComponentInParent<ShipSystem2>();
            if (temp != null)
            {
                if (timer <= 0)
                {
                    timer = damageInterval;
                    temp.OnDamage(laserDamage);
                }
            }
        }
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
}
