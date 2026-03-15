# Vanta.Comm.Contracts

## 역할

통신 도메인에서 공유하는 계약 모델과 DTO를 정의하는 모듈입니다.

## 책임

- 장치, 블록, 태그 모델
- 프로세스, 시퀀스 모델
- 설정 모델
- 알람, 이벤트, 트렌드 관련 전달 모델
- 열거형, 상수 대체 타입

## 의존 규칙

- `Vanta.Core`만 참조합니다.
- 구현 코드나 인프라 세부사항을 포함하지 않습니다.

## 포팅 대상

레거시 기준:

- `DefineStruct.h`
- `DefineTag.h`
- 일부 `Common.h` 상수 정의

## 현재 상태

- 모듈 앵커만 생성됨
- 1차 포팅 대상은 `DEVICE_PARAM`, `BLOCK_PARAM`, `TAG_PARAM`, `PROCESS_PARAM`, `SEQUENCE_PARAM`
- 1차 계약 모델 추가 완료:
  `DeviceDefinition`, `BlockDefinition`, `TagDefinition`, `DefineTagDefinition`,
  `ProcessDefinition`, `SequenceDefinition`, `SignalTowerDefinition`,
  `CurrentAlarm`, `EventKeyDefinition`, `TraceEntry`, `TrendItemDefinition`
- 1차 enum 추가 완료:
  `CommunicationType`, `MemoryKind`, `AddressFormat`, `InterfaceType`,
  `ThreadEnableFlags`, `SignalChannel`, `OperationResultCode`
