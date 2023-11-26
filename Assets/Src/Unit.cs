using Assets.Src.Enums;
using Assets.Src.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private PlayerController playerSubject;
    [SerializeField] public int numberOfUnitMembers = 5;
    [SerializeField] public int memberHP = 50;
    [SerializeField] public float unitSpeed = 1f;
    [SerializeField] public Material quadMaterial;
    [SerializeField] public GameObject unitMemberPrefab;
    [SerializeField] public float circleRadius = 2f;
    [SerializeField] public float rotationSpeed = 1f;


    public Guid UnitId { get; internal set; }

    public event Action<Guid, OrderType, Vector3> OnOrder;

    private VectorContainer _currentMovementOrder;
    private List<UnitMember> unitMembers;
    private GameObject selectionRing;
    private bool isSelected = false;

    private void Start()
    {
        UnitId = Guid.NewGuid();
        _currentMovementOrder = new VectorContainer();

        // Add a BoxCollider to make the Unit selectable
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        // Adjust the size of the collider according to your Unit size
        boxCollider.size = new Vector3(1f, 1f, 1f);

        playerSubject.MovementOrder += OnMovementOrder;
        SpawnUnitMembers();

        CreateSelectionRing();

        AddCompoundCollider();

        SetSelectionRingVisibility(false);
    }

    private void CreateSelectionRing()
    {
        // Create an empty GameObject for the selection ring
        selectionRing = new GameObject("SelectionRing");
        selectionRing.transform.parent = transform;

        // Create a quad for the ring
        GameObject ringQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ringQuad.name = "RingQuad";
        ringQuad.transform.parent = selectionRing.transform;

        // Adjust the scale and position to make it look like a ring
        // You may need to tweak these values based on your unit's size
        ringQuad.transform.localScale = new Vector3(circleRadius * 2, circleRadius * 2, 2f);

        // Position the ring at the base of the unit
        ringQuad.transform.localPosition = new Vector3(0f, 0.01f, 0f); // At the base of the unit

        ringQuad.transform.Rotate(new Vector3(90, 0, 0));

        ringQuad.GetComponent<Renderer>().material = quadMaterial;
    }


    public void SetSelectionRingVisibility(bool visible)
    {
        if (selectionRing != null)
        {
            selectionRing.SetActive(visible);
            isSelected = visible;
        }
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

    private void RotateTexture()
    {
        // Rotate the texture here
        // You can adjust the rotation speed based on your preferences
        float rotationAngle = rotationSpeed * Time.deltaTime;
        selectionRing.transform.Rotate(Vector3.up, rotationAngle);
    }

    void SpawnUnitMembers()
    {
        unitMembers = new List<UnitMember>();

        for (int i = 0; i < numberOfUnitMembers; i++)
        {
            float angle = i * 2 * Mathf.PI / numberOfUnitMembers;
            Vector3 memberPosition = transform.position + new Vector3(Mathf.Cos(angle) * circleRadius, 0f, Mathf.Sin(angle) * circleRadius);
            Quaternion memberRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
            var instansiationObject = Instantiate(unitMemberPrefab, memberPosition, memberRotation);
            UnitMember unitMember = instansiationObject.AddComponent<UnitMember>();

            unitMember.transform.parent = transform;
            unitMember.SetParentID(this.UnitId);
            unitMember.SetParentSubject(this);
            unitMember.maxHP = memberHP;
            unitMembers.Add(unitMember);
        }
    }

    private void OnMovementOrder(Guid _unitId, Vector3 movementCoordinates)
    {
        if (UnitId == _unitId)
        {
            movementCoordinates.y = 0.01f;
            OnOrder?.Invoke(_unitId, OrderType.Move, movementCoordinates);
        }
    }

    // Inside your Unit class
    private void PlayAnimationOnUnitMember(int memberIndex, string animationName)
    {
        if (memberIndex >= 0 && memberIndex < numberOfUnitMembers)
        {
            Animator memberAnimator = unitMembers[memberIndex].GetComponentInChildren<Animator>();

            if (memberAnimator != null)
            {
                memberAnimator.Play(animationName);
            }
            else
            {
                Debug.LogWarning("Animator component not found on unit member " + memberIndex);
            }
        }
        else
        {
            Debug.LogError("Invalid unit member index: " + memberIndex);
        }
    }

    private IEnumerator MoveUnit(Vector3 destination)
    {
        //float totalDistance = Vector3.Distance(transform.position, destination);
        //float elapsedTime = 0f;
        //Vector3 startPosition = transform.position;

        //while (elapsedTime < 1f)
        //{
        //    float currentSpeed = unitSpeed * Time.deltaTime / totalDistance;
        //    transform.position = Vector3.Lerp(startPosition, destination, elapsedTime);
        //    elapsedTime += currentSpeed;

        yield return null;
        //}

        //transform.position = destination; // Ensure the final position is exactly the destination
        //for (int i = 0; i < numberOfUnitMembers; i++)
        //{
        //    PlayAnimationOnUnitMember(i, "Idle");
        //}
    }


    private void Update()
    {
        // Rotate the texture if the unit is selected
        if (isSelected)
        {
            RotateTexture();
        }
    }

    private void OnDestroy()
    {
        if (playerSubject != null)
        {
            playerSubject.MovementOrder -= OnMovementOrder;
        }

    }
}
