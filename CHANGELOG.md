# Changelog

All notable changes to **Dangl.Common** are documented here.

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
