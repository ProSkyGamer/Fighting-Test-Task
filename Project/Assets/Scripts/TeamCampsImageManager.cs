#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class TeamCampsImageManager : MonoBehaviour
{
    public static TeamCampsImageManager Instance { get; private set; }

    #region Created Classes

    [Serializable]
    public class CampTeamImage
    {
        public EntitiesCampSingle.CampTeam campTeam;
        public Sprite campTeamSprite;
    }

    #endregion

    #region Variables & Refernces

    [SerializeField] private List<CampTeamImage> campTeamImages;
    private readonly Dictionary<EntitiesCampSingle.CampTeam, Sprite> allCampTeamImagesDictionary = new();

    #endregion

    #region Initialization

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        foreach (var campTeamImage in campTeamImages)
        {
            if (allCampTeamImagesDictionary.ContainsKey(campTeamImage.campTeam)) continue;

            allCampTeamImagesDictionary.Add(campTeamImage.campTeam, campTeamImage.campTeamSprite);
        }
    }

    #endregion

    #region Get

    public Sprite GetCampTeamSprite(EntitiesCampSingle.CampTeam campTeam)
    {
        return allCampTeamImagesDictionary[campTeam];
    }

    #endregion
}