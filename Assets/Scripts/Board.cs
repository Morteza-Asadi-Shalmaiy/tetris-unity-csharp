using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public GameOverScreen GameOverScreen;
    public NextPiece nextPieceDisplay;
    public HoldPiece holdPieceDisplay;
    private TetrominoData nextPiece;
    public int score { get; private set; }
    public int level { get; private set; }
    public int linesCleared { get; private set; }
    private int linesToNextLevel = 10;
    private float stepDelay = 1f;
    private float speedIncrease = 0.85f;
    private int comboMultiplier = 1;
    private float timeSinceLastClear = 0f;
    private const int basePointsPerLine = 100;
    public ScoringLog scoringLog;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI levelDisplayText;

    private void UpdateLevelUI()
    {
        if (levelDisplayText != null)
        {
            levelDisplayText.text = "Level : " + level.ToString();
        }
    }

    private void UpdateScoreUI()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = "Current Score : " + score.ToString();
        }
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score : " + LoadHighScore().ToString();
        }
    }
    private void Update()
    {
        timeSinceLastClear += Time.deltaTime;
    }
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
        score = 0;
        level = 1;
        linesCleared = 0;
    }
    private void Start()
    {
        UpdateHighScoreUI();
        UpdateLevelUI();
        if (nextPieceDisplay != null)
        {
            nextPiece = nextPieceDisplay.SelectNextPiece();
        }
        else
        {
            int random = Random.Range(0, tetrominoes.Length);
            nextPiece = tetrominoes[random];
        }
        SpawnPiece();
    }
    public void SpawnPiece()
    {
        if (holdPieceDisplay != null)
        {
            holdPieceDisplay.EnableSwap();
        }
        TetrominoData data = nextPiece;
        if (nextPieceDisplay != null)
        {
            nextPiece = nextPieceDisplay.SelectNextPiece();
        }
        else
        {
            int random = Random.Range(0, tetrominoes.Length);
            nextPiece = tetrominoes[random];
        }
        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }
    public void HoldPiece()
    {
        if (holdPieceDisplay == null || !holdPieceDisplay.CanSwap())
            return;
        TetrominoData currentData = activePiece.data;
        Clear(activePiece);
        TetrominoData? heldPiece = holdPieceDisplay.Hold(currentData);

        if (heldPiece.HasValue)
        {
            activePiece.Initialize(this, spawnPosition, heldPiece.Value);
        }
        else
        {
            activePiece.Initialize(this, spawnPosition, nextPiece);
            
            if (nextPieceDisplay != null)
            {
                nextPiece = nextPieceDisplay.SelectNextPiece();
            }
            else
            {
                int random = Random.Range(0, tetrominoes.Length);
                nextPiece = tetrominoes[random];
            }
        }
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }
    public void GameOver()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverMusic();
        }
        if (activePiece != null)
        {
            Destroy(activePiece);
        }
        tilemap.ClearAllTiles();
        int highScore = LoadHighScore();
        if (score > highScore)
        {
            SaveHighScore(score);
            highScore = score; 
        }
        GameOverScreen.Setup(score, highScore, level);
        UpdateScoreUI();
        this.enabled = false;
    }
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }
    private void ResetCombo()
    {
        comboMultiplier = 1;
    }
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesClearedThisMove = 0;
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                linesCleared++;
                linesClearedThisMove++;
            }
            else
            {
                row++;
            }
        }
        if (linesClearedThisMove > 0)
        {
            UpdateScore(linesClearedThisMove);
            timeSinceLastClear = 0f;
        }
        else
        {
            comboMultiplier = 1;
        }
        if (linesCleared >= linesToNextLevel)
        {
            LevelUp();
        }
        if (linesClearedThisMove > 0)
        {
            UpdateScore(linesClearedThisMove);
            timeSinceLastClear = 0f;
        }
        else
        {
            ResetCombo();
        }
    }
    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }
    public void LineClear(int row)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.lineClearSound);
        }
        RectInt bounds = Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }
            row++;
        }
    }
    private void UpdateScore(int linesClearedThisMove)
    {
        int pointsPerLine = basePointsPerLine + (level - 1) * 50;
        float bonusMultiplier = 1f;
        if (linesClearedThisMove == 2) bonusMultiplier = 1.5f;
        else if (linesClearedThisMove == 3) bonusMultiplier = 2f;
        else if (linesClearedThisMove == 4) bonusMultiplier = 3f;
        float timeBonus = Mathf.Max(0, 10 - timeSinceLastClear) * 10;
        int moveScore = Mathf.RoundToInt(linesClearedThisMove * pointsPerLine * level * bonusMultiplier * comboMultiplier + timeBonus);
        score += moveScore;
        UpdateScoreUI();
        if (scoringLog != null)
        {
            string logMessage = $"Lines Cleared: {linesClearedThisMove}\n" +
                                $"Points Per Line: {pointsPerLine}\n" +
                                $"Level: {level}\n" +
                                $"Bonus Multiplier: {bonusMultiplier}x\n" +
                                $"Combo Multiplier: {comboMultiplier}x\n" +
                                $"Time Bonus: {timeBonus:F2}\n" +
                                $"Move Score: {moveScore}\n" +
                                $"Total Score: {score}\n";
            scoringLog.AddLogMessage(logMessage);
        }
        if (linesClearedThisMove > 0)
        {
            comboMultiplier++;
        }
        if (IsBoardEmpty())
        {
            score += 10000;
            if (scoringLog != null)
            {
                scoringLog.AddLogMessage($"Perfect Clear Bonus: +10000\nTotal Score: {score}");
            }
        }
    }
    private bool IsBoardEmpty()
    {
        RectInt bounds = Bounds;
        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                if (tilemap.HasTile(position))
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void LevelUp()
    {
        level++;
        linesCleared = 0;
        stepDelay *= speedIncrease;
        linesToNextLevel += 5;
        UpdateLevelUI();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.AdjustMusicTempo(1 / speedIncrease);
        }
    }
    public void AddToScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }
    private void SaveHighScore(int score)
    {
        PlayerPrefs.SetInt("HighScore", score);
        PlayerPrefs.Save();
    }
    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
}