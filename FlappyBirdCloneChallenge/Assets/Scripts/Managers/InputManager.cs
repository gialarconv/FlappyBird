using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)] //execute first
public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance => _instance;

    public delegate void StartJumpEvent();
    public event StartJumpEvent OnStartJump;
    public delegate void EndJumpEvent();
    public event EndJumpEvent OnEndJump;

    private PlayerInputSystem _inputSystem;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (_instance == null)
            _instance = this;

        else if (_instance != this)
            Destroy(gameObject);

        _inputSystem = new PlayerInputSystem();
    }
    private void OnEnable()
    {
        if (_inputSystem != null)
        {
            _inputSystem.Enable();
        }
    }
    private void OnDisable()
    {
        if (_inputSystem != null)
        {
            _inputSystem.Disable();
        }
    }
    private void Start()
    {
        _inputSystem.Player.Jump.started += context => StartJump(context);
        _inputSystem.Player.Jump.canceled += context => EndJump(context);
    }
    
    private void StartJump(InputAction.CallbackContext context)
    {
        if (OnStartJump != null)
            OnStartJump();
    }

    private void EndJump(InputAction.CallbackContext context)
    {
        if (OnEndJump != null)
            OnEndJump();
    }
}