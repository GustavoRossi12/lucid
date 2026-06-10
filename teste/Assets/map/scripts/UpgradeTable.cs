using System.Collections;
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
    public float zoomedOutFOV = 75f;
    public float zoomSpeed = 2f;

    private bool playerInRange;
    private PlayerStats playerStats;
    private ParkourController playerController;
    private CameraOrbit cameraOrbit;
    private Camera playerCamera;
    private float originalFOV;
    private Coroutine zoomRoutine;

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
            originalFOV = tableCamera.fieldOfView;
            if (zoomRoutine != null) StopCoroutine(zoomRoutine);
            zoomRoutine = StartCoroutine(ZoomFOV(originalFOV, zoomedOutFOV));
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
            if (zoomRoutine != null) StopCoroutine(zoomRoutine);
            tableCamera.fieldOfView = originalFOV;
            tableCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ui.Close();
    }

    IEnumerator ZoomFOV(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += zoomSpeed * Time.deltaTime;
            tableCamera.fieldOfView = Mathf.Lerp(from, to, t);
            yield return null;
        }
        tableCamera.fieldOfView = to;
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
