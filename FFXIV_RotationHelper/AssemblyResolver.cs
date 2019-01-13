using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Advanced_Combat_Tracker;

namespace FFXIV_RotationHelper
{
    public class AssemblyResolver : IDisposable
    {
        private IActPluginV1 plugin;

        public AssemblyResolver(IActPluginV1 _plugin)
        {
            plugin = _plugin;

            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            plugin = null;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dir = ActGlobals.oFormActMain?.PluginGetSelfData(plugin)?.pluginFile.DirectoryName;
            Assembly assembly = TryLoadAssembly(dir, args.Name);

            return assembly;
        }

        private Assembly TryLoadAssembly(string dir, string name)
        {
            AssemblyName assembly = new AssemblyName(name);
            string path = Path.Combine(dir, name);
            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }

            return null;
        }
    }
}
