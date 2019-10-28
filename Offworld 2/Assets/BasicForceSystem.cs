using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicForceSystem : MonoBehaviour
{
    private Vector3 velocity;

    [System.Serializable]
    public class MovementValues
    {
        //KeyCode:
        //Alpha = forward and backwards vectors
        //Beta = left and right vectors
        //Gamma = up and down vectors

        public float maxAlphaSpeed;
        public float maxBetaSpeed;
        public float maxGammaSpeed;

        public float inputSlowdownMultiplier; //Used for then there are no inputs and the ship needs to slow down on itself (used so it doesn't just come to an immediate stop and is a nice smooth slowdown)

        public float alphaForce;
        public float betaForce;
        public float gammaForce;

        [System.Serializable]
        public class ThrusterEffects //My god this is horrible, look away
        {
            public GameObject positiveEffects;
            public GameObject negativeEffects;

            public void HandleInput(float inputReceived)
            {
                if (positiveEffects != null && negativeEffects != null) //this tries to fire certain thruster effects based on an input, which is usually the direction the ship is applying force in.
                {
                    if (inputReceived > 0.1f)
                    {
                        ParticleSystem[] thrusterEffects = positiveEffects.GetComponentsInChildren<ParticleSystem>();
                        foreach (ParticleSystem effect in thrusterEffects)
                        {
                            effect.Play();
                        }
                        thrusterEffects = negativeEffects.GetComponentsInChildren<ParticleSystem>();
                        foreach (ParticleSystem effect in thrusterEffects)
                        {
                            effect.Stop();
                        }
                    }
                    else if (inputReceived < -0.1f)
                    {
                        ParticleSystem[] thrusterEffects = positiveEffects.GetComponentsInChildren<ParticleSystem>();
                        foreach (ParticleSystem effect in thrusterEffects)
                        {
                            effect.Stop();
                        }
                        thrusterEffects = negativeEffects.GetComponentsInChildren<ParticleSystem>();
                        foreach (ParticleSystem effect in thrusterEffects)
                        {
                            effect.Play();
                        }
                    }
                    else
                    {
                        ParticleSystem[] thrusterEffects = positiveEffects.GetComponentsInChildren<ParticleSystem>();
                        foreach (ParticleSystem effect in thrusterEffects)
                        {
                            effect.Stop();
                        }
                        thrusterEffects = negativeEffects.GetComponentsInChildren<ParticleSystem>();
                        foreach (ParticleSystem effect in thrusterEffects)
                        {
                            effect.Stop();
                        }
                    }
                }
            }
        }

        public ThrusterEffects alphaEffect;
        public ThrusterEffects betaEffect;
        public ThrusterEffects gammaEffect;
    }

    private MovementValues movementValues = new MovementValues();

    public Vector3 CalculateFinalInput(Vector3 inputVector, Vector3 maxSpeedVector) //receives a vector holding the inputs and a vector of the maximum speeds in each axis
    {
        //calculates in which directions the ship needs to thrust depending on player input.
        Vector3 forwardThrustVector = HandleForce(transform.forward, inputVector.x, movementValues.alphaForce, maxSpeedVector.x, movementValues.alphaEffect);
        Vector3 lateralThrustVector = HandleForce(transform.right, inputVector.y, movementValues.betaForce, maxSpeedVector.y, movementValues.betaEffect);
        Vector3 verticalThrustVector = HandleForce(transform.up, inputVector.z, movementValues.gammaForce, maxSpeedVector.z, movementValues.gammaEffect);
        Vector3 finalInput = forwardThrustVector + lateralThrustVector + verticalThrustVector;
        return finalInput; //returns a final physics vector for unity's rigidbody to use.
    }

    public void SetVelocity(Vector3 receivedVelocity)
    {
        velocity = receivedVelocity; //sets the velocity of the ship
    }

    public void SetMovementValues(MovementValues receivedMovementValues)
    {
        movementValues = receivedMovementValues; //some jank to get this working
    }

    Vector3 HandleForce(Vector3 shipAxisDirection, float playerInput, float force, float maxValue, MovementValues.ThrusterEffects thrusterEffects) //using the power of dot product, used to figure out slowing down and flying.
    {
        float velocityDotProduct = Vector3.Dot(shipAxisDirection, velocity); //get a dot product of the the axis of the ship and the direction it is travelling
        float localPlayerInput = playerInput * 2; //the multiplication is really a buffer, separates the player input in another variable to be used for the actual input (for visual reasons)

        if (Mathf.Abs(velocityDotProduct) > maxValue) //check if the ship has reached cruise speed
        {
            localPlayerInput = 0; //set the player input to 0, as it should stop receiving inputs in this direction
        }

        float inputDirection = localPlayerInput + Mathf.Clamp(-velocityDotProduct * Mathf.Clamp(movementValues.inputSlowdownMultiplier, 0, 1), -1, 1); // calculate the direction the ship needs to thrust in. as you can see it takes both player input and counter thrust into consideration.

        thrusterEffects.HandleInput(playerInput + inputDirection); //check which visual thrusters to fire.

        return shipAxisDirection *  Mathf.Clamp(inputDirection, -1, 1) * force; //return a velocity vector in with the right direction and force.
    }
}
