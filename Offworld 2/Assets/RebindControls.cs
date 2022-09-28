using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindControls : MonoBehaviour
{
    public InputActionAsset inputs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RebindAxis(InputAction action){
        var rebindAction = action.PerformInteractiveRebinding().Start();
        action.Dispose();
    }
}
