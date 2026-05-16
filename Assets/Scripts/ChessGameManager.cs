using UnityEngine;
using System.Collections.Generic;

public class ChessGameManager : MonoBehaviour
{
    public static ChessGameManager Instance;

    public PieceColor currentTurn = PieceColor.White;
    private ChessUIPanel uiPanel;//add the ui panel
    private ChessBoardState     boardState;
    private ChessMoveValidator  moveValidator;
    private ChessBoardGenerator boardGen;
    private ChessAI             chessAI;

    private List<GameObject> highlights = new List<GameObject>();
    public Material highlightMaterial;

    void Awake() { Instance = this; }

    void Start()
    {
        boardState    = FindFirstObjectByType<ChessBoardState>();
        moveValidator = FindFirstObjectByType<ChessMoveValidator>();
        boardGen      = FindFirstObjectByType<ChessBoardGenerator>();
        chessAI       = FindFirstObjectByType<ChessAI>();
    uiPanel = FindFirstObjectByType<ChessUIPanel>();
        Debug.Log("Game started! White's turn.");
    }

    public void OnPiecePickedUp(ChessPiece piece)
    {
        if (piece.pieceColor != currentTurn)
        {
            Debug.Log($"It's {currentTurn}'s turn, not {piece.pieceColor}'s!");
            return;
        }

        ClearHighlights();
        List<Vector2Int> legalMoves = moveValidator.GetLegalMoves(piece);
        ShowHighlights(legalMoves);
        Debug.Log($"Picked up {piece.name} — {legalMoves.Count} legal moves available");
        ChessSoundManager.Instance?.PlayPickup();
    }

    public void OnPiecePlacedDown(ChessPiece piece, int toRow, int toCol)
    {
        ClearHighlights();

        if (piece.pieceColor != currentTurn) return;

        List<Vector2Int> legalMoves = moveValidator.GetLegalMoves(piece);
        Vector2Int target = new Vector2Int(toRow, toCol);

        if (legalMoves.Contains(target))
        {
            boardState.MovePiece(piece, toRow, toCol);
            piece.hasMoved = true;

            // Snap visually to square
            Vector3 snapPos = boardGen.GetTileWorldPosition(toRow, toCol);
            piece.transform.position = new Vector3(
                snapPos.x, snapPos.y + 0.022f, snapPos.z);
            piece.transform.rotation = Quaternion.identity;

            if (CheckForWin()) return;

            // Switch turn
            currentTurn = currentTurn == PieceColor.White
                ? PieceColor.Black : PieceColor.White;
                uiPanel?.UpdateUI(currentTurn, "Your move!");

            Debug.Log($"Move made! Now it's {currentTurn}'s turn.");

            // If it's AI's turn, trigger it
            if (currentTurn == chessAI.aiColor)
                chessAI.TakeTurn();
                ChessSoundManager.Instance?.PlayPlaced();
        }
        else
        {
            // Illegal — snap back
            Vector3 snapBack = boardGen.GetTileWorldPosition(piece.boardRow, piece.boardCol);
            piece.transform.position = new Vector3(
                snapBack.x, snapBack.y + 0.022f, snapBack.z);
            Debug.Log("Illegal move! Piece returned to original square.");
        }
    }

    // Called by AI after it finishes its move
    public void OnAIMoveComplete()
    {
        if (CheckForWin()) return;

        currentTurn = currentTurn == PieceColor.White
            ? PieceColor.Black : PieceColor.White;

        Debug.Log($"AI done. Now it's {currentTurn}'s turn.");
    }

    bool CheckForWin()
    {
        bool whiteKingAlive = false;
        bool blackKingAlive = false;

        ChessPiece[] allPieces = FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);
        foreach (ChessPiece p in allPieces)
        {
            if (p.pieceType == PieceType.King)
            {
                if (p.pieceColor == PieceColor.White) whiteKingAlive = true;
                if (p.pieceColor == PieceColor.Black) blackKingAlive = true;
            }
        }

        if (!blackKingAlive) { Debug.Log("⭐ WHITE WINS! ⭐"); return true; }
        if (!whiteKingAlive) { Debug.Log("⭐ BLACK WINS! ⭐"); return true; }
        return false;
    }

    void ShowHighlights(List<Vector2Int> squares)
    {
        foreach (Vector2Int sq in squares)
        {
            Vector3 pos = boardGen.GetTileWorldPosition(sq.x, sq.y);

            GameObject h = GameObject.CreatePrimitive(PrimitiveType.Quad);
            h.name = "Highlight";
            h.transform.position    = new Vector3(pos.x, pos.y + 0.006f, pos.z);
            h.transform.rotation    = Quaternion.Euler(90, 0, 0);
            h.transform.localScale  = Vector3.one * 0.085f;

            if (highlightMaterial != null)
                h.GetComponent<Renderer>().material = highlightMaterial;

            Destroy(h.GetComponent<Collider>());
            highlights.Add(h);
        }
    }

    void ClearHighlights()
    {
        foreach (GameObject h in highlights)
            if (h != null) Destroy(h);
        highlights.Clear();
    }
}