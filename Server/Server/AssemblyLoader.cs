using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer {
	public class AssemblyLoader : MarshalByRefObject {

		public void RegisterAssembly() {
		}

		public void loadModules(string modpath) {
			Console.WriteLine(modpath);
			try {
				string[] modules = Directory.GetFiles(modpath);
				foreach(string s in modules) {

					if(!s.Substring(s.Length - 4).Equals(".dll"))
						throw new FileLoadException();
					string name = s.Replace(modpath, "").Replace(".dll", "");
					Console.WriteLine(name);
					Assembly a = Assembly.LoadFrom(s);
					Type t = a.GetType(a.GetName().Name + ".Main");
					object c = Activator.CreateInstance(t);
					Program.classes.Add(c);
					Console.WriteLine(Program.classes.Count);
				}
			} catch(Exception e) {
				Console.WriteLine(e.Message + " : " + e.StackTrace);
			}
		}

	}
}
