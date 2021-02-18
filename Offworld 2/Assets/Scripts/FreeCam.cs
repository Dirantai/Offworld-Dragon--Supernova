using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class FreeCam : MonoBehaviour
{

    public Transform thirdPersonTrans;
    public Transform firstPersonTrans;
    public GameObject radarUI;
    public ShipSystem2 playerSystem;
    public SimpleCameraController freeCamController;
    public bool active;
    public bool died;
    private bool firstPerson;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            active = !active;
        }

        Transform parent = null;

        if (Input.GetKeyDown(KeyCode.C))
        {
            firstPerson = !firstPerson;
        }

        if (firstPerson)
        {
            radarUI.SetActive(false);
            parent = firstPersonTrans;
        }
        else
        {
            radarUI.SetActive(true);
            parent = thirdPersonTrans;
        }

        if (playerSystem == null)
        {
            if (!died)
            {
                died = true;
                transform.rotation = Quaternion.Euler(0, 0, 0);
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
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (playerSystem != null)
            {
                playerSystem.active = true;
            }
            freeCamController.enabled = false;
        }
    }
}
