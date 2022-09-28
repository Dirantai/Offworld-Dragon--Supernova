using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class FreeCam : MonoBehaviour
{

    public Transform thirdPersonTrans;
    public Transform firstPersonTrans;
    public CameraShake shakeSystem;
    public GameObject radarUI;
    public ShipSystem2 playerSystem;
    public SimpleCameraController freeCamController;
    public bool active;
    public bool died;
    private bool firstPerson;

    // Update is called once per frame
    void Update()
    {
        if (playerSystem.inputs["Free Cam"].triggered)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            active = !active;
        }

        Transform parent = null;

        if (playerSystem.inputs["Change Camera"].triggered)
        {
            firstPerson = !firstPerson;
        }

        if (firstPerson)
        {
            radarUI.SetActive(false);
            parent = firstPersonTrans;
            playerSystem.mutedVisuals = true;
        }
        else
        {
            radarUI.SetActive(true);
            parent = thirdPersonTrans;
            playerSystem.mutedVisuals = false;
        }

        shakeSystem.setParent(parent);

        if (playerSystem == null)
        {
            if (!died)
            {
                died = true;
            }
            active = true;
        }

        if (active)
        {
            transform.parent = null;
            if (playerSystem != null)
            {
                playerSystem.active = false;
            }
            freeCamController.enabled = true;
        }
        else
        {
            transform.parent = parent;
            if (playerSystem != null && transform.localPosition != Vector3.zero)
            {
                playerSystem.active = true;
            }
            transform.localPosition = Vector3.zero;
            freeCamController.enabled = false;
        }
    }
}
