using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneTrail : MonoBehaviour
{
    private const string antTag = "Ant";
    private int lifetime;
    private Food attachedFood;

    public int Lifetime
    {
        get => lifetime;
        set
        {
            lifetime = value;
            StartCoroutine(LifeTimer(lifetime));
        }
    }

    public Food AttachedFood
    {
        set
        {
            attachedFood = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(antTag) && !other.gameObject.GetComponent<Ant>().HoldingFood)
        {
            Ant tempAnt = other.gameObject.GetComponent<Ant>();
            tempAnt.PheromoneFound(attachedFood);
        }
    }

    private IEnumerator LifeTimer(int timeToDie)
    {
        yield return new WaitForSecondsRealtime(timeToDie);
        Destroy(gameObject);
    }
}
