using UnityEngine;

public class SpellProjHandler : MonoBehaviour
{
    //#region Variables
    //private int projectileSpeed = 5;
    //private GameObject spellSpawnPosAndRot;
    //#endregion

    //#region Start and Update Function
    //private void Start()
    //{
    //    transform.SetPositionAndRotation(
    //        spellSpawnPosAndRot.transform.position,
    //        spellSpawnPosAndRot.transform.rotation
    //    );
    //}
    //private void Update()
    //{
    //    //Developer function to clear all projectiles in the scene
    //    ClearProjectiles();

    //    //Make the projectile travel at a constant speed forward
    //    transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    //}
    //#endregion

    //#region Projectile Called Functions
    //private void ClearProjectiles()
    //{
    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        Destroy(gameObject);
    //        Debug.Log("Removed all the projectiles");
    //    }
    //}

    //public void SetProjectilePosAndRot(GameObject objectRef)
    //{
    //    spellSpawnPosAndRot = objectRef;
    //}

    ///// <summary>
    ///// When the projectile collides with the opposing player, it calls a function to deal damage to
    ///// the player before deleting itself. Upon hitting border it deletes itself and does nothing.
    ///// </summary>
    ///// <param name="other">
    ///// The gameObject the projectile collided with.
    ///// </param>
    //void OnTriggerEnter(Collider other)
    //{
    //    if (!(other.CompareTag("Player") || other.CompareTag("Border")))
    //        return;

    //    if (other.CompareTag("Player"))
    //    {
    //        //PlayerPVP playerPVPScript = other.GetComponent<PlayerPVP>();
    //        //playerPVPScript.TakeDamage(10);
    //        CharacterEntity entityBaseScript = other.GetComponent<CharacterEntity>();
    //        entityBaseScript.TakeDamage(10);
    //        Destroy(gameObject);
    //    }
    //    else if (other.CompareTag("Border"))
    //    {
    //        Destroy(gameObject);
    //        Debug.Log($"Projectile missed!");
    //    }
    //}
    //#endregion

    // Type of spell enum

    private bool initialized = false;

    private ScriptableObjectSpells spell;

    private float projectileSpeed;

    public float damageMultiplier = 1f;

    public void Init(ScriptableObjectSpells spell)
    {
        this.spell = spell;
        projectileSpeed = spell.spellSpeed;

        if (spell == null)
        {
            Debug.LogError("Null Spell");
        }

        initialized = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Destroy(gameObject);
            return;
        }

        if (!initialized)
            return;

        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Border"))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            CharacterEntity character = other.GetComponent<CharacterEntity>();
            if (character == null)
            {
                Debug.LogError($"[SpellProjHandler]: No CharacterEntity script on Player tag object");
                return;
            }
            float damage = spell.spellDamage * damageMultiplier;
            Debug.Log($"[SpellProjHandler] '{spell.name}' hit {other.name} for {damage} (base {spell.spellDamage} x{damageMultiplier})");
            character.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Border"))
        {
            Debug.Log("Projectile missed!");
            Destroy(gameObject);

            return;
        }
    }
}