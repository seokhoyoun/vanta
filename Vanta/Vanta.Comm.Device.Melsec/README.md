# Vanta.Comm.Device.Melsec

## 역할

레거시 `IF_MELSEC_RTYPE_G` 계열을 C#으로 포팅하는 Mitsubishi MELSEC 장비 드라이버 모듈입니다.

## 책임

- 장비 초기화, 시작, 중지
- 블록 구성 적용
- 태그 구성 적용
- direct tag read/write
- block memory read/write
- MELSEC 통신 클라이언트 추상화

## 의존 규칙

- `Vanta.Core`, `Vanta.Comm.Contracts`, `Vanta.Comm.Abstractions`만 참조합니다.
- `Adapter` 구현에는 직접 의존하지 않습니다.
- 드라이버 생성과 등록은 상위 모듈에서 수행합니다.

## 현재 구현 상태

- `MelsecDeviceDriver` 구현 완료
- `IMelsecCommunicationClient` 추상화 추가
- `MelsecMc3EBinaryClient` 실제 TCP 통신 구현 추가
- `SimulatedMelsecCommunicationClient` 범용 시뮬레이션 메모리 맵 연동 추가
- `InMemoryMelsecCommunicationClient` 단순 테스트용 메모리 구현 유지
- `MelsecAddressParser` 주소 해석기 추가
- 블록/태그 런타임 컨텍스트 추가
- `MelsecDeviceDriverRegistration` 등록 helper 추가
- `MelsecSimulationFactory` 생성 helper 추가

## 현재 가정

- 프로토콜은 Mitsubishi MC Protocol 3E Binary over TCP 기준입니다.
- 장치 연결 정보는 `DeviceDefinition.DeviceIpAddress`, `DeviceDefinition.DevicePort`,
  `DeviceDefinition.NetworkNumber`, `DeviceDefinition.StationNumber`, `DeviceDefinition.Timeout`를 사용합니다.
- 현재 지원 device head는 `X`, `Y`, `M`, `L`, `F`, `V`, `B`, `D`, `W`, `R`, `ZR`,
  `TS`, `TC`, `TN`, `SS`, `SC`, `SN`, `CS`, `CC`, `CN`, `SB`, `SW`, `SM`, `SD`, `DX`, `DY`, `Z`입니다.
- 드라이버 alias는 `MELSEC`, `IF_MELSEC_RTYPE_G`, `MELSEC_RTYPE_G`를 지원합니다.

## 아직 남은 작업

- 실제 현장 장비 기준 device head/주소 규칙 검증
- reconnect, keep-alive, 재시도 정책 보강
- trace, alarm, timeout 상세 처리 연동
- 레거시 `CCCLinkLib` 기반 동작과 차이점 비교 검증
- `Adapter`에서 simulation driver 선택 경로 연결
