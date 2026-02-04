#region

using System;
using UnityEngine;

#endregion

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance { get; private set; }

    #region Events

    public event EventHandler OnCoinsCountChanged;

    #endregion

    #region Variables & References

    private const string PLAYER_COINS_PLAYER_PREFS = "PlayerCountPlayerPrefs";

    #endregion

    #region Intialization

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    #endregion

    #region Coins

    public void AddCoins(int deltaCoins)
    {
        PlayerPrefs.SetInt(PLAYER_COINS_PLAYER_PREFS, PlayerPrefs.GetInt(PLAYER_COINS_PLAYER_PREFS) + deltaCoins);

        OnCoinsCountChanged?.Invoke(this, EventArgs.Empty);
    }


    public void SpendCoins(int deltaCoins)
    {
        PlayerPrefs.SetInt(PLAYER_COINS_PLAYER_PREFS, PlayerPrefs.GetInt(PLAYER_COINS_PLAYER_PREFS) - deltaCoins);

        OnCoinsCountChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Get

    public int GetCurrentCoins()
    {
        return PlayerPrefs.GetInt(PLAYER_COINS_PLAYER_PREFS, 0);
    }

    public bool IsHasEnoughCoins(int coins)
    {
        return coins <= GetCurrentCoins();
    }

    #endregion
}