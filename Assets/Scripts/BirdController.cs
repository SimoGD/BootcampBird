using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BirdController : MonoBehaviour
{
    [SerializeField] private float _steerForce;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    private Rigidbody _rigidbody;
    private bool _isAscendPressed;
    private float _direction;
    public UnityEvent CollisionEvent;
    public UnityEvent IncreaseScoreEvent;

    void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();

        _isAscendPressed = false;
    }
    void LateUpdate()
    {
        ClampPlayerMovement();
    }

    private void FixedUpdate()
    {
        SteerPlayer();
        AscendPlayer();
    }

    private void OnAscend()
    {
        if (Time.timeScale == 0)
            return;
        
        PlayAudio("flap");
        _isAscendPressed = true;
    }

    private void OnSteer(InputValue value)
    {
        _direction = value.Get<float>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayAudio("collision");
        CollisionEvent.Invoke();
        PlayDeathAnimation();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayAudio("point");
        IncreaseScoreEvent.Invoke();
    }

    private void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }
    private void SteerPlayer()
    {
            _rigidbody.AddForce(Vector3.right * _direction * _steerForce, ForceMode.Force);    
    }

    private void AscendPlayer()
    {
        if (_isAscendPressed)
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            _isAscendPressed = false;
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
        position.y = Mathf.Clamp(position.y, -4, ceiling-2);

        transform.position = position;
    }

    public void DeathAnimationFinished()
    {
        animator.enabled = false;
    }

    private void PlayAudio(string fileName)
    {
        foreach (AudioClip audioClip in audioClips)
        {
            if (audioClip.name == fileName)
            {
                if (fileName == "collision")
                    audioSource.volume = 0.3f;
                else
                    audioSource.volume = 0.6f;

                audioSource.PlayOneShot(audioClip);
            }
        }
    }
}
