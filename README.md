# Ignore

![Build](https://github.com/goelhardik/ignore/workflows/Build/badge.svg?branch=main)
[![codecov](https://codecov.io/gh/goelhardik/ignore/branch/main/graph/badge.svg?token=5YE7LBW8K0)](https://codecov.io/gh/goelhardik/ignore)
[![NuGet](https://img.shields.io/nuget/v/Ignore)](https://www.nuget.org/packages/Ignore)

.gitignore based parser implemented in C# according to the [.gitignore spec 2.29.2](https://git-scm.com/docs/gitignore).

The library is tested against real `git status` outputs. The tests use `LibGit2Sharp` for that.

## Installation

Ignore can be installed from NuGet.

```PowerShell
Install-Package Ignore
```

## Usage

```cs
// Initialize ignore
var ignore = new Ignore();

// Add a rule
ignore.Add(".vs/");

// Add multiple rules
ignore.Add(new[] { "*.user", "obj/*" });

// Add rules fluently
ignore
    .Add(".vs/")
    .Add(new[] { "*.user", "obj/*" });

// Filter paths to exclude paths ignored as per the rules
var filteredFiles = ignore.Filter(new[] { ".vs/a.txt", "x.user", "obj/a.dll" });

// Check if a path is ignored
var isIgnored = ignore.IsIgnored("x.user");
```

## Developing

Ignore targets `netstandard2.0` and `net8.0` for the main library and uses `net6.0` and `net8.0` for the unit tests (Xunit).

### Build

From the root directory

```console
dotnet build
```

### Test

From the root directory

```console
dotnet test
```
