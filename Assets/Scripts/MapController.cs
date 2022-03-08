using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private GameObject _ant, _antHill, _food, _terrain;

    private List<int> propertys; //_antCount, _foodCount, _lifeTime, _antSpeed, _foodDist, _trackDist, _deltaAnt, _deltaFood, _pheromoneTime;

    private Vector3 antHillPos;

    private List<Vector3> foodPoses;
    private List<int> foodSizes;

    private List<Ant> antList;
    private List<Food> foodList;
    private Anthill antHill;

    private int foodCount = 0;
    private int simulationTime = 0;
    private int antCount = 0;

    private bool isSimulationStarted = false;

    private NavMeshSurface navMeshSurface;

    public bool IsSimulationStarted
    {
        get => isSimulationStarted;
    }

    public int FoodCount
    {
        get => foodCount;
        set
        {
            foodCount = value;
        }
    }
    public int SimulationTime
    {
        get => simulationTime;
    }

    public int AntCount
    {
        get => antCount;
        set
        {
            antCount = value;
        }
    }

    public List<Food> FoodList
    {
        get => foodList;
    }

    public Anthill AntHill
    {
        get => antHill;
    }

    public int PheromoneTrailLifeTime
    {
        get => propertys[8];
    }

    private void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    public void StartSimulation(List<int> values)
    {
        isSimulationStarted = true;
        propertys = values;
        GenerateMap();
        //Camera.main.transform.LookAt(antHillPos);
        StartCoroutine(SimulationTimer());
    }

    public void StopSimulation()
    {
        isSimulationStarted = false;
        simulationTime = 0;
        antCount = 0;
        Destroy(antHill.gameObject);
        for (int i = 0; i < foodList.Count; i++)
        {
            Destroy(foodList[i].gameObject);
        }
        for (int i = 0; i < antList.Count; i++)
        {
            Destroy(antList[i].gameObject);
        }
        PheromoneTrail[] pheromoneTrails = FindObjectsOfType<PheromoneTrail>();
        for (int i = 0; i < pheromoneTrails.Length; i++)
        {
            Destroy(pheromoneTrails[i].gameObject);
        }
    }

    private void GenerateMap()
    {
        CalculateStartFoodSizes();
        CalculateStartCoords();
        antHill = Instantiate(_antHill, antHillPos, Quaternion.identity).GetComponent<Anthill>();
        MakeFood();
        navMeshSurface.BuildNavMesh();
        MakeAnts();
    }

    private void CalculateStartFoodSizes()
    {
        //foodSizes = new List<int>(propertys[1]);
        foodSizes = new List<int>();
        for (int i = 0; i < propertys[1]; i++)
        {
            int foodSize = Random.Range(1, 6);
            foodSizes.Add(foodSize * 10);
        }
    }

    private void CalculateStartCoords()
    {
        antHillPos = new Vector3(Random.Range(-25, 25), 0, Random.Range(-25, 25));
        foodPoses = new List<Vector3>();
        for (int i = 0; i < propertys[1]; i++)
        {
            Vector3 pos = new Vector3(0, 0, 0);
            while (CheckFoodPoseToCollisionWithOtherFood(pos) || CheckFoodPoseToCollisionWithAnthill(pos) || pos == Vector3.zero)
            {
                pos.x = Random.Range(-47.5f, 47.5f);
                pos.z = Random.Range(-47.5f, 47.5f);
            }
            pos.y = (foodSizes[i] / 20.0f) - 0.5f;
            foodPoses.Add(pos);
        }
    }

    private bool CheckFoodPoseToCollisionWithAnthill(Vector3 pos)
    {
        float cubeRadius = foodSizes[foodPoses.Count] / 20.0f;
        Vector3 leftDownAngle = new Vector3(pos.x - cubeRadius, pos.y, pos.z - cubeRadius);
        Vector3 vectorSize = new Vector3((pos.x + cubeRadius) - leftDownAngle.x, pos.y, (pos.z + cubeRadius) - leftDownAngle.z);
        Vector3 antHillLeftDownAngle = new Vector3(antHillPos.x - 5, antHillPos.y, antHillPos.z - 5);
        Vector3 antHillVectorSize = new Vector3((antHillPos.x + 5) - antHillLeftDownAngle.x, antHillPos.y, (antHillPos.z + 5) - antHillLeftDownAngle.z);
        return IsTwoRectsOverlap(leftDownAngle, vectorSize, antHillLeftDownAngle, antHillVectorSize);
    }

    private bool CheckFoodPoseToCollisionWithOtherFood(Vector3 pos)
    {
        float cubeRadius = foodSizes[foodPoses.Count] / 20.0f;
        Vector3 leftDownAngle = new Vector3(pos.x - cubeRadius, pos.y, pos.z - cubeRadius); //1
        Vector3 vectorSize = new Vector3((pos.x + cubeRadius) - leftDownAngle.x, pos.y, (pos.z + cubeRadius) - leftDownAngle.z);
        for (int i = 0; i < foodPoses.Count; i++)
        {
            float tempCubeRadius = foodSizes[i] / 20.0f;
            Vector3 tempLeftDownAngle = new Vector3(foodPoses[i].x - tempCubeRadius, foodPoses[i].y, foodPoses[i].z - tempCubeRadius); //2
            Vector3 tempVectorSize = new Vector3((foodPoses[i].x + tempCubeRadius) - tempLeftDownAngle.x, foodPoses[i].y, (foodPoses[i].z + tempCubeRadius) - tempLeftDownAngle.z);
            if (IsTwoRectsOverlap(leftDownAngle, vectorSize, tempLeftDownAngle, tempVectorSize))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsTwoRectsOverlap(Vector3 leftDownAngleOne, Vector3 vectorSizeOne, Vector3 tempLeftDownAngleOne, Vector3 tempVectorSizeOne)
    {
        if ((leftDownAngleOne.x < tempLeftDownAngleOne.x + tempVectorSizeOne.x) &&
                (leftDownAngleOne.z < tempLeftDownAngleOne.z + tempVectorSizeOne.z) &&
                (tempLeftDownAngleOne.x < leftDownAngleOne.x + vectorSizeOne.x) &&
                (tempLeftDownAngleOne.z < leftDownAngleOne.z + vectorSizeOne.z))
        {
            return true;
        }
        return false;
    }

    private void MakeFood() //deltaFood - 7
    {
        foodCount = propertys[1];
        //foodList = new List<Food>(propertys[1]);
        foodList = new List<Food>();
        for (int i = 0; i < propertys[1]; i++)
        {
            GameObject tempFoodObj = Instantiate(_food, foodPoses[i], Quaternion.identity);
            Food tempFood = tempFoodObj.GetComponent<Food>();
            tempFood.Size = foodSizes[i];
            foodList.Add(tempFood);
        }
        if (propertys[7] != 0)
        {
            StartCoroutine(MakeNewFood(propertys[7]));
        }
    }

    private IEnumerator MakeNewFood(int deltaTime)
    {
        if (isSimulationStarted)
        {
            yield return new WaitForSecondsRealtime(deltaTime);
            if (foodCount < 150)
            {
                foodCount++;

                int newFoodSize = Random.Range(1, 6);
                foodSizes.Add(newFoodSize * 10);

                Vector3 pos = new Vector3(0, 0, 0);

                while (CheckFoodPoseToCollisionWithOtherFood(pos) || CheckFoodPoseToCollisionWithAnthill(pos) || pos == Vector3.zero)
                {
                    pos.x = Random.Range(-47.5f, 47.5f);
                    pos.z = Random.Range(-47.5f, 47.5f);
                }

                pos.y = (foodSizes[foodSizes.Count - 1] / 20.0f) - 0.5f;
                foodPoses.Add(pos);

                GameObject tempFoodObj = Instantiate(_food, foodPoses[foodPoses.Count - 1], Quaternion.identity);
                Food tempFood = tempFoodObj.GetComponent<Food>();
                tempFood.Size = foodSizes[foodSizes.Count - 1];
                foodList.Add(tempFood);
            }
            else
            {
                yield return new WaitUntil(() => foodCount < 150);
            }
            StartCoroutine(MakeNewFood(deltaTime));
        }
    }

    private void MakeAnts() //deltaAnt - 6
    {
        //antList = new List<Ant>(propertys[0]);
        antList = new List<Ant>();
        float cornerForOneAnt = 360.0f / propertys[0];
        for (int i = 0; i < propertys[0]; i++)
        {
            GameObject tempAntObj = Instantiate(_ant, antHillPos, Quaternion.Euler(0.0f,cornerForOneAnt,0.0f));
            Ant tempAnt = tempAntObj.GetComponent<Ant>();
            tempAnt.MapController = this;
            tempAnt.Lifetime = propertys[2];
            tempAnt.AntHill = antHill;
            tempAnt.Speed = propertys[3];
            antList.Add(tempAnt);
            cornerForOneAnt += cornerForOneAnt;
        }
        if (propertys[6] != 0)
        {
            StartCoroutine(MakeNewAnt(propertys[6]));
        }
    }

    private IEnumerator MakeNewAnt(int deltaFoods)
    {
        if (isSimulationStarted)
        {
            int expectedAmountOfFood = antHill.FoodCount + deltaFoods;
            yield return new WaitUntil(() => antHill.FoodCount == expectedAmountOfFood);

            GameObject tempAntObj = Instantiate(_ant, antHillPos, Quaternion.identity);
            Ant tempAnt = tempAntObj.GetComponent<Ant>();
            tempAnt.MapController = this;
            tempAnt.Lifetime = propertys[2];
            tempAnt.AntHill = antHill;
            tempAnt.Speed = propertys[3];
            antList.Add(tempAnt);

            StartCoroutine(MakeNewAnt(deltaFoods));
        }
    }

    private IEnumerator SimulationTimer()
    {
        if (isSimulationStarted)
        {
            yield return new WaitForSecondsRealtime(1);
            simulationTime++;
            StartCoroutine(SimulationTimer());
        }
    }
}
/*/
float cubeRadius = foodSizes[index] / 2.0f;
Vector3 frondEdge = new Vector3(pos.x, pos.y, pos.z + cubeRadius);
Vector3 backEdge = new Vector3(pos.x, pos.y, pos.z - cubeRadius);
Vector3 rightEdge = new Vector3(pos.x + cubeRadius, pos.y, pos.z);
Vector3 leftEdge = new Vector3(pos.x - cubeRadius, pos.y, pos.z);

Vector3 leftTopEdge = new Vector3(pos.x - cubeRadius, pos.y, pos.z + cubeRadius);
Vector3 leftDownEdge = new Vector3(pos.x - cubeRadius, pos.y, pos.z - cubeRadius);
Vector3 rightTopEdge = new Vector3(pos.x + cubeRadius, pos.y, pos.z + cubeRadius);
Vector3 rightDownEdge = new Vector3(pos.x + cubeRadius, pos.y, pos.z - cubeRadius);

    private bool CheckFoodPoseToCollision(Vector3 pos)
    {
        for (int i = 0; i < foodPoses.Count; i++)
        {
            if (pos == foodPoses[i] || CheckFoodPoseToCollisionAccordingToSize(pos, i))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckFoodPoseToCollisionAccordingToSize(Vector3 pos, int index)
    {
        float cubeRadius = foodSizes[index] / 2.0f;
        Vector3 leftDownAngle = new Vector3(pos.x - cubeRadius, pos.y, pos.z - cubeRadius); //1
        Vector3 vectorSize = new Vector3((pos.x + cubeRadius) - leftDownAngle.x, pos.y, (pos.z + cubeRadius) - leftDownAngle.z);
        for (int i = 0; i < foodPoses.Count; i++)
        {
            float tempCubeRadius = foodSizes[i] / 2.0f;
            Vector3 tempLeftDownAngle = new Vector3(foodPoses[i].x - tempCubeRadius, foodPoses[i].y, foodPoses[i].z - tempCubeRadius); //2
            Vector3 tempVectorSize = new Vector3((foodPoses[i].x + tempCubeRadius) - tempLeftDownAngle.x, foodPoses[i].y, (foodPoses[i].z + tempCubeRadius) - tempLeftDownAngle.z);
            if ((leftDownAngle.x < tempLeftDownAngle.x + tempVectorSize.x) &&
                (leftDownAngle.z < tempLeftDownAngle.z + tempVectorSize.z) &&
                (tempLeftDownAngle.x < leftDownAngle.x + vectorSize.x) &&
                (tempLeftDownAngle.z < leftDownAngle.z + vectorSize.z))
            {
                return true;
            }
        }
        return false;
    }
/*/