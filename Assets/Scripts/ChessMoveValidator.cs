using UnityEngine;
using System.Collections.Generic;

public class ChessMoveValidator : MonoBehaviour
{
    private ChessBoardState boardState;

    void Start()
    {
        boardState = FindFirstObjectByType<ChessBoardState>();
    }

    // Returns list of all legal squares this piece can move to
    public List<Vector2Int> GetLegalMoves(ChessPiece piece)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        switch (piece.pieceType)
        {
            case PieceType.Pawn:   AddPawnMoves(piece, moves);   break;
            case PieceType.Rook:   AddRookMoves(piece, moves);   break;
            case PieceType.Knight: AddKnightMoves(piece, moves); break;
            case PieceType.Bishop: AddBishopMoves(piece, moves); break;
            case PieceType.Queen:  AddQueenMoves(piece, moves);  break;
            case PieceType.King:   AddKingMoves(piece, moves);   break;
        }

        return moves;
    }

    void AddPawnMoves(ChessPiece piece, List<Vector2Int> moves)
    {
        int dir = piece.pieceColor == PieceColor.White ? 1 : -1;
        int r = piece.boardRow;
        int c = piece.boardCol;

        // Single step forward
        if (boardState.IsEmpty(r + dir, c))
        {
            moves.Add(new Vector2Int(r + dir, c));

            // Double step from starting row
            bool onStartRow = (piece.pieceColor == PieceColor.White && r == 1) ||
                              (piece.pieceColor == PieceColor.Black && r == 6);
            if (onStartRow && boardState.IsEmpty(r + dir * 2, c))
                moves.Add(new Vector2Int(r + dir * 2, c));
        }

        // Diagonal captures
        if (boardState.IsEnemy(r + dir, c + 1, piece.pieceColor))
            moves.Add(new Vector2Int(r + dir, c + 1));
        if (boardState.IsEnemy(r + dir, c - 1, piece.pieceColor))
            moves.Add(new Vector2Int(r + dir, c - 1));
    }

    void AddRookMoves(ChessPiece piece, List<Vector2Int> moves)
    {
        AddSlidingMoves(piece, moves, new Vector2Int[] {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
        });
    }

    void AddBishopMoves(ChessPiece piece, List<Vector2Int> moves)
    {
        AddSlidingMoves(piece, moves, new Vector2Int[] {
            new Vector2Int(1,1),  new Vector2Int(1,-1),
            new Vector2Int(-1,1), new Vector2Int(-1,-1)
        });
    }

    void AddQueenMoves(ChessPiece piece, List<Vector2Int> moves)
    {
        AddSlidingMoves(piece, moves, new Vector2Int[] {
            new Vector2Int(1,0),  new Vector2Int(-1,0),
            new Vector2Int(0,1),  new Vector2Int(0,-1),
            new Vector2Int(1,1),  new Vector2Int(1,-1),
            new Vector2Int(-1,1), new Vector2Int(-1,-1)
        });
    }

    void AddSlidingMoves(ChessPiece piece, List<Vector2Int> moves, Vector2Int[] directions)
    {
        foreach (Vector2Int dir in directions)
        {
            int r = piece.boardRow + dir.x;
            int c = piece.boardCol + dir.y;

            while (boardState.IsValidSquare(new Vector2Int(r, c)))
            {
                if (boardState.IsEmpty(r, c))
                {
                    moves.Add(new Vector2Int(r, c));
                }
                else
                {
                    if (boardState.IsEnemy(r, c, piece.pieceColor))
                        moves.Add(new Vector2Int(r, c));
                    break; // Blocked
                }
                r += dir.x;
                c += dir.y;
            }
        }
    }

    void AddKnightMoves(ChessPiece piece, List<Vector2Int> moves)
    {
        Vector2Int[] jumps = {
            new Vector2Int(2,1),  new Vector2Int(2,-1),
            new Vector2Int(-2,1), new Vector2Int(-2,-1),
            new Vector2Int(1,2),  new Vector2Int(1,-2),
            new Vector2Int(-1,2), new Vector2Int(-1,-2)
        };

        foreach (Vector2Int jump in jumps)
        {
            int r = piece.boardRow + jump.x;
            int c = piece.boardCol + jump.y;
            Vector2Int sq = new Vector2Int(r, c);

            if (boardState.IsValidSquare(sq) &&
               (boardState.IsEmpty(r, c) || boardState.IsEnemy(r, c, piece.pieceColor)))
            {
                moves.Add(sq);
            }
        }
    }

    void AddKingMoves(ChessPiece piece, List<Vector2Int> moves)
    {
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;

                int r = piece.boardRow + dr;
                int c = piece.boardCol + dc;
                Vector2Int sq = new Vector2Int(r, c);

                if (boardState.IsValidSquare(sq) &&
                   (boardState.IsEmpty(r, c) || boardState.IsEnemy(r, c, piece.pieceColor)))
                {
                    moves.Add(sq);
                }
            }
        }
    }
}