using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSystem : MonoBehaviour
{

    public Rigidbody shipRigid;
    
    [System.Serializable]
    public class MovementValues
    {
        public float maxForwardSpeed;
        public float maxLateralSpeed;
        public float maxVerticalSpeed;

        public float zeroInputSlowdownMultiplier;
        public float maxInputSlowdownMultiplier;

        public float forwardThrust;
        public float lateralThrust;
        public float verticalThrust;

    }
    [System.Serializable]
    public class RotationalValues
    {
        public float maxPitchRate;
        public float maxYawRate;
        public float maxRollRate;

        public float zeroInputSlowdownMultiplier;
        public float maxInputSlowdownMultiplier;

        public float pitchTorque;
        public float yawTorque;
        public float rollTorque;
    }

    [System.Serializable]
    public class MovementSystem
    {
        public float maxPitchRate;
        public float maxYawRate;
        public float maxRollRate;

        public float zeroInputSlowdownMultiplier;
        public float maxInputSlowdownMultiplier;

        public float pitchTorque;
        public float yawTorque;
        public float rollTorque;
    }

    public MovementValues shipMovementValues;
    public RotationalValues shipRotationalValues;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardThrustVector = HandleThruster(transform.forward, Input.GetAxis("Vertical"), shipMovementValues.forwardThrust, shipMovementValues.maxForwardSpeed, shipRigid.velocity, shipMovementValues.zeroInputSlowdownMultiplier, shipMovementValues.maxInputSlowdownMultiplier);
        Vector3 lateralThrustVector = HandleThruster(transform.right, Input.GetAxis("Horizontal"), shipMovementValues.lateralThrust, shipMovementValues.maxLateralSpeed, shipRigid.velocity, shipMovementValues.zeroInputSlowdownMultiplier, shipMovementValues.maxInputSlowdownMultiplier);
        Vector3 verticalThrustVector = HandleThruster(transform.up, Input.GetAxis("JumpThrust"), shipMovementValues.verticalThrust, shipMovementValues.maxVerticalSpeed, shipRigid.velocity, shipMovementValues.zeroInputSlowdownMultiplier, shipMovementValues.maxInputSlowdownMultiplier);
        Vector3 finalInput = forwardThrustVector + lateralThrustVector + verticalThrustVector;
        shipRigid.AddForce(finalInput);

        Vector3 pitchVector = HandleThruster(transform.right, MouseLookSystem(Input.mousePosition.y, Screen.height), shipRotationalValues.pitchTorque, shipRotationalValues.maxPitchRate * Mathf.Abs(MouseLookSystem(Input.mousePosition.y, Screen.height)), shipRigid.angularVelocity, shipRotationalValues.zeroInputSlowdownMultiplier, shipRotationalValues.maxInputSlowdownMultiplier);
        Vector3 rollVector = HandleThruster(transform.forward, Input.GetAxis("Roll"), shipRotationalValues.rollTorque, shipRotationalValues.maxRollRate, shipRigid.angularVelocity, shipRotationalValues.zeroInputSlowdownMultiplier, shipRotationalValues.maxInputSlowdownMultiplier);
        Vector3 yawVector = HandleThruster(-transform.up, MouseLookSystem(Input.mousePosition.x, Screen.width), shipRotationalValues.yawTorque, shipRotationalValues.maxYawRate * Mathf.Abs(MouseLookSystem(Input.mousePosition.x, Screen.width)), shipRigid.angularVelocity, shipRotationalValues.zeroInputSlowdownMultiplier, shipRotationalValues.maxInputSlowdownMultiplier);
        finalInput = pitchVector + rollVector + yawVector;
        shipRigid.AddTorque(finalInput);
    }

    float MouseLookSystem(float mouseInputAxis, int maxValue)
    {
        float finalInput = new float();

        float screenCentre = maxValue / 2;

        finalInput = (mouseInputAxis - screenCentre) / screenCentre / 2;

        return Mathf.Clamp(-finalInput, -1, 1);
    }

    Vector3 HandleThruster(Vector3 shipAxisDirection, float playerInput, float force, float maxValue, Vector3 velocity, float zeroInputSlowdownModifier, float maxInputSlowsownModifier)
    {
        float velocityDotProduct = Vector3.Dot(shipAxisDirection, velocity);
        Vector3 finalInput = new Vector3();

        if (Mathf.Abs(velocityDotProduct) < maxValue)
        {
            finalInput = shipAxisDirection * playerInput;
        }
        else
        {
            finalInput = shipAxisDirection * -velocityDotProduct * maxInputSlowsownModifier;
        }

        if(playerInput == 0)
        {
            finalInput = -shipAxisDirection * (velocityDotProduct / (maxValue + 0.1f) ) * zeroInputSlowdownModifier;
        }

        return finalInput * force;
    }
}
