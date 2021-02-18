using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSystem : MonoBehaviour
{

    public GameObject[] markers;
    public GameObject markerPrefab;
    public Material inFront;
    public Material behind;
    public Transform player;
    public LineRenderer lines;
    public Vector3[] radarPositions;
    public float RadarSize;

    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (player != null)
        {

            if (markers.Length != enemies.Length)
            {
                foreach (GameObject marker in markers)
                {
                    Destroy(marker);
                }
                markers = new GameObject[enemies.Length];
                radarPositions = new Vector3[enemies.Length * 2];
                lines.positionCount = enemies.Length * 2;
            }

            for (int enemy = 0; enemy < enemies.Length; enemy++)
            {
                if (markers[enemy] == null)
                {
                    markers[enemy] = Instantiate(markerPrefab, transform.position, transform.rotation, transform);
                }
                Vector3 playerToEnemy = enemies[enemy].transform.position - player.transform.position;

                float dotProduct = Vector3.Dot(player.forward, playerToEnemy);

                if (dotProduct > 0)
                {
                    markers[enemy].GetComponentInChildren<MeshRenderer>().material = inFront;
                }
                else
                {
                    markers[enemy].GetComponentInChildren<MeshRenderer>().material = behind;
                }

                float distanceFromPLayer = playerToEnemy.magnitude;

                distanceFromPLayer = Mathf.Clamp(distanceFromPLayer, 0, RadarSize);

                float radarDistance = Mathf.Clamp(0.15f * (distanceFromPLayer / RadarSize), 0.1f, 0.065f);

                markers[enemy].transform.position = transform.position + playerToEnemy.normalized * radarDistance;
                markers[enemy].transform.GetChild(0).rotation = enemies[enemy].transform.rotation;
                markers[enemy].transform.GetChild(1).LookAt(transform.position);

                markers[enemy].transform.localScale = new Vector3(5, 5, 5) * (distanceFromPLayer / RadarSize);

                radarPositions[2 * enemy] = markers[enemy].transform.localPosition;
            }

            lines.SetPositions(radarPositions);

            transform.rotation = player.rotation;
        }
        else
        {
            foreach (GameObject marker in markers)
            {
                Destroy(marker);
            }
        }
    }
}
