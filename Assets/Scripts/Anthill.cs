using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anthill : MonoBehaviour
{
    private int foodCount = 0;
    private const string antTag = "Ant";

    public int FoodCount
    {
        get => foodCount;
        set
        {
            foodCount = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(antTag) && other.gameObject.GetComponent<Ant>().HoldingFood)
        {
            Ant tempAnt = other.gameObject.GetComponent<Ant>();
            tempAnt.HoldingFood = false;
            foodCount++;            
        }
    }
}
