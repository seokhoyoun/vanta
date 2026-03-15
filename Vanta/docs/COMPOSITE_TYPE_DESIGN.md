# Composite Type Design

## Goal

Support user-defined structure-like tag types without collapsing everything into a single `DataType` string rule.

Examples:

- `RecipeHeader`
- `AxisStatus`
- `SensorPacket`

## Core Model

### TagDefinition

`TagDefinition` remains the runtime entry point for PLC tags.

Added fields:

- `DataShapeKind`
- `ElementShapeKind`
- `ElementCount`
- `CompositeTypeId`
- `CompositeTypeName`

Interpretation:

- `DataShapeKind = Scalar`
  Primitive scalar value such as `int32`, `float`, `bool`
- `DataShapeKind = Array`
  Repeated value such as `float[4]` or `AxisStatus[6]`
- `DataShapeKind = Composite`
  User-defined structure referenced by `CompositeTypeId` or `CompositeTypeName`

`ElementShapeKind` is used when `DataShapeKind = Array`.

- `ElementShapeKind = Scalar`
  Array of primitive values
- `ElementShapeKind = Composite`
  Array of composite values

### CompositeTypeDefinition

Represents one reusable composite type definition.

Key fields:

- `CompositeTypeId`
- `CompositeTypeName`
- `Version`
- `WordLength`
- `IsEnabled`
- `Fields`

### CompositeFieldDefinition

Represents one field inside a composite type.

Key fields:

- `FieldSequence`
- `CompositeTypeId`
- `FieldName`
- `DataShapeKind`
- `ElementShapeKind`
- `DataType`
- `CompositeTypeName`
- `WordOffset`
- `WordLength`
- `BitOffset`
- `BitLength`
- `ElementCount`
- `DecimalPosition`

## Rules

### Scalar field

Use:

- `DataShapeKind = Scalar`
- `DataType = int16 | uint16 | int32 | float | double | string`

### Array field

Use:

- `DataShapeKind = Array`
- `ElementCount > 0`

Primitive array:

- `ElementShapeKind = Scalar`
- `DataType = float` or another primitive base type

Composite array:

- `ElementShapeKind = Composite`
- `CompositeTypeName` or `CompositeTypeId` references the element structure type
- `ElementCount > 0`

Array length is formal metadata, not only a string suffix.

Nested array-of-array is not part of the current design.

### Composite field

Use:

- `DataShapeKind = Composite`
- `CompositeTypeName` or `CompositeTypeId` references another composite definition

This is how nested structures are expressed.

## Example

`AxisStatus`:

- `CommandPosition : int32`
- `ActualPosition : int32`
- `ServoOn : bool`
- `AlarmCode : uint16`
- `Temperatures : float[4]`
- `AxisHistory : AxisSample[3]`

Possible layout:

- `CommandPosition` at word offset `0`, word length `2`
- `ActualPosition` at word offset `2`, word length `2`
- `ServoOn` at word offset `4`, bit offset `0`
- `AlarmCode` at word offset `5`, word length `1`
- `Temperatures` at word offset `6`, word length `8`, element count `4`
- `AxisHistory` at word offset `14`, word length `AxisSample.WordLength * 3`, element count `3`

## Repository Contract

`IConfigurationRepository` now exposes:

- `GetCompositeTypesAsync()`
- `GetCompositeTypeAsync(int compositeTypeId)`
- `GetCompositeTypeAsync(string compositeTypeName)`

This allows DB-backed configuration loaders to provide composite metadata through the same configuration path as devices, blocks, and tags.

## Current Boundary

Implemented now:

- contract models
- tag reference fields
- configuration repository and adapter snapshot support

Not implemented yet:

- runtime composite encode/decode in `Vanta.Comm.Device.Melsec`
- DB table or stored procedure mapping for composite definitions
- validation that tag `WordLength` matches referenced composite `WordLength`
- JSON or object materialization format for composite values

## Recommended Next Step

Define runtime value format for composite tags before implementing codec logic.

Recommended direction:

- external representation: JSON object string
- internal representation: composite metadata + field codecs + PLC word buffer

Example:

```json
{
  "CommandPosition": 1000,
  "ActualPosition": 998,
  "ServoOn": true,
  "AlarmCode": 0,
  "Temperatures": [32.1, 32.0, 31.9, 32.2],
  "AxisHistory": [
    {
      "Position": 1000,
      "Velocity": 12.5
    },
    {
      "Position": 1010,
      "Velocity": 12.4
    }
  ]
}
```
