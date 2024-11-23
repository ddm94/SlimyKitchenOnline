using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO() { return kitchenObjectSO; }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        // Clear previous parent
        this.kitchenObjectParent?.ClearKitchenObject();

        // Assign new parent
        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject");
        }

        // Set kitchen object to new parent
        kitchenObjectParent.SetKitchenObject(this);

        // Set position to the new clear counter follow point
        //transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();

        // Ensure the KitchenObject's position and rotation remain unchanged
        //transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;
    }

    public IKitchenObjectParent GetKitchenObjectParent() { return kitchenObjectParent; }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject() ;

        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;

            return true;
        }
        else
        {
            plateKitchenObject = null;

            return false;
        }
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }
}
