using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class BirdController : MonoBehaviour
{

    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;
    private Rigidbody _rigidbody;
    private bool _isAscendPressed;
    private bool _isSteerPressed;
    private float _direction;
    
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
    void LateUpdate()
    {
        ClampPlayerMovement();
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
            newVelocity = new Vector3(_rigidbody.linearVelocity.x, _jumpForce, 0);
            _isAscendPressed = false;
        }

        if (newVelocity != Vector3.zero)
        {
            _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
            _rigidbody.AddForce(newVelocity, ForceMode.Impulse);
        }
    }


    // bron: https://stackoverflow.com/questions/42800645/how-to-completely-prevent-the-player-from-going-offscreen-in-unity
    private void ClampPlayerMovement()
    {
        Vector3 position = transform.position;

        float distance = transform.position.z - Camera.main.transform.position.z;

        float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x;
        float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance)).x;

        float ceiling = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, distance)).y;

        if (ceiling < transform.position.y)
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, 0);
        }

        position.x = Mathf.Clamp(position.x, leftBorder, rightBorder);
        position.y = Mathf.Clamp(position.y, -4, ceiling);

        transform.position = position;
    }
}
