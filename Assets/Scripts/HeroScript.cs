﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class HeroScript : MonoBehaviour
{
    private Rigidbody2D _hero;
    private Collider2D _heroZone;
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


    // Start is called before the first frame update
    void Start()
    {
        _hero = GetComponent<Rigidbody2D>();
        _heroZone = GetComponent<Collider2D>();
        _heroImages = GetComponentsInChildren<SpriteRenderer>();
        Debug.Log($"Init projectile {projectile.GetHashCode()}");
        _direction = new Vector3(1, 0);
        _gun = _marker.GetComponent<GunScript>();

    }

    // Update is called once per frame
    private void Update()
    {
        _marker.transform.position = _hero.position;
        ////var touch= Input.GetTouch(0);
        ////Debug.Log(touch);
        ////if (Input.GetKeyDown(KeyCode.Space))
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump!");
            _jump = true;
            _keyPressed = KeyCode.Space;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire!");
            _fire = true;
            _keyPressed = KeyCode.Z;
        }

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _direction = new Vector3(_horizontalInput == 0 ? _direction.x : _horizontalInput, _verticalInput).normalized;
        _currentDirection = new Vector3(_horizontalInput, _verticalInput);
        _gun.UpdateDirection(GetGunDirection(_currentDirection, _direction,  _isGrounded), GetGunOffset(_currentDirection, _isGrounded));
        Debug.Log(_direction.normalized);
        _pointsUp = _verticalInput > 0;
        _pointsRight = _horizontalInput > 0;
        _pointsNeutralH = _horizontalInput == 0;
        _pointsNeutralV = _verticalInput == 0;
        ChangeHeroDirection(_currentDirection);

        if (_horizontalInput > 0)
        {
            foreach (var image in _heroImages)
            {
                image.flipX = false;
            }
            ////_heroImages[0].flipX = false;
            ////_heroImages[1].flipX = false;
        }
        else if (_horizontalInput < 0)
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
        if (isGrounded && currentDirection.y<0 && currentDirection.x ==0)
        {
            return new Vector3(direction.x, 0).normalized;
        }

        return direction;
    }

    private Vector3 GetGunOffset(Vector3 direction, bool isGrounded)
    {
        if(direction.y < 0 && direction.x == 0 && isGrounded)
        {
            return Vector3.down * 0.5f;

        }
        else if(!isGrounded)
        {
            return Vector3.zero;
        }

        return Vector3.up * 0.5f;
    }

    private void ChangeHeroDirection(Vector3 direction)
    {
        var isUp = direction.y > 0 && direction.x == 0;
        var isUpDiag = direction.y >0 && direction.x != 0;
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
        _hero.velocity = new Vector2((float)5 * _currentDirection.normalized.x, _hero.velocity.y);
        var direction = _direction.normalized.x / Math.Abs(_direction.normalized.x);// _horizontalInput > 0 ? -1 : 1;
        _heroImages[1].transform.Rotate(new Vector3(0, 0, -18f * direction));
        ////_heroImages[1].transform.Rotate(_direction.normalized,18f);
    }

    private void ProcessJump()
    {
        if (_isGrounded)
        {
            _hero.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
            Debug.Log(_hero.velocity);
            Debug.Log(Vector2.up);
        }
    }

    private void FireProjectile(Vector3 direction)
    {
        Debug.Log($"Hero pos {gameObject.transform.position}");
        _marker.GetComponent<GunScript>().Fire();
        return;
        //Instantiate<Projectile>
        ////throw new NotImplementedException();
        ///

        ////var resource = Resources.Load("Projectile");
        ////Debug.Log($"Resource {resource}");
        var thisProjectile = Instantiate(projectile, gameObject.transform.position + new Vector3(direction.normalized.x * 2, _direction.normalized.y * 2), Quaternion.identity);    // Instantiate(projectile);
        thisProjectile.GetComponent<ProjectileScript>().Setup(_direction);
        Debug.Log($"Proj direction {direction}");
        ////Debug.Log($"ThisProj {thisProjectile}");
        var body = ((GameObject)thisProjectile).GetComponent<Rigidbody2D>();
        body.velocity = transform.forward * 1; //new Vector2(25, 0);
        Debug.Log(thisProjectile);
        ////Debug.Log("FIRE!!!!");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        _collisions++;
        Debug.Log($"Collisions { _collisions}");

        Debug.Log($"Is grounded {_isGrounded}");
        _isGrounded = _collisions > 0;//  true;
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
        Debug.Log($"Collisions { _collisions}");
        HideImages();
        _isGrounded = _collisions > 0; //allColliders.Any(c => c.GetContacts(tempcolliders) > 0);  //collision.contactCount > 0;
        _heroImages[1].enabled = !_isGrounded;
        _heroImages[0].enabled = _isGrounded;
        Debug.Log($"Is grounded {_isGrounded}");
    }
}
