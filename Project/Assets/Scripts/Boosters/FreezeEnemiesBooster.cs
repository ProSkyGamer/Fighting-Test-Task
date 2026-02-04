#region

using UnityEngine;

#endregion

public class FreezeEnemiesBooster : BaseBoosterSingle
{
    #region Variables & References

    [SerializeField] private EntitiesCampSingle.CampTeam activatingCampTeam = EntitiesCampSingle.CampTeam.Player;

    #endregion

    #region Initialization

    protected override void OnBoosterActivation()
    {
        GameManager.Instance.FreezeEntities(activatingCampTeam);

        base.OnBoosterActivation();
    }

    protected override void OnBoosterDeactivation()
    {
        GameManager.Instance.UnfreezeEntities(activatingCampTeam);

        base.OnBoosterDeactivation();
    }

    #endregion

    #region Get

    public override BoosterType GetBoosterType()
    {
        boosterType = BoosterType.Freeze;
        return base.GetBoosterType();
    }

    #endregion
}