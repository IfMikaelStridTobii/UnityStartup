using Assets.Src.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private NavMeshAgent navMeshAgent;
    private bool hasMovementOrder;
    private Vector3 _destinationPoint;

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

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            // Add NavMeshAgent if not already attached
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        // TODO: Set NavMeshAgent properties here (e.g., speed, acceleration, etc.)
    }

    private void Update()
    {
        float distanceToDestination = Vector3.Distance(transform.position, _destinationPoint);

        if (hasMovementOrder && navMeshAgent != null && distanceToDestination > 0.5f)
        {
            navMeshAgent.SetDestination(_destinationPoint);
        }
        else
        {
            hasMovementOrder = false;
            PlayAnimation("Idle");
        }
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
        hasMovementOrder = true;
        PlayAnimation("Run");
        _destinationPoint = destinationPoint;
    }

    public void PlayAnimation(string animationIdentifier)
    {
        Animator memberAnimator = this.GetComponent<Animator>();
        if (memberAnimator != null)
        {
            memberAnimator.Play(animationIdentifier);
        }

    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
