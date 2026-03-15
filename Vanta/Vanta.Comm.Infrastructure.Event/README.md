# Vanta.Comm.Infrastructure.Event

## 역할

레거시 `DLL_FRAME_EVENT`에 대응하는 운영 이벤트 인프라 모듈입니다.

## 책임

- 알람 생성, 해제
- 트렌드 로그 작성
- 운영 로그 작성
- 이벤트 동기화
- IPC, 외부 알림, 상태 전파

## 의존 규칙

- `Vanta.Core`, `Vanta.Comm.Contracts`, `Vanta.Comm.Abstractions`를 참조합니다.
- 장치 드라이버 구현에 직접 의존하지 않습니다.

## 포팅 대상

레거시 기준:

- `DLL_COMM_MAIN`
- 알람 맵 관리
- 트렌드 스레드
- GUI sync, socket, pipe 연동

## 현재 상태

- 모듈 앵커만 생성됨
- 초기 포팅 범위는 알람/로그 서비스 인터페이스의 구현체 정리
