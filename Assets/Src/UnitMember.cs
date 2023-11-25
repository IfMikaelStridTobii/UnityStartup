using Assets.Src.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitMember : MonoBehaviour
{
    public int maxHP;
    private int _currentHP;

    public int currentHP
    {
        get { return _currentHP; }
        set
        {
            if (_currentHP != value)
            {
                _currentHP = value;
                OnHPChanged?.Invoke(_currentHP);

                if (_currentHP <= 0)
                {
                    Die();
                }
            }
        }
    }

    public event Action<int> OnHPChanged;
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
        currentHP = maxHP;
        parentSubject.OnOrder += OnOrder;
    }

    void OnOrder(Guid _parentID, OrderType orderType, Vector3 destinationPoint)
    {

        if (_parentID == parentID)
        {
            switch (orderType)
            {
                case OrderType.Move:
                    MoveUnitMember(destinationPoint);
                    break;
                default:
                    break;
            }
        }
    }

    private void MoveUnitMember(Vector3 destinationPoint)
    {
        print("Moving member");
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
