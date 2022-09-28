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

    private Vector3 Node;
    public List<Vector3> nodePath= new List<Vector3>();
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
                    missiles?.FireMissile(shipTarget);
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

                if(distanceToShipTarget > 100){
                    shipState = ShipState.Orbitting;
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

    public override void HandleMovement(Vector3 movementInput, Vector3 rotationInput, float maxSpeedMultiplier)
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

                trueMovementTarget = (transform.position + (transform.forward * 10) + (transform.right * direction));
                trueRotationTarget = trueMovementTarget;

                break;
            case ShipState.Orbitting:

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
                    trueMovementTarget = player.position + (player.right * (wingmanPosition.x / 10)) + (player.forward * (wingmanPosition.z / 10));
                    //movementTarget = trueMovementTarget;
                    trueRotationTarget = player.forward * 10 + transform.position;
                    rollTarget = transform.position + player.up;
                    maxSpeedMultiplier = Mathf.Clamp((distanceToTarget / 30), 0.01f, 1);
                    Debug.Log(maxSpeedMultiplier);
                    roll = true;
                }
                break;
            case ShipState.Pathing:
                maxSpeedMultiplier = 0.3f;
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
                    movementVector = Node;
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
        
        base.HandleMovement(movementInput, rotationInput, maxSpeedMultiplier);
    }

    private bool pathFound;
    private bool pathFind;
    void PathFinder(Vector3 TargetVector)
    {
        RaycastHit hit;
        Vector3 hitPos = new Vector3();
        Debug.DrawLine(transform.position, movementTarget, Color.blue);
        //Check to see if the AI can reach the target with a raycast.
        if (Physics.Raycast(transform.position, TargetVector, out hit, TargetVector.magnitude, sightMask))
        {
            hitPos = hit.point;
        }

        //If there is an obstacle, the hitPos will not be at (0,0,0).
        if (hitPos != Vector3.zero)
        {
            //Debugging purposes, shows the path found.
            for(int i=0;i<nodePath.Count - 1;i++){
             Debug.DrawLine(nodePath[i], nodePath[i+1], Color.yellow);
            }

            //Force the shipstate into pathing
            shipState = ShipState.Pathing;
            //find a path

            //if a path is found, begin the movement
            if (pathFound)
            {
                //Since the nodePath list is used like a Stack, the last inserted object will be the first port of the path.
                //this checks if the currentNode is within the nodePath list. If it isn't, find that fucking path again.
                if (currentNode < nodePath.Count && currentNode >= 0)
                {
                    //get the node
                    Node = nodePath[currentNode];
                    if (Physics.Raycast(transform.position, Node - transform.position, out hit, (Node - transform.position).magnitude, sightMask))
                    {
                        hitPos = hit.point;
                    }
                    //if the AI reaches within a certain distance of the node, move on to the enxt one
                    if ((transform.position - Node).magnitude <= 10)
                    {
                        currentNode -= 1;
                    }
                    hitPos = Vector3.zero;
                    //check to see if the next node is reachable in the path
                    if (Physics.Raycast(transform.position, Node - transform.position, out hit, (Node - transform.position).magnitude, sightMask))
                    {
                        hitPos = hit.point;
                    }
                    //if it isn't, recalculate the path.
                    if (hitPos != Vector3.zero)
                    {
                        currentNode -= 1;
                        if(currentNode < 0){
                            pathFound = false;
                            pathFind = true;
                        }
                    }
                }
                else
                {
                    //stupid thought there was a path.
                    pathFound = false;
                }
            }else{
                pathFind = true;
            }
        }
        else
        {
            //reset the pathfinder if it is no longer needed.
            if(shipState == ShipState.Pathing)
            {
                shipState = ShipState.Idle;
            }
            //there won't be more than 315 nodes, right? Right???
            currentNode = 315;
            nodePath.Clear();
            pathFound = false;
            pathFind = false;
        }

        //if There isn't a path found and a path is needed, run this.
        if(pathFind && !pathFound)
        {
            //clear the nodepath to get a fresh one
            nodePath.Clear();
            nodePath.Add(movementTarget);
            //Generate that node path baby. Start from the target to the AI's position
            GetNodes(movementTarget, transform.position);
            //if a path still cannot be found, the AI will Idle about until a path can be found.
            if (!pathFound)
            {
                shipState = ShipState.Idle;
            }
        }
    }

    //This is where the magic happens.
    void GetNodes(Vector3 StartPosition, Vector3 TargetPosition)
    {
        Vector3 hitPos = new Vector3();
        //set the starting node as the starting position. Essentially treats the target position as a node itself.
        Vector3 nextNode = StartPosition;
        //Find all of the nodes in the scene.
        GameObject[] foundNodes = GameObject.FindGameObjectsWithTag("Node");
        int ClosestStartNode = 0;
        //Nodepaths will have a maximum length of 30. I don't know why this arbitrary number is here
        //ask past me.
        int i=0;
        while(i < 30 && !pathFound)
        {
            //check if the node can be seen by the AI
            hitPos = Vector3.zero;
            Debug.DrawLine(nextNode, TargetPosition, Color.blue);
            if (Physics.Raycast(nextNode, TargetPosition - nextNode, out RaycastHit hit, (TargetPosition - nextNode).magnitude, sightMask))
            {
                hitPos = hit.point;
            }

            if (hitPos != Vector3.zero)
            {
                //if it isn't, continue pathing
                //first, find the next nodes that can be reached from the current node.
                GameObject[] newNodes = FindAvailableNodes(foundNodes, nextNode);
                //Then, find the closest node out of the available nodes to the goal
                ClosestStartNode = FindShortestDistance(newNodes, TargetPosition, transform.position);
                //set the next node to be iterated
                nextNode = newNodes[ClosestStartNode].transform.position;
                //save the node in the path.
                nodePath.Add(newNodes[ClosestStartNode].transform.position);
                //repeat for 30 times or a clear path is found.
            }
            else
            {
                //if it is, then a valid path has been found and the AI can proceed.
                pathFound = true;
                nodePath.Add(transform.position);
                nodePath = BerzierTest.UpdateLine(nodePath);
                currentNode = nodePath.Count - 1;
            }
        }
        pathFind = false;
    }

    GameObject[] FindAvailableNodes(GameObject[] Nodes, Vector3 StartPosition)
    {
        //Create a new list.
        List<GameObject> availableNodes = new List<GameObject>();
        //loop through all nodes given.
        for (int n = 0; n < Nodes.Length; n++)
        {
            //Get the vector from the current node to the checked Node and check if it is a clear vector,
            Vector3 CurrentNode = Nodes[n].transform.position - StartPosition;
            Vector3 hitPos = new Vector3();
            if (Physics.Raycast(StartPosition, CurrentNode, out RaycastHit hit, CurrentNode.magnitude, sightMask))
            {
                hitPos = hit.point;
            }
            //if the node can be reached, it is an available
            if (hitPos == Vector3.zero)
            {
                availableNodes.Add(Nodes[n]);
            }
        }
        //convert from list to Array, I don't know why I did this, my past monkey brain wanted things to be this stupid.
        GameObject[] newNodes = new GameObject[availableNodes.Count];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = availableNodes[n];
        }
        return newNodes;
    }

    int FindShortestDistance(GameObject[] Nodes, Vector3 startPosition, Vector3 goalPosition)
    {
        //oh boy more tomfoolery. This finds which node is the shortest distance out of the list.
        int ClosestNodeIndex = 0;
        for (int n = 0; n < Nodes.Length; n++)
        {
            //get the current checked node vector and the closest node vector.
            float CurrentNodeHDistance = (startPosition - Nodes[n].transform.position).magnitude + (goalPosition - Nodes[n].transform.position).magnitude;
            float ClosestNodeHDistance = (startPosition - Nodes[ClosestNodeIndex].transform.position).magnitude + (goalPosition - Nodes[ClosestNodeIndex].transform.position).magnitude;
            //    Vector3 CurrentNode = startPosition - Nodes[n].transform.position
            //    Vector3 ClosestNode = startPosition - Nodes[ClosestNodeIndex].transform.position;
            bool cancel = false;
            //if the current checked node is closer than the closest node, become the new closest node
            if (CurrentNodeHDistance < ClosestNodeHDistance)
            {
                //loop through each node in the current nodepath to ensure it isn't a node going backwards.
                foreach (Vector3 node in nodePath)
                {
                    //if the current node does exist in the node path, ignore it
                    if (Nodes[n].transform.position == node)
                    {
                        cancel = true;
                    }
                }
                //if it isn't in the node path, remember the node.
                if (!cancel)
                {
                    ClosestNodeIndex = n;
                }
            }
        }
        //return the node found.S
        return ClosestNodeIndex;
    }

    public override void OnDeath()
    {
        float distanceToShipTarget = (transform.position - shipTarget.position).magnitude;
        shipTarget.GetComponent<ShipSystem2>().OnKill(distanceToShipTarget);
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
        float horizontalProduct = Vector3.Dot(transform.right, (trueMovementTarget - transform.position));
        float verticalProduct = Vector3.Dot(transform.up, (trueMovementTarget - transform.position));
        float forwardProduct = Vector3.Dot(transform.forward, (trueMovementTarget - transform.position));

        return new Vector3(Mathf.Clamp(forwardProduct, -1, 1), Mathf.Clamp(horizontalProduct, -1, 1), Mathf.Clamp(verticalProduct, -1, 1));
    }
}
