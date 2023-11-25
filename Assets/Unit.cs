using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorContainer
{
    public Guid UnitId { get; private set; }
    public Vector3 OriginPoint { get; private set; }
    public Vector3 DestinationPoint { get; private set; }

    public void UpdateVectors(Vector3 newFirstVector, Vector3 newSecondVector)
    {
        OriginPoint = newFirstVector;
        DestinationPoint = newSecondVector;
    }
}

public class Unit : MonoBehaviour
{
    [SerializeField] private PlayerController playerSubject;
    [SerializeField] public int numberOfUnitMembers = 5;
    [SerializeField] public float unitSpeed = 1f;
    [SerializeField] public Material quadMaterial;
    [SerializeField] public GameObject unitMemberPrefab;
    [SerializeField] public float circleRadius = 2f;
    [SerializeField] public float rotationSpeed = 1f;
    private VectorContainer _currentMovementOrder;
    public Guid UnitId { get; internal set; }
    private Transform[] unitMembers;
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

        // Remove individual BoxColliders
        foreach (BoxCollider collider in GetComponentsInChildren<BoxCollider>())
        {
            Destroy(collider);
        }
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
        unitMembers = new Transform[numberOfUnitMembers];

        for (int i = 0; i < numberOfUnitMembers; i++)
        {
            float angle = i * 2 * Mathf.PI / numberOfUnitMembers;
            Vector3 memberPosition = transform.position + new Vector3(Mathf.Cos(angle) * circleRadius, 0f, Mathf.Sin(angle) * circleRadius);
            Quaternion memberRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);

            GameObject unitMember = Instantiate(unitMemberPrefab, memberPosition, memberRotation);
            unitMembers[i] = unitMember.transform;

            // Set the parent of the unit member to the unit, so it moves with the unit
            unitMember.transform.parent = transform;
        }
    }

    private void OnMovementOrder(Guid _unitId, Vector3 movementCoordinates)
    {
        if (UnitId == _unitId)
        {
            movementCoordinates.y = 0.01f;
            StopAllCoroutines(); // Stop any ongoing interpolation
            StartCoroutine(MoveUnit(movementCoordinates));
            // Ensure the final rotation is towards the
            int unitCount = 0;
            foreach (var unitMember in unitMembers)
            {
                Quaternion finalRotation = Quaternion.LookRotation(movementCoordinates - unitMember.position);
                unitMember.rotation = finalRotation;
                PlayAnimationOnUnitMember(unitCount, "Run");
                unitCount++;
            }
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
        float totalDistance = Vector3.Distance(transform.position, destination);
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < 1f)
        {
            float currentSpeed = unitSpeed * Time.deltaTime / totalDistance;
            transform.position = Vector3.Lerp(startPosition, destination, elapsedTime);
            elapsedTime += currentSpeed;

            yield return null;
        }

        transform.position = destination; // Ensure the final position is exactly the destination
        for (int i = 0; i < numberOfUnitMembers; i++)
        {
            PlayAnimationOnUnitMember(i, "Idle");
        }
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