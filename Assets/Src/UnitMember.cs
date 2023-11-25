using Assets.Src.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitMember : MonoBehaviour
{
    private Unit parentSubject;
    private Guid parentID;

    // Setter method to set the parent ID
    public void SetParentID(Guid id)
    {
        parentID = id;
    }

    // Getter method to get the parent ID
    public Guid GetParentID()
    {
        return parentID;
    }

    // Setter method to set the parent subject
    public void SetParentSubject(Unit unit)
    {
        parentSubject = unit;
    }

    // Getter method to get the parent subject
    public Unit GetParentSubject()
    {
        return parentSubject;
    }

    private void Start()
    {
        parentSubject.onOrder += onOrder;
        
    }

    void onOrder(Guid _parentID, OrderType orderType, Vector3 destinationPoint)
    {

        if (_parentID == parentID)
        {
            switch (orderType)
            {
                case OrderType.Move:
                    moveUnitMember(destinationPoint);
                    break;
                default:
                    break;
            }
        }
    }

    private void moveUnitMember(Vector3 destinationPoint)
    {
        print("Moving member");
    }
}
