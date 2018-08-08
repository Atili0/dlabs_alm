namespace Deloitte.Labs
{
    using System;
    using System.Text;
    using System.Management.Automation;
    using Microsoft.Xrm.Tooling.Connector;


    /// <summary>
    /// <param type="synopsis"></param>
    /// <para type="description">Also part of the longer cmdlet description.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DataEntityByFetch")]
    public class ExportDataByFetch : Cmdlet
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
        [Alias("e")]
        [Parameter(Mandatory = true, HelpMessage = "Entity who will be extract records")]
        public string Entity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("p")]
        [Parameter(HelpMessage = "Path where outout file will be save" )]
        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("f")]
        [Parameter(Mandatory = true, HelpMessage = "Fetch what will be used for extract data")]
        public string Fetch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("unc")]
        [Parameter(HelpMessage = "Causes the export data to be written as raw (uncompressed) text.")]
        public SwitchParameter Uncompressed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("enc")]
        [Parameter(HelpMessage = "If you want to encript the file")]
        public SwitchParameter Encrypt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ValidateLength(16, 20)]
        [Alias("k")]
        [Parameter(HelpMessage = "Key word for encrypt file")]
        public string KeyWord { get; set; }

        protected override void ProcessRecord()
        {
            ErrorRecord _error;
            try
            {

                byte[] bytes = Encoding.ASCII.GetBytes(this.KeyWord);
                if (bytes.Length < 16)
                {
                    _error = new ErrorRecord(new Exception("You keyword should be > 16 bytes"),
                        "error_bytes",
                        ErrorCategory.InvalidData, this);
                    WriteError(_error);
                    return;
                }


                WriteVerbose("Create object to process");
                ObjectExporter _objectexporter = new ObjectExporter()
                {
                    ServiceClient = this.Conn,
                    Entity = this.Entity,
                    Path = this.Path,
                    Fetch = this.Fetch,
                    Encrypt = this.Encrypt,
                    KeyWord = this.KeyWord
                };

                Export _export = new Export(_objectexporter);
                if(this.Uncompressed)
                    _export.ExportTocompressed();
                else _export.ExportToUncompressed();
                
            }
            catch (System.Exception ex)
            {
                _error = new ErrorRecord(ex, "", ErrorCategory.OpenError, "");
                WriteError(_error);
            }
        }
    }
}