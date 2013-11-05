                                                                     
                                                                     
                                                                     
                                             
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
				properties.SchemaAction = SchemaAutoAction.Update;
				properties.ConnectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=Jobs;Integrated Security=true";
			});

			cfg.AddAssembly(typeof(Building).Assembly);

			var sessionFactory = cfg.BuildSessionFactory();

			//using (var session = sessionFactory.OpenSession(new CountQueries()))
			//using (var tx = session.BeginTransaction())
			//{
			//	var dog = new Dog
			//	{
			//		Barks = true,
			//		Expiration = DateTime.Now,
			//		RegistrationId = "!23213",
			//		Attributes = new Hashtable
			//		{
			//			{"Fubar", "hi"}
			//		},
			//		Home = new Address { City = "London", Street = "Fleet" },
			//		Vet = new Address { City = "London", Street = "Summer" },
			//	};
			//	session.Save(dog);
			//	var building = new Building { Name = "Building 1" };
			//	session.Save(building);
			//	var emp = new Employee
			//	{
			//		Name = "Employee 1",
			//		EmergencyPhoneNumbers = new List<string>(),
			//		AssoicatedWith = dog
			//	};
			//	emp.EmergencyPhoneNumbers.Add("121");
			//	emp.EmergencyPhoneNumbers.Add("122");
			//	emp.EmergencyPhoneNumbers.Add("124");
			//	emp.EmergencyPhoneNumbers.Add("125");
			//	emp.EmergencyPhoneNumbers.Add("126");
			//	var emp2 = new Employee
			//	{
			//		Name = "Employee 2",
			//		EmergencyPhoneNumbers = new List<string>(),
			//		AssoicatedWith = dog
			//	};
			//	emp2.EmergencyPhoneNumbers.Add("221");
			//	emp2.EmergencyPhoneNumbers.Add("3222");
			//	emp2.EmergencyPhoneNumbers.Add("12324");
			//	emp2.EmergencyPhoneNumbers.Add("12522");
			//	emp2.EmergencyPhoneNumbers.Add("1232");
			//	session.Save(emp);
			//	session.Save(emp2);
			//	var maintenance = new MaintenanceJob
			//	{
			//		At = DateTime.Now,
			//		Building = building,
			//		By = new HashSet<Employee>(),
			//		EmpsByWork = new Dictionary<string, Employee>()
			//	};
			//	maintenance.EmpsByWork.Add("Employee 1", emp);
			//	maintenance.EmpsByWork.Add("Employee 2", emp2);
			//	maintenance.By.Add(emp);
			//	maintenance.By.Add(emp2);

			//	session.Save(maintenance);

			//	session.Save(new Employee
			//	{
			//		AssoicatedWith = building
			//	});


			//	tx.Commit();
			//}

			for (int i = 0; i < 5; i++)
			{
				using (var session = sessionFactory.OpenSession(new CountQueries()))
				using (var tx = session.BeginTransaction())
				{
					var jobs = session.Query<MaintenanceJob>()
						.Fetch(x => x.Building)
						.FetchMany(x => x.Parts)
						.FetchMany(x => x.By)
						.ThenFetchMany(x => x.EmergencyPhoneNumbers)
						.Where(x => x.Id == 1)
						.ToList()
						.FirstOrDefault();

				}
				using (var session = sessionFactory.OpenSession(new CountQueries()))
				using (var tx = session.BeginTransaction())
				{

					var jobs = session.Query<MaintenanceJob>()
						.Fetch(x => x.Building)
						.Where(x => x.Id == 1)
						.ToFuture();

					session.Query<MaintenanceJob>()
						.FetchMany(x => x.Parts)
						.Where(x => x.Id == 1)
						.ToFuture();

					session.Query<MaintenanceJob>()
						.FetchMany(x => x.By)
						.Where(x => x.Id == 1)
						.ToFuture();

					session.Query<Employee>()
						.FetchMany(x => x.EmergencyPhoneNumbers)
						.Where(x => x.Jobs.Any(j => j.Id == 1))
						.ToFuture();

					jobs.ToList();

					//Console.WriteLine("Job Id= " + jobs.Id);
					//Console.WriteLine("Building name= " + jobs.Building.Name);
					//Console.WriteLine("All Employees working there: \n");
					//jobs.By.ForEach(x => Console.WriteLine("Employee: " + x.Name + "\n"));

					//jobs.By.ForEach(x => Console.WriteLine("Emergency numbers: " + x.EmergencyPhoneNumbers + "\n"));

					tx.Commit();
				}
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
			//if (++count > 2)
			//	throw new InvalidOperationException("Too many queries");
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
