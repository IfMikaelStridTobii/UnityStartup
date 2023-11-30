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
    public GameObject _memberPrefab;
    public string ringMaterialName = "Materials/SelectedRingMaterial";

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
    private GameObject selectionRing;
    private float circleRadius = 1.0f; // Adjust this based on your preference
    public Material quadMaterial;
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

        CreateSelectionRing();

        navMeshAgent = GetComponent<NavMeshAgent>();


        if (navMeshAgent == null)
        {
            // Add NavMeshAgent if not already attached
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            // TODO: Set NavMeshAgent properties here (e.g., speed, acceleration, etc.)
            navMeshAgent.radius = 0.2f;
        }

    }

    private void LoadMaterials()
    {
        // Find Renderer component dynamically (in the children)
        Renderer rendererComponent = GetComponentInChildren<Renderer>();

        if (rendererComponent != null)
        {
            // Load material from Resources folder
            Material loadedMaterial = Resources.Load<Material>(ringMaterialName);

            // Apply the material to the found renderer component
            if (loadedMaterial != null)
            {
                rendererComponent.material = loadedMaterial;
            }
            else
            {
                Debug.LogError("Material not found: " + ringMaterialName);
            }
        }
        else
        {
            Debug.LogError("Renderer component not found in children.");
        }
    }


    private void CreateSelectionRing()
    {
        // Create an empty GameObject for the selection ring as a child of the unit member
        selectionRing = new GameObject("SelectionRing");
        selectionRing.transform.parent = transform;
        // Create a quad for the ring
        GameObject ringQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        
        Destroy(ringQuad.GetComponent<Collider>());
        ringQuad.name = "RingQuad";
        //ringQuad.transform.parent = selectionRing.transform;
        // Adjust the scale to make it look like a ring
        ringQuad.transform.localScale = new Vector3(circleRadius * 2, circleRadius * 2, 2f);
        // Position the ring at the center of the unit member, accounting for any offset

        ringQuad.transform.SetPositionAndRotation(this.transform.position, Quaternion.identity);
        ringQuad.transform.Rotate(new Vector3(90, 0, 0));
        ringQuad.transform.SetParent(this.transform);
        ringQuad.GetComponent<Renderer>().material = (Material)Resources.Load(ringMaterialName);
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
