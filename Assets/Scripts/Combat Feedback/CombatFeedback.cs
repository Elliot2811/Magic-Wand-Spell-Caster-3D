using System.Collections;
using UnityEngine;

public class CombatFeedback : MonoBehaviour
{
    public static CombatFeedback Instance { get; private set; }

    [Header("Screen Shake")]
    [SerializeField] private Camera targetCamera;

    [Header("Floating Text")]
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private Vector3 floatingTextOffset = new Vector3(0f, 2f, 0f);

    [Header("Shield Destroyed")]
    [SerializeField] private AudioClip shieldBreakSFX;
    [SerializeField] private float shieldBreakSFXVolume = 1f;
    [SerializeField] private float shieldBreakShakeIntensity = 0.3f;
    [SerializeField] private float shieldBreakShakeDuration = 0.25f;
    [SerializeField] private string shieldBreakText = "BLOCKED!";
    [SerializeField] private Color shieldBreakTextColor = Color.cyan;

    [Header("Player Hit")]
    [SerializeField] private AudioClip playerHitSFX;
    [SerializeField] private float playerHitSFXVolume = 1f;
    [SerializeField] private float playerHitShakeIntensity = 0.15f;
    [SerializeField] private float playerHitShakeDuration = 0.12f;
    [SerializeField] private Color playerHitTextColor = Color.red;

    private Vector3 cameraOriginalLocalPos;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            cameraOriginalLocalPos = targetCamera.transform.localPosition;
        else
            Debug.LogWarning("[CombatFeedback]: No camera assigned/found — screen shake will be skipped.");
    }

    public void PlayShieldBreak(Vector3 worldPosition, ScriptableObjectSpell spell = null)
    {
        AudioClip sfx = spell != null && spell.hitSFX != null ? spell.hitSFX : shieldBreakSFX;
        float vol = spell != null && spell.hitSFX != null ? spell.hitVolume : shieldBreakSFXVolume;
        float intensity = spell != null && spell.hitShakeIntensity >= 0f ? spell.hitShakeIntensity : shieldBreakShakeIntensity;
        float duration = spell != null && spell.hitShakeDuration >= 0f ? spell.hitShakeDuration : shieldBreakShakeDuration;

        AudioManager.Instance?.PlaySFX(sfx, vol, 1f);
        Shake(intensity, duration);
        //SpawnFloatingText(worldPosition, shieldBreakText, shieldBreakTextColor);
    }

    public void PlayPlayerHit(Vector3 worldPosition, float damage, ScriptableObjectSpell spell = null)
    {
        AudioClip sfx = spell != null && spell.hitSFX != null ? spell.hitSFX : playerHitSFX;
        float vol = spell != null && spell.hitSFX != null ? spell.hitVolume : playerHitSFXVolume;
        float intensity = spell != null && spell.hitShakeIntensity >= 0f ? spell.hitShakeIntensity : playerHitShakeIntensity;
        float duration = spell != null && spell.hitShakeDuration >= 0f ? spell.hitShakeDuration : playerHitShakeDuration;

        AudioManager.Instance?.PlaySFX(sfx, vol, 1f);
        Shake(intensity, duration);
        //SpawnFloatingText(worldPosition, Mathf.RoundToInt(damage).ToString(), playerHitTextColor);
    }

    private void Shake(float intensity, float duration)
    {
        if (targetCamera == null) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            targetCamera.transform.localPosition = cameraOriginalLocalPos;
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float damper = 1f - (elapsed / duration); // ease out toward the end
            Vector2 offset = Random.insideUnitCircle * intensity * damper;
            targetCamera.transform.localPosition = cameraOriginalLocalPos + new Vector3(offset.x, offset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }
        targetCamera.transform.localPosition = cameraOriginalLocalPos;
        shakeCoroutine = null;
    }

    //private void SpawnFloatingText(Vector3 worldPosition, string text, Color color)
    //{
    //    if (floatingTextPrefab == null) return;

    //    Quaternion rot = targetCamera != null
    //        ? Quaternion.LookRotation(-targetCamera.transform.forward, targetCamera.transform.up)
    //        : Quaternion.identity;

    //    FloatingText instance = Instantiate(floatingTextPrefab, worldPosition + floatingTextOffset, rot);

    //    Vector3 scale = instance.transform.localScale;
    //    scale.x *= -1f;
    //    instance.transform.localScale = scale;

    //    instance.Init(text, color);
    //}
}