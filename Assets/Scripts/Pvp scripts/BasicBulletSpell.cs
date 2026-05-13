using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBulletSpell : MonoBehaviour
{
    private int projectileSpeed = 10;
    private GameObject SpellSpawnPosRot;
    //public TempGameManager TempGameManagerScript;
    private void Start()
    {
        //TempGameManagerScript = TempGameManager2.GetComponent<TempGameManager>();
        transform.SetPositionAndRotation(
            SpellSpawnPosRot.transform.position,
            SpellSpawnPosRot.transform.rotation
        );
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Destroy(gameObject);
            Debug.Log("Removed all the projectiles");
        }

        //Make the projectile travel at a constant speed forward
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    public void SetOwner(GameObject objectRef)
    {
        SpellSpawnPosRot = objectRef;
    }

    //protected void OnTriggerEnter(Collider other)
    //{
    //    var target = other.GetComponent<EntityBase>();
    //    if (target != null)
    //    {
    //        target.TakeDamage(20);
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        //if (!(other.CompareTag("Player") || other.CompareTag("Border")))
        //    return;
        Debug.Log("test");

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log("Player hit the opponent!");
        }
        else if (other.CompareTag("Border"))
        {
            Destroy(gameObject);
            Debug.Log("Projectile missed!");
        }
    }
}