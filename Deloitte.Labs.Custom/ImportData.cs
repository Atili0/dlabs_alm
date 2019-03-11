namespace Deloitte.Labs.Custom
{
    using Microsoft.Xrm.Tooling.Connector;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Push, "ImportData")]
    public class ImportData : Cmdlet
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
        [Alias("e")]
        [Parameter(Mandatory = true, HelpMessage = "Entity who will be extract records")]
        public string Entity { get; set; }

        protected override void ProcessRecord()
        {
            ErrorRecord _error;
            try
            {
                WriteVerbose("Create object to process");

                Super _super = new Super()
                {
                    ServiceClient = Conn,
                    Entity = Entity,
                    Path = Path
                };

                IImport _import = new Import();
                _import.SetImport(_super);
                _import.ImportaData();
            }
            catch (System.Exception ex)
            {
                _error = new ErrorRecord(ex, "", ErrorCategory.OpenError, "");
                WriteError(_error);
            }
        }
    }
}