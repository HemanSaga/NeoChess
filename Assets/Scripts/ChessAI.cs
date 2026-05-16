using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChessAI : MonoBehaviour
{
    [Header("AI Settings")]
    public PieceColor aiColor     = PieceColor.Black;
    public int        searchDepth = 3; // Higher = smarter but slower

    private ChessBoardState     boardState;
    private ChessMoveValidator  moveValidator;
    private ChessGameManager    gameManager;
    private ChessBoardGenerator boardGen;

    // Piece values for scoring
    const int PAWN_VALUE   = 100;
    const int KNIGHT_VALUE = 320;
    const int BISHOP_VALUE = 330;
    const int ROOK_VALUE   = 500;
    const int QUEEN_VALUE  = 900;
    const int KING_VALUE   = 20000;

    void Start()
    {
        boardState    = FindFirstObjectByType<ChessBoardState>();
        moveValidator = FindFirstObjectByType<ChessMoveValidator>();
        gameManager   = FindFirstObjectByType<ChessGameManager>();
        boardGen      = FindFirstObjectByType<ChessBoardGenerator>();
    }

    // Called by GameManager when it's AI's turn
    public void TakeTurn()
    {
        StartCoroutine(ThinkAndMove());
    }

    IEnumerator ThinkAndMove()
    {
        Debug.Log("AI is thinking...");

        // Wait one frame so UI updates first
        yield return new WaitForSeconds(0.5f);

        BestMove best = FindBestMove();

        if (best != null && best.piece != null)
        {
            Debug.Log($"AI moves {best.piece.name} to [{best.toRow},{best.toCol}]");

            // Execute the move
            boardState.MovePiece(best.piece, best.toRow, best.toCol);
            best.piece.hasMoved = true;

            // Snap piece to new position visually
            Vector3 newPos = boardGen.GetTileWorldPosition(best.toRow, best.toCol);
            best.piece.transform.position = new Vector3(
                newPos.x,
                newPos.y + 0.022f,
                newPos.z
            );

            best.piece.transform.rotation = Quaternion.identity;

            // Tell game manager move is done
            gameManager.OnAIMoveComplete();
        }
        else
        {
            Debug.Log("AI has no moves — Game over!");
        }
    }

    // ─── Minimax ──────────────────────────────────────────────

    class BestMove
    {
        public ChessPiece piece;
        public int toRow, toCol;
        public int score;
    }

    BestMove FindBestMove()
    {
        List<ChessPiece> aiPieces = GetPiecesForColor(aiColor);
        BestMove best = null;
        int bestScore = int.MinValue;

        foreach (ChessPiece piece in aiPieces)
        {
            List<Vector2Int> moves = moveValidator.GetLegalMoves(piece);

            foreach (Vector2Int move in moves)
            {
                // Simulate move
                SimulationState sim = SimulateMove(piece, move.x, move.y);

                // Evaluate with minimax
                int score = Minimax(searchDepth - 1, false, int.MinValue, int.MaxValue);

                // Undo move
                UndoSimulation(sim);

                if (score > bestScore)
                {
                    bestScore = score;
                    best = new BestMove {
                        piece = piece,
                        toRow = move.x,
                        toCol = move.y,
                        score = score
                    };
                }
            }
        }

        return best;
    }

    int Minimax(int depth, bool isMaximizing, int alpha, int beta)
    {
        if (depth == 0)
            return EvaluateBoard();

        PieceColor turn = isMaximizing ? aiColor :
            (aiColor == PieceColor.White ? PieceColor.Black : PieceColor.White);

        List<ChessPiece> pieces = GetPiecesForColor(turn);

        if (isMaximizing)
        {
            int maxScore = int.MinValue;

            foreach (ChessPiece piece in pieces)
            {
                List<Vector2Int> moves = moveValidator.GetLegalMoves(piece);
                foreach (Vector2Int move in moves)
                {
                    SimulationState sim = SimulateMove(piece, move.x, move.y);
                    int score = Minimax(depth - 1, false, alpha, beta);
                    UndoSimulation(sim);

                    maxScore = Mathf.Max(maxScore, score);
                    alpha    = Mathf.Max(alpha, score);
                    if (beta <= alpha) return maxScore; // Alpha-beta pruning
                }
            }
            return maxScore;
        }
        else
        {
            int minScore = int.MaxValue;

            foreach (ChessPiece piece in pieces)
            {
                List<Vector2Int> moves = moveValidator.GetLegalMoves(piece);
                foreach (Vector2Int move in moves)
                {
                    SimulationState sim = SimulateMove(piece, move.x, move.y);
                    int score = Minimax(depth - 1, true, alpha, beta);
                    UndoSimulation(sim);

                    minScore = Mathf.Min(minScore, score);
                    beta     = Mathf.Min(beta, score);
                    if (beta <= alpha) return minScore; // Alpha-beta pruning
                }
            }
            return minScore;
        }
    }

    // ─── Board Evaluation ─────────────────────────────────────

    int EvaluateBoard()
    {
        int score = 0;

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                ChessPiece piece = boardState.board[r, c];
                if (piece == null) continue;

                int value = GetPieceValue(piece.pieceType);

                // Add positional bonus
                value += GetPositionBonus(piece, r, c);

                if (piece.pieceColor == aiColor)
                    score += value;
                else
                    score -= value;
            }
        }

        return score;
    }

    int GetPieceValue(PieceType type)
    {
        switch (type)
        {
            case PieceType.Pawn:   return PAWN_VALUE;
            case PieceType.Knight: return KNIGHT_VALUE;
            case PieceType.Bishop: return BISHOP_VALUE;
            case PieceType.Rook:   return ROOK_VALUE;
            case PieceType.Queen:  return QUEEN_VALUE;
            case PieceType.King:   return KING_VALUE;
            default:               return 0;
        }
    }

    // Bonus points for good positions (center control etc)
    int GetPositionBonus(ChessPiece piece, int row, int col)
    {
        // Reward controlling the center
        int centerBonus = 0;
        if (col >= 2 && col <= 5 && row >= 2 && row <= 5)
            centerBonus = 10;
        if (col >= 3 && col <= 4 && row >= 3 && row <= 4)
            centerBonus = 20;

        // Reward pawn advancement
        int advanceBonus = 0;
        if (piece.pieceType == PieceType.Pawn)
        {
            advanceBonus = piece.pieceColor == PieceColor.Black
                ? (7 - row) * 5
                : row * 5;
        }

        return centerBonus + advanceBonus;
    }

    // ─── Move Simulation ──────────────────────────────────────

    class SimulationState
    {
        public ChessPiece piece;
        public int fromRow, fromCol;
        public int toRow,   toCol;
        public ChessPiece capturedPiece;
    }

    SimulationState SimulateMove(ChessPiece piece, int toRow, int toCol)
    {
        SimulationState sim = new SimulationState
        {
            piece         = piece,
            fromRow       = piece.boardRow,
            fromCol       = piece.boardCol,
            toRow         = toRow,
            toCol         = toCol,
            capturedPiece = boardState.board[toRow, toCol]
        };

        // Apply move on board array only (no visual change)
        boardState.board[piece.boardRow, piece.boardCol] = null;
        boardState.board[toRow, toCol] = piece;
        piece.boardRow = toRow;
        piece.boardCol = toCol;

        return sim;
    }

    void UndoSimulation(SimulationState sim)
    {
        // Restore piece to original square
        boardState.board[sim.toRow,   sim.toCol]   = sim.capturedPiece;
        boardState.board[sim.fromRow, sim.fromCol] = sim.piece;
        sim.piece.boardRow = sim.fromRow;
        sim.piece.boardCol = sim.fromCol;

        // Restore captured piece's position if any
        if (sim.capturedPiece != null)
        {
            sim.capturedPiece.boardRow = sim.toRow;
            sim.capturedPiece.boardCol = sim.toCol;
        }
    }

    List<ChessPiece> GetPiecesForColor(PieceColor color)
    {
        List<ChessPiece> result = new List<ChessPiece>();
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (boardState.board[r, c] != null &&
                    boardState.board[r, c].pieceColor == color)
                    result.Add(boardState.board[r, c]);
        return result;
    }
}