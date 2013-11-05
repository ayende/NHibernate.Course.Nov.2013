using System;
using System.Collections;

namespace NovCoure.Model
{
	public class Animal
	{
		public virtual int Version { get; set; }
		public virtual int Id { get; set; }
		public virtual Address Home { get; set; }
		public virtual Address Vet { get; set; }
		public virtual IDictionary Attributes { get; set; }

		public virtual DateTime Expiration { get; set; }
		public virtual string RegistrationId { get; set; }
	}

	public class Address
	{
		public string City { get; set; }
		public string Street { get; set; }
	}

	public class Dog : Animal
	{
		public virtual bool Barks { get; set; }
	}

	public class Cat : Animal
	{
		public virtual bool Annoying { get; set; }
	}
}