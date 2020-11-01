using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    private Vector3 _direction;
    public GameObject _projectile;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void UpdateDirection(Vector3 direction, Vector3 offset)
    {
        gameObject.transform.position += direction + offset;
        var rotAngle = Vector3.SignedAngle(_direction, direction, Vector3.forward);
        gameObject.transform.Rotate(0, 0, rotAngle);
        _direction = direction;
    }

    public void Fire()
    {
        FireProjectile(_direction);
    }

    private void FireProjectile(Vector3 direction)
    {
        Debug.Log($"Gun pos {gameObject.transform.position}");

        //Instantiate<Projectile>
        ////throw new NotImplementedException();
        ///

        ////var resource = Resources.Load("Projectile");
        ////Debug.Log($"Resource {resource}");
        ////var thisProjectile = Instantiate(_projectile, gameObject.transform.position + new Vector3(direction.normalized.x, direction.normalized.y), Quaternion.identity);    // Instantiate(projectile);
        var thisProjectile = Instantiate(_projectile, gameObject.transform.position + 0.2f * direction.normalized, Quaternion.identity);    // Instantiate(projectile);
        thisProjectile.GetComponent<ProjectileScript>().Setup(direction);
        Debug.Log($"Proj direction {direction}");
        ////Debug.Log($"ThisProj {thisProjectile}");
        var body = ((GameObject)thisProjectile).GetComponent<Rigidbody2D>();
        body.velocity = transform.forward * 1; //new Vector2(25, 0);
        Debug.Log(thisProjectile);
        ////Debug.Log("FIRE!!!!");
    }
}
