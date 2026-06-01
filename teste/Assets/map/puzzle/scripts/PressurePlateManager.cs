using UnityEngine;
using UnityEngine.Events;

public class PressurePlateManager : MonoBehaviour
{
    [Header("Configuração")]
    public PressurePlate[] plates;
    public int requiredCount = 1;

    [Header("Evento")]
    public UnityEvent onAllPressed;

    private int currentCount;
    private bool alreadyTriggered;

    void OnEnable()
    {
        foreach (var plate in plates)
        {
            if (plate != null)
                plate.onPressed.AddListener(OnPlatePressed);
        }
    }

    void OnDisable()
    {
        foreach (var plate in plates)
        {
            if (plate != null)
                plate.onPressed.RemoveListener(OnPlatePressed);
        }
    }

    void OnPlatePressed()
    {
        currentCount++;

        if (currentCount >= requiredCount && !alreadyTriggered)
        {
            alreadyTriggered = true;
            onAllPressed.Invoke();
        }
    }
}
