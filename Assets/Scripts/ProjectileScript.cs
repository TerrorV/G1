using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    Vector3 _direction;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Setup(Vector3 direction)
    {
       // Debug.Log($"Start projectile {gameObject.GetHashCode()}");
        Destroy(gameObject, 5f);

        _direction = direction.normalized;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

            transform.position += _direction * 0.3f;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"Projectile hit");

        Destroy(gameObject);
    }
}
