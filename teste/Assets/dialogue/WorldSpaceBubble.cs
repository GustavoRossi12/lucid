using UnityEngine;
using TMPro;

public class WorldSpaceBubble : MonoBehaviour
{
    public GameObject bubbleRoot;
    public TMP_Text bubbleText;
    public Transform lookTarget;

    void Start()
    {
        bubbleRoot.SetActive(false);
        if (lookTarget == null)
            lookTarget = Camera.main.transform;
    }

    void LateUpdate()
    {
        // billboard towards camera
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
