#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class EntitySingleTeamUI : MonoBehaviour
{
    #region Variables & References

    [SerializeField] private Image entityTeamImage;
    private Camera mainCamera;

    #endregion

    #region Initialize

    private void Awake()
    {
        entityTeamImage.gameObject.SetActive(false);
    }

    public void Initialize(EntitiesCampSingle.CampTeam entityCampTeam)
    {
        entityTeamImage.gameObject.SetActive(true);

        entityTeamImage.sprite = TeamCampsImageManager.Instance.GetCampTeamSprite(entityCampTeam);

        mainCamera = Camera.main;
    }

    #endregion

    #region Update

    private void Update()
    {
        if (mainCamera == null) return;

        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    #endregion
}