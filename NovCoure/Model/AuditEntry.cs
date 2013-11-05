using System;

namespace NovCoure.Model
{
	public class AuditEntry
	{
		public virtual int Id { get; set; }
		public virtual string EntityName { get; set; }
		public virtual int EntityId { get; set; }
		public virtual string Changes { get; set; }
		public virtual DateTime Date { get; set; }
	}
}