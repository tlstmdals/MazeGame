using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    [Header("Door Wall Prefabs")]
    public GameObject fakeDoorWallPrefab;
    public GameObject goalDoorWallPrefab;

    public float cellSize = 1f;

    public int fakeDoorCountMin = 3;
    public float fakeDoorDensity = 0.12f;

    // ✅ 이제 goal/fake를 코드에서 붙이지 않을 거면 false 권장
    public bool addDoorScriptToChild = false;
    public string doorChildName = "Door";

    private MazeGrid grid;

    public void Render(MazeGrid grid)
    {
        if (grid == null) return;
        this.grid = grid;

        // 이전 생성물 정리
        List<Transform> children = new();
        foreach (Transform child in transform) children.Add(child);
        foreach (Transform child in children) Destroy(child.gameObject);

        // 1) 바닥 렌더 + 워커블 셀 수집
        var walkable = new List<Vector3>();

        if (floorPrefab != null)
        {
            for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
            {
                Vector3 cellPos = CellCenter(x, y);
                SpawnWithGroundSnap(floorPrefab, cellPos, Quaternion.identity);

                // ✅ 텔포 후보 셀 등록(바닥 전체를 워커블로 취급)
                walkable.Add(cellPos);
            }
        }

        // ✅ 텔포 유틸에 등록
        TeleportUtility.RegisterWalkableCells(walkable);

        // 2) doorWall을 배치할 "내벽" 엣지들을 선택 (외벽 제외)
        List<WallEdge> doorWalls = PickDoorWallEdgesInnerOnly();

        // 2-1) ✅ 도어 엣지별 프리팹 매핑 구성
        // - 1개: Goal
        // - 나머지: Fake
        var doorPrefabMap = new Dictionary<WallEdge, GameObject>(new WallEdgeComparer());

        if (doorWalls.Count > 0)
        {
            // GoalDoor 1개 선택 (첫 번째를 Goal로 고정)
            // 원하면 랜덤으로 바꿔도 됨.
            WallEdge goalEdge = doorWalls[0];

            if (goalDoorWallPrefab != null)
                doorPrefabMap[goalEdge] = goalDoorWallPrefab;
            else
                Debug.LogWarning("[MazeRenderer] goalDoorWallPrefab이 비어있습니다. GoalDoor 배치가 되지 않습니다.");

            // 나머지는 FakeDoor
            for (int i = 1; i < doorWalls.Count; i++)
            {
                if (fakeDoorWallPrefab != null)
                    doorPrefabMap[doorWalls[i]] = fakeDoorWallPrefab;
                else
                    Debug.LogWarning("[MazeRenderer] fakeDoorWallPrefab이 비어있습니다. FakeDoor 배치가 되지 않습니다.");
            }
        }
        else
        {
            Debug.LogWarning("[MazeRenderer] doorWalls가 0개입니다. 문이 배치되지 않습니다.");
        }

        // 3) 벽 렌더: doorPrefabMap에 포함되면 해당 도어프리팹, 아니면 wallPrefab
        RenderEdges(doorPrefabMap);

        // 4) 필요하면 여기서 doorWallPrefab 내부 Door에 스크립트 붙이는 로직 유지 가능
        if (addDoorScriptToChild)
        {
            ApplyCommonDoorScriptToAllDoorWalls(doorWalls);
        }
    }

    // ------------------- Edge Rendering -------------------

    private void RenderEdges(Dictionary<WallEdge, GameObject> doorPrefabMap)
    {
        // 세로벽
        for (int vx = 0; vx < grid.Width + 1; vx++)
        for (int y = 0; y < grid.Height; y++)
        {
            if (!grid.VerticalWalls[vx, y]) continue;

            var edge = new WallEdge(true, vx, y);
            Vector3 pos = VerticalWallCenter(vx, y);
            Quaternion rot = Quaternion.Euler(0f, 90f, 0f);

            GameObject prefabToUse = doorPrefabMap.TryGetValue(edge, out var doorPrefab)
                ? doorPrefab
                : wallPrefab;

            if (prefabToUse == null) continue;

            GameObject spawned = SpawnWithGroundSnap(prefabToUse, pos, rot);

            // ✅ FakeDoor면 PenaltyManager에 등록 (변환 로직은 PenaltyManager가 담당)
            RegisterIfFakeDoor(spawned);
        }

        // 가로벽
        for (int x = 0; x < grid.Width; x++)
        for (int hy = 0; hy < grid.Height + 1; hy++)
        {
            if (!grid.HorizontalWalls[x, hy]) continue;

            var edge = new WallEdge(false, x, hy);
            Vector3 pos = HorizontalWallCenter(x, hy);
            Quaternion rot = Quaternion.identity;

            GameObject prefabToUse = doorPrefabMap.TryGetValue(edge, out var doorPrefab)
                ? doorPrefab
                : wallPrefab;

            if (prefabToUse == null) continue;

            GameObject spawned = SpawnWithGroundSnap(prefabToUse, pos, rot);

            // ✅ FakeDoor면 PenaltyManager에 등록
            RegisterIfFakeDoor(spawned);
        }
    }

private void RegisterIfFakeDoor(GameObject spawned)
{
    if (spawned == null) return;

    // ✅ 루트에 없으면 자식에서 찾기
    DoorBase door = spawned.GetComponent<DoorBase>();
    if (door == null)
        door = spawned.GetComponentInChildren<DoorBase>();

    if (door is FakeDoor)
    {
        if (PenaltyManager.Instance != null)
        {
            PenaltyManager.Instance.RegisterFakeDoor(door);
            Debug.Log("[MazeRenderer] FakeDoor registered to PenaltyManager.");
        }
        else
        {
            Debug.LogWarning("[MazeRenderer] PenaltyManager.Instance is null.");
        }
    }
}


    // ------------------- Pick DoorWall Edges (Inner Only) -------------------

    private List<WallEdge> PickDoorWallEdgesInnerOnly()
    {
        var candidates = new List<WallEdge>();

        // 세로벽 내벽: vx 1..Width-1 만 허용 (0, Width는 외벽)
        for (int vx = 1; vx < grid.Width; vx++)
        for (int y = 0; y < grid.Height; y++)
        {
            if (!grid.VerticalWalls[vx, y]) continue;
            candidates.Add(new WallEdge(true, vx, y));
        }

        // 가로벽 내벽: hy 1..Height-1 만 허용 (0, Height는 외벽)
        for (int x = 0; x < grid.Width; x++)
        for (int hy = 1; hy < grid.Height; hy++)
        {
            if (!grid.HorizontalWalls[x, hy]) continue;
            candidates.Add(new WallEdge(false, x, hy));
        }

        int target = Mathf.Max(fakeDoorCountMin, Mathf.RoundToInt(candidates.Count * fakeDoorDensity));
        target = Mathf.Min(target, candidates.Count);

        var rng = new System.Random();
        var result = new List<WallEdge>(target);

        for (int i = 0; i < target; i++)
        {
            int idx = rng.Next(candidates.Count);
            result.Add(candidates[idx]);
            candidates.RemoveAt(idx);
        }

        return result;
    }

    // ------------------- (Optional) Common Door Script Binding -------------------

    private void ApplyCommonDoorScriptToAllDoorWalls(List<WallEdge> doorWalls)
    {
        // 필요 시 구현:
        // - SpawnWithGroundSnap 반환값을 저장해두고,
        // - 그 인스턴스 안의 Door(doorChildName)에게 공통 Door 스크립트만 붙이는 방식
        // 지금은 "프리팹에서 이미 스크립트 세팅"을 권장하므로 비워둠.
    }

    // ------------------- Geometry Helpers -------------------

    private Vector3 CellCenter(int x, int y) => new Vector3(x * cellSize, 0f, y * cellSize);

    private Vector3 VerticalWallCenter(int vx, int y)
    {
        float wx = (vx - 0.5f) * cellSize;
        float wz = y * cellSize;
        return new Vector3(wx, 0f, wz);
    }

    private Vector3 HorizontalWallCenter(int x, int hy)
    {
        float wx = x * cellSize;
        float wz = (hy - 0.5f) * cellSize;
        return new Vector3(wx, 0f, wz);
    }

    private GameObject SpawnWithGroundSnap(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject go = Instantiate(prefab, position, rotation, transform);

        var r = go.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            float bottomY = r.bounds.min.y;
            go.transform.position += new Vector3(0f, -bottomY, 0f);
        }

        return go;
    }

    // ------------------- Types -------------------

    private struct WallEdge
    {
        public bool isVertical;
        public int a;
        public int b;
        public WallEdge(bool isV, int a, int b) { isVertical = isV; this.a = a; this.b = b; }
    }

    private class WallEdgeComparer : IEqualityComparer<WallEdge>
    {
        public bool Equals(WallEdge x, WallEdge y)
            => x.isVertical == y.isVertical && x.a == y.a && x.b == y.b;

        public int GetHashCode(WallEdge obj)
            => (obj.isVertical ? 1 : 0) * 1000000 + obj.a * 1000 + obj.b;
    }
}
