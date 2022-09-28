using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerzierTest
{

    public static List<Vector3> UpdateLine(List<Vector3> nodes){
        List<Vector3> finalCurve = new List<Vector3>();
        nodes.Reverse();
        for(int i = 0;i< nodes.Count - 1;i++){

            if((i + 2 > nodes.Count - 1)){
                if(nodes[i + 1] != null) finalCurve.Add(nodes[i + 1]);
            }else{

                for(float t = 0; t <= 5;t++){
                    float trueT = t/10;

                    Vector3 middlePoint = (nodes[i] * Mathf.Pow(0.5f, 2)) +
                                        (nodes[i + 2] * Mathf.Pow(0.5f, 2)) -
                                        nodes[i + 1];

                    middlePoint = middlePoint / (-2 * Mathf.Pow(0.5f, 2));

                    Vector3 berzierPoint = middlePoint + ((nodes[i] - middlePoint) * Mathf.Pow(1-trueT, 2)) +
                                                        (Mathf.Pow(trueT, 2) * (nodes[i + 2] - middlePoint));

                    finalCurve.Add(berzierPoint);
                }
            }
        }
        finalCurve.Reverse();
        return finalCurve;
    }
}
