using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private void Update()
    {

    }

    public override void Interact(Player player)
    {
        // There is no KitchenObject here
        if (!HasKitchenObject())
        {
            // The Player is carrying something
            if (player.HasKitchenObject())
            {
                // Player drops the KitchenObject on this counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else // Player is not carrying anything
            {

            }
        }
        else // There is a KitchenObject here
        {
            // Player is already carrying a KitchenObject
            if (player.HasKitchenObject())
            {
                // Player is holding a plate
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Add ingredient to the plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // Destroy the old object on the counter
                        GetKitchenObject().DestroySelf();
                    }
                }
                else // Player is not carrying a plate but something else
                {
                    // Counter is holding a plate
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // Try add the ingredient the player is carrying to the plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else // Player is not carrying anything
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
