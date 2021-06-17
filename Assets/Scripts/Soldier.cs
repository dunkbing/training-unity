using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Soldier : Entity, ISpawn
{
    private Animator _animator;
    private Rigidbody2D _rb;

    private bool _moving;

    private float _speed;

    private static readonly int OnGround = Animator.StringToHash("OnGround");
    private static readonly int Moving = Animator.StringToHash("Moving");
    public GameObject explosionEffect;

    public bool RightSide { get; set; }

    public void Spawn()
    {
        _moving = false;
        _animator.SetBool(OnGround, false);
        // if (!RightSide)
        // {
        //     gameObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        // }
    }

    private void Awake()
    {
        _speed = Random.Range(.5f, 3f);
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();

        OnExplode += (() =>
        {
            // TODO: add explosion anim later
            Debug.Log("soldier exploded");
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        // _animator.SetBool(OnGround, false);
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
        if (other.gameObject.CompareTag("Ground"))
        {
            _animator.SetBool(OnGround, true);
            _animator.SetBool(Moving, true);
            _moving = true;
            transform.rotation = Quaternion.Euler(0, RightSide ? 0 : 180, 0);
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else if(other.gameObject.CompareTag("Bullet"))
        {
            _rb.constraints = RigidbodyConstraints2D.None;
            Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity), 0.5f);
            AudioManager.Instance.Play(nameof(Explosion));
            MenuHandler.Instance.IncreaseScore();
            Explode();
        } else if (other.gameObject.CompareTag("Fragment"))
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private IEnumerator DelayPause()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("Global").GetComponent<MenuHandler>().Pause();
        Pool.Instance.RetrieveAll();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cat"))
        {
            var entity = other.gameObject.GetComponent<Entity>();
            entity.Explode();
            _moving = false;
            _animator.SetBool(Moving, false);
            AudioManager.Instance.Play(nameof(Explosion));
            StartCoroutine(nameof(DelayPause));
        }
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
