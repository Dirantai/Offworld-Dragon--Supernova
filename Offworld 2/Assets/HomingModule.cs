using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class HomingModule : MonoBehaviour
{

    public Transform target;
    public Bullet bulletModule;
    public float turnSpeed;
    public float homingLifetime;

    private float timer;

    void Start()
    {
        timer = homingLifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }

            InterceptionSystem interceptor = new InterceptionSystem();
            Vector3 aimPoint = interceptor.CalculateInterceptPosition(target.position, target.GetComponent<Rigidbody>().velocity, transform.position, bulletModule.BulletVelocity);
            //float passed = Vector3.Dot(aimPoint, aimPoint - transform.position);
            //if(passed > 0)
            //{
            transform.Rotate(AimAtTarget(aimPoint) * turnSpeed * Mathf.Clamp(timer / homingLifetime, 0.1f, 1));
            //}
        }
    }

    Vector3 AimAtTarget(Vector3 target)
    {
        float horizontalProduct = Vector3.Dot(transform.right, (target - transform.position));
        float verticalProduct = Vector3.Dot(-transform.up, (target - transform.position));
        return new Vector3(Mathf.Clamp(verticalProduct, -1, 1), Mathf.Clamp(horizontalProduct, -1, 1), 0);
    }
}
