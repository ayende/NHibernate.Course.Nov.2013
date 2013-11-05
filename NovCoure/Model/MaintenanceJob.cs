using System;
using System.Collections.Generic;
using NHibernate.Search.Attributes;

namespace NovCoure.Model
{
	[Indexed]
	public class MaintenanceJob
	{
		[DocumentId]
		public virtual int Id { get; set; }

		[IndexedEmbedded] 
		public virtual Building Building { get; set; }
		public virtual DateTime At { get; set; }
		public virtual ICollection<Employee> By { get; set; }
		public virtual IDictionary<string,Employee> EmpsByWork { get; set; }

		public virtual IList<string> Parts { get; set; }
	}
}