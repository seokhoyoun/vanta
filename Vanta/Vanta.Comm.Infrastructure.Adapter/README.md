# Vanta.Comm.Infrastructure.Adapter

## 역할

레거시 `DLL_FRAME_ADAPTER`에 대응하는 인프라 모듈입니다.

## 책임

- DB 조회, 업데이트
- 메타데이터 캐시 관리
- 태그 이름, 시퀀스, 장치 매핑
- 공유메모리 연동
- 레거시 런타임 라우팅 구조의 C# 재구성

## 의존 규칙

- `Vanta.Core`, `Vanta.Comm.Contracts`, `Vanta.Comm.Abstractions`를 참조합니다.
- 다른 인프라 모듈에 직접 결합하지 않습니다.

## 포팅 대상

레거시 기준:

- `DLL_FRAME_ADAPTER`
- `ADP_COMM_MAIN*`
- `ADP_DLL_INTERFACE*`
- DB queue / refresh / config 반영 흐름

## 현재 상태

- 모듈 앵커만 생성됨
- 우선순위는 Stored Procedure 호출 래퍼와 메타데이터 저장소 구현
- 1차 골격 추가 완료:
  `AdapterConfigurationSnapshot`, `IAdapterConfigurationSource`,
  `AdapterConfigurationRepository`, `InMemoryAdapterTagStore`,
  `InMemorySharedMemoryStore`, `AdapterTagValueService`,
  `AdapterRuntimeControlService`, 드라이버/프로세스 팩토리
