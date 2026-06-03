using UnityEngine;

public class BasicBulletSpell : MonoBehaviour
{
    private int projectileSpeed = 5;
    private GameObject spellSpawnPosAndRot;

    #region Start and Update Function
    private void Start()
    {
        transform.SetPositionAndRotation(
            spellSpawnPosAndRot.transform.position,
            spellSpawnPosAndRot.transform.rotation
        );
    }
    private void Update()
    {
        //Developer function to clear all projectiles in the scene
        ClearProjectiles();

        //Make the projectile travel at a constant speed forward
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }
    #endregion

    #region Projectile Called Functions
    private void ClearProjectiles()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Destroy(gameObject);
            Debug.Log("Removed all the projectiles");
        }
    }

    public void SetProjectilePosAndRot(GameObject objectRef)
    {
        spellSpawnPosAndRot = objectRef;
    }

    /// <summary>
    /// When the projectile collides with the opposing player, it calls a function to deal damage to
    /// the player before deleting itself. Upon hitting border it deletes itself and does nothing.
    /// </summary>
    /// <param name="other">
    /// The gameObject the projectile collided with.
    /// </param>
    void OnTriggerEnter(Collider other)
    {
        if (!(other.CompareTag("Player") || other.CompareTag("Border")))
            return;

        if (other.CompareTag("Player"))
        {
            //PlayerPVP playerPVPScript = other.GetComponent<PlayerPVP>();
            //playerPVPScript.TakeDamage(10);
            EntityBase entityBaseScript = other.GetComponent<EntityBase>();
            entityBaseScript.TakeDamage(10);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Border"))
        {
            Destroy(gameObject);
            Debug.Log($"Projectile missed!");
        }
    }
    #endregion
}