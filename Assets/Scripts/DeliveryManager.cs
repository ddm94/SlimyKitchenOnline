using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    // Here we place the recipes the customers are waiting for
    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer;
    private float spawnRecipeTImerMax = 4f;
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
        // Start timer
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTImerMax;

            // Spawn a new recipe only if the game has started AND under the max recipe count
            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                // Grab a random recipe from the list
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                //Debug.Log(waitingRecipeSO.recipeName);

                // Add it to the waiting list
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
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

                    successfulRecipesAmount++;

                    // Remove the delivered recipe from the waiting list
                    waitingRecipeSOList.RemoveAt(i);

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

                    return;
                }
            }
        }

        // No matches found!
        // Player did not deliver a correct recipe
        //Debug.Log("Player did not deliver a correct recipe");
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
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
