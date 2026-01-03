using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public PlayerData _playerData;
    private PlayerAction _playerAction;


    private Vector2 Input;
    private CharacterController characterController;
    
    void Awake()
    {   
        characterController = GetComponent<CharacterController>();
        _playerAction = new PlayerAction();
        BindAction();
    }

    
    void Update()
    {
        Move();
    }

    private void OnEnable()
    {
        _playerAction.Enable();
    }

    private void OnDisable()
    {
        _playerAction.Disable();
    }

    private void BindAction()
    {
        _playerAction.Player.move.performed += ctx => Input = ctx.ReadValue<Vector2>();
        _playerAction.Player.move.canceled += ctx => Input = Vector2.zero;

    }

    private void Move()
    {
        float Speed = _playerData.moveSpeed;

        Vector3 moveDir = transform.right * Input.x + transform.forward * Input.y;
        if (moveDir.magnitude > 1f) moveDir.Normalize();

        characterController.Move(moveDir * Speed * Time.deltaTime);
    }
}
