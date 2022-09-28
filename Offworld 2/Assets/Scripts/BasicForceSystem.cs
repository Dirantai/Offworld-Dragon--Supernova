
using UnityEngine;

public class BasicForceSystem : MonoBehaviour
{
    private Vector3 velocity;
    public Transform shipModel;
    public bool decoupled;

    [System.Serializable]
    public class MovementValues
    {

        public Vector3 maxSpeedVector;
        public Vector3 maxForceVector;

        public float inputSlowdownMultiplier; //Used for then there are no inputs and the ship needs to slow down on itself (used so it doesn't just come to an immediate stop and is a nice smooth slowdown)

        public float speedMultiplier;
        public float forceMultiplier;
    }

    public Transform orbittingBody;

    private MovementValues movementValues = new MovementValues();
    public bool boosting;
    public float boostDuration;
    public float currentBoost;

    private Vector3 angularVelocity;

    private ParticleSystem[] thrusterEffects;

    public void setThrusters(ParticleSystem[] thrusters){
        thrusterEffects = thrusters;
    }

    void HandleThrusterEffects(Vector3 shipAxisDirection, float inputReceived, bool canBoost)
    {
        foreach (ParticleSystem thruster in thrusterEffects)
        {
            float direction = Vector3.Dot(-thruster.transform.forward, shipAxisDirection * inputReceived);
            float boostMultiplier = 1;

            if(!boosting && boostDuration > 0 && canBoost){
                boostMultiplier = 5;
            }

            if (direction > 0.01f)
            {
                ParticleSystem.MainModule mainTest = thruster.main;
                mainTest.startSizeXMultiplier = 200 * Mathf.Clamp(Mathf.Abs(inputReceived), 0, 1);
                mainTest.startSizeYMultiplier = 200 * Mathf.Clamp(Mathf.Abs(inputReceived), 0, 1);
                mainTest.startSizeZMultiplier = 750 * Mathf.Clamp(Mathf.Abs(inputReceived * boostMultiplier), 0, 1 * boostMultiplier);
                thruster.Play();
            }
        }
    }

    private void HandleThrusterEffects(Vector3 inputReceived)
    {
        foreach (ParticleSystem thruster in thrusterEffects)
        {
            CheckThrusterRotation(thruster, shipModel.up, thruster.transform.localPosition.x, inputReceived.x);
            CheckThrusterRotation(thruster, -shipModel.up, thruster.transform.localPosition.z, inputReceived.y);
            CheckThrusterRotation(thruster, shipModel.right, thruster.transform.localPosition.z, inputReceived.z);
        }
    }

    void CheckThrusterRotation(ParticleSystem thruster, Vector3 rotationDirection, float shipAxis, float inputReceived){
            float roll = 0;
            if(shipAxis > 0){
                roll = Vector3.Dot(-thruster.transform.forward, rotationDirection * inputReceived);
            }else{
                roll = Vector3.Dot(thruster.transform.forward, rotationDirection * inputReceived);
            }

            if (roll > 0.01f)
            {
                ParticleSystem.MainModule mainTest = thruster.main;
                mainTest.startSizeXMultiplier = 200 * Mathf.Clamp(Mathf.Abs(inputReceived * 2), 0, 1);
                mainTest.startSizeYMultiplier = 200 * Mathf.Clamp(Mathf.Abs(inputReceived * 2), 0, 1);
                mainTest.startSizeZMultiplier = 750 * Mathf.Clamp(Mathf.Abs(inputReceived * 2), 0, 1);
                thruster.Play();
            }
    }

    public Vector3 CalculateFinalInput(Vector3 inputVector, Vector3 maxSpeedVector, Vector3 maxForceVector) //receives a vector holding the inputs and a vector of the maximum speeds in each axis
    {
        //calculates in which directions the ship needs to thrust depending on player input.
        Vector3 forwardThrustVector = HandleForce(shipModel.forward, inputVector.x, maxForceVector.x * 1000, maxSpeedVector.x, true);
        Vector3 lateralThrustVector = HandleForce(shipModel.right, inputVector.y, maxForceVector.y * 1000, maxSpeedVector.y, false);
        Vector3 verticalThrustVector = HandleForce(shipModel.up, inputVector.z, maxForceVector.z * 1000, maxSpeedVector.z, false);

        Vector3 finalInput = forwardThrustVector + lateralThrustVector + verticalThrustVector;
        return finalInput; //returns a final physics vector for unity's rigidbody to use.
    }

    public Vector3 CalculateFinalInput(Vector3 inputVector, Vector3 maxSpeedVector, Vector3 maxForceVector, float inputVisualScale){
        float pitch = CalculateRotationInput(maxForceVector.y, inputVector.y, angularVelocity.x);
        float yaw = CalculateRotationInput(maxForceVector.z, inputVector.z, angularVelocity.y);
        float roll = CalculateRotationInput(maxForceVector.x,inputVector.x, angularVelocity.z);

        Vector3 finalVector = new Vector3(pitch, yaw, roll);
        angularVelocity += finalVector;

        angularVelocity.x = LimitSpeed(angularVelocity.x, maxSpeedVector.x);
        angularVelocity.y = LimitSpeed(angularVelocity.y, maxSpeedVector.y);
        angularVelocity.z = LimitSpeed(angularVelocity.z, maxSpeedVector.z);

        HandleThrusterEffects(inputVector * inputVisualScale);

        return angularVelocity;
    }

    public void SetVelocity(Vector3 receivedVelocity)
    {
        velocity = receivedVelocity; //sets the velocity of the ship
    }

    public void SetMovementValues(MovementValues receivedMovementValues)
    {
        movementValues = receivedMovementValues; //some jank to get this working
    }

    private float CalculateRotationInput(float force, float input, float currentSpeed){
        float slowdownMultiplier = movementValues.inputSlowdownMultiplier;
        float localPlayerInput = input * 2;
        input = localPlayerInput + Mathf.Clamp(-(currentSpeed) * Mathf.Clamp(slowdownMultiplier, 0, 1), -1, 1);
        return input * force * Time.fixedDeltaTime;
    }

    private float LimitSpeed(float currentSpeed, float maxSpeed){
        if(Mathf.Abs(currentSpeed) > maxSpeed){
            currentSpeed = maxSpeed * (currentSpeed / Mathf.Abs(currentSpeed));
        }
        return currentSpeed;
    }

    Vector3 HandleForce(Vector3 shipAxisDirection, float playerInput, float force, float maxValue, bool canBoost) //using the power of dot product, used to figure out slowing down and flying.
    {
        float velocityDotProduct = Vector3.Dot(shipAxisDirection, velocity); //get a dot product of the the axis of the ship and the direction it is travelling
        float localPlayerInput = playerInput * 2; //the multiplication is really a buffer, separates the player input in another variable to be used for the actual input (for visual reasons)
        float slowdownMultiplier = movementValues.inputSlowdownMultiplier;
        float gravityVelocityDot = 0;
        float gravityDotProduct = 0;

        if(orbittingBody != null && !decoupled){
            gravityVelocityDot = Mathf.Clamp(Vector3.Dot(velocity, (orbittingBody.position - shipModel.position).normalized), -1, 1);
            if(gravityVelocityDot > 0){
                gravityDotProduct = Mathf.Clamp(Vector3.Dot(shipAxisDirection, (orbittingBody.position - shipModel.position).normalized), -1, 1);
            }
        }

        if (decoupled) slowdownMultiplier = 0;

        if (Mathf.Abs(velocityDotProduct) > maxValue || velocity.magnitude > maxValue) //check if the ship has reached cruise speed
        {
            if(localPlayerInput > 0 && velocityDotProduct > 0 || localPlayerInput < 0 && velocityDotProduct < 0)
            {
                localPlayerInput = 0; //set the player input to 0, as it should stop receiving inputs in this direction
            }
        }
    
        float inputDirection = localPlayerInput + Mathf.Clamp(-(velocityDotProduct * Mathf.Clamp(slowdownMultiplier, 0, 1)), -1, 1); // calculate the direction the ship needs to thrust in. as you can see it takes both player input and counter thrust into consideration.

        HandleThrusterEffects(shipAxisDirection, playerInput + inputDirection, canBoost);

        // + (shipAxisDirection * (-gravityDotProduct) * 10000)

        return shipAxisDirection * Mathf.Clamp(inputDirection, -1, 1) * force; //return a velocity vector in with the right direction and force.
    }
}