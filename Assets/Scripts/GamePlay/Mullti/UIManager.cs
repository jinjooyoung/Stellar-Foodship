using JinJooYoung;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ===============================
    // MAIN MENU
    // ===============================

    [Header("Main Menu")]
    [SerializeField] private CanvasGroup mainMenuUI;
    [SerializeField] private CanvasGroup joinRoomPopup;

    // ===============================
    // LOBBY
    // ===============================

    [Header("Lobby")]
    [SerializeField] private CanvasGroup lobbyUI;

    [SerializeField] private TMP_Text roomCodeText;

    [SerializeField] private GameObject player1Image;
    [SerializeField] private GameObject player2Image;

    [SerializeField] private GameObject player1ReadyImage;
    [SerializeField] private GameObject player2ReadyImage;

    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;

    [SerializeField] private Button leaveRoomButton;

    // ===============================
    // JOIN ROOM
    // ===============================

    [Header("Join Room")]
    [SerializeField] private TMP_InputField roomCodeInput;

    // ===============================
    // TOAST
    // ===============================

    [Header("Toast")]
    [SerializeField] private CanvasGroup toastUI;
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private float toastDuration = 2f;

    Coroutine toastRoutine;

    private void Start()
    {
        ShowMainMenu();
    }

    public void CreateRoomUISetting(string roomCode)
    {
        ShowLobby();
        SetRoomCode(roomCode);
        SetHostUI();
        SetPlayerCount(1);
        SetReadyState(false, false);
        SetStartButtonInteractable(false);
    }

    // ===============================
    // SCREEN
    // ===============================

    public void ShowMainMenu()
    {
        SetCanvas(mainMenuUI, true);
        SetCanvas(joinRoomPopup, false);
        SetCanvas(lobbyUI, false);
    }

    public void ShowLobby()
    {
        SetCanvas(mainMenuUI, false);
        SetCanvas(joinRoomPopup, false);
        SetCanvas(lobbyUI, true);
    }

    public void OpenJoinRoomPopup()
    {
        SetCanvas(joinRoomPopup, true);
    }

    public void CloseJoinRoomPopup()
    {
        SetCanvas(joinRoomPopup, false);
    }

    // ===============================
    // ROOM CODE
    // ===============================

    public void SetRoomCode(string code)
    {
        roomCodeText.text = $"ROOM : {code}";
    }

    // ===============================
    // PLAYER SLOT
    // ===============================

    public void SetPlayerCount(int count)
    {
        player1Image.SetActive(count >= 1);
        player2Image.SetActive(count >= 2);
    }

    public void SetReadyState(bool p1Ready, bool p2Ready)
    {
        player1ReadyImage.SetActive(p1Ready);
        player2ReadyImage.SetActive(p2Ready);
    }

    // ===============================
    // HOST / CLIENT
    // ===============================

    public void SetHostUI()
    {
        startButton.gameObject.SetActive(true);
    }

    public void SetClientUI()
    {
        startButton.gameObject.SetActive(false);
    }

    public void SetStartButtonInteractable(bool value)
    {
        startButton.interactable = value;
    }

    // ===============================
    // TOAST
    // ===============================

    public void ShowToast(string message)
    {
        if (toastRoutine != null)
            StopCoroutine(toastRoutine);

        toastRoutine = StartCoroutine(ShowRoutine(message));
    }

    IEnumerator CoToast(string msg)
    {
        toastText.text = msg;

        SetCanvas(toastUI, true);

        yield return new WaitForSeconds(toastDuration);

        SetCanvas(toastUI, false);
    }

    IEnumerator ShowRoutine(string message)
    {
        toastText.text = message;

        MyTween.Fade(
            toastUI,
            1f,
            0.2f);

        yield return new WaitForSeconds(
            0.2f + 1.5f);

        MyTween.Fade(
            toastUI,
            0f,
            0.2f);

        yield return new WaitForSeconds(
            0.2f);

        toastRoutine = null;
    }

    // ===============================
    // ROOM CODE VALIDATION
    // ===============================

    public bool TryGetRoomCode(out string roomCode)
    {
        roomCode = roomCodeInput.text.Trim();

        if (roomCode.Length != 6)
        {
            ShowToast("올바르지 않은 방코드입니다.");
            return false;
        }

        foreach (char c in roomCode)
        {
            if (!char.IsDigit(c))
            {
                ShowToast("올바르지 않은 방코드입니다.");
                return false;
            }
        }

        return true;
    }

    // ===============================
    // UTIL
    // ===============================

    private void SetCanvas(CanvasGroup cg, bool active)
    {
        cg.alpha = active ? 1 : 0;
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }
}