using Assets.Src.Enums;
using System;
using UnityEngine;

namespace Assets.Src.Player
{
    public class EventController : MonoBehaviour
    {
        void Start()
        {
            InstantiateUnit();
            InstantiateUnit();
        }

        void InstantiateUnit()
        {
            // Assuming you have a reference to the PlayerController
            PlayerController playerController = GetComponent<PlayerController>();

            // Instantiate the Unit prefab
            GameObject unitGameObject = new GameObject("Unit");
            Unit unitComponent = unitGameObject.AddComponent<Unit>();

            // Set properties of the Unit component
            unitComponent.playerController = playerController; // Set other properties as needed

            // You may want to set the position or parent the unitGameObject to another GameObject in the scene

            // Optionally, you can set other properties or invoke methods on the unitComponent as needed

            // Example:
            unitComponent.numberOfUnitMembers = 10;

            // Example:
            unitComponent.unitSpeed = 2f;
            GameObject unitMemberPrefab = Resources.Load<GameObject>("Units/Basic/space_man_model");

        
            // Example:
            unitComponent.unitMemberPrefab = unitMemberPrefab;

            // Example:
            unitComponent.circleRadius = 1f;

            // Example:
            unitComponent.rotationSpeed = 2f;

            // Example: unitComponent.quadMaterial = yourMaterial;

            // Example: unitComponent.SpawnUnitMembers(); // Call any initialization method

            // Example: unitComponent.SetSelectionRingVisibility(true); // Call any other methods

            // Example: unitComponent.OnOrder += HandleUnitOrder; // Subscribe to events if needed

            // Store the unitGameObject reference if you need to access it later
            // e.g., you want to destroy the unit or perform other operations

            // Example: Store reference if needed
            // playerController.SetUnitReference(unitComponent);

        }

        void HandleUnitDestroy()
        {
            // Handle cleanup or other actions when the Unit is destroyed
            Debug.Log("Unit destroyed");
        }

        // Example event handler method
        void HandleUnitOrder(Guid unitId, OrderType orderType, Vector3 orderLocation)
        {
            // Handle Unit order event
        }
    }
}
