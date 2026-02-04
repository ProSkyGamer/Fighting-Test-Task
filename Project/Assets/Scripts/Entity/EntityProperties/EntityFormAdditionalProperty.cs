#region

using UnityEngine;

#endregion

public class EntityFormAdditionalProperty : EntityBaseAdditionalProperty
{
    #region Enums

    public enum EntityFormTypes
    {
        Cube,
        Sphere
    }

    #endregion

    #region Variables & References

    [SerializeField] private EntityFormTypes entityFormType;
    [SerializeField] private Mesh entityMeshFormType;

    private MeshFilter meshFilter;

    #endregion

    #region Initialization

    public override void Initialize(Transform newEntityVisuals)
    {
        base.Initialize(newEntityVisuals);

        meshFilter = entityVisuals.GetComponent<MeshFilter>();
        meshFilter.mesh = entityMeshFormType;
    }

    #endregion

    #region Get

    public override EntityAdditionalPropertyType GetEntityAdditionalPropertyType()
    {
        additionalPropertyType = EntityAdditionalPropertyType.Form;

        return base.GetEntityAdditionalPropertyType();
    }

    public EntityFormTypes GetEntityFormType()
    {
        return entityFormType;
    }

    #endregion
}