using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipUI : MonoBehaviour
{
    private ShipSystem2 controller;
    public Transform healthBar;
    public Transform shieldBar;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ShipSystem2>();
    }

    // Update is called once per frame
    void Update()
    {

        healthBar.localScale = new Vector3(0.5f, 0.5f * controller.GetHealth() / 100, 1);
        shieldBar.localScale = new Vector3(0.5f, 0.5f * controller.GetShield() / 100, 1);
        healthBar.localPosition = new Vector3(healthBar.localPosition.x, -160 *  ((100 - controller.GetHealth()) / 100), 0);
        shieldBar.localPosition = new Vector3(shieldBar.localPosition.x, -160 * ((100 - controller.GetShield()) / 100), 0);
    }
}
