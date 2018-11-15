namespace Deloitte.Labs
{
    using Microsoft.Xrm.Tooling.Connector;
    using System;

    public class ObjectAddon
    {
        public CrmServiceClient ServiceClient { get; set; }
        public PConfig o_Config { get; set; }
        public Type o_Type { get; set; }
        public Stepimage[] l_Stepimage { get; set; }
        public Guid g_Assembly { get; set; }
        public Guid g_PluginType { get; set; }
        public Guid g_MessageId { get; set; }
        public Guid g_FilterId { get; set; }
        public Guid g_MessageStep { get; set; }
        public String  s_Event { get; set; }
        public string s_EntityName { get; set; }
    }
}
