#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#endregion

[RequireComponent(typeof(NavMeshAgent))]
public class EntitySingle : MonoBehaviour
{
    #region Events

    public event EventHandler OnEntityDeath;
    public event EventHandler OnEnemyTagetDefeated;

    #endregion

    #region Variables & References

    [SerializeField] private float baseEntityHP = 100f;
    [SerializeField] private float baseEntityAtk = 10f;
    [SerializeField] private float baseEntitySpeed = 10f;
    [SerializeField] private float baseEntityAtkSpeed = 1f;
    [SerializeField] private EntitySingleTeamUI entityTeamUI;
    [SerializeField] private Transform entityVisuals;
    [SerializeField] private Transform freezeVisuals;

    private bool isEntityFrozen;

    private float currentEntityHP;
    private float currentEntityMaxHP;
    private float currentEntityAtk;
    private float currentEntitySpeed;
    private float currentEntityAtkSpeed;
    private float currentEntityAttackInterval;
    private float currentEntityAttackIntervalTimer;

    private readonly List<EntityBaseAdditionalProperty> allEntityCurrentAdditionalProperties = new();

    private NavMeshAgent navMeshAgent;

    [SerializeField] private float enemyFightingDistance = 1.5f;

    private EntitySingle currentEnemyTarget;
    private bool isGoingToEnemyTaget;

    [SerializeField] private float updatingEnemyPositionInterval = .25f;
    private float updatingEnemyPositionTimer;
    private EntitiesCampSingle.CampTeam entityCampTeam;

    #endregion

    #region Initialize

    public void Initialize(List<EntityBaseAdditionalProperty> allEntityAdditionalProperties, EntitiesCampSingle.CampTeam newCampTeam)
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        entityCampTeam = newCampTeam;

        currentEntityMaxHP = baseEntityHP;
        currentEntityAtk = baseEntityAtk;
        currentEntitySpeed = baseEntitySpeed;
        currentEntityAtkSpeed = baseEntityAtkSpeed;

        foreach (var entityAdditionalProperty in allEntityAdditionalProperties)
        {
            AddEntityProperty(entityAdditionalProperty);
        }

        currentEntityAttackInterval = currentEntityAtkSpeed;
        currentEntityHP = currentEntityMaxHP;

        entityTeamUI.Initialize(entityCampTeam);

        freezeVisuals.gameObject.SetActive(false);
    }

    #endregion

    #region Update

    private void Update()
    {
        if (!GameStageManager.Instance.IsFighting()) return;
        if (currentEnemyTarget == null) return;
        if (isEntityFrozen) return;

        if (isGoingToEnemyTaget)
        {
            updatingEnemyPositionTimer -= Time.deltaTime;
            if (updatingEnemyPositionTimer <= 0f)
                UpdateEnemyPosition();
        }
        else
        {
            UpdateEnemyPosition();

            currentEntityAttackIntervalTimer -= Time.deltaTime;

            if (currentEntityAttackIntervalTimer <= 0f)
            {
                AttackEnemy();
                currentEntityAttackIntervalTimer = currentEntityAttackInterval;
            }
        }
    }

    private void UpdateEnemyPosition()
    {
        if (currentEnemyTarget == null) return;
        if (isEntityFrozen) return;

        navMeshAgent.destination = currentEnemyTarget.transform.position;
        updatingEnemyPositionTimer = updatingEnemyPositionInterval;

        isGoingToEnemyTaget = (currentEnemyTarget.transform.position - transform.position).magnitude > enemyFightingDistance;
        navMeshAgent.speed = !isGoingToEnemyTaget ? 0f : currentEntitySpeed;
    }

    #endregion

    #region Fighting

    public void FreezeEntity()
    {
        isEntityFrozen = true;
        navMeshAgent.speed = 0f;
        freezeVisuals.gameObject.SetActive(true);
    }

    public void UnfreezeEntity()
    {
        isEntityFrozen = false;
        navMeshAgent.speed = !isGoingToEnemyTaget ? 0f : currentEntitySpeed;
        freezeVisuals.gameObject.SetActive(false);
    }

    public void SetEnemyTarget(EntitySingle newEnemyTarget)
    {
        if (newEnemyTarget.GetEntityCampTeam() == entityCampTeam) return;

        currentEnemyTarget = newEnemyTarget;
        navMeshAgent.speed = currentEntitySpeed;
        currentEntityAttackIntervalTimer = currentEntityAttackInterval;
        currentEnemyTarget.OnEntityDeath += CurrentEnemyTarget_OnEntityDeath;

        UpdateEnemyPosition();
    }

    private void CurrentEnemyTarget_OnEntityDeath(object sender, EventArgs e)
    {
        currentEnemyTarget = null;
        navMeshAgent.speed = 0f;
        currentEntityAttackIntervalTimer = currentEntityAttackInterval;

        OnEnemyTagetDefeated?.Invoke(this, EventArgs.Empty);
    }

    private void AttackEnemy()
    {
        currentEnemyTarget.TakeDamage(currentEntityAtk);
    }

    #endregion

    #region Health

    public void TakeDamage(float damage)
    {
        currentEntityHP -= damage;

        if (currentEntityHP <= 0f)
        {
            OnEntityDeath?.Invoke(this, EventArgs.Empty);

            Destroy(gameObject);
        }
    }

    #endregion

    #region Entity Properties

    private void AddEntityProperty(EntityBaseAdditionalProperty entityAdditionalProperty)
    {
        foreach (var entityBaseAdditionalProperty in allEntityCurrentAdditionalProperties)
        {
            if (entityBaseAdditionalProperty.GetEntityAdditionalPropertyType() == entityAdditionalProperty.GetEntityAdditionalPropertyType()) return;
        }

        var newEntityAdditionalProperty = Instantiate(entityAdditionalProperty, transform);
        newEntityAdditionalProperty.Initialize(entityVisuals);
        allEntityCurrentAdditionalProperties.Add(newEntityAdditionalProperty);

        currentEntityMaxHP += newEntityAdditionalProperty.GetEntityAdditionalHP();
        currentEntityAtk += newEntityAdditionalProperty.GetEntityAdditionalAtk();
        currentEntitySpeed += newEntityAdditionalProperty.GetEntityAdditionalSpeed();
        currentEntityAtkSpeed += newEntityAdditionalProperty.GetEntityAdditionalAtkSpeed();
    }

    #endregion

    #region Get

    public EntitiesCampSingle.CampTeam GetEntityCampTeam()
    {
        return entityCampTeam;
    }

    #endregion

    private void OnDestroy()
    {
        if (currentEnemyTarget != null)
            currentEnemyTarget.OnEntityDeath -= CurrentEnemyTarget_OnEntityDeath;
    }
}