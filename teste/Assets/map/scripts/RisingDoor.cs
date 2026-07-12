using UnityEngine;

public class RisingDoor : MonoBehaviour
{
    [Header("Movimento")]
    public float riseHeight = 3f;
    public float riseSpeed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * riseHeight;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        transform.position = Vector3.Lerp(transform.position, target, riseSpeed * Time.deltaTime);
    }

    public void Open()
    {
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
    }
}
