using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColour;
    [SerializeField] private Color failedColour;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

    private const string POPUP = "Popup";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);

        animator.SetTrigger(POPUP);

        backgroundImage.color = failedColour;

        iconImage.sprite = failedSprite;

        messageText.text = "DELIVERY\nFAILED";
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);

        animator.SetTrigger(POPUP);

        backgroundImage.color = successColour;

        iconImage.sprite = successSprite;

        messageText.text = "DELIVERY\nSUCCESS";
    }
}
