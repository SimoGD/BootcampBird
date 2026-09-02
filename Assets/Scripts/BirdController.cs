using UnityEngine;
using UnityEngine.InputSystem;

public class BirdController : MonoBehaviour
{

    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;
    private Rigidbody _rigidbody;
    private bool _isAscendPressed;
    private bool _isSteerPressed;
    private float _direction;
    private Vector3 _velocity;
    
    void OnAscend()
    {
        _isAscendPressed = true;
    }

    void OnSteer(InputValue value)
    {
        _direction = value.Get<float>();
        _isSteerPressed = true;
    }

    void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _isAscendPressed = false;
        _isSteerPressed = false;
    }

    private void FixedUpdate()
    {
        Vector3 newVelocity = Vector3.zero;

        if (_isSteerPressed)
        {
            newVelocity = new Vector3(_moveSpeed * _direction, 0, 0);
            _isSteerPressed = false;
        }

        if (_isAscendPressed)
        {
            newVelocity = new Vector3(0, _jumpForce, 0);
            _isAscendPressed = false;
        }

        if (newVelocity != Vector3.zero)
        {
            _rigidbody.AddForce(newVelocity, ForceMode.Impulse);
            _velocity = newVelocity;
        }
    }
}
