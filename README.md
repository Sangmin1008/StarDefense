# SlimeDefense

SlimeDefense는 Unity 기반의 그리드 타워 디펜스 게임입니다.
그리드 위에 영웅을 배치하고 웨이브를 막아 지휘관을 끝까지 지켜내는 것이 목표입니다.

단기적인 기능 구현보다 **기능이 추가되거나 수정될 때 영향 범위를 좁힐 수 있는 구조**를 우선 목표로 삼았습니다.
MVP 패턴, 의존성 주입(DI), 반응형 프로그래밍(FRP), 비동기 제어 등을 적용하여
데이터 / 뷰 / 로직의 역할을 나누고, 클래스 간 의존을 줄이는 방향으로 작업했습니다.

---

## 주요 기술 스택 (Tech Stack)

| 분류 | 사용 기술 |
|---|---|
| Engine | Unity 2D 2022.3.62f3 |
| Architecture | MVP, Assembly Definition |
| DI | VContainer |
| Reactive / Async | UniRx, UniTask |
| Optimization | Object Pooling, Sprite Atlas |
| Data | ScriptableObject |

---

## 핵심 아키텍처 설계 의사결정

### 1. MVP + VContainer로 책임과 의존성 분리

이전 프로젝트에서 MonoBehaviour 하나에 데이터와 UI 코드가 섞이거나, 싱글톤을 남용해 클래스 간 의존이 늘어나는 문제를 경험했습니다.
이를 개선하기 위해 Model / View / Presenter로 역할을 나누고, VContainer를 도입해 필요한 클래스만 생성자로 주입받도록 구성했습니다.

**Scope 단위 수명주기 관리**

VContainer의 Scope를 3단계로 나눠 생명주기와 메모리 정리 범위를 명확히 했습니다.

- **ProjectLifetimeScope (최상위):** 씬이 바뀌어도 유지되는 전체 스테이지 데이터(StageConfig), 게임 흐름 제어(GameManagerModel, SceneFlowService)를 싱글톤으로 관리합니다.
- **InGameLifetimeScope (전투):** 전투 씬에서만 쓰이는 적 스포너, 유닛 모델, 전투 UI 프리팹 등을 묶어, 로비로 돌아갈 때 관련 메모리가 한 번에 정리되도록 했습니다.
- **LobbyLifetimeScope (로비):** 로비 진입 시 LobbyView 프리팹을 동적으로 생성하고 LobbyPresenter만 가볍게 연결합니다.

생성자를 보면 해당 클래스가 무엇에 의존하는지 파악할 수 있고, UI 구조가 바뀌어도 게임 로직(Model)은 건드리지 않아도 되는 구조를 만들었습니다.

---

### 2. Assembly Definition으로 단방향 의존성 구축

프로젝트를 Core / Gameplay / UI / Lobby / Scopes로 나누고, asmdef로 각 모듈이 참조할 수 있는 방향을 제한했습니다.

```
[Scopes]  →  [Gameplay] [Lobby] [UI]  →  [Core]
```

- 하위 모듈이 상위 모듈을 직접 참조하지 않도록 제한하여, 기능을 추가할 때 영향 범위를 파악하기 쉬워졌습니다.
- 변경된 모듈만 재컴파일하기 때문에 전체 컴파일 시간이 줄었습니다.
- Model이나 Config에서 View를 직접 참조하던 코드는 GameObject로 디커플링했습니다.

---

### 3. UniRx를 이용한 반응형 상태 동기화

체력, 코인, 웨이브 상태를 Update에서 폴링하면 조건문이 늘고 UI 갱신 시점이 코드 곳곳으로 흩어지는 문제가 있습니다.
핵심 상태를 ReactiveProperty로 선언하고, Presenter가 구독하여 변화가 생겼을 때만 UI를 갱신하도록 구성했습니다.

```csharp
// Presenter에서의 구독 예시
_model.CurrentHp
    .Subscribe(hp => _view.UpdateHpBar(hp))
    .AddTo(_disposables);
```

- Model은 View의 존재를 알지 못하고 순수 로직에만 집중합니다.
- GameUIPresenter에서 Observable.CombineLatest, Where 등을 활용해 다중 데이터 변화를 감지하고 UI를 갱신했습니다.
- 오브젝트가 파괴될 때 CompositeDisposable을 일괄 해제해 구독이 남아있지 않도록 처리했습니다.

---

### 4. UniTask 기반 비동기 제어

웨이브 대기, 적 스폰, UI 연출처럼 시간이 걸리는 로직은 씬 전환과 충돌하기 쉽습니다.
Unity의 기존 Coroutine은 실행마다 GC를 생성하는 문제도 있었습니다.

비동기 로직은 UniTask를 주로 사용했고, CancellationToken으로 종료 조건을 명시해 오브젝트가 파괴된 뒤에도 작업이 계속 도는 상황을 방지했습니다.

**웨이브 제어 (EnemySpawner)**

```csharp
async UniTaskVoid WaveRoutineAsync(CancellationToken token)
{
    try
    {
        foreach (var wave in 총_웨이브_목록)
        {
            while (대기_시간_남음)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
                // 카운트다운 UI 갱신
            }
            for (int i = 0; i < 이번_웨이브_스폰_수; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(스폰_간격), cancellationToken: token);
                // 적 오브젝트 스폰
            }
            await UniTask.WaitUntil(() => 모든_적_사망 || 게임오버, cancellationToken: token);
        }
    }
    catch (OperationCanceledException)
    {
        // 씬 전환 또는 게임 재시작 시 안전하게 루프 종료
    }
}
```

**애니메이션 최적화 (ProgressTweener)**

DOTween 대신 UniTask 기반의 간단한 트위너를 직접 작성했습니다. 기능은 단순하지만 GC 부하를 줄이는 데 목적이 있었습니다.

---

### 5. 코어 기반 유틸리티 시스템

**Save System (씬 로드 타이밍 문제 대응)**

JSON 직렬화 기반의 저장 시스템을 구현했습니다.
세이브 데이터를 불러올 때 씬이 완전히 로드되기 전에 Player 오브젝트에 접근하면 null 참조가 발생하는 문제가 있었습니다.
SceneManager.sceneLoaded 이벤트를 구독하고, 콜백 내에서 데이터를 주입한 뒤 즉시 구독을 해제하는 방식으로 해결했습니다.

```csharp
void ApplySaveDataToGame(SaveData data)
{
    UnityAction<Scene, LoadSceneMode> onLoaded = null;
    onLoaded = (scene, mode) =>
    {
        ApplyPlayerData(data);
        FindPlayer();
        SceneManager.sceneLoaded -= onLoaded; // 1회 실행 후 해제
    };
    SceneManager.sceneLoaded += onLoaded;
    SceneLoadManager.Instance.LoadMainScene();
}
```

**Table Manager (에디터 자동 등록)**

테이블 추가 시마다 인스펙터에서 수동으로 등록해야 하는 번거로움이 있었습니다.
에디터에서 지정 경로를 탐색해 리스트를 자동으로 채우는 기능을 만들었고, 런타임에는 Type을 키로 딕셔너리에 캐싱해 조회 시 리스트를 처음부터 순회하지 않도록 했습니다.

```csharp
// 에디터 자동 등록 (의사 코드)
public void AutoAssignTables()
{
    tableList.Clear();
    var assets = FindAllScriptableObjects("Assets/Tables/");
    foreach (var asset in assets)
    {
        if (asset is ITable) tableList.Add(asset);
    }
}
```

**UI Framework (Stack 기반 팝업 관리)**

팝업이 겹쳐 있을 때 Cancel 입력이 가장 위 UI만 닫도록, Stack 구조를 적용했습니다.
씬 전환 시에는 스택과 딕셔너리를 모두 초기화하고 새 씬의 UIRoot를 다시 스캔합니다.

---

### 6. 전투 연산 및 렌더링 최적화

**Enemy Registry 기반 전투 매핑**

EnemyModel과 EnemyView를 양방향 Dictionary로 등록하여, 충돌 발생 시 GetComponent 탐색 없이 즉시 대상 모델을 찾을 수 있도록 구성했습니다.

**Sprite Atlas와 렌더링 최적화**

에너미, 로비 UI, 인게임 UI 등 성격이 다른 에셋을 별도 Atlas로 분리하여 배칭 효율을 높였습니다.
아틀라스는 2의 배수(POT) 규격 및 ASTC 4x4 포맷으로 압축하고 타겟 프레임을 60으로 제한하여, 모바일 기기의 발열과 메모리 낭비를 줄였습니다.

동일한 부하 테스트 환경(화면 내 유닛·투사체 약 50개 기준) 프로파일링 결과:

| 항목 | 적용 전 | 적용 후 | 변화 |
|---|---|---|---|
| SetPass Calls | 57 | 13 | -77% |
| Batches | 60 | 10 | -83% |

**Sub-Canvas 분리 (UI 렌더링 스파이크 방지)**

화면 레이아웃을 정적 캔버스와 동적 캔버스로 분리해 Rebuild 연산을 줄였습니다.
팝업 창은 SetActive 대신 Canvas.enabled를 조작하여 오버드로우와 프레임 끊김 현상을 방지했습니다.

---

## 기능별 세부 시스템

### 맵 및 그리드 상호작용 시스템

- **GridClickDetector:** 타일맵 클릭을 감지하여 셀 좌표와 월드 좌표를 반환합니다. 게임 시작 시 맵 전체를 스캔하여 파괴된 타일 데이터를 수집합니다.
- **GridManager:** 2D 그리드 데이터를 Dictionary와 HashSet으로 관리합니다. 셀의 점유 상태, 영웅 배치/제거, 파괴된 셀 관리를 전담하는 순수 데이터 클래스입니다.
- **GridInteractionPresenter:** 마우스 클릭 이벤트를 구독하여 셀 상태에 따라 수리(Repair), 영웅 소환(Summon), 업그레이드 팝업을 호출합니다.
- **WaypointPath & PathDataSO:** 적의 이동 경로를 ScriptableObject 좌표 배열로 정의하여 다중 경로 확장이 가능하도록 설계했습니다.

### 엔티티 시스템

- **Hero:** HeroManager를 통해 코인을 소모하여 무작위 타입으로 소환합니다. 동일 등급·타입의 영웅 두 마리를 병합하여 상위 등급(Legendary까지)으로 진화시킬 수 있습니다. 주변 적을 탐지하여 투사체 발사를 지시합니다.
- **Enemy:** EnemyRegistry의 레지스트리 패턴으로 활성화된 적을 추적합니다. 경로를 따라 이동하며, 처치 시 코인을 드롭하고 목적지 도달 시 지휘관에게 데미지를 입힙니다.
- **Commander:** 적 탈출 이벤트 발생 시 체력이 감소하며, 체력이 0이 되면 게임 오버를 트리거합니다.

### 웨이브 시스템

- **WaveModel:** 현재 웨이브 인덱스, 생존 적 수, 대기 시간, 게임 오버 여부를 ReactiveProperty로 관리합니다.
- **EnemySpawner:** UniTask 기반 루프로 웨이브 카운트다운과 적 스폰을 비동기로 처리합니다. CancellationToken으로 게임 오버 시 즉시 중단합니다.
- **WavePresenter:** 지휘관 사망 시 패배 처리, 마지막 웨이브 섬멸 시 승리 처리를 담당합니다.

### 투사체 시스템

- **ProjectileManager:** 프리팹별로 ObjectPool을 유지하여 발사체 동적 생성·파괴로 인한 GC 부하를 줄입니다.
- 영웅이 조준한 타겟을 향해 이동하며, 이동 도중 타겟이 사망하더라도 마지막 방향으로 직진하여 다른 적에게도 데미지를 줄 수 있습니다.
- 투사체가 EnemyView와 충돌하면 EnemyRegistry를 통해 매핑된 EnemyModel을 O(1)로 찾아 데미지를 적용합니다.

### 경제 시스템

- **CoinModel:** 현재 보유 코인을 ReactiveProperty로 관리합니다. 초기 자본은 StageConfig에서 설정됩니다.
- 적 처치 시 Config에 정의된 보상만큼 코인이 증가하며, 소환·승급·수리 시 TrySpendCoin을 통해 차감합니다. 영웅 등급별 비용은 HeroCostHelper에서 계산합니다.

### 데이터 주도 설계

밸런스 수치와 기획 데이터는 스크립트 수정 없이 인스펙터에서 조정할 수 있도록 ScriptableObject로 관리합니다.

- **StageConfig:** 초기 코인, 지휘관 설정 및 위치, 웨이브 목록
- **WaveConfig:** 웨이브별 적 스폰 종류, 수량, 간격, 대기 시간
- **HeroConfig / EnemyConfig / CommanderConfig:** 체력, 공격력, 사거리, 이동 속도, 투사체 매핑, 보상 등 개별 스탯

---

## 플레이 흐름

1. **초기화:** ProjectLifetimeScope가 전역 데이터를 세팅하고, 인게임 진입 시 InGameLifetimeScope가 의존성을 주입하여 StageInitializer가 지휘관을 배치합니다.
2. **배치 및 수리:** 초기 코인으로 깨진 타일을 복구하거나 빈 그리드에 영웅을 소환합니다.
3. **전투 진행:** 대기 시간 후 적 웨이브가 경로를 따라 생성됩니다. 영웅들은 자동 타겟팅으로 적을 처치합니다.
4. **성장:** 적 처치로 코인을 획득하고, 동일 등급·타입의 영웅을 병합하여 상위 등급으로 업그레이드합니다.
5. **결과:** 모든 웨이브를 막아내면 승리, 지휘관 체력이 0이 되면 패배 화면으로 전환됩니다.
