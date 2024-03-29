using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinGrid : MonoBehaviour
{
    #region Inspector
    [Header("Grid Settings")]
    [SerializeField]
    internal int _width;
    [SerializeField]
    internal int _height;
    [SerializeField]
    internal float _cellSize;
    [SerializeField]
    internal CellDisplay _cellDisplayPrefab;
    [SerializeField]
    private int minDistanceToPlayerPin;

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
    private bool[,] _grid;

    internal virtual void Start()
    {
        if (_enableDebugPlacement)
        {
            InputController.AddMouseUpListener(() => TryDebugPlacement());
        }

        _gridBackground = new CellDisplay[_width, _height];
        _grid = new bool[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _grid[x, y] = true;
                var cellDisplay = Instantiate(_cellDisplayPrefab, GetWorldPosition(x, y, true), Quaternion.identity, transform);
                _gridBackground[x, y] = cellDisplay;
                _gridBackground[x, y].AddMouseEnterListener(() => HighlightCell(cellDisplay));
                _gridBackground[x, y].AddMouseExitListener(() => ResetCellColor(cellDisplay));

                ResetCellColor(cellDisplay);
            }
        }
    }

    internal bool IsPlaceblaByPlayer(int x, int y)
    {
        return _grid[x, y];
    }

    internal Vector3 GetWorldPosition(int x, int y, bool getCenter = false)
    {
        var result = transform.position + (new Vector3(x, y) * _cellSize);

        if (getCenter)
        {
            result += new Vector3(1f, 1f, 0f) * _cellSize * 0.5f;
        }

        return result;
    }

    public void GetGridPosition(Vector3 worldPosition, out int x, out int y)
    {
        var temp = worldPosition - transform.position;
        x = Mathf.FloorToInt(temp.x / _cellSize);
        y = Mathf.FloorToInt(temp.y / _cellSize);
    }

    public bool IsValidGridPosition(int x, int y, bool checkPlaceable = false)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        if (x > _width-1 || y > _height-1)
        {
            return false;
        }

        if (checkPlaceable && !_grid[x, y])
        {
            return false;
        }

        return true;
    }

    public ActionPin PlacePin(int x, int y, ActionPin prefab)
    {
        var pin = Instantiate(prefab, GetWorldPosition(x, y, true), Quaternion.identity);

        SetSurroundingNotPlaceble(minDistanceToPlayerPin,x, y);

        return pin;
    }

    public ActionPin PlacePin(Vector3 worldPosition, ActionPin prefab)
    {
        GetGridPosition(worldPosition, out int x, out int y);
        return PlacePin(x, y, prefab);
    }

    public void RemovePin(ActionPin pin)
    {
        GetGridPosition(pin.transform.position, out int x, out int y);
        SetSurroundingPlaceble(minDistanceToPlayerPin, x, y);

        Destroy(pin.gameObject);
    }

    public void SetSurroundingNotPlaceble(int fields , int x, int y)
    {
        for(int i = x-fields; i <= x + fields; i++)
        {
            for (int j = y-fields; j <= y+fields; j++)
            {
                if (IsValidGridPosition(i, j, false))
                {
                    _grid[i, j] = false;
                    ResetCellColor(_gridBackground[i, j]);
                }
            }
        }
    }

    public void SetSurroundingPlaceble(int fields, int x, int y)
    {
        for (int i = x - fields; i <= x + fields; i++)
        {
            for (int j = y - fields; j <= y + fields; j++)
            {
                if (IsValidGridPosition(i, j, false))
                {
                    _grid[i, j] = true;
                    ResetCellColor(_gridBackground[i, j]);
                }
            }
        }
    }

    internal void HighlightCell(CellDisplay cell)
    {
        GetGridPosition(cell.transform.position, out int x, out int y);

        // TODO: Actual implementation
        if (_grid[x, y]==false)
        {
            _gridBackground[x, y].SetColor(_disabledColor);
        }
        else
        {
            _gridBackground[x, y].SetColor(_hoverColor);
        }
    }

    internal void ResetCellColor(CellDisplay cell)
    {
        GetGridPosition(cell.transform.position, out int x, out int y);

        // TODO: Actual implementation
        if (_grid[x, y] == false)
        {
            _gridBackground[x, y].SetColor(_disabledColor);
        }
        else
        {
            _gridBackground[x, y].SetColor(_baseColor);
        }
    }

    public void ShowGrid()
    {
        foreach(var cell in _gridBackground)
        {
            cell.gameObject.SetActive(true);
        }
    }

    public void HideGrid()
    {
        foreach (var cell in _gridBackground)
        {
            cell.gameObject.SetActive(false);
        }
    }

    internal void TryDebugPlacement()
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

    internal void OnDrawGizmosSelected()
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
