using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Start projectile {gameObject.GetHashCode()}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Projectile hit");
        Destroy(gameObject);
    }
}
