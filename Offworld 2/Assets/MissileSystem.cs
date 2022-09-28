using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{

    public Turret.WeaponValues missileValues;
    public Transform shipTarget;

    private float Timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FireMissile(Transform shipTarget)
    {
        if (missileValues.loaded)
        {
            GameObject instancedBullet = Instantiate(missileValues.bulletModel, missileValues.Barrel.position, transform.rotation) as GameObject;
            instancedBullet.GetComponent<Bullet>().BulletVelocity = missileValues.projectileSpeed;
            instancedBullet.GetComponent<Bullet>().Damage = missileValues.baseDamage;
            instancedBullet.GetComponent<Bullet>().HitMarkerDetector = missileValues.hitMarker;
            instancedBullet.GetComponent<HomingModule>().target = shipTarget;
            missileValues.loaded = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        float delay = 60 / missileValues.fireRate;
        if (!missileValues.loaded)
        {
            if (Timer >= delay)
            {
                Timer = 0;
                missileValues.loaded = true;
            }
            else
            {
                Timer += Time.deltaTime;
            }
        }
    }
}
