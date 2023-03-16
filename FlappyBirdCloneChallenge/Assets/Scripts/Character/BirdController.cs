using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    private const float Z_OFFSET = 0f;

    [Header("Character data")]
    [SerializeField] private float _jumpForce = 60f;
    [SerializeField] private float _gravity = 15f;
    [SerializeField] private Animator _animator;

    [Header("Look At")]
    [SerializeField] private Transform _bird;
    [SerializeField] private Transform _upLookAt;
    [SerializeField] private Transform _downLookAt;

    [Header("Detection")]
    [SerializeField] private Transform _characterOffset;
    [SerializeField] private float _detectorRadius;
    [SerializeField] private LayerMask _pipesLayer;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private Transform _currentLookAtTarget;
    private Vector3 _initialPos;
    private Quaternion _initialRot;
    private Collider[] _colliders;

    private void OnEnable()
    {
        GameStateController.OnResetGame += ResetPlayer;
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("InputManager doesn't found!");
            return;
        }
        InputManager.Instance.OnStartJump += Jump;
    }
    private void OnDisable()
    {
        GameStateController.OnResetGame -= ResetPlayer;
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("InputManager doesn't found!");
            return;
        }
        InputManager.Instance.OnStartJump -= Jump;
    }
    private void Start()
    {
        _initialPos = transform.position;
        _initialRot = transform.rotation;

        _characterController = GetComponent<CharacterController>();
    }
    /// <summary>
    /// Reset player position and rotation
    /// </summary>
    public void ResetPlayer()
    {
        transform.position = _initialPos;
        transform.rotation = _initialRot;
        _velocity = Vector3.zero;
    }

    void Update()
    {
        if (GameStateController.OnGetCurrentGameState() == EnumGameState.Progress)
        {
            ObstacleDetection();

            ApplyGravity();

            LookAtRotation();

            Movement();
        }
    }

    private void Jump()
    {
        _velocity.y = 0;
        _velocity.y = Mathf.Sqrt(_jumpForce);
        _animator.SetTrigger("OnFlip");
    }
    private void LookAtRotation()
    {
        _currentLookAtTarget = _velocity.y > 0 ? _upLookAt : _downLookAt;

        // Update the target rotation based on the position of the target object
        Vector3 directionToTarget = _currentLookAtTarget.position - _bird.position;
        float targetRotationZ = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - Z_OFFSET;
        _bird.transform.localRotation = Quaternion.Euler(0f, 0f, targetRotationZ);
    }

    private void ApplyGravity()
    {
        _velocity.y += -_gravity * Time.deltaTime;
    }

    private void Movement()
    {
        _characterController.Move(_velocity * Time.deltaTime);
    }

    #region Detection
    /// <summary>
    /// use Physics to detecet the collision between the character and pipes
    /// is much better use this instead of use colliders, because there is no dual-geometry collision calculation happening.
    /// </summary>
    private void ObstacleDetection()
    {

        _colliders = Physics.OverlapSphere(_characterOffset.position, _detectorRadius, _pipesLayer);

        if (_colliders.Length > 0 && GameStateController.OnGetCurrentGameState() == EnumGameState.Progress || GameStateController.OnGetCurrentGameState() == EnumGameState.Paused)
        {
            GameStateController.OnGameOver?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_characterOffset.position, _detectorRadius);
    }
    #endregion
}