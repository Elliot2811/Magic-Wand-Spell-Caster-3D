using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBulletSpell : MonoBehaviour
{
    private int bulletSpeed = 10;
    public enum ProjectileDirections
    {
        left,
        right
    }
    public ProjectileDirections projectileDir;

    //void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player"))
    //        return;
    //    //PlayerPVP.bulletHitPlayer1Event?.Invoke();
    //    Destroy(gameObject);
    //}

    public void destroyProjectile()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        //--------Original method where projectile determines its direction based of where it got instanciated-------
        //if (transform.position.x < 0 )
        //{
        //    projectileDir = ProjectileDirections.left;
        //    Debug.Log("Player is Left");
        //}
        //else if (transform.position.x > 0)
        //{
        //    projectileDir = ProjectileDirections.right;
        //    Debug.Log("Player is Right");
        //}
        //else
        //{
        //    Debug.Log("Error - cannot determine target because prefab is at x = 0");
        //}

        switch (projectileDir)
        {
            case ProjectileDirections.left:
                transform.position = new Vector3(-4.8F, 1.2F, 0);
                break;
            case ProjectileDirections.right:
                transform.position = new Vector3(4.8F, 1.2F, 0);
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            destroyProjectile();
            Debug.Log("Removed all the projectiles");
        }
    }
}