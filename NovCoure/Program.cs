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

			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction())
			{

				session.Query<Animal>().ToList();
				session.Query<Dog>().ToList();
				session.Query<Cat>().ToList();
				tx.Commit();
			}
		}
	}
}

