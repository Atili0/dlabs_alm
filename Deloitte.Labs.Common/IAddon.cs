namespace Deloitte.Labs
{
    public interface IAddonCore
    {
        PluginAssembly _pluginAssembly { get; set; }
        ObjectAddon _addon { get; set; }

        void CreateAssembly();
        void LoadAssembly();
        void CreatePluginType();
    }
}
