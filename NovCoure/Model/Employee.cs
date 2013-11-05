using System.Collections.Generic;

namespace NovCoure.Model
{
	public class Employee
	{
		public virtual int Id { get; set; } 
		public virtual string Name { get; set; }
		public virtual ICollection<MaintenanceJob> Jobs { get; set; }
		public virtual IList<string> EmergencyPhoneNumbers { get; set; }
		public virtual IDictionary<string,string>  Attributes { get; set; }

		public virtual object AssoicatedWith { get; set; }
	}
}