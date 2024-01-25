using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Eshava.Core.Extensions;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Models;

namespace Eshava.Core.Linq.Constants
{
	internal static class ExpressionConstants
	{
		public static readonly ConstantExpression StringNull = Expression.Constant(null, TypeConstants.String);
		public static readonly ConstantExpression ObjectNull = Expression.Constant(null, TypeConstants.Object);
		public static readonly ConstantExpression CompareTo = Expression.Constant(0, TypeConstants.Int);

		private static readonly Dictionary<Type, Func<string, Type, CompareOperator, WhereQueryEngineOptions, ConstantExpression>> _constantExpressions = new()
		{
			{ TypeConstants.Guid, GetConstantGuid },
			{ TypeConstants.String, GetConstantString },
			{ TypeConstants.Bool, GetConstantBoolean },
			{ TypeConstants.Int, GetConstantInteger },
			{ TypeConstants.Long, GetConstantLong },
			{ TypeConstants.Decimal, GetConstantDecimal },
			{ TypeConstants.Double, GetConstantDouble },
			{ TypeConstants.Float, GetConstantFloat },
			{ TypeConstants.DateTime, GetConstantDateTime },
			{ TypeConstants.Enum, GetConstantEnum }
		};

		public static bool ContainsConstantOperation(this Type type)
		{
			return _constantExpressions.ContainsKey(type);
		}

		public static ConstantExpression GetConstantExpression(this Type type, string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			return _constantExpressions[type](value, dataType, compareOperator, options);
		}

		private static ConstantExpression GetConstantGuid(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<Guid>();
				foreach (var item in value.Split('|'))
				{
					if (Guid.TryParse(item, out var valueItemGuid))
					{
						values.Add(valueItemGuid);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Guid.TryParse(value, out var valueGuid))
			{
				return null;
			}

			return Expression.Constant(valueGuid, dataType);
		}

		private static ConstantExpression GetConstantString(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (options.CaseInsensitive ?? false)
			{
				value = value.ToLower();
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = value.Split('|').ToList();

				return Expression.Constant(values, values.GetType());
			}

			return Expression.Constant(value, dataType);
		}

		private static ConstantExpression GetConstantBoolean(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			var boolean = value.ToBoolean();

			return Expression.Constant(boolean, dataType);
		}

		private static ConstantExpression GetConstantDecimal(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<decimal>();
				foreach (var item in value.Split('|'))
				{
					if (Decimal.TryParse(item, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueItemDecimal))
					{
						values.Add(valueItemDecimal);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueDecimal))
			{
				return null;
			}

			return Expression.Constant(valueDecimal, dataType);
		}

		private static ConstantExpression GetConstantDouble(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<double>();
				foreach (var item in value.Split('|'))
				{
					if (Double.TryParse(item, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueItemDouble))
					{
						values.Add(valueItemDouble);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueDouble))
			{
				return null;
			}

			return Expression.Constant(valueDouble, dataType);
		}

		private static ConstantExpression GetConstantFloat(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<float>();
				foreach (var item in value.Split('|'))
				{
					if (Single.TryParse(item, NumberStyles.Float, CultureInfo.InvariantCulture, out var valueItemFloat))
					{
						values.Add(valueItemFloat);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Single.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var valueFloat))
			{
				return null;
			}

			return Expression.Constant(valueFloat, dataType);
		}

		private static ConstantExpression GetConstantInteger(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<int>();
				foreach (var item in value.Split('|'))
				{
					if (Int32.TryParse(item, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueItemInteger))
					{
						values.Add(valueItemInteger);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueInt))
			{
				return null;
			}

			return Expression.Constant(valueInt, dataType);
		}

		private static ConstantExpression GetConstantLong(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<long>();
				foreach (var item in value.Split('|'))
				{
					if (Int64.TryParse(item, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueItemLong))
					{
						values.Add(valueItemLong);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Int64.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueLong))
			{
				return null;
			}

			return Expression.Constant(valueLong, dataType);
		}

		private static ConstantExpression GetConstantEnum(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = value.Split('|').Select(v => Enum.Parse(dataType, v)).ToList();
				try
				{
					return Expression.Constant(values, values.GetType());
				}
				catch
				{
					return null;
				}
			}

			try
			{
				return Expression.Constant(Enum.Parse(dataType, value), dataType);
			}
			catch
			{
				return null;
			}
		}

		private static ConstantExpression GetConstantDateTime(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<DateTime>();
				foreach (var item in value.Split('|'))
				{
					if (TryParseDateTime(item, options, out var valueItemDateTime))
					{
						values.Add(valueItemDateTime);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!TryParseDateTime(value, options, out var valueDateTime))
			{
				return null;
			}

			return Expression.Constant(valueDateTime, dataType);
		}
		private static bool TryParseDateTime(string value, WhereQueryEngineOptions options, out DateTime result)
		{
			if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture, DateTimeStyles.None, out result)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture, DateTimeStyles.None, out result)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
			{

				if (options.UseUtcDateTime ?? false)
				{
					result = result.ToUniversalTime();
				}

				return true;
			}

			return false;
		}

	}
}