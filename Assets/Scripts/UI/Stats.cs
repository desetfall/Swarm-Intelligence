using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField]
    private GameObject statsPanel;

    [SerializeField]
    private Text antCount, foodCount, foodCountInHill, simulationTimer;

    [SerializeField]
    private MapController mapController;

    private PlayerInput _input;

    private const string ANT_COUNT = "���������� ��������: ",
                         FOOD_COUNT = "���������� ��� �� ����� (���/�������.��������): ",
                         FOOD_COUNT_IN_HILL = "���������� ��� � �����������: ",
                         SIMULATION_TIMER = "����� ���������: ",
                         SIMULATION_TIMER_2 = " ���.";
                       
    private void Awake()
    {
        _input = new PlayerInput();
        _input.Player.Tab.performed += context => Tab();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Tab()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (mapController.IsSimulationStarted)
        {
            antCount.text = ANT_COUNT + mapController.AntCount.ToString();
            foodCount.text = FOOD_COUNT + mapController.FoodCount + "/" + CalculateAbsoluteFoodCount().ToString();
            foodCountInHill.text = FOOD_COUNT_IN_HILL + mapController.AntHill.FoodCount.ToString();
            simulationTimer.text = SIMULATION_TIMER + mapController.SimulationTime.ToString() + SIMULATION_TIMER_2;
        }
    }

    private int CalculateAbsoluteFoodCount()
    {
        int absoluteFoodCount = 0;
        foreach (Food food in mapController.FoodList)
        {
            absoluteFoodCount += food.Size;
        }
        return absoluteFoodCount;
    }
}
