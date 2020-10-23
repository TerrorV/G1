﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Rigidbody2D _hero;
    private SpriteRenderer[] _heroImages;
    private bool _jump;
    private KeyCode _keyPressed;
    private float _horizontalInput;
    private float _verticalInput;
    private bool _pointsUp;
    private bool _pointsRight;
    private bool _pointsNeutralH;
    private bool _pointsNeutralV;
    private bool _isGrounded;
    private int _collisions;


    // Start is called before the first frame update
    void Start()
    {
        _hero = GetComponent<Rigidbody2D>();
        _heroImages = GetComponentsInChildren<SpriteRenderer>();

    }

    // Update is called once per frame
    private void Update()
    {
        ////var touch= Input.GetTouch(0);
        ////Debug.Log(touch);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("UP!");
            _jump = true;
            _keyPressed = KeyCode.Space;
        }

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _pointsUp = _verticalInput > 0;
        _pointsRight = _horizontalInput > 0;
        _pointsNeutralH = _horizontalInput == 0;
        _pointsNeutralV = _verticalInput == 0;

        var isUp = _pointsNeutralH && _pointsUp;
        var isUpDiag = !_pointsNeutralH && _pointsUp;
        var isProne = _pointsNeutralH && !_pointsUp && !_pointsNeutralV;
        var isDownDiag = !_pointsNeutralH && !_pointsUp && !_pointsNeutralV;
        var isSide = !_pointsNeutralH && _pointsNeutralV;

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

    private void FixedUpdate()
    {
        switch (_keyPressed)
        {
            case KeyCode.Space:
                if (!_isGrounded)
                {
                    _keyPressed = KeyCode.Question;
                    return;
                }

                _hero.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
                _keyPressed = KeyCode.Question;
                Debug.Log(_hero.velocity);
                Debug.Log(Vector2.up);
                break;
            default:
                break;
        }

        _hero.velocity = new Vector2((float)3 * _horizontalInput, _hero.velocity.y);
        //_heroImages[1].transform.Rotate(new Vector3(0, _heroImages[1].transform.rotation.eulerAngles.y + 1.8f));
        var direction = _horizontalInput > 0 ? -1 : 1;
        _heroImages[1].transform.Rotate(new Vector3(0, 0, 18f * direction));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        _collisions++;
        Debug.Log($"Collisions { _collisions}");

        Debug.Log($"Is grounded {_isGrounded}");
        _isGrounded = _collisions > 0;//  true;
        //_heroImages[0].enabled = true;
        //_heroImages[1].enabled = false;
        HideImages();
    }

    private void HideImages()
    {
        foreach (var image in _heroImages)
        {
            image.enabled = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        _collisions--;
        Debug.Log($"Collisions { _collisions}");
        HideImages();
        ////Collider2D[] allColliders = null;
        ////_hero.GetAttachedColliders(allColliders);
        ////Debug.Log(allColliders);
        ////Collider2D[] tempcolliders = null;
        _isGrounded = _collisions > 0; //allColliders.Any(c => c.GetContacts(tempcolliders) > 0);  //collision.contactCount > 0;
        ////_heroImages[0].enabled = false;
        _heroImages[1].enabled = !_isGrounded;
        _heroImages[0].enabled = _isGrounded;
        ////Debug.Log($"Collisions count {collision.contactCount}");
        Debug.Log($"Is grounded {_isGrounded}");
    }
}
