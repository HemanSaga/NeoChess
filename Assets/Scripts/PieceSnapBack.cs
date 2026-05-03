using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PieceSnapBack : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private ChessBoardGenerator board;
    private Vector3 lastValidPosition;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        board = FindFirstObjectByType<ChessBoardGenerator>();

        lastValidPosition = transform.position;

        // Listen for release event
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        // Find the nearest tile center and snap to it
        Vector3 nearest = board.GetNearestTilePosition(transform.position);
        transform.position = nearest;

        // Freeze rotation so piece stands upright
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}