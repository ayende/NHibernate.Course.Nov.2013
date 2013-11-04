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
				properties.SchemaAction = SchemaAutoAction.Recreate;
				properties.ConnectionString = @"Data Source=.\SqlExpress;Initial Catalog=Jobs;Integrated Security=true";
			});

			cfg.AddAssembly(typeof(Building).Assembly);

			var sessionFactory = cfg.BuildSessionFactory();

			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var building = new Building
				{
					ZipCode = "ECA 123A",
					Name = "107-110 Fleet St",
					Jobs = new HashSet<MaintenanceJob>()
				};
				var emp = new Employee
				{
					Name = "Prachett"
				};
				var maintenanceJob = new MaintenanceJob
				{
					Building = building,
					At = DateTime.Today.AddDays(1),
					By = new HashSet<Employee>{emp}
				};
				building.Jobs.Add(maintenanceJob);

				session.Save(emp);
				session.Save(building);
				session.Save(maintenanceJob);

				//var q = session.Query<MaintenanceJob>()
				//	.ToList();

				//foreach (var maintenanceJob in q)
				//{
				//	Console.WriteLine(maintenanceJob.Building.Name + " " + maintenanceJob.At);
				//}


				//session.Query<Building>()
				//	.Where(x => x.Jobs.Count > 1)
				//	.ToList();


				tx.Commit();
			}
		}
	}
}

