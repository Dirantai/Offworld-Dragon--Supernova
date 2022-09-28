using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

    [System.Serializable]
    public class WeaponValues
    {
        public float baseDamage;
        public float projectileSpeed;
        public float fireRate;
        public float spread;
        public GameObject bulletModel;
        public Transform Barrel;
        public float turnSpeed;
        public bool loaded;
        public HitMarkerSystem hitMarker;
    }

    
    [System.Serializable]
    public class Parts
    {
        public Transform Base;
        public float MaxTurnLimit;
        public float MinTurnLimit;
        public float Angle;
    }
    public WeaponValues weaponValues;
    public GameObject reticleObject;
    public GameObject indecatorReticleObject;
    public Transform reticleParent;
    public UIElementSystem reticle;
    public Transform indecatorReticle;

    public Parts[] TurretParts;
   
    public ParticleSystem Flash;
    public bool CanFire;

    private float Timer;

    private void Start()
    {
        if (reticleObject != null && indecatorReticleObject != null)
        {
            GameObject tempObj = Instantiate(reticleObject, reticleParent) as GameObject;
            reticle = tempObj.GetComponent<UIElementSystem>();
            tempObj = Instantiate(indecatorReticleObject, reticleParent) as GameObject;
            indecatorReticle = tempObj.transform;
            weaponValues.hitMarker = reticle.GetComponent<HitMarkerSystem>();
        }
        weaponValues.loaded = true;
        
    }

    private void Update()
    {
        if (CanFire)
        {
            HandleFireRate();
        }
        else
        {
            weaponValues.loaded = false;
        }
    }

    public void HandleUI(float distance)
    {
        if (reticle != null)
        {
            reticle.iconPosition = weaponValues.Barrel.position + weaponValues.Barrel.forward * distance;
        }
    }

    void HandleFireRate()
    {
        float delay = 60 / weaponValues.fireRate;
        if (!weaponValues.loaded)
        {
            if (Timer >= delay)
            {
                Timer = 0;
                weaponValues.loaded = true;
            }
            else
            {
                Timer += Time.deltaTime;
            }
        }
    }

    // Update is called once per frame
    public void TurretTurn (Vector3 AimPoint, float Sensitivity) {
        float Angle = Vector3.Angle(TurretParts[1].Base.forward, AimPoint - weaponValues.Barrel.position);
        ApplyRotation(TurretParts[0].Base, false, TurretParts[0].Base.right, TurretParts[0].MaxTurnLimit, TurretParts[0].MinTurnLimit, ref TurretParts[0].Angle, AimPoint, Sensitivity);
        ApplyRotation(TurretParts[1].Base, true, TurretParts[1].Base.up, TurretParts[1].MaxTurnLimit, TurretParts[1].MinTurnLimit, ref TurretParts[1].Angle, AimPoint, Sensitivity);
        if (Angle > 5)
        {
            CanFire = false;
        }
        else
        {
            CanFire = true;
        }
    }

    void ApplyRotation(Transform Base, bool Barrel, Vector3 PerpendicularVector, float MaxTurnLimit, float MinTurnLimit, ref float Angle, Vector3 AimPoint, float Sensitivity)
    {
        float Displacement = Vector3.Dot(PerpendicularVector * 1000 / (AimPoint * 1000 - Base.position * 1000).magnitude, AimPoint - Base.position);
        Angle += Displacement * Sensitivity;
        Angle = Mathf.Clamp(Angle, MinTurnLimit, MaxTurnLimit);
        if (!Barrel)
        {
            Base.localRotation = Quaternion.Euler(0, Angle, 0);
        }
        else
        {
            Base.localRotation = Quaternion.Euler(-Angle, 0, 0);
        }
    }
}
