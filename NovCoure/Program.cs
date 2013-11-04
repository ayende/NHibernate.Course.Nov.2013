using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NovCoure.Model;

namespace NovCoure
{
	class Program
	{
		static void Main(string[] args)
		{
			App_Start.NHibernateProfilerBootstrapper.PreStart();

			var cfg = new Configuration();
			//cfg.SetInterceptor(new DontBeSlow());
			//cfg.SetNamingStrategy(new DbaAreFoolsToTryToUnderstandMe());
			cfg.DataBaseIntegration(properties =>
			{
				properties.Dialect<MsSql2008Dialect>();
				properties.SchemaAction = SchemaAutoAction.Create;
				properties.ConnectionString = @"Data Source=.\SqlExpress;Initial Catalog=Jobs;Integrated Security=true";
			});

			cfg.AddAssembly(typeof(Building).Assembly);

			var sessionFactory = cfg.BuildSessionFactory();

			using (var session = sessionFactory.OpenSession(new CountQueries()))
			using (var tx = session.BeginTransaction())
			{
				session.Save(new Dog
				{
					Barks = true,
					Home = new Address{City = "London", Street = "Fleet"},
					Vet = new Address { City = "London", Street = "Summer" },
				});
				session.Save(new Cat
				{
					Annoying = true,
					Home = new Address(),
					Vet = null
				});

				tx.Commit();
			}

			using (var session = sessionFactory.OpenSession(new CountQueries()))
			using (var tx = session.BeginTransaction())
			{

				session.Query<Animal>().ToList();
				session.Query<Dog>().ToList();
				session.Query<Cat>().ToList();
				tx.Commit();
			}
		}
	}

	public class DbaAreFoolsToTryToUnderstandMe : INamingStrategy
	{
		public string ClassToTableName(string className)
		{
			return className;
		}

		public string PropertyToColumnName(string propertyName)
		{
			return propertyName;
		}

		public string TableName(string tableName)
		{
			return "`_" + BitConverter.ToString(Encoding.UTF8.GetBytes(tableName)).Replace("-", "_") + "`";
		}

		public string ColumnName(string columnName)
		{
			return "`_" + BitConverter.ToString(Encoding.UTF8.GetBytes(columnName)).Replace("-", "_") + "`";
		}

		public string PropertyToTableName(string className, string propertyName)
		{
			return className + "_" + propertyName;
		}

		public string LogicalColumnName(string columnName, string propertyName)
		{
			return columnName;
		}
	}

	public class CountQueries : EmptyInterceptor
	{
		private int count = 0;
		public override SqlString OnPrepareStatement(SqlString sql)
		{
			if (++count > 2)
				throw new InvalidOperationException("Too many queries");
			return base.OnPrepareStatement(sql);
		}
	}

	public class DontBeSlow : EmptyInterceptor
	{
		public override SqlString OnPrepareStatement(SqlString sql)
		{
			if (sql.LastIndexOfCaseInsensitive("Cats") != -1)
				throw new InvalidOperationException("Can't don't like queries, curisoity killed the cat");
			return base.OnPrepareStatement(sql);
		}

	}
}

