using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _player, _ppVolume;

    [SerializeField]
    private Slider _mouseSensSlider;

    [SerializeField]
    private Toggle _graphicsToggle;

    private CameraController _playerController;

    private void Start()
    {
        _playerController = _player.GetComponent<CameraController>();
    }

    public void ChangeSensValue()
    {
        _playerController.ChangeMouseSens((int)_mouseSensSlider.value);
    }

    public void ToggleGraphics()
    {
        _ppVolume.SetActive(_graphicsToggle.isOn);
    }
}
