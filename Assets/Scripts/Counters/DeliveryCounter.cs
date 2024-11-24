public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // Only accepts plates
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                // Destroy the plate
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }
}
