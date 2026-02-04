#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class BoostersManager : MonoBehaviour
{
    public static BoostersManager Instance { get; private set; }

    #region Events

    public event EventHandler OnBoosterCountChanged;
    public event EventHandler OnBoosterActivated;
    public event EventHandler OnBoosterDeactivated;

    #endregion

    #region Variables & References

    [SerializeField] private AllBoostersSO allAvailableBoosterPrefabs;
    [SerializeField] private Transform allBoostersContainer;
    private readonly List<BaseBoosterSingle> allActiveBoosters = new();
    private readonly Dictionary<BaseBoosterSingle.BoosterType, float> allBoostersCooldown = new();
    private const string BASE_BOOSTER_AVAILABLE_COUNT_PLAYER_PREFS = "BoosterAvailableCount_{0}";

    #endregion

    #region Initialization

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        foreach (var boosterSingle in allAvailableBoosterPrefabs.allBoostersSingle)
        {
            if (allBoostersCooldown.ContainsKey(boosterSingle.GetBoosterType())) continue;

            allBoostersCooldown.Add(boosterSingle.GetBoosterType(), boosterSingle.GetBoosterCooldown());
        }
    }

    #endregion

    #region Update

    private void Update()
    {
        foreach (var boosterSingle in allAvailableBoosterPrefabs.allBoostersSingle)
        {
            if (allBoostersCooldown[boosterSingle.GetBoosterType()] > 0)
                allBoostersCooldown[boosterSingle.GetBoosterType()] -= Time.deltaTime;
        }
    }

    #endregion

    #region Activate

    public void ActivateBooster(BaseBoosterSingle.BoosterType boosterType)
    {
        if (GetBoosterCount(boosterType) <= 0) return;
        if (allBoostersCooldown[boosterType] > 0) return;

        foreach (var activeBooster in allActiveBoosters)
        {
            if (activeBooster.GetBoosterType() == boosterType) return;
        }

        foreach (var boosterPrefab in allAvailableBoosterPrefabs.allBoostersSingle)
        {
            if (boosterPrefab.GetBoosterType() != boosterType) continue;

            var newActivatingBooster = Instantiate(boosterPrefab, allBoostersContainer);
            newActivatingBooster.OnBoosterDeactivated += NewActivatingBooster_OnBoosterDeactivated;
            allActiveBoosters.Add(newActivatingBooster);
            newActivatingBooster.ActivateBooster();
            PlayerPrefs.SetInt(GetBoosterAvailableCountPlayerPrefsAccessString(boosterType),
                PlayerPrefs.GetInt(GetBoosterAvailableCountPlayerPrefsAccessString(boosterType)) - 1);
            OnBoosterCountChanged?.Invoke(this, EventArgs.Empty);
            break;
        }
    }

    public void BuyBooster(BaseBoosterSingle.BoosterType boosterType, int buyingAmount)
    {
        foreach (var boosterPrefab in allAvailableBoosterPrefabs.allBoostersSingle)
        {
            if (boosterPrefab.GetBoosterType() != boosterType) continue;

            var boosterPrice = boosterPrefab.GetBoosterCoinsCost();
            var totalPrice = boosterPrice * buyingAmount;

            if (!CoinsManager.Instance.IsHasEnoughCoins(totalPrice)) return;

            PlayerPrefs.SetInt(GetBoosterAvailableCountPlayerPrefsAccessString(boosterType),
                PlayerPrefs.GetInt(GetBoosterAvailableCountPlayerPrefsAccessString(boosterType)) + buyingAmount);

            CoinsManager.Instance.SpendCoins(totalPrice);
            OnBoosterCountChanged?.Invoke(this, EventArgs.Empty);
            break;
        }
    }

    private void NewActivatingBooster_OnBoosterDeactivated(object sender, EventArgs e)
    {
        var deactivatedBooster = sender as BaseBoosterSingle;
        if (deactivatedBooster == null) return;

        allActiveBoosters.Remove(deactivatedBooster);
        allBoostersCooldown[deactivatedBooster.GetBoosterType()] = deactivatedBooster.GetBoosterCooldown();
    }

    #endregion

    #region Get

    public int GetBoosterCount(BaseBoosterSingle.BoosterType boosterType)
    {
        return PlayerPrefs.GetInt(GetBoosterAvailableCountPlayerPrefsAccessString(boosterType), 0);
    }

    private string GetBoosterAvailableCountPlayerPrefsAccessString(BaseBoosterSingle.BoosterType boosterType)
    {
        var boosterAvailableCountPlayerPrefsAccessString = string.Format(BASE_BOOSTER_AVAILABLE_COUNT_PLAYER_PREFS, boosterType);

        return boosterAvailableCountPlayerPrefsAccessString;
    }

    public bool IsBoosterActive(BaseBoosterSingle.BoosterType boosterType)
    {
        foreach (var activeBooster in allActiveBoosters)
        {
            if (activeBooster.GetBoosterType() != boosterType) continue;

            return true;
        }

        return false;
    }

    public float GetBoosterTimeTotal(BaseBoosterSingle.BoosterType boosterType)
    {
        foreach (var activeBooster in allActiveBoosters)
        {
            if (activeBooster.GetBoosterType() != boosterType) continue;

            return activeBooster.GetBoosterDuration();
        }

        foreach (var boosterPrefab in allAvailableBoosterPrefabs.allBoostersSingle)
        {
            if (boosterPrefab.GetBoosterType() != boosterType) continue;

            return boosterPrefab.GetBoosterCooldown();
        }

        return -1f;
    }

    public float GetBoosterTimeCurrent(BaseBoosterSingle.BoosterType boosterType)
    {
        foreach (var activeBooster in allActiveBoosters)
        {
            if (activeBooster.GetBoosterType() != boosterType) continue;

            return activeBooster.GetBoosterRemainingDuration();
        }

        return allBoostersCooldown[boosterType];
    }

    #endregion
}