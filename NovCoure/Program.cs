using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Linq;
using NovCoure.Model;

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
				properties.Dialect<MsSql2008Dialect>();
				properties.SchemaAction = SchemaAutoAction.Update;
				properties.ConnectionString = @"Data Source=.\SqlExpress;Initial Catalog=Jobs;Integrated Security=true";
			});

			cfg.AddAssembly(typeof(Building).Assembly);

			var sessionFactory = cfg.BuildSessionFactory();

			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var emp = session.Load<Employee>(1);

				emp.Attributes["oren"] = "eini";
				emp.Attributes["location"] = "London";

			
				tx.Commit();
			}
		}
	}
}

