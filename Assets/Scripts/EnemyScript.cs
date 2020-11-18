using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EnemyScript : MonoBehaviour
{
    private Rigidbody2D _hero;
    private Collider2D _heroZone;
    public CircleCollider2D _leftBorder;
    public CircleCollider2D _rightBorder;
    private SpriteRenderer[] _heroImages;
    private bool _jump;
    private bool _fire;
    private KeyCode _keyPressed;
    private float _horizontalInput;
    private float _verticalInput;
    private bool _pointsUp;
    private bool _pointsRight;
    private bool _pointsNeutralH;
    private bool _pointsNeutralV;
    private bool _isGrounded;
    private int _collisions;
    public GameObject projectile;
    public GameObject _marker;
    private Vector3 _direction;
    private GunScript _gun;
    private Vector3 _currentDirection;
    private Random _rnd;


    // Start is called before the first frame update
    void Start()
    {
        _rnd = new Random((uint)UnityEngine.Random.Range(1, 100000));
        _hero = GetComponent<Rigidbody2D>();
        _heroZone = GetComponent<CapsuleCollider2D>();
        _heroImages = GetComponentsInChildren<SpriteRenderer>();
        Debug.Log($"Init projectile {projectile.GetHashCode()}");
        _direction = new Vector3(1, 0);
        _gun = _marker.GetComponent<GunScript>();
        _direction = new Vector3(1, 0).normalized;
        _currentDirection = new Vector3(1, 0);

    }

    // Update is called once per frame
    private void Update()
    {
        _marker.transform.position = _hero.position;
        ////var touch= Input.GetTouch(0);
        ////Debug.Log(touch);
        ////if (Input.GetKeyDown(KeyCode.Space))
        ////if (Input.GetButtonDown("Jump"))
        ////{
        ////    Debug.Log("Jump!");
        ////    _jump = true;
        ////    _keyPressed = KeyCode.Space;
        ////}

        ////if (Input.GetButtonDown("Fire1"))
        ////{
        ////    Debug.Log("Fire!");
        ////    _fire = true;
        ////    _keyPressed = KeyCode.Z;
        ////}

        //_horizontalInput = 1;// Input.GetAxis("Horizontal");
        //_verticalInput = 0;// Input.GetAxis("Vertical");
        //_direction = new Vector3(_horizontalInput == 0 ? _direction.x : _horizontalInput, _verticalInput).normalized;
        //_currentDirection = new Vector3(_horizontalInput, _verticalInput);
        _gun.UpdateDirection(GetGunDirection(_currentDirection, _direction, _isGrounded), GetGunOffset(_currentDirection, _isGrounded));
        ////Debug.Log(_direction.normalized);
        _pointsUp = _verticalInput > 0;
        _pointsRight = _horizontalInput > 0;
        _pointsNeutralH = _horizontalInput == 0;
        _pointsNeutralV = _verticalInput == 0;
        ChangeHeroDirection(_currentDirection);

        if (_currentDirection.x > 0)
        {
            foreach (var image in _heroImages)
            {
                image.flipX = false;
            }
            ////_heroImages[0].flipX = false;
            ////_heroImages[1].flipX = false;
        }
        else if (_currentDirection.x < 0)
        {
            foreach (var image in _heroImages)
            {
                image.flipX = true;
            }
            ////_heroImages[0].flipX = true;
            ////_heroImages[1].flipX = true;

        }
    }

    private Vector3 GetGunDirection(Vector3 currentDirection, Vector3 direction, bool isGrounded)
    {
        if (isGrounded && currentDirection.y < 0 && currentDirection.x == 0)
        {
            return new Vector3(direction.x, 0).normalized;
        }

        return direction;
    }

    private Vector3 GetGunOffset(Vector3 direction, bool isGrounded)
    {
        if (direction.y < 0 && direction.x == 0 && isGrounded)
        {
            return Vector3.down * 0.5f;

        }
        else if (!isGrounded)
        {
            return Vector3.zero;
        }

        return Vector3.up * 0.5f;
    }

    private void ChangeHeroDirection(Vector3 direction)
    {
        var isUp = direction.y > 0 && direction.x == 0;
        var isUpDiag = direction.y > 0 && direction.x != 0;
        var isProne = direction.y < 0 && direction.x == 0;
        var isDownDiag = direction.y < 0 && direction.x != 0;
        var isSide = direction.y == 0 && direction.x != 0;

        HideImages();

        if (!_isGrounded)
        {
            _heroImages[1].enabled = true;
        }
        else if (isUp)
        {
            //UP
            _heroImages[3].enabled = true;
        }
        else if (isUpDiag)
        {
            // UP DIAG
            _heroImages[2].enabled = true;
        }
        else if (isProne)
        {
            // DOWN PRONE
            _heroImages[4].enabled = true;
        }
        else if (isDownDiag)
        {
            // DOWN DIAG
            _heroImages[5].enabled = true;
        }
        else if (isSide)
        {
            // SIDE
            _heroImages[0].enabled = true;
        }
        else
        {
            _heroImages[0].enabled = true;
        }

        //_heroImages.Last().enabled = true;
    }

    private void FixedUpdate()
    {
        var rnd = _rnd.NextInt(0, 5) > 3;
        Debug.Log(rnd);
        if (_jump)
        {
            ProcessJump();
            _jump = false;
        }

        if (_fire)
        {
            try
            {
                FireProjectile(_direction);
            }
            finally
            {
                _fire = false;
            }
        }

        ProcessMove();
    }

    private void ProcessMove()
    {
        _hero.velocity = new Vector2((float)3 * _currentDirection.normalized.x, _hero.velocity.y);
        var direction = _direction.normalized.x / Math.Abs(_direction.normalized.x);// _horizontalInput > 0 ? -1 : 1;
        _heroImages[1].transform.Rotate(new Vector3(0, 0, -18f * direction));
        ////_heroImages[1].transform.Rotate(_direction.normalized,18f);
    }

    private void ProcessJump()
    {
        if (_isGrounded)
        {
            Debug.Log("JUMP JUMP JUMP!!!!");
            _hero.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
            ////Debug.Log(_hero.velocity);
            ////Debug.Log(Vector2.up);
            _isGrounded = false;
        }
    }

    private void FireProjectile(Vector3 direction)
    {
        Debug.Log($"Hero pos {gameObject.transform.position}");
        _marker.GetComponent<GunScript>().Fire();
        return;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D[] colliders = new Collider2D[100];
        _isGrounded = _heroZone.GetContacts(colliders) > 0 || _heroZone.attachedRigidbody.velocity.y == 0;//  true;

        if (collision.otherCollider == _leftBorder || collision.otherCollider == _rightBorder)
        {
            Debug.Log($"Collisions { collision.collider}");
            Debug.Log($"Collisions { collision.otherCollider}");

            //var rnd = new System.Random().Next(0, 1);
            //Debug.Log(rnd);
            var jump = _rnd.NextInt(0,5)>4 && _isGrounded;
            Debug.Log($"Should jump { jump}");

            if (jump)
            {
                //ProcessJump();
            }
            else
            {
                _direction *= -1;
                _currentDirection *= -1;
            }

            ////_currentDirection *= -1;
            ////_direction *= -1;
        }

        if (collision.gameObject.name.StartsWith("Projectile"))
        {
            Destroy(collision.otherCollider.gameObject);
        }

        _collisions++;
        ////Debug.Log($"Collisions { _collisions}");
        var proj = collision.otherRigidbody.GetComponentInParent<ProjectileScript>();

        ////Debug.Log($"Collisions {  }");

        ////Debug.Log($"Is grounded {_isGrounded}");
        ///
        HideImages();
    }

    private void HideImages()
    {
        foreach (var image in _heroImages)
        {
            image.enabled = false;
        }

        //_heroImages.Last().enabled = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        _collisions--;
        if (collision.otherCollider == _leftBorder || collision.otherCollider == _rightBorder)
        {
        Debug.Log($"Collisions { collision.collider}");
        Debug.Log($"Collisions { collision.otherCollider}");

            var jump = _rnd.NextInt(0, 5) > 3;
            if(jump)
            {
            ProcessJump();
                            }
            else
            {
                _direction *= -1;
                _currentDirection *= -1;
            }

            ////_currentDirection *= -1;
            ////_direction *= -1;
        }

        ////Debug.Log($"Collisions { _collisions}");
        HideImages();
        Collider2D[] colliders = new Collider2D[100];
        _isGrounded = _heroZone.GetContacts(colliders) > 0 || _heroZone.attachedRigidbody.velocity.y == 0;//  true;

       // _isGrounded = _collisions > 0; //allColliders.Any(c => c.GetContacts(tempcolliders) > 0);  //collision.contactCount > 0;
        _heroImages[1].enabled = !_isGrounded;
        _heroImages[0].enabled = _isGrounded;
        ////Debug.Log($"Is grounded {_isGrounded}");
    }
}
