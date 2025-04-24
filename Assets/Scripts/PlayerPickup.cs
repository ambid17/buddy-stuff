using FishNet.Object;
using UnityEngine;

public class PlayerPickup : NetworkBehaviour
{
    public LayerMask layerMask;
    public CameraMove cameraMove;
    public Rigidbody objectControlled;
    public Vector3 pickupTargetPosition;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectControlled != null)
            {
                objectControlled.linearDamping = 1f;
                objectControlled.useGravity = true;
                objectControlled = null;
            }
            else
            {
                var rayStart = cameraMove.playerCamera.transform.position;
                var rayDirection = cameraMove.playerCamera.transform.forward;
                if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, PlayerConstants.pickupDistance, layerMask))
                {
                    // hit interactable
                    var rigidbody = hit.transform.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        objectControlled = rigidbody;
                        objectControlled.linearDamping = 4f;
                        objectControlled.useGravity = false;
                    }
                    else
                    {
                        Debug.LogError("Tried to pick up an object on the pickup layer, but it doesn't have a rigidbody");
                    }
                }
            }
        }

        if (objectControlled != null)
        {
            var rayStart = cameraMove.playerCamera.transform.position;
            var rayDirection = cameraMove.playerCamera.transform.forward;
            var newPosition = rayStart + (rayDirection * PlayerConstants.pickupDistance);
            pickupTargetPosition = newPosition;
        }
    }

    private void FixedUpdate()
    {
        if (objectControlled != null)
        {
            var force = pickupTargetPosition - objectControlled.transform.position;
            objectControlled.AddForce(force * PlayerConstants.pickupForce * Time.deltaTime, ForceMode.Impulse);
        }
    }
}
