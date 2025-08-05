using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    private Rigidbody _rigidbody;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
}
