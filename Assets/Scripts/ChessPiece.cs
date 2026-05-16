using UnityEngine;

public enum PieceType  { Pawn, Rook, Knight, Bishop, Queen, King }
public enum PieceColor { White, Black }

public class ChessPiece : MonoBehaviour
{
    public PieceType  pieceType;
    public PieceColor pieceColor;

    // Current square on the board
    public int boardRow;
    public int boardCol;

    // Has this pawn moved yet (for double-step rule)
    public bool hasMoved = false;
}