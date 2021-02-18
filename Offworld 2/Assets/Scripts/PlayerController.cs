using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : BasicForceSystem
{

    public bool active;
    public Rigidbody shipRigid;


    public MovementValues MovementValues;

    public float jumpForce;
    public float rotationalSensitivity;

    public float currentShieldHealth;
    public float currentHealth;

    public bool grounded;
    public bool jumping;
    public float groundCheckDistance;
    public bool visuals;

    public Transform camera;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = 100;
        currentShieldHealth = 100;
    }


    void FixedUpdate()
    {
        if (active)
        {
            HandleJumping();
            Vector3 movementInput = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0);
            float rotationInput = Input.GetAxis("Mouse X") * rotationalSensitivity;
            HandleMovement(movementInput, rotationInput);
            GroundChecker();
            //if(visuals) HandleVisuals(rotationInput);
        }
        else
        {
            HandleMovement(Vector3.zero, 0);
        }
    }

    void GroundChecker()
    {
        float distance = 1;
        Vector3 position = Vector3.zero;
        if (Physics.SphereCast(transform.position, 0.4f, -transform.up, out RaycastHit hitInfo, 10))
        {
            position = hitInfo.point;
            distance = hitInfo.distance;
        }

        if (position != Vector3.zero)
        {
            if (distance > 1 && distance <= groundCheckDistance + 1 || distance <= groundCheckDistance + 1)
            {
                if (!jumping)
                {
                    transform.position = new Vector3(transform.position.x, position.y + 1, transform.position.z);
                }
                if (!grounded)
                {
                    jumping = false;
                    grounded = true;
                }

            }
            else if (distance >= groundCheckDistance + 1)
            {
                grounded = false;
            }
        }
        else
        {
            grounded = false;
        }
    }

    void HandleJumping()
    {
        if (Input.GetAxis("JumpThrust") > 0 && grounded)
        {
            var jumpVelocity = Vector3.up * Mathf.Sqrt(jumpForce * 2 * Physics.gravity.magnitude);
            shipRigid.velocity += jumpVelocity;

            jumping = true;
            grounded = false;
        }
    }

    void HandleVisuals(Vector3 rotationalInput)
    {
        float xOffset = 0;
        float yOffset = 0;
        float modifier = 1;
        shipModel.localRotation = Quaternion.Lerp(shipModel.localRotation, Quaternion.Euler(yOffset * 25 / modifier, -xOffset * 30 / modifier, (xOffset * 50 + rotationalInput.x * 10) / modifier), 6 * Time.deltaTime);

        float xPosOffset = Vector3.Dot(transform.right, shipRigid.velocity);
        float yPosOffset = Vector3.Dot(transform.up, shipRigid.velocity);
        float zPosOffset = Vector3.Dot(transform.forward, shipRigid.velocity);

        Vector3 visualVector = new Vector3(xPosOffset, yPosOffset, zPosOffset);

        shipModel.localPosition = Vector3.Lerp(shipModel.localPosition, visualVector / modifier, 6 * Time.deltaTime);
    }
    private float pitch = 0;
    public virtual void HandleMovement(Vector3 movementInput, float rotationInput)
    {

        SetMovementValues(MovementValues);
        SetVelocity(shipRigid.velocity);
        Vector3 maxSpeedVector = new Vector3(MovementValues.maxSpeedVector.x, MovementValues.maxSpeedVector.y, MovementValues.maxSpeedVector.z);
        //if (boosting)
        //{
        //    maxSpeedVector = maxSpeedVector * shipMovementValues.speedMultiplier;
        //}

        Vector3 finalVector = CalculateFinalInput(movementInput, maxSpeedVector);
        finalVector = new Vector3(finalVector.x, 0, finalVector.z);
        shipRigid.AddForce(finalVector);

        if (!grounded)
        {
            shipRigid.velocity += Vector3.down * -Physics.gravity.y * Time.deltaTime * 2;
        }

        pitch += (Input.GetAxis("Mouse Y") * rotationalSensitivity);
        camera.localRotation = Quaternion.Euler(Mathf.Clamp(-pitch, -80, 80), 0, 0);
        transform.Rotate(0, rotationInput, 0);
    }
}
