using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementSystem : MonoBehaviour
{

    public Vector3 originalScale;
    public Vector3 iconPosition;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(iconPosition);

        if (player != null)
        {
            float reticleProduct = Vector3.Dot(iconPosition - player.position, player.forward);
            if (reticleProduct < 0)
            {
                transform.localScale = Vector3.zero;
            }
            else
            {
                transform.localScale = originalScale;
            }
        }
        else
        {
            player = Camera.main.transform;
        }
        }
    }
}
