using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeusCannonULT : MonoBehaviour
{

    public Turret turret;
    public GunTest gunSystem;
    public float coolDownTime;

    private float timer;
    private int selectedEnemy;
    public bool ready;
    public bool active;
    private Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0;
            ready = true;
        }

        if (Input.GetKeyDown(KeyCode.F) && ready)
        {
            ready = false;
            active = true;
            timer = coolDownTime;
        }

        if (active)
        {
            turret.CanFire = true;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (selectedEnemy < 3)
            {
                if (enemies.Length >= 1)
                {
                    position = enemies[selectedEnemy].transform.position;
                    StartCoroutine("TargetEnemy");
                }
                else
                {
                    
                    StartCoroutine("TargetEnemy");
                }
            }
            else
            {
                selectedEnemy = 0;
                active = false;
            }
        }
        else
        {
            turret.CanFire = false;
        }
    }

    IEnumerator TargetEnemy()
    {
        turret.TurretTurn(position, 30);
        yield return new WaitForSeconds(1);
        gunSystem.GunShoot(turret.weaponValues, true);
        yield return new WaitForSeconds(1);
        gunSystem.GunShoot(turret.weaponValues, false);
        yield return new WaitForSeconds(1);
        selectedEnemy++;
    }
}
