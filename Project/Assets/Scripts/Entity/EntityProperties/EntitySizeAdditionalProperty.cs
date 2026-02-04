#region

using UnityEngine;

#endregion

public class EntitySizeAdditionalProperty : EntityBaseAdditionalProperty
{
    #region Enums

    public enum EntitySizeTypes
    {
        Small,
        Big
    }

    #endregion

    #region Variables & References

    [SerializeField] private EntitySizeTypes entitySizeType;
    [SerializeField] private Vector3 entitySizeVector;
    [SerializeField] private Vector3 entityAdditionalOffset;

    #endregion

    #region Initialization

    public override void Initialize(Transform newEntityVisuals)
    {
        base.Initialize(newEntityVisuals);

        entityVisuals.localScale = entitySizeVector;
        entityVisuals.position += entityAdditionalOffset;
    }

    #endregion

    #region Get

    public override EntityAdditionalPropertyType GetEntityAdditionalPropertyType()
    {
        additionalPropertyType = EntityAdditionalPropertyType.Size;

        return base.GetEntityAdditionalPropertyType();
    }

    public EntitySizeTypes GetEntitySizeType()
    {
        return entitySizeType;
    }

    #endregion
}