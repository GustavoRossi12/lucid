using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    [Header("Sprite")]
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite attackSprite;
    public float attackSpriteDuration = 0.3f;
    public float spriteScale = 1f;

    private Transform cameraTransform;
    private float attackTimer;
    private bool showingAttack;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        if (spriteRenderer != null && idleSprite != null)
            spriteRenderer.sprite = idleSprite;
        transform.localScale = Vector3.one * spriteScale;
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            Vector3 lookDir = cameraTransform.forward;
            lookDir.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        if (showingAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                showingAttack = false;
                if (spriteRenderer != null && idleSprite != null)
                    spriteRenderer.sprite = idleSprite;
            }
        }
    }

    public void SetAttacking()
    {
        showingAttack = true;
        attackTimer = attackSpriteDuration;
        if (spriteRenderer != null && attackSprite != null)
            spriteRenderer.sprite = attackSprite;
    }
}
