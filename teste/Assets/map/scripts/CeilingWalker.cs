using UnityEngine;

public class CeilingWalker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var controller = other.GetComponent<ParkourController>();
        if (controller != null) controller.SetCeiling(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var controller = other.GetComponent<ParkourController>();
        if (controller != null) controller.SetCeiling(false);
    }
}
