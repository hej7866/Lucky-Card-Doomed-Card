# Lucky Card, Doomed Dice

> **"행운의 카드, 파멸의 카드"**  
> 전략적 카드 조합과 주사위의 확률이 교차하는 턴제 전략 게임

## 소개

**Lucky Card, Doomed Dice**는 플레이어가 **두 장의 카드를 조합하여** 전투를 수행하는 **전략 턴제 게임**이다.  
각 카드의 고유 효과와 조합 결과에 따라 전황이 극적으로 바뀌며, 주사위의 결과가 변수로 작용해 매 턴마다 긴장감을 유발한다.

## 주요 기능

- **카드 조합 시스템**: 두 장의 카드를 선택하여 점수 계산
- **멀티 플레이 기반 구조**

## 기술 스택

- **Engine**: Unity2022.3.14f1
- **Language** : C#
- **IDE** : Visual Studio Code

---

## 프로젝트 구조
```
Assets/
├── Scripts/
│ ├── Managers/ # 싱글톤 매니저 (GameManager 등)
│ └── CrackCard/ # 크랙카드 시스템 관리
│ └── Effect/ # 전투 이펙트 관리
├── ScriptableObjects/
│ └── CrackCards/ # 크랙카드 데이터 관리
├── Prefabs/ # 카드, UI, 효과 등 프리팹
└── Scenes/ # 게임 씬 / 로비 씬
```

## 실행 방법

1. 다운받은 폴더 열기
2. Lunky Card, Doomed Card.exe 실행
3. 닉네임을 설정 한 후 방 개설 및 입장하여 플레이

## 향후 개발 계획

- 카드 능력치 및 조합 시너지 밸런스 조정  
- 전투 연출 및 이펙트 강화  
- 덱 구성 시스템 추가  

