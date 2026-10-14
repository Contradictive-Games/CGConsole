# CG Console Changelog

## [Unreleased]

## [v0.8.0] - 2026-10-12

### Added
- `CommandTarget` option on `[ConsoleCmd]` (`First` / `All`)
- Automatic command discovery: no more `ConsoleCommands.RegisterCommandsFrom()`

### Changed
- **Breaking:** namespace `ContradictiveGames.CGConsole` → `CG.Console`
- Default commands are now static

### Fixed
- Package settings not saving changes

[Unreleased]: https://github.com/Contradictive-Games/CGConsole/compare/v0.8.0...HEAD
[v0.8.0]: https://github.com/Contradictive-Games/CGConsole/compare/v0.7...v0.8.0


## [v0.7.260105] - 2026-01-05

This release focuses primarily on being able to customize CGConsole to your needs, as well as adding a few QoL improvements and features to help with custom command creation and debugging.

### Features

- Add project settings tab to custom CGConsole behaviors
- Add default commands that can be selectively enabled/disabled
- Command suggestion box when typing commands into console input field
- Adding `?` to end of command logs description and required arguments for a ConsoleCommand


### Fixes/Improvements

- Command naming validation through Regex
- Users can define custom Regex string for naming validation
- Commands can be hidden from being listed in default `help` command
- Commands can be hidden from suggestions list
- Users can toggle command regisration logs
- Users can toggle requirement for any class trying to register a `ConsoleCommand` needing to implement `ICommandProvider` interface
- Update `Samples` to better fit new release
- Better command registration handling

### Misc. Changes

- Add `ResponseType.Info` to `CommandResponse`
- Update `package.json`
- Update documentation to fit new release


## [v0.6.251230] - 2025-12-130

This marks the second release of CGConsole.

### Features

- The `[ConsoleCmd]` attribute can now be used within a class
- `ConsoleCommand` can now have null targets with static methods, allowing for default commands to be created as well


### Fixes/Improvements

- `CGConsoleCommands` now caches and stores `ConsoleCommand` data better
- Command registration logs can be turned on/off in the `CGConsoleCommands` class
- `CommandResponse` will now log an `Assert` or `Error` based on the response type
- Update `Samples` and demo scripts to better fit new structure



### Misc. Changes

- `ConsoleCommandRegistry` renamed to `CGConsoleCommands`
- Root namespace change to `CG.Console` from `CGConsole`
- The `Console` class is no longer an abstract, and is a concrete MonoBehavior that can be added to any GameObject within the game


## [v0.5] - 2025-12-29

This marks the first release of CGConsole.

### Features

- `CGConsole` was created, allowing users to:
    - Create custom console commands by using the attribute: <br>
    `ConsoleCmd("name", "optional-description")]`
    - Create a custom console by inheriting the `Console` class
    - Create a custom theme for the console by using the `ConsoleSettings` ScriptableObject
    - Type in custom commands by the cmd name into an InputField and pressing enter
    - Execute a custom command by calling: <br> `ConsoleCommandRegistry.TryExecute("cmd-name");`