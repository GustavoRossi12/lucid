using UnityEngine;
using TMPro;

public class WorldSpaceBubble : MonoBehaviour
{
    public GameObject bubbleRoot;   // o GameObject com o canvas/balão
    public TMP_Text bubbleText;
    public Transform lookTarget;    // geralmente a câmera principal

    void Start()
    {
        bubbleRoot.SetActive(false);
        if (lookTarget == null)
            lookTarget = Camera.main.transform;
    }

    void LateUpdate()
    {
        // balão sempre olha para a câmera
        if (bubbleRoot.activeSelf && lookTarget != null)
            bubbleRoot.transform.LookAt(lookTarget);
    }

    public void ShowText(string text)
    {
        bubbleText.text = text;
        bubbleRoot.SetActive(true);
    }

    public void HideText() => bubbleRoot.SetActive(false);
}
