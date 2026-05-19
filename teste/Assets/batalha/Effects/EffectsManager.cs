using System.Collections;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }

    [Header("Screen Shake")]
    public Transform cameraTransform;
    public float defaultShakeIntensity = 0.2f;
    public float defaultShakeDuration = 0.3f;

    [Header("Slow Motion")]
    public float hitSlowDuration = 0.08f;
    public float hitSlowTimeScale = 0.1f;

    private Vector3 cameraOriginalPos;
    private bool isShaking = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            cameraOriginalPos = cameraTransform.localPosition;
    }

    #region Screen Shake

    public void ShakeScreen(float intensity = -1f, float duration = -1f)
    {
        if (isShaking) StopCoroutine(nameof(ShakeCoroutine));
        float i = intensity < 0 ? defaultShakeIntensity : intensity;
        float d = duration < 0 ? defaultShakeDuration : duration;
        StartCoroutine(ShakeCoroutine(i, d));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = 1f - (elapsed / duration);
            float x = Random.Range(-intensity, intensity) * t;
            float y = Random.Range(-intensity, intensity) * t;

            if (cameraTransform != null)
                cameraTransform.localPosition = cameraOriginalPos + new Vector3(x, y, 0);

            yield return null;
        }

        if (cameraTransform != null)
            cameraTransform.localPosition = cameraOriginalPos;

        isShaking = false;
    }

    #endregion

    #region Slow Motion Hit Effect

    public void HitStop()
    {
        StartCoroutine(HitStopCoroutine());
    }

    private IEnumerator HitStopCoroutine()
    {
        Time.timeScale = hitSlowTimeScale;
        yield return new WaitForSecondsRealtime(hitSlowDuration);
        Time.timeScale = 1f;
    }

    #endregion

    #region VFX Spawn

    public GameObject SpawnVFX(GameObject prefab, Vector3 position, float destroyAfter = 3f)
    {
        if (prefab == null) return null;
        var vfx = Instantiate(prefab, position, Quaternion.identity);
        if (destroyAfter > 0) Destroy(vfx, destroyAfter);
        return vfx;
    }

    public void SpawnVFXOnCharacter(GameObject prefab, BattleCharacter character, float destroyAfter = 3f)
    {
        if (character == null) return;
        Vector3 pos = character.vfxSpawnPoint != null
            ? character.vfxSpawnPoint.position
            : character.transform.position;
        SpawnVFX(prefab, pos, destroyAfter);
    }

    #endregion

    #region Flash Effect

    public IEnumerator FlashCharacter(BattleCharacter character, Color flashColor, int times = 3, float interval = 0.1f)
    {
        var sr = character.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color original = sr.color;
        for (int i = 0; i < times; i++)
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(interval);
            sr.color = original;
            yield return new WaitForSeconds(interval);
        }
    }

    #endregion
}
