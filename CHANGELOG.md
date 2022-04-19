# Changelog

All notable changes to **Dangl.Common** are documented here.

## v1.6.3:
- Fixed a bug when decrypting and / or decompressing long strings with a pattern that resembles random strings, e.g. long Base64 strings. This only applies for .NET 6 runtimes
- Added tests for `net6.0`

## v1.6.2:
- Fixed a bug where `TrulyObservableCollection.Clear()` kept event subscriptions to child elements alive, those are now properly unsubscribed
- Added `TrulyObservableCollection.AddRange()` and `TrulyObservableCollection.InsertRange()`

## v1.6.1:
- Add `net40` as target framework

## v1.6.0:
- Add `ObservableDictionary<TKey, TValue>` class

## v1.5.1:
- Drop tests for `netcoreapp2.2` and add tests for `netcoreapp3.1`
- Add `StringExtensions.WithoutUnprintableCharacters()`

## v1.5.0:
- The generated assemblies now have a strong name. This is a breaking change of the binary API and will require recompilation on all systems that consume this package. The strong name of the generated assembly allows compatibility with other, signed tools. Please note that this does not increase security or provide tamper-proof binaries, as the key is available in the source code per [Microsoft guidelines](https://msdn.microsoft.com/en-us/library/wd40t7ad(v=vs.110).aspx)

## v1.4.7:
- Tests are now also run in Linux
- Drop tests for `netcoreapp2.0`, add tests for `netcoreapp2.2`

## v1.4.6:
- Add `WithoutLinebreaks` to `StringExtensions`

## v1.4.5
- Add `StringHashExtensions` with `ToMd5()` and `ToSha256()`

## v1.4.4
- Add `WithMaxLength()` to `StringExtensions`
- Add `WithMaxAbsoluteValue()` on new `DecimalExtensions`

## v1.4.3
- Add `netstandard2.0` target
- Switch build system to NUKE

## v1.4.2
- Fix NullReferenceException when adding or removing null items to TrulyObservableCollection

## v1.4.1
- Downgrade to netstandard1.3 and net45 for broader compatibility
      
## v1.4.0
- PBKDF2 Iterations in the StringEncryptionExtensions are now configurable

## v1.3.2
- Target NETStandard 1.3
