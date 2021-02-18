using UnityEngine;
using System;
using System.Collections;

public class ScaleImage : MonoBehaviour { //scales the image depending on how far away it is from the camera.
    public Transform ScaleGuide; //the guide to be used to check the size of the image
    public float Radius; //The needed size of the image (from centre to scaleguide)
    public Transform ImageTransform; //The Image itself.
	// Update is called once per frame
	void Update () {
        Vector2 GuideScreenPos = Camera.main.WorldToScreenPoint(ScaleGuide.position); //gets the position of the guide on screen
        Vector2 ImageScreenPos = Camera.main.WorldToScreenPoint(transform.position); //gets the position of the image on screen
        Vector2 DirectionVector = GuideScreenPos - ImageScreenPos; //create a vector between the guide and image position, basically the radius of the image vector.
        Vector3 PositionToCamera = Camera.main.transform.position - transform.position; // the world vector between the image and camera
        transform.LookAt(transform.position - PositionToCamera); //make the image face this vector above.
        if (DirectionVector.magnitude > 0) //Check if the radius vector is > 0 (so it doesn't get /0)
        {
            float Multiplier = Radius / DirectionVector.magnitude; //Divide the required radius by the current radius of the image. this is the multiplier to get the right scale.
            Multiplier = (float)Math.Round(Multiplier, 2); //Round it to 2 decimal places.
            ImageTransform.localScale = ImageTransform.localScale * Multiplier; //Apply the multiplier to the image scale so that it is the right size on screen.
        }
        else
        {
            ImageTransform.localScale = new Vector3(1, 1, 1); //if magnitude <= 0, then set the scale to a default scale.
        }
    }
}
