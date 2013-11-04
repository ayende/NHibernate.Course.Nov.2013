using System.ComponentModel.Design;
using Iesi.Collections;

namespace NovCoure.Model
{
	public class Building
	{
		public virtual int Id { get; set; } 
		public virtual string ZipCode { get; set; }
		public virtual string Name { get; set; }
	}
}