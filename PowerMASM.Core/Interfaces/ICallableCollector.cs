using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.Interfaces;
public class ICallableCollector {
	public List<ICallable> Callables { get; set; } = new();
	public ICallableCollector() { }
	public List<string> GetCallableNames() => Callables.Select(c => c.Name).ToList();
	public ICallable GetCallableByName(string name)
	{
		// better debugging info
		List<ICallable> callables = null;
		try
		{
			callables = Callables.Where(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
		}
		catch (Exception ex)
		{
			Console.WriteLine();
			//throw new Exception($"Error while searching for callable '{name}': {ex.Message}", ex);
		}
		if (callables.Count == 0)
		{
			throw new Exception($"No callable found with the name '{name}'.");
		}
		return callables.First();
	}

    public List<ICallable> GetCallablesByParameterCount(int count) => Callables.Where(c => c.ParameterCount == count).ToList();
	public void Collect() {
		// get the current assembly and the PowerMASM assembly
		var assembly = Assembly.GetExecutingAssembly();
		try {
			var types = assembly.GetTypes().Where(t => typeof(ICallable).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
			foreach (var type in types) {
				try {
					if (Activator.CreateInstance(type) is ICallable instance) {
						Callables.Add(instance);
					}
				} catch (Exception ex) {
					// Handle exceptions (e.g., log them)
					Console.WriteLine($"Error creating instance of {type.FullName}: {ex.Message}");
				}
			}
		} catch (ReflectionTypeLoadException ex) {
			foreach (var loaderException in ex.LoaderExceptions) {
				Console.WriteLine($"Loader Exception: {loaderException.Message}");
			}
		} catch (Exception ex) {
			Console.WriteLine($"Error collecting callables: {ex.Message}");
		}

		// uncomment to return the list of callables
		//return Callables;
	}

}
