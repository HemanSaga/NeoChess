using UnityEngine;

public class ChessBoardGenerator : MonoBehaviour
{
    [Header("Board Settings")]
    public float squareSize = 0.09f;
    public Material lightMaterial;
    public Material darkMaterial;

    // Store all tiles so spawner can look them up
    private GameObject[,] tiles = new GameObject[8, 8];

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = $"Tile_{row}_{col}";
                tile.transform.parent = this.transform;

                tile.transform.localPosition = new Vector3(
                    col * squareSize,
                    0,
                    row * squareSize
                );

                tile.transform.localScale = new Vector3(squareSize, 0.005f, squareSize);

                bool isLight = (row + col) % 2 == 0;
                tile.GetComponent<Renderer>().material = isLight ? lightMaterial : darkMaterial;

                // Store reference
                tiles[row, col] = tile;
            }
        }

        transform.position = new Vector3(
            -(squareSize * 8) * 0.5f,
             0.8f,
            -(squareSize * 8) * 0.5f
        );
    }

    // Call this from PieceSpawner to get exact tile center in world space
    public Vector3 GetTileWorldPosition(int row, int col)
    {
        if (tiles[row, col] == null)
        {
            Debug.LogError($"Tile {row},{col} is null! Make sure GenerateBoard runs before SpawnAllPieces.");
            return Vector3.zero;
        }
        // Return the tile's world position — it's already perfectly centered
        Vector3 pos = tiles[row, col].transform.position;
        return pos;
    }

    //adding the new method to the file 
    public Vector3 GetNearestTilePosition(Vector3 worldPos)
{
    Vector3 nearest = tiles[0, 0].transform.position;
    float minDist = float.MaxValue;

    for (int row = 0; row < 8; row++)
    {
        for (int col = 0; col < 8; col++)
        {
            if (tiles[row, col] == null) continue;

            float dist = Vector3.Distance(worldPos, tiles[row, col].transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = tiles[row, col].transform.position;
            }
        }
    }

    // Lift piece above tile surface
    return new Vector3(nearest.x, nearest.y + 0.022f, nearest.z);
}
}