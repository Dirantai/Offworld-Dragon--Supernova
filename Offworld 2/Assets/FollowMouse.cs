using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    public InputActionAsset inputs;

    // Update is called once per frame
    void Update()
    {
        transform.position = inputs["Mouse Position UI"].ReadValue<Vector2>();
    }
}
