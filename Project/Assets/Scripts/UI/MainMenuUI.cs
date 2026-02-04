#region

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class MainMenuUI : MonoBehaviour
{
    #region Events

    public static event EventHandler OnRandomiseCampsButtonClicked;
    public static event EventHandler OnStartGameButtonClicked;

    #endregion

    #region Veriables & Refernces

    [SerializeField] private Button randomiseCampsButton;
    [SerializeField] private Button startGameButton;

    [SerializeField] private TextMeshProUGUI currentCoinsCount;
    [SerializeField] private BoosterSingleShopItemUI boosterSingleShopItemPrefab;
    [SerializeField] private Transform boostersShopItemsContainer;
    [SerializeField] private AllBoostersSO allBoostersSO;

    #endregion

    #region Initialization

    private void Awake()
    {
        randomiseCampsButton.onClick.AddListener(() =>
        {
            OnRandomiseCampsButtonClicked?.Invoke(this, EventArgs.Empty);

            startGameButton.interactable = GameManager.Instance.IsAllCampsFull();
        });

        startGameButton.onClick.AddListener(() =>
        {
            if (!GameManager.Instance.IsAllCampsFull()) return;

            startGameButton.interactable = false;

            OnStartGameButtonClicked?.Invoke(this, EventArgs.Empty);
        });

        startGameButton.interactable = false;

        boosterSingleShopItemPrefab.gameObject.SetActive(false);
    }

    private void Start()
    {
        GameStageManager.Instance.OnGameStageChanged += GameStageManager_OnGameStageChanged;
        CoinsManager.Instance.OnCoinsCountChanged += CoinsManager_OnCoinsCountChanged;

        foreach (var boosterSingle in allBoostersSO.allBoostersSingle)
        {
            var newBoosterSellingItemSingle = Instantiate(boosterSingleShopItemPrefab, boostersShopItemsContainer);
            newBoosterSellingItemSingle.gameObject.SetActive(true);
            newBoosterSellingItemSingle.Initialize(boosterSingle);
        }
    }

    private void CoinsManager_OnCoinsCountChanged(object sender, EventArgs e)
    {
        UpdateCoinsCount();
    }

    private void GameStageManager_OnGameStageChanged(object sender, EventArgs e)
    {
        if (GameStageManager.Instance.IsWaitingForTeamsToSpawn())
            Show();
        else
            Hide();
    }

    #endregion

    #region Visuals

    private void Show()
    {
        gameObject.SetActive(true);

        UpdateCoinsCount();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateCoinsCount()
    {
        currentCoinsCount.text = CoinsManager.Instance.GetCurrentCoins().ToString();
    }

    #endregion
}