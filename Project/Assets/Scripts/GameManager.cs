#region

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region Events

    public event EventHandler OnGameEnded;
    public event EventHandler OnEntityCountChanged;

    #endregion

    #region Variables & References

    [SerializeField] private List<EntitiesCampSingle> allEntitiesCamps;
    [SerializeField] private int coinsPerDefeatedEnemyTarget;
    private bool isPlayerWon;

    #endregion

    #region Initialization

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        MainMenuUI.OnRandomiseCampsButtonClicked += MainMenuUI_OnRandomiseCampsButtonClicked;

        foreach (var entitiesCampSingle in allEntitiesCamps)
        {
            entitiesCampSingle.OnAllEntitiesDied += EntitiesCampSingle_OnAllEntitiesDied;
            entitiesCampSingle.OnEntityTargetDefeated += EntitiesCampSingle_OnEntityTargetDefeated;
        }

        GameStageManager.Instance.OnGameStageChanged += GameStageManager_OnGameStageChanged;
    }

    private void EntitiesCampSingle_OnEntityTargetDefeated(object sender, EntitiesCampSingle.OnEntityTargetDefeatedEventArgs e)
    {
        var allAvailableEnemies = new List<EntitySingle>();
        var campTeam = e.entitySingle.GetEntityCampTeam();
        foreach (var entityCampSingle in allEntitiesCamps)
        {
            if (entityCampSingle.GetCampTeam() == campTeam) continue;

            allAvailableEnemies.AddRange(entityCampSingle.GetAllCampEntities());
        }

        OnEntityCountChanged?.Invoke(this, EventArgs.Empty);

        if (allAvailableEnemies.Count <= 0) return;

        var randomEnemyTargetIndex = Random.Range(0, allAvailableEnemies.Count);
        var randomEnemyTarget = allAvailableEnemies[randomEnemyTargetIndex];

        e.entitySingle.SetEnemyTarget(randomEnemyTarget);
    }

    private void GameStageManager_OnGameStageChanged(object sender, EventArgs e)
    {
        if (GameStageManager.Instance.IsFighting())
        {
            isPlayerWon = false;

            var allFightingEntities = new Dictionary<EntitiesCampSingle.CampTeam, List<EntitySingle>>();

            foreach (var campSingle in allEntitiesCamps)
            {
                if (!allFightingEntities.ContainsKey(campSingle.GetCampTeam()))
                    allFightingEntities.Add(campSingle.GetCampTeam(), new List<EntitySingle>());

                allFightingEntities[campSingle.GetCampTeam()].AddRange(campSingle.GetAllCampEntities());
            }

            foreach (var entitiesCampSingle in allEntitiesCamps)
            {
                var campTeam = entitiesCampSingle.GetCampTeam();
                var allAvailableTeamsString = Enum.GetNames(typeof(EntitiesCampSingle.CampTeam));
                var allEnemyTeams = new List<EntitiesCampSingle.CampTeam>();
                for (var i = 0; i < allAvailableTeamsString.Length; i++)
                    allEnemyTeams.Add((EntitiesCampSingle.CampTeam)i);

                allEnemyTeams.Remove(campTeam);
                var allAvailableEnemies = new List<EntitySingle>();
                foreach (var enemyTeam in allEnemyTeams)
                {
                    allAvailableEnemies.AddRange(allFightingEntities[enemyTeam]);
                }

                if (allAvailableEnemies.Count <= 0) return;

                var allFightingTeamEntities = new List<EntitySingle>();
                allFightingTeamEntities.AddRange(entitiesCampSingle.GetAllCampEntities());

                foreach (var entitySingle in allFightingTeamEntities)
                {
                    var randomEnemyTargetIndex = Random.Range(0, allAvailableEnemies.Count);
                    var randomEnemyTarget = allAvailableEnemies[randomEnemyTargetIndex];

                    entitySingle.SetEnemyTarget(randomEnemyTarget);
                    randomEnemyTarget.SetEnemyTarget(entitySingle);

                    allAvailableEnemies.Remove(randomEnemyTarget);

                    allFightingEntities[entitySingle.GetEntityCampTeam()].Remove(entitySingle);
                    allFightingEntities[randomEnemyTarget.GetEntityCampTeam()].Remove(randomEnemyTarget);
                }
            }
        }
    }

    private void EntitiesCampSingle_OnAllEntitiesDied(object sender, EventArgs e)
    {
        var remainingTeamTypes = new List<EntitiesCampSingle.CampTeam>();

        foreach (var entityCampSingle in allEntitiesCamps)
        {
            if (entityCampSingle.IsCampEmpty()) continue;

            remainingTeamTypes.Add(entityCampSingle.GetCampTeam());

            if (remainingTeamTypes.Count > 1)
                return;
        }

        isPlayerWon = remainingTeamTypes[0] == EntitiesCampSingle.CampTeam.Player;
        OnGameEnded?.Invoke(this, EventArgs.Empty);

        var defeatedEnemyTargets = 0;
        foreach (var entitiesCampSingle in allEntitiesCamps)
        {
            if (entitiesCampSingle.GetCampTeam() == EntitiesCampSingle.CampTeam.Player) continue;

            defeatedEnemyTargets += entitiesCampSingle.GetDeadEntityTargets();
        }

        var gainedCoins = defeatedEnemyTargets * coinsPerDefeatedEnemyTarget;
        CoinsManager.Instance.AddCoins(gainedCoins);
    }

    private void MainMenuUI_OnRandomiseCampsButtonClicked(object sender, EventArgs e)
    {
        foreach (var campSingle in allEntitiesCamps)
        {
            campSingle.DestroyAllEntities();
            campSingle.SpawnAllEntities();
        }
    }

    #endregion

    #region Boosters

    public void FreezeEntities(EntitiesCampSingle.CampTeam activatingCampTeam)
    {
        foreach (var entitiesCamp in allEntitiesCamps)
        {
            if (entitiesCamp.GetCampTeam() == activatingCampTeam) continue;

            entitiesCamp.FreezeEntities(entitiesCamp.GetAllCampEntities());
        }
    }

    public void UnfreezeEntities(EntitiesCampSingle.CampTeam activatingCampTeam)
    {
        foreach (var entitiesCamp in allEntitiesCamps)
        {
            if (entitiesCamp.GetCampTeam() == activatingCampTeam) continue;

            entitiesCamp.UnfreezeEntities(entitiesCamp.GetAllCampEntities());
        }
    }

    #endregion

    #region Get

    public bool IsAllCampsFull()
    {
        foreach (var entitiesCampSingle in allEntitiesCamps)
        {
            if (!entitiesCampSingle.IsCampFull()) return false;
        }

        return true;
    }

    public int GetCampTeamEntitiesCount(EntitiesCampSingle.CampTeam campTeam)
    {
        var campTeamEntitiesCount = 0;

        foreach (var entitiesCampSingle in allEntitiesCamps)
        {
            if (entitiesCampSingle.GetCampTeam() != campTeam) continue;

            campTeamEntitiesCount += entitiesCampSingle.GetAllCampEntities().Count;
        }

        return campTeamEntitiesCount;
    }

    public List<EntitiesCampSingle.CampTeam> GetAllCampEntities()
    {
        var allCampEntities = new List<EntitiesCampSingle.CampTeam>();
        foreach (var entitiesCampSingle in allEntitiesCamps)
        {
            if (allCampEntities.Contains(entitiesCampSingle.GetCampTeam())) continue;

            allCampEntities.Add(entitiesCampSingle.GetCampTeam());
        }

        return allCampEntities;
    }

    public int GetDefeatedEnemiesCount()
    {
        var defeatedEnemyTargets = 0;
        foreach (var entitiesCampSingle in allEntitiesCamps)
        {
            if (entitiesCampSingle.GetCampTeam() == EntitiesCampSingle.CampTeam.Player) continue;

            defeatedEnemyTargets += entitiesCampSingle.GetDeadEntityTargets();
        }

        return defeatedEnemyTargets;
    }

    public int GetGainedCoinsCount()
    {
        var defeatedEnemyTargets = GetDefeatedEnemiesCount();

        var gainedCoins = defeatedEnemyTargets * coinsPerDefeatedEnemyTarget;

        return gainedCoins;
    }

    public bool IsPlayerWon()
    {
        return isPlayerWon;
    }

    #endregion
}