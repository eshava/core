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

		public Guid? MessageGuid { get; private set; }

		[JsonIgnore]
		public string RawMessage { get; private set; }

		[JsonIgnore]
		public Exception Exception { get; private set; }

		public IEnumerable<ValidationError> ValidationErrors { get; private set; }

		public int StatusCode { get; private set; }

		/// <summary>
		/// Copies the data of this faulty response data instance into a new instance of a different type
		/// </summary>
		/// <typeparam name="U"></typeparam>
		/// <returns></returns>
		public ResponseData<U> ConvertTo<U>()
		{
			return new ResponseData<U>
			{
				IsFaulty = IsFaulty,
				Message = Message,
				MessageGuid = MessageGuid,
				RawMessage = RawMessage,
				Exception = Exception,
				StatusCode = StatusCode,
				ValidationErrors = ValidationErrors,
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

		/// <summary>
		/// Creates a faulty response data instance
		/// </summary>
		/// <param name="message"></param>
		/// <param name="validationErrors"></param>
		/// <param name="statusCode"></param>
		/// <param name="messageGuid"></param>
		/// <returns></returns>
		public static ResponseData<T> CreateFaultyResponse(string message, IEnumerable<ValidationError> validationErrors = null, int statusCode = (int)HttpStatusCode.BadRequest, Guid? messageGuid = null)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = message,
				MessageGuid = messageGuid,
				StatusCode = statusCode,
				ValidationErrors = validationErrors
			};
		}

		/// <summary>
		/// Creates a faulty response data instance
		/// </summary>
		/// <param name="message"></param>
		/// <param name="rawMessage"></param>
		/// <param name="validationErrors"></param>
		/// <param name="statusCode"></param>
		/// <param name="messageGuid"></param>
		/// <returns></returns>
		public static ResponseData<T> CreateFaultyResponse(string message, string rawMessage, IEnumerable<ValidationError> validationErrors = null, int statusCode = (int)HttpStatusCode.BadRequest, Guid? messageGuid = null)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = message,
				MessageGuid = messageGuid,
				RawMessage = rawMessage,
				StatusCode = statusCode,
				ValidationErrors = validationErrors
			};
		}

		/// <summary>
		/// Creates a faulty response data instance
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception">Message set to <see cref="RawMessage"/></param>
		/// <param name="validationErrors"></param>
		/// <param name="statusCode"></param>
		/// <param name="messageGuid"></param>
		/// <returns></returns>
		public static ResponseData<T> CreateFaultyResponse(string message, Exception exception, IEnumerable<ValidationError> validationErrors = null, int statusCode = (int)HttpStatusCode.InternalServerError, Guid? messageGuid = null)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = message,
				MessageGuid = messageGuid,
				RawMessage = exception.Message,
				Exception = exception,
				StatusCode = statusCode,
				ValidationErrors = validationErrors
			};
		}

		/// <summary>
		/// Creates a faulty response data instance for status code <see cref="HttpStatusCode.InternalServerError"/>
		/// </summary>
		/// <param name="message">Message set to <see cref="Message"/></param>
		/// <param name="exception">Message set to <see cref="RawMessage"/></param>
		/// <param name="messageGuid">Guid set to <see cref="MessageGuid"/></param>
		/// <returns></returns>
		public static ResponseData<T> CreateInternalServerError(string message, Exception exception, Guid? messageGuid = null)
		{
			return CreateFaultyResponse(message, exception, statusCode: (int)HttpStatusCode.InternalServerError, messageGuid: messageGuid);
		}

		/// <summary>
		/// Copies the data of a faulty response data instance into a new instance of a different type
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <param name="responseData"></param>
		/// <param name="validationErrors">Override data of <paramref name="responseData"/></param>
		/// <param name="statusCode">Override data of <paramref name="responseData"/></param>
		/// <param name="messageGuid">Override data of <paramref name="responseData"/></param>
		/// <returns></returns>
		public static ResponseData<T> CreateFaultyResponse<T1>(ResponseData<T1> responseData, IEnumerable<ValidationError> validationErrors = null, int? statusCode = null, Guid? messageGuid = null)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = responseData.Message,
				MessageGuid = messageGuid ?? responseData.MessageGuid,
				RawMessage = responseData.RawMessage,
				Exception = responseData.Exception,
				StatusCode = statusCode ?? responseData.StatusCode,
				ValidationErrors = validationErrors ?? responseData.ValidationErrors
			};
		}
	}
}