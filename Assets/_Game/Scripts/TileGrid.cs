using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [SerializeField] private TileRow _tileRowPrefab;

    [Header("Board Settings")]
    [SerializeField] private float _cellOffset = 0.1f;
    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }

    public int Size => cells.Length;
    public int Height => rows.Length;
    public int Width => Size / Height;
    private RectTransform _rectTransform;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        // rows = GetComponentsInChildren<TileRow>();
        // cells = GetComponentsInChildren<TileCell>();

        // for (int i = 0; i < cells.Length; i++)
        // {
        //     cells[i].coordinates = new Vector2Int(i % Width, i / Width);
        // }
    }
    public void BuildBoard(int width, int height)
    {
        DeleteAllCell();

        rows = new TileRow[height];

        List<TileCell> allCells = new List<TileCell>(width * height);

        // Size của TileGrid
        float gridWidth = _rectTransform.rect.width;
        float gridHeight = _rectTransform.rect.height;

        // ------------------------------------------------
        // Tính kích thước cell hình vuông
        // ------------------------------------------------

        float cellSizeByWidth =
            (gridWidth - (width - 1) * _cellOffset) / width;

        float cellSizeByHeight =
            (gridHeight - (height - 1) * _cellOffset) / height;

        // Cell luôn là hình vuông
        float cellSize = Mathf.Min(
            cellSizeByWidth,
            cellSizeByHeight
        );

        // Kích thước thực tế của toàn bộ board
        float boardWidth =
            width * cellSize +
            (width - 1) * _cellOffset;

        float boardHeight =
            height * cellSize +
            (height - 1) * _cellOffset;

        // ------------------------------------------------
        // Spawn Row
        // ------------------------------------------------

        for (int y = 0; y < height; y++)
        {
            TileRow row = Instantiate(_tileRowPrefab, transform);

            RectTransform rowRect = row.GetComponent<RectTransform>();

            // Row rộng bằng toàn bộ board
            rowRect.sizeDelta = new Vector2(
                boardWidth,
                cellSize
            );

            // Tính vị trí row
            float x = 0f;

            float startY =
                boardHeight * 0.5f -
                cellSize * 0.5f;

            float yPos =
                startY -
                y * (cellSize + _cellOffset);

            rowRect.anchoredPosition = new Vector2(
                x,
                yPos
            );

            row.CreateCell(
                width,
                cellSize,
                _cellOffset
            );

            rows[y] = row;

            allCells.AddRange(row.cells);
        }

        // ------------------------------------------------
        // Convert List -> Array
        // ------------------------------------------------

        cells = allCells.ToArray();

        // ------------------------------------------------
        // Set coordinates
        // ------------------------------------------------

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].coordinates = new Vector2Int(
                i % Width,
                i / Width
            );
        }
    }
    public void DeleteAllCell()
    {
        if (rows != null)
        {
            foreach (var row in rows)
            {
                Destroy(row);
            }
        }
        if (cells != null)
        {
            foreach (var cell in cells)
            {
                Destroy(cell);
            }
        }
        rows = null;
        cells = null;
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return rows[y].cells[x];
        }
        else
        {
            return null;
        }
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;

        while (cells[index].Occupied)
        {
            index++;

            if (index >= cells.Length)
            {
                index = 0;
            }

            // all cells are occupied
            if (index == startingIndex)
            {
                return null;
            }
        }

        return cells[index];
    }

}
