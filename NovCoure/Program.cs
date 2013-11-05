                                                                     
                                                                     
                                                                     
                                             
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
﻿using Iesi.Collections;
﻿using NHibernate;
﻿using NHibernate.Cache;
﻿using NHibernate.Cfg;
using NHibernate.Dialect;
﻿using NHibernate.Event;
﻿using NHibernate.Event.Default;
﻿using NHibernate.Impl;
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

			//cfg.SetListener(ListenerType.Delete, new BarkingDogsWillNotBeDeleted());

			cfg.SetListeners(ListenerType.Delete, new IDeleteEventListener[]
			{
				new DefaultBuildingDeleteListener(), 
				new DefaultDeleteEventListener()
			});

			// secret: make nhibernate faster toggle
			cfg.SetProperty("default_batch_fetch_size", "15");

			cfg.Cache(properties =>
			{
				properties.UseQueryCache = true;
				properties.Provider<HashtableCacheProvider>();
			});

			cfg.AddAssembly(typeof (Building).Assembly);

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

			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete(session.Get<Building>(4));

				tx.Commit();
			}

			Console.WriteLine("Done");
			Console.ReadLine();
		}

		public class MyResult
		{
			public bool Barks;
			public int Count;
		}
	}

	public class DefaultBuildingDeleteListener : IDeleteEventListener
	{
		public void OnDelete(DeleteEvent @event)
		{
			OnDelete(@event, null);
		}

		public void OnDelete(DeleteEvent @event, ISet transientEntities)
		{
			var building = @event.Entity as Building;

			if (building == null)
				return;

			if (building.Id == 1)
				throw new InvalidOperationException("Cannot delete default");

			foreach (var maintenanceJob in building.Jobs)
			{
				maintenanceJob.Building = @event.Session.Load<Building>(1);
			}
		}
	}
	

	public class BarkingDogsWillNotBeDeleted : IDeleteEventListener
	{
		public  void OnDelete(DeleteEvent @event)
		{
			OnDelete(@event, null);
		}

		public void OnDelete(DeleteEvent @event, ISet transientEntities)
		{
			var dog = @event.Entity as Dog;
			if (dog != null && dog.Barks)
				throw new ApplicationException("Why you delete me? I'll stop barking!");
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
