using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetData : MonoBehaviour
{

    public int dangerLevel; //Danger level, from 0 to 6. Not completing a mission will increment it by 1. finishing a mission decreases by 2. Danger level 6 is considered lost.
    private Transform planetModel; //Used for selection and Hover
    private string missionType; //self explanitory
    private string Name; //it's right in the name

    // Start is called before the first frame update
    void Start()
    {
        missionType = "Clear Enemies"; //placeholder
        Name = transform.name;
        planetModel = transform.GetChild(0);
    }

    public string GetUIData()
    {
        return CompilePlanetUIData();
    }

    public int GetDangerLvl()
    {
        return dangerLevel;
    }

    public void SetDangerLvl(int level)
    {
        dangerLevel = level;
    }

    public Vector3 GetPlanetPosition()
    {
        return planetModel.position;
    }

    string CompilePlanetUIData()
    {
        string UIData = Name + "\nDanger Lvl: " + dangerLevel.ToString() + "\nMission: " + missionType; //all of the data into a nice string for the UI
        return UIData;
    }
}
