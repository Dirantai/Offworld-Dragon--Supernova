using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicForceSystem : MonoBehaviour
{
    private Vector3 velocity;

    [System.Serializable]
    public class MovementValues
    {
        public float maxAlphaSpeed;
        public float maxBetaSpeed;
        public float maxGammaSpeed;

        public float inputSlowdownMultiplier;

        public float alphaForce;
        public float betaForce;
        public float gammaForce;

        [System.Serializable]
        public class ThrusterEffects
        {
            public GameObject positiveEffects;
            public GameObject negativeEffects;

            public void HandleInput(float inputReceived)
            {
                if (positiveEffects != null && negativeEffects != null)
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

    public Vector3 CalculateFinalInput(Vector3 inputVector, Vector3 maxSpeedVector)
    {
        Vector3 forwardThrustVector = HandleForce(transform.forward, inputVector.x, movementValues.alphaForce, maxSpeedVector.x, movementValues.alphaEffect);
        Vector3 lateralThrustVector = HandleForce(transform.right, inputVector.y, movementValues.betaForce, maxSpeedVector.y, movementValues.betaEffect);
        Vector3 verticalThrustVector = HandleForce(transform.up, inputVector.z, movementValues.gammaForce, maxSpeedVector.z, movementValues.gammaEffect);
        Vector3 finalInput = forwardThrustVector + lateralThrustVector + verticalThrustVector;
        return finalInput;
    }

    public void SetVelocity(Vector3 receivedVelocity)
    {
        velocity = receivedVelocity;
    }

    public void SetMovementValues(MovementValues receivedMovementValues)
    {
        movementValues = receivedMovementValues;
    }

    Vector3 HandleForce(Vector3 shipAxisDirection, float playerInput, float force, float maxValue, MovementValues.ThrusterEffects thrusterEffects)
    {
        float velocityDotProduct = Vector3.Dot(shipAxisDirection, velocity);
        float localPlayerInput = playerInput * 2;

        if (Mathf.Abs(velocityDotProduct) > maxValue)
        {
            localPlayerInput = 0;
        }

        float inputDirection = localPlayerInput + Mathf.Clamp(-velocityDotProduct * Mathf.Clamp(movementValues.inputSlowdownMultiplier, 0, 1), -1, 1);

        thrusterEffects.HandleInput(playerInput + inputDirection);

        return shipAxisDirection * inputDirection * force;
    }
}
