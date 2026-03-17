# Vanta Porting Init

작성일: 2026-03-14

## 목적

`Comm-Tool` 레거시 MFC/C++ 코드베이스를 `Vanta.xxx.xxx` 네이밍 규칙의 C# 모듈 프로젝트로 분리하고, 단계적으로 포팅 및 리팩토링한다.

이 문서는 초기 모듈 구조, 레거시 매핑, 포팅 순서를 고정하기 위한 시작점이다.

공통 코딩 규칙은 `docs/CODING_RULES.md`를 따른다.

## 레거시 기준 축

원본 `Comm-Tool`은 크게 아래 축으로 나뉜다.

1. `CommDaemon`
   설정 UI, 실행/정지, DB 수정, 트리 기반 운영 화면
2. `DLL_FRAME_ADAPTER`
   DB/공유메모리/태그/정의태그/동적 DLL 라우팅
3. `DLL_FRAME_EVENT`
   알람, 트렌드, 로그, IPC
4. `IF_*`
   장치 인터페이스 DLL
5. `SEQ_*`
   프로세스/시퀀스 DLL
6. `Library`
   공통 상수, 구조체, 유틸리티

## Vanta 초기 모듈 구조

현재 솔루션에는 아래 모듈을 초기 생성한다.

| 프로젝트 | 역할 |
| --- | --- |
| `Vanta.Core` | 공통 기본 타입, 예외, 유틸리티, 시간/결과 모델 |
| `Vanta.Comm.Contracts` | DTO, 설정 모델, 태그/장치/시퀀스 계약 모델 |
| `Vanta.Comm.Abstractions` | 포트/인터페이스, 저장소 계약, 런타임 서비스 계약 |
| `Vanta.Comm.Application` | 유스케이스, 오케스트레이션, 명령/조회 흐름 |
| `Vanta.Comm.Infrastructure.Adapter` | 레거시 `DLL_FRAME_ADAPTER` 대응 계층 |
| `Vanta.Comm.Infrastructure.Event` | 레거시 `DLL_FRAME_EVENT` 대응 계층 |
| `Vanta.Comm.Device.Mitsubishi.PLC.McProtocol` | Mitsubishi PLC MC Protocol 구현 |
| `Vanta.Comm.Device.Mitsubishi.PLC.MxComponent` | Mitsubishi PLC MX Component 드라이버 골격 |
| `Vanta.Comm.Device.Mitsubishi.PLC.CCLink` | Mitsubishi PLC CC-Link 드라이버 골격 |
| `Vanta.Comm.Device.Mitsubishi.PLC.CCLinkIE` | Mitsubishi PLC CC-Link IE 드라이버 골격 |
| `Vanta.Comm.Process.Recipe` | 레거시 `SEQ_RECIPE_DOWNLOAD` 대응 계층 |

UI 프로젝트는 아직 만들지 않는다.

이유:

- `CommDaemon`의 대체 UI는 WPF, WinForms, Avalonia, Web 중 선택지가 남아 있다.
- 먼저 런타임/도메인/인프라 계층을 고정해야 UI 기술 결정 비용이 줄어든다.

## 의존 방향

의존은 아래 방향만 허용한다.

```text
Vanta.Core
  -> Vanta.Comm.Contracts
    -> Vanta.Comm.Abstractions
      -> Vanta.Comm.Application
      -> Vanta.Comm.Infrastructure.Adapter
      -> Vanta.Comm.Infrastructure.Event
      -> Vanta.Comm.Device.Mitsubishi.PLC.McProtocol
      -> Vanta.Comm.Device.Mitsubishi.PLC.MxComponent
      -> Vanta.Comm.Device.Mitsubishi.PLC.CCLink
      -> Vanta.Comm.Device.Mitsubishi.PLC.CCLinkIE
      -> Vanta.Comm.Process.Recipe
```

규칙:

- 인프라 모듈끼리 직접 결합하지 않는다.
- 구체 구현 간 호출이 필요하면 `Abstractions` 계약을 통해 연결한다.
- 레거시의 전역 함수, 매크로, 공유메모리 접근은 Application 또는 Infrastructure 서비스로 치환한다.

## 레거시 매핑 기준

### `Library/Common.h`, `DefineTag.h`, `DefineStruct.h`

이관 방향:

- 상수, 열거형 성격: `Vanta.Comm.Contracts`
- 범용 유틸리티: `Vanta.Core`
- 런타임 인터페이스, 서비스 계약: `Vanta.Comm.Abstractions`

### `DLL_FRAME_ADAPTER`

이관 방향:

- DB 저장소
- 공유메모리 서비스
- 태그 리졸버
- 장치, 프로세스 로더
- 런타임 구성 캐시

대상 프로젝트:

- `Vanta.Comm.Infrastructure.Adapter`

### `DLL_FRAME_EVENT`

이관 방향:

- 알람 서비스
- 트렌드 로그 서비스
- 운영 로그 서비스
- IPC, 동기화 서비스

대상 프로젝트:

- `Vanta.Comm.Infrastructure.Event`

### `IF_MELSEC_RTYPE_G`

이관 방향:

- MELSEC, CC-Link 드라이버
- 블록 읽기, 쓰기
- 태그 입출력
- 레시피 다운로드 명령

대상 프로젝트:

- `Vanta.Comm.Device.Mitsubishi.PLC.McProtocol`
- `Vanta.Comm.Device.Mitsubishi.PLC.MxComponent`
- `Vanta.Comm.Device.Mitsubishi.PLC.CCLink`
- `Vanta.Comm.Device.Mitsubishi.PLC.CCLinkIE`

### `SEQ_RECIPE_DOWNLOAD`

이관 방향:

- 프로세스 런타임
- 시퀀스 실행기
- Lua 연계가 필요하면 별도 어댑터로 분리

대상 프로젝트:

- `Vanta.Comm.Process.Recipe`

## 1차 포팅 원칙

1. UI보다 런타임을 먼저 포팅한다.
2. DB 스키마와 Stored Procedure를 바꾸지 않고 감싸는 방향으로 시작한다.
3. 공유메모리는 바로 제거하지 않고 인터페이스 뒤로 숨긴다.
4. 문자열 기반 태그 API는 초기에 유지하되, 이후 Typed API로 확장한다.
5. 레거시 전역 상태는 `State`, `Registry`, `Snapshot` 객체로 점진 치환한다.
6. 장치 드라이버별 로직은 프로젝트 단위로 격리한다.
7. `??` 문법은 기본적으로 자제한다.
8. 가능하면 람다와 LINQ 사용을 자제하고, 반복문과 명시적 분기문을 우선한다.

## 1차 작업 순서

1. 레거시 분석 문서를 기준으로 모델 목록을 확정한다.
2. `DEVICE_PARAM`, `BLOCK_PARAM`, `TAG_PARAM`, `PROCESS_PARAM`, `SEQUENCE_PARAM`를 C# 계약 모델로 변환한다.
3. Adapter의 조회 흐름을 저장소 인터페이스로 추상화한다.
4. 태그 접근 API를 서비스 인터페이스로 정의한다.
5. MELSEC 장치 읽기, 쓰기 포트를 별도 인터페이스로 분리한다.
6. 시퀀스 실행 컨텍스트를 설계한다.
7. 이후 UI 호스트를 선택한다.

## 보류 항목

- `CommDaemon` 대체 UI 기술 선정
- Lua 유지 여부
- Shared Memory 유지 여부
- IPC 수단 유지 여부
- DB 직접 호출 vs 별도 백엔드 계층 도입 여부

## 참고 문서

- `docs/legacy/Comm-Tool/Comm-Tool.CODE_ANALYSIS.md`
- `docs/CODING_RULES.md`
