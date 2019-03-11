namespace Deloitte.Labs.Custom
{
    using Microsoft.Xrm.Tooling.Connector;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Get, "DataEntity")]
    public class Addon : Cmdlet
    {
        /// <summary>
        /// <para type="description">The flavor for the gizmo.</para>
        /// </summary>
        [Alias("c")]
        [Parameter(Mandatory = true, HelpMessage = "CRM Connection")]
        public CrmServiceClient Conn { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Alias("p")]
        [Parameter(HelpMessage = "Path where outout file will be save")]
        public string Path { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Alias("cp")]
        [Parameter(HelpMessage = "Key word for encrypt file")]
        public string ConfigPath { get; set; }

        protected override void ProcessRecord()
        {
            ErrorRecord _error;
            try
            {
                WriteVerbose("Create object to process");
                ObjectAddon _objectAddon = new ObjectAddon()
                {
                    ServiceClient = Conn
                };

                IAddonCore _addon = new AddonCore();
                _addon._addon = _objectAddon;

                //CREAR EL PROCESO PARA EL REGISTRO DEL PLUGIN EN POWERSHELL
            }
            catch (System.Exception ex)
            {
                _error = new ErrorRecord(ex, "", ErrorCategory.OpenError, "");
                WriteError(_error);
            }
        }
    }
}