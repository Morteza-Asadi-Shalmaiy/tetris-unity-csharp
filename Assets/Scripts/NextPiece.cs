using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using TMPro;
public class NextPiece : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public TetrominoData[] tetrominoes;
    public TextMeshProUGUI bagCounterText;
    public TextMeshProUGUI tetrominoStatsText;
    public TextMeshProUGUI currentBagText;
    private Vector3Int[] cells;
    [SerializeField] private Vector3Int displayPosition = new Vector3Int(0, 0, 0);
    private Queue<TetrominoData> pieceBag = new Queue<TetrominoData>();
    private int bagsUsed = 0;
    private Dictionary<Tetromino, int> tetrominoUsage = new Dictionary<Tetromino, int>();
    private List<Tetromino> currentBagContents = new List<Tetromino>();
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
        

        foreach (Tetromino tetromino in System.Enum.GetValues(typeof(Tetromino)))
        {
            tetrominoUsage[tetromino] = 0;
        }

        if (tetrominoes != null)
        {
            for (int i = 0; i < tetrominoes.Length; i++)
            {
                tetrominoes[i].Initialize();
            }
            RefillBag();
        }
        else
        {
            Debug.LogError("NextPiece: Tetrominoes array is not assigned!");
        }
    }

    public TetrominoData SelectNextPiece()
    {
        Clear();
        
        if (tetrominoes == null || tetrominoes.Length == 0)
        {
            Debug.LogError("NextPiece: No tetrominoes assigned!");
            return default;
        }
        
        if (pieceBag.Count == 0)
        {
            RefillBag();
            bagsUsed++;
            UpdateStatisticsUI();
        }
        
        TetrominoData nextPieceData = pieceBag.Dequeue();
        currentBagContents.Remove(nextPieceData.tetromino);
        tetrominoUsage[nextPieceData.tetromino]++;
        
        Display(nextPieceData);
        UpdateStatisticsUI();
        
        return nextPieceData;
    }

    private void RefillBag()
    {
        currentBagContents.Clear();
        List<TetrominoData> tempBag = new List<TetrominoData>(tetrominoes);
        
 
        for (int i = 0; i < tempBag.Count; i++)
        {
            TetrominoData temp = tempBag[i];
            int randomIndex = Random.Range(i, tempBag.Count);
            tempBag[i] = tempBag[randomIndex];
            tempBag[randomIndex] = temp;
            
            currentBagContents.Add(tempBag[i].tetromino);
        }
        
        pieceBag.Clear();
        foreach (var piece in tempBag)
        {
            pieceBag.Enqueue(piece);
        }
    }

    private void UpdateStatisticsUI()
    {
        if (bagCounterText != null)
        {
            bagCounterText.text = $"Bags Used: {bagsUsed}";
        }
    
        if (tetrominoStatsText != null)
        {
            string stats = "";
            foreach (var entry in tetrominoUsage)
            {
                stats += $"{entry.Key} : {entry.Value}   ";
            }
            tetrominoStatsText.text = stats;
        }
    
        if (currentBagText != null)
        {
            if (currentBagContents.Count > 0)
            {
                string bagContents = "Current Bag:\n";
                bagContents += string.Join(" - ", currentBagContents);
                currentBagText.text = bagContents;
            }
            else
            {
                currentBagText.text = "Current Bag:\n(Empty)";
            }
        }
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
        if (data.cells == null)
        {
            Debug.LogError("NextPiece: Piece cells are null!");
            return;
        }
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