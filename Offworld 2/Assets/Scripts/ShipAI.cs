using System.Collections.Generic;
using UnityEngine;

public class ShipAI : ShipSystem2
{
    
    private Vector3 movementTarget;
    private Vector3 movementVector;

    public GameObject marker;
    private UIElementSystem markerUI;
    private Vector3 currentInput;
    public Transform canvas;
    public float shootInterval;
    public float cooldownInterval;

    [System.Serializable]
    public class Ranges
    {
        public float gunRange;
        public float missileRange;
        public float specialRange;
    }

    public Ranges engagementRanges;

    private float chaseTimer;
    private float shootTimer;

    public enum ShipState
    {
        Breakingoff,
        Orbitting,
        Idle,
        Wrestling,
        Pathing,
    }

    public ShipState shipState;

    private bool shoot;

    private GameObject Node;
    private List<GameObject> previousNodes = new List<GameObject>();
    private GameObject[] seenNodes;
    private bool PathFinding;
    private LayerMask sightMask;

    // Start is called before the first frame update
    public override void OnStart()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        GameObject tempObj = Instantiate(marker, canvas) as GameObject;
        markerUI = tempObj.GetComponent<UIElementSystem>();
    }

    public void SetAIVariables(ShipAI AIParams)
    {
        shootInterval = AIParams.shootInterval;
        cooldownInterval = AIParams.cooldownInterval;
        engagementRanges = AIParams.engagementRanges;
        shipStats = AIParams.shipStats;
        shipMovementValues = AIParams.shipMovementValues;
        shipRotationalValues = AIParams.shipRotationalValues;
    }
    public override void OnUpdate()
    {
        if (shipTarget != null)
        {
            float distanceToShipTarget = (transform.position - shipTarget.position).magnitude;
            if (!PathFinding)
            {
                movementTarget = shipTarget.position;
                if (distanceToShipTarget < engagementRanges.gunRange * 0.1f)
                {
                    if (shootTimer > 0)
                    {
                        shootTimer -= Time.deltaTime;
                    }
                    else
                    {
                        shoot = !shoot;
                        if (shoot)
                        {
                            shootTimer = shootInterval;
                        }
                        else
                        {
                            shootTimer = cooldownInterval;
                        }
                    }
                }

                if(distanceToShipTarget < engagementRanges.missileRange * 0.1f)
                {
                    if (missiles != null)
                    {
                        missiles.shipTarget = shipTarget;
                        missiles.FireMissile();
                    }
                }
            }

            if (gunSystem != null)
            {
                gunSystem.shipTarget = shipTarget;
                gunSystem.Shoot = shoot;
            }


            if (chaseTimer > 0)
            {
                chaseTimer -= Time.deltaTime;
            }
            else
            {
                chaseTimer = Random.Range(6, 9);
                if (shipState == ShipState.Breakingoff)
                {
                    SelectTarget();
                    if(chaseTimer % 2 == 0)
                    {
                        shipState = ShipState.Orbitting;
                    }
                    else
                    {
                        shipState = ShipState.Wrestling;
                    }
                    
                }
                else
                {
                    shipState = ShipState.Breakingoff;
                }
                movementTarget = Vector3.zero;

            }
        }
        else
        {
            shipState = ShipState.Idle;
            SelectTarget();
            gunSystem.Shoot = false;
        }

        PathFinder(movementTarget - transform.position);
        if (markerUI != null)
        {
            markerUI.iconPosition = transform.position;
        }
    }


    private float actionTimer;
    private int direction;
    private Vector3 wingmanPosition;

    public void setWingmanStation(Vector3 position)
    {
        wingmanPosition = position;
    }

    public override void HandleMovement(Vector3 movementInput, Vector3 rotationInput)
    {

        float distanceToTarget = (movementTarget - transform.position).magnitude;

        if (PathFinding)
        {
            if (Node != null)
            {
                movementVector = Node.transform.position;
            }
            else
            {
                Debug.Log("Failed!");
                PathFinding = false;
            }
        }
        else
        {
            movementVector = movementTarget;
        }
        Vector3 trueMovementTarget = movementVector;
        Vector3 trueRotationTarget = movementTarget;
        Vector3 rollTarget = transform.up;
        bool roll = false;

        if (actionTimer <= 0)
        {
            if (Mathf.RoundToInt(distanceToTarget) % 2 == 0)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            actionTimer = Random.Range(3, 7);
            if(actionTimer >= 6)
            {
                direction = 0;
            }
        }
        else
        {
            actionTimer -= Time.deltaTime;
        }

        switch (shipState)
        {
            case ShipState.Breakingoff:

                boosting = true;
                trueMovementTarget = (transform.position + (transform.forward * 10) + (transform.right * direction));
                trueRotationTarget = trueMovementTarget;

                break;
            case ShipState.Orbitting:

                boosting = false;
                roll = true;
                rollTarget = movementTarget;
                trueMovementTarget = movementVector;
                if (distanceToTarget < 100 && !PathFinding)
                {
                    trueMovementTarget = ((transform.position - movementTarget).normalized * -80);
                }
                trueRotationTarget = movementTarget;

                break;
            case ShipState.Wrestling:

                boosting = true;
                movementVector = new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), Random.Range(-30f, 30f));
                if (shipTarget != null)
                {
                    trueMovementTarget = movementTarget + movementVector - (shipTarget.forward * 40);
                }
                else
                {
                    shipState = ShipState.Idle;
                }
                trueRotationTarget = movementTarget;

                break;
            case ShipState.Idle:
                Transform player = null;
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").transform;
                }

                if (player != null && transform.tag == "Ally")
                {
                    boosting = player.GetComponent<ShipSystem2>().boosting;
                    trueMovementTarget = player.position + (player.right * (wingmanPosition.x / 10)) + (player.forward * (wingmanPosition.z / 10));
                    movementTarget = trueMovementTarget;
                    trueRotationTarget = player.forward * 10 + transform.position;
                    rollTarget = trueMovementTarget + player.up;
                    roll = true;
                }
                break;
        }

        movementInput = MoveToTarget(trueMovementTarget);
        rotationInput = AimAtTarget(rollTarget, trueRotationTarget, roll);

        if (shipState == ShipState.Orbitting && direction != 0)
        {
            movementInput = new Vector3(movementInput.x, direction, movementInput.z);
        }

        movementInput = HandleCollisionDetection(movementInput);
        base.HandleMovement(movementInput, rotationInput);
    }
    void PathFinder(Vector3 TargetVector)
    {
        RaycastHit hit;
        Vector3 hitPos = new Vector3();
        int ClosestNode = 0;
        if (Physics.Raycast(transform.position, TargetVector, out hit, TargetVector.magnitude, sightMask))
        {
            hitPos = hit.point;
        }

        if (hitPos != Vector3.zero)
        {
            PathFinding = true;
            GameObject[] Nodes = GameObject.FindGameObjectsWithTag("Node");
            if (Node == null)
            {
                previousNodes.Clear();
                seenNodes = FindAvailableNodes(Nodes, transform.position);
                ClosestNode = FindShortestDistance(seenNodes, TargetVector + transform.position);
                try
                {
                    Node = seenNodes[ClosestNode];
                }
                catch
                {
                    Debug.Log(ClosestNode + gameObject.name);
                }
            }
            else
            {
                Vector3 CurrentNodeVector = Node.transform.position - transform.position;
                if (CurrentNodeVector.magnitude <= 3f)
                {
                    previousNodes.Add(Node);
                    seenNodes = FindAvailableNodes(Nodes, transform.position);
                    ClosestNode = FindShortestDistance(seenNodes, TargetVector + transform.position);
                    Node = seenNodes[ClosestNode];
                }
                hitPos = new Vector3();
                if (Physics.Raycast(transform.position, CurrentNodeVector, out hit, CurrentNodeVector.magnitude, sightMask))
                {
                    hitPos = hit.point;
                }
                if (hitPos != Vector3.zero)
                {
                    Node = null;
                }
            }
        }
        else
        {
            previousNodes.Clear();
            Node = null;
            PathFinding = false;
        }
    }

    GameObject[] FindAvailableNodes(GameObject[] Nodes, Vector3 StartPosition)
    {
        List<GameObject> availableNodes = new List<GameObject>();
        for (int n = 0; n < Nodes.Length; n++)
        {
            Vector3 CurrentNode = Nodes[n].transform.position - StartPosition;
            Vector3 hitPos = new Vector3();
            if (Physics.Raycast(transform.position, CurrentNode, out RaycastHit hit, CurrentNode.magnitude, sightMask))
            {
                hitPos = hit.point;
            }
            if (hitPos == Vector3.zero)
            {
                availableNodes.Add(Nodes[n]);
            }
        }
        GameObject[] newNodes = new GameObject[availableNodes.Count];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = availableNodes[n];
        }
        return newNodes;
    }


    int FindShortestDistance(GameObject[] Nodes, Vector3 StartPosition)
    {
        int ClosestNodeIndex = 0;
        for (int n = 0; n < Nodes.Length; n++)
        {
            Vector3 CurrentNode = StartPosition - Nodes[n].transform.position;
            Vector3 ClosestNode = StartPosition - Nodes[ClosestNodeIndex].transform.position;
            if (CurrentNode.magnitude < ClosestNode.magnitude)
            {
                if (Nodes[n] != Node)
                {
                    bool cancel = false;
                    foreach (GameObject node in previousNodes)
                    {
                        if (Nodes[n] == node)
                        {
                            cancel = true;
                        }
                    }
                    if (!cancel)
                    {
                        ClosestNodeIndex = n;
                    }
                }
            }
        }

        return ClosestNodeIndex;
    }

    public override void OnDeath()
    {
        Destroy(markerUI.gameObject);
    }

    Vector3 HandleCollisionDetection(Vector3 input)
    {
        input.x = CheckCollisionOnAxis(transform.forward, input.x);
        input.y = CheckCollisionOnAxis(transform.right, input.y);
        input.z = CheckCollisionOnAxis(transform.up, input.z);
        //input = CheckCollisionOnVelocity(shipRigid.velocity, input);
        return input;
    }

    Vector3 CheckCollisionOnVelocity(Vector3 direction, Vector3 input)
    {
        Vector3 position = Vector3.zero;
        Debug.DrawRay(transform.position, direction * Mathf.Clamp(shipRigid.velocity.magnitude, 5, 20), Color.green);
        if (shipRigid.SweepTest(direction.normalized, out RaycastHit hitInfo, Mathf.Clamp(shipRigid.velocity.magnitude, 5, 20), QueryTriggerInteraction.Ignore))
        {
            position = hitInfo.point;
        }

        if (position != Vector3.zero)
        {
            Debug.Log("Hit!");
            float forwardVelocity = Vector3.Dot(transform.forward, position - transform.position);
            float rightVelocity = Vector3.Dot(-transform.right, position - transform.position);
            float upVelocity = Vector3.Dot(-transform.up, position - transform.position);
            input = new Vector3(Mathf.Clamp(forwardVelocity * 10, -1, 1), Mathf.Clamp(-rightVelocity * 10, -1, 1), Mathf.Clamp(-upVelocity * 10, -1, 1));
        }
        return input;
    }

    float CheckCollisionOnAxis(Vector3 direction, float directionInput)
    {
        Collider collider = new Collider();
        Collider collider2 = new Collider();
        float directionalSpeed = Vector3.Dot(direction, shipRigid.velocity);
        float positive = 0;
        float negative = 0;
        if (directionalSpeed > 0)
        {
            positive = directionalSpeed;
        }
        else if (directionalSpeed < 0)
        {
            negative = directionalSpeed;
        }
        Debug.DrawRay(transform.position, direction * Mathf.Clamp(positive, 5, 20), Color.green);
        Debug.DrawRay(transform.position, -direction * Mathf.Clamp(-negative, 5, 20), Color.red);

        if (shipRigid.SweepTest(direction, out RaycastHit hitInfo, Mathf.Clamp(positive, 5, 20), QueryTriggerInteraction.Ignore))
        {
            //positive = hitInfo.distance;
            collider = hitInfo.collider;
        }


        if (shipRigid.SweepTest(-direction, out hitInfo, Mathf.Clamp(-negative, 5, 20), QueryTriggerInteraction.Ignore))
        {
            //negative = hitInfo.distance;
            collider2 = hitInfo.collider;
        }

        if (collider != null)
        {

            if (collider2 != null)
            {
                if (0.1f + positive > 0.1f - negative)
                {
                    directionInput = 1;
                }
                else
                {
                    directionInput = -1;
                }
            }
            else
            {
                directionInput = -1;
            }
        }
        else if (collider2 != null)
        {
            directionInput = 1;
        }
        return directionInput;
    }

    void SelectTarget()
    {
        GameObject closestEnemy = null;
        float randomNumber = Random.Range(0, 100);
        if (transform.tag == "Ally")
        {

            GameObject[] TargetList = GameObject.FindGameObjectsWithTag("Enemy");

            float closestDistance = 0;
            foreach (GameObject enemy in TargetList)
            {
                if (closestDistance == 0)
                {
                    closestDistance = (transform.position - enemy.transform.position).magnitude;
                    closestEnemy = enemy;
                }
                else
                {
                    float distanceFromTarget = (transform.position - enemy.transform.position).magnitude;
                    if (distanceFromTarget < closestDistance)
                    {
                        closestDistance = distanceFromTarget;
                        closestEnemy = enemy;
                    }
                }
            }
        }

        if (transform.tag == "Enemy")
        {

            GameObject[] TargetList = GameObject.FindGameObjectsWithTag("Ally");
            GameObject[] updatedTargetList = new GameObject[TargetList.Length + 1];
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                updatedTargetList[0] = GameObject.FindGameObjectWithTag("Player");
                for (int i = 1; i < updatedTargetList.Length; i++)
                {
                    updatedTargetList[i] = TargetList[i - 1];
                }
            }
            else
            {
                updatedTargetList = TargetList;
            }

            float closestDistance = 0;
            foreach (GameObject enemy in updatedTargetList)
            {
                if (closestDistance == 0)
                {
                    closestDistance = (transform.position - enemy.transform.position).magnitude;
                    closestEnemy = enemy;
                }
                else
                {
                    float distanceFromTarget = (transform.position - enemy.transform.position).magnitude;
                    if (distanceFromTarget < closestDistance)
                    {
                        closestDistance = distanceFromTarget;
                        closestEnemy = enemy;
                    }
                }
            }
        }

        if (closestEnemy != null)
        {
            shipTarget = closestEnemy.transform;
            shipState = ShipState.Wrestling;
            if (shipTarget.tag != "Player")
            {
                shipTarget.GetComponent<ShipAI>().OnLock(transform);
            }

        } 
    }

    public void OnLock(Transform attacker)
    {
        shipTarget = attacker;
    }

    Vector3 AimAtTarget(Vector3 trueMovementTarget, Vector3 target, bool roll)
    {
        float horizontalProduct = Vector3.Dot(transform.right, (target - transform.position));
        float verticalProduct = Vector3.Dot(-transform.up, (target - transform.position));
        float rollProduct = 0;
        if (roll)
        {
            rollProduct = Vector3.Dot(-transform.right, trueMovementTarget - transform.position);
        }
        return new Vector3(Mathf.Clamp(rollProduct, -1, 1), Mathf.Clamp(verticalProduct, -1, 1), Mathf.Clamp(horizontalProduct, -1, 1));
    }

    Vector3 MoveToTarget(Vector3 trueMovementTarget)
    {
        float horizontalProduct = Vector3.Dot(transform.right, trueMovementTarget - transform.position);
        float verticalProduct = Vector3.Dot(transform.up, trueMovementTarget - transform.position);
        float forwardProduct = Vector3.Dot(transform.forward, trueMovementTarget - transform.position);

        return new Vector3(Mathf.Clamp(forwardProduct, -1, 1), Mathf.Clamp(horizontalProduct, -1, 1), Mathf.Clamp(verticalProduct, -1, 1));
    }
}
