using UnityEngine;
using UnityEngine.Tilemaps;
public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board mainBoard;
    public Piece trackingPiece;
    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }
    private void Start()
    {
        if (mainBoard == null)
        {
            Debug.LogError("Ghost: MainBoard reference is missing!");
        }
        
        if (trackingPiece == null)
        {
            Debug.LogError("Ghost: TrackingPiece reference is missing!");
        }
        
        if (tilemap == null)
        {
            Debug.LogError("Ghost: Tilemap component is missing!");
        }
    }
    private void LateUpdate()
    {
        if (mainBoard == null || trackingPiece == null || tilemap == null)
        {
            return;
        }
        
        Clear();
        Copy();
        Drop();
        Set();
    }
    private void Clear()
    {
        if (tilemap == null || cells == null) return;
        
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }
    private void Copy()
    {
        if (trackingPiece == null || cells == null || trackingPiece.cells == null)
        {
            return;
        }
        for (int i = 0; i < cells.Length && i < trackingPiece.cells.Length; i++) {
            cells[i] = trackingPiece.cells[i];
        }
    }
    private void Drop()
    {
        if (mainBoard == null || trackingPiece == null) return;
        Vector3Int position = trackingPiece.position;
        int current = position.y;
        int bottom = -mainBoard.boardSize.y / 2 - 1;
        mainBoard.Clear(trackingPiece);
        for (int row = current; row >= bottom; row--)
        {
            position.y = row;
            if (mainBoard.IsValidPosition(trackingPiece, position)) {
                this.position = position;
            } else {
                break;
            }
        }
        mainBoard.Set(trackingPiece);
    }
    private void Set()
    {
        if (tilemap == null || cells == null || tile == null) return;
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }
}