using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace Eshava.Core.Models
{
	public class ResponseData<T>
	{
		public ResponseData()
		{
			StatusCode = (int)HttpStatusCode.OK;
		}

		public ResponseData(T data) : this()
		{
			Data = data;
		}

		public ResponseData(T data, HttpStatusCode statusCode)
		{
			Data = data;
			StatusCode = (int)statusCode;
		}

		public bool IsFaulty { get; private set; }

		public T Data { get; set; }

		public string Message { get; private set; }

		[JsonIgnore]
		public string RawMessage { get; private set; }

		[JsonIgnore]
		public Exception Exception { get; private set; }

		public IEnumerable<ValidationError> ValidationErrors { get; private set; }

		public int StatusCode { get; private set; }

		public ResponseData<U> ConvertTo<U>()
		{
			return new ResponseData<U>
			{
				IsFaulty = IsFaulty,
				Message = Message,
				RawMessage = RawMessage,
				Exception = Exception,
				StatusCode = StatusCode,
				ValidationErrors = ValidationErrors
			};
		}

		/// <summary>
		/// Adds a validation error to the current response data instance
		/// </summary>
		/// <param name="validationError"></param>
		/// <returns></returns>
		public ResponseData<T> AddValidationError(ValidationError validationError)
		{
			var validationErrors = ValidationErrors?.ToList() ?? [];
			validationErrors.Add(validationError);

			ValidationErrors = validationErrors;

			return this;
		}

		/// <summary>
		/// Adds a validation error to the current response data instance
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="errorType"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ResponseData<T> AddValidationError(string propertyName, string errorType, object value = null, string methodType = null, string propertyNameFrom = null, string propertyNameTo = null)
		{
			return AddValidationError(new ValidationError
			{
				PropertyName = propertyName,
				PropertyNameFrom = propertyNameFrom,
				PropertyNameTo = propertyNameTo,
				ErrorType = errorType,
				MethodType = methodType,
				Value = value
			});
		}

		public static ResponseData<T> CreateFaultyResponse(string message, IEnumerable<ValidationError> validationErrors = null, int statusCode = 400)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = message,
				StatusCode = statusCode,
				ValidationErrors = validationErrors
			};
		}

		public static ResponseData<T> CreateFaultyResponse(string message, string rawMessage = null, IEnumerable<ValidationError> validationErrors = null, int statusCode = 400)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = message,
				RawMessage = rawMessage,
				StatusCode = statusCode,
				ValidationErrors = validationErrors
			};
		}

		public static ResponseData<T> CreateFaultyResponse(string message, Exception exception = null, IEnumerable<ValidationError> validationErrors = null, int statusCode = 400)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = message,
				RawMessage = exception?.Message,
				Exception = exception,
				StatusCode = statusCode,
				ValidationErrors = validationErrors
			};
		}

		public static ResponseData<T> CreateFaultyResponse<T1>(ResponseData<T1> responseData, IEnumerable<ValidationError> validationErrors = null, int? statusCode = null)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = responseData.Message,
				RawMessage = responseData.RawMessage,
				Exception = responseData.Exception,
				StatusCode = statusCode ?? responseData.StatusCode,
				ValidationErrors = validationErrors ?? responseData.ValidationErrors
			};
		}
	}
}