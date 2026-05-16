using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PieceHoverEffect : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Renderer[] renderers;
    private Color originalColor;

    [Header("Glow Settings")]
    public Color hoverColor   = new Color(1f, 0.8f, 0f, 1f); // golden glow
    public float hoverIntensity = 1.5f;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        renderers        = GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
            originalColor = renderers[0].material.color;

        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        foreach (Renderer r in renderers)
        {
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", hoverColor * hoverIntensity);
        }
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        foreach (Renderer r in renderers)
        {
            r.material.SetColor("_EmissionColor", Color.black);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
            grabInteractable.hoverExited.RemoveListener(OnHoverExit);
        }
    }
}