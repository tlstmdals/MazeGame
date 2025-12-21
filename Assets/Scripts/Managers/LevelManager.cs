using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("미로 렌더러 참조")]
    [SerializeField] private MazeRenderer mazeRenderer;

    [Header("레벨별 미로 크기 설정 (기본은 홀수 유지)")]
    [SerializeField] private int baseWidth = 21;
    [SerializeField] private int baseHeight = 21;
    [SerializeField] private int sizeGrowthPerLevel = 0;

    [Header("시드 설정")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int baseSeed = 0;

    public int CurrentLevelIndex { get; private set; } = -1;

    private MazeGenerator generator;

    // ✅ 미로 씬 이름(호러 씬에서 복귀할 때 사용)
    private string mazeSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        generator = new MazeGenerator();
    }

    /// <summary>요청된 레벨을 초기화하고 미로를 생성/렌더링합니다.</summary>
    public void StartLevel(int levelIndex)
    {
        // ✅ 현재 씬을 "미로 씬"으로 기록(복귀용)
        mazeSceneName = SceneManager.GetActiveScene().name;

        CurrentLevelIndex = Mathf.Max(0, levelIndex);

        ResetLevelState();

        if (!TryPrepareRenderer())
        {
            Debug.LogError("[LevelManager] MazeRenderer를 찾을 수 없습니다. 레벨을 시작할 수 없습니다.");
            return;
        }

        MazeConfig config = BuildConfig(CurrentLevelIndex);
        MazeGrid grid = generator.Generate(config);

        mazeRenderer.Render(grid);
        RegisterPlayerForTeleport();

        Debug.Log($"[LevelManager] 레벨 {CurrentLevelIndex} 시작 완료. 미로 크기: {config.Width}x{config.Height}");
    }

    /// <summary>다음 레벨로 진행합니다.</summary>
    public void StartNextLevel()
    {
        StartLevel(CurrentLevelIndex + 1);
    }

    /// <summary>현재 레벨을 다시 시작합니다.</summary>
    public void RestartLevel()
    {
        if (CurrentLevelIndex < 0)
        {
            StartLevel(0);
            return;
        }

        StartLevel(CurrentLevelIndex);
    }

    /// <summary>호러 씬으로 진입합니다(씬 전환은 LevelManager가 책임).</summary>
    public void EnterHorrorScene(string horrorSceneName)
    {
        // 안전장치: 아직 기록이 없으면 현재 씬을 미로 씬으로 기록
        if (string.IsNullOrEmpty(mazeSceneName))
            mazeSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(horrorSceneName);
    }

    /// <summary>호러 씬에서 호출: 미로 씬으로 복귀한 뒤 현재 레벨을 리스타트합니다(A안).</summary>
    public void ReturnFromHorrorAndRestart()
    {
        if (string.IsNullOrEmpty(mazeSceneName))
        {
            Debug.LogError("[LevelManager] mazeSceneName이 비어 있습니다. 복귀할 씬을 알 수 없습니다.");
            return;
        }

        SceneManager.sceneLoaded += OnMazeSceneLoadedForRestart;
        SceneManager.LoadScene(mazeSceneName);
    }

    private void OnMazeSceneLoadedForRestart(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != mazeSceneName) return;

        SceneManager.sceneLoaded -= OnMazeSceneLoadedForRestart;
        RestartLevel();
    }

    /// <summary>레벨 시작 전 공유 상태를 초기화합니다.</summary>
    private void ResetLevelState()
    {
        if (PenaltyManager.Instance != null)
        {
            PenaltyManager.Instance.ResetLocalPenalty();
        }
    }

    /// <summary>레벨 인덱스를 기반으로 미로 설정을 생성합니다.</summary>
    private MazeConfig BuildConfig(int levelIndex)
    {
        int width = baseWidth + sizeGrowthPerLevel * levelIndex;
        int height = baseHeight + sizeGrowthPerLevel * levelIndex;

        // 미로 특성상 홀수 크기를 유지
        if (width % 2 == 0) width += 1;
        if (height % 2 == 0) height += 1;

        MazeConfig config = new MazeConfig(width, height, 0)
        {
            UseRandomSeed = useRandomSeed,
            Seed = baseSeed + levelIndex
        };

        return config;
    }

    /// <summary>미로 렌더러 참조를 확보합니다.</summary>
    private bool TryPrepareRenderer()
    {
        if (mazeRenderer == null)
        {
            mazeRenderer = FindAnyObjectByType<MazeRenderer>();
        }

        return mazeRenderer != null;
    }
    private void RegisterPlayerForTeleport()
    {
        // 1) Tag로 찾기 (추천: Player 오브젝트 Tag를 "Player"로 설정)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        // 2) Tag가 없으면 PlayerController로 찾기(안전망)
        if (playerObj == null)
        {
            var pc = FindAnyObjectByType<PlayerController>();
            if (pc != null) playerObj = pc.gameObject;
        }

        if (playerObj == null)
        {
            Debug.LogWarning("[LevelManager] Player를 찾지 못해 TeleportUtility.RegisterPlayer를 호출하지 못했습니다.");
            return;
        }

        TeleportUtility.RegisterPlayer(playerObj.transform);
    }

}
