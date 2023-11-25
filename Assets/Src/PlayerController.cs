using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public event Action<Guid, Vector3> MovementOrder;

    private List<Unit> _selection = new List<Unit>();
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectUnit();
        }
        if (Input.GetMouseButtonDown(1))
        {
            SelectAction();
        }
    }

    private void SelectAction()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            Vector3 targetPosition = hitInfo.point;
            _selection.ForEach(x => MovementOrder?.Invoke(x.UnitId, targetPosition));
        }
    }

    private void SelectUnit()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            // Check if the clicked object has the GameObjectExtensions script
            Unit unit = hitInfo.collider.GetComponent<Unit>();
            if (unit != null)
            {
                _selection.Add(unit);
                unit.SetSelectionRingVisibility(true);
            }
            else
            {
                foreach (var selectedUnit in _selection)
                {
                    selectedUnit.SetSelectionRingVisibility(false);
                }
                _selection.Clear();
            }
        }
    }
}
