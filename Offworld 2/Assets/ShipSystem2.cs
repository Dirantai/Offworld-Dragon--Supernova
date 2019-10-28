using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSystem2 : BasicForceSystem
{
    public Rigidbody shipRigid;

    public MovementValues shipMovementValues;
    public MovementValues shipRotationalValues;

    public Transform shipModel;

    float MouseLookSystem(float mouseInputAxis, int maxValue)
    {

        float screenCentre = maxValue / 2;

        float finalInput = (mouseInputAxis - screenCentre) / screenCentre / 2;

        return Mathf.Clamp(-finalInput, -1, 1);
    }

    void Update()
    {

        HandleMovement();
        HandleVisuals();

    }

    void HandleVisuals()
    {
        float xOffset = MouseLookSystem(Input.mousePosition.x, Screen.width);
        float yOffset = MouseLookSystem(Input.mousePosition.y, Screen.height);
        shipModel.localRotation = Quaternion.Lerp(shipModel.localRotation, Quaternion.Euler(yOffset * 25, -xOffset * 30, xOffset * 50 + Input.GetAxis("Roll") * 10), 6 * Time.deltaTime);

        float xPosOffset = Vector3.Dot(transform.right, shipRigid.velocity);
        float yPosOffset = Vector3.Dot(transform.up, shipRigid.velocity);
        float zPosOffset = Vector3.Dot(transform.forward, shipRigid.velocity);

        Vector3 visualVector = new Vector3(xPosOffset, yPosOffset, zPosOffset);

        shipModel.localPosition = Vector3.Lerp(shipModel.localPosition,  visualVector, 6 * Time.deltaTime);
    }

    void HandleMovement()
    {
        SetMovementValues(shipMovementValues);
        SetVelocity(shipRigid.velocity);

        Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("JumpThrust"));
        Vector3 maxSpeedVector = new Vector3(shipMovementValues.maxAlphaSpeed, shipMovementValues.maxBetaSpeed, shipMovementValues.maxGammaSpeed);

        shipRigid.AddForce(CalculateFinalInput(inputVector, maxSpeedVector));

        SetMovementValues(shipRotationalValues);
        SetVelocity(shipRigid.angularVelocity);

        inputVector = new Vector3(Input.GetAxis("Roll"), MouseLookSystem(Input.mousePosition.y, Screen.height), -MouseLookSystem(Input.mousePosition.x, Screen.width));
        maxSpeedVector = new Vector3(shipRotationalValues.maxAlphaSpeed, shipRotationalValues.maxBetaSpeed * Mathf.Abs(MouseLookSystem(Input.mousePosition.y, Screen.height)), shipRotationalValues.maxGammaSpeed * Mathf.Abs(MouseLookSystem(Input.mousePosition.x, Screen.width)));

        shipRigid.AddTorque(CalculateFinalInput(inputVector, maxSpeedVector));
    }
}
