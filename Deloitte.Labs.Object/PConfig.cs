namespace Deloitte.Labs
{
    public class PConfig
    {
        public D365_DLL D365_DLL { get; set; }

    }



    public class Rootobject
    {
        public D365_DLL D365_DLL { get; set; }
    }

    public class D365_DLL
    {
        public string name { get; set; }
        public string path { get; set; }
        public Type[] Type { get; set; }
    }

    public class Type
    {
        public string TypeName { get; set; }
        public string name { get; set; }
        public string friendlyname { get; set; }
        public string description { get; set; }
        public Step[] Step { get; set; }
    }

    public class Step
    {
        public string entityname { get; set; }
        public string filterattributes { get; set; }
        public string eventname { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string mode { get; set; }
        public string deletestatus { get; set; }
        public string unsecure_configuration { get; set; }
        public Secure_Configuration secure_configuration { get; set; }
        public string stage { get; set; }
        public string rank { get; set; }
        public string impersonatinguserid { get; set; }
        public string sdkmessagefilterid { get; set; }
        public string supporteddeployment { get; set; }
        public Stepimage[] StepImage { get; set; }
        public string plugintypeid { get; set; }
        public string sdkmessageid { get; set; }
        public string configuration { get; set; }
    }

    public class Secure_Configuration
    {
        public string config { get; set; }
    }

    public class Stepimage
    {
        public string name { get; set; }
        public string entityalias { get; set; }
        public string imagetype { get; set; }
        public string attributes { get; set; }
    }



}
