using UnityEngine;

public class TileRow : MonoBehaviour
{
    [SerializeField] private TileCell _tileCellPrefab;

    public TileCell[] cells { get; private set; }

    private void Awake()
    {
        cells = GetComponentsInChildren<TileCell>();
    }

    public void CreateCell(
        int count,
        float cellSize,
        float offset
    )
    {
        cells = new TileCell[count];

        RectTransform rowRect =
            GetComponent<RectTransform>();

        float totalWidth =
            count * cellSize +
            (count - 1) * offset;

        // ------------------------------------------------
        // Spawn Cell
        // ------------------------------------------------

        for (int i = 0; i < count; i++)
        {
            TileCell cell =
                Instantiate(_tileCellPrefab, transform);

            RectTransform cellRect =
                cell.GetComponent<RectTransform>();

            // Cell luôn hình vuông
            cellRect.sizeDelta = new Vector2(
                cellSize,
                cellSize
            );

            // ------------------------------------------------
            // Position
            // ------------------------------------------------

            float startX =
                -totalWidth * 0.5f +
                cellSize * 0.5f;

            float x =
                startX +
                i * (cellSize + offset);

            cellRect.anchoredPosition = new Vector2(
                x,
                0f
            );

            cells[i] = cell;
        }
    }
}