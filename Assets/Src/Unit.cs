using Assets.Src.Enums;
using Assets.Src.Model;
using Assets.Src.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] public int numberOfUnitMembers = 5;
    [SerializeField] public int memberHP = 50;
    [SerializeField] public float unitSpeed = 1f;
    [SerializeField] public Material quadMaterial;
    [SerializeField] public GameObject unitMemberPrefab;
    [SerializeField] public float circleRadius = 2f;
    [SerializeField] public float rotationSpeed = 1f;


    public Guid UnitId { get; internal set; }

    public event Action<Guid, OrderType, Vector3> OnOrder;

    private SelectionController _selectionControllerSubject;
    private VectorContainer _currentMovementOrder;
    private List<UnitMember> unitMembers;
    private GameObject selectionRing;

    private void Start()
    {
        UnitId = Guid.NewGuid();
        _currentMovementOrder = new VectorContainer();

        // Add a BoxCollider to make the Unit selectable
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        // Adjust the size of the collider according to your Unit size
        boxCollider.size = new Vector3(1f, 1f, 1f);

        _selectionControllerSubject = playerController.GetSelectionController();
        _selectionControllerSubject.MovementOrder += OnMovementOrder;
        SpawnUnitMembers();
        AddCompoundCollider();
    }

    private void AddCompoundCollider()
    {
        // Get all colliders of the unit and its children
        Collider[] colliders = GetComponentsInChildren<Collider>();

        // Calculate the compound bounds
        Bounds compoundBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Collider collider in colliders)
        {
            compoundBounds.Encapsulate(collider.bounds);
        }

        // Create a new BoxCollider using the compound bounds
        BoxCollider compoundCollider = gameObject.AddComponent<BoxCollider>();
        compoundCollider.center = compoundBounds.center - transform.position;
        compoundCollider.size = compoundBounds.size;
    }

    void SpawnUnitMembers()
    {
        unitMembers = new List<UnitMember>();
        int rowSize = (int)Mathf.Sqrt(numberOfUnitMembers);  // Adjust this based on your preference
        float spacing = 2.0f;

        for (int i = 0; i < numberOfUnitMembers; i++)
        {
            int row = i / rowSize;
            int col = i % rowSize;

            // Calculate the center position for the prefab instantiation
            Vector3 centerPosition = new Vector3((rowSize - 1) * spacing / 2f, 0, (rowSize - 1) * spacing / 2f);

            // Calculate the final position
            Vector3 position = new Vector3(col * spacing, 0, row * spacing) - centerPosition;

            // Instantiate the unit member prefab
            var instantiationObject = Instantiate(unitMemberPrefab, position, Quaternion.identity);
            UnitMember unitMember = instantiationObject.AddComponent<UnitMember>();

            // Set unit member properties
            unitMember.transform.parent = transform;
            unitMember.SetParentID(this.UnitId);
            unitMember.SetParentSubject(this);
            unitMember.maxHP = memberHP;
            unitMembers.Add(unitMember);

        }
    }

    public void SetSelectionRingVisibility(bool visible)
    {
        unitMembers.ForEach(member =>
        {
            member.SetSelectionRingVisibility(visible);
        });
    }

    private void OnMovementOrder(Guid _unitId, Vector3 movementCoordinates)
    {
        if (UnitId == _unitId)
        {
            int rowSize = (int)Mathf.Sqrt(unitMembers.Count);  // Assuming unitMembers is a List<UnitMember>
            float spacing = 2.0f;

            for (int i = 0; i < unitMembers.Count; i++)
            {
                UnitMember member = unitMembers[i];

                int row = i / rowSize;
                int col = i % rowSize;

                // Calculate the center position for the new formation
                Vector3 centerPosition = new Vector3((rowSize - 1) * spacing / 2f, 0, (rowSize - 1) * spacing / 2f);

                // Calculate the target position
                Vector3 targetPosition = new Vector3(col * spacing, 0, row * spacing) - centerPosition + movementCoordinates;

                // Move the unit member to the target position
                member.MoveUnitMember(targetPosition);
            }
        }
    }

    private void OnDestroy()
    {
        if (playerController != null)
        {
            _selectionControllerSubject.MovementOrder -= OnMovementOrder;
        }

    }
}
