using UnityEngine;

public class Soldier : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;

    private bool _moving;
    
    private int _speed;

    private static readonly int OnGround = Animator.StringToHash("OnGround");

    public bool RightSide { get; set; }

    private void Awake()
    {
        _speed = Random.Range(3, 8);
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator.SetBool(OnGround, false);
        if (!RightSide)
        {
            gameObject.transform.Rotate(new Vector3(0, 180, 0));
        }
    }

    void FixedUpdate()
    {
        if (_moving)
        {
            MoveForward();
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Ground")) return;
        _animator.SetBool(OnGround, true);
        _moving = true;
    }
    
    private void MoveForward()
    {
        if (RightSide)
        {
            _rb.MovePosition(Vector3.right * (Time.fixedDeltaTime * _speed) + transform.position);
            return;
        }
        _rb.MovePosition(Vector3.left * (Time.fixedDeltaTime * _speed) + transform.position);
    }

}
