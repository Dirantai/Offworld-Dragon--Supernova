using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSystem2 : BasicForceSystem
{
    public Rigidbody shipRigid;
    public Transform shipTarget;
    public Transform FPSCameraRig;
    public GameObject deathExplosion;

    public bool visuals;
    public bool mutedVisuals;

    public MovementValues shipMovementValues;
    public MovementValues shipRotationalValues;
    public MissileSystem missiles;
    public GunTest gunSystem;

    [System.Serializable]
    public class ShipStats
    {
        public float shieldGrade;
        public float hullGrade;
        public float shieldDelay;
        public float shieldRechargeRate;
    }
    
    public float currentShieldHealth;
    public float currentHullHealth;

    public bool active;
    public ShipStats shipStats;

    float MouseLookSystem(float mouseInputAxis, int maxValue)
    {

        float screenCentre = maxValue / 2;

        float finalInput = (mouseInputAxis - screenCentre) / screenCentre / 2;

        return Mathf.Clamp(-finalInput, -1, 1);
    }

    private void Start()
    {
        active = true;

        Cursor.visible = false;
        thrusterEffects = gameObject.GetComponentsInChildren<ParticleSystem>();

        shipStats.hullGrade = Mathf.Clamp(shipStats.hullGrade / 10, 0.1f, 100);
        shipStats.shieldGrade = Mathf.Clamp(shipStats.shieldGrade / 10, 0.1f, 100);

        currentShieldHealth = 100;
        currentHullHealth = 100;

        OnStart();
    }

    public virtual void OnStart() { }

    public float GetHealth()
    {
        return currentHullHealth;
    }

    public float GetShield()
    {
        return currentShieldHealth;
    }

    void FixedUpdate()
    {
        if (active)
        {
            if (currentHullHealth <= 0)
            {
                OnDeath();
                Instantiate(deathExplosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            Vector3 movementInput = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("JumpThrust"));
            Vector3 rotationInput = new Vector3(Input.GetAxis("Roll"), MouseLookSystem(Input.mousePosition.y, Screen.height), -MouseLookSystem(Input.mousePosition.x, Screen.width));
            HandleMovement(movementInput, rotationInput);
            if (visuals) HandleVisuals(rotationInput);
        }
        else
        {
            HandleMovement(Vector3.zero, Vector3.zero);
        }
        OnUpdate();
        ShieldRegen();
    }

    float currentRegenTime;

    void ShieldRegen()
    {
        if(currentRegenTime > 0)
        {
            currentRegenTime -= Time.deltaTime;
        }
        else
        {
            currentRegenTime = 0;
            if(currentShieldHealth < 100)
            {
                currentShieldHealth += Time.deltaTime * shipStats.shieldRechargeRate;
            }
            else
            {
                currentShieldHealth = 100;
            }
        }
    }

    public virtual void OnUpdate()
    {
        if (Input.GetButton("Fire2") && missiles != null)
        {
            missiles.shipTarget = gunSystem.shipTarget;
            missiles.FireMissile();
        }
        boosting = Input.GetButton("Boost");
        if (Input.GetKeyDown(KeyCode.V))
        {
            decoupled = !decoupled;
        }
    }

    public bool boosting;

    void HandleVisuals(Vector3 rotationalInput)
    {
        float xOffset = MouseLookSystem(Input.mousePosition.x, Screen.width);
        float yOffset = MouseLookSystem(Input.mousePosition.y, Screen.height);
        float modifier = 1;
        if (mutedVisuals)
        {
            modifier = 25;
        }
        shipModel.localRotation = Quaternion.Lerp(shipModel.localRotation, Quaternion.Euler(yOffset * 25 / modifier, -xOffset * 30 / modifier, (xOffset * 50 + rotationalInput.x * 10) / modifier), 6 * Time.deltaTime);
        FPSCameraRig.localRotation = Quaternion.Lerp(FPSCameraRig.localRotation, Quaternion.Euler(yOffset * 25 / modifier, -xOffset * 30 / modifier, 0), 6 * Time.deltaTime);
        float xPosOffset = Vector3.Dot(transform.right, shipRigid.velocity);
        float yPosOffset = Vector3.Dot(transform.up, shipRigid.velocity);
        float zPosOffset = Vector3.Dot(transform.forward, shipRigid.velocity);

        Vector3 visualVector = new Vector3(xPosOffset * 2, yPosOffset * 2, zPosOffset * 2);

        shipModel.localPosition = Vector3.Lerp(shipModel.localPosition,  (visualVector / modifier) / 10, 6 * Time.deltaTime);
    }
    private bool reactivateDecouple;
    public virtual void HandleMovement(Vector3 movementInput, Vector3 rotationInput)
    {

        SetMovementValues(shipMovementValues);
        SetVelocity(shipRigid.velocity);
        Vector3 maxSpeedVector = new Vector3(shipMovementValues.maxSpeedVector.x, shipMovementValues.maxSpeedVector.y, shipMovementValues.maxSpeedVector.z);
        if (boosting)
        {
            maxSpeedVector = maxSpeedVector * shipMovementValues.speedMultiplier;
        }
        
        Vector3 finalVector = CalculateFinalInput(movementInput, maxSpeedVector);

        shipRigid.AddForce(finalVector);

        if (decoupled)
        {
            decoupled = false;
            reactivateDecouple = true;
        }

        SetMovementValues(shipRotationalValues);
        SetVelocity(shipRigid.angularVelocity);
        maxSpeedVector = new Vector3(shipRotationalValues.maxSpeedVector.x, shipRotationalValues.maxSpeedVector.y * Mathf.Abs(MouseLookSystem(Input.mousePosition.y, Screen.height)), shipRotationalValues.maxSpeedVector.z * Mathf.Abs(MouseLookSystem(Input.mousePosition.x, Screen.width)));


        shipRigid.AddTorque(CalculateFinalInput(rotationInput, maxSpeedVector));

        if (reactivateDecouple)
        {
            reactivateDecouple = false;
            decoupled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Bullet")
        {
            OnDamage(collision.transform.GetComponent<Bullet>().Damage);
        }
    }

    public void OnDamage(float damage)
    {
        if (currentShieldHealth > 0)
        {
            currentShieldHealth -= damage / shipStats.shieldGrade;
            currentRegenTime = shipStats.shieldDelay;
        }
        else
        {
            currentShieldHealth = 0;
            currentHullHealth -= damage / shipStats.hullGrade;
        }
    }

    public virtual void OnDeath()
    {
        currentHullHealth = 0;
        Transform camRig = GameObject.FindGameObjectWithTag("CamRig").transform;
        camRig.transform.parent = null;
    }
}
