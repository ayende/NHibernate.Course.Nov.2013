namespace NovCoure.Model
{
	public class Animal
	{
		public virtual int Id { get; set; }
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