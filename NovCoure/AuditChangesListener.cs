using System;
using System.Text;
using NHibernate;
using NHibernate.Event;
using NovCoure.Model;

namespace NovCoure
{
	public class AuditChangesListener : IPreUpdateEventListener
	{
		public bool OnPreUpdate(PreUpdateEvent @event)
		{
			if (@event.Entity is AuditEntry)
				throw new InvalidOperationException("You cannot update audit entries!!!!");

			var entityPersister = @event.Persister;

			var sb = new StringBuilder();
			for (int i = 0; i < @event.State.Length; i++)
			{
				var compare = entityPersister.PropertyTypes[i].Compare(@event.State[i], @event.OldState[i], EntityMode.Poco);
				if (compare == 0)
					continue;
				sb.Append(entityPersister.PropertyNames[i])
					.Append(": ")
					.Append(@event.OldState[i])
					.Append(" -> ")
					.Append(@event.State[i])
					.AppendLine();
			}
			using (var child = @event.Session.GetSession(EntityMode.Poco))
			{
				child.Save(new AuditEntry
				{
					Changes = sb.ToString(),
					Date = DateTime.Now,
					EntityId = (int) @event.Id,
					EntityName = @event.Persister.EntityName,
				});
				child.Flush();
			}
			return false;
		}
	}
}