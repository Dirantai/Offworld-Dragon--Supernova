using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AISpawner : MonoBehaviour
{
    public GameObject UIMenu;

    public GameObject[] AITypes;
    
    [System.Serializable]
    public class InputFields
    {
        public string Name;
        public TMP_InputField[] Fields;
    }

    public InputFields[] inputFields;
    public TMP_Dropdown shipTypeDropdown;
    public Toggle friendlyToggle;
    public Toggle defaultStatsToggle;

    public bool friendly;
    public bool defaultStats;
    
    public int selectedType;
    public int currentType;

    public ShipAI AIParams;

    private bool menuOpen;
    public InputActionAsset inputs;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        inputs.actionMaps[2].Enable();
        currentType = 315;
    }


    public void SpawnNewAI()
    {
        Vector3 randomPosition = player.position + new Vector3( Random.Range(-500, 500), Random.Range(-500, 500), Random.Range(-500, 500));
        GameObject g = Instantiate(AITypes[currentType], randomPosition, Quaternion.identity);
        if (!defaultStats)
        {
            AIParams.shootInterval = int.Parse(inputFields[0].Fields[0].text); //Weapon Fire Interval

            AIParams.cooldownInterval = int.Parse(inputFields[1].Fields[0].text); //Weapon Fire Cooldown

            AIParams.engagementRanges.gunRange = int.Parse(inputFields[2].Fields[0].text); //Weapon Range

            inputFields[3].Fields[0].text = "0"; //Missile Fire Interval

            inputFields[4].Fields[0].text = "0"; //Missile Reload

            AIParams.engagementRanges.missileRange = int.Parse(inputFields[5].Fields[0].text); //Missile Range

            AIParams.shipMovementValues.maxSpeedVector = new Vector3(int.Parse(inputFields[6].Fields[0].text), int.Parse(inputFields[6].Fields[1].text), int.Parse(inputFields[6].Fields[2].text)); //Ship Speed FB/LR/UD

            AIParams.shipMovementValues.maxForceVector = new Vector3(int.Parse(inputFields[7].Fields[0].text), int.Parse(inputFields[7].Fields[1].text), int.Parse(inputFields[7].Fields[2].text)); //Ship Force FB/LR/UD

            AIParams.shipMovementValues.speedMultiplier = float.Parse(inputFields[8].Fields[0].text); //Boost Multiplier

            AIParams.shipRotationalValues.maxSpeedVector = new Vector3(int.Parse(inputFields[9].Fields[0].text), int.Parse(inputFields[9].Fields[1].text), int.Parse(inputFields[9].Fields[2].text)); //Rotational Speed R/P/Y

            AIParams.shipRotationalValues.maxForceVector = new Vector3(int.Parse(inputFields[10].Fields[0].text), int.Parse(inputFields[10].Fields[1].text), int.Parse(inputFields[10].Fields[2].text)); //Rotational Torque R/P/Y

            AIParams.shipStats.hullGrade = int.Parse(inputFields[11].Fields[0].text); //Ship Hull Grade

            AIParams.shipStats.shieldGrade = int.Parse(inputFields[12].Fields[0].text); //Ship Hull Grade

            AIParams.shipStats.shieldDelay = int.Parse(inputFields[13].Fields[0].text); //Ship Hull Grade

            AIParams.shipStats.shieldRechargeRate = int.Parse(inputFields[14].Fields[0].text); //Ship Hull Grade

            g.GetComponent<ShipAI>().SetAIVariables(AIParams);
        }
    }

    void HandleEnemyForgeUI()
    {
        if (currentType != selectedType)
        {
            currentType = selectedType;
            ShipAI dummyAIParams = AITypes[currentType].GetComponent<ShipAI>();
            inputFields[0].Fields[0].text = dummyAIParams.shootInterval.ToString(); //Weapon Fire Interval

            inputFields[1].Fields[0].text = dummyAIParams.cooldownInterval.ToString(); //Weapon Fire Cooldown

            inputFields[2].Fields[0].text = dummyAIParams.engagementRanges.gunRange.ToString(); //Weapon Range

            inputFields[3].Fields[0].text = "0"; //Missile Fire Interval

            inputFields[4].Fields[0].text = "0"; //Missile Reload

            inputFields[5].Fields[0].text = dummyAIParams.engagementRanges.missileRange.ToString(); //Missile Range

            inputFields[6].Fields[0].text = dummyAIParams.shipMovementValues.maxSpeedVector.x.ToString(); //Ship Speed F/B
            inputFields[6].Fields[1].text = dummyAIParams.shipMovementValues.maxSpeedVector.y.ToString(); //Ship Speed L/R
            inputFields[6].Fields[2].text = dummyAIParams.shipMovementValues.maxSpeedVector.z.ToString(); //Ship Speed U/D

            inputFields[7].Fields[0].text = dummyAIParams.shipMovementValues.maxForceVector.x.ToString(); //Ship Force F/B
            inputFields[7].Fields[1].text = dummyAIParams.shipMovementValues.maxForceVector.y.ToString(); //Ship Force L/R
            inputFields[7].Fields[2].text = dummyAIParams.shipMovementValues.maxForceVector.z.ToString(); //Ship Force U/D

            inputFields[8].Fields[0].text = dummyAIParams.shipMovementValues.speedMultiplier.ToString(); //Boost Multiplier

            inputFields[9].Fields[0].text = dummyAIParams.shipRotationalValues.maxSpeedVector.x.ToString(); //Rotational Speed Roll
            inputFields[9].Fields[1].text = dummyAIParams.shipRotationalValues.maxSpeedVector.y.ToString(); //Rotational Speed Pitch
            inputFields[9].Fields[2].text = dummyAIParams.shipRotationalValues.maxSpeedVector.z.ToString(); //Rotational Speed Yaw

            inputFields[10].Fields[0].text = dummyAIParams.shipRotationalValues.maxForceVector.x.ToString(); //Rotational Force Roll
            inputFields[10].Fields[1].text = dummyAIParams.shipRotationalValues.maxForceVector.y.ToString(); //Rotational Force Pitch
            inputFields[10].Fields[2].text = dummyAIParams.shipRotationalValues.maxForceVector.z.ToString(); //Rotational Force Yaw

            inputFields[11].Fields[0].text = dummyAIParams.shipStats.hullGrade.ToString(); //Ship Hull Grade

            inputFields[12].Fields[0].text = dummyAIParams.shipStats.shieldGrade.ToString(); //Ship Shield Grade

            inputFields[13].Fields[0].text = dummyAIParams.shipStats.shieldDelay.ToString(); //Ship Shield Delay

            inputFields[14].Fields[0].text = dummyAIParams.shipStats.shieldRechargeRate.ToString(); //Ship Shield Regen
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (menuOpen)
        {
            selectedType = shipTypeDropdown.value;
            friendly = friendlyToggle.isOn;
            defaultStats = defaultStatsToggle.isOn;
            HandleEnemyForgeUI();
        }

        
        UIMenu.SetActive(menuOpen);

        if (inputs["ShowForge"].triggered) menuOpen = !menuOpen;
    }
}
