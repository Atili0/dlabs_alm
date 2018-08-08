namespace Deloitte.Labs
{
    using System;
    using Microsoft.Xrm.Tooling.Connector;

    public class ObjectImporter
    {
        public String Path { get; set; }
        public String Result { get; set; }
        public Boolean Encrypt { get; set; }
        public String KeyWord { get; set; }
        public String FullPath { get; set; }
    }
}
