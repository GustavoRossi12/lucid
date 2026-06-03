using UnityEngine;

public class UpgradeTable : MonoBehaviour
{
    [Header("Proximidade")]
    public float triggerRadius = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public UpgradeUI ui;

    [Header("Câmera da Mesa")]
    public Camera tableCamera;

    private bool playerInRange;
    private PlayerStats playerStats;
    private ParkourController playerController;
    private CameraOrbit cameraOrbit;
    private Camera playerCamera;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (playerStats == null)
                playerStats = FindAnyObjectByType<PlayerStats>();

            if (playerStats != null && !ui.IsOpen)
                OpenUI();
            else if (ui.IsOpen)
                CloseUI();
        }

        if (ui.IsOpen && Input.GetKeyDown(KeyCode.Escape))
            CloseUI();
    }

    void OpenUI()
    {
        FindPlayerComponents();

        if (playerController != null)
        {
            playerController.canLook = false;
            playerController.canMove = false;
        }
        if (cameraOrbit != null) cameraOrbit.enabled = false;

        if (tableCamera != null)
        {
            playerCamera = Camera.main;
            if (playerCamera != null) playerCamera.gameObject.SetActive(false);
            tableCamera.gameObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ui.Open(playerStats);
    }

    void CloseUI()
    {
        if (playerController != null)
        {
            playerController.canLook = true;
            playerController.canMove = true;
        }
        if (cameraOrbit != null) cameraOrbit.enabled = true;

        if (tableCamera != null && playerCamera != null)
        {
            tableCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ui.Close();
    }

    void FindPlayerComponents()
    {
        if (playerController == null)
            playerController = FindAnyObjectByType<ParkourController>();
        if (cameraOrbit == null)
            cameraOrbit = FindAnyObjectByType<CameraOrbit>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (ui.IsOpen) CloseUI();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
