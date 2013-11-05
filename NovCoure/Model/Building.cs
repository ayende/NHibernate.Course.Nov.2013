using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Iesi.Collections;

namespace NovCoure.Model
{
	public class Building
	{
		public virtual DateTime CreatedAt { get; set; }
		public virtual DateTime ModifiedAt { get; set; }
		public virtual int Id { get; set; } 
		public virtual string ZipCode { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<MaintenanceJob> Jobs { get; set; }
	}
}