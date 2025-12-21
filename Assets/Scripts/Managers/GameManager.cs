using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("레벨 진행 관리자")]
    [SerializeField] private LevelManager levelManager;

    private bool isInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환에도 유지되도록 처리
    }

    private void Start()
    {
        InitializeGame();
    }

    /// <summary>게임 시작 시 전역 상태를 준비하고 레벨을 위임합니다.</summary>
    private void InitializeGame()
    {
        if (isInitialized)
        {
            Debug.Log("[GameManager] 이미 초기화된 상태입니다.");
            return;
        }

        isInitialized = true;

        if (levelManager == null)
        {
            levelManager = FindAnyObjectByType<LevelManager>();
        }

        if (levelManager == null)
        {
            Debug.LogError("[GameManager] LevelManager를 찾을 수 없어 게임을 시작할 수 없습니다.");
            return;
        }

        Debug.Log("[GameManager] 게임 초기화 완료. 첫 레벨을 시작합니다.");
        levelManager.StartLevel(0);
    }

    /// <summary>외부에서 게임 시작을 다시 요청할 때 사용합니다.</summary>
    public void StartGame()
    {
        InitializeGame();
    }
}
