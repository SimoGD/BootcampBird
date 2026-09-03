using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BirdController : MonoBehaviour
{
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;

    private Rigidbody _rigidbody;
    private bool _isAscendPressed;
    private bool _isSteerPressed;
    private float _direction;

    public UnityEvent CollisionEvent;
    public UnityEvent IncreaseScoreEvent;

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

        newVelocity = SteerPlayer(newVelocity);

        newVelocity = AscendPlayer(newVelocity);

        ApplyForce(newVelocity);
    }

    private void OnAscend()
    {
        _isAscendPressed = true;
    }

    private void OnSteer(InputValue value)
    {
        _direction = value.Get<float>();
        _isSteerPressed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("GAME OVER");
        CollisionEvent.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        IncreaseScoreEvent.Invoke();
    }

    private Vector3 SteerPlayer(Vector3 newVelocity)
    {
        if (_isSteerPressed)
        {
            newVelocity = new Vector3(_moveSpeed * _direction, _rigidbody.linearVelocity.y, 0);
            _isSteerPressed = false;
        }

        return newVelocity;
    }

    private Vector3 AscendPlayer(Vector3 newVelocity)
    {
        if (_isAscendPressed)
        {
            newVelocity = new Vector3(_rigidbody.linearVelocity.x, _jumpForce, 0);
            _isAscendPressed = false;
        }

        return newVelocity;
    }

    private void ApplyForce(Vector3 newVelocity)
    {
        if (newVelocity != Vector3.zero)
        {
            // Gravity is disabled until the player presses a key.
            if (_rigidbody.isKinematic)
            {
                _rigidbody.isKinematic = false;
            }

            // reset old velocity
            _rigidbody.linearVelocity = Vector3.zero;

            // apply new force
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
