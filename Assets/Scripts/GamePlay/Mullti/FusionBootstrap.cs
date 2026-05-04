using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class FusionBootstrap : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Session")]
    [SerializeField] private string sessionName = "Room_01";

    [Header("Player")]
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private const int MAX_PLAYERS = 2;

    private NetworkRunner runner;
    private Dictionary<PlayerRef, NetworkObject> playerObjects = new();

    public struct NetworkInputData : INetworkInput
    {
        public Vector2 move;
        public NetworkButtons buttons;
    }

    public enum InputButton
    {
        InteractPrimary = 0,
        InteractSecondary = 1,
        Dash = 2
    }

    // ========================= START =========================

    public void StartHost() => _ = StartGame(GameMode.Host);
    public void StartClient() => _ = StartGame(GameMode.Client);

    private async Task StartGame(GameMode mode)
    {
        if (runner != null) return;

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        var sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            SceneManager = sceneManager,
            PlayerCount = MAX_PLAYERS   // 2명 제한
        });

        if (result.Ok)
            Debug.Log($"[Fusion] StartGame OK - {mode}");
        else
            Debug.LogError($"[Fusion] StartGame FAILED - {result.ShutdownReason}");
    }

    // ========================= PLAYER JOIN =========================

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 입장: {player}");

        if (!runner.IsServer)
            return;

        // ❗ 2명 초과 차단
        if (runner.ActivePlayers.Count() > MAX_PLAYERS)
        {
            Debug.LogWarning("룸이 가득 찼습니다. 입장 거부");
            runner.Disconnect(player);
            return;
        }

        Vector3 spawnPos = GetSpawnPosition(player);

        var obj = runner.Spawn(
            playerPrefab,
            spawnPos,
            Quaternion.identity,
            player
        );

        playerObjects[player] = obj;

        // 2명 아니면 게임 시작 안됨
        TryStartGame();
    }

    private void TryStartGame()
    {
        if (runner.ActivePlayers.Count() == MAX_PLAYERS)
        {
            Debug.Log("2명 충족 → 게임 시작 조건 만족");
            // GameState RPC or Scene Start 가능
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        if (playerObjects.TryGetValue(player, out var obj))
        {
            runner.Despawn(obj);
            playerObjects.Remove(player);
        }

        Debug.Log($"플레이어 퇴장: {player}");
    }

    // ========================= INPUT =========================

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData();

        data.move = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        var buttons = new NetworkButtons();

        buttons.Set((int)InputButton.InteractPrimary, Input.GetMouseButton(0));
        buttons.Set((int)InputButton.InteractSecondary, Input.GetMouseButton(1));
        buttons.Set((int)InputButton.Dash, Input.GetKey(KeyCode.Space));

        data.buttons = buttons;

        input.Set(data);
    }

    // ========================= SPAWN =========================

    private Vector3 GetSpawnPosition(PlayerRef player)
    {
        if (spawnPoints != null && spawnPoints.Length >= 2)
        {
            int index = player.RawEncoded % 2;
            return spawnPoints[index].position;
        }

        return new Vector3(player.RawEncoded * 2, 1, 0);
    }

    // ========================= REQUIRED CALLBACKS =========================

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected: {reason}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        Debug.Log($"Shutdown: {reason}");
        this.runner = null;
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}