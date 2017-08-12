# Dangl.Common
[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=Dangl.Common.Tests)](https://jenkins.dangl.me/job/Dangl.Common.Tests/)

This library contains common utilities.

Currently, there is:

#### BindableBase
Base class for property binding with INotifyPropertyChanged.

#### StringEncryption
Encryption and decryption methods using AES and PBKDF2.

#### StringExtensionMethods
* Sanitize method to normalize line endings to current environments default and to also trim whitespaces at each line end
* ToBase64 and FromBase64 methods
* Compress / Decompress methods using GZip and returning Base64 output

#### TrulyObservableCollection

Collection that notifies of item changes (add, delete) as well as whenever a child item that implements INotifyPropertyChanged is changed.
