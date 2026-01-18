
# 목차
1. [📌 게임 소개](#1-📌-게임-소개)
2. [👥 팀원 소개](#2-👥-팀원-소개)
3. [🕹️ 게임 규칙 및 주요 기능](#3-🕹️-게임-규칙-및-주요-기능)
4. [🛠 기술 구현](#4-🛠-기술-구현)
   * P2P환경 구축
   * UDP 통신
   * 스킬 관리 시스템
   * 전략 패턴을 활용한 맵 기믹 교체


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
| 김규진 | Graphic |<table>
  





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
* **AWS Lambda를 활용한 매칭 시스템**
<br><br><br>





* **UDP 소켓 구현을 통한 끊김 없는 대전 환경 구현**
<br><br><br>





* **스킬 관리 시스템**
<br><br><br>



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

















