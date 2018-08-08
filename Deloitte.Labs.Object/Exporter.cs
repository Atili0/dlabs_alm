namespace Deloitte.Labs
{
    using System;
    using Microsoft.Xrm.Tooling.Connector;

    public class ObjectExporter
    {
        public String Entity { get; set; }
        public String Path { get; set; }
        public String Result { get; set; }
        public String  Fetch { get; set; }
        public CrmServiceClient ServiceClient { get; set; }
        public Boolean Encrypt { get; set; }
        public String KeyWord { get; set; }
        public String FullPath { get; set; }

        public int index { get; set; }
        public string CompletePath { get; set; }
        public String JsonSerializable { get; set; }
        
    }
}
