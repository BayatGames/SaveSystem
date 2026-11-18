# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

- Support Unity 6.6 while retaining the older Editor API paths.
- Preserve asset and prefab reference databases during import. Existing save GUIDs and the legacy integer object and refresh-rate fields are unchanged in format.
- Read 3D physics material type names used both before and after Unity 6. Keep the TextMeshPro sample's legacy wrapping and kerning fields.

## [1.25.8] - 2024-12-26

### Added

- Resolving assets by label & include ignored (by @fitbie)
- Add ignored labels & includeIgnored fields to AssetReferenceResolver editor class (by @fitbie)

### Fixed

- IOR exception when typesPageCount is 0 (by @fitbie)
