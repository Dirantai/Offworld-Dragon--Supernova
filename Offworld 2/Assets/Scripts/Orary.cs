using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Orary : MonoBehaviour
{


    public Camera oraryCam;
    public TextMeshProUGUI planetDataUI;
    public GameObject oraryObj;
    public GameObject gameUI;
    public Transform CursorUI;
    public Transform excalTransform;
    public Transform playerTransform;

    private bool open;
    private PlanetData[] Planets;
    private PlanetData selectedPlanet;
    private PlanetData excaliburPlanet;
    private PlanetData currentPlayerPlanet;
    private PlanetData nextTarget;

    // Start is called before the first frame update
    void Start()
    {
        Planets = FindObjectsOfType<PlanetData>();
        selectedPlanet = Planets[0];
        currentPlayerPlanet = selectedPlanet;
        excaliburPlanet = Planets[3];
        nextTarget = excaliburPlanet;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTransitions();
        oraryCam.enabled = open;
        oraryObj.SetActive(open);
        gameUI.SetActive(!open);
        if (open)
        {
            PlanetSelection();
            excaliburPlanet = ShipMovement(excalTransform, excaliburPlanet, nextTarget);
            currentPlayerPlanet = ShipMovement(playerTransform, currentPlayerPlanet, selectedPlanet);
        }
    }

    void PlanetDataDisplay(PlanetData planet)
    {
            planetDataUI.text = planet.GetUIData();
    }

    void HandleTransitions()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            open = !open;
        }
    }
    int nextplanet;

    PlanetData ShipMovement(Transform shipTrans,  PlanetData currentPlanet, PlanetData destination)
    {
        var line = shipTrans.GetComponent<LineRenderer>();
        line.SetPosition(0, currentPlanet.GetPlanetPosition());
        line.SetPosition(1, destination.GetPlanetPosition());
        shipTrans.position = currentPlanet.GetPlanetPosition();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            foreach (PlanetData planet in Planets)
            {
                planet.SetDangerLvl(Random.Range(0, 6));
            }
                currentPlanet = destination;
        }
        return currentPlanet;
    }

    void PlanetSelection()
    {
        CursorUI.position = oraryCam.WorldToScreenPoint(selectedPlanet.GetPlanetPosition());
        PlanetData viewedPlanet = null;
        int highestLevel = 0;
        foreach (PlanetData planet in Planets)
        {
            if(planet.GetDangerLvl() > highestLevel)
            {
                highestLevel = planet.GetDangerLvl();
                nextTarget = planet;
            }

            Vector3 planetPosition = oraryCam.WorldToScreenPoint(planet.GetPlanetPosition());
            float distance = (planetPosition - Input.mousePosition).magnitude;

            if (distance < 25)
            {
                viewedPlanet = planet;
                HoverPlanet(planet);
            }
        }

       
        if (viewedPlanet == null)
        {
            PlanetDataDisplay(selectedPlanet);
        }
        else
        {
            PlanetDataDisplay(viewedPlanet);
        }
    }

    void HoverPlanet(PlanetData planet)
    {
        CursorUI.position = oraryCam.WorldToScreenPoint(planet.GetPlanetPosition());
        if (Input.GetButtonDown("Fire1"))
        {
            selectedPlanet = planet;
        }
    }
}
