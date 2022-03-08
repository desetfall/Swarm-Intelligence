using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    private const string WARNING_MSG_VALUE_ERROR = "Неправильно введены данные, проверьте поля ввода параметров!",
                         WARNING_MSG_VALUE_TOO_LARGE = "Значения данных не попадают в допустимый диапазон!";

    [SerializeField]
    private GameObject _player, _statsPanel;

    [SerializeField]
    private Button _btnStartSimulation, _btnStopSimulation;

    [SerializeField]
    private List<InputField> _inputFields;
    //private InputField _antCount, _foodCount, _lifeTime, _antSpeed, _foodDist, _trackDist, _deltaAnt, _deltaFood, _pheromoneTime;

    [SerializeField]
    private Text _warningMessage;

    private bool _isSimulationStarted = false;

    private CameraController _playerController;
    private PanelsAnim _panelsAnim;

    private void Start()
    {
        _playerController = _player.GetComponent<CameraController>();
        _panelsAnim = _player.GetComponent<PanelsAnim>();
        
        List<int> propertys = new List<int>() { 50, 10, 200, 3, 1, 1, 5, 150, 20 };
        for (int i = 0; i < _inputFields.Count; i++)
        {
            _inputFields[i].text = propertys[i].ToString();
        }
        
    }

    public void StartSimulation()
    {
        _warningMessage.text = string.Empty;
        List<int> propertys = new List<int>();
        foreach (InputField inpf in _inputFields)
        {
            int value;
            if (!int.TryParse(inpf.text, out value))
            {
                _warningMessage.text = WARNING_MSG_VALUE_ERROR;
                return;
            }
            else
            {
                propertys.Add(value);
            }
        }
        if (propertys[0] > 1000 || propertys[0] < 1)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        if (propertys[1] > 150 || propertys[1] < 0)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        if (propertys[2] < 0)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        if (propertys[3] > 100 || propertys[3] < 1)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        if (propertys[4] > 20 || propertys[4] < 1)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        if (propertys[5] > 20 || propertys[5] < 1)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        if (propertys[6] < 0)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        if (propertys[7] < 0)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        if (propertys[8] < 0)
        {
            _warningMessage.text = WARNING_MSG_VALUE_TOO_LARGE;
            return;
        }
        _panelsAnim.Esc();
        _playerController.SwipeControllerPauseState();
        SwipeSimulationStatus();
        gameObject.GetComponent<MapController>().StartSimulation(propertys);
        _statsPanel.SetActive(true);
    }

    public void StopSimulation()
    {
        SwipeSimulationStatus();
        gameObject.GetComponent<MapController>().StopSimulation();
        _statsPanel.SetActive(false);
    }

    private void SwipeSimulationStatus()
    {
        _btnStartSimulation.interactable = _isSimulationStarted;
        _isSimulationStarted = !_isSimulationStarted;   
        _btnStopSimulation.interactable = _isSimulationStarted;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
