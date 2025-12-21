using UnityEngine;
using System.Collections.Generic;

public class PenaltyManager : MonoBehaviour
{
    public static PenaltyManager Instance;

    [Header("Penalty Counters")]
    public int LevelPenalty { get; private set; } = 0;
    public int GlobalPenalty { get; private set; } = 0; // reserved for future

    [Header("Penalty Settings")]
    public int penaltyThreshold = 3;     // Required penalties before conversion
    public int convertCount = 1;         // Number of doors to convert at threshold

    [Header("Horror Door Replacement")]
    [Tooltip("FakeDoor를 Horror 문벽 프리팹으로 교체할 때 사용할 프리팹(반드시 HorrorDoor 포함 권장)")]
    public GameObject horrorDoorWallPrefab;

    private readonly List<FakeDoor> registeredFakeDoors = new List<FakeDoor>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // MazeRenderer가 FakeDoor 스폰 시 호출
    public void RegisterFakeDoor(DoorBase door)
    {
        FakeDoor fake = door as FakeDoor;
        if (fake != null && !registeredFakeDoors.Contains(fake))
        {
            registeredFakeDoors.Add(fake);
        }
    }

    public void AddLocalPenalty()
    {
        LevelPenalty++;
        Debug.Log("[PenaltyManager] Local penalty increased: " + LevelPenalty);

        if (LevelPenalty >= penaltyThreshold)
        {
            ConvertFakeDoorsToHorror();
            LevelPenalty = 0;
        }
    }

    public void ResetLocalPenalty()
    {
        LevelPenalty = 0;
        Debug.Log("[PenaltyManager] Local penalty reset.");
    }

    private void ConvertFakeDoorsToHorror()
    {
        // ✅ 이미 삭제된 문 참조 제거
        registeredFakeDoors.RemoveAll(d => d == null);

        if (registeredFakeDoors.Count == 0)
        {
            Debug.LogWarning("[PenaltyManager] No FakeDoors available to convert.");
            return;
        }

        if (horrorDoorWallPrefab == null)
        {
            Debug.LogError("[PenaltyManager] horrorDoorWallPrefab이 비어있습니다. 프리팹 교체 변환을 할 수 없습니다.");
            return;
        }

        int count = Mathf.Min(convertCount, registeredFakeDoors.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, registeredFakeDoors.Count);
            FakeDoor fake = registeredFakeDoors[index];

            if (fake == null)
            {
                registeredFakeDoors.RemoveAt(index);
                i--;
                continue;
            }

            GameObject oldObj = fake.gameObject;

            // ✅ 기존 오브젝트의 트랜스폼/부모 정보 저장
            Transform oldT = oldObj.transform;
            Vector3 pos = oldT.position;
            Quaternion rot = oldT.rotation;
            Vector3 scale = oldT.localScale;
            Transform parent = oldT.parent; // 보통 MazeRenderer의 transform

            // ✅ 새 Horror 문벽 프리팹 생성(교체)
            GameObject newObj = Instantiate(horrorDoorWallPrefab, pos, rot, parent);
            newObj.transform.localScale = scale;

            // (선택) 이름 유지하면 Hierarchy에서 찾기 쉬움
            newObj.name = oldObj.name + "_Horror";

            // ✅ 기존 FakeDoor 문벽 제거
            Destroy(oldObj);

            // ✅ 리스트에서 제거
            registeredFakeDoors.RemoveAt(index);

            // ✅ 로그(콘솔 클릭하면 오브젝트 선택됨)
            Debug.Log("[PenaltyManager] Replaced FakeDoor with HorrorDoorWallPrefab.", newObj);
        }
    }
}
