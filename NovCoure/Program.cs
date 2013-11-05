﻿                                                                     
                                                                     
                                                                     
                                             
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
﻿using System.Data.SQLite;
﻿using System.Linq;
using System.Text;
﻿using Iesi.Collections;
﻿using Lucene.Net.Analysis;
﻿using NHibernate;
﻿using NHibernate.Cache;
﻿using NHibernate.Cfg;
using NHibernate.Dialect;
﻿using NHibernate.Event;
﻿using NHibernate.Event.Default;
﻿using NHibernate.Impl;
using NHibernate.Linq;
﻿using NHibernate.Persister.Entity;
﻿using NHibernate.Search;
﻿using NHibernate.Search.Event;
﻿using NHibernate.Search.Store;
﻿using NHibernate.SqlCommand;
﻿using NHibernate.Tool.hbm2ddl;
﻿using NovCoure.Model;
﻿using Environment = NHibernate.Search.Environment;

namespace NovCoure
{
	class Program
	{
		static void Main(string[] args)
		{
			App_Start.NHibernateProfilerBootstrapper.PreStart();

			var cfg = new Configuration();
			cfg.DataBaseIntegration(properties =>
			{
				properties.Dialect<SQLiteDialect>();
				properties.ConnectionString = @"Data Source=:memory:";
			});
			cfg.AddAssembly(typeof(Building).Assembly);

			// secret: make nhibernate faster toggle
			cfg.SetProperty("default_batch_fetch_size", "15");

			cfg.Cache(properties =>
			{
				properties.UseQueryCache = true;
				properties.Provider<HashtableCacheProvider>();
			});

			var sessionFactory = cfg.BuildSessionFactory();

			using (var con = new SQLiteConnection("Data Source=:memory:"))
			{
				con.Open();

				new SchemaExport(cfg).Execute(false, true, false, con, null);

				using (var session = sessionFactory.OpenSession(con))
				using (var tx = session.BeginTransaction())
				{
					session.Save(new MaintenanceJob
					{
						At = DateTime.Now
					});

					tx.Commit();
				}

				using (var session = sessionFactory.OpenSession(con))
				using (var tx = session.BeginTransaction())
				{
					Console.WriteLine(session.Load<MaintenanceJob>(1).At);
					tx.Commit();
				}
			}

			

		}

		public class MyResult
		{
			public bool Barks;
			public int Count;
		}
	}

	public class BuildingAuditListener : IPreUpdateEventListener, IPreInsertEventListener
	{
		public bool OnPreUpdate(PreUpdateEvent @event)
		{
			var building = @event.Entity as Building;
			if (building == null)
				return false;

			var now = DateTime.Now;
			building.ModifiedAt = now;
			SetPropertyOnState(@event.Persister, @event.State, "ModifiedAt", now);
			return false;
			return false;
		}

		public bool OnPreInsert(PreInsertEvent @event)
		{
			var building = @event.Entity as Building;
			if (building == null)
				return false;

			var now = DateTime.Now;
			building.CreatedAt = now;
			SetPropertyOnState(@event.Persister, @event.State, "CreatedAt", now);
			building.ModifiedAt = now;
			SetPropertyOnState(@event.Persister, @event.State, "ModifiedAt", now);
			return false;
		}

		private static void SetPropertyOnState(IEntityPersister persister, object[] state, string name, DateTime value)
		{
			var indexOf = Array.IndexOf(persister.PropertyNames, name);
			state[indexOf] = value;
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

			@event.Session
				.CreateQuery("update MaintenanceJob j set j.Building = :new where j.Building = :old ")
				.SetParameter("old", building)
				.SetParameter("new", @event.Session.Load<Building>(1))
				.ExecuteUpdate();

		}
	}


	public class BarkingDogsWillNotBeDeleted : IDeleteEventListener
	{
		public void OnDelete(DeleteEvent @event)
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

	public class AuditIntercepter : EmptyInterceptor
	{
		public override SqlString OnPrepareStatement(SqlString sql)
		{
			Console.WriteLine(sql);
			return sql;
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
