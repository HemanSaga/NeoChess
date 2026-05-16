using UnityEngine;

public class ChessBoardState : MonoBehaviour
{
    // 8x8 grid storing which piece is on each square
    // null means empty
    public ChessPiece[,] board = new ChessPiece[8, 8];

    void Start()
    {
        Invoke(nameof(InitializeFromScene), 0.6f);
    }

    void InitializeFromScene()
    {
        // Clear board
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                board[r, c] = null;

        // Find all pieces and register their positions
        ChessPiece[] allPieces = FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);
        ChessBoardGenerator boardGen = FindFirstObjectByType<ChessBoardGenerator>();

        foreach (ChessPiece piece in allPieces)
        {
            Vector2Int square = GetSquareFromWorldPos(piece.transform.position, boardGen);
            if (IsValidSquare(square))
            {
                board[square.x, square.y] = piece;
                piece.boardRow = square.x;
                piece.boardCol = square.y;
                Debug.Log($"Registered {piece.name} at [{square.x},{square.y}]");
            }
        }

        Debug.Log("Board state initialized!");
    }

    public Vector2Int GetSquareFromWorldPos(Vector3 worldPos, ChessBoardGenerator boardGen)
    {
        // Find the closest tile
        int bestRow = 0, bestCol = 0;
        float minDist = float.MaxValue;

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                Vector3 tilePos = boardGen.GetTileWorldPosition(r, c);
                float dist = Vector2.Distance(
                    new Vector2(worldPos.x, worldPos.z),
                    new Vector2(tilePos.x,  tilePos.z)
                );
                if (dist < minDist)
                {
                    minDist  = dist;
                    bestRow  = r;
                    bestCol  = c;
                }
            }
        }

        return new Vector2Int(bestRow, bestCol);
    }

    public bool IsValidSquare(Vector2Int sq)
    {
        return sq.x >= 0 && sq.x < 8 && sq.y >= 0 && sq.y < 8;
    }

    public bool IsEmpty(int row, int col)
    {
        return IsValidSquare(new Vector2Int(row, col)) && board[row, col] == null;
    }

    public bool IsEnemy(int row, int col, PieceColor myColor)
    {
        if (!IsValidSquare(new Vector2Int(row, col))) return false;
        if (board[row, col] == null) return false;
        return board[row, col].pieceColor != myColor;
    }

    public void MovePiece(ChessPiece piece, int toRow, int toCol)
    {
        // Remove from old square
        board[piece.boardRow, piece.boardCol] = null;

        // Capture enemy if present
        ChessPiece target = board[toRow, toCol];
        if (target != null)
        {
            Debug.Log($"Captured {target.name}!");
            Destroy(target.gameObject);
        }
        if (target != null)
    ChessSoundManager.Instance?.PlayCapture();

        // Place on new square
        board[toRow, toCol] = piece;
        piece.boardRow = toRow;
        piece.boardCol = toCol;
    }
}