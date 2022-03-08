using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField]
    private MapController mapController;

    private int size;
    private Vector3 pose;
    private Transform tr;
    private const string antTag = "Ant";

    private bool foodActive = true;

    public Vector3 Pose
    {
        get => pose;
    }

    public int Size
    {
        get => size;
        set
        {
            size = value;
            if (size > 10)
            {
                float scale = (float)size / 10.0f;
                gameObject.transform.localScale = new Vector3(scale,scale,scale);
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, (scale / 2.0f - 0.5f), gameObject.transform.position.z);
            }
        }
    }
    private void Start()
    {
        tr = transform;
        pose = tr.position;
        mapController = FindObjectOfType<MapController>().GetComponent<MapController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(antTag) && !other.gameObject.GetComponent<Ant>().HoldingFood)
        {
            Ant tempAnt = other.gameObject.GetComponent<Ant>();
            if (size == 0)
            {
                //Destroy(gameObject);
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                if (foodActive)
                {
                    foodActive = false;
                    mapController.FoodCount--;
                }
                tempAnt.FoodInPileIsOver();
                tempAnt.HoldingFood = false;
            }
            else
            {
                tempAnt.FoodFound(gameObject.GetComponent<Food>());
                tempAnt.HoldingFood = true;
            }
        }
    }
}
