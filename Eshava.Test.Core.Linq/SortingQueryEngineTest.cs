using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Eshava.Core.Linq;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Models;
using Eshava.Test.Core.Linq.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Linq
{
	[TestClass, TestCategory("Core.Linq")]
	public class SortingQueryEngineTest
	{
		private SortingQueryEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new SortingQueryEngine();
		}

		[TestMethod]
		public void BuildSortConditionInvalidSortOrderTest()
		{
			// Act
			var result = _classUnderTest.BuildSortCondition<Alpha>(SortOrder.None, alpha => alpha.Beta);

			// Assert
			result.Should().BeNull();
		}

		[TestMethod, ExpectedException(typeof(NullReferenceException))]
		public void BuildSortConditionNUllExpressionTest()
		{
			// Act
			_classUnderTest.BuildSortCondition<Alpha>(SortOrder.Ascending, null);
		}

		[TestMethod]
		public void BuildSortConditionAscendingPrimitiveDataTypeTest()
		{
			// Act
			var result = _classUnderTest.BuildSortCondition<Alpha>(SortOrder.Ascending, p => p.Beta);

			// Assert
			result.SortOrder.Should().Be(SortOrder.Ascending);
			result.Member.Member.Name.Should().Be(nameof(Alpha.Beta));
			result.Parameter.Name.Should().Be("p");
		}

		[TestMethod]
		public void BuildSortConditionDescendingEnumerableDataTypeTest()
		{
			// Act
			var result = _classUnderTest.BuildSortCondition<Alpha>(SortOrder.Descending, p => p.Sigma);

			// Assert
			result.SortOrder.Should().Be(SortOrder.Descending);
			result.Member.Member.Name.Should().Be(nameof(Alpha.Sigma));
			result.Parameter.Name.Should().Be("p");
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void BuildSortConditionsNoParameterTest()
		{
			// Arrange
			var queryParameters = default(QueryParameters);

			// Act
			_classUnderTest.BuildSortConditions<Alpha>(queryParameters);
		}

		[TestMethod]
		public void BuildSortConditionsEmptyParameterTest()
		{
			// Arrange
			var queryParameters = new QueryParameters();

			// Act
			var result = _classUnderTest.BuildSortConditions<Alpha>(queryParameters);

			// Assert
			result.Should().HaveCount(0);
		}

		[TestMethod]
		public void BuildSortConditionsTest()
		{
			// Arrange
			var parameter = new QueryParameters
			{
				SortingQueryProperties = new List<SortingQueryProperty>
				{
					new SortingQueryProperty
					{
						PropertyName = nameof(Alpha.Beta),
						SortOrder = SortOrder.Ascending
					},
					new SortingQueryProperty
					{
						PropertyName = nameof(Alpha.Sigma),
						SortOrder = SortOrder.Descending
					},
					new SortingQueryProperty
					{
						PropertyName = nameof(Alpha.Kappa),
						SortOrder = SortOrder.Descending
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildSortConditions<Alpha>(parameter);

			// Assert
			result.Should().HaveCount(3);
			var resultItem = result.First();
			resultItem.SortOrder.Should().Be(SortOrder.Ascending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Beta));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");

			resultItem = result.Skip(1).First();
			resultItem.SortOrder.Should().Be(SortOrder.Descending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Sigma));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");

			resultItem = result.Skip(2).First();
			resultItem.SortOrder.Should().Be(SortOrder.Descending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Kappa));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");
		}

		[TestMethod]
		public void BuildSortConditionsMappingTest()
		{
			// Arrange
			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Rho), new List<Expression<Func<Alpha, object>>> { p => p.Kappa.Psi } }
			};

			var parameter = new QueryParameters
			{
				SortingQueryProperties = new List<SortingQueryProperty>
				{
					new SortingQueryProperty
					{
						PropertyName = nameof(Alpha.Rho),
						SortOrder = SortOrder.Ascending
					}
				}
			};

			// Act
			var result = _classUnderTest.BuildSortConditions(parameter, mappings);

			// Assert
			result.Should().HaveCount(1);
			var resultItem = result.First();
			resultItem.SortOrder.Should().Be(SortOrder.Ascending);
			resultItem.Member.Member.Name.Should().Be(nameof(Omega.Psi));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Omega));
			resultItem.Parameter.Name.Should().Be("p");
		}

		[TestMethod]
		public void ApplySortingNullConditionsTest()
		{
			// Act
			var result = _classUnderTest.ApplySorting<Alpha>(null, null);

			// Assert
			result.Should().BeNull();
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ApplySortingNullQueryTest()
		{
			// Act
			_classUnderTest.ApplySorting<Alpha>(null, new List<OrderByCondition> { new OrderByCondition() });
		}

		[TestMethod]
		public void ApplySortingTest()
		{
			// Arrange
			var elements = new List<Alpha>
			{
				new Alpha {
					Rho = 1,
					Beta = 15,
					Gamma = null
				},
				new Alpha {
					Rho = 2,
					Beta = 10,
					Gamma = "A"
				},
				new Alpha {
					Rho = 3,
					Beta = 10,
					Gamma = "C"
				},
				new Alpha {
					Rho = 4,
					Beta = 10,
					Gamma = "B"
				}
			};

			var query = elements.AsQueryable();
			var parameter = new QueryParameters
			{
				SortingQueryProperties = new List<SortingQueryProperty>
				{
					new SortingQueryProperty
					{
						PropertyName = nameof(Alpha.Beta),
						SortOrder = SortOrder.Ascending
					},
					new SortingQueryProperty
					{
						PropertyName = nameof(Alpha.Gamma),
						SortOrder = SortOrder.Descending
					}
				}
			};

			var conditions = _classUnderTest.BuildSortConditions<Alpha>(parameter);

			// Act
			var result = _classUnderTest.ApplySorting(query, conditions).ToList();

			// Assert
			result[0].Rho.Should().Be(3);
			result[1].Rho.Should().Be(4);
			result[2].Rho.Should().Be(2);
			result[3].Rho.Should().Be(1);
		}

		[TestMethod]
		public void AddOrderTest()
		{
			// Arrange
			var elements = new List<Alpha>
			{
				new Alpha {
					Rho = 1,
					Beta = 15,
					Gamma = null
				},
				new Alpha {
					Rho = 2,
					Beta = 1,
					Gamma = "A"
				},
				new Alpha {
					Rho = 3,
					Beta = 5,
					Gamma = "C"
				},
				new Alpha {
					Rho = 4,
					Beta = 7,
					Gamma = "B"
				}
			};

			var query = elements.AsQueryable();
			var condition = _classUnderTest.BuildSortCondition<Alpha>(SortOrder.Descending, p => p.Beta);

			// Act
			var result = _classUnderTest.AddOrder(query, condition).ToList();

			// Assert
			result[0].Rho.Should().Be(1);
			result[1].Rho.Should().Be(4);
			result[2].Rho.Should().Be(3);
			result[3].Rho.Should().Be(2);
		}

		[TestMethod]
		public void AddOrderThen()
		{
			// Arrange
			var elements = new List<Alpha>
			{
				new Alpha {
					Rho = 1,
					Beta = 10,
					OmegaDateTime = DateTime.Today
				},
				new Alpha {
					Rho = 2,
					Beta = 10,
					OmegaDateTime = DateTime.Today.AddDays(5)
				},
				new Alpha {
					Rho = 3,
					Beta = 10,
					OmegaDateTime = DateTime.Today.AddDays(-5)
				},
				new Alpha {
					Rho = 4,
					Beta = 10,
					OmegaDateTime = DateTime.Today.AddDays(1)
				}
			};

			var firstCondition = _classUnderTest.BuildSortCondition<Alpha>(SortOrder.Descending, p => p.Beta);
			var secondCondition = _classUnderTest.BuildSortCondition<Alpha>(SortOrder.Ascending, p => p.OmegaDateTime);
			var query = _classUnderTest.AddOrder(elements.AsQueryable(), firstCondition);

			// Act
			var result = _classUnderTest.AddOrderThen(query, secondCondition).ToList();

			// Assert
			result[0].Rho.Should().Be(3);
			result[1].Rho.Should().Be(1);
			result[2].Rho.Should().Be(4);
			result[3].Rho.Should().Be(2);
		}

		[TestMethod]
		public void AddOrderByDescendingEnumTest()
		{
			// Arrange
			var elements = new List<Omega>
			{
				new Omega {
					Sigma = 1,
					Number = Number.One
				},
				new Omega {
					Sigma = 2,
					Number = Number.Two
				},
				new Omega {
					Sigma = 3,
					Number = Number.Three
				},
				new Omega {
					Sigma = 4,
					Number = Number.Four
				}
			};

			var query = elements.AsQueryable();
			var condition = _classUnderTest.BuildSortCondition<Omega>(SortOrder.Descending, p => p.Number);

			// Act
			var result = _classUnderTest.AddOrder(query, condition).ToList();

			// Assert
			result[0].Number.Should().Be(Number.Four);
			result[1].Number.Should().Be(Number.Three);
			result[2].Number.Should().Be(Number.Two);
			result[3].Number.Should().Be(Number.One);
		}

		[TestMethod]
		public void AddOrderByEnumTest()
		{
			// Arrange
			var elements = new List<Omega>
			{
				new Omega {
					Sigma = 1,
					Number = Number.Two
				},
				new Omega {
					Sigma = 2,
					Number = Number.Four
				},
				new Omega {
					Sigma = 3,
					Number = Number.Three
				},
				new Omega {
					Sigma = 4,
					Number = Number.One
				}
			};

			var query = elements.AsQueryable();
			var condition = _classUnderTest.BuildSortCondition<Omega>(SortOrder.Ascending, p => p.Number);

			// Act
			var result = _classUnderTest.AddOrder(query, condition).ToList();

			// Assert
			result[0].Number.Should().Be(Number.One);
			result[1].Number.Should().Be(Number.Two);
			result[2].Number.Should().Be(Number.Three);
			result[3].Number.Should().Be(Number.Four);
		}

		[TestMethod]
		public void AddOrderThenByDescendingEnumTest()
		{
			// Arrange
			var elements = new List<Omega>
			{
				new Omega {
					Sigma = 4,
					Number = Number.One
				},
				new Omega {
					Sigma = 2,
					Number = Number.Two
				},
				new Omega {
					Sigma = 4,
					Number = Number.Three
				},
				new Omega {
					Sigma = 2,
					Number = Number.Four
				}
			};

			var firstCondition = _classUnderTest.BuildSortCondition<Omega>(SortOrder.Descending, p => p.Sigma);
			var secondCondition = _classUnderTest.BuildSortCondition<Omega>(SortOrder.Descending, p => p.Number);
			var query = _classUnderTest.AddOrder(elements.AsQueryable(), firstCondition);

			// Act
			var result = _classUnderTest.AddOrderThen(query, secondCondition).ToList();


			// Assert
			result[0].Number.Should().Be(Number.Three);
			result[1].Number.Should().Be(Number.One);
			result[2].Number.Should().Be(Number.Four);
			result[3].Number.Should().Be(Number.Two);
		}

		[TestMethod]
		public void AddOrderThenByEnumTest()
		{
			// Arrange
			var elements = new List<Omega>
			{
				new Omega {
					Sigma = 4,
					Number = Number.Two
				},
				new Omega {
					Sigma = 2,
					Number = Number.Four
				},
				new Omega {
					Sigma = 2,
					Number = Number.Three
				},
				new Omega {
					Sigma = 4,
					Number = Number.One
				}
			};

			var firstCondition = _classUnderTest.BuildSortCondition<Omega>(SortOrder.Ascending, p => p.Sigma);
			var secondCondition = _classUnderTest.BuildSortCondition<Omega>(SortOrder.Ascending, p => p.Number);
			var query = _classUnderTest.AddOrder(elements.AsQueryable(), firstCondition);

			// Act
			var result = _classUnderTest.AddOrderThen(query, secondCondition).ToList();

			// Assert
			result[0].Number.Should().Be(Number.Three);
			result[1].Number.Should().Be(Number.Four);
			result[2].Number.Should().Be(Number.One);
			result[3].Number.Should().Be(Number.Two);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void NoSortingObjectTest()
		{
			// Arrange
			var sortings = default(SortingModel);

			// Act
			_classUnderTest.BuildSortConditions<Alpha>(sortings);
		}

		[TestMethod]
		public void BuildSortConditionsBySortingObjectTest()
		{
			// Arrange
			var sortings = new SortingModel
			{
				Beta = new SpecialSortField
				{
					SortIndex = 0,
					SortOrder = SortOrder.Ascending
				},
				Sigma = new SortField
				{
					SortIndex = 1,
					SortOrder = SortOrder.Descending
				},
				Kappa = new SortField
				{
					SortIndex = 2,
					SortOrder = SortOrder.Descending
				}
			};

			// Act
			var result = _classUnderTest.BuildSortConditions<Alpha>(sortings);

			// Assert
			result.Should().HaveCount(3);
			var resultItem = result.First();
			resultItem.SortOrder.Should().Be(SortOrder.Ascending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Beta));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");

			resultItem = result.Skip(1).First();
			resultItem.SortOrder.Should().Be(SortOrder.Descending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Sigma));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");

			resultItem = result.Skip(2).First();
			resultItem.SortOrder.Should().Be(SortOrder.Descending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Kappa));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");
		}

		[TestMethod]
		public void BuildSortConditionsMappingBSortingObjectTest()
		{
			// Arrange
			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Rho), new List<Expression<Func<Alpha, object>>> { p => p.Kappa.Psi } }
			};

			var sortings = new SortingModel
			{
				Rho = new SortField
				{
					SortIndex = 0,
					SortOrder = SortOrder.Ascending
				}
			};

			// Act
			var result = _classUnderTest.BuildSortConditions(sortings, mappings);

			// Assert
			result.Should().HaveCount(1);
			var resultItem = result.First();
			resultItem.SortOrder.Should().Be(SortOrder.Ascending);
			resultItem.Member.Member.Name.Should().Be(nameof(Omega.Psi));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Omega));
			resultItem.Parameter.Name.Should().Be("p");
		}

		[TestMethod]
		public void BuildSortConditionsBySortingListTest()
		{
			// Arrange
			var sortingQueryProperties = new List<SortingQueryProperty>
			{
				new SortingQueryProperty
				{
					PropertyName = nameof(Alpha.Beta),
					SortOrder = SortOrder.Ascending
				},
				new SortingQueryProperty
				{
					PropertyName = nameof(Alpha.Sigma),
					SortOrder = SortOrder.Descending
				},
				new SortingQueryProperty
				{
					PropertyName = nameof(Alpha.Kappa),
					SortOrder = SortOrder.Descending
				}
			};

			// Act
			var result = _classUnderTest.BuildSortConditions<Alpha>(sortingQueryProperties);

			// Assert
			result.Should().HaveCount(3);
			var resultItem = result.First();
			resultItem.SortOrder.Should().Be(SortOrder.Ascending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Beta));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");

			resultItem = result.Skip(1).First();
			resultItem.SortOrder.Should().Be(SortOrder.Descending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Sigma));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");

			resultItem = result.Skip(2).First();
			resultItem.SortOrder.Should().Be(SortOrder.Descending);
			resultItem.Member.Member.Name.Should().Be(nameof(Alpha.Kappa));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Alpha));
			resultItem.Parameter.Name.Should().Be("p");
		}

		[TestMethod]
		public void BuildSortConditionsMappingBySortingListTest()
		{
			// Arrange
			var mappings = new Dictionary<string, List<Expression<Func<Alpha, object>>>>
			{
				{ nameof(Alpha.Rho), new List<Expression<Func<Alpha, object>>> { p => p.Kappa.Psi } }
			};

			var sortingQueryProperties = new List<SortingQueryProperty>
			{
				new SortingQueryProperty
				{
					PropertyName = nameof(Alpha.Rho),
					SortOrder = SortOrder.Ascending
				}
			};

			// Act
			var result = _classUnderTest.BuildSortConditions(sortingQueryProperties, mappings);

			// Assert
			result.Should().HaveCount(1);
			var resultItem = result.First();
			resultItem.SortOrder.Should().Be(SortOrder.Ascending);
			resultItem.Member.Member.Name.Should().Be(nameof(Omega.Psi));
			resultItem.Member.Member.DeclaringType.Should().Be(typeof(Omega));
			resultItem.Parameter.Name.Should().Be("p");
		}
	}
}