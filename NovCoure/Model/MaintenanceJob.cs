using System;
using System.Collections.Generic;

namespace NovCoure.Model
{
	public class MaintenanceJob
	{
		public virtual int Id { get; set; } 
		public virtual Building Building { get; set; }
		public virtual DateTime At { get; set; }
		public virtual ICollection<Employee> By { get; set; }
	}
}