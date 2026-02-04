#region

using UnityEngine;

#endregion

public class EntityColorAdditionalProperty : EntityBaseAdditionalProperty
{
    #region Enums

    public enum EntityColorTypes
    {
        Blue,
        Green,
        Red
    }

    #endregion

    #region Variables & References

    [SerializeField] private EntityColorTypes entityColorType;
    [SerializeField] private Material entityMeshRendererColorType;

    private MeshRenderer meshRenderer;

    #endregion

    #region Initialization

    public override void Initialize(Transform newEntityVisuals)
    {
        base.Initialize(newEntityVisuals);

        meshRenderer = entityVisuals.GetComponent<MeshRenderer>();
        meshRenderer.material = entityMeshRendererColorType;
    }

    #endregion

    #region Get

    public override EntityAdditionalPropertyType GetEntityAdditionalPropertyType()
    {
        additionalPropertyType = EntityAdditionalPropertyType.Color;

        return base.GetEntityAdditionalPropertyType();
    }

    public EntityColorTypes GetEntityColorType()
    {
        return entityColorType;
    }

    #endregion
}