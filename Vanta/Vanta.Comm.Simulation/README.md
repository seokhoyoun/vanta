# Vanta.Comm.Simulation

## 역할

장비 종류에 종속되지 않는 범용 메모리 기반 시뮬레이션 모듈입니다.

## 책임

- 가상 장비 메모리 맵 유지
- word/bit read/write
- 초기 메모리 preset 로드
- 시뮬레이션 시나리오 hook 제공

## 의존 규칙

- `Vanta.Core`, `Vanta.Comm.Contracts`, `Vanta.Comm.Abstractions`만 참조합니다.
- 특정 장비 프로토콜을 직접 구현하지 않습니다.

## 현재 구현 상태

- `SimulatedDeviceMemoryMap` 구현 완료
- `DeviceSimulationProfile`, `DeviceSimulationMemoryPreset` 추가
- `IDeviceMemoryMap`, `IDeviceSimulationScenario` 기반 구조 연결

## 사용 방향

- 장비 프로젝트는 이 모듈을 참조하여 vendor-specific simulation client를 만듭니다.
- `Melsec`는 `SimulatedMelsecCommunicationClient`로 이 모듈을 사용합니다.
