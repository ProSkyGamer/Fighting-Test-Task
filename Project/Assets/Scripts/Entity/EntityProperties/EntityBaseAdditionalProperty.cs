#region

using UnityEngine;

#endregion

public class EntityBaseAdditionalProperty : MonoBehaviour
{
    #region Enum

    public enum EntityAdditionalPropertyType
    {
        Form,
        Size,
        Color
    }

    #endregion

    #region Variables & Refernces

    protected EntityAdditionalPropertyType additionalPropertyType;

    [SerializeField] private float entityAdditionalHP;
    [SerializeField] private float entityAdditionalAtk;
    [SerializeField] private float entityAdditionalSpeed;
    [SerializeField] private float entityAdditionalAtkSpeed;

    protected Transform entityVisuals;

    #endregion

    #region Initialize

    public virtual void Initialize(Transform newEntityVisuals)
    {
        entityVisuals = newEntityVisuals;
    }

    #endregion

    #region Get

    public virtual EntityAdditionalPropertyType GetEntityAdditionalPropertyType()
    {
        return additionalPropertyType;
    }

    public float GetEntityAdditionalHP()
    {
        return entityAdditionalHP;
    }

    public float GetEntityAdditionalAtk()
    {
        return entityAdditionalAtk;
    }

    public float GetEntityAdditionalSpeed()
    {
        return entityAdditionalSpeed;
    }

    public float GetEntityAdditionalAtkSpeed()
    {
        return entityAdditionalAtkSpeed;
    }

    #endregion
}