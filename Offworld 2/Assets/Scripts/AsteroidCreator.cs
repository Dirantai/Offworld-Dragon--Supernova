using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCreator : MonoBehaviour
{

    public GameObject asteroid;
    public float numberOfAsteroids;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numberOfAsteroids; i++)
        {
            float randomZ = Random.Range(40, 500);
            float randomY = Random.Range(40, 500);
            float randomX = Random.Range(40, 500);

            float randomAlpha = Random.Range(0, 100);
            float randomBeta = Random.Range(0, 100);
            float randomGamma = Random.Range(0, 100);

            if(randomAlpha > 50)
            {
                randomX *= -1;
            }

            if(randomBeta < 50)
            {
                randomY *= -1;
            }

            if(randomGamma > 50)
            {
                randomZ *= -1;
            }

            Vector3 position = new Vector3(randomX, randomY, randomZ);

            GameObject tempOBJ = Instantiate(asteroid, position, Quaternion.identity, transform) as GameObject;

            float temp = Random.Range(25, 100);

            tempOBJ.transform.localScale = new Vector3(temp, temp, temp);
        }
    }
}
