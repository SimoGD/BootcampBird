using UnityEngine;

public class WallMovement : MonoBehaviour
{
    private float _speed;
    private Vector3 _orignalPosition;
    private Rigidbody _rigidbody;
    private void Awake()
    {
        _orignalPosition = transform.parent.position;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        gameObject.transform.position = _orignalPosition;
    }
    void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + Vector3.back * Time.deltaTime * _speed);
    }
}
