## 0.5.6
**:sparkles: Features**
- Add events for `AssetsLoaded` and `AssetsFinalized` in `ISystem`

## 0.5.5
**:sparkles: Features**
- Add overrides for `AssetsLoaded` and `AssetsFinalized` in `System`

## 0.5.4
**:bug: Bug Fixes**
- Fix CTD due to a null `Privileges` array

## 0.5.3
**:question: Other**
- Configuration UI tweaks

## 0.5.2
**:bug: Bug Fixes**
- Add guards to config mutators

## 0.5.1
**:sparkles: Features**
- Add `MutateCommon`, `MutateServer` and `MutateClient` to `IConfigSystem`

## 0.5.0
**:recycle: Refactors**
- Major cleanup

## 0.4.2
**:sparkles: Features**
- Update to VS 1.21.5
- Add `InvalidSideException`

## 0.4.1
**:bug: Bug Fixes**
- Fix null `languageCode`

## 0.4.0
**:sparkles: Features**
- Add `ITranslations` and `Translations`
- Make config UI labels and descriptions translatable

## 0.3.2
**:sparkles: Features**
- Add static `Instance` to `System`

## 0.3.1
**:boom: Breaking Changes**
- Remove `SystemSide`, use `EnumAppSide` instead

**:sparkles: Features**
- Add static `Get()` to `System`

## 0.3.0
**:boom: Breaking Changes**
- Remove `Synchronized` event from `IConfigSystem`
- Add `RootConfigType` parameter to `Updated` event in `IConfigSystem`

**:sparkles: Features**
- Overhaul config change detection

## 0.2.6
**:sparkles: Features**
- Added `Updated` event to `IConfigSystem`

**:recycle: Refactors**
- Minor refactors

## 0.2.5
**:bug: Bug Fixes**
- Fix `ConfigPacket` tags

**:question: Other**
- Moved network channel creation to `StartPre`

## 0.2.4
**:question: Other**
- Add explicit tags to packet contract

## 0.2.3
**:bug: Bug Fixes**
- Fixed `IConfigSystem.Synchronized` not being in the interface, making it unusable without direct coupling

## 0.2.2
**:question: Other**
- Shrink input width to accomodate long labels

## 0.2.1
**:bug: Bug Fixes**
- Fixed trying to (de)serialize common or server config without checking for its existence

**:question: Other**
- Remove unnecessary empty space in configuration UI
- Wrap long text in configuration UI

## 0.2.0
**:sparkles: Features**
- Added `Enum` config entry type

**:bug: Bug Fixes**
- Fixed inability to save `Common` and `Server` configs in single player

## 0.1.5
**:recycle: Refactors**
- Made `Common.Mod.System.Container` public

## 0.1.4
**:question: Other**
- Correctly publish `Common.Mod.Generator`

## 0.1.3
**:question: Other**
- Add cross project substitution to fix NuGet reference

## 0.1.2
**:question: Other**
- Correct missing name in `Common.Mod.Common`

## 0.1.1
**:question: Other**
- Correctly publish `Common.Mod.Common`

## 0.1.0
**:question: Other**
- Initial release
