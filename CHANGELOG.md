# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.4.1] - 2024-05-15

### Fixed

- Bug of grenade effect in client.

## [1.4.0] - 2024-05-15

### Added

- Grenade effect in client.
- Grenade explosion victim in server.

## [1.3.2] - 2024-05-13

### Fixed

- Tick never goes.

## [1.3.1] - 2024-05-13

### Fixed

- Client crash when players abandon items.
- Severe performance impact.

## [1.3.0] - 2024-05-12

### Added

- Added grenade explosion events in record files.

### Changed

- Change the WALL_GENERATE_TRY_TIMES from 1024 to 2048

## [1.2.0] - 2024-05-12

### Added

- Added token display in client.
- Added grenade audio.

### Fixed

- Fixed bug that player can tp in tick 200.

## [1.1.0] - 2024-05-11

### Added

- Added new features to the beam.
- Updated audio effects in the client.
- Implemented functionality to destroy held items in the server.

### Changed

- Improved sound effects in the game.
- Optimized some game logic.

### Fixed

- Fixed issues in the beam.
- Resolved bugs in the client.
- Fix bugs that player know its enemy position in 200 ticks.

## [1.0.0] - 2024-05-09

### Added

- Result winner ID.
- Prefabs for bullet, armors, and M16.
- Pick up and use grenade cooldown.
- Boost grenade.
- Adjust armor.
- Player on GUI.
- Destroy picked items.
- Add armor and firearm UI.
- Value adjustment.
- Larger beam in client.
- Gun fire animation.
- FaceTo.

### Changed

- Update audio effect in client.
- Modify ticks.
- Adjust log timestamp level to millisecond.

### Fixed

- Bug in client.
- Bugs in camera.
- Typo.

## [0.4.1] - 2024-05-05

### Fixed

- Dockerfile soft link problem.

## [0.4.0] - 2024-05-05

### Added

- Ability to keep old records when running a new game.
- Generation of bullets with weapons.

### Changed

- Refactored to use switch to handle commands.
- Adjusted some log messages for clarity.
- Added padding to obstacle shapes.

### Fixed

- Stopped the server after running for too long seconds.
- Recorded the last tick.
- Closed socket on socket error.

## [0.3.0] - 2024-05-03

### Added

- Allow users to choose a target file to log.
- Option to disable whitelist if it is empty.
- Support for saving results when the game is finished.
- Ability for multiple clients to use the same token.
- Whitelist feature to restrict access.
- Dockerfile for judger.
- Video control fix in documentation.

### Changed

- Updated UI in Unity.
- Adjusted some log messages for clarity.
- Refactored exception capturing mechanism.
- Task creation for each connection when sending.
- Improvement in counting supply.
- Updated documentation.
- Improved CI/CD pipeline for building and documentation.

### Deprecated

- No longer using the `IGameRunner` interface.

### Removed

- Unused `IFormat` code.

### Fixed

- Fixed storing all pages of records.
- Adjusted position of exception handling.
- Closed socket on error to prevent leakage.
- Fixed not waiting for the task to complete.
- Avoided waiting forever when agent crashes.

## [0.2.0] - 2024-04-27

### Changed

- The player can only pick up items within one square of their feet

## [0.1.0] - 2024-04-24

### Added

- Docker image support

### Fixed

- Several problems

## [0.0.2] - 2024-04-18

### Added

- First version of client
- A `config.json` file for user to change some settings

### Changed

- Now new record will not cover old record

### Fixed

- Sometimes player may stuck in wall

## [0.0.1] - 2024-04-15

### Added

- Nothing.

[1.4.0]: https://github.com/thuasta/thuai-7/compare/v1.3.2...v1.4.0
[1.3.2]: https://github.com/thuasta/thuai-7/compare/v1.3.1...v1.3.2
[1.3.1]: https://github.com/thuasta/thuai-7/compare/v1.3.0...v1.3.1
[1.3.0]: https://github.com/thuasta/thuai-7/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/thuasta/thuai-7/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/thuasta/thuai-7/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/thuasta/thuai-7/compare/v0.4.1...v1.0.0
[0.4.1]: https://github.com/thuasta/thuai-7/compare/v0.4.0...v0.4.1
[0.4.0]: https://github.com/thuasta/thuai-7/compare/v0.3.0...v0.4.0
[0.3.0]: https://github.com/thuasta/thuai-7/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/thuasta/thuai-7/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/thuasta/thuai-7/compare/v0.0.2...v0.1.0
[0.0.2]: https://github.com/thuasta/thuai-7/compare/v0.0.1...v0.0.2
[0.0.1]: https://github.com/thuasta/thuai-7/releases/tag/v0.0.1
