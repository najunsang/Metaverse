using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Display Images")]
    public Image imagePlayer;
    public Image imageComputer;

    [Header("Sprites")]
    public Sprite spriteScissors;
    public Sprite spriteRock;
    public Sprite spritePaper;

    // 스프라이트 순환용 변수
    private bool isAnimating = true;
    private float animationInterval = 0.1f;
    private float animationTimer = 0f;
    private int currentSpriteIndex = 0;
    private Sprite[] sprites;

    [Header("Result Text")]
    public TextMeshProUGUI resultText;
    public enum Choice { None, Scissors, Rock, Paper }

    [Header("Score Board")]
    public TextMeshProUGUI textScorePlayer;
    public TextMeshProUGUI textScoreComputer;

    [Header("UI Buttons")]
    public Button buttonScissors;
    public Button buttonRock;
    public Button buttonPaper;
    public Button buttonRestart;
    public Button buttonReset;
    public Button buttonExit;

    [Header("UI Panel")]
    public GameObject panelResult;
    public TextMeshProUGUI textResultPanel;

    private int scorePlayer = 0;
    private int scoreComputer = 0;
    private bool gameEnded = false; // 누락되었던 게임 종료 상태 변수 추가

    // 플레이어가 선택한 값 저장
    private Choice playerChoice = Choice.None;

    const int WinScore = 5;
    
    void Start()
    {
        sprites = new Sprite[] { spriteScissors, spriteRock, spritePaper };

        // 버튼 리스너 연결
        buttonScissors.onClick.AddListener(() => OnPlayerChoice(Choice.Scissors));
        buttonRock.onClick.AddListener(() => OnPlayerChoice(Choice.Rock));
        buttonPaper.onClick.AddListener(() => OnPlayerChoice(Choice.Paper));
        
        if (buttonRestart != null) buttonRestart.onClick.AddListener(() => OnRestart());
        if (buttonReset != null) buttonReset.onClick.AddListener(() => OnReset());
        if (buttonExit != null) buttonExit.onClick.AddListener(() => OnExit());

        // 시작 시 결과 패널은 숨기기
        if (panelResult != null) panelResult.SetActive(false);

        resultText.text = "가위바위보 중 하나를 선택하세요!";
        textScorePlayer.text = "0";
        textScoreComputer.text = "0";
    }

    void Update()
    {
        if (isAnimating && !gameEnded)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationInterval)
            {
                animationTimer = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % 3;
                imageComputer.sprite = sprites[(currentSpriteIndex + 1) % 3];
            }
        }
    }

    Choice GetComputerChoice()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0: return Choice.Scissors;
            case 1: return Choice.Rock;
            case 2: return Choice.Paper;
            default: return Choice.Rock;
        }
    }

    string DetermineWinner(Choice player, Choice computer)
    {
        if (player == computer)
            return "무승부!";

        // 플레이어 승리 조건: 가위->보, 바위->가위, 보->바위
        bool playerWins = (player == Choice.Scissors && computer == Choice.Paper) ||
                          (player == Choice.Rock && computer == Choice.Scissors) ||
                          (player == Choice.Paper && computer == Choice.Rock);

        if (playerWins)
            scorePlayer++;
        else
            scoreComputer++;

        return playerWins ? "플레이어 승리!" : "컴퓨터 승리!";
    }

    void OnPlayerChoice(Choice choice)
    {
        if (gameEnded) return; // 게임이 끝났다면 클릭 무시

        isAnimating = false; // 컴퓨터 선택 애니메이션 중지

        playerChoice = choice;
        Debug.Log("플레이어 선택: " + choice.ToString());

        // 컴퓨터 선택 및 승부 판정
        Choice computerChoice = GetComputerChoice();
        Debug.Log("컴퓨터 선택: " + computerChoice.ToString());

        imagePlayer.sprite = GetSpriteFromChoice(playerChoice);
        imageComputer.sprite = GetSpriteFromChoice(computerChoice);

        string result = DetermineWinner(playerChoice, computerChoice);
        resultText.text = result;
        textScorePlayer.text = scorePlayer.ToString();
        textScoreComputer.text = scoreComputer.ToString();
        Debug.Log("결과: " + result);

        if (scorePlayer >= WinScore || scoreComputer >= WinScore)
        {
            EndGame();
        }
        else
        {
            // 5점에 도달하지 않았다면 잠시 후 다시 애니메이션을 재개하거나 다음 판을 준비할 수 있도록 설정 가능
        }
    }

    Sprite GetSpriteFromChoice(Choice choice)
    {
        switch (choice)
        {
            case Choice.Scissors: return spriteScissors;
            case Choice.Rock: return spriteRock;
            case Choice.Paper: return spritePaper;
            default: return spriteRock;
        }
    }

    void OnRestart()
    {
        // 한 판 재시작 (점수는 유지하고 애니메이션과 선택만 초기화)
        gameEnded = false;
        isAnimating = true;
        animationTimer = 0f;
        currentSpriteIndex = 0;
        imagePlayer.sprite = spriteRock;
        imageComputer.sprite = spriteRock;
        resultText.text = "가위 바위 보 중 하나를 선택하세요!";
        SetRpsButtonsInteractable(true);
    }

    void SetRpsButtonsInteractable(bool value)
    {
        if (buttonScissors != null) buttonScissors.interactable = value;
        if (buttonRock != null) buttonRock.interactable = value;
        if (buttonPaper != null) buttonPaper.interactable = value;
    }

    void EndGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (panelResult != null)
            panelResult.SetActive(true);

        if (textResultPanel != null)
        {
            if (scorePlayer >= WinScore)
                textResultPanel.text = "게임 종료!\n\n플레이어 승리! (5점 달성)";
            else
                textResultPanel.text = "게임 종료!\n\n컴퓨터 승리! (5점 달성)";
        }
        
        SetRpsButtonsInteractable(false);
    }

    void OnExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void OnReset()
    {
        // 전체 게임 초기화 (점수 0점부터 다시 시작)
        gameEnded = false;
        scorePlayer = 0;
        scoreComputer = 0;
        textScorePlayer.text = "0";
        textScoreComputer.text = "0";

        if (panelResult != null)
            panelResult.SetActive(false);

        isAnimating = true;
        animationTimer = 0f;
        currentSpriteIndex = 0;
        imagePlayer.sprite = spriteRock;
        imageComputer.sprite = spriteRock;
        resultText.text = "가위 바위 보 중 하나를 선택하세요!";

        SetRpsButtonsInteractable(true);
    }
}