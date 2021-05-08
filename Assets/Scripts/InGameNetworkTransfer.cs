using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public enum GameState : int
{
    None = 0,
    Ready,
    Running,
    End
}

[System.Serializable]
public class InGameNetworkTransfer : NetworkBehaviour
{
    private const float gameReadyInterval = 3.0f;

    private readonly NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, GameState.None);
    private readonly NetworkVariable<float> countingStartTime = new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    public GameState CurrentGameState => currentGameState.Value;

    [ClientRpc]
    public void GameStartClientRpc()
    {
        Debug.Log("RpcGameStart");
        countingStartTime.Value = Time.time;
        currentGameState.Value = GameState.Ready;

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.EnemyManager.Prepare();
        inGameSceneMain.BulletManager.Prepare();
    }

    private void Update()
    {
        float currentTime = Time.time;
        if (CurrentGameState == GameState.Ready)
        {
            if (currentTime - countingStartTime.Value > gameReadyInterval)
            {
                SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().SquadronManager.StartGame();
                currentGameState.Value = GameState.Running;
            }
        }
    }
}
