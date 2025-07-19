using UnityEngine;

public class PlayerBase : CharacterBase
{
    protected InputSystem _inputSystem;
    protected override void Awake()
    {
        base.Awake();
        _inputSystem = new InputSystem();
    }

    protected void OnEnable()
    {
        _inputSystem.Enable();
    }

    protected void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + _moveSpeed * Time.fixedDeltaTime * _movement);
    }
}
