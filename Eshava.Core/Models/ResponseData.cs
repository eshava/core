﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eshava.Core.Models
{
	public class ResponseData<T>
	{
		public ResponseData()
		{

		}

		public ResponseData(T data)
		{
			Data = data;
		}

		public bool IsFaulty { get; set; }

		public T Data { get; set; }

		public string Message { get; set; }

		[JsonIgnore]
		public string RawMessage { get; set; }
		[JsonIgnore]
		public Exception Exception { get; set; }

		public IList<ValidationError> ValidationErrors { get; set; }

		public int StatusCode { get; set; }

		public ResponseData<U> ConvertTo<U>()
		{
			return new ResponseData<U>
			{
				IsFaulty = IsFaulty,
				Message = Message,
				RawMessage = RawMessage,
				StatusCode = StatusCode,
				ValidationErrors = ValidationErrors
			};
		}

		public static ResponseData<T> CreateFaultyResponse(string message, string rawMessage = null, IList<ValidationError> validationErrors = null, int statusCode = 400)
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

		public static ResponseData<T> CreateFaultyResponse<T1>(ResponseData<T1> responseData, IList<ValidationError> validationErrors = null, int? statusCode = null)
		{
			return new ResponseData<T>
			{
				IsFaulty = true,
				Message = responseData.Message,
				RawMessage = responseData.RawMessage,
				StatusCode = statusCode ?? responseData.StatusCode,
				ValidationErrors = validationErrors ?? responseData.ValidationErrors
			};
		}
	}
}