using UnityEngine;
using UnityEngine.Tilemaps;
public class HoldPiece : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    private TetrominoData? heldPiece;
    private Vector3Int[] cells;
    [SerializeField] private Vector3Int displayPosition = new Vector3Int(0, 0, 0);
    private bool canSwap = true;
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }
    public TetrominoData? Hold(TetrominoData currentPiece)
    {
        if (!canSwap)
        {
            return null;
        }

        Clear();
        
        TetrominoData? pieceToReturn = null;
        
        if (heldPiece.HasValue)
        {
            pieceToReturn = heldPiece.Value;
        }
        
        heldPiece = currentPiece;
        
        Display(currentPiece);
        
        canSwap = false;
        
        return pieceToReturn;
    }
    public void EnableSwap()
    {
        canSwap = true;
    }
    public bool CanSwap()
    {
        return canSwap;
    }
    private void Clear()
    {
        if (cells == null || tilemap == null) return;
        
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + displayPosition;
            tilemap.SetTile(tilePosition, null);
        }
    }
    private void Display(TetrominoData data)
    {
        if (tilemap == null) return;
        
        for (int i = 0; i < data.cells.Length && i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
            Vector3Int tilePosition = cells[i] + displayPosition;
            tilePosition = AdjustDisplayPosition(data.tetromino, tilePosition);
            tilemap.SetTile(tilePosition, data.tile);
        }
    }
    private Vector3Int AdjustDisplayPosition(Tetromino tetromino, Vector3Int position)
    {
        switch (tetromino)
        {
            case Tetromino.I:
                return position;
            case Tetromino.O:
                return position;
            case Tetromino.J:
            case Tetromino.L:
                return position;
            case Tetromino.S:
            case Tetromino.Z:
                return position;
            case Tetromino.T:
                return position;
            default:
                return position;
        }
    }
}