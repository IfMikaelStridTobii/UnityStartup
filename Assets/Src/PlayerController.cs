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
            Selection();
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

    private void Selection()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            // Check if the clicked object has the GameObjectExtensions script
            Unit unit = hitInfo.collider.GetComponent<Unit>();
            UnitMember unitMember = hitInfo.collider.GetComponent<UnitMember>();
            if (unit != null)
            {
                SelectUnit(unit);
            }
            else if (unitMember != null)
            {
                var parent = unitMember.transform.parent.GetComponent<Unit>();
                SelectUnit(parent);
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
    private void SelectUnit(Unit unit)
    {
        _selection.Add(unit);
        unit.SetSelectionRingVisibility(true);
    }
}
