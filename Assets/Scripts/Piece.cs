using UnityEngine;
public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }
    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;
    private float stepTime;
    private float moveTime;
    private float lockTime;
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;
        if (cells == null) {
            cells = new Vector3Int[data.cells.Length];
        }
        for (int i = 0; i < cells.Length; i++) {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }
    private void Update()
    {
        if (!board.enabled)
        {
            return;
        }
        board.Clear(this);
        lockTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q)) {
            Rotate(-1);
        } else if (Input.GetKeyDown(KeyCode.E)) {
            Rotate(1);
        }
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.C)) {
            board.HoldPiece();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            HardDrop();
        }
        if (Time.time > moveTime) {
            HandleMoveInputs();
        }
        if (Time.time > stepTime) {
            Step();
        }
        board.Set(this);
    }
    private void HandleMoveInputs()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down)) {
                stepTime = Time.time + stepDelay;
            }
        }
        if (Input.GetKey(KeyCode.A)) {
            Move(Vector2Int.left);
        } else if (Input.GetKey(KeyCode.D)) {
            Move(Vector2Int.right);
        }
    }
    public void Step()
    {
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);

        if (lockTime >= lockDelay) {
            Lock();
        }
    }
    private void HardDrop()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.tetrominoHardDropSound);
        }
        int distanceDropped = 0;
        while (Move(Vector2Int.down))
        {
            distanceDropped++;
        }
        int hardDropPoints = Mathf.Max(1, distanceDropped) * 10;
        board.AddToScore(hardDropPoints);
        if (board.scoringLog != null)
        {
            board.scoringLog.AddLogMessage($"Hard Drop Bonus: +{hardDropPoints}\nTotal Score: {board.score}\n");
        }
        Lock();
    }
    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.tetrominoLandSound);
        }
    }
    private bool Move(Vector2Int translation)
    {
        
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        bool valid = board.IsValidPosition(this, newPosition);
        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f;
            if (valid && AudioManager.Instance != null && translation.y == 0)
            {
                AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.tetrominoMoveSound);
            }
        }
        return valid;
    }
    private void Rotate(int direction)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.tetrominoRotateSound);
        }
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
        if (IsTSpin())
        {
            board.AddToScore(400);  
            if (board.scoringLog != null)
            {
                board.scoringLog.AddLogMessage($"T-Spin Bonus: +400\nTotal Score: {board.score}");
            }
        }
    }
    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];
            int x, y;
            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }
            cells[i] = new Vector3Int(x, y, 0);
        }
    }
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
                return true;
            }
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;
        if (rotationDirection < 0) {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }
    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } 
        else {
            return min + (input - min) % (max - min);
        }
    }
    private bool IsTSpin()
    {
        if (data.tetromino != Tetromino.T)
        {
            return false;
        }
        Vector3Int[] corners = new Vector3Int[]
        {
            new Vector3Int(position.x - 1, position.y + 1, 0),
            new Vector3Int(position.x + 1, position.y + 1, 0),
            new Vector3Int(position.x - 1, position.y - 1, 0),
            new Vector3Int(position.x + 1, position.y - 1, 0)
        };
        int filledCorners = 0;
        foreach (Vector3Int corner in corners)
        {
            if (board.tilemap.HasTile(corner) || !board.Bounds.Contains((Vector2Int)corner))
            {
                filledCorners++;
            }
        }
        return filledCorners >= 3;
    }
}