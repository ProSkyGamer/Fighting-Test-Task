#region

using System;
using UnityEngine;

#endregion

public class BaseBoosterSingle : MonoBehaviour
{
    #region Enum

    public enum BoosterType
    {
        Freeze
    }

    #endregion

    #region Events

    public event EventHandler OnBoosterActivated;
    public event EventHandler OnBoosterDeactivated;

    #endregion

    #region Variables & References

    protected BoosterType boosterType;
    [SerializeField] private string boosterName;
    [SerializeField] private Sprite boosterIconSprite;
    [SerializeField] private int boosterCoinsCost;
    [SerializeField] private float boosterDuration;
    private float boosterDurationTimer;
    [SerializeField] private float boosterCooldown;
    private bool isBoosterActive;

    #endregion

    #region Initialization

    public virtual void ActivateBooster()
    {
        boosterDurationTimer = boosterDuration;
        isBoosterActive = true;
        OnBoosterActivation();
    }

    protected virtual void OnBoosterActivation()
    {
        OnBoosterActivated?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnBoosterDeactivation()
    {
        OnBoosterDeactivated?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    #endregion

    #region Update

    private void Update()
    {
        if (!isBoosterActive) return;

        boosterDurationTimer -= Time.deltaTime;

        if (boosterDurationTimer <= 0f)
            OnBoosterDeactivation();
    }

    #endregion

    #region Get

    public float GetBoosterCooldown()
    {
        return boosterCooldown;
    }

    public string GetBoosterName()
    {
        return boosterName;
    }

    public Sprite GetBoosterIcon()
    {
        return boosterIconSprite;
    }

    public int GetBoosterCoinsCost()
    {
        return boosterCoinsCost;
    }

    public virtual BoosterType GetBoosterType()
    {
        return boosterType;
    }

    public float GetBoosterDuration()
    {
        return boosterDuration;
    }

    public float GetBoosterRemainingDuration()
    {
        return boosterDurationTimer;
    }

    #endregion
}