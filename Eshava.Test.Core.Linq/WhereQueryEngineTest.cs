using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eshava.Core.Linq;
using Eshava.Core.Linq.Attributes;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Models;
using Eshava.Test.Core.Linq.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Linq
{
	[TestClass, TestCategory("Core.Linq")]
	public class WhereQueryEngineTest
	{
		private WhereQueryEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = false,
				SkipInvalidWhereQueries = false
			});
		}

		[TestMethod]
		public void BuildQueryExpressionsWithNullInputTest()
		{
			// Arrange
			var queryParameters = default(QueryParameters);

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameters);

			// Assert
			result.IsFaulty.Should().BeTrue();
			result.Message.Should().Be("InvalidInput");
		}

		[TestMethod]
		public void BuildQueryExpressionsNoConditionsTest()
		{
			// Arrange
			var queryParameter = new QueryParameters();

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermTest()
		{
			// Arrange
			var queryParameter = new QueryParameters
			{
				SearchTerm = "Darkwing Duck"
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, queryParameter.SearchTerm);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermCaseInsensitiveOneTest()
		{
			// Arrange
			var propertyValue = "Darkwing Duck";
			var queryParameter = new QueryParameters
			{
				SearchTerm = propertyValue.ToLower()
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyValue);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermCaseInsensitiveTwoTest()
		{
			// Arrange
			var propertyValue = "Darkwing Duck";
			var queryParameter = new QueryParameters
			{
				SearchTerm = propertyValue
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyValue.ToLower());
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermCaseSensitiveOneTest()
		{
			// Arrange
			var propertyValue = "Darkwing Duck";
			var queryParameter = new QueryParameters
			{
				SearchTerm = propertyValue.ToLower()
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyValue);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermCaseSensitiveTwoTest()
		{
			// Arrange
			var propertyValue = "Darkwing Duck";
			var queryParameter = new QueryParameters
			{
				SearchTerm = propertyValue
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyValue.ToLower());
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[DataTestMethod]
		[DataRow(false, DisplayName = "Deactivated split option")]
		[DataRow(true, DisplayName = "Activated split option")]
		public void BuildQueryExpressionsGlobalSearchTermWithSplitContainsOptionTest(bool containsSearchSplitBySpace)
		{
			// Arrange
			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = containsSearchSplitBySpace
			});

			var propertyContent = "Darkwing Duck";
			var queryParameter = new QueryParameters
			{
				SearchTerm = "Dark Du"
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyContent);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			if (containsSearchSplitBySpace)
			{
				resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			}
			else
			{
				resultWhere.Should().HaveCount(0);
			}
			propertyCountQueryIgnore.Should().Be(1);
		}

		[DataTestMethod]
		[DataRow(false, DisplayName = "Deactivated split option")]
		[DataRow(true, DisplayName = "Activated split option")]
		public void BuildQueryExpressionsGlobalSearchTermWithSplitContainsOptionCaseInsensitiveOneTest(bool containsSearchSplitBySpace)
		{
			// Arrange
			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = containsSearchSplitBySpace
			});

			var propertyContent = "Darkwing Duck";
			var queryParameter = new QueryParameters
			{
				SearchTerm = "Dark Du".ToLower()
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyContent);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			if (containsSearchSplitBySpace)
			{
				resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			}
			else
			{
				resultWhere.Should().HaveCount(0);
			}
			propertyCountQueryIgnore.Should().Be(1);
		}

		[DataTestMethod]
		[DataRow(false, DisplayName = "Deactivated split option")]
		[DataRow(true, DisplayName = "Activated split option")]
		public void BuildQueryExpressionsGlobalSearchTermWithSplitContainsOptionCaseInsensitiveTwoTest(bool containsSearchSplitBySpace)
		{
			// Arrange
			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = containsSearchSplitBySpace
			});

			var propertyContent = "Darkwing Duck".ToLower();
			var queryParameter = new QueryParameters
			{
				SearchTerm = "Dark Du"
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyContent);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			if (containsSearchSplitBySpace)
			{
				resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			}
			else
			{
				resultWhere.Should().HaveCount(0);
			}
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermWithSplitContainsOptionCaseSensitiveOneTest()
		{
			// Arrange
			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = true
			});

			var propertyContent = "Darkwing Duck";
			var queryParameter = new QueryParameters
			{
				SearchTerm = "Dark Du".ToLower()
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyContent);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermWithSplitContainsOptionCaseSensitiveTwoTest()
		{
			// Arrange
			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = true
			});

			var propertyContent = "Darkwing Duck".ToLower();
			var queryParameter = new QueryParameters
			{
				SearchTerm = "Dark Du"
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyContent);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultGamma = p => p.Gamma == "Darkwing Duck";
			Expression<Func<Alpha, bool>> expectedResultDelta = p => p.Delta != null && p.Delta.Contains("QuackFu");

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultGamma);
			result.Data.Last().Should().BeEquivalentTo(expectedResultDelta);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyCaseInsensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Darkwing Duck".ToLower()
					},
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu".ToLower()
					}
				}
			};


			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyCaseInsensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD".ToLower(),
					Delta = "QuackFu better than KungFu".ToLower()
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck".ToLower(),
					Delta = "QuackFu better than KungFu".ToLower()
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck".ToLower(),
					Delta = "KungFu".ToLower()
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu"
					}
				}
			};


			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Darkwing Duck".ToLower()
					},
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu".ToLower()
					}
				}
			};


			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD".ToLower(),
					Delta = "QuackFu better than KungFu".ToLower()
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck".ToLower(),
					Delta = "QuackFu better than KungFu".ToLower()
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck".ToLower(),
					Delta = "KungFu".ToLower()
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu"
					}
				}
			};


			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringContainsNotPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Delta = "KungFu is the worst"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu"
					},
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "worst"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringContainsNotPropertyCaseInsensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Delta = "KungFu is the worst"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QUACKFU"
					},
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "WORST"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringContainsNotPropertyCaseInsensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU WORSE THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU"
				},
				new() {
					Beta = 4,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU IS THE WORST"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu"
					},
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "worst"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringContainsNotPropertyCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Delta = "KungFu is the worst"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QUACKFU"
					},
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "WORST"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringContainsNotPropertyCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU WORSE THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU"
				},
				new() {
					Beta = 4,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU IS THE WORST"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu"
					},
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "worst"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringStartsWithPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.StartsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KungFu"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultDelta = p => p.Delta != null && p.Delta.StartsWith("KungFu");

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);
			result.Data.Single().Should().BeEquivalentTo(expectedResultDelta);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringStartsWithPropertyCaseInsensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.StartsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KUNGFU"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringStartsWithPropertyCaseInsensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU WORSE THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.StartsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KungFu"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringStartsWithPropertyCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.StartsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KUNGFU"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringStartsWithPropertyCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU WORSE THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.StartsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KungFu"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringEndsWithPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu is the best"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.EndsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KungFu"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultDelta = p => p.Delta != null && p.Delta.EndsWith("KungFu");

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);
			result.Data.Single().Should().BeEquivalentTo(expectedResultDelta);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringEndsWithPropertyCaseInsensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu is the best"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.EndsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KUNGFU"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringEndsWithPropertyCaseInsensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU WORSE THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU IS THE BEST"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.EndsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KungFu"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringEndsWithPropertyCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu worse than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu is the best"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.EndsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KUNGFU"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringEndsWithPropertyCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU WORSE THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU IS THE BEST"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.EndsWith,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "KungFu"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[DataTestMethod]
		[DataRow(false, DisplayName = "Deactivated split option")]
		[DataRow(true, DisplayName = "Activated split option")]
		public void BuildQueryExpressionsStringPropertyWithSplitContainsOptionTest(bool containsSearchSplitBySpace)
		{
			// Arrange
			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = containsSearchSplitBySpace
			});

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Dark Du"
					}
				}
			};

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			if (containsSearchSplitBySpace)
			{
				result.Data.Should().HaveCount(2);

				var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
				resultWhere.Should().HaveCount(1);
				resultWhere.First().Beta.Should().Be(2);
			}
			else
			{
				result.Data.Should().HaveCount(1);
				var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
				resultWhere.Should().HaveCount(0);
			}
		}

		[DataTestMethod]
		[DataRow(false, DisplayName = "Deactivated split option")]
		[DataRow(true, DisplayName = "Activated split option")]
		public void BuildQueryExpressionsStringPropertyWithSplitContainsOptionCaseInsensitiveOneTest(bool containsSearchSplitBySpace)
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = containsSearchSplitBySpace
			});

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "DARK DU"
					}
				}
			};

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			if (containsSearchSplitBySpace)
			{
				result.Data.Should().HaveCount(2);

				var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
				resultWhere.Should().HaveCount(1);
				resultWhere.First().Beta.Should().Be(2);
			}
			else
			{
				result.Data.Should().HaveCount(1);
				var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
				resultWhere.Should().HaveCount(0);
			}
		}

		[DataTestMethod]
		[DataRow(false, DisplayName = "Deactivated split option")]
		[DataRow(true, DisplayName = "Activated split option")]
		public void BuildQueryExpressionsStringPropertyWithSplitContainsOptionCaseInsensitiveTwoTest(bool containsSearchSplitBySpace)
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = containsSearchSplitBySpace
			});

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING",
					Delta = "KUNGFU"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Dark Du"
					}
				}
			};

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			if (containsSearchSplitBySpace)
			{
				result.Data.Should().HaveCount(2);

				var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
				resultWhere.Should().HaveCount(1);
				resultWhere.First().Beta.Should().Be(2);
			}
			else
			{
				result.Data.Should().HaveCount(1);
				var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
				resultWhere.Should().HaveCount(0);
			}
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyWithSplitContainsOptionCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = true
			});

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing",
					Delta = "KungFu"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "DARK DU"
					}
				}
			};

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyWithSplitContainsOptionCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = true
			});

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING",
					Delta = "KUNGFU"
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Dark Du"
					}
				}
			};

			// Act
			var result = classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyQueryIgnoreTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					DeltaTwo = "Darkwing Duck",
					Delta = "QuackFu"
				},
				new() {
					Beta = 2,
					DeltaTwo = "MegaVolt",
					Delta = "QuackFu"
				},
				new() {
					Beta = 3,
					DeltaTwo = "Darkwing Duck",
					Delta = "MegaVolt"
				},
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.DeltaTwo),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Delta),
						SearchTerm = "QuackFu"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.Single().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(2);
		}


		[TestMethod]
		public void BuildQueryExpressionsIntegerPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Lambda = 4,
					LambdaNullable = 10
				},
				new() {
					Beta = 2,
					Lambda = 6,
					LambdaNullable = 11
				},
				new() {
					Beta = 3,
					Lambda = 6,
					LambdaNullable = 8
				},
				new() {
					Beta = 4,
					Lambda = 6,
					LambdaNullable = 13
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.GreaterThan,
						PropertyName = nameof(Alpha.Lambda),
						SearchTerm = "5"
					},
					new() {
						Operator =  CompareOperator.LessThan,
						PropertyName = nameof(Alpha.LambdaNullable),
						SearchTerm = "12"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultLambda = p => p.Lambda > 5;
			Expression<Func<Alpha, bool>> expectedResultLambdaNullable = p => p.LambdaNullable < 12;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultLambda);
			result.Data.Last().Should().BeEquivalentTo(expectedResultLambdaNullable);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(2);
			resultWhere.Last().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsLongPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					LambdaLong = 4L,
					LambdaLongNullable = 10L
				},
				new() {
					Beta = 2,
					LambdaLong = 6L,
					LambdaLongNullable = 11L
				},
				new() {
					Beta = 3,
					LambdaLong = 6L,
					LambdaLongNullable = 8L
				},
				new() {
					Beta = 4,
					LambdaLong = 6L,
					LambdaLongNullable = 13L
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.GreaterThan,
						PropertyName = nameof(Alpha.LambdaLong),
						SearchTerm = "5"
					},
					new() {
						Operator =  CompareOperator.LessThan,
						PropertyName = nameof(Alpha.LambdaLongNullable),
						SearchTerm = "12"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultLambda = p => p.LambdaLong > 5L;
			Expression<Func<Alpha, bool>> expectedResultLambdaNullable = p => p.LambdaLongNullable < 12L;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultLambda);
			result.Data.Last().Should().BeEquivalentTo(expectedResultLambdaNullable);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(2);
			resultWhere.Last().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsDecimalPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					My = 4m,
					MyNullableOne = 10m
				},
				new() {
					Beta = 2,
					My = 4.26m,
					MyNullableOne = 10.49m
				},
				new() {
					Beta = 3,
					My = 6.25m,
					MyNullableOne = 8.1m
				},
				new() {
					Beta = 4,
					My = 5m,
					MyNullableOne = 11m
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.GreaterThan,
						PropertyName = nameof(Alpha.My),
						SearchTerm = "4.25"
					},
					new() {
						Operator =  CompareOperator.LessThan,
						PropertyName = nameof(Alpha.MyNullableOne),
						SearchTerm = "10.5"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultMy = p => p.My > 4.25m;
			Expression<Func<Alpha, bool>> expectedResultMyNullableOne = p => p.MyNullableOne < 10.5m;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultMy);
			result.Data.Last().Should().BeEquivalentTo(expectedResultMyNullableOne);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(2);
			resultWhere.Last().Beta.Should().Be(3);
		}


		[TestMethod]
		public void BuildQueryExpressionsFloatPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Ny = 4f,
					NyNullable = 10f
				},
				new() {
					Beta = 2,
					Ny = 4.25f,
					NyNullable = 10.5f
				},
				new() {
					Beta = 3,
					Ny = 6f,
					NyNullable = 8f
				},
				new() {
					Beta = 4,
					Ny = 5f,
					NyNullable = 11f
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.GreaterThanOrEqual,
						PropertyName = nameof(Alpha.Ny),
						SearchTerm = "4.25"
					},
					new() {
						Operator =  CompareOperator.LessThanOrEqual,
						PropertyName = nameof(Alpha.NyNullable),
						SearchTerm = "10.5"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultNy = p => p.Ny >= 4.25f;
			Expression<Func<Alpha, bool>> expectedResultNyNullable = p => p.NyNullable <= 10.5f;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultNy);
			result.Data.Last().Should().BeEquivalentTo(expectedResultNyNullable);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(2);
			resultWhere.Last().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsDoublePropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Xi = 0,
					XiNullable = 0
				},
				new() {
					Beta = 2,
					Xi = 4.25,
					XiNullable = 4.25
				},
				new() {
					Beta = 3,
					Xi = 4.25,
					XiNullable = 10.5
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Xi),
						SearchTerm = "4.25"
					},
					new() {
						Operator =  CompareOperator.NotEqual,
						PropertyName = nameof(Alpha.XiNullable),
						SearchTerm = "10.5"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultXi = p => p.Xi == 4.25;
			Expression<Func<Alpha, bool>> expectedResultXiNullable = p => p.XiNullable != 10.5;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultXi);
			result.Data.Last().Should().BeEquivalentTo(expectedResultXiNullable);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsDateTimePropertyTest()
		{
			// Arrange
			var dateTimePsi = DateTime.Today.Add(new TimeSpan(12, 30, 15));
			var dateTimeOmega = DateTime.Today.Add(new TimeSpan(9, 45, 30));
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Psi = null,
					OmegaDateTime = dateTimeOmega
				},
				new() {
					Beta = 2,
					Psi = dateTimeOmega,
					OmegaDateTime = dateTimePsi
				},
				new() {
					Beta = 3,
					Psi = dateTimePsi,
					OmegaDateTime = dateTimePsi
				},
				new() {
					Beta = 4,
					Psi = DateTime.Today.AddDays(1),
					OmegaDateTime = dateTimePsi
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.LessThanOrEqual,
						PropertyName = nameof(Alpha.Psi),
						SearchTerm = dateTimePsi.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
					},
					new() {
						Operator =  CompareOperator.NotEqual,
						PropertyName = nameof(Alpha.OmegaDateTime),
						SearchTerm = dateTimeOmega.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultPsi = p => p.Psi <= dateTimePsi;
			Expression<Func<Alpha, bool>> expectedResultOmegaDateTime = p => p.OmegaDateTime != dateTimeOmega;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultPsi);
			result.Data.Last().Should().BeEquivalentTo(expectedResultOmegaDateTime);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(2);
			resultWhere.Last().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsWrongDataTypesTest()
		{
			// Arrange
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.GreaterThan,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm =  ""
					},
					new() {
						Operator =  CompareOperator.GreaterThan,
						PropertyName = nameof(Alpha.Lambda),
						SearchTerm =  "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.GreaterThanOrEqual,
						PropertyName = nameof(Alpha.My),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.GreaterThanOrEqual,
						PropertyName = nameof(Alpha.Ny),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Xi),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Psi),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.StigmaOne),
						SearchTerm =  ""
					},
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Chi),
						SearchTerm = "Darkwing Duck"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeTrue();
			result.Message.Should().Be("InvalidData");

			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.First().PropertyName.Should().Be("Gamma");
			result.ValidationErrors.First().ErrorType.Should().Be("InvalidFilterValue");
		}

		[TestMethod]
		public void BuildQueryExpressionsBooleanPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					StigmaOne = true,
					StigmaTwo = null
				},
				new() {
					Beta = 2,
					StigmaOne = true,
					StigmaTwo = false
				},
				new() {
					Beta = 3,
					StigmaOne = true,
					StigmaTwo = true
				},
				new() {
					Beta = 4,
					StigmaOne = false,
					StigmaTwo = false
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.StigmaOne),
						SearchTerm = "1"
					},
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.StigmaTwo),
						SearchTerm = "false"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultStigmaOne = p => p.StigmaOne == true;
			Expression<Func<Alpha, bool>> expectedResultStigmaTwo = p => p.StigmaTwo == false;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultStigmaOne);
			result.Data.Last().Should().BeEquivalentTo(expectedResultStigmaTwo);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsBooleanNullablePropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					StigmaTwo = null
				},
				new() {
					Beta = 2,
					StigmaTwo = false
				},
				new() {
					Beta = 3,
					StigmaTwo = true
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.NotEqual,
						PropertyName = nameof(Alpha.StigmaTwo),
						SearchTerm = "true"
					},
					new() {
						Operator =  CompareOperator.NotEqual,
						PropertyName = nameof(Alpha.StigmaTwo),
						SearchTerm = "false"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultStigmaOne = p => p.StigmaTwo != true;
			Expression<Func<Alpha, bool>> expectedResultStigmaTwo = p => p.StigmaTwo != false;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultStigmaOne);
			result.Data.Last().Should().BeEquivalentTo(expectedResultStigmaTwo);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGuidPropertyTest()
		{
			// Arrange
			var guidToMatch = Guid.Parse("87428c4c-26c5-4208-861f-6875606e356f");

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Chi = null
				},
				new() {
					Beta = 2,
					Chi = Guid.Empty
				},
				new() {
					Beta = 3,
					Chi = guidToMatch
				}
				,
				new() {
					Beta = 4,
					Chi = Guid.Parse("3f93f67c-1345-48c4-bb71-547378e71c6c")
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Chi),
						SearchTerm = guidToMatch.ToString()
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultStigmaOne = p => p.Chi == guidToMatch;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);
			result.Data.First().Should().BeEquivalentTo(expectedResultStigmaOne);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyMappingTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Kappa = new Omega
					{
						Psi = ""
					}
				},
				new() {
					Beta = 2,
					Kappa = new Omega
					{
						Psi = null
					}
				},
				new() {
					Beta = 3,
					Kappa = new Omega
					{
						Psi = "Darkwing Duck"
					}
				},
				new() {
					Beta = 4,
					Kappa = new Omega
					{
						Psi = "QuackFu"
					}
				}
			};

			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Chi), new List<Expression<Func<Alpha, object>>> { p => p.Kappa.Psi } }
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Chi),
						SearchTerm = "Darkwing Duck"
					},
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Chi),
						SearchTerm = "QuackFu"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultEqual = p => p.Kappa.Psi == "Darkwing Duck";
			Expression<Func<Alpha, bool>> expectedResultContains = p => p.Kappa.Psi != null && p.Kappa.Psi.Contains("QuackFu");

			// Act
			var result = _classUnderTest.BuildQueryExpressions(queryParameter, mappings);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultEqual);
			result.Data.Last().Should().BeEquivalentTo(expectedResultContains);

			exampleList.Where(result.Data.First().Compile()).Single().Beta.Should().Be(3);
			exampleList.Where(result.Data.Last().Compile()).Single().Beta.Should().Be(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyMappingMultipleWithInvalidFilterValueTest()
		{
			// Arrange
			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Chi), new List<Expression<Func<Alpha, object>>> { p => p.Kappa.Psi, p => p.Kappa.Chi, p => p.Kappa.Sigma } }
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Chi),
						SearchTerm = "Darkwing Duck"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultEqual = p => p.Kappa.Psi == "Darkwing Duck" || p.Kappa.Chi == "Darkwing Duck";

			// Act
			var result = _classUnderTest.BuildQueryExpressions(queryParameter, mappings);

			// Assert
			result.IsFaulty.Should().BeTrue();
			result.Message.Should().Be("InvalidData");

			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.First().PropertyName.Should().Be("Sigma");
			result.ValidationErrors.First().ErrorType.Should().Be("InvalidFilterValue");
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyMappingMultipleTest()
		{
			// Arrange
			_classUnderTest = new WhereQueryEngine(new WhereQueryEngineOptions
			{
				UseUtcDateTime = true,
				ContainsSearchSplitBySpace = false,
				SkipInvalidWhereQueries = true
			});

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Kappa = new Omega
					{
						Psi = "",
						Chi = ""
					}
				},
				new() {
					Beta = 2,
					Kappa = new Omega
					{
						Psi = null,
						Chi = null
					}
				},
				new() {
					Beta = 3,
					Kappa = new Omega
					{
						Psi = "Darkwing Duck",
						Chi = ""
					}
				},
				new() {
					Beta = 4,
					Kappa = new Omega
					{
						Psi = "",
						Chi = "Darkwing Duck"
					}
				}
			};

			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Chi), new List<Expression<Func<Alpha, object>>> { p => p.Kappa.Psi, p => p.Kappa.Chi, p => p.Beta } }
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Alpha.Chi),
						SearchTerm = "Darkwing Duck"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultEqual = p => p.Kappa.Psi == "Darkwing Duck" || p.Kappa.Chi == "Darkwing Duck";

			// Act
			var result = _classUnderTest.BuildQueryExpressions(queryParameter, mappings);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);
			result.Data.First().Should().BeEquivalentTo(expectedResultEqual);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(3);
			resultWhere.Last().Beta.Should().Be(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyMappingEnumarableTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Sigma = null
				},
				new() {
					Beta = 2,
					Sigma = new List<int>()
				},
				new() {
					Beta = 3,
					Sigma = new List<int> { 7, 5, 3 }
				},
				new() {
					Beta = 4,
					Sigma = new List<int> { 1, 2, 3, }
				}
			};

			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Chi), new List<Expression<Func<Alpha, object>>> { p => p.Sigma } }
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Chi),
						SearchTerm = "7"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultEqual = p => p.Sigma != null && p.Sigma.Contains(7);

			// Act
			var result = _classUnderTest.BuildQueryExpressions(queryParameter, mappings);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);
			result.Data.First().Should().BeEquivalentTo(expectedResultEqual);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyEnumarableTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Sigma = null
				},
				new() {
					Beta = 2,
					Sigma = new List<int>()
				},
				new() {
					Beta = 3,
					Sigma = new List<int> { 7, 5, 3 }
				},
				new() {
					Beta = 4,
					Sigma = new List<int> { 1, 2, 3, }
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Alpha.Sigma),
						SearchTerm = "7"
					}
				}
			};

			Expression<Func<Alpha, bool>> expectedResultEqual = p => p.Sigma != null && p.Sigma.Contains(7);

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);
			result.Data.First().Should().BeEquivalentTo(expectedResultEqual);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(3);
		}

		[TestMethod]
		public void RemovePropertyMappingsGuidSearchTest()
		{
			// Arrange
			var queryProperties = new List<WhereQueryProperty>
			{
				new() {
					PropertyName = nameof(Alpha.Chi),
					SearchTerm = Guid.NewGuid().ToString(),
					Operator = CompareOperator.Contains
				}
			};

			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Chi), new List< Expression<Func<Alpha, object>>>{ p => p.Kappa.Psi } },
				{ nameof(Alpha.Delta), new List< Expression<Func<Alpha, object>>>{ p => p.DeltaTwo } },
			};


			// Act
			_classUnderTest.RemovePropertyMappings(queryProperties, mappings);

			// Assert
			mappings.Keys.Should().HaveCount(1);
			mappings.ContainsKey(nameof(Alpha.Delta)).Should().BeTrue();
			queryProperties.First().Operator.Should().Be(CompareOperator.Equal);
		}

		[TestMethod]
		public void RemovePropertyMappingsStringSearchTest()
		{
			// Arrange
			var queryProperties = new List<WhereQueryProperty>
			{
				new() {
					PropertyName = nameof(Alpha.Chi),
					SearchTerm = "Darkwing Duck",
					Operator = CompareOperator.Equal
				}
			};

			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Chi), new List< Expression<Func<Alpha, object>>>{ p => p.Kappa.Psi } },
				{ nameof(Alpha.Delta), new List< Expression<Func<Alpha, object>>>{ p => p.DeltaTwo } },
			};


			// Act
			_classUnderTest.RemovePropertyMappings(queryProperties, mappings);

			// Assert
			mappings.Keys.Should().HaveCount(2);
			mappings.ContainsKey(nameof(Alpha.Chi)).Should().BeTrue();
			mappings.ContainsKey(nameof(Alpha.Delta)).Should().BeTrue();
		}

		[TestMethod]
		public void RemovePropertyMappingsNoMappingsTest()
		{
			// Arrange
			var queryProperties = new List<WhereQueryProperty>
			{
				new() {
					PropertyName = nameof(Alpha.Chi),
					SearchTerm = "Darkwing Duck",
					Operator = CompareOperator.Equal
				}
			};

			var unexpectedException = default(Exception);

			// Act
			try
			{
				_classUnderTest.RemovePropertyMappings<Alpha>(queryProperties, null);
			}
			catch (Exception ex)
			{
				unexpectedException = ex;
			}

			// Assert
			unexpectedException.Should().BeNull();
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void RemovePropertyMappingsNoPropertiesTest()
		{
			// Arrange
			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Chi), new List< Expression<Func<Alpha, object>>>{ p => p.Kappa.Psi } },
				{ nameof(Alpha.Delta), new List< Expression<Func<Alpha, object>>>{ p => p.DeltaTwo } },
			};

			// Act
			_classUnderTest.RemovePropertyMappings(null, mappings);
		}

		[TestMethod]
		public void BuildQueryExpressionsEnumPropertyTest()
		{
			// Arrange
			var exampleList = new List<Omega>
			{
				new() {
					Sigma = 1,
					Number = Number.Four
				},
				new() {
					Sigma = 2,
					Number = Number.One
				},
				new() {
					Sigma = 3,
					Number = Number.Three
				},
				new() {
					Sigma = 4,
					Number = Number.One
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Omega.Number),
						SearchTerm = "One"
					},
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName = nameof(Omega.Number),
						SearchTerm = "1"
					}
				}
			};

			Expression<Func<Omega, bool>> expectedResultNumber = p => p.Number == Number.One;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Omega>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultNumber);
			result.Data.Last().Should().BeEquivalentTo(expectedResultNumber);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Sigma.Should().Be(2);
			resultWhere.Last().Sigma.Should().Be(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsEnumPropertyMappingTest()
		{
			// Arrange
			var exampleList = new List<Omega>
			{
				new() {
					Sigma = 1,
					Number = Number.One,
					InnerOmega = new Omega
					{
						Number = Number.Four
					}
				},
				new() {
					Sigma = 2,
					Number = Number.One,
					InnerOmega = new Omega
					{
						Number = Number.One
					}
				},
				new() {
					Sigma = 3,
					Number = Number.One,
					InnerOmega = new Omega
					{
						Number = Number.Three
					}
				},
				new() {
					Sigma = 4,
					Number = Number.One,
					InnerOmega = new Omega
					{
						Number = Number.One
					}
				}
			};

			var mappings = new Dictionary<string, List<Expression<Func<Omega, object>>>>
			{
				{  "InnerOmegaNumber",  new List<Expression<Func<Omega, object>>> { v => v.InnerOmega.Number } }
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName =  "InnerOmegaNumber",
						SearchTerm = "One"
					},
					new() {
						Operator =  CompareOperator.Equal,
						PropertyName =  "InnerOmegaNumber",
						SearchTerm = "1"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Omega>(queryParameter, mappings);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Sigma.Should().Be(2);
			resultWhere.Last().Sigma.Should().Be(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsEnumGreaterAndLessPropertyTest()
		{
			// Arrange
			var exampleList = new List<Omega>
			{
				new() {
					Sigma = 1,
					Number = Number.Four
				},
				new() {
					Sigma = 2,
					Number = Number.One
				},
				new() {
					Sigma = 3,
					Number = Number.Three
				},
				new() {
					Sigma = 4,
					Number = Number.One
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.GreaterThan,
						PropertyName = nameof(Omega.Number),
						SearchTerm = "One"
					},
					new() {
						Operator =  CompareOperator.LessThan,
						PropertyName = nameof(Omega.Number),
						SearchTerm = "4"
					}
				}
			};

			Expression<Func<Omega, bool>> expectedResultGreaterNumber = p => p.Number > Number.One;
			Expression<Func<Omega, bool>> expectedResultLessNumber = p => p.Number < Number.One;

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Omega>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);
			result.Data.First().Should().BeEquivalentTo(expectedResultGreaterNumber);
			result.Data.Last().Should().BeEquivalentTo(expectedResultLessNumber);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Sigma.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsEnumContainsPropertyTest()
		{
			// Arrange
			var exampleList = new List<Omega>
			{
				new() {
					Sigma = 1,
					Numbers = new List<Number> { Number.One, Number.Two }
				},
				new() {
					Sigma = 2,
					Numbers = new List<Number> { Number.Four, Number.Two, Number.Three }
				},
				new() {
					Sigma = 3,
					Numbers = new List<Number> { Number.Four }
				},
				new() {
					Sigma = 4,
					Numbers = new List<Number> { Number.Three }
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.Contains,
						PropertyName = nameof(Omega.Numbers),
						SearchTerm = "Two"
					},
					new() {
						Operator =  CompareOperator.ContainsNot,
						PropertyName = nameof(Omega.Numbers),
						SearchTerm = "4"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Omega>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Sigma.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsEnumContainedInPropertyTest()
		{
			// Arrange
			var exampleList = new List<Omega>
			{
				new() {
					Sigma = 1,
					Number = Number.Four
				},
				new() {
					Sigma = 2,
					Number = Number.One
				},
				new() {
					Sigma = 3,
					Number = Number.Three
				},
				new() {
					Sigma = 4,
					Number = Number.Two
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Omega.Number),
						SearchTerm = "Two|Four"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Omega>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Sigma.Should().Be(1);
			resultWhere.Last().Sigma.Should().Be(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsContainedInPropertyTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Lambda = 11
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Lambda = 22
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing",
					Lambda = 22
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Lambda = 44
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Darkwing Duck|DD"
					},
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Lambda),
						SearchTerm = "11|44"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsContainedInPropertyCaseInsensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Lambda = 11
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Lambda = 22
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing",
					Lambda = 22
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Lambda = 44
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "DARKWING DUCK|DD"
					},
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Lambda),
						SearchTerm = "11|44"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsContainedInPropertyCaseInsensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Lambda = 11
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Lambda = 22
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING",
					Lambda = 22
				},
				new() {
					Beta = 4,
					Gamma = "DARKWING DUCK",
					Lambda = 44
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Darkwing Duck|DD"
					},
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Lambda),
						SearchTerm = "11|44"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(4);
		}

		[TestMethod]
		public void BuildQueryExpressionsContainedInPropertyCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Lambda = 11
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Lambda = 22
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing",
					Lambda = 22
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Lambda = 44
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "DARKWING DUCK|DD"
					},
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Lambda),
						SearchTerm = "11|44"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsContainedInPropertyCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Lambda = 11
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Lambda = 22
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING",
					Lambda = 22
				},
				new() {
					Beta = 4,
					Gamma = "DARKWING DUCK",
					Lambda = 44
				}
			};

			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = "Darkwing Duck|DD"
					},
					new() {
						Operator =  CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Lambda),
						SearchTerm = "11|44"
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(1);
		}

		[TestMethod]
		public void BuildGlobalQueryConditionForMappings()
		{
			// Arrange
			var queryParameter = new QueryParameters
			{
				SearchTerm = "Darkwing Duck"
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();

			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ "virtual", new List<Expression<Func<Alpha, object>>> { t => t.Kappa.Chi } }
			};

			var exampleList = new List<Alpha>();
			var alphaOne = new Alpha
			{
				Kappa = new Omega
				{
					Chi = queryParameter.SearchTerm
				}
			};
			var alphaTwo = new Alpha
			{
				DeltaTwo = queryParameter.SearchTerm,
				Kappa = new Omega()
			};

			exampleList.Add(alphaOne);
			exampleList.Add(alphaTwo);

			// Act
			var result = _classUnderTest.BuildQueryExpressions(queryParameter, mappings);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var filter = new FilterModel
			{
				Gamma = new SpecialFilterField
				{
					Operator = CompareOperator.Equal,
					SearchTerm = "Darkwing Duck"
				},
				Delta = new FilterField
				{
					Operator = CompareOperator.Contains,
					SearchTerm = "QuackFu"
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectCaseInsensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var filter = new FilterModel
			{
				Gamma = new SpecialFilterField
				{
					Operator = CompareOperator.Equal,
					SearchTerm = "DARKWING DUCK"
				},
				Delta = new FilterField
				{
					Operator = CompareOperator.Contains,
					SearchTerm = "QUACKFU"
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectCaseInsensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU"
				}
			};

			var filter = new FilterModel
			{
				Gamma = new SpecialFilterField
				{
					Operator = CompareOperator.Equal,
					SearchTerm = "Darkwing Duck"
				},
				Delta = new FilterField
				{
					Operator = CompareOperator.Contains,
					SearchTerm = "QuackFu"
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
			resultWhere.First().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var filter = new FilterModel
			{
				Gamma = new SpecialFilterField
				{
					Operator = CompareOperator.Equal,
					SearchTerm = "DARKWING DUCK"
				},
				Delta = new FilterField
				{
					Operator = CompareOperator.Contains,
					SearchTerm = "QUACKFU"
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 3,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU"
				}
			};

			var filter = new FilterModel
			{
				Gamma = new SpecialFilterField
				{
					Operator = CompareOperator.Equal,
					SearchTerm = "Darkwing Duck"
				},
				Delta = new FilterField
				{
					Operator = CompareOperator.Contains,
					SearchTerm = "QuackFu"
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectWithFilterGroupTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "Quack_Fu better than Kung-Fu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "Quack-Fu better than Kung-Fu"
				},
				new() {
					Beta = 3,
					Gamma = "DD",
					Delta = "Quack_Fu better than KungFu"
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Delta = "Quack-Fu better than KungFu"
				},
				new() {
					Beta = 5,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than Kung-Fu"
				},
				new() {
					Beta = 6,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var filter = new FilterModel
			{
				ComplexFilterField = new ComplexFilterField
				{
					Operator = CompareOperator.None,
					LinkOperator = LinkOperator.And,
					LinkOperations = new List<ComplexFilterField>
					{
						new() {
							Operator = CompareOperator.None,
							LinkOperator = LinkOperator.Or,
							LinkOperations = new List<ComplexFilterField>
							{
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "Quack_Fu"
								},
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "Quack-Fu"
								}
							}
						},
						new() {
							Field = "Delta",
							Operator = CompareOperator.ContainsNot,
							SearchTerm = "KungFu"
						}
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectWithFilterGroupCaseInsensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "Quack_Fu better than Kung-Fu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "Quack-Fu better than Kung-Fu"
				},
				new() {
					Beta = 3,
					Gamma = "DD",
					Delta = "Quack_Fu better than KungFu"
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Delta = "Quack-Fu better than KungFu"
				},
				new() {
					Beta = 5,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than Kung-Fu"
				},
				new() {
					Beta = 6,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var filter = new FilterModel
			{
				ComplexFilterField = new ComplexFilterField
				{
					Operator = CompareOperator.None,
					LinkOperator = LinkOperator.And,
					LinkOperations = new List<ComplexFilterField>
					{
						new() {
							Operator = CompareOperator.None,
							LinkOperator = LinkOperator.Or,
							LinkOperations = new List<ComplexFilterField>
							{
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "QUACK_FU"
								},
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "QUACK-FU"
								}
							}
						},
						new() {
							Field = "Delta",
							Operator = CompareOperator.ContainsNot,
							SearchTerm = "KUNGFU"
						}
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectWithFilterGroupCaseInsensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACK_FU BETTER THAN KUNG-FU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACK-FU BETTER THAN KUNG-FU"
				},
				new() {
					Beta = 3,
					Gamma = "DD",
					Delta = "QUACK_FU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 4,
					Gamma = "DARKWING DUCK",
					Delta = "QUACK-FU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 5,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNG-FU"
				},
				new() {
					Beta = 6,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU"
				}
			};

			var filter = new FilterModel
			{
				ComplexFilterField = new ComplexFilterField
				{
					Operator = CompareOperator.None,
					LinkOperator = LinkOperator.And,
					LinkOperations = new List<ComplexFilterField>
					{
						new() {
							Operator = CompareOperator.None,
							LinkOperator = LinkOperator.Or,
							LinkOperations = new List<ComplexFilterField>
							{
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "Quack_Fu"
								},
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "Quack-Fu"
								}
							}
						},
						new() {
							Field = "Delta",
							Operator = CompareOperator.ContainsNot,
							SearchTerm = "KungFu"
						}
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
			resultWhere.First().Beta.Should().Be(1);
			resultWhere.Last().Beta.Should().Be(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectWithFilterGroupCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "Quack_Fu better than Kung-Fu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "Quack-Fu better than Kung-Fu"
				},
				new() {
					Beta = 3,
					Gamma = "DD",
					Delta = "Quack_Fu better than KungFu"
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Delta = "Quack-Fu better than KungFu"
				},
				new() {
					Beta = 5,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than Kung-Fu"
				},
				new() {
					Beta = 6,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var filter = new FilterModel
			{
				ComplexFilterField = new ComplexFilterField
				{
					Operator = CompareOperator.None,
					LinkOperator = LinkOperator.And,
					LinkOperations = new List<ComplexFilterField>
					{
						new() {
							Operator = CompareOperator.None,
							LinkOperator = LinkOperator.Or,
							LinkOperations = new List<ComplexFilterField>
							{
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "QUACK_FU"
								},
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "QUACK-FU"
								}
							}
						},
						new() {
							Field = "Delta",
							Operator = CompareOperator.ContainsNot,
							SearchTerm = "KUNGFU"
						}
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectWithFilterGroupCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QUACK_FU BETTER THAN KUNG-FU"
				},
				new() {
					Beta = 2,
					Gamma = "DARKWING DUCK",
					Delta = "QUACK-FU BETTER THAN KUNG-FU"
				},
				new() {
					Beta = 3,
					Gamma = "DD",
					Delta = "QUACK_FU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 4,
					Gamma = "DARKWING DUCK",
					Delta = "QUACK-FU BETTER THAN KUNGFU"
				},
				new() {
					Beta = 5,
					Gamma = "DARKWING DUCK",
					Delta = "QUACKFU BETTER THAN KUNG-FU"
				},
				new() {
					Beta = 6,
					Gamma = "DARKWING DUCK",
					Delta = "KUNGFU"
				}
			};

			var filter = new FilterModel
			{
				ComplexFilterField = new ComplexFilterField
				{
					Operator = CompareOperator.None,
					LinkOperator = LinkOperator.And,
					LinkOperations = new List<ComplexFilterField>
					{
						new() {
							Operator = CompareOperator.None,
							LinkOperator = LinkOperator.Or,
							LinkOperations = new List<ComplexFilterField>
							{
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "Quack_Fu"
								},
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "Quack-Fu"
								}
							}
						},
						new() {
							Field = "Delta",
							Operator = CompareOperator.ContainsNot,
							SearchTerm = "KungFu"
						}
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectWithFilterGroupNotAllowedFilterFieldTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "Quack_Fu better than Kung-Fu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "Quack-Fu better than Kung-Fu"
				},
				new() {
					Beta = 3,
					Gamma = "DD",
					Delta = "Quack_Fu better than KungFu"
				},
				new() {
					Beta = 4,
					Gamma = "Darkwing Duck",
					Delta = "Quack-Fu better than KungFu"
				},
				new() {
					Beta = 5,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than Kung-Fu"
				},
				new() {
					Beta = 6,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var filter = new FilterModel
			{
				ComplexFilterField = new ComplexFilterField
				{
					Operator = CompareOperator.None,
					LinkOperator = LinkOperator.And,
					LinkOperations = new List<ComplexFilterField>
					{
						new() {
							Operator = CompareOperator.None,
							LinkOperator = LinkOperator.Or,
							LinkOperations = new List<ComplexFilterField>
							{
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "Quack_Fu"
								},
								new() {
									Field = "Delta",
									Operator = CompareOperator.Contains,
									SearchTerm = "Quack-Fu"
								}
							}
						},
						new() {
							Field = "Gamma",
							Operator = CompareOperator.ContainsNot,
							SearchTerm = "KungFu"
						}
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null);

			// Assert
			result.IsFaulty.Should().BeTrue();
			result.Data.Should().BeNull();

			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.First().PropertyName.Should().Be("Gamma");
			result.ValidationErrors.First().ErrorType.Should().Be("NotAllowed");
		}

		[TestMethod]
		public void BuildQueryExpressionsStringPropertyByFilterObjectWithNotAllowedCompareOperationsTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Gamma = "DD",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 2,
					Gamma = "Darkwing Duck",
					Delta = "QuackFu better than KungFu"
				},
				new() {
					Beta = 3,
					Gamma = "Darkwing Duck",
					Delta = "KungFu"
				}
			};

			var filter = new FilterModel
			{
				Gamma = new SpecialFilterField
				{
					Operator = CompareOperator.StartsWith,
					SearchTerm = "Darkwing Duck"
				},
				Delta = new FilterField
				{
					Operator = CompareOperator.EndsWith,
					SearchTerm = "QuackFu"
				}
			};

			Expression<Func<Alpha, bool>> expectedResultGamma = p => p.Gamma == "Darkwing Duck";
			Expression<Func<Alpha, bool>> expectedResultDelta = p => p.Delta != null && p.Delta.Contains("QuackFu");

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, null);

			// Assert
			result.IsFaulty.Should().BeTrue();
			result.Message.Should().Be("InvalidFilter");

			result.ValidationErrors.Should().HaveCount(2);
			result.ValidationErrors.First().PropertyName.Should().Be(nameof(FilterModel.Gamma));
			result.ValidationErrors.First().ErrorType.Should().Be("InvalidOperator");
			result.ValidationErrors.First().Value.Should().Be(CompareOperator.StartsWith.ToString());

			result.ValidationErrors.Last().PropertyName.Should().Be(nameof(FilterModel.Delta));
			result.ValidationErrors.Last().ErrorType.Should().Be("InvalidOperator");
			result.ValidationErrors.Last().Value.Should().Be(CompareOperator.EndsWith.ToString());
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermByFilterObjectTest()
		{
			// Arrange
			var filter = new FilterModel
			{
				SearchTerm = "Darkwing Duck"
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, filter.SearchTerm);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, filter.SearchTerm);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermByFilterObjectCaseInsensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var propertyValue = "Darkwing Duck";
			var filter = new FilterModel
			{
				SearchTerm = propertyValue.ToUpper()
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyValue);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, filter.SearchTerm, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermByFilterObjectCaseInsensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var propertyValue = "Darkwing Duck";
			var filter = new FilterModel
			{
				SearchTerm = propertyValue
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyValue.ToUpper());
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, filter.SearchTerm, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(properties.Count - propertyCountQueryIgnore);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermByFilterObjectCaseSensitiveOneTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var propertyValue = "Darkwing Duck";
			var filter = new FilterModel
			{
				SearchTerm = propertyValue.ToUpper()
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyValue);
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, filter.SearchTerm, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermByFilterObjectCaseSensitiveTwoTest()
		{
			// Arrange
			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var propertyValue = "Darkwing Duck";
			var filter = new FilterModel
			{
				SearchTerm = propertyValue
			};

			var typeString = typeof(string);
			var properties = typeof(Alpha).GetProperties().Where(p => p.PropertyType == typeString && p.CanWrite).ToList();
			var propertyCountQueryIgnore = 0;

			var exampleList = new List<Alpha>();
			foreach (var propertyInfo in properties)
			{
				var alpha = new Alpha();
				propertyInfo.SetValue(alpha, propertyValue.ToUpper());
				exampleList.Add(alpha);

				if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
				{
					propertyCountQueryIgnore++;
				}
			}

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(filter, filter.SearchTerm, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
			propertyCountQueryIgnore.Should().Be(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyMappingByFilterObjectTest()
		{
			// Arrange
			var exampleList = new List<Alpha>
			{
				new() {
					Beta = 1,
					Kappa = new Omega
					{
						Psi = ""
					}
				},
				new() {
					Beta = 2,
					Kappa = new Omega
					{
						Psi = null
					}
				},
				new() {
					Beta = 3,
					Kappa = new Omega
					{
						Psi = "Darkwing Duck"
					}
				},
				new() {
					Beta = 4,
					Kappa = new Omega
					{
						Psi = "QuackFu"
					}
				}
			};

			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Chi), new List<Expression<Func<Alpha, object>>> { p => p.Kappa.Psi } }
			};

			var filter = new FilterModel
			{
				Chi = new SpecialFilterField
				{
					Operator = CompareOperator.Equal,
					SearchTerm = "Darkwing Duck"
				}
			};

			Expression<Func<Alpha, bool>> expectedResultEqual = p => p.Kappa.Psi == "Darkwing Duck";

			// Act
			var result = _classUnderTest.BuildQueryExpressions(filter, null, mappings);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);
			result.Data.First().Should().BeEquivalentTo(expectedResultEqual);

			exampleList.Where(result.Data.First().Compile()).Single().Beta.Should().Be(3);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringListTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueOne
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueTwo
					}
				}
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					San = null
				},
				new() {
					San = new List<string> { "MegaVolt" }
				},
				new() {
					San = new List<string> { "MegaVolt", propertyValueOne }
				},
				new() {
					San = new List<string> { "QuackFu", propertyValueOne }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringListCaseInsensitiveOneTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueOne.ToUpper()
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueTwo.ToUpper()
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					San = null
				},
				new() {
					San = new List<string> { "MegaVolt" }
				},
				new() {
					San = new List<string> { "MegaVolt", propertyValueOne }
				},
				new() {
					San = new List<string> { "QuackFu", propertyValueOne }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringListCaseInsensitiveTwoTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueOne
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueTwo
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					San = null
				},
				new() {
					San = new List<string> { "MEGAVOLT" }
				},
				new() {
					San = new List<string> { "MEGAVOLT", propertyValueOne.ToUpper() }
				},
				new() {
					San = new List<string> { "QUACKFU", propertyValueOne.ToUpper() }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringListCaseSensitiveOneTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueOne.ToUpper()
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueTwo.ToUpper()
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					San = null
				},
				new() {
					San = new List<string> { "MegaVolt" }
				},
				new() {
					San = new List<string> { "MegaVolt", propertyValueOne }
				},
				new() {
					San = new List<string> { "QuackFu", propertyValueOne }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringListCaseSensitiveTwoTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueOne
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.San),
						SearchTerm = propertyValueTwo
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					San = null
				},
				new() {
					San = new List<string> { "MEGAVOLT" }
				},
				new() {
					San = new List<string> { "MEGAVOLT", propertyValueOne.ToUpper() }
				},
				new() {
					San = new List<string> { "QUACKFU", propertyValueOne.ToUpper() }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringIEnumerableTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueOne
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueTwo
					}
				}
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Heta = null
				},
				new() {
					Heta = Array.Empty<string>()
				},
				new() {
					Heta = new List<string> { "MegaVolt" }
				},
				new() {
					Heta = new List<string> { "MegaVolt", propertyValueOne }
				},
				new() {
					Heta = new List<string> { "QuackFu", propertyValueOne }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringIEnumerableCaseInsensitiveOneTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueOne.ToUpper()
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueTwo.ToUpper()
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Heta = null
				},
				new() {
					Heta = Array.Empty<string>()
				},
				new() {
					Heta = new List<string> { "MegaVolt" }
				},
				new() {
					Heta = new List<string> { "MegaVolt", propertyValueOne }
				},
				new() {
					Heta = new List<string> { "QuackFu", propertyValueOne }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringIEnumerableCaseInsensitiveTwoTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueOne
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueTwo
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Heta = null
				},
				new() {
					Heta = Array.Empty<string>()
				},
				new() {
					Heta = new List<string> { "MEGAVOLT" }
				},
				new() {
					Heta = new List<string> { "MEGAVOLT", propertyValueOne.ToUpper() }
				},
				new() {
					Heta = new List<string> { "QUACKFU", propertyValueOne.ToUpper() }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(1);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringIEnumerableCaseSensitiveOneTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueOne.ToUpper()
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueTwo.ToUpper()
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Heta = null
				},
				new() {
					Heta = Array.Empty<string>()
				},
				new() {
					Heta = new List<string> { "MegaVolt" }
				},
				new() {
					Heta = new List<string> { "MegaVolt", propertyValueOne }
				},
				new() {
					Heta = new List<string> { "QuackFu", propertyValueOne }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyContainsStringIEnumerableCaseSensitiveTwoTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.Contains,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueOne
					},
					new() {
						Operator = CompareOperator.ContainsNot,
						PropertyName = nameof(Alpha.Heta),
						SearchTerm = propertyValueTwo
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Heta = null
				},
				new() {
					Heta = Array.Empty<string>()
				},
				new() {
					Heta = new List<string> { "MEGAVOLT" }
				},
				new() {
					Heta = new List<string> { "MEGAVOLT", propertyValueOne.ToUpper() }
				},
				new() {
					Heta = new List<string> { "QUACKFU", propertyValueOne.ToUpper() }
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(2);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).Where(result.Data.Last().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyStringContainedInTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = $"{propertyValueOne}|{propertyValueTwo}"
					}
				}
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Gamma = null
				},
				new() {
					Gamma = "MegaVolt"
				},
				new() {
					Gamma = propertyValueOne
				},
				new() {
					Gamma = propertyValueTwo
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyStringContainedInCaseInsensitiveOneTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = $"{propertyValueOne.ToUpper()}|{propertyValueTwo.ToUpper()}"
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Gamma = null
				},
				new() {
					Gamma = "MegaVolt"
				},
				new() {
					Gamma = propertyValueOne
				},
				new() {
					Gamma = propertyValueTwo
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyStringContainedInCaseInsensitiveTwoTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = $"{propertyValueOne}|{propertyValueTwo}"
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = true
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Gamma = null
				},
				new() {
					Gamma = "MegaVolt"
				},
				new() {
					Gamma = propertyValueOne.ToUpper()
				},
				new() {
					Gamma = propertyValueTwo.ToUpper()
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(2);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyStringContainedInCaseSensitiveOneTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = $"{propertyValueOne.ToUpper()}|{propertyValueTwo.ToUpper()}"
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Gamma = null
				},
				new() {
					Gamma = "MegaVolt"
				},
				new() {
					Gamma = propertyValueOne
				},
				new() {
					Gamma = propertyValueTwo
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildQueryExpressionsPropertyStringContainedInCaseSensitiveTwoTest()
		{
			// Arrange
			var propertyValueOne = "Darkwing Duck";
			var propertyValueTwo = "QuackFu";
			var queryParameter = new QueryParameters
			{
				WhereQueryProperties = new List<WhereQueryProperty>
				{
					new() {
						Operator = CompareOperator.ContainedIn,
						PropertyName = nameof(Alpha.Gamma),
						SearchTerm = $"{propertyValueOne}|{propertyValueTwo}"
					}
				}
			};

			var options = new WhereQueryEngineOptions
			{
				CaseInsensitive = false
			};

			var typeString = typeof(string);

			var exampleList = new List<Alpha>
			{
				new() {
					Gamma = null
				},
				new() {
					Gamma = "MegaVolt"
				},
				new() {
					Gamma = propertyValueOne.ToUpper()
				},
				new() {
					Gamma = propertyValueTwo.ToUpper()
				}
			};

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter, options: options);

			// Assert
			result.IsFaulty.Should().BeFalse();
			result.Data.Should().HaveCount(1);

			var resultWhere = exampleList.Where(result.Data.First().Compile()).ToList();
			resultWhere.Should().HaveCount(0);
		}
	}
}