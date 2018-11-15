namespace Deloitte.Labs
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class AddonCore : IAddonCore
    {
        public ObjectAddon _addon { get; set; }
        public PluginAssembly _pluginAssembly { get; set; }

        

        public void CreateAssembly()
        {
            _addon.g_Assembly =  this._addon.ServiceClient.Create(this._pluginAssembly);
        }

        public void GetMessageId()
        {
            using (CrmServiceContext _context = new CrmServiceContext(this._addon.ServiceClient))
            {
                this._addon.g_MessageId = _context.CreateQuery("sdkmessage")
                                                                .Where(s => s.GetAttributeValue<string>("name").Equals(this._addon.s_Event))
                                                                .Select(s => s.GetAttributeValue<Guid>("sdkmessageid"))
                                                                .First();
            }
        }

        public void GetFilterId() {
            using (CrmServiceContext _context = new CrmServiceContext(this._addon.ServiceClient))
            {
                this._addon.g_FilterId = _context.CreateQuery("sdkmessagefilter")
                                                                 .Where(s => s.GetAttributeValue<string>("primaryobjecttypecode").Equals(this._addon.s_EntityName)
                                                                     && s.GetAttributeValue<EntityReference>("sdkmessageid").Id.Equals(this._addon.g_MessageId))
                                                                 .Select(s => s.GetAttributeValue<Guid>("sdkmessagefilterid"))
                                                                 .First();
            }
        }

        private void CreateSecureConfiguration(string pSecureConfig) {
            SdkMessageProcessingStepSecureConfig _secureStep = new SdkMessageProcessingStepSecureConfig() {
                SdkMessageProcessingStepSecureConfigId = this._addon.g_MessageStep,
                SecureConfig = pSecureConfig
            };

            this._addon.ServiceClient.Create(_secureStep);
        }

        private void CreateStep()
        {
            foreach (var item in this._addon.o_Type.Step)
            {
                this._addon.s_Event = item.eventname;
                this._addon.s_EntityName = item.entityname;

                GetMessageId();
                GetFilterId();

                SdkMessageProcessingStep _messageStep = new SdkMessageProcessingStep()
                {
                    Name = item.name,
                    Description = item.description,
                    PluginTypeId = new EntityReference("plugintype", this._addon.g_PluginType),
                    SdkMessageId = new EntityReference("sdkmessage", this._addon.g_MessageId),
                    Configuration = item.unsecure_configuration,
                    Stage = new OptionSetValue(int.Parse(item.stage)),
                    Rank = 1,
                    Mode = new Microsoft.Xrm.Sdk.OptionSetValue(int.Parse(item.mode)),
                    //ImpersonatingUserId = new EntityReference("systemuser", context.UserId),
                    SdkMessageFilterId = new EntityReference("sdkmessagefilter", this._addon.g_FilterId),
                    FilteringAttributes = item.filterattributes
                };

                if (item.mode.Equals((int)Common.ModeAsynPlugin.Asyn))
                    _messageStep.AsyncAutoDelete = Boolean.Parse(item.deletestatus);

                this._addon.g_MessageStep = this._addon.ServiceClient.Create(_messageStep);

                if (!String.IsNullOrEmpty(item.secure_configuration.config))
                    CreateSecureConfiguration(item.secure_configuration.config);

                if (item.StepImage != null)
                {
                    this._addon.l_Stepimage = item.StepImage;
                    CreateStepImage();
                }
            }
        }

        private void CreateStepImage()
        {
            foreach (var item in this._addon.l_Stepimage)
            {
                SdkMessageProcessingStepImage _StepImage = new SdkMessageProcessingStepImage()
                {
                    EntityAlias = item.entityalias,
                    Name = item.name,
                    ImageType = new OptionSetValue(int.Parse(item.imagetype)),
                    IsCustomizable = new BooleanManagedProperty(true),
                    SdkMessageProcessingStepId = new EntityReference(SdkMessageProcessingStep.EntityLogicalName, this._addon.g_MessageStep),
                    Attributes1 = item.attributes,
                    MessagePropertyName = "POST_CRE_UPD_ACCOUNT_AsignarEquipo"
                };
                this._addon.ServiceClient.Create(_StepImage);
            }
        }

        /// <summary>
        /// Create type of plugin
        /// </summary>
        public void CreatePluginType() {        
            foreach (var item in this._addon.o_Config.D365_DLL.Type)
            {
                try
                {
                    PluginType _pluginType = new PluginType()
                    {
                        PluginAssemblyId = new Microsoft.Xrm.Sdk.EntityReference(PluginAssembly.EntityLogicalName, this._addon.g_Assembly),
                        TypeName = $"{this._addon.o_Config.D365_DLL.name}.{item.TypeName}",
                        Name = item.name,
                        FriendlyName = item.friendlyname,
                        Description = item.description
                    };

                    this._addon.g_PluginType =  this._addon.ServiceClient.Create(_pluginType);
                    this._addon.o_Type = item;
                    CreateStep();
                }
                catch (Exception ex)
                {
                    
                }
            }
      
        }

        public void LoadAssembly() {
            //https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/pluginassembly?view=dynamics-ce-odata-9

            Assembly assembly = Assembly.LoadFile(_addon.o_Config.D365_DLL.path);
            PluginAssembly pluginAssembly = new PluginAssembly();
            pluginAssembly.Name = assembly.GetName().Name;
            pluginAssembly.SourceType = new Microsoft.Xrm.Sdk.OptionSetValue(0);
            pluginAssembly.Culture = assembly.GetName().CultureInfo.ToString();
            pluginAssembly.Version = assembly.GetName().Version.ToString();


            //1   None
            //2   Sandbox
            //3   External
            pluginAssembly.IsolationMode = new Microsoft.Xrm.Sdk.OptionSetValue(2);

            if (string.IsNullOrEmpty(pluginAssembly.Culture))
            {
                pluginAssembly.Culture = "neutral";
            }

            byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
            StringBuilder tokenBuilder = new StringBuilder();
            foreach (byte b in publicKeyToken)
            {
                tokenBuilder.Append(b.ToString("x").PadLeft(2, '0'));
            }
            pluginAssembly.PublicKeyToken = tokenBuilder.ToString();

            pluginAssembly.Content = Convert.ToBase64String(
                File.ReadAllBytes(_addon.o_Config.D365_DLL.path));

            this._pluginAssembly =  pluginAssembly;
        }

    }
}
