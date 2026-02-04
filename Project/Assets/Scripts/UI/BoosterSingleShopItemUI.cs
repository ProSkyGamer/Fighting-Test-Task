#region

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class BoosterSingleShopItemUI : MonoBehaviour
{
    #region Variables & References

    [SerializeField] private Image boosterIconImage;
    [SerializeField] private TextMeshProUGUI boosterPriceText;
    [SerializeField] private TextMeshProUGUI boosterOwnedCount;
    [SerializeField] private NumberInputFieldFilter buyingAmountInputFieldFilter;
    [SerializeField] private Button increaseAmountButton;
    [SerializeField] private Button decreaseAmountButton;
    [SerializeField] private Button buyBoostersButton;

    private BaseBoosterSingle sellingBooster;

    #endregion

    #region Initialization

    public void Initialize(BaseBoosterSingle newSellingBooster)
    {
        sellingBooster = newSellingBooster;

        boosterIconImage.sprite = sellingBooster.GetBoosterIcon();
        buyingAmountInputFieldFilter.SetIntValue(1);
        boosterPriceText.text = sellingBooster.GetBoosterCoinsCost().ToString();

        buyBoostersButton.onClick.AddListener(() =>
        {
            var boostersPrice = sellingBooster.GetBoosterCoinsCost() * buyingAmountInputFieldFilter.GetInputFieldIntValue();
            if (!CoinsManager.Instance.IsHasEnoughCoins(boostersPrice)) return;

            BoostersManager.Instance.BuyBooster(sellingBooster.GetBoosterType(), buyingAmountInputFieldFilter.GetInputFieldIntValue());
        });

        increaseAmountButton.onClick.AddListener(() =>
        {
            var currentBuyingAmount = buyingAmountInputFieldFilter.GetInputFieldIntValue();
            currentBuyingAmount += 1;

            buyingAmountInputFieldFilter.SetIntValue(currentBuyingAmount);
        });

        boosterOwnedCount.text = BoostersManager.Instance.GetBoosterCount(sellingBooster.GetBoosterType()).ToString();

        decreaseAmountButton.onClick.AddListener(() =>
        {
            var currentBuyingAmount = buyingAmountInputFieldFilter.GetInputFieldIntValue();
            currentBuyingAmount -= 1;

            buyingAmountInputFieldFilter.SetIntValue(currentBuyingAmount);
        });

        buyingAmountInputFieldFilter.OnInputFieldChanged += BuyingAmountInputFieldFilter_OnInputFieldChanged;

        BoostersManager.Instance.OnBoosterCountChanged += BoostersManager_OnBoosterCountChanged;

        var boostersPrice = sellingBooster.GetBoosterCoinsCost() * buyingAmountInputFieldFilter.GetInputFieldIntValue();
        buyBoostersButton.interactable = CoinsManager.Instance.IsHasEnoughCoins(boostersPrice);

        CoinsManager.Instance.OnCoinsCountChanged += CoinsManager_OnCoinsCountChanged;
    }

    private void CoinsManager_OnCoinsCountChanged(object sender, EventArgs e)
    {
        var boostersPrice = sellingBooster.GetBoosterCoinsCost() * buyingAmountInputFieldFilter.GetInputFieldIntValue();
        boosterPriceText.text = boostersPrice.ToString();

        buyBoostersButton.interactable = CoinsManager.Instance.IsHasEnoughCoins(boostersPrice);
    }

    private void BoostersManager_OnBoosterCountChanged(object sender, EventArgs e)
    {
        boosterOwnedCount.text = BoostersManager.Instance.GetBoosterCount(sellingBooster.GetBoosterType()).ToString();
    }

    private void BuyingAmountInputFieldFilter_OnInputFieldChanged(object sender, EventArgs e)
    {
        var boostersPrice = sellingBooster.GetBoosterCoinsCost() * buyingAmountInputFieldFilter.GetInputFieldIntValue();
        boosterPriceText.text = boostersPrice.ToString();

        buyBoostersButton.interactable = CoinsManager.Instance.IsHasEnoughCoins(boostersPrice);
    }

    #endregion
}