using System.Collections;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    [Header("Character States")]
    [SerializeField] private GameObject idleSprite;
    [SerializeField] private GameObject attackSprite;

    [Header("Attack Animation")]
    [SerializeField] private float attackDuration = 0.6f;

    private Coroutine attackCoroutine;

    private void Start()
    {
        if (transform.position.x < 0)
        {
            // Left player
            idleSprite.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            attackSprite.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            // Scale
            idleSprite.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            attackSprite.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            // Move upwards
            idleSprite.transform.localPosition = new Vector3(0f, 1f, 0f);
            attackSprite.transform.localPosition = new Vector3(0f, 1f, 0f);
        }
        else
        {
            // Right player
            idleSprite.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            attackSprite.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            // Scale
            idleSprite.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
            attackSprite.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
            // Move upwards
            idleSprite.transform.localPosition = new Vector3(0f, 0.7f, 0f);
            attackSprite.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        }

        ShowIdle();
    }

    public void PlayAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = StartCoroutine(AttackAnimation());
    }

    private IEnumerator AttackAnimation()
    {
        idleSprite.SetActive(false);
        attackSprite.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        ShowIdle();
    }

    private void ShowIdle()
    {
        idleSprite.SetActive(true);
        attackSprite.SetActive(false);
    }
}