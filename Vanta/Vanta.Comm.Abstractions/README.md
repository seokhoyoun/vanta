# Vanta.Comm.Abstractions

## 역할

애플리케이션과 인프라를 연결하는 인터페이스 계층입니다.

## 책임

- 저장소 인터페이스
- 태그 접근 서비스 인터페이스
- 장치 드라이버 인터페이스
- 이벤트, 알람, 로그 서비스 인터페이스
- 공유메모리, IPC 추상화 인터페이스

## 의존 규칙

- `Vanta.Core`, `Vanta.Comm.Contracts`만 참조합니다.
- 구체 구현을 포함하지 않습니다.

## 포팅 대상

레거시의 전역 함수 호출, DLL export 호출, 공유메모리 접근을 인터페이스 뒤로 숨기는 작업의 기준점이 됩니다.

## 현재 상태

- 모듈 앵커만 생성됨
- Adapter, Event, Device, Process 모듈이 구현해야 할 포트를 정의할 예정
- 1차 포트 정의 완료:
  `IConfigurationRepository`, `ITagValueService`, `IRuntimeControlService`,
  `IAlarmService`, `ITraceService`, `ISharedMemoryStore`,
  `IDeviceDriver`, `IProcessRuntime`
