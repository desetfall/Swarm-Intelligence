using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ant : MonoBehaviour
{
    [SerializeField]
    private GameObject pheromoneGO;

    private int pheromoneTrailLifeTime;

    [SerializeField] //Для отладки
    private bool isBrownianMotionNedeed = false;
    [SerializeField] //Для отладки
    private bool holdingFood = false;
    [SerializeField] //Для отладки
    private bool isActiveFood = false;
    private Food activeFood;
    private float speed;
    private int lifetime;

    private Vector3 antHillPose;
    [SerializeField] //Для отладки
    private Vector3 moveDirection;
    [SerializeField] //Для отладки
    private Vector3 isActiveFoodPose;
    private Vector3 lastPosOfPheromone;

    private Transform tr;

    private Anthill anthill;

    private MapController mapController;

    private NavMeshAgent agent;

    private TrailRenderer trail;

    public MapController MapController
    {
        set
        {
            mapController = value;
        }
    }

    public int Lifetime
    {
        get => lifetime;
        set
        {
            lifetime = value;
            StartCoroutine(LifeTimer(lifetime));
        }
    }

    public Anthill AntHill
    {
        get => anthill;
        set
        {
            anthill = value;
            antHillPose = anthill.GetComponent<Transform>().position;
        }
    }

    public float Speed
    {
        get => speed;
        set
        {
            speed = value;
        }
    }

    public bool HoldingFood
    {
        get => holdingFood;
        set
        {
            holdingFood = value;
            gameObject.GetComponentsInChildren<MeshRenderer>()[2].enabled = holdingFood;
        }
    }

    private void Start()
    {
        SetAgentPropertys();
        tr = transform;
        trail = gameObject.GetComponentInChildren<TrailRenderer>();      
        pheromoneTrailLifeTime = mapController.PheromoneTrailLifeTime;
        trail.time = pheromoneTrailLifeTime;
        moveDirection = Vector3.zero;
        StartCoroutine(InfiniteMotion());
        StartCoroutine(InitialMotion());
        StartCoroutine(BrownianMotion());
        StartCoroutine(StuckHandler());
        mapController.AntCount++;
    }

    private void SetAgentPropertys()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    private void Update()
    {
        //   Debug.Log("DeltaPos: " + agent.remainingDistance + "; Speed: " + agent.velocity); //velocity - Vector3
        if (holdingFood)
        {
            if (moveDirection != antHillPose)
            {
                moveDirection = antHillPose;
            } 
        }
        else
        {
            if (isActiveFood)
            {
                if (moveDirection != isActiveFoodPose)
                {
                    moveDirection = isActiveFoodPose;
                }
            }
            //new Vector3(isActiveFoodPose.x, tr.position.y, isActiveFoodPose.z)
        }
    }

    public void FoodFound(Food food)
    {
        activeFood = food;
        if (!isActiveFood)
        {
            isActiveFood = true;
            if (pheromoneTrailLifeTime != 0)
            {
                StartCoroutine(MakePheromoneTrail());
                trail.enabled = true;
            }
        }
        food.Size -= 1;
        isActiveFoodPose = food.Pose;
        isBrownianMotionNedeed = false;
    }

    public void FoodInPileIsOver()
    {
        isBrownianMotionNedeed = true;
        isActiveFood = false;
        trail.enabled = false;
    }

    public void PheromoneFound(Food food)
    {
        isBrownianMotionNedeed = false;
        moveDirection = food.Pose;
    }

    private IEnumerator InfiniteMotion()
    {
        agent.SetDestination(moveDirection);
        Vector3 currentDest = moveDirection;
        yield return new WaitUntil(() => agent.remainingDistance == 0 || currentDest != moveDirection);
        StartCoroutine(InfiniteMotion());
    }

    private IEnumerator InitialMotion()
    {
        moveDirection = new Vector3(Random.Range(-49, 50), tr.position.y, Random.Range(-49, 50));
        yield return new WaitUntil(() => agent.remainingDistance == 0 || holdingFood);
        if (!holdingFood)
        {
            isBrownianMotionNedeed = true;          
        }
    }

    private IEnumerator BrownianMotion()
    {
        if (isBrownianMotionNedeed)
        {
            float x = tr.position.x + Random.Range(-18, 18);
            float z = tr.position.z + Random.Range(-18, 18);
            if (x > 49 || x < -49 || z > 49 || z < -49)
            {
                x = Random.Range(-40, 40);
                z = Random.Range(-40, 40);
            }
            moveDirection = new Vector3(x, tr.position.y, z);
            yield return new WaitUntil(() => agent.remainingDistance == 0 || !isBrownianMotionNedeed);
        }
        else
        {
            yield return new WaitUntil(() => isBrownianMotionNedeed);
        }
        StartCoroutine(BrownianMotion());
    }

    private IEnumerator MakePheromoneTrail()
    {
        if (isActiveFood)
        {
            GameObject tempFeromoneGO = Instantiate(pheromoneGO, tr.position, Quaternion.Euler(tr.rotation.x, tr.rotation.y, tr.rotation.z));
            PheromoneTrail pheromoneTrail = tempFeromoneGO.GetComponent<PheromoneTrail>();
            pheromoneTrail.Lifetime = pheromoneTrailLifeTime;
            pheromoneTrail.AttachedFood = activeFood;
            lastPosOfPheromone = tempFeromoneGO.GetComponent<Transform>().position;
            yield return new WaitUntil(() => Mathf.Sqrt(Mathf.Pow(tr.position.x - lastPosOfPheromone.x, 2) + Mathf.Pow(tr.position.z - lastPosOfPheromone.z, 2)) > 2); //Затычка
            StartCoroutine(MakePheromoneTrail());
        }
    }

    private IEnumerator StuckHandler()
    {
        float savedRemainingDist = agent.remainingDistance;
        Vector3 savedMoveDir = moveDirection;
        yield return new WaitForSecondsRealtime(5);
        if (savedMoveDir == moveDirection && agent.remainingDistance > savedRemainingDist - 1.5f)
        {
            float x = tr.position.x + Random.Range(-20, 20);
            float z = tr.position.z + Random.Range(-20, 20);
            if (x > 49 || x < -49 || z > 49 || z < -49)
            {
                x = Random.Range(-40, 40);
                z = Random.Range(-40, 40);
            }
            moveDirection = new Vector3(x, tr.position.y, z);
            yield return new WaitUntil(() => agent.remainingDistance == 0 || !isBrownianMotionNedeed);
            moveDirection = savedMoveDir;
        }
        StartCoroutine(StuckHandler());
    }

    private IEnumerator LifeTimer(int timeToDie)
    {
        if (timeToDie != 0)
        {
            yield return new WaitForSecondsRealtime(timeToDie + Random.Range(-15, 16));
            mapController.AntCount--;
            Destroy(gameObject);
        }
    }
    /*/
    private void Update()
    {
        if (isBrownianMotionNedeed)
        {
            tr.Rotate(rotateDirection);
            if (isTimerDone)
            {
                StartCoroutine(SetRandomRotate());
            }
            isTimerDone = false;
        }
        else if (holdingFood)
        {
            tr.LookAt(new Vector3(antHillPose.x, tr.position.y, antHillPose.z));
        }
        else
        {
            tr.LookAt(new Vector3(isActiveFoodPose.x, tr.position.y, isActiveFoodPose.z));
        }
        CheckForReverseRotation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        tr.Translate(moveDirection * Time.deltaTime * speed);
    } 

    private void CheckForReverseRotation()
    {
        if (tr.position.x > 49 || tr.position.x < -49 || tr.position.z > 49 || tr.position.z < -49)
        {
            tr.LookAt(new Vector3(0, tr.position.y, 0));
        }
    }

    public void FoodFound(Food food)
    {
        food.Size -= 1;
        isActiveFoodPose = food.Pose;
        isBrownianMotionNedeed = false;
    }

    public void FoodInPileIsOver()
    {
        isBrownianMotionNedeed = true;
    }

    IEnumerator SetRandomRotate()
    {
        int random = Random.Range(1, 4);
        rotateDirection = random == 1 ? Vector3.zero : random == 2 ? Vector3.up : Vector3.down;
        yield return new WaitForSecondsRealtime(Random.Range(0.5f, 1.2f));
        rotateDirection = Vector3.zero;
        yield return new WaitForSecondsRealtime(Random.Range(5, 12));
        isTimerDone = true;
    }

    IEnumerator InitialForwardMovement()
    {
        moveDirection = Vector3.forward;
        yield return new WaitForSecondsRealtime(1f);
        isBrownianMotionNedeed = true;
    }

    IEnumerator LifeTimer(int timeToDie)
    {
        yield return new WaitForSecondsRealtime(timeToDie + Random.Range(-15, 16));
        mapController.AntCount--;
        Destroy(gameObject);
    }
    /*/
}
