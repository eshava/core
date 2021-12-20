using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Eshava.Core.Extensions;
using Eshava.Core.Linq;
using Eshava.Test.Core.Linq.Enums;
using Eshava.Test.Core.Linq.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Linq
{
	[TestClass, TestCategory("Core.Linq")]
	public class TransformQueryEngineTest
	{
		private TransformQueryEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new TransformQueryEngine();
		}

		[TestMethod]
		public void TransformDomainModelToDataModelTest()
		{
			// Arrange
			var today = DateTime.Today;
			Expression<Func<DomainModel, bool>> sourcExpression = s => s.Id == 5 && s.Name == "Test" && today == s.Date;

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today },
				new DataModel { Id = 5, Name = "Test A", Date = DateTime.Today },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today.AddDays(1) }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void TransformDataModelToDomainModelTest()
		{
			// Arrange
			var today = DateTime.Today;
			Expression<Func<DataModel, bool>> sourcExpression = s => s.Id == 5 && s.Name == "Test" && today == s.Date;

			var list = new List<DomainModel>
			{
				new DomainModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 5, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 5, Name = "Test A", Date = DateTime.Today },
				new DomainModel { Id = 5, Name = "Test", Date = DateTime.Today.AddDays(1) }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void TransformDomainModelToDataModelWithSubNavigationTest()
		{
			// Arrange
			var today = DateTime.Today;
			Expression<Func<DomainModel, bool>> sourcExpression = s => s.Sub.SubId == 8 || s.Sub.SubName == "Demo A" || s.Sub.Sub.SubId == 45;

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 8, SubName = "Demo A", Sub = new SubSubDataModel { SubId = 44, SubName = "Nav" } } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 9, SubName = "Demo B", Sub = new SubSubDataModel { SubId = 45, SubName = "Nav" } } },
				new DataModel { Id = 5, Name = "Test A", Date = DateTime.Today, Sub = new SubDataModel { SubId = 7, SubName = "Demo A", Sub = new SubSubDataModel { SubId = 46, SubName = "Nav" } } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today.AddDays(1), Sub = new SubDataModel { SubId = 6, SubName = "Demo B", Sub = new SubSubDataModel { SubId = 74, SubName = "Nav" } } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(3);
		}

		[TestMethod]
		public void TransformDomainModelToDataModelWithCaseDistinctionAndSubNavigationTest()
		{
			// Arrange
			var today = DateTime.Today;
			Expression<Func<DomainModel, bool>> sourcExpression = s => (s.Sub.SubId == 8 || s.Sub.SubName == "Demo A") && s.Sub.Sub.SubId == 46;

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 8, SubName = "Demo A", Sub = new SubSubDataModel { SubId = 44, SubName = "Nav" } } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 9, SubName = "Demo B", Sub = new SubSubDataModel { SubId = 45, SubName = "Nav" } } },
				new DataModel { Id = 5, Name = "Test A", Date = DateTime.Today, Sub = new SubDataModel { SubId = 7, SubName = "Demo A", Sub = new SubSubDataModel { SubId = 46, SubName = "Nav" } } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today.AddDays(1), Sub = new SubDataModel { SubId = 6, SubName = "Demo B", Sub = new SubSubDataModel { SubId = 74, SubName = "Nav" } } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void TransformDataModelToDomainModelWithCompareToConstantTest()
		{
			// Arrange
			Expression<Func<DataModel, bool>> sourcExpression = s => s.Id.Value.CompareTo(2) == 0;

			var list = new List<DomainModel>
			{
				new DomainModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 2, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 3, Name = "Test A", Date = DateTime.Today },
				new DomainModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1) }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void TransformDomainModelToDataModelWithCompareToPropertyTest()
		{
			// Arrange
			Expression<Func<DomainModel, bool>> sourcExpression = s => s.Id.CompareTo(s.Sub.SubId) == 0;

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 2, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 3, Name = "Test A", Date = DateTime.Today, Sub = new SubDataModel { SubId = 3 } },
				new DataModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1), Sub = new SubDataModel { SubId = 0 } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}


		[TestMethod]
		public void TransformDataModelToDomainModelWithCompareToPropertyTest()
		{
			// Arrange
			Expression<Func<DataModel, bool>> sourcExpression = s => s.Id.Value.CompareTo(s.Sub.SubId.Value) == 0;

			var list = new List<DomainModel>
			{
				new DomainModel { Id = 1, Name = "Test", Date = DateTime.Today, Sub = new SubDomainModel { SubId = 0 } },
				new DomainModel { Id = 2, Name = "Test", Date = DateTime.Today, Sub = new SubDomainModel { SubId = 0 } },
				new DomainModel { Id = 3, Name = "Test A", Date = DateTime.Today, Sub = new SubDomainModel { SubId = 3 } },
				new DomainModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1), Sub = new SubDomainModel { SubId = 0 } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}


		[TestMethod]
		public void TransformDataModelToDomainModelWithCompareNullablePropertyValuesTest()
		{
			// Arrange
			Expression<Func<DataModel, bool>> sourcExpression = s => s.NullableStuff.Value == s.Sub.NullableStuff.Value;

			var list = new List<DomainModel>
			{
				new DomainModel { NullableStuff = 1, Name = "Test", Date = DateTime.Today, Sub = new SubDomainModel { NullableStuff = 0 } },
				new DomainModel { NullableStuff = 2, Name = "Test", Date = DateTime.Today, Sub = new SubDomainModel { NullableStuff = 0 } },
				new DomainModel { NullableStuff = 3, Name = "Test A", Date = DateTime.Today, Sub = new SubDomainModel { NullableStuff = 3 } },
				new DomainModel { NullableStuff = 4, Name = "Test", Date = DateTime.Today.AddDays(1), Sub = new SubDomainModel { NullableStuff = 0 } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void TransformDataModelToDomainModelWithCompareNullablePropertyTest()
		{
			// Arrange
			Expression<Func<DataModel, bool>> sourcExpression = s => s.NullableStuff == s.Sub.NullableStuff;

			var list = new List<DomainModel>
			{
				new DomainModel { NullableStuff = 1, Name = "Test", Date = DateTime.Today, Sub = new SubDomainModel { NullableStuff = 0 } },
				new DomainModel { NullableStuff = 2, Name = "Test", Date = DateTime.Today, Sub = new SubDomainModel { NullableStuff = 0 } },
				new DomainModel { NullableStuff = 3, Name = "Test A", Date = DateTime.Today, Sub = new SubDomainModel { NullableStuff = 3 } },
				new DomainModel { NullableStuff = 4, Name = "Test", Date = DateTime.Today.AddDays(1), Sub = new SubDomainModel { NullableStuff = 0 } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void TransformDataModelToDomainModelWithContainedInTest()
		{
			// Arrange
			var ids = new[] { 1, 3, 5 };

			Expression<Func<DataModel, bool>> sourcExpression = s => ids.Any(id => id == s.Id);

			var list = new List<DomainModel>
			{
				new DomainModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 2, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 3, Name = "Test A", Date = DateTime.Today },
				new DomainModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1) }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(2);
		}

		[TestMethod]
		public void TransformDataModelToDomainModelWithContainedInForNullableArrayTest()
		{
			// Arrange
			var ids = new int?[] { 1, 3, 5 };

			Expression<Func<DataModel, bool>> sourcExpression = s => ids.Contains(s.Id);

			var list = new List<DomainModel>
			{
				new DomainModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 2, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 3, Name = "Test A", Date = DateTime.Today },
				new DomainModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1) }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(2);
		}

		[TestMethod]
		public void TransformDomainModelToDataModelWithContainedInTest()
		{
			// Arrange
			var ids = new int[] { 1, 3, 5 };

			Expression<Func<DomainModel, bool>> sourcExpression = s => ids.Contains(s.Id);

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DataModel { Id = 2, Name = "Test", Date = DateTime.Today },
				new DataModel { Id = 3, Name = "Test A", Date = DateTime.Today },
				new DataModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1) }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(2);
		}

		[TestMethod]
		public void TransformDomainModelToDataModelWithContainedInForEnumTest()
		{
			// Arrange
			var array = new Color[] { Color.Black, Color.White };

			Expression<Func<DomainModel, bool>> sourcExpression = s => array.Contains(s.Color);

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", Date = DateTime.Today, Color = Color.Black },
				new DataModel { Id = 2, Name = "Test", Date = DateTime.Today, Color = Color.Blue },
				new DataModel { Id = 3, Name = "Test A", Date = DateTime.Today, Color = Color.Blue },
				new DataModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1), Color = Color.White }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(2);
		}

		[TestMethod]
		public void TransformDomainModelToDataModelWithContainedInForNullableArrayTest()
		{
			// Arrange
			var ids = new int?[] { 1, 3, 5 };

			Expression<Func<DomainModel, bool>> sourcExpression = s => ids.Contains(s.Id);

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DataModel { Id = 2, Name = "Test", Date = DateTime.Today },
				new DataModel { Id = 3, Name = "Test A", Date = DateTime.Today },
				new DataModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1) }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(2);
		}

		[TestMethod]
		public void TransformDataModelToDomainModelWithContainedInForStringTest()
		{
			// Arrange
			var names = new string[] { "Test A", "Test B" };

			Expression<Func<DataModel, bool>> sourcExpression = s => names.Contains(s.Name);

			var list = new List<DomainModel>
			{
				new DomainModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 2, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 3, Name = "Test A", Date = DateTime.Today },
				new DomainModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1) }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void TransformDataModelToDomainModelWithContainsForStringTest()
		{
			// Arrange
			Expression<Func<DataModel, bool>> sourcExpression = s => s.Name.Contains("A") || (s.Sub != null && s.Sub.SubName.Contains("De"));

			var list = new List<DomainModel>
			{
				new DomainModel { Id = 1, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 2, Name = "Test", Date = DateTime.Today },
				new DomainModel { Id = 3, Name = "Test A", Date = DateTime.Today },
				new DomainModel { Id = 4, Name = "Test", Date = DateTime.Today.AddDays(1), Sub = new SubDomainModel{ SubName = "Demo" } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(2);
		}

		[TestMethod]
		public void TransformDataModelToDomainModelWithStartsWithForStringTest()
		{
			// Arrange
			Expression<Func<DataModel, bool>> sourcExpression = s => s.Name.StartsWith("A") || (s.Sub != null && s.Sub.SubName.StartsWith("De"));

			var list = new List<DomainModel>
			{
				new DomainModel { Id = 1, Name = "A Test", Date = DateTime.Today },
				new DomainModel { Id = 2, Name = "B Test", Date = DateTime.Today },
				new DomainModel { Id = 3, Name = "C Test A", Date = DateTime.Today },
				new DomainModel { Id = 4, Name = "D Test", Date = DateTime.Today.AddDays(1), Sub = new SubDomainModel{ SubName = "Demo" } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DataModel, DomainModel>(sourcExpression, true);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(2);
		}

		[TestMethod]
		public void ProfileTest()
		{
			// Arrange
			new TestTransformProfile();

			Expression<Func<DomainModel, bool>> sourcExpression = s => s.Id == 5;

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 5 } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test A", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void ProfileWithPropertyTypeChangesTest()
		{
			// Arrange
			new TestTransformProfile();

			Expression<Func<DomainModel, bool>> sourcExpression = s => s.IAmAnInteger == 5 || s.IAmAnDecimal == 2.5m || s.IAmAString == "8.25";

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", IAmAString = "5", Date = DateTime.Today, Sub = new SubDataModel { SubId = 5 } },
				new DataModel { Id = 5, Name = "Test", IAmAnInteger = 2, Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test A",IAmAnDecimal = 8.25m, Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(3);
		}

		[TestMethod]
		public void ProfileWithPartialMemberExpressionsTest()
		{
			// Arrange
			new TestTransformProfile();

			Expression<Func<DomainModel, bool>> sourcExpression = s => s.Sub.Sub.SubId == 5;

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", IAmAString = "5", Date = DateTime.Today, Sub = new SubDataModel { SubId = 5 } },
				new DataModel { Id = 5, Name = "Test", IAmAnInteger = 2, Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test A",IAmAnDecimal = 8.25m, Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } }
			};

			// Act
			var targetExpression = _classUnderTest.Transform<DomainModel, DataModel>(sourcExpression);

			// Assert
			var result = list.Where(targetExpression.Compile()).ToList();

			result.Should().HaveCount(1);
		}

		[TestMethod]
		public void TransformMemberExpressionDomainModelToDataModelTest()
		{
			// Arrange
			new TestTransformProfile();

			Expression<Func<DomainModel, object>> sourcExpression = s => s.Sub.SubId;
			var sourceMemberExpression = (sourcExpression.Body as UnaryExpression).Operand as MemberExpression;

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", IAmAString = "5", Date = DateTime.Today, Sub = new SubDataModel { SubId = 5 } },
				new DataModel { Id = 5, Name = "Test", IAmAnInteger = 2, Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test A",IAmAnDecimal = 8.25m, Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } }
			};

			// Act
			var targetMemberExpression = _classUnderTest.TransformMemberExpression<DomainModel, DataModel>(sourceMemberExpression, true);

			// Assert
			var query = list.AsQueryable();

			var orderByExpression = Expression.Lambda(targetMemberExpression.Member, targetMemberExpression.Parameter);
			var typeArguments = new Type[] { typeof(DataModel), targetMemberExpression.Member.Type };
			var resultExpression = Expression.Call(typeof(Queryable), nameof(Queryable.OrderBy), typeArguments, query.Expression, Expression.Quote(orderByExpression));

			var result = query.Provider.CreateQuery<DataModel>(resultExpression).ToList();

			result.First().Sub.SubId.Should().Be(0);
			result.Last().Sub.SubId.Should().Be(5);
		}

		[TestMethod]
		public void TransformMemberExpressionDomainModelToDataModelWithProfileTest()
		{
			// Arrange
			new TestTransformProfile();

			Expression<Func<DomainModel, object>> sourcExpression = s => s.Sub.Sub.SubId;
			var sourceMemberExpression = (sourcExpression.Body as UnaryExpression).Operand as MemberExpression;

			var list = new List<DataModel>
			{
				new DataModel { Id = 1, Name = "Test", IAmAString = "5", Date = DateTime.Today, Sub = new SubDataModel { SubId = 5 } },
				new DataModel { Id = 5, Name = "Test", IAmAnInteger = 2, Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test A",IAmAnDecimal = 8.25m, Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } },
				new DataModel { Id = 5, Name = "Test", Date = DateTime.Today, Sub = new SubDataModel { SubId = 0 } }
			};

			// Act
			var targetMemberExpression = _classUnderTest.TransformMemberExpression<DomainModel, DataModel>(sourceMemberExpression);

			// Assert
			var query = list.AsQueryable();

			var orderByExpression = Expression.Lambda(targetMemberExpression.Member, targetMemberExpression.Parameter);
			var typeArguments = new Type[] { typeof(DataModel), targetMemberExpression.Member.Type };
			var resultExpression = Expression.Call(typeof(Queryable), nameof(Queryable.OrderBy), typeArguments, query.Expression, Expression.Quote(orderByExpression));

			var result = query.Provider.CreateQuery<DataModel>(resultExpression).ToList();

			result.First().Sub.SubId.Should().Be(0);
			result.Last().Sub.SubId.Should().Be(5);
		}

		public class TestTransformProfile : TransformProfile
		{
			public TestTransformProfile()
			{
				CreateMap<DomainModel, DataModel>()
					.ForPath(s => s.Id, t => t.Sub.SubId)
					.ForPath(s => s.IAmAnInteger, t => t.IAmAString)
					.ForPath(s => s.IAmAnDecimal, t => t.IAmAnInteger)
					.ForPath(s => s.IAmAString, t => t.IAmAnDecimal)
					.ForPath(s => s.Sub.Sub, t => t.Sub)
					;
			}
		}
	}
}