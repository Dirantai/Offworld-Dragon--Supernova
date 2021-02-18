using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public float DecayTime; //Time take till object is removed
    public float Damage;
    public Rigidbody Rigid; //The physics object
    public float BulletVelocity;
    public HitMarkerSystem HitMarkerDetector; //The class that the bullet tells if it hits an enemy
    public bool BypassShields;
    public GameObject CollisionEffect; //The effect that is shown when the bullet collides
    public Transform effect;

    float ElapsedTime = 0; //This will increment in real time.

    void FixedUpdate () {
        transform.GetChild(0).transform.localScale = new Vector3(0.1f, 2 * (BulletVelocity / 100), 0.1f);
        Rigid.velocity = transform.forward * BulletVelocity; //Sets the velocity of the physics object
        ElapsedTime += Time.deltaTime; //Increment time (in seconds)
        if(ElapsedTime >= DecayTime) //Check if the bullet should expire
        {
            Destroy(gameObject); //Expire.
        }
    }

    private void OnCollisionEnter(Collision collision) //Checks if the bullet collides with anything.
    {
        if (HitMarkerDetector != null) //Checks if the class is assigned (if not it's an enemy bullet)
        {
            if (collision.transform.tag == "Enemy") //check if the object is an enemy
            {
                HitMarkerDetector.Hit = true; //Show the hit marker.
            }
        }
        if (CollisionEffect != null) //Check if there his a hit effect
        {
            Instantiate(CollisionEffect, collision.contacts[0].point, transform.rotation); //If so, spawn the hit.
        }
        if(effect != null)
        {
            effect.parent = null;
        }
        Destroy(gameObject); //Destroy the Bullet.
    }
}
