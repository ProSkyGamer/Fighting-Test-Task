#region

using System;
using UnityEngine;

#endregion

public class GameStageManager : MonoBehaviour
{
    public static GameStageManager Instance { get; private set; }

    #region Enums

    public enum GameStage
    {
        WaitingForTeamSpawn,
        Fighting
    }

    #endregion

    #region Events

    public event EventHandler OnGameStageChanged;

    #endregion

    #region Variables & References

    [SerializeField] private GameStage startingGameStage = GameStage.WaitingForTeamSpawn;
    private GameStage currentGameStage;
    private GameStage previousGameStage;

    private bool isFirstUpdate = true;

    #endregion

    #region Initialization

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        MainMenuUI.OnStartGameButtonClicked += MainMenuUI_OnStartGameButtonClicked;
        GameManager.Instance.OnGameEnded += GameManager_OnGameEnded;
    }

    private void GameManager_OnGameEnded(object sender, EventArgs e)
    {
        ChangeGameStage(GameStage.WaitingForTeamSpawn);
    }

    private void MainMenuUI_OnStartGameButtonClicked(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsAllCampsFull()) return;

        ChangeGameStage(GameStage.Fighting);
    }

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            ChangeGameStage(startingGameStage);
        }
    }

    #endregion

    #region Game Stage

    private void ChangeGameStage(GameStage newGameStage)
    {
        currentGameStage = newGameStage;

        OnGameStageChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Get

    public bool IsWaitingForTeamsToSpawn()
    {
        return currentGameStage is GameStage.WaitingForTeamSpawn;
    }

    public bool IsFighting()
    {
        return currentGameStage is GameStage.Fighting;
    }

    #endregion

    private void OnDestroy()
    {
        MainMenuUI.OnStartGameButtonClicked -= MainMenuUI_OnStartGameButtonClicked;
    }
}