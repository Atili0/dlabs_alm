namespace Deloitte.Labs
{
    using System;
    using Microsoft.Xrm.Tooling.Connector;

    public class Super : ObjectImporter
    {
        public String Entity { get; set; }
        public String Fetch { get; set; }
        public CrmServiceClient ServiceClient { get; set; }
        
    }
}
