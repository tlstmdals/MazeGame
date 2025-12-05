# 🧩 Unity Maze Horror Prototype  
**Unity Version:** 6000.2.14f1  
**Input System:** Both (Old + New Input System Enabled)  

본 프로젝트는 절차적 미로 생성, 다양한 타입의 문(페이크/호러/골) 상호작용,  
패널티 시스템, 랜덤 텔레포트, 공포 맵 전환 등의 기능을 포함한  
**1인칭 미로 탐험 게임(Horror Prototype)** 입니다.

---

## 🚀 현재 구현된 핵심 기능

### ✔ 미로 생성 시스템 (MazeGenerator / MazeRenderer)
- 셀 기반 랜덤 미로 생성
- 바닥, 벽 생성
- 문(Fake / Horror / Goal) 자동 배치
- walkableCells 등록 → 랜덤 텔레포트 기능 연동

### ✔ 문 시스템 (Door Framework)
모든 문은 `DoorBase` 를 상속하며 다음 타입을 가짐:

| 문 종류 | 설명 |
|--------|------|
| FakeDoor | 패널티 증가 + 랜덤 텔레포트 |
| HorrorDoor | 패널티 증가 + 공포씬(HorrorScene) 이동 |
| GoalDoor | 패널티 초기화 + 씬 재시작 / 추후 다음 레벨 이동 |

### ✔ 문 상호작용 (DoorInteraction)
- Trigger 방식 상호작용
- 문 근처에서 **E 키** 입력 (WASD 이동 포함)
- Canvas UI 연동 가능

### ✔ 패널티 시스템 (PenaltyManager)
- 레벨 내부(Local) 패널티만 존재
- 임계치 도달 시 FakeDoor → HorrorDoor 변환
- MazeRenderer 생성 시 FakeDoor 자동 등록

### ✔ 텔레포트 유틸리티 (TeleportUtility)
- 미로 내부 walkableCells 기반 랜덤 텔레포트
- CharacterController 정상 이동 처리 포함

---

## 🎮 플레이어 이동 (WASD + 마우스)
플레이어는 **Unity Input System 설정을 Both로 적용**하여  
Old/New Input System 둘 다 사용 가능하도록 구성함.

### 포함된 기능:
- WASD 이동
- 마우스 시점 회전
- CharacterController 기반

---

## 📁 프로젝트 디렉토리 구조

```plaintext
Assets/
├─ Scripts/
│  ├─ Maze/
│  │  ├─ Maze.cs
│  │  ├─ MazeGenerator.cs
│  │  ├─ MazeRenderer.cs
│  │
│  ├─ Door/
│  │  ├─ DoorBase.cs
│  │  ├─ FakeDoor.cs
│  │  ├─ HorrorDoor.cs
│  │  ├─ GoalDoor.cs
│  │  ├─ DoorInteraction.cs
│  │
│  ├─ Managers/
│  │  ├─ PenaltyManager.cs
│  │
│  ├─ Utility/
│     ├─ TeleportUtility.cs
│
├─ Prefabs/
│  ├─ Door.prefab
│  ├─ Wall.prefab
│  ├─ Floor.prefab
│  ├─ Player.prefab
│
├─ Scenes/
│  ├─ TestScene.unity
│  ├─ HorrorScene.unity (예정)
