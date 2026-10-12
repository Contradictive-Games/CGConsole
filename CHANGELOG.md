# CG Console Changelog

## [v0.8.0] - 2026, Oct. 12

This release is mostly a simplification of previous class naming. Because this will break any older version Public API, this is being bumped to a higher version. There has also be a few upgrades.

### Features

- X

### Fixes/Improvements

- Package no longer imports documentation pictures
- Fix sample package errors
- Samples no longer required
- Simplify names from ContradictiveGames.CGConsole -> CG.Console
- Update README to use new names

### Misc. Changes

- Changed version naming to be more standardized
- Add gitattributes, gitignore, .github folder
- Add bug report template


## [v0.7.260105] - 01-05-2026

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


## [v0.6.251230] - 12-30-2025

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


## [v0.5] - 12-29-2025

This marks the first release of CGConsole.

### Features

- `CGConsole` was created, allowing users to:
    - Create custom console commands by using the attribute: <br>
    `ConsoleCmd("name", "optional-description")]`
    - Create a custom console by inheriting the `Console` class
    - Create a custom theme for the console by using the `ConsoleSettings` ScriptableObject
    - Type in custom commands by the cmd name into an InputField and pressing enter
    - Execute a custom command by calling: <br> `ConsoleCommandRegistry.TryExecute("cmd-name");`