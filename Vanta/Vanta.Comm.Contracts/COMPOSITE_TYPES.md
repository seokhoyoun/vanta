# Vanta.Comm.Contracts Composite Types

Added formal contract models for structure-like user-defined types:

- `DataShapeKind`
- `CompositeTypeDefinition`
- `CompositeFieldDefinition`

These models allow tags to reference reusable composite metadata instead of encoding everything in a single `DataType` string.

Array support includes both:

- array of scalar values
- array of composite values

For array contracts, `ElementShapeKind` distinguishes whether the array element is primitive or composite.

See also:

- [Composite Type Design](C:\git\vanta\Vanta\docs\COMPOSITE_TYPE_DESIGN.md)
