using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _rotateSpeed;
    protected float _moveSpeed;

    protected Rigidbody _rigidbody;
    protected Vector3 _movement;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _moveSpeed = _walkSpeed;
    }
}
