using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FusionBootstrap : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Session")]
    [SerializeField] private string sessionName;
    private List<SessionInfo> cachedSessions = new();

    [Header("Player")]
    //[SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private NetworkPrefabRef[] playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Pickable Spawn")]
    [SerializeField] private NetworkPrefabRef testPickablePrefab;
    [SerializeField] private Transform[] pickableSpawnPoints;

    private bool pickableSpawned = false;

    [SerializeField] private int maxPlayers = 2;

    [Header("Lobby")]
    [SerializeField] private NetworkPrefabRef lobbyDataPrefab;

    private Dictionary<PlayerRef, NetworkObject> lobbyObjects = new();

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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    private void OnDestroy()
    {
        Debug.Log("FusionBootstrap Destroy");
    }

    // ========================= 메인화면 ===========================

    public void CreateRoom()
    {
        Debug.Log($"방 생성 버튼 눌림");
        string roomCode = GenerateRoomCode();

        sessionName = roomCode;

        Debug.Log($"방 생성 : {roomCode}");

        StartHost();
        UIManager.Instance.CreateRoomUISetting(roomCode);
    }

    private string GenerateRoomCode()
    {
        string code;

        do
        {
            code = UnityEngine.Random
                .Range(100000, 1000000)
                .ToString();
        }
        while (SessionExists(code));

        return code;
    }

    private bool SessionExists(string roomCode)
    {
        foreach (var session in cachedSessions)
        {
            if (session.Name == roomCode)
                return true;
        }

        return false;
    }

    public void TryJoinRoom()
    {
        if (!UIManager.Instance.TryGetRoomCode(out string roomCode))
            return;

        sessionName = roomCode;

        StartClient();
    }

    public async void LeaveRoom()
    {
        if (runner == null)
            return;

        GameObject runnerObject = runner.gameObject;

        await runner.Shutdown();

        Destroy(runnerObject);

        runner = null;

        lobbyObjects.Clear();
        playerObjects.Clear();

        pickableSpawned = false;

        UIManager.Instance.ShowMainMenu();
    }

    // ========================= START =========================

    public void StartHost() => _ = StartGame(GameMode.Host);
    public void StartClient() => _ = StartGame(GameMode.Client);

    private async Task StartGame(GameMode mode)
    {
        if (runner != null)
            return;

        GameObject runnerObject =
            new GameObject("NetworkRunnerObject");

        runnerObject.transform.SetParent(transform);

        runner =
            runnerObject.AddComponent<NetworkRunner>();

        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        var sceneManager =
            runnerObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            SceneManager = sceneManager,
            PlayerCount = MAX_PLAYERS,
            EnableClientSessionCreation = false
        });

        if (result.Ok)
        {
            if (mode == GameMode.Client)
            {
                UIManager.Instance.ShowLobby();
                UIManager.Instance.SetPlayerCount(2);
                UIManager.Instance.SetClientUI();
            }
            
            /*Debug.Log($"[Fusion] StartGame OK - {mode}");

            if (runner.IsServer)
            {
                PickableSpawn();
            }*/
        }
        else
        {
            if (mode == GameMode.Host) return;

            switch (result.ShutdownReason)
            {
                case ShutdownReason.GameNotFound:
                    UIManager.Instance.ShowToast("존재하지 않는 방입니다.");
                    break;

                case ShutdownReason.GameIsFull:
                    UIManager.Instance.ShowToast("이미 가득 찬 방입니다.");
                    break;

                default:
                    UIManager.Instance.ShowToast("접속에 실패했습니다.");
                    break;
            }
        }
    }

    public async void StartStageSelectScene()
    {
        if (runner == null)
            return;

        if (!runner.IsServer)
            return;

        Debug.Log("게임 시작");

        await runner.LoadScene(
            SceneRef.FromIndex(1),
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }

    public void PickableSpawn()
    {
        if (!runner.IsServer) return;

        if (pickableSpawned) return;

        pickableSpawned = true;

        if (pickableSpawnPoints == null || pickableSpawnPoints.Length == 0) return;

        foreach (var point in pickableSpawnPoints)
        {
            if(point == null) continue;

            runner.Spawn(testPickablePrefab, point.position, point.rotation, null);
        }

        Debug.Log($"상자 {pickableSpawnPoints.Length} 개 생성 완료");
    }

    // ========================= PLAYER JOIN =========================

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 입장: {player}");

        if (!runner.IsServer)
            return;

        // 2명 초과 차단
        if (lobbyObjects.Count >= maxPlayers)
        {
            Debug.LogWarning($"최대 인원 초과 : {player}");
            return;
        }

        NetworkObject lobbyObj = runner.Spawn(
            lobbyDataPrefab,
            Vector3.zero,
            Quaternion.identity,
            player
        );

        lobbyObjects[player] = lobbyObj;

        Debug.Log($"로비 데이터 생성 완료 : {player}");

        UIManager.Instance.SetPlayerCount(lobbyObjects.Count);
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

        UIManager.Instance.SetPlayerCount(lobbyObjects.Count);
    }

    /*private void Update()
    {
        if (runner == null)
            return;

        if (!runner.IsServer)
            return;

        CheckReadyState();
    }*/

    public void CheckReadyState()
    {
        PlayerLobbyData[] datas =
        FindObjectsByType<PlayerLobbyData>(FindObjectsSortMode.None);

        bool p1Ready = false;
        bool p2Ready = false;

        int playerCount = 0;

        foreach (var data in datas)
        {
            if (data.Object == null)
                continue;

            if (!data.Object.IsValid)
                continue;

            playerCount++;

            if (playerCount == 1)
                p1Ready = data.IsReady;

            if (playerCount == 2)
                p2Ready = data.IsReady;
        }

        UIManager.Instance.SetPlayerCount(playerCount);
        UIManager.Instance.SetReadyState(p1Ready, p2Ready);

        bool allReady =
            playerCount == 2 &&
            p1Ready &&
            p2Ready;

        if (runner.IsServer)
        {
            UIManager.Instance.SetStartButtonInteractable(allReady);
        }
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
        UIManager.Instance.ShowToast("호스트가 방을 종료했습니다.");
        UIManager.Instance.ShowMainMenu();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        Debug.Log($"Shutdown: {reason}");
        this.runner = null;
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        cachedSessions = sessionList;
    }
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