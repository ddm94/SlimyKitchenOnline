using UnityEngine;

/// <summary>
/// This class is responsible for cleaning up any leftover static data, such as static listeners.
/// </summary>
public class ResetStaticDataManager : MonoBehaviour
{
    private void Awake()
    {
        BaseCounter.ResetStaticData();
        CuttingCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        Player.ResetStaticData();
    }
}
