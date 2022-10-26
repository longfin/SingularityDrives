using System.Reflection;
using AppWithPlugin;
using Libplanet.Action;
using Libplanet.Blocks;

namespace SingularityDrives;

public class DynamicTypeLoader : IActionTypeLoader
{
    private readonly string _basePath;
    private readonly string _assemblyFileName;

    public DynamicTypeLoader(string basePath, string assemblyFileName)
    {
        _basePath = basePath;
        _assemblyFileName = assemblyFileName;
    }

    public IDictionary<string, Type> Load(IPreEvaluationBlockHeader blockHeader)
    {
        var asm = GetAssembly(blockHeader.Index);
        var actionType = typeof(IAction);
        var types = new Dictionary<string, Type>();

        foreach (Type t in asm.GetTypes())
        {
            if (actionType.IsAssignableFrom(t) &&
                ActionTypeAttribute.ValueOf(t) is string actionId)
            {
                types[actionId] = t;
            }
        }

        return types;
    }

    private Assembly GetAssembly(long blockIndex)
    {
        var pluginPath = Path.Join(_basePath, blockIndex % 2 == 0 ? "Lalc" : "Casio");
        var assemblyPath = Path.GetFullPath(Path.Join(pluginPath, _assemblyFileName));
        var context = new PluginLoadContext(assemblyPath);
        return context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath)));
    }
}
