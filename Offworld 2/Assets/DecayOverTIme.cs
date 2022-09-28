using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecayOverTIme : MonoBehaviour
{
    public float decayTime = 3;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, decayTime);
    }
}
