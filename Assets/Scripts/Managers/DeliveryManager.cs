using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    // Here we place the recipes the customers are waiting for
    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;
    private int successfulRecipesAmount;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("There is more than one DeliveryManager instance.");

        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        // Only the server will generate the recipes
        if (!IsServer)
            return;

        // Start timer
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            // Spawn a new recipe only if the game has started AND under the max recipe count
            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                int waitingRecipeIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                //Debug.Log(waitingRecipeSO.recipeName);

                // We want to tell the client that a new recipe has been generated
                SpawnNewWaitingRecipeClientRpc(waitingRecipeIndex);
            }
        }
    }

    // NOTE - The host is also a client, so it will run both the code in Update and that
    // inside this method. The clients, however, will only run this method.
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeIndex)
    {
        // Grab a random recipe from the list
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeIndex];

        // Add it to the waiting list
        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; ++i)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // The recipe on the plate (made by us) has the same NUMBER of ingredients as the one the customer is waiting for
            if (waitingRecipeSO.kitchenObjectsList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;

                // Cycle through all ingredients in the recipe
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectsList)
                {
                    bool ingredientFound = false;

                    // Cycle through all ingredients in the plate
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // Check that each ingredient matches
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) 
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    // This Recipe ingredient was not found on the Plate
                    if (!ingredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }
                
                // Player delivered the correct recipe!
                if (plateContentsMatchesRecipe) 
                {
                    Debug.Log("Player delivered the correct recipe!");

                    DeliverCorrectRecipeServerRpc(i);

                    return;
                }
            }
        }

        // No matches found!
        // Player did not deliver a correct recipe
        //Debug.Log("Player did not deliver a correct recipe");
        DeliverIncorrectRecipeServerRpc();
    }

    // RequireOwnership = false; This means that the client that does not own
    // this network object will be able to trigger this server rpc
    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    // NOTE - Only the server will run this method
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        successfulRecipesAmount++;

        // Remove the delivered recipe from the waiting list
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipeAmount()
    {
        return successfulRecipesAmount;
    }
}
