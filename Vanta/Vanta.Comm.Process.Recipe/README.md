# Vanta.Comm.Process.Recipe

## 역할

레거시 `SEQ_RECIPE_DOWNLOAD`에 대응하는 프로세스, 시퀀스 실행 모듈입니다.

## 책임

- 프로세스 런타임
- 시퀀스 실행기
- 시퀀스별 상태 처리
- 장치 명령 연계
- 필요 시 Lua 실행 연동 지점 제공

## 의존 규칙

- `Vanta.Core`, `Vanta.Comm.Contracts`, `Vanta.Comm.Abstractions`를 참조합니다.
- UI나 특정 인프라 구현에 직접 결합하지 않습니다.

## 포팅 대상

레거시 기준:

- `SEQ_RECIPE_DOWNLOAD`
- `SEQ_COMM_MAIN`
- `SEQ_COMM_PROCESS`
- `SEQ_COMM_SEQUENCE`

## 현재 상태

- 모듈 앵커만 생성됨
- 초기 포팅 범위는 시퀀스 실행 컨텍스트와 프로세스 러너 정의
