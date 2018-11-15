using System;

namespace Deloitte.Labs.Test
{
    internal interface IPluginObject
    {
        Guid Id { get; set; }
        string Content { get; set; }
        Guid PluginAssemblyId { get; set; }
        string Name { get; set; }
        string Isolationmode { get; set; }
        string Componentstate { get; set; }
        Guid Solutionid { get; set; }
        string Introducedversion { get; set; }
        string Customizationlevel { get; set; }
        bool Ishidden { get; set; }
        string Ismanaged { get; set; }
    }

    public class PluginObject : IPluginObject
    {
        public string Content { get; set; }
        public Guid PluginAssemblyId { get; set; }
        public string Name { get; set; }
        public string Isolationmode { get; set; }
        public string Componentstate { get; set; }
        public Guid Solutionid { get; set; }
        public string Introducedversion { get; set; }
        public string Customizationlevel { get; set; }
        public bool Ishidden { get; set; }
        public string Ismanaged { get; set; }
        public Guid Id { get; set; }
    }
}