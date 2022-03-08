using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelsAnim : MonoBehaviour
{
    [SerializeField]
    private Animator _gamePanelAnimator, _settingsPanelAnimator;
    private bool _gamePaused = true;

    private string _menuOpen = "MenuOpen";
    private string _menuClose = "MenuClose";

    private PlayerInput _input;

    private void Awake()
    {
        _input = new PlayerInput();
        _input.Player.Esc.performed += context => Esc();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    public void Esc()
    {
        if (_gamePaused)
        {
            _gamePaused = false;
            _gamePanelAnimator.SetTrigger(_menuClose);
            _settingsPanelAnimator.SetTrigger(_menuClose);
        }
        else
        {
            _gamePaused = true;
            _gamePanelAnimator.SetTrigger(_menuOpen);
            _settingsPanelAnimator.SetTrigger(_menuOpen);
        }
    }
}
