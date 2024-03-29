using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    private PinGrid _grid;
    [SerializeField]
    private PinSelectionDisplay _selectionDisplay;
    [SerializeField]
    private PlayerBehavior _player;
    #endregion

    private Queue<ActionPin> _pinsToPlace;

    private List<ActionPin> _playerpins = new List<ActionPin>();

    private void Start()
    {
        TurnController.AddStartActionListener(AdjustGridVisibility);
        TurnController.AddStartPlacementListener(AdjustGridVisibility);
        TurnController.AddStartMonsterPlacementListener(AdjustGridVisibility);
        InputController.AddMouseUpListener(() => TryPlacePin());
        GameController.AddStartUpgradingListener(RemovePlayerPins);
        _selectionDisplay.AddPinCollectionSelectedListener(SetPinsToPlace);
    }

    private void OnDestroy()
    {
        TurnController.RemoveStartActionListener(AdjustGridVisibility);
        TurnController.RemoveStartPlacementListener(AdjustGridVisibility);
        TurnController.RemoveStartMonsterPlacementListener(AdjustGridVisibility);
        InputController.RemoveMouseUpListener(() => TryPlacePin());
        GameController.RemoveStartUpgradingListener(RemovePlayerPins);
        _selectionDisplay.RemovePinCollectionSelectedListener(SetPinsToPlace);
    }

    private void TryPlacePin(bool advancePhase = true)
    {
        if (TurnController.GetCurrentPhase() != TurnPhase.Placement)
        {
            return;
        }

        if (InputController.IsMouseOverUI())
        {
            return;
        }

        if (_pinsToPlace == null || _pinsToPlace.Count <= 0)
        {
            return;
        }

        _grid.GetGridPosition(InputController.GetMousePosition(), out int x, out int y);

        if (!_grid.IsValidGridPosition(x, y, true))
        {
            return;
        }

        var pin = _pinsToPlace.Dequeue();
        var pinObject = _grid.PlacePin(InputController.GetMousePosition(), pin);
        pinObject.SetOwner(_player);
        _playerpins.Add(pinObject);

        AudioManager.instance.Play("PlacePin");

        if (_pinsToPlace.Count <= 0 && advancePhase)
        {
            TurnController.NextPhase();
        }
        else
        {
            ShowPlacementInstruction();
        }
    }

    private void AdjustGridVisibility()
    {
        if (TurnController.GetCurrentPhase() != TurnPhase.Placement)
        {
            _grid.HideGrid();
        }
        else
        {
            _grid.ShowGrid();
        }
    }

    private void SetPinsToPlace(ActionPinCollection pinData)
    {
        if (pinData == null)
        {
            TurnController.NextPhase();
            return;
        }

        _pinsToPlace = new Queue<ActionPin>(pinData.Pins);
        ShowPlacementInstruction();
    }

    private void RemovePlayerPins()
    {
        foreach(var pin in _playerpins)
        {
            _grid.RemovePin(pin);
        }

        _playerpins.Clear();
    }

    public void ShowPlacementInstruction()
    {
        var pin = _pinsToPlace.Peek();
        TutorialDisplay.SetText($"Click to place a {pin.displayName} pin");
    }
}
