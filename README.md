# 목차
1. [📌 게임 소개](#1--게임-소개)
2. [👥 팀원 소개](#2--팀원-소개)
3. [🕹️ 게임 규칙 및 주요 기능](#3-️-게임-규칙-및-주요-기능)
4. [🛠 기술 구현](#4--기술-구현)
   - [P2P 매칭 시스템](#p2p-매칭-시스템)
   - [UDP 기반 P2P 실시간 대전 환경 구현](#udp-기반-p2p-실시간-대전-환경-구현)
   - [스킬 애니메이션 시스템](#스킬-애니메이션-시스템)
   - [전략 패턴을 활용한 맵 기믹 교체](#전략-패턴을-활용한-맵-기믹-교체)

<br>

# KittyFight

## 1. 📌 게임 소개
> **한 줄 소개 :** Unity 기반의 캐주얼 멀티플레이어 대전 게임입니다.

* **개발 기간 :** 2025.09 ~ 2026.01
* **개발 인원 :** 4명
* **플랫폼 :** PC
* **Engine :** Unity 2022.3.16f1
* Steam: https://store.steampowered.com/app/4249750/Thirteen_Kitty/
  
<div align="center">
  <img width="800" alt="image" src="https://github.com/user-attachments/assets/e0873b54-79d2-4330-ad16-79a2d98d62be" />
</div>

“Thirteen Kitty”는 12지신에 포함되지 못한 고양이가 13번째 십이지 동물이 되기 위해 경쟁한다는 세계관을 가진 **실시간 1:1 대전 액션 게임**입니다.
플레이어는 귀여운 고양이 캐릭터를 조작하여 상대를 쓰러뜨리고 최고의 자리에 올라야 합니다. 게임의 핵심은 전투 중 등장하는 12지신의 권능을 **스킬 형태로 실시간 획득**하여 활용하는 시스템입니다.
단순한 피지컬 싸움을 넘어, 전황에 따라 변화하는 스킬 조합과 다양한 맵 기믹을 전략적으로 활용해야 하며, 캐주얼한 비주얼 속에서 액션의 재미를 제공하고자 합니다.

<br><br><br><br>

## 2. 👥 팀원 소개
| 이름 | 역할 |
|:---:|:---:|
| 허재승 | Programmer, Game Designer|
| 박민재 | Programmer | 
| 문경서 | Graphic | 
| 박민재 | Graphic |

### 🎁 Special Thanks
| 이름 | 도움 주신 부분 |
|:---:|:---:|
| 손효민 | Acting |
| 김규진 | Graphic |

<br><br><br><br>

## 3. 🕹️ 게임 규칙 및 주요 기능

* **게임 규칙**
<div align="center">
  <table>
    <tr>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/c5c006e2-3dac-490d-85a0-28852e2dec42" width="500" alt="Left Image 1">
      </td>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/a9cc65aa-d6be-4b9c-a05d-731d718a158e" width="500" alt="Right Image 1">
      </td>
    </tr>
  </table>
</div>
<div align="center">
  <table>
    <tr>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/02c37c73-c3f5-4b86-8939-291bbfe78bfb" width="400" alt="Left Image 1">
      </td>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/cf02a1b9-8fb9-4034-9fa5-e5dd7204e58d" width="400" alt="Right Image 1">
      </td>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/27f0a94e-51af-4118-98ab-78212141266f" width="400" alt="Left Image 1">
      </td>
    </tr>
  </table>
</div>
게임의 규칙은 간단합니다. 매칭된 양측 플레이어는 자신이 가지고 있는 스킬들을 활용해서 대전을 하게 되고 상대 플레이어를 쓰러뜨릴 때마다 1포인트를 획득하게 됩니다. 총 11포인트를 획득하면 게임에서 승리하게 됩니다. 
<br><br><br>

* **다양한 스킬들을 활용한 대전**

<div align="center">
  <table>
    <tr>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/ca1b789c-b09d-4f41-8c56-9f10bdfc3fe6" width="500" alt="Left Image 1">
      </td>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/f49d482c-5858-42a0-a1c8-9b1c96257022" width="500" alt="Right Image 1">
      </td>
    </tr>
    <tr>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/b201e28c-f330-4ed4-b713-0b93d035f77f" width="500" alt="Left Image 2">
      </td>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/ed11f3f7-67c5-4262-a762-f43ba1e74d99" width="500" alt="Right Image 2">
      </td>
    </tr>
  </table>
</div>

"Thirteen Kitty"에는 총 60종의 스킬이 있습니다. 게임 플레이 도중 상대방이 2포인트를 획득할 때마다 새로운 스킬을 획득할 수 있고 이를 활용하면 더욱 다이나믹 한 전투 플레이를 즐길 수 있습니다. 각 스킬들은 '소 - 대쉬 관련', '토끼 - 점프 관련', '호랑이 - 공격 관련'과 같이 각 동물들의 특징들을 컨셉으로 한 스킬들이 등장합니다. 
<br><br><br>

* **12종의 맵 기믹 시스템**
<div align="center">
  <table>
    <tr>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/3886d455-d9b8-41d4-be87-f6b63e954289" width="500" alt="Left Image 1">
      </td>
      <td align="center">
        <img src="https://github.com/user-attachments/assets/89ea7497-eef2-433d-8917-69d5541c837f" width="500" alt="Right Image 1">
      </td>
    </tr>
  </table>
</div>
게임 맵은 총 3개의 배경테마, 각각 8개의 맵이 있으며 총 24개의 맵이 있습니다. 이외에도 양측 플레이어의 점수가 5의 배수가 될 때마다 게임에 새로운 규칙이 적용됩니다. '용 기믹'이 등장하게 되면 일정시간 마다 화면 전테를 가로지르는 레이저 공격이 날아오거나, '원숭이 기믹'이 등장하게 되면 일정 시간마다 양측 플레이어의 스킬 쿨타임이 초기화됩니다. 같은 맵이더라도 이에 적용되는 기믹을 다르게 하여 플레이어에게 재미를 주려고 하였습니다. 

<br><br><br><br>

## 4. 🛠 기술 구현

> 빠른 이동: [P2P 매칭](#p2p-매칭-시스템) · [네트워크 전송 계층](#네트워크-전송-계층-udp--steam) · [UDP 통신](#udp-기반-p2p-실시간-대전-환경-구현) · [스킬 시스템](#스킬-애니메이션-시스템) · [맵 기믹](#전략-패턴을-활용한-맵-기믹-교체)

---

### P2P 매칭 시스템

본 프로젝트는 **서버리스(Serverless) 구조**를 기반으로,  
**GameLift는 매칭만 담당**하고 실제 게임 통신은 **클라이언트 간 P2P**로 이루어지도록 설계했습니다.

![Image](https://github.com/user-attachments/assets/229724f3-c8f3-459c-8481-e3c841d51c67)

#### 매칭 시스템 흐름

1. **플레이어 접속 정보 등록**  
   매칭 요청 전, 각 플레이어는 자신의 `IP / Port / Nickname / Steam` 정보 등  
   AWS Lambda를 통해 DynamoDB에 저장합니다.  
   저장된 데이터는 TTL을 적용하여 일정 시간이 지나면 자동 삭제됩니다.

2. **GameLift 매칭 요청**  
   각 클라이언트는 고유한 `PlayerId`를 포함해 GameLift에 매칭을 요청합니다.  
   GameLift는 1:1 매칭이 성사될 때까지 티켓 기반으로 매칭 상태를 관리합니다.

3. **매칭 완료 이벤트 처리**  
   매칭이 완료되면 GameLift는 SNS 이벤트를 발행하며,  
   이를 구독 중인 Lambda가 매칭된 `matchId`와 각 플레이어의 `PlayerId`를  
   DynamoDB에 저장합니다.

4. **상대 플레이어 정보 조회**  
   클라이언트는 **자신의 PlayerId만 전달**하여 Lambda를 호출하고,  
   내부적으로 매칭 정보를 조회한 뒤 상대방의  
   `IP / Port / Nickname / Steam` 정보를 전달받습니다.

5. **P2P 연결 시작**  
   전달받은 네트워크 정보를 기반으로  
   클라이언트 간 **직접 P2P 통신**을 시작하여 실시간 대전을 진행합니다.
   이때 NAT 환경에 따라 **UDP Hole Punching** 또는 **Steam P2P(릴레이 포함)**를 선택하여 연결합니다.
   
---

### 네트워크 전송 계층 (UDP / Steam)

본 프로젝트는 다양한 NAT 환경에서 안정적인 1:1 대전을 제공하기 위해  
**UDP 기반 P2P**와 **Steam 기반 P2P(SteamNetworkingSockets)**를 모두 지원합니다.

- **UDP P2P**  
  지연을 최소화하는 것을 최우선으로 하며, 대다수 NAT 환경에서 Hole Punching을 통해 직접 연결을 시도합니다.

- **Steam P2P (릴레이 포함)**  
  **Symmetric NAT** 등 UDP Hole Punching이 실패할 가능성이 높은 환경에서는  
  SteamNetworkingSockets의 **P2P 연결 및 Steam Relay 경로**를 사용하여 연결 성공률을 확보합니다.

> 핵심: **게임 메시지 포맷/처리는 동일**하고, “전송 경로(Transport)”만 UDP 또는 Steam으로 분기됩니다.

---

### UDP 기반 P2P 실시간 대전 환경 구현

본 프로젝트의 실시간 대전은 **UDP 기반 P2P 통신**을 통해 구현되었습니다.  
서버를 경유하지 않고 클라이언트 간 직접 통신을 사용함으로써,  
입력 지연을 최소화하고 빠른 반응성을 확보하는 것을 목표로 했습니다.

<br>

#### UDP 선택 배경

실시간 액션 게임 특성상, 패킷의 **완전한 신뢰성**보다  
**지연 최소화와 지속적인 상태 동기화**가 더 중요하다고 판단했습니다.  
UDP는 패킷 손실 가능성이 존재하지만, 잦은 상태(State) 전송을 통해  
자연스럽게 보정할 수 있어 실시간 대전에 적합하다고 판단했습니다.

<br>

#### Sender / Handler / Dispatcher 구조

UDP 통신 로직은 역할에 따라 명확히 분리되어 있습니다.

- **Sender**  
  `FixedUpdate` 주기로 플레이어의 상태(State)를 지속적으로 전송합니다.  
  위치, 방향, 입력, 애니메이션 상태 등 프레임 단위로 갱신되는 정보를 포함하며,  
  최초 연결 시에는 Hole Punching을 위한 초기 패킷을 송신합니다.

- **Handler (Receiver)**  
  UDP 소켓으로 수신되는 모든 패킷의 단일 진입점 역할을 하며,  
  수신된 메시지를 **Prefix 기반으로 분류**한 뒤, 등록된 각 Handler로 전달합니다.

- **Dispatcher**  
  게임 시작 전에 `P2PStateHandler`, `DamageHandler`, `SkillExecuteHandler` 등  
  메시지 타입별 핸들러를 **등록/관리**하고, 수신된 메시지를 순회하며  
  `CanHandle(msg)`가 true인 핸들러를 찾아 `Handle(msg)`를 실행합니다.  
  이를 통해 메시지 타입이 추가되더라도 조건 분기 없이 확장할 수 있도록 구성했습니다.

<br>

#### NAT 환경 대응 및 Hole Punching

대부분의 플레이어는 NAT(Network Address Translation) 환경에 위치해 있어,  
외부에서 로컬 IP/Port로 직접 접근할 수 없는 구조를 가집니다.

이를 해결하기 위해 **UDP Hole Punching 방식**을 적용했습니다.

1. 매칭 완료 후, 양측 플레이어는 서로의 공인 IP / Port 정보를 획득  
2. 양측이 거의 동시에 상대방에게 UDP 패킷을 송신  
3. NAT 장비에서 포트 매핑이 생성되며 양방향 통신 가능 상태가 됨  

Hole Punching은 연결이 성립된 이후에는 더 이상 필요하지 않지만,  
실제로는 로딩 완료 타이밍이 플레이어마다 다를 수 있기 때문에  
**양쪽 클라이언트가 서로의 패킷을 수신할 때까지 일정 시간 동안 Hole Punching 패킷을 반복 송신**합니다.  
상호 수신이 확인되면 Hole Punching 송신을 중단하고, 이후에는 `FixedUpdate` 기반 State 패킷만 전송합니다.

<br>

#### 연결 상태 유지 전략

본 프로젝트에서는 별도의 **KeepAlive / Heartbeat 패킷을 사용하지 않습니다.**

대신,
- `FixedUpdate` 주기로 전송되는 **State 패킷 자체를 연결 상태 판단 기준**으로 활용
- 일정 시간 동안 State 패킷 수신이 없을 경우 연결 이상으로 판단합니다.

이를 통해:
- 패킷 종류를 최소화하고
- 불필요한 네트워크 트래픽을 줄이며
- 실시간 게임 특성에 자연스럽게 부합하는 구조를 구현했습니다.

---

### Steam 기반 P2P 연결 구현 (NAT 우회)

UDP Hole Punching이 동작하지 않는 일부 NAT 환경(특히 **Symmetric NAT**)에서는  
클라이언트 간 직접 UDP 통신이 성립되지 않을 수 있습니다.

이를 보완하기 위해 본 프로젝트는 **SteamNetworkingSockets 기반 P2P 연결**을 지원합니다.

#### Steam 사용 시 특징

- **전송 계층만 변경 (메시지/핸들러 공통)**  
  Sender/Dispatcher/Handler 구조 및 메시지 Prefix 프로토콜은 그대로 유지하고,  
  패킷을 보내는 “통로”만 UDP 소켓 대신 SteamNetworkingSockets로 전환합니다.

- **Hole Punching/ACK 루틴 미사용**  
  Steam 연결은 내부적으로 라우팅(필요 시 릴레이 포함)을 설정한 뒤 `Connected` 상태가 되면  
  곧바로 송수신이 가능합니다.  
  따라서 UDP에서 사용하던 Hole Punching/ACK 기반 `IsReadyToStartGame` 루틴은 Steam 모드에서는 사용하지 않습니다.

- **Host / Client 역할 고정**  
  PlayerNumber를 기반으로 역할을 결정합니다.  
  - `PlayerNumber == 1` → Host (Listen + Accept)  
  - `PlayerNumber == 2` → Client (ConnectP2P)

#### Steam 연결 흐름 요약

1. 매칭 결과로 상대의 `SteamID64`를 교환  
2. Host는 `ListenSocketP2P`를 열고 Client 접속을 대기  
3. Client는 `ConnectP2P(opponentSteamId)`로 연결 시도  
4. `Connected` 상태 진입 시 게임 시작 조건을 만족(Ready)  
5. 이후는 UDP와 동일하게 `FixedUpdate` 주기의 State 패킷 및 이벤트 메시지를 송수신  

> Steam 모드에서의 “Ready”는 UDP의 Hole Punching 성공 여부가 아니라  
> **Steam 연결 상태(Connected)** 를 기준으로 판단합니다.

#### Steam 모드에서의 연결 종료 / 재매칭 안정성

SteamNetworkingSockets는 씬 전환만으로 자동 정리되는 것을 전제로 하지 않으므로,  
재매칭/씬 전환/Disconnect 시점에 **명시적으로 연결을 종료**하여 안정성을 확보했습니다.

- `CloseConnection` / `CloseListenSocket`를 호출하여 리소스를 정리  
- `static` 매니저 구조에서도 재연결 시 항상 `Dispose → Init` 순서로 초기화

<br>

#### 설계 요약

- 서버리스 매칭 + 클라이언트 간 P2P 통신 기반 실시간 대전
- **UDP P2P**: 지연 최소화, Hole Punching으로 NAT 대응
- **Steam P2P**: Symmetric NAT 등에서 연결 성공률 확보 (릴레이 포함)
- Sender / Dispatcher / Handler 구조는 **공통 유지**, 전송 계층만 전환
- UDP는 State 패킷 기반 연결 유지, Steam은 Connected 상태 기반 Ready 처리
- 재매칭/씬 전환 시 명시적 Dispose로 안정성 확보

---

### 스킬 애니메이션 시스템
* **스킬 애니메이션 시스템 (Template Method Pattern)**
  <br>
  60종이 넘는 방대한 스킬 데이터를 효율적으로 관리하고, 카드 획득 시의 다양한 연출을 유연하게 처리하기 위해 데이터 기반 설계와 디자인 패턴을 결합했습니다.
  
  * **Data-Driven Architecture:** 스킬의 속성(아이콘, 쿨타임, 타입)을 `ScriptableObject`로 분리하여 코드 수정 없이 데이터만으로 콘텐츠 확장이 가능하도록 설계했습니다.
  * **Smart Filtering & Acquisition:** 런타임에 플레이어의 보유 스킬을 체크(`IsSkillOwned`)하여 중복을 방지하고, 선택된 스킬을 즉시 인스턴스화(`CreateSkillInstance`)하여 메모리 낭비를 최소화했습니다.
  * **Template Method Pattern (UI):** 카드마다 제각각인 연출을 관리하기 위해 부모 클래스(`CardAnimationBase`)가 생명주기(위치 저장/복구, 트윈 제거)를 관리하고, 자식 클래스가 구체적인 연출(`ExecuteAnimation`)을 담당하는 구조를 적용했습니다.

  ```mermaid
   classDiagram
      SkillCardController --> SkillCard_SO : 1. Load Data
      SkillCardController --> PlayerAbility : 2. Equip Skill
      SkillCardController ..> SkillCard_UI : 3. Control UI
      
      SkillCard_UI --> ICardAnimation : Strategy
      ICardAnimation <|.. CardAnimationBase : Implements
      CardAnimationBase <|-- CardAnimation_Num_1 : Inherits

      class SkillCardController{
          +List~SkillCard_SO~ allSkills
          -IsSkillOwned()
          -CreateSkillInstance()
      }

      class CardAnimationBase{
          +StartAnimation()
          +StopAnimation()
          #ExecuteAnimation()*
          -SaveOriginalPositions()
      }

      class PlayerAbility{
          +SetSkill()
          +EquipPassive()
      }

    
<br><br><br>

---

### 전략 패턴을 활용한 맵 기믹 교체
* **전략 패턴을 활용한 맵 기믹 교체**

  12지신 컨셉에 맞춰 12종의 서로 다른 맵 패턴(기믹)을 구현해야 했습니다. 복잡한 if-else 분기 대신 전략 패턴(Strategy Pattern)을 활용하여 유지보수성과 확장성을 확보했습니다.
  
  * **Abstraction:** 모든 기믹이 상속받는 `AbstractMapGimic` 추상 클래스를 정의하여 공통 규격(Start, Update, End)을 통일했습니다.
  * **Decoupling:** `MapManager`는 구체적인 기믹 내용(쥐, 소, 호랑이 등)을 알 필요 없이, 현재 설정된 `currentGimmick`만 실행하면 되도록 설계했습니다.
  * **OCP (Open-Closed Principle):** 새로운 기믹을 추가할 때 기존 매니저 코드를 수정할 필요 없이, 단순히 새로운 기믹 클래스를 추가하기만 하면 되는 유연한 구조를 완성했습니다.

  ```mermaid
  classDiagram
      MapManager --> AbstractMapGimic : 1. Updates Current Gimmick
      AbstractMapGimic <|-- MapGimic_Rat : Inherits
      AbstractMapGimic <|-- MapGimic_Cow : Inherits
      AbstractMapGimic <|-- MapGimic_Dragon : Inherits

      class MapManager{
          -List~AbstractMapGimic~ gimmicks
          +SetMapGimicIndex(index)
          -FixedUpdate()
      }
      class AbstractMapGimic{
          +OnGimicStart()
          +OnGimmickUpdate()
          +OnGimicEnd()
      }
