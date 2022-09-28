using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private float currentIntensity;
    private float currentDuration;
    private float maxDuration;
    private Transform currentParent;
    private float currentImpulse;
    private Vector3 currentSpread;

    public bool impulseTest;

    public float impulseRate;

    // Update is called once per frame
    void Update()
    {

        if(currentDuration < maxDuration){
            
            float impulseDuration = 1 / impulseRate;
            
            if(currentImpulse < impulseDuration){
                currentImpulse += Time.deltaTime;
            }else{
                currentImpulse = 0;
                float timeline = (currentDuration / maxDuration);
                currentSpread = CalculateSpreadVector(currentIntensity * Mathf.Clamp((2 - Mathf.Exp(timeline)), 0.01f, 1));

            }

            currentDuration += Time.deltaTime;
        }else{
            currentDuration = maxDuration;
            currentSpread = Vector3.zero;
        }

        if(impulseTest){
            impulseTest = false;
            ShakeImpulse(30, 3, true);
        }
        Quaternion spread = Quaternion.Euler(currentSpread);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, spread, 3 * Time.deltaTime);
    }

    Vector3 CalculateSpreadVector(float maxSpread)
    {
        Vector3 spreadVector = new Vector3(Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread), 0);
        return spreadVector;
    }

    public void ShakeImpulse(float intensity, float duration, bool overwrite){
        if((!overwrite && currentDuration > maxDuration - 0.5f) || overwrite){
            currentIntensity = intensity;
            maxDuration = duration;
            currentDuration = 0;
            currentImpulse = 1 / impulseRate;
        }
    }

    public void setParent(Transform parent){
        currentParent = parent;
    }
}
