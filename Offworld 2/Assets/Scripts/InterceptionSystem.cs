using UnityEngine;
using System.Collections;

public class InterceptionSystem {

    public float CalculateInterceptTime(Vector3 TargetPosition, Vector3 InterceptorStartPosition, Vector3 TargetVelocity, float InterceptorSpeed)
    {
        //Okay this took me a while to get.
        float a = TargetVelocity.sqrMagnitude - (InterceptorSpeed * InterceptorSpeed); //(TargetVelocity^2) - InterceptorSpeed^2. (TargetVeloctiy^2) is the vector dot product of TargetVelocity by itself. For some reason I can't square floats using ^ 2, so I just have to multiply it by itself.
        float b = Vector3.Dot((TargetPosition - InterceptorStartPosition), TargetVelocity); //I keep forgetting if I want to multiply two vectors I actually need to dot product them...
        float c = (TargetPosition - InterceptorStartPosition).sqrMagnitude; //get the square
        //Now we shove these into the quadratic equation! Yes! Calculating intercept time is a quadratic equation! Don't worry, this'll be explained later.
        float Time1 = (-b + Mathf.Sqrt((b * b) - (4 * a * c))) / (2 * a); //source: quadratic equation
        float Time2 = (-b - Mathf.Sqrt((b * b) - (4 * a * c))) / (2 * a); //source: quadratic equation
        if(Time1 < 0) //Since the real time we need shouldn't be negative, use simple comparissons to find which is the positive answer.
        {
            return Time2;
        }
        else if(Time2 < 0)
        {
            return Time1;
        }
        else //if none of them are positive, that means we (or possibly you) have messed up and the interception is considered impossible. So just go to the Target directly.
        {
            return 0;
        }
    }

    public Vector3 CalculateInterceptPosition(Vector3 targetPosition, Vector3 targetVelocity, Vector3 interceptorStartPosition, float interceptorSpeed)
    {
        //Since the intercept point is basically just a position in the velocity direction of the target,
        //Just multiply the velocity vector by the intercept time, and add it on to the original position to get the intercept position.
        //Using a whole bunch of maths, we can get intercept time without even needing the intercept position. Look to the function above to see the time!
        float interceptTime = CalculateInterceptTime(targetPosition, interceptorStartPosition, targetVelocity, interceptorSpeed);
        return targetPosition + (targetVelocity * interceptTime);
    }
}
