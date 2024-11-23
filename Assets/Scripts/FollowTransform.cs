using UnityEngine;

/// <summary>
/// Resolves the InvalidParentException error: "Invalid parenting, NetworkObject moved under a non-NetworkObject parent."
/// 
/// Explanation:
/// - Both Player and Counters are NetworkObjects.
/// - The parent of a KitchenObject is a child transform within their respective Prefabs, which is not a NetworkObject.
/// 
/// Note:
/// - Netcode imposes a limitation: you cannot set a NetworkObject as a child of another NetworkObject.
/// 
/// This class provides a workaround to handle this scenario.
/// </summary>
public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    private void LateUpdate()
    {
        if (targetTransform == null)
            return;

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
