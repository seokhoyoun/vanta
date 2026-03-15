# Vanta Region Layout Rule

## Purpose

Use consistent `#region` ordering in non-trivial classes so members are easy to scan and maintain.

## Recommended Order

1. `public properties`
2. `fields`
3. `constructors`
4. `public methods`
5. `private helpers`
6. `events`

## Region Names

Use fixed region names where possible:

- `Public Properties`
- `Fields`
- `Constructors`
- `Public Methods`
- `Private Helpers`
- `Events`

## Notes

- Apply this rule to service classes, forms, drivers, repositories, and other implementation-heavy classes.
- Small DTO, enum, and simple model classes do not need regions if the file is already obvious.
- Do not overuse nested regions.
- Keep the same order even when some sections are empty; omit empty regions instead of adding placeholders.
- New code should follow this rule first. Existing code can be normalized gradually during refactoring.
