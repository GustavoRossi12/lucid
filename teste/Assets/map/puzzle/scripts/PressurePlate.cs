using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Movimento")]
    public float lowerDistance = 0.2f;
    public float lowerSpeed = 3f;

    [Header("Evento")]
    public UnityEvent onPressed;

    private Vector3 upPos;
    private Vector3 downPos;
    private bool isPressed;

    void Start()
    {
        upPos = transform.position;
        downPos = upPos - transform.up * lowerDistance;
    }

    void Update()
    {
        Vector3 target = isPressed ? downPos : upPos;
        transform.position = Vector3.Lerp(transform.position, target, lowerSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPressed) return;
        if (!other.CompareTag("Player")) return;

        isPressed = true;
        onPressed.Invoke();
    }

    public bool IsPressed => isPressed;
}
