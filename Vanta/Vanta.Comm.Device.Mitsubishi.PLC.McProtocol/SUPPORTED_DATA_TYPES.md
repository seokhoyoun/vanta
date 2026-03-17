# Vanta.Comm.Device.Mitsubishi.PLC.McProtocol Supported Data Types

## Scalar

- `bool`
- `int16`
- `uint16`
- `int32`
- `uint32`
- `int64`
- `uint64`
- `float`
- `double`

## Text

- `string[n]`
- `ascii[n]`

## Array

- `int16[4]`
- `uint16[8]`
- `int32[2]`
- `float[4]`
- `double[2]`
- `AxisStatus[6]` using composite metadata

Array values are exchanged as comma-separated strings.

Examples:

- `1,2,3,4`
- `0.1,0.2,0.3,0.4`

## Composite

User-defined composite types are not implemented yet.

What you meant by `complex` is closer to a structure definition such as:

- `RecipeHeader`
- `AxisStatus`
- `SensorPacket`

That kind of type needs a separate metadata model because one logical value must map to multiple fields and multiple PLC words.

Composite arrays are part of the contract design as well, but runtime encode/decode is still pending.

## Notes

- The abstraction boundary still uses `string` tag values.
- The MELSEC driver converts between PLC word memory and typed string values internally.
- Unknown or unspecified data types fall back to raw word lists such as `100,200,300`.
