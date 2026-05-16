using UnityEngine;
using TMPro;

public class ChessUIPanel : MonoBehaviour
{
    public TextMeshPro turnText;
    public TextMeshPro statusText;

    void Start()
    {
        ChessGameManager gm = FindFirstObjectByType<ChessGameManager>();
        UpdateUI(PieceColor.White, "Game started!");
    }

    public void UpdateUI(PieceColor currentTurn, string status)
    {
        if (turnText != null)
            turnText.text = currentTurn == PieceColor.White
                ? "⬜ WHITE'S TURN"
                : "⬛ BLACK'S TURN";

        if (statusText != null)
            statusText.text = status;
    }
}
