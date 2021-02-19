using System.Collections.Generic;
using System.IO;
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

    public GameObject Node;
    public List<GameObject> nodePath= new List<GameObject>();
    public LayerMask sightMask;

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
            if (shipState != ShipState.Pathing)
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
            else
            {
                shoot = false;
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
                    if (chaseTimer % 2 == 0)
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
    private int currentNode;
    private Vector3 wingmanPosition;

    public void setWingmanStation(Vector3 position)
    {
        wingmanPosition = position;
    }

    public override void HandleMovement(Vector3 movementInput, Vector3 rotationInput)
    {
        movementVector = movementTarget;
        float distanceToTarget = (movementTarget - transform.position).magnitude;
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
                if (distanceToTarget < 100)
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
            case ShipState.Pathing:
                boosting = false;
                player = null;
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").transform;
                }

                if (player != null && transform.tag == "Ally")
                {
                    if (shipTarget == null)
                    {
                        movementTarget = player.position + (player.right * (wingmanPosition.x / 10)) + (player.forward * (wingmanPosition.z / 10));
                    }
                }
                else
                {
                    if (shipTarget != null)
                    {
                        movementTarget = shipTarget.position;
                    }
                }

                if (Node != null)
                {
                    movementVector = Node.transform.position;
                }
                distanceToTarget = (transform.position - movementVector).magnitude;
                trueMovementTarget = movementVector;
                trueRotationTarget = movementVector;
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

    public bool pathFound;
    public bool pathFind;
    void PathFinder(Vector3 TargetVector)
    {
        RaycastHit hit;
        Vector3 hitPos = new Vector3();
        Debug.DrawLine(transform.position, movementTarget, Color.blue);
        if (Physics.Raycast(transform.position, TargetVector, out hit, TargetVector.magnitude, sightMask))
        {
            hitPos = hit.point;
        }

        if (hitPos != Vector3.zero)
        {
            shipState = ShipState.Pathing;
            pathFind = true;
            if (pathFound)
            {
                if (currentNode < nodePath.Count && currentNode >= 0)
                {
                    Node = nodePath[currentNode];
                    if ((transform.position - Node.transform.position).magnitude < 1)
                    {
                        currentNode -= 1;
                    }
                    hitPos = Vector3.zero;
                    if (Physics.Raycast(transform.position, Node.transform.position - transform.position, out hit, (Node.transform.position - transform.position).magnitude, sightMask))
                    {
                        hitPos = hit.point;
                    }
                    if (hitPos != Vector3.zero)
                    {
                        pathFound = false;
                        pathFind = true;
                    }
                }
                else
                {
                    pathFound = false;
                }
            }
        }
        else
        {
            if(shipState == ShipState.Pathing)
            {
                shipState = ShipState.Idle;
            }
            currentNode = 315;
            nodePath.Clear();
            pathFound = false;
            pathFind = false;
        }

        if(pathFind && !pathFound)
        {
            nodePath.Clear();
            GetNodes(movementTarget, transform.position);
            if (!pathFound)
            {
                shipState = ShipState.Idle;
            }
        }
    }

    void GetNodes(Vector3 StartPosition, Vector3 TargetPosition)
    {
        Vector3 hitPos = new Vector3();
        Vector3 nextNode = StartPosition;
        GameObject[] foundNodes = GameObject.FindGameObjectsWithTag("Node");
        int ClosestStartNode = 0;
        for (int i = 0; i < 30; i++)
        {
            hitPos = Vector3.zero;
            Debug.DrawLine(nextNode, TargetPosition, Color.blue);
            if (Physics.Raycast(nextNode, TargetPosition - nextNode, out RaycastHit hit, (TargetPosition - nextNode).magnitude, sightMask))
            {
                hitPos = hit.point;
            }
            if (hitPos != Vector3.zero)
            {
                GameObject[] newNodes = FindAvailableNodes(foundNodes, nextNode);
                ClosestStartNode = FindShortestDistance(newNodes, TargetPosition);
                nextNode = newNodes[ClosestStartNode].transform.position;
                nodePath.Add(newNodes[ClosestStartNode]);
            }
            else
            {
                pathFound = true;
                pathFind = false;
                currentNode = nodePath.Count - 1;
                return;
            }
        }
        pathFind = false;
    }

    GameObject[] FindAvailableNodes(GameObject[] Nodes, Vector3 StartPosition)
    {
        List<GameObject> availableNodes = new List<GameObject>();
        for (int n = 0; n < Nodes.Length; n++)
        {
            Vector3 CurrentNode = Nodes[n].transform.position - StartPosition;
            Vector3 hitPos = new Vector3();
            if (Physics.Raycast(StartPosition, CurrentNode, out RaycastHit hit, CurrentNode.magnitude, sightMask))
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
            bool cancel = false;
            if (CurrentNode.magnitude < ClosestNode.magnitude)
            {
                foreach (GameObject node in nodePath)
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
        Debug.DrawRay(transform.position, direction * Mathf.Clamp(positive, 2, 20), Color.green);
        Debug.DrawRay(transform.position, -direction * Mathf.Clamp(-negative, 2, 20), Color.red);

        if (shipRigid.SweepTest(direction, out RaycastHit hitInfo, Mathf.Clamp(positive, 2, 20), QueryTriggerInteraction.Ignore))
        {
            //positive = hitInfo.distance;
            collider = hitInfo.collider;
        }


        if (shipRigid.SweepTest(-direction, out hitInfo, Mathf.Clamp(-negative, 2, 20), QueryTriggerInteraction.Ignore))
        {
            //negative = hitInfo.distance;
            collider2 = hitInfo.collider;
        }

        if (collider != null)
        {
            if (collider2 != null)
            {
                boosting = false;
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
                boosting = true;
                directionInput = -1;
            }
        }
        else if (collider2 != null)
        {
            boosting = true;
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
