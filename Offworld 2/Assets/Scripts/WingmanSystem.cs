using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingmanSystem : MonoBehaviour
{

    public GameObject[] allies;
    public float currentAllyCount;

    // Start is called before the first frame update
    void Start()
    {
        allies = GameObject.FindGameObjectsWithTag("Ally");
    }

    // Update is called once per frame
    void Update()
    {
        allies = GameObject.FindGameObjectsWithTag("Ally");
            for (int allySelected = 0; allySelected < allies.Length; allySelected++)
            {

                float leftOrRight = 1;
            float row = 0;

                if (allySelected % 2 == 0)
                {
                
                    leftOrRight = -1;
                }

            if (allySelected % 2 == 1)
            {

                row = 1;
            }

            allies[allySelected].GetComponent<ShipAI>().setWingmanStation(new Vector3((-35 + (-35 *( allySelected - row))) * leftOrRight, 0, 0 - (30 * (allySelected - row))));
            }
    }
}
