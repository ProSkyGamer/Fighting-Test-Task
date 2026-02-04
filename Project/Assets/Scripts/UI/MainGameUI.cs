#region

using System;
using TMPro;
using UnityEngine;

#endregion

public class MainGameUI : MonoBehaviour
{
    #region Variables & References

    [SerializeField] private string teamEntitiesCountBaseStringFormat = "{0}: {1}";
    [SerializeField] private TextMeshProUGUI teamEntitiesCountTextPrefab;
    [SerializeField] private Transform teamEntitiesCountTextContainer;
    [SerializeField] private TextMeshProUGUI currentCoinsCount;

    [SerializeField] private AllBoostersSO allBoostersSO;
    [SerializeField] private BoosterSingleUI boosterSinglePrefab;
    [SerializeField] private Transform allBoostersContainer;

    #endregion

    #region Initialization

    private void Awake()
    {
        teamEntitiesCountTextPrefab.gameObject.SetActive(false);

        boosterSinglePrefab.gameObject.SetActive(false);
        teamEntitiesCountTextPrefab.gameObject.SetActive(false);
    }

    public void Start()
    {
        GameStageManager.Instance.OnGameStageChanged += GameStageManager_OnGameStageChanged;
        GameManager.Instance.OnEntityCountChanged += GameManager_OnEntityCountChanged;
        CoinsManager.Instance.OnCoinsCountChanged += CoinsManager_OnCoinsCountChanged;

        foreach (var boosterSingle in allBoostersSO.allBoostersSingle)
        {
            var newBoosterSingle = Instantiate(boosterSinglePrefab, allBoostersContainer);
            newBoosterSingle.gameObject.SetActive(true);
            newBoosterSingle.Initialize(boosterSingle);
        }
    }

    private void CoinsManager_OnCoinsCountChanged(object sender, EventArgs e)
    {
        UpdateCoinsCount();
    }

    private void GameManager_OnEntityCountChanged(object sender, EventArgs e)
    {
        UpdateEntityCountVisuals();
    }

    private void GameStageManager_OnGameStageChanged(object sender, EventArgs e)
    {
        if (GameStageManager.Instance.IsFighting())
            Show();
        else
            Hide();
    }

    #endregion

    #region Visuals

    private void Show()
    {
        gameObject.SetActive(true);

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        UpdateCoinsCount();
        UpdateEntityCountVisuals();
    }

    private void UpdateCoinsCount()
    {
        currentCoinsCount.text = CoinsManager.Instance.GetCurrentCoins().ToString();
    }

    private void UpdateEntityCountVisuals()
    {
        foreach (var toDelete in teamEntitiesCountTextContainer.GetComponentsInChildren<Transform>())
        {
            if (toDelete == teamEntitiesCountTextContainer || toDelete == teamEntitiesCountTextPrefab.transform) continue;

            Destroy(toDelete.gameObject);
        }

        var allCampEntityTeams = GameManager.Instance.GetAllCampEntities();
        foreach (var campEntityTeam in allCampEntityTeams)
        {
            var campEntityTeamCount = GameManager.Instance.GetCampTeamEntitiesCount(campEntityTeam);

            var newCampEntityTeamCountText = Instantiate(teamEntitiesCountTextPrefab, teamEntitiesCountTextContainer);
            newCampEntityTeamCountText.gameObject.SetActive(true);
            var newCampEntityCountTextString = string.Format(teamEntitiesCountBaseStringFormat, campEntityTeam, campEntityTeamCount);
            newCampEntityTeamCountText.text = newCampEntityCountTextString;
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion
}