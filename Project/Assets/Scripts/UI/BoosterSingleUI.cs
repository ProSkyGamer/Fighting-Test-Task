#region

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class BoosterSingleUI : MonoBehaviour
{
    #region Variables & References

    [SerializeField] private Image boosterIconImage;
    [SerializeField] private TextMeshProUGUI boosterOwnedCount;
    [SerializeField] private Button useBoosterButton;

    [SerializeField] private Image cooldownCircleImage;
    [SerializeField] private Transform cooldownTextTransform;
    [SerializeField] private Transform inUseTextTransform;
    [SerializeField] private Transform notEnoughCountTransform;

    private BaseBoosterSingle usingBooster;

    private bool isBoosterTimerNotZero;

    #endregion

    #region Initialization

    public void Initialize(BaseBoosterSingle newUsingBooster)
    {
        usingBooster = newUsingBooster;

        boosterIconImage.sprite = usingBooster.GetBoosterIcon();

        boosterOwnedCount.text = BoostersManager.Instance.GetBoosterCount(usingBooster.GetBoosterType()).ToString();

        useBoosterButton.onClick.AddListener(() =>
        {
            if (BoostersManager.Instance.GetBoosterCount(usingBooster.GetBoosterType()) <= 0) return;
            if (BoostersManager.Instance.IsBoosterActive(usingBooster.GetBoosterType())) return;
            if (BoostersManager.Instance.GetBoosterTimeCurrent(usingBooster.GetBoosterType()) > 0f) return;

            BoostersManager.Instance.ActivateBooster(usingBooster.GetBoosterType());
        });

        useBoosterButton.interactable = BoostersManager.Instance.GetBoosterCount(usingBooster.GetBoosterType()) > 0;

        BoostersManager.Instance.OnBoosterCountChanged += BoostersManager_OnBoosterCountChanged;
        BoostersManager.Instance.OnBoosterActivated += BoosterManager_OnBoosterActivated;
        BoostersManager.Instance.OnBoosterDeactivated += BoosterManager_OnBoosterDeactivated;

        UpdateVisuals();
    }

    private void BoosterManager_OnBoosterDeactivated(object sender, EventArgs e)
    {
        UpdateVisuals();
    }

    private void BoosterManager_OnBoosterActivated(object sender, EventArgs e)
    {
        UpdateVisuals();
    }

    private void BoostersManager_OnBoosterCountChanged(object sender, EventArgs e)
    {
        boosterOwnedCount.text = BoostersManager.Instance.GetBoosterCount(usingBooster.GetBoosterType()).ToString();
        UpdateVisuals();
    }

    #endregion

    #region Update

    private void Update()
    {
        if (isBoosterTimerNotZero)
            UpdateVisuals();
    }

    #endregion

    #region Visuals

    private void UpdateVisuals()
    {
        notEnoughCountTransform.gameObject.SetActive(false);
        inUseTextTransform.gameObject.SetActive(false);
        cooldownTextTransform.gameObject.SetActive(false);
        cooldownCircleImage.gameObject.SetActive(false);

        if (BoostersManager.Instance.GetBoosterCount(usingBooster.GetBoosterType()) <= 0)
        {
            notEnoughCountTransform.gameObject.SetActive(true);
        }
        else
        {
            var currentBoosterTotalTime = BoostersManager.Instance.GetBoosterTimeTotal(usingBooster.GetBoosterType());
            var currentBoosterTime = BoostersManager.Instance.GetBoosterTimeCurrent(usingBooster.GetBoosterType());

            if (BoostersManager.Instance.IsBoosterActive(usingBooster.GetBoosterType()))
                inUseTextTransform.gameObject.SetActive(true);
            else if (currentBoosterTime > 0f)
                cooldownTextTransform.gameObject.SetActive(true);

            cooldownCircleImage.gameObject.SetActive(currentBoosterTime > 0f);
            cooldownCircleImage.fillAmount = currentBoosterTime / currentBoosterTotalTime;
            isBoosterTimerNotZero = currentBoosterTime > 0f;
        }

        useBoosterButton.interactable = BoostersManager.Instance.GetBoosterCount(usingBooster.GetBoosterType()) > 0 &&
                                        BoostersManager.Instance.GetBoosterTimeCurrent(usingBooster.GetBoosterType()) <= 0f;
    }

    #endregion
}