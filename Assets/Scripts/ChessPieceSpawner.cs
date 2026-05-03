using UnityEngine;

public class ChessPieceSpawner : MonoBehaviour
{
    [Header("Materials")]
    public Material whiteMaterial;
    public Material blackMaterial;

    [Header("Board Reference")]
    public ChessBoardGenerator chessBoard;

    void Start()
    {
        // Small delay so ChessBoardGenerator.Start() runs first
        Invoke(nameof(SpawnAllPieces), 0.1f);
    }

    void SpawnAllPieces()
    {
        // White back row
        SpawnPiece(PieceType.Rook,   PieceColor.White, 0, 0);
        SpawnPiece(PieceType.Knight, PieceColor.White, 0, 1);
        SpawnPiece(PieceType.Bishop, PieceColor.White, 0, 2);
        SpawnPiece(PieceType.Queen,  PieceColor.White, 0, 3);
        SpawnPiece(PieceType.King,   PieceColor.White, 0, 4);
        SpawnPiece(PieceType.Bishop, PieceColor.White, 0, 5);
        SpawnPiece(PieceType.Knight, PieceColor.White, 0, 6);
        SpawnPiece(PieceType.Rook,   PieceColor.White, 0, 7);

        // White pawns
        for (int col = 0; col < 8; col++)
            SpawnPiece(PieceType.Pawn, PieceColor.White, 1, col);

        // Black back row
        SpawnPiece(PieceType.Rook,   PieceColor.Black, 7, 0);
        SpawnPiece(PieceType.Knight, PieceColor.Black, 7, 1);
        SpawnPiece(PieceType.Bishop, PieceColor.Black, 7, 2);
        SpawnPiece(PieceType.Queen,  PieceColor.Black, 7, 3);
        SpawnPiece(PieceType.King,   PieceColor.Black, 7, 4);
        SpawnPiece(PieceType.Bishop, PieceColor.Black, 7, 5);
        SpawnPiece(PieceType.Knight, PieceColor.Black, 7, 6);
        SpawnPiece(PieceType.Rook,   PieceColor.Black, 7, 7);

        // Black pawns
        for (int col = 0; col < 8; col++)
            SpawnPiece(PieceType.Pawn, PieceColor.Black, 6, col);
    }

    void SpawnPiece(PieceType type, PieceColor color, int row, int col)
    {
        GameObject piece = BuildPieceShape(type);
        piece.name = $"{color}_{type}_{row}_{col}";

        // Get the EXACT center of this tile from the board
        Vector3 tilePos = chessBoard.GetTileWorldPosition(row, col);

        // Place piece at tile center, lifted above tile surface
        piece.transform.position = new Vector3(
            tilePos.x,
            tilePos.y + GetPieceHeightOffset(type),
            tilePos.z
        );

        Material mat = color == PieceColor.White ? whiteMaterial : blackMaterial;
        foreach (var r in piece.GetComponentsInChildren<Renderer>())
            r.material = mat;

        ChessPiece cp = piece.AddComponent<ChessPiece>();
        cp.pieceType  = type;
        cp.pieceColor = color;
    }

    float GetPieceHeightOffset(PieceType type)
    {
        switch (type)
        {
            case PieceType.Pawn:   return 0.018f;
            case PieceType.Rook:   return 0.022f;
            case PieceType.Knight: return 0.020f;
            case PieceType.Bishop: return 0.026f;
            case PieceType.Queen:  return 0.030f;
            case PieceType.King:   return 0.028f;
            default:               return 0.018f;
        }
    }

    GameObject BuildPieceShape(PieceType type)
    {
        GameObject root = new GameObject();
        float s = 0.09f * 0.35f;

        switch (type)
        {
            case PieceType.Pawn:
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                body.transform.parent = root.transform;
                body.transform.localPosition = Vector3.zero;
                body.transform.localScale = new Vector3(s, s * 0.6f, s);

                GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                head.transform.parent = root.transform;
                head.transform.localPosition = new Vector3(0, s * 0.85f, 0);
                head.transform.localScale = new Vector3(s * 0.8f, s * 0.8f, s * 0.8f);
                break;
            }
            case PieceType.Rook:
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                body.transform.parent = root.transform;
                body.transform.localPosition = Vector3.zero;
                body.transform.localScale = new Vector3(s, s * 0.9f, s);

                GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
                top.transform.parent = root.transform;
                top.transform.localPosition = new Vector3(0, s * 1.05f, 0);
                top.transform.localScale = new Vector3(s * 1.1f, s * 0.25f, s * 1.1f);
                break;
            }
            case PieceType.Knight:
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                body.transform.parent = root.transform;
                body.transform.localPosition = Vector3.zero;
                body.transform.localScale = new Vector3(s, s * 0.8f, s);

                GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
                head.transform.parent = root.transform;
                head.transform.localPosition = new Vector3(0, s * 1.0f, s * 0.2f);
                head.transform.localRotation = Quaternion.Euler(20, 0, 0);
                head.transform.localScale = new Vector3(s * 0.7f, s * 0.5f, s * 0.9f);
                break;
            }
            case PieceType.Bishop:
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                body.transform.parent = root.transform;
                body.transform.localPosition = Vector3.zero;
                body.transform.localScale = new Vector3(s * 0.75f, s * 1.1f, s * 0.75f);

                GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                tip.transform.parent = root.transform;
                tip.transform.localPosition = new Vector3(0, s * 1.4f, 0);
                tip.transform.localScale = new Vector3(s * 0.4f, s * 0.4f, s * 0.4f);
                break;
            }
            case PieceType.Queen:
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                body.transform.parent = root.transform;
                body.transform.localPosition = Vector3.zero;
                body.transform.localScale = new Vector3(s * 0.85f, s * 1.3f, s * 0.85f);

                GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                crown.transform.parent = root.transform;
                crown.transform.localPosition = new Vector3(0, s * 1.55f, 0);
                crown.transform.localScale = new Vector3(s * 0.7f, s * 0.7f, s * 0.7f);
                break;
            }
            case PieceType.King:
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                body.transform.parent = root.transform;
                body.transform.localPosition = Vector3.zero;
                body.transform.localScale = new Vector3(s * 0.85f, s * 1.2f, s * 0.85f);

                GameObject crossV = GameObject.CreatePrimitive(PrimitiveType.Cube);
                crossV.transform.parent = root.transform;
                crossV.transform.localPosition = new Vector3(0, s * 1.55f, 0);
                crossV.transform.localScale = new Vector3(s * 0.18f, s * 0.6f, s * 0.18f);

                GameObject crossH = GameObject.CreatePrimitive(PrimitiveType.Cube);
                crossH.transform.parent = root.transform;
                crossH.transform.localPosition = new Vector3(0, s * 1.7f, 0);
                crossH.transform.localScale = new Vector3(s * 0.5f, s * 0.18f, s * 0.18f);
                break;
            }
        }

        return root;
    }
}