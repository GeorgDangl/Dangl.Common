# Dangl.Common
[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=Dangl.Common.Tests)](https://jenkins.dangl.me/job/Dangl.Common.Tests/)
[![NuGet](https://img.shields.io/nuget/v/Dangl.Common.svg)](https://www.nuget.org/packages/Dangl.Common)
[![MyGet](https://img.shields.io/myget/dangl/v/Dangl.Common.svg)]()

[![Built with Nuke](http://nuke.build/rounded)](https://www.nuke.build)  

This library contains common, shared functionality.

[Link to documentation](https://docs.dangl-it.com/Projects/Dangl.Common)  
[Changelog](./CHANGELOG.md)  

## CI Builds

CI builds are available via MyGet

    https://www.myget.org/F/dangl/api/v3/index.json

## Compatibility

This project targets both `netstandard1.3` and `net45`. Due to .Net 4.5.2 being the currently latest supported version
by Microsoft and the xUnit test suite, no tests are run for `net45` and `net451`.

## Classes

#### BindableBase
Base class for property binding with INotifyPropertyChanged.

#### StringEncryptionExtensions
Encryption and decryption methods using AES and PBKDF2.

#### StringExtensions
* Sanitize method to normalize line endings to current environments default and to also trim whitespaces at each line end
* ToBase64 and FromBase64 methods
* Compress / Decompress methods using GZip and returning Base64 output

#### TrulyObservableCollection

Collection that notifies of item changes (add, delete) as well as whenever a child item that implements INotifyPropertyChanged is changed.

## Supported Frameworks

The library supports `netstandard1.3`, `netstandard2.0` as well as `net45`. Binaries for the full framework are separately generated for older build tools that do not properly integrate with .NET Standard.
If supported by the tooling (Visual Studio 2017 or the dotnet CLI should be fine), it's advised to use the `netstandard1.3` target.
When using .NET Standard, all features should be available on **Windows**, **Linux** and **Mac OS**, but unit and integration tests are only performed for the following frameworks on **Windows**:
  - `netcoreapp2.1`
  - `netcoreapp2.0`
  - `net461`
  - `net46`
  - `net47`
  - `net452`

There are no known issues with other configurations, but neither is their functionality tested.

---

[MIT License](LICENSE.md)
