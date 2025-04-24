using FishNet.Object;
using UnityEngine;

public class PlayerPickup : NetworkBehaviour
{
    public LayerMask layerMask;
    public CameraMove cameraMove;
    public Rigidbody objectControlled;
    public NetworkObject netObjectControlled;
    public Vector3 pickupTargetPosition;

    public override void OnStartClient()
    {
        if (!IsOwner)
            enabled = false;
        return;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectControlled != null)
            {
                Drop();
            }
            else
            {
                Pickup();
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

    void Pickup()
    {
        var rayStart = cameraMove.playerCamera.transform.position;
        var rayDirection = cameraMove.playerCamera.transform.forward;
        if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, PlayerConstants.pickupDistance, layerMask))
        {
            // hit interactable
            var rigidbody = hit.transform.GetComponent<Rigidbody>();
            var netObject = hit.transform.GetComponent<NetworkObject>();
            Debug.Log($"Tried to pick up object. Owner: {netObject.Owner}");
            if (rigidbody != null)
            {
                if(netObject != null && !netObject.Owner.IsValid)
                {
                    Debug.Log($"picked up object.");
                    netObjectControlled = netObject;
                    netObjectControlled.GiveOwnership(Owner);
                    objectControlled = rigidbody;
                    TogglePickUpServer(objectControlled.gameObject, isPickedUp: true);
                }
                else
                {
                    Debug.LogWarning($"Could not pick up, already owned by: {netObject.Owner}");
                }
                
            }
            else
            {
                Debug.LogWarning("Tried to pick up an object on the pickup layer, but it doesn't have a rigidbody");
            }
        }
    }

    void Drop()
    {
        Debug.Log("Drop");
        TogglePickUpServer(objectControlled.gameObject, isPickedUp: false);
        objectControlled = null;
        netObjectControlled.RemoveOwnership();
        netObjectControlled = null;
    }

    [ServerRpc]
    void TogglePickUpServer(GameObject objectToPickup, bool isPickedUp)
    {
        string action = isPickedUp ? "pick up" : "drop";
        Debug.Log($"TogglePickUpServer, object: {objectToPickup.name}, action: {action}");
        TogglePickUpObserver(objectToPickup, isPickedUp);
    }

    [ObserversRpc]
    void TogglePickUpObserver(GameObject objectToPickup, bool isPickedUp)
    {
        string action = isPickedUp ? "pick up" : "drop";
        Debug.Log($"TogglePickUpObserver, object: {objectToPickup.name}, action: {action}");
        var rigidbody = objectToPickup.GetComponent<Rigidbody>();
        rigidbody.linearDamping = isPickedUp ? 4f : 1f;
        rigidbody.useGravity = isPickedUp ? false : true;
    }
}
