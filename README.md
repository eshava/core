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

## core.dynamic.fields
Extension to define and configure dynamic fields that are not hard-coded in a C# class. 	
	
## core.dynamic.fields.validation
Extension for validation of dynamic fields configured rules

* Uses core.dynamic.fields
* Validating an object instance
* Creating a set of validation rules based on dynamic fields	
		
## core.io
Extension to encapsulate i/o actions

* Archive engine
    * Zip archive (System.IO.Compression.ZipFile)
* File system engine
	
## core.linq
Extension for dynamic creation of filter and sort queries based on IQueryable
* WhereQueryEngine
* SortingQueryEngine

Extension for transformation of expression tree from one data type to an other data type
* TransformQueryEngine
* TransformProfile

```csharp
public IEnumerable<Target> ReadSomething(Expression<Func<Source, bool>> sourcExpression)
{
	var targetData = new List<Target>();
	var engine = new TransformQueryEngine();
	
	var targetExpression = engine.Transform<Source, Target>(sourcExpression);
	
	var result = targetData
					.Where(targetExpression.Compile())
					.ToList()
					;
			
	return result;
}
```

```csharp
public enum CustomTransformProfile : TransformProfile
{
	public CustomTransformProfile()
	{
		CreateMap<Source, Target>()
			.ForPath(s => s.SourceId, t => t.TargetId)
			;
	}
}
```

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