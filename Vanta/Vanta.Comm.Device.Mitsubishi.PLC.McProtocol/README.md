# Vanta.Comm.Device.Mitsubishi.PLC.McProtocol

## Role

This module ports the legacy `IF_MELSEC_RTYPE_G` line into a Mitsubishi PLC driver based on MC Protocol.

## Scope

- device initialize, start, stop
- block configuration apply/remove
- tag configuration apply/remove
- direct tag read/write
- block memory read/write
- MC Protocol communication client abstraction

## Current Implementation

- `McProtocolDeviceDriver`
- `IMelsecCommunicationClient`
- `MelsecMc3EBinaryClient`
- `SimulatedMelsecCommunicationClient`
- `InMemoryMelsecCommunicationClient`
- `MelsecAddressParser`
- `McProtocolDeviceDriverRegistration`
- `McProtocolSimulationFactory`

## Notes

- transport is Mitsubishi MC Protocol 3E Binary over TCP
- supported driver aliases are `MC_PROTOCOL`, `MELSEC`, `IF_MELSEC_RTYPE_G`, `MELSEC_RTYPE_G`
- internal address and codec types keep the `Melsec*` naming because they model MELSEC memory semantics
