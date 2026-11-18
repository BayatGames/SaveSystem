# Save System

The source code of the Save System for Unity engine.

> This is a paid package for Unity game engine delivered on Unity Asset Store and available here for contributions from the community and a source of education.

## Resources

- [Unity Asset Store](https://assetstore.unity.com/packages/slug/108890)
- [Manual](https://docs.bayat.io/save-system/manual/)
- [API](https://docs.bayat.io/save-system/api/)
- [Integrations](https://docs.bayat.io/save-system/manual/integrations/)
- [Unity Forum Thread](https://forum.unity.com/threads/bayat-save-system-an-ultimate-data-management-solution.817416/)
- [Discord](https://discord.com/invite/HWMqD7T)

## Getting Started

### Prerequisites

- The sample project in this repository is pinned to Unity **6.6** (`6000.6.7f1`).
- The Save System package also runs on older Editors, including Unity **2019.4.40f1**. For a source import, copy `Assets/Plugins/Bayat` and keep your project's existing package manifest and settings. The bundled sample scenes, generated converters, and Asset Store publishing tools are separate from the Save System package.

### Existing saves

The Unity upgrade does not require a save migration. Keep your existing scene, asset, and prefab reference databases when updating or moving between Editors: their Bayat GUIDs link saves to Unity objects. If a previous upgrade emptied a reference database, restore it from a backup or version control. The legacy JSON field `instanceID` still has an integer value; on Unity 6.4 and later it is written as `0` because Unity no longer guarantees that its session-local object IDs fit that field. Save System does not use it to restore references.

Use the updated package on both Editor versions when exchanging saves. For example, it can read both the old and new Unity type names for 3D physics materials.

### Download

- Clone the project:

```bash
git clone https://github.com/BayatGames/SaveSystem.git
```

- [Download the latest release](https://github.com/BayatGames/save-system-unity/releases/latest)
- [Download the latest unity package from Unity Asset Store](https://assetstore.unity.com/packages/slug/108890)

### Build documentation

```
docfx docs/docfx.json
```

### Contribute

> We’re excited that you’re interested in contributing! Take a moment to read [`CONTRIBUTING.md`](contribute).

Open the sample project in the pinned Editor and run the Edit Mode tests in the Unity Test Runner.

## License

[Unity Asset Store Terms and EULA][license] © [Bayat][author]

---

Made with ❤️ by [Bayat][author]

<!-- Definitions -->

[license]: https://unity3d.com/legal/as_terms

[author]: https://bayat.io

[contribute]: https://github.com/BayatGames/.github/blob/main/CONTRIBUTING.md
