#region

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class EntitiesCampSingle : MonoBehaviour
{
    #region Enums

    public enum CampTeam
    {
        Player,
        Enemy
    }

    #endregion

    #region Events

    public event EventHandler OnAllEntitiesDied;
    public event EventHandler<OnEntityTargetDefeatedEventArgs> OnEntityTargetDefeated;

    public class OnEntityTargetDefeatedEventArgs : EventArgs
    {
        public EntitySingle entitySingle;
    }

    #endregion

    #region Variables & References

    [SerializeField] private CampTeam campTeam;
    [SerializeField] private int maxEntitiesCount = 20;
    [SerializeField] private EntitySingle entitySinglePrefab;
    [SerializeField] private Transform allEntitiesContainer;
    [SerializeField] private List<Transform> allEntitySpawningPositions;
    [SerializeField] private AllEntityAdditionalPropertiesSO allEntityAdditionalProperties;

    private readonly Dictionary<EntityBaseAdditionalProperty.EntityAdditionalPropertyType, List<EntityBaseAdditionalProperty>>
        allEntityAdditionalPropertiesDictionary = new();

    [SerializeField] private List<EntityBaseAdditionalProperty.EntityAdditionalPropertyType> allEntityRequiredProperties;
    private readonly List<EntitySingle> allSpawnedEntities = new();

    private int deadEntityTargets;

    #endregion

    #region Initialization

    private void Awake()
    {
        foreach (var entityBaseAdditionalProperty in allEntityAdditionalProperties.allEntityAdditionalProperties)
        {
            if (!allEntityAdditionalPropertiesDictionary.ContainsKey(entityBaseAdditionalProperty.GetEntityAdditionalPropertyType()))
                allEntityAdditionalPropertiesDictionary.Add(entityBaseAdditionalProperty.GetEntityAdditionalPropertyType(),
                    new List<EntityBaseAdditionalProperty>());

            allEntityAdditionalPropertiesDictionary[entityBaseAdditionalProperty.GetEntityAdditionalPropertyType()].Add(entityBaseAdditionalProperty);
        }
    }

    #endregion

    #region Spawn Entities

    public void SpawnAllEntities()
    {
        if (allEntitySpawningPositions.Count >= maxEntitiesCount)
            maxEntitiesCount = allEntitySpawningPositions.Count;

        foreach (var entitySpawningPosition in allEntitySpawningPositions)
        {
            var newSpawnedEntity = Instantiate(entitySinglePrefab, entitySpawningPosition.position, entitySpawningPosition.rotation,
                allEntitiesContainer);
            var newEnemyRequiredRandomProperties = GetRandomEntityRequiredAdditionalProperties();
            newSpawnedEntity.Initialize(newEnemyRequiredRandomProperties, campTeam);
            allSpawnedEntities.Add(newSpawnedEntity);

            newSpawnedEntity.OnEntityDeath += NewSpawnedEntity_OnEntityDeath;
            newSpawnedEntity.OnEnemyTagetDefeated += NewSpawnedEntity_OnEnemyTagetDefeated;
        }
    }

    private void NewSpawnedEntity_OnEnemyTagetDefeated(object sender, EventArgs e)
    {
        var entitySingle = sender as EntitySingle;
        if (entitySingle == null) return;

        OnEntityTargetDefeated?.Invoke(this, new OnEntityTargetDefeatedEventArgs
        {
            entitySingle = entitySingle
        });
    }

    private void NewSpawnedEntity_OnEntityDeath(object sender, EventArgs e)
    {
        var diedEntity = sender as EntitySingle;
        if (diedEntity == null) return;

        allSpawnedEntities.Remove(diedEntity);

        deadEntityTargets += 1;
        if (!GameStageManager.Instance.IsFighting()) return;
        if (allSpawnedEntities.Count <= 0)
            OnAllEntitiesDied?.Invoke(this, EventArgs.Empty);
    }

    public void DestroyAllEntities()
    {
        foreach (var entitySingle in allSpawnedEntities)
        {
            Destroy(entitySingle.gameObject);
        }

        deadEntityTargets = 0;

        allSpawnedEntities.Clear();
    }

    #endregion

    #region Boosters

    public void FreezeEntities(List<EntitySingle> freezingEntities)
    {
        foreach (var freezingEntity in freezingEntities)
        {
            freezingEntity.FreezeEntity();
        }
    }

    public void UnfreezeEntities(List<EntitySingle> freezingEntities)
    {
        foreach (var freezingEntity in freezingEntities)
        {
            freezingEntity.UnfreezeEntity();
        }
    }

    #endregion

    #region Get

    private List<EntityBaseAdditionalProperty> GetRandomEntityRequiredAdditionalProperties()
    {
        var allEntityRequiredRandomProperties = new List<EntityBaseAdditionalProperty>();

        for (var i = 0; i < allEntityRequiredProperties.Count; i++)
        {
            var entityAdditionalPropertyType = allEntityRequiredProperties[i];
            if (!allEntityAdditionalPropertiesDictionary.TryGetValue(entityAdditionalPropertyType, out var allAvailablePropertyTypeVariations))
            {
                Debug.Log($"{entityAdditionalPropertyType}");
                allEntityRequiredProperties.RemoveAt(i);
                i--;
                continue;
            }

            var availablePropertyTypesVariationsCount = allAvailablePropertyTypeVariations.Count;
            var randomPropertyTypeVariationIndex = Random.Range(0, availablePropertyTypesVariationsCount);

            allEntityRequiredRandomProperties.Add(allAvailablePropertyTypeVariations[randomPropertyTypeVariationIndex]);
        }

        return allEntityRequiredRandomProperties;
    }

    public bool IsCampFull()
    {
        return allSpawnedEntities.Count >= maxEntitiesCount;
    }

    public bool IsCampEmpty()
    {
        return allSpawnedEntities.Count == 0;
    }

    public CampTeam GetCampTeam()
    {
        return campTeam;
    }

    public List<EntitySingle> GetAllCampEntities()
    {
        return allSpawnedEntities;
    }

    public int GetDeadEntityTargets()
    {
        return deadEntityTargets;
    }

    #endregion
}