# Contributing to CG Console

## Setting up
1. Clone the repo into a Unity 6000.0+ project's `Packages/` (or `Assets/`) folder.
2. Run `git config core.hooksPath .githooks` once.
3. To work on the sample, copy `Samples~/ConsoleDemo` into the `dev/` folder (git-ignored).

## Making changes
- Branch from `main`; open a PR back into `main`.
- Add a bullet under `## [Unreleased]` in `CHANGELOG.md`.
- Commit `.meta` files for every new file or folder.
- Code style is enforced by `.editorconfig`.

## Releasing (maintainers)
1. `bash .github/scripts/release.sh X.Y.Z`
2. `git push --follow-tags`