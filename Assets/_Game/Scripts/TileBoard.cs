using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
public class TileBoard : MonoBehaviour
{
    private sealed class BoardSnapshot
    {
        public readonly int[] stateIndices;
        public readonly int score;

        public BoardSnapshot(int[] stateIndices, int score)
        {
            this.stateIndices = stateIndices;
            this.score = score;
        }
    }

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileState[] tileStates;
    [SerializeField] private float swipeThreshold = 50f;
    private TileGrid grid;
    private List<Tile> tiles;
    private Stack<BoardSnapshot> history;
    private bool waiting;
    private Vector2 touchStartPosition;
    private bool trackingTouch;
#if UNITY_EDITOR
    private bool touchSimulationEnabled;
#endif

    public bool CanUndo => !waiting && history != null && history.Count > 0;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>();
        history = new Stack<BoardSnapshot>();
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (Touchscreen.current == null)
        {
            TouchSimulation.Enable();
            touchSimulationEnabled = true;
        }
#endif
    }

    private void OnDisable()
    {
        trackingTouch = false;

#if UNITY_EDITOR
        if (touchSimulationEnabled)
        {
            TouchSimulation.Disable();
            touchSimulationEnabled = false;
        }
#endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            trackingTouch = false;
        }
    }

    public void BuildBoard()
    {
        ClearBoard();
        grid.BuildBoard(GameManager.WIDTH, GameManager.HEIGHT);
        history.Clear();
    }

    public void ClearBoard()
    {
        if (grid != null && grid.cells != null)
        {
            foreach (var cell in grid.cells)
            {
                Destroy(cell.gameObject);
            }
        }

        if (tiles != null)
        {
            foreach (var tile in tiles)
            {
                Destroy(tile.gameObject);
            }

            tiles.Clear();
        }
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0]);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void Update()
    {
        if (waiting) return;
        if (grid == null || grid.cells == null || grid.cells.Length == 0) return;

        HandleTouchInput();

        if (Keyboard.current != null &&
            (Keyboard.current.wKey.wasPressedThisFrame ||
             Keyboard.current.upArrowKey.wasPressedThisFrame))
        {
            Move(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (Keyboard.current != null &&
                 (Keyboard.current.aKey.wasPressedThisFrame ||
                  Keyboard.current.leftArrowKey.wasPressedThisFrame))
        {
            Move(Vector2Int.left, 1, 1, 0, 1);
        }
        else if (Keyboard.current != null &&
                 (Keyboard.current.sKey.wasPressedThisFrame ||
                  Keyboard.current.downArrowKey.wasPressedThisFrame))
        {
            Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
        }
        else if (Keyboard.current != null &&
                 (Keyboard.current.dKey.wasPressedThisFrame ||
                  Keyboard.current.rightArrowKey.wasPressedThisFrame))
        {
            Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
        }

    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;
        Debug.Log("Touch 00");
        switch (touch.phase.ReadValue())
        {
            case UnityEngine.InputSystem.TouchPhase.Began:
                Debug.Log("Touch");
                touchStartPosition = touch.position.ReadValue();
                trackingTouch = true;
                break;

            case UnityEngine.InputSystem.TouchPhase.Ended:
                if (!trackingTouch) break;

                Vector2 delta = touch.position.ReadValue() - touchStartPosition;
                trackingTouch = false;
                TryMoveFromSwipe(delta);
                break;

            case UnityEngine.InputSystem.TouchPhase.Canceled:
                trackingTouch = false;
                break;
        }
    }

    private void TryMoveFromSwipe(Vector2 delta)
    {
        if (delta.magnitude < swipeThreshold) return;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            Move(delta.x > 0 ? Vector2Int.right : Vector2Int.left,
                delta.x > 0 ? grid.Width - 2 : 1,
                delta.x > 0 ? -1 : 1,
                0,
                1);
        }
        else
        {
            Move(delta.y > 0 ? Vector2Int.up : Vector2Int.down,
                0,
                1,
                delta.y > 0 ? 1 : grid.Height - 2,
                delta.y > 0 ? 1 : -1);
        }
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        BoardSnapshot beforeMove = CaptureSnapshot();
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed)
        {
            history.Push(beforeMove);
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        b.SetState(newState);
        GameManager.Ins.IncreaseScore(newState.number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        if (tiles.Count != grid.Size)
        {
            CreateTile();
        }

        if (CheckForGameOver())
        {
            GameManager.Ins.GameOver();
        }
    }

    public bool Undo()
    {
        if (!CanUndo) return false;

        StopAllCoroutines();
        waiting = false;

        BoardSnapshot snapshot = history.Pop();
        RestoreSnapshot(snapshot);
        return true;
    }

    private BoardSnapshot CaptureSnapshot()
    {
        int[] stateIndices = new int[grid.Size];

        for (int i = 0; i < grid.cells.Length; i++)
        {
            Tile tile = grid.cells[i].tile;
            stateIndices[i] = tile == null ? -1 : IndexOf(tile.state);
        }

        return new BoardSnapshot(stateIndices, GameManager.Ins.Score);
    }

    private void RestoreSnapshot(BoardSnapshot snapshot)
    {
        ClearBoard();
        grid.BuildBoard(GameManager.WIDTH, GameManager.HEIGHT);

        for (int i = 0; i < snapshot.stateIndices.Length; i++)
        {
            int stateIndex = snapshot.stateIndices[i];
            if (stateIndex < 0) continue;

            Tile tile = Instantiate(tilePrefab, grid.transform);
            tile.SetState(tileStates[stateIndex]);
            tile.Spawn(grid.cells[i]);
            tiles.Add(tile);
        }

        GameManager.Ins.RestoreScore(snapshot.score);
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.Size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }

}
