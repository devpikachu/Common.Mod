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
