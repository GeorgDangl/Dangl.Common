# Dangl.Common
[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=Dangl.Common.Tests)](https://jenkins.dangl.me/job/Dangl.Common.Tests/)
[![NuGet](https://img.shields.io/nuget/v/Dangl.Common.svg)](https://www.nuget.org/packages/Dangl.Common)

This library contains common, shared functionality.

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

---

[MIT License](LICENSE.md)
