using System.Collections.Generic;

namespace Chess3D.Dependency
{
	public class InjectionContext
	{
		protected Dictionary<string, object> context = new Dictionary<string, object>();

		public void Inject<T>(string name, T value)
		{
			context.Add(name, value);
		}

		public T Resolve<T>(string name)
		{
			return (T)context[name];
		}

		public void Clear()
		{
			context.Clear();
		}
	}
}
