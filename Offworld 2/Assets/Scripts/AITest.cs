using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AITest : ShipSystem2
{

    public Vector3 movementTarget;
    public float distanceToTarget;

    public GameObject marker;
    public float engagementRange;
    private UIElementSystem markerUI;
    private Vector3 currentInput;
    public Transform canvas;
    private float chaseTimer;
    private float shootTimer;
    private float shootTime;
    public bool breakOff;
    public bool shoot;

    public override void OnStart()
    {
        engagementRange = engagementRange * 0.1f;
        GameObject tempObj = Instantiate(marker, canvas) as GameObject;
        markerUI = tempObj.GetComponent<UIElementSystem>();
    }

    public override void OnUpdate()
    {
            if (shipTarget != null)
            {
                float distanceToShipTarget = (transform.position - shipTarget.position).magnitude;
                if (distanceToShipTarget < engagementRange && !PathFinding)
                {
                    if (shootTimer < shootTime)
                    {
                        shootTimer += Time.deltaTime;
                    }
                    else
                    {
                        if (shoot)
                        {
                            shoot = false;
                        }
                        else
                        {

                            shoot = true;
                        }
                        shootTime = Random.Range(1, 3);
                        shootTimer = 0;
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
                    if (breakOff)
                    {
                        SelectTarget();
                        breakOff = false;
                    }
                    else
                    {

                        breakOff = true;
                    }
                    movementTarget = Vector3.zero;
                    chaseTimer = Random.Range(1, 5);
                }

                PathFinder(shipTarget.position - transform.position);
            }
            else
            {
            PathFinder(movementTarget - transform.position);
            SelectTarget();
                gunSystem.Shoot = false;
            }

            markerUI.iconPosition = transform.position;

    }
    public void OnLock(Transform attacker)
    {
        shipTarget = attacker;
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
            if (shipTarget.tag != "Player")
            {
                shipTarget.GetComponent<AITest>().OnLock(transform);
            }
            
        }


    }

    Vector3 AimAtTarget(Vector3 trueMovementTarget, Vector3 target, bool roll)
    {
        float horizontalProduct = Vector3.Dot(transform.right, (target - transform.position).normalized);
        float verticalProduct = Vector3.Dot(-transform.up, (target - transform.position).normalized);
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

    Vector3 StrafeTarget()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), Random.Range(-30f, 30f));

        Vector3 movementTarget = randomPosition;

        return movementTarget;
    }

    Vector3 BreakOff()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));

        Vector3 movementTarget = randomPosition;

        boosting = true;

        return movementTarget;
    }

    public Vector3 testPosition;
    private float randomNumber;

    public override void HandleMovement(Vector3 movementInput, Vector3 rotationInput, float maxSpeedMultiplier)
    {
        movementInput = new Vector3();
        rotationInput = new Vector3();
        if (PathFinding && Node != null)
        {
            boosting = true;
            Transform player = null;
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            Vector3  trueMovementTarget = Vector3.zero;
            if (shipTarget == null)
            {
                trueMovementTarget = player.position + (player.right * (testPosition.x / 100)) + (player.forward * (testPosition.z / 100));
                movementTarget = trueMovementTarget;
            }
            trueMovementTarget = Node.transform.position;
            movementInput = MoveToTarget(trueMovementTarget);
            movementInput *= Mathf.Clamp(0.5f * Mathf.Pow(Mathf.Clamp(distanceToTarget, 0, 30) - 0, 2) + 0.2f, 0.5f, 1.5f);
            shipMovementValues.speedMultiplier = Mathf.Clamp(0.5f * Mathf.Pow(Mathf.Clamp(distanceToTarget, 0, 30) - 0, 2) + 0.2f, 0.5f, 1.9f);
            rotationInput = AimAtTarget(transform.up, trueMovementTarget, true);
        }
        else
        {
            if (shipTarget != null)
            {
                boosting = false;
                Vector3 trueMovementTarget = shipTarget.position + movementTarget - (shipTarget.forward * 40);
                distanceToTarget = (transform.position - trueMovementTarget).magnitude;
                if (distanceToTarget < 0.1f || movementTarget == Vector3.zero)
                {
                    if (!breakOff)
                    {
                        movementTarget = StrafeTarget();
                        randomNumber = Random.Range(0, 100);
                    }
                    else
                    {
                        movementTarget = BreakOff();
                        randomNumber = Random.Range(0, 100);
                    }
                }

                if (breakOff) trueMovementTarget = movementTarget;


                bool rolling = randomNumber > 50 ? true : false;

                rotationInput = AimAtTarget(trueMovementTarget, shipTarget.position, rolling);

                movementInput = MoveToTarget(trueMovementTarget);

            }
            else
            {
                Transform player = null;
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").transform;
                }
                Vector3 trueMovementTarget = Vector3.zero;
                Vector3 target = Vector3.zero;

                if (player != null && transform.tag == "Ally")
                {
                    boosting = true;
                    target = player.forward * 10 + transform.position;
                    trueMovementTarget = player.position + (player.right * (testPosition.x / 10)) + (player.forward * (testPosition.z / 10));
                    distanceToTarget = (transform.position - trueMovementTarget).magnitude;
                    movementInput = MoveToTarget(trueMovementTarget);
                    movementInput *= Mathf.Clamp(0.5f * Mathf.Pow(Mathf.Clamp(distanceToTarget, 0, 30) - 0, 2) + 0.2f, 0.5f, 1.5f);
                    shipMovementValues.speedMultiplier = Mathf.Clamp(0.5f * Mathf.Pow(Mathf.Clamp(distanceToTarget, 0, 30) - 0, 2) + 0.2f, 0.5f, 1.9f);

                    movementTarget = trueMovementTarget;
                    rotationInput = AimAtTarget(trueMovementTarget + player.up, target, true);
                }
                else
                {
                    movementInput = Vector3.zero;
                    rotationInput = Vector3.zero;
                }
            }
        }

        movementInput = HandleCollisionDetection(movementInput);
        currentInput = Vector3.Lerp(currentInput, movementInput, Time.deltaTime);
        base.HandleMovement(movementInput, rotationInput, 1);
    }


    Vector3 HandleCollisionDetection(Vector3 input)
    {
        input.x = CheckCollisionOnAxis(transform.forward, input.x);
        input.y = CheckCollisionOnAxis(transform.right, input.y);
        input.z = CheckCollisionOnAxis(transform.up, input.z);
        input = CheckCollisionOnVelocity(shipRigid.velocity, input);
        return input;
    }

    Vector3 CheckCollisionOnVelocity(Vector3 direction, Vector3 input)
    {
        Collider collider = new Collider();
        if (shipRigid.SweepTest(direction.normalized, out RaycastHit hitInfo, Mathf.Clamp(shipRigid.velocity.magnitude, 0, 2), QueryTriggerInteraction.Ignore))
        {
            collider = hitInfo.collider;
        }

        if (collider != null)
        {
            float forwardVelocity = Vector3.Dot(transform.forward, shipRigid.velocity);
            float rightVelocity = Vector3.Dot(-transform.right, shipRigid.velocity);
            float upVelocity = Vector3.Dot(-transform.up, shipRigid.velocity);
            input = new Vector3(Mathf.Clamp(-forwardVelocity * 10, -1, 1), Mathf.Clamp(-rightVelocity * 10, -1, 1), Mathf.Clamp(-upVelocity * 10, -1, 1));
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
        //float positiveWeight = 0;
        //float directional = Mathf.Clamp(Vector3.Dot(direction, shipRigid.velocity.normalized), -0.75f, 0.75f);
        //if (collider != null) {
        //    positiveWeight = Mathf.Clamp((positive / 20) - directional, -2, 2);
        //}
        //else
        //{
        //    positiveWeight = 2;
        //}

        //float negativeWeight = 0;
        //directional = Mathf.Clamp(Vector3.Dot(-direction, shipRigid.velocity.normalized), -1, 1);
        //if (collider != null)
        //{

        //    negativeWeight = Mathf.Clamp((negative / 20), -2, 2);
        //}
        //else
        //{
        //    negativeWeight = 2 + directional;
        //}

        //if(collider == null && collider2 == null)
        //{
        //    return directionInput;
        //}

        //if(positiveWeight > negativeWeight)
        //{
        //    return 1;
        //}
        //else
        //{
        //    return -1;
        //}

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

    public GameObject Node;
    public List<GameObject> previousNodes = new List<GameObject>();
    public GameObject[] seenNodes;
    public bool PathFinding;
    public LayerMask sightMask;

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
                }catch {
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
                if(hitPos != Vector3.zero)
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
                    foreach(GameObject node in previousNodes)
                    {
                        if(Nodes[n] == node)
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
}
