# Comm-Tool 소스코드 분석

작성일: 2026-03-14

## 1. 개요

`Comm-Tool`은 Windows/MFC 기반의 통신 운영 도구로, 다음 3개 축으로 구성된 구조입니다.

1. `CommDaemon`
   운영자가 장치, 블록, 태그, 프로세스, 시퀀스를 설정하고 실행/정지하는 메인 GUI입니다.
2. `DLL_FRAME_ADAPTER`
   DB/공유메모리/동적 DLL 로딩을 담당하는 중앙 라우터입니다.
3. `IF_*`, `SEQ_*`, `DLL_FRAME_EVENT`
   실제 장치 인터페이스, 프로세스 시퀀스, 알람/이벤트/트렌드 처리를 담당하는 실행 모듈입니다.

즉, 이 프로젝트는 "설정 UI + 런타임 메타데이터/라우팅 계층 + 플러그인형 장치/시퀀스 DLL" 구조로 설계되어 있습니다.

## 2. 솔루션 구성

솔루션 파일: `CommDaemon.sln`

주요 프로젝트:

| 프로젝트 | 역할 |
| --- | --- |
| `CommDaemon` | 메인 실행 프로그램. 트레이 아이콘 기반 운영 UI |
| `TraceWnd` | 별도 로그/트레이스 뷰어 |
| `DLL_FRAME_ADAPTER` | DB 로딩, 공유메모리, DLL 라우팅, 태그 접근 API |
| `DLL_FRAME_APP` | 장치/프로세스 DLL 공통 베이스 프레임워크 |
| `DLL_FRAME_EVENT` | 알람, 로그, 트렌드, 소켓/파이프 동기화 |
| `IF_MELSEC_RTYPE_G` | Mitsubishi/MELSEC 계열 장치 인터페이스 DLL |
| `SEQ_RECIPE_DOWNLOAD` | 프로세스/시퀀스 DLL 예제 또는 커스텀 시퀀스 |
| `IF_TEMPLETE_DLL` | 인터페이스 DLL 템플릿 |
| `SEQ_TEMPLETE_DLL` | 시퀀스 DLL 템플릿 |
| `Library` | 공통 라이브러리 및 외부 연동 코드 집합 |

정적 관찰 결과:

- 전체 파일 수: 574
- 주요 확장자: `.h` 280개, `.cpp` 179개
- 가장 큰 폴더: `Library` 298개 파일, `CommDaemon` 127개 파일

## 3. 아키텍처 요약

### 3.1 계층 구조

```text
CommDaemon (GUI / 운영설정 / 실행제어)
  -> DLL_FRAME_ADAPTER.dll
       -> DB Stored Procedure 호출
       -> Shared Memory 관리
       -> Device/Block/Tag/Process/Sequence 메타데이터 캐시
       -> 개별 장치 DLL / 프로세스 DLL 동적 로딩
  -> DLL_FRAME_EVENT.dll
       -> 알람/이벤트/트렌드/로그/IPC

개별 장치 DLL
  -> IF_MELSEC_RTYPE_G.dll
  -> IF_TEMPLETE_DLL.dll

개별 프로세스 DLL
  -> SEQ_RECIPE_DOWNLOAD.dll
  -> SEQ_TEMPLETE_DLL.dll
```

### 3.2 플러그인 프레임워크

`DLL_FRAME_APP`는 장치/프로세스 플러그인이 상속받는 베이스 클래스를 제공합니다.

- `CDLL_FRAME_MAIN`
  장치 컨트롤러(`CDLL_FRAME_CTRL`)와 프로세스(`CDLL_FRAME_PROCESS`)의 상위 관리자
- `CDLL_FRAME_CTRL`
  장치 단위 런타임. 통신 스레드, 블록/태그 관리, 소켓/시리얼/파이프 지원
- `CDLL_FRAME_BLOCK`
  메모리 블록 단위 읽기/쓰기와 태그 집합 관리
- `CDLL_FRAME_TAG`
  태그 단위 데이터 변환과 I/O 처리
- `CDLL_FRAME_PROCESS`
  프로세스 스레드 및 시퀀스 집합 관리
- `CDLL_FRAME_SEQUENCE`
  개별 시퀀스 로직, Lua 호출, 장치 명령 전송 담당

이 구조 덕분에 장치 인터페이스 DLL과 시퀀스 DLL은 베이스 클래스를 상속해 최소 구현만 추가하면 됩니다.

## 4. 실행 흐름

### 4.1 `CommDaemon` 시작

`CommDaemon`은 `FRAME_WNDCLASS=COMM_FRAME` 단일 인스턴스를 강제합니다.

시작 시 주요 흐름:

1. 기존 `COMM_FRAME` 윈도우 존재 여부 확인
2. Mutex 생성
3. `setup.ini` 기준으로 시스템 설정 로드
4. 메인 프레임 생성 후 숨김 상태로 시작
5. 타이머(`TIMER_INIT_PROGRAM`)로 초기 로딩 수행
6. `DLL_FRAME_ADAPTER.dll`, `DLL_FRAME_EVENT.dll` 로드
7. 장치/블록/프로세스/시퀀스 목록 로드
8. 자동으로 `IDM_TRAY_START` 호출하여 런타임 시작

실행 시작 시:

- `m_pDllAdapter->_START_ALL_DEVICE()`
- `m_pDllEqpEvent->_START_THREAD()`

실행 중지 시:

- `m_pDllAdapter->_ABORT_ALL_DEVICE()`
- `m_pDllEqpEvent->_ABORT_THREAD()`

즉, GUI는 실제 통신을 직접 수행하지 않고 DLL API를 통해 전체 런타임을 제어합니다.

### 4.2 운영 UI 특징

- MFC SDI 기반
- 트레이 아이콘 중심 실행/정지/로그뷰어 호출
- 좌측 트리(`SystemMenuTree`)에서 Device/Block/Process/Sequence 관리
- 다수의 설정 다이얼로그로 DB 업데이트 후 런타임에 반영

## 5. 데이터 모델

`Library/DefineStruct.h`에 핵심 런타임 구조체가 정의돼 있습니다.

| 구조체 | 의미 |
| --- | --- |
| `DEVICE_PARAM` | 장치 설정, 포트/네트워크 정보, DLL명 |
| `BLOCK_PARAM` | 장치 내 메모리 블록 정보 |
| `TAG_PARAM` | 태그 주소, 데이터형, 이벤트그룹, 범위 |
| `DEFINE_TAG` | 논리 태그명과 실제 Linked Tag 매핑 |
| `PROCESS_PARAM` | 프로세스 DLL 정보 |
| `SEQUENCE_PARAM` | 프로세스 내 시퀀스 정보 |
| `SIGNAL_TOWER` | 타워램프 구성 |
| `CURRENT_ALARM` | 현재 활성 알람 정보 |
| `TRACE_DATA` | TraceWnd로 전달되는 로그 데이터 |

핵심 포인트:

- 실제 I/O 태그와 별도로 `DEFINE_TAG` 계층이 존재합니다.
- 따라서 상위 로직은 물리 주소 대신 정의 태그 기반으로 동작할 수 있습니다.
- 태그 데이터는 문자열 중심 API로 표준화되어 있고, 내부에서 정수/실수/비트열 변환을 수행합니다.

## 6. 설정 및 외부 의존성

### 6.1 `setup.ini`

`Library/Common.cpp` 기준으로 프로젝트는 실행 폴더의 `setup.ini`를 읽습니다.

주요 항목:

- `SW_Name`
- `Machine_ID`
- `Database_Addr`
- `Database_Name1`
- `Database_Name2`
- `FTP_*`
- `XGEM_USED`
- `INTERFACE_TYPE`
- `LP_COUNT`
- `PM_COUNT`
- `LP_SLOT_COUNT`
- `PM_SLOT_COUNT`
- `HOST_JOB_COUNT`

중요:

- 현재 저장소 안에서는 `setup.ini` 파일이 보이지 않습니다.
- 즉, 이 프로젝트는 "소스만으로 완전 실행 가능한 형태"가 아니라 배포 환경의 외부 설정 파일에 의존합니다.

### 6.2 DB 연결

ODBC 연결 문자열은 코드에서 직접 생성합니다.

- `Driver={SQL Server};Server=%s,1433;UID=sa;PWD=1234;Database=%s;`

관찰 사항:

- DB 계정과 비밀번호가 코드에 하드코딩돼 있습니다.
- 운영 환경에서는 보안상 취약합니다.

### 6.3 DB 의존 구조

설정 데이터는 파일이 아니라 DB Stored Procedure에서 로드됩니다.

대표 호출:

- `ESP_SELECT_DEVICE_INFO`
- `ESP_SELECT_DEVICE_BLOCK`
- `ESP_SELECT_TAG_LIST`
- `ESP_SELECT_DEFINE_TAG`
- `ESP_SELECT_PROCESS_INFO`
- `ESP_SELECT_PROCESS_SEQUENCE`
- `ESP_SELECT_VID_EVENT_TABLE`
- `ESP_UPDATE_DEVICE_INFO`
- `ESP_UPDATE_DEVICE_BLOCK`
- `ESP_UPDATE_TAG_LIST`
- `ESP_UPDATE_PROCESS_INFO`
- `ESP_UPDATE_PROCESS_SEQUENCE`
- `ESP_UPDATE_SYS_ALARM_ON`

즉, 설정 변경은 대부분 "DB 반영 -> Adapter가 다시 읽어서 런타임 객체 갱신" 방식입니다.

## 7. Adapter 계층 분석

`DLL_FRAME_ADAPTER`는 이 솔루션의 핵심 런타임 허브입니다.

주요 책임:

- `setup.ini` 기반 시스템 설정 로드
- ODBC를 통해 장치/블록/태그/프로세스/시퀀스 메타데이터 로드
- 공유메모리 `[Comm]Shared-CurrentTag` 관리
- 태그 이름/번호/정의태그 lookup
- 동적 DLL 로딩 및 export 함수 호출
- 이벤트 그룹/CEID 매핑 유지
- DB update queue 스레드 처리

내부 캐시:

- `m_listDevice`, `m_mapDeviceID`, `m_mapDeviceName`
- `m_listBlock`, `m_mapBlockSeq`, `m_mapBlockName`
- `m_listTagItem`, `m_mapTagSeq`, `m_mapTagName`
- `m_listProcess`, `m_mapProcessID`, `m_mapProcessName`
- `m_listSequence`, `m_mapSequenceID`, `m_mapSequenceName`
- `m_listDefineTag`, `m_mapDefineTagSeq`

이 계층은 사실상 "메타데이터 저장소 + 런타임 API 게이트웨이" 역할을 합니다.

## 8. Device 인터페이스 DLL 분석

### 8.1 `IF_MELSEC_RTYPE_G`

이 프로젝트는 장치 인터페이스 DLL의 실제 구현 예입니다.

구조:

- `CDRV_COMM_MAIN : CDLL_FRAME_MAIN`
- `CDRV_COMM_CTRL : CDLL_FRAME_CTRL`
- `CDRV_COMM_BLOCK : CDLL_FRAME_BLOCK`
- `CDRV_COMM_TAG : CDLL_FRAME_TAG`

특징:

- `CCCLinkLib`를 사용해 Mitsubishi/CC-Link 계열 통신 수행
- `_CONNECT_DEVICE()`에서 네트워크/스테이션/채널 기반 연결
- `_READ_DEVICE_BIT_MEMORY`, `_WRITE_DEVICE_BIT_MEMORY`
- `_READ_DEVICE_WORD_MEMORY`, `_WRITE_DEVICE_WORD_MEMORY`
- `RECIPE_COMMAND_DOWNLOAD`, `PARAMETER_COMMAND_DOWNLOAD` 명령 처리

즉, 실제 PLC/필드 장치 접근은 이 계층에서 수행됩니다.

### 8.2 인터페이스 확장 방식

새 장치 연동을 추가하려면:

1. `IF_TEMPLETE_DLL`를 복제
2. `CDLL_FRAME_MAIN`, `CDLL_FRAME_CTRL`, `CDLL_FRAME_BLOCK`, `CDLL_FRAME_TAG` 파생 구현
3. `CommDaemon` DB 설정에서 장치 DLL명을 지정

플러그인 방식이라 장치 추가 확장성은 높은 편입니다.

## 9. Process/Sequence DLL 분석

### 9.1 `SEQ_RECIPE_DOWNLOAD`

프로세스 DLL 예제이며 구조는 다음과 같습니다.

- `CSEQ_COMM_MAIN : CDLL_FRAME_MAIN`
- `CSEQ_COMM_PROCESS : CDLL_FRAME_PROCESS`
- `CSEQ_COMM_SEQUENCE : CDLL_FRAME_SEQUENCE`

현재 코드 상태를 보면:

- 스레드 시작/종료 로그는 구현되어 있음
- 시퀀스 본체 `_PROC_SEQUENCE1~4()`는 대부분 비어 있음
- Lua 호출 코드가 주석 또는 최소 상태

따라서 이 프로젝트는 "완성된 업무 로직"보다는 시퀀스 프레임워크/템플릿 성격이 강합니다.

### 9.2 Lua 지원

`CommDaemon/ProcessLua.cpp`와 `DLL_FRAME_APP/DLL_FRAME_LUA.cpp`를 통해 Lua 5.1을 지원합니다.

노출 기능 예:

- linked tag 읽기/쓰기
- enum tag 읽기/쓰기
- trigger tag 처리
- trace 출력
- alarm 설정
- thread/process helper 함수

스크립트 경로 규칙:

- `%RUN_PATH%/PROCESS/SCRIPT/*.lua`

즉, 시퀀스 로직 일부를 C++ 대신 Lua로 외부화하도록 설계돼 있습니다.

## 10. Event/Alarm 계층 분석

`DLL_FRAME_EVENT`는 단순 콜백 DLL이 아니라 별도 런타임 모듈입니다.

주요 기능:

- 알람 발생/해제
- 현재 알람 맵 유지
- 타워램프 제어
- 트렌드 로그 작성
- Always Log 작성
- GUI 태그 동기화
- 소켓 서버/클라이언트 처리
- Named Pipe 기반 동기화
- `WM_COPYDATA` 기반 GUI 알람 전달

공유메모리:

- `[Comm]Shared-EventIndex`
- `[Comm]Shared-UpdateTag`
- `[Comm]Shared-CurrentTag`

특징:

- 주기 스레드 3개(`Event`, `Trend`, `Sync`) 운용
- `FRAME_WNDCLASS` 메인 윈도우에 알람을 `WM_COPYDATA`로 전달
- 일부 로직에서 외부 프로그램 실행(`WinExec`)도 수행

즉, 운영 알람/이벤트 축은 Adapter가 아니라 Event DLL에서 집중 관리합니다.

## 11. Trace/운영 보조 도구

### 11.1 `TraceWnd`

별도 MFC 실행 파일이며 `TRACE_WNDCLASS=COMM_TRACE` 단일 인스턴스를 강제합니다.

역할:

- 로그 출력
- Trace 옵션 설정
- `CommDaemon`에서 `TraceWnd.exe` 실행 가능

운영자가 메인 UI와 로그 뷰를 분리해서 사용할 수 있게 설계돼 있습니다.

## 12. 코드베이스 특징 요약

강점:

- 장치 인터페이스와 프로세스 로직이 플러그인 DLL로 분리돼 확장성이 높음
- DB/공유메모리/이벤트를 중앙 Adapter/Event 계층으로 모아 구조가 비교적 일관됨
- MFC 기반 운영툴로 실제 제조/설비 현장 운영 패턴에 맞는 구조
- Lua 연동으로 일부 시퀀스 로직의 외부화 가능

한계/주의점:

- MFC + 전역 상태 + 메시지 기반 구조라 현대화 난이도가 높음
- DB 비밀번호(`sa/1234`) 하드코딩
- `setup.ini`, DB Stored Procedure, 외부 DLL이 없으면 재현이 어려움
- Windows 전용 아키텍처에 강하게 결합
- `WinExec`, `WM_COPYDATA`, 공유메모리, 전역 매크로 의존도가 높음
- 일부 시퀀스 프로젝트는 템플릿 수준 구현

## 13. 전체 결론

이 프로젝트는 "산업 장비 통신/운영 관리 툴" 성격의 MFC 기반 통합 프레임워크입니다.

핵심 구조는 다음과 같이 이해하면 됩니다.

- `CommDaemon`은 운영 설정/실행 UI
- `DLL_FRAME_ADAPTER`는 메타데이터/태그/API 허브
- `DLL_FRAME_EVENT`는 알람/트렌드/동기화 런타임
- `IF_*`는 실제 장치 통신 플러그인
- `SEQ_*`는 공정/프로세스 시퀀스 플러그인
- `Library`는 공통 통신/DB/UI/유틸 묶음

실제 분석 또는 유지보수 우선순위는 다음이 적절합니다.

1. `CommDaemon`의 사용자 동작이 어떤 DB Stored Procedure를 호출하는지 파악
2. `DLL_FRAME_ADAPTER`의 태그/정의태그 흐름 파악
3. 사용하는 실제 장치 DLL(`IF_*`)의 통신 방식 분석
4. 실제 운영 시퀀스 DLL(`SEQ_*`)과 Lua 스크립트 존재 여부 확인
5. 배포 환경의 `setup.ini`, DB 스키마, Stored Procedure 확보

## 14. 추가로 확인하면 좋은 외부 요소

소스 외부 의존성이 크므로 아래가 확보돼야 실제 전체 동작 분석이 가능합니다.

- 배포본의 `setup.ini`
- SQL Server DB 스키마 및 Stored Procedure 정의
- 운영 중인 실제 `IF_*` / `SEQ_*` DLL 목록
- `PROCESS/SCRIPT` 폴더의 Lua 스크립트
- 장비별 통신 프로토콜 문서
