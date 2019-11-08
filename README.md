# core
Collection of methods, classes and helpers to simplify recurring standard actions

## core.communication
Extension to http, ftp and mail functionality

* Ftp
	* FtpClient based on System.Net.WebRequest (Type: "System.Net.WebRequest");
	* SftpClient based on SSH.NET (Type: "SSH.NET.FTP")
* Http
	* HttpClient based on System.Net.Http.HttpClient
* Mail
	* MailClient based on System.Net.Mail.SmtpClient

## core.io
Extension to encapsulate i/o actions

* Archive engine
    * Zip archive (System.IO.Compression.ZipFile)
* File system engine
	
## core.linq
Extension for dynamic creation of filter and sort queries based on IQueryable

## core.logging
Extension to collect logging information

* Exception logging (Microsoft.Extensions.Logging.Abstractions)
* Data record property modifications

## core.security
Extension to secure data worthy of protection

* Checksum
	* Fletcher's checksum
* Cryptography
	* Rijndael engine (symmetric)
* Hash
	* Password engine

## core.validation
Extension for validation of objects based on default annotation attributes and additional custom annotation attributes

* Uses System.ComponentModel.DataAnnotations
* Validating an object instance
* Creating a set of validation rules based on a class

## project dependency diagram

* view on https://draw.io/