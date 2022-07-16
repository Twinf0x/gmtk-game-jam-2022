using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinGrid : MonoBehaviour
{
    #region Inspector
    [Header("Grid Settings")]
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;
    [SerializeField]
    private float _cellSize;
    [SerializeField]
    private CellDisplay _cellDisplayPrefab;

    [Space(5)]
    [Header("Display Settings")]
    [SerializeField]
    private Color _baseColor;
    [SerializeField]
    private Color _hoverColor;
    [SerializeField]
    private Color _disabledColor;
    [SerializeField]
    private Color _placeableColor;
    [SerializeField]
    private Color _notPlaceableColor;

    [Space(5)]
    [Header("Debug")]
    [SerializeField]
    private bool _enableDebugPlacement = false;
    [SerializeField]
    private ActionPin _debugObject;
    #endregion

    private CellDisplay[,] _gridBackground;
    private ActionPin[,] _grid;

    private void Start()
    {
        if (_enableDebugPlacement)
        {
            InputController.AddMouseUpListener(() => TryDebugPlacement());
        }

        _gridBackground = new CellDisplay[_width, _height];
        _grid = new ActionPin[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var cellDisplay = Instantiate(_cellDisplayPrefab, GetWorldPosition(x, y, true), Quaternion.identity, transform);
                _gridBackground[x, y] = cellDisplay;
                _gridBackground[x, y].AddMouseEnterListener(() => HighlightCell(cellDisplay));
                _gridBackground[x, y].AddMouseExitListener(() => ResetCellColor(cellDisplay));

                ResetCellColor(cellDisplay);
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y, bool getCenter = false)
    {
        var result = transform.position + (new Vector3(x, y) * _cellSize);

        if (getCenter)
        {
            result += new Vector3(1f, 1f, 0f) * _cellSize * 0.5f;
        }

        return result;
    }

    private void GetGridPosition(Vector3 worldPosition, out int x, out int y)
    {
        var temp = worldPosition - transform.position;
        x = Mathf.FloorToInt(temp.x / _cellSize);
        y = Mathf.FloorToInt(temp.y / _cellSize);
    }

    public bool IsValidGridPosition(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        if (x > _width || y > _height)
        {
            return false;
        }

        return true;
    }

    public void SetPin(int x, int y, ActionPin value)
    {
        _grid[x, y] = value;
    }

    public void SetPin(Vector3 worldPosition, ActionPin value)
    {
        GetGridPosition(worldPosition, out int x, out int y);
        SetPin(x, y, value);
    }

    public ActionPin GetPin(int x, int y)
    {
        return _grid[x, y];
    }

    private void HighlightCell(CellDisplay cell)
    {
        GetGridPosition(cell.transform.position, out int x, out int y);

        // TODO: Actual implementation
        if (_grid[x, y] != null)
        {
            _gridBackground[x, y].SetColor(_disabledColor);
        }
        else
        {
            _gridBackground[x, y].SetColor(_hoverColor);
        }
    }

    private void ResetCellColor(CellDisplay cell)
    {
        GetGridPosition(cell.transform.position, out int x, out int y);

        // TODO: Actual implementation
        if (_grid[x, y] != null)
        {
            _gridBackground[x, y].SetColor(_disabledColor);
        }
        else
        {
            _gridBackground[x, y].SetColor(_baseColor);
        }
    }

    private void TryDebugPlacement()
    {
        var mousePosition = InputController.GetMousePosition();
        Debug.Log($"Mouse position: {mousePosition}");
        GetGridPosition(mousePosition, out int x, out int y);

        if (!IsValidGridPosition(x, y))
        {
            Debug.Log($"Invalid grid position: {x}/{y}");
            return;
        }
        
        var debugPin = Instantiate(_debugObject, GetWorldPosition(x, y, true), Quaternion.identity);
        _grid[x, y] = debugPin;

        ResetCellColor(_gridBackground[x, y]);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Gizmos.DrawWireCube(GetWorldPosition(x, y, true), Vector3.one * _cellSize);
            }
        }
    }
}
