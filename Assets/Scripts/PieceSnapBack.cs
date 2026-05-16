using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PieceSnapBack : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private ChessBoardGenerator board;
    private ChessPiece piece;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        board  = FindFirstObjectByType<ChessBoardGenerator>();
        piece  = GetComponent<ChessPiece>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        // Tell game manager piece was picked up
        if (ChessGameManager.Instance != null)
            ChessGameManager.Instance.OnPiecePickedUp(piece);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        // Find nearest tile
        ChessBoardState boardState = FindFirstObjectByType<ChessBoardState>();
        Vector2Int nearestSquare = boardState.GetSquareFromWorldPos(
            transform.position,
            board
        );

        // Tell game manager where piece was dropped
        if (ChessGameManager.Instance != null)
            ChessGameManager.Instance.OnPiecePlacedDown(
                piece,
                nearestSquare.x,
                nearestSquare.y
            );

        // Snap upright
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}