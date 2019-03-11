namespace Deloitte.Labs
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class AddonCore : IAddonCore
    {
        public ObjectAddon _addon { get; set; }
        public PluginAssembly _pluginAssembly { get; set; }

        public EntityCollection _ExistAssembly { get; set; }
        public EntityCollection _ExistPlyginType { get; set; }
        public EntityCollection _ExistProcessStep { get; set; }



        public void CreateAssembly()
        {
            RetrievePluginAssemblyByName(this._addon.o_Config.D365_DLL.name);
            if (this._ExistAssembly.Entities.Count > 0)
            {
                _addon.g_Assembly = this._ExistAssembly.Entities[0].Id;
                this._pluginAssembly.Id = this._ExistAssembly.Entities[0].Id;
                this._addon.ServiceClient.Update(this._pluginAssembly);
            }
            else
                _addon.g_Assembly = this._addon.ServiceClient.Create(this._pluginAssembly);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSecureConfig"></param>
        private void CreateSecureConfiguration(string pSecureConfig) {
            SdkMessageProcessingStepSecureConfig _secureStep = new SdkMessageProcessingStepSecureConfig() {
                SdkMessageProcessingStepSecureConfigId = this._addon.g_MessageStep,
                SecureConfig = pSecureConfig
            };

            this._addon.ServiceClient.Create(_secureStep);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_Item"></param>
        private void RetrieveStep(Step p_Item)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression("sdkmessageprocessingstep")
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria = {
                    Conditions = {
                        new ConditionExpression("eventhandler", ConditionOperator.Equal,this._addon.g_PluginType),
                        new ConditionExpression("name", ConditionOperator.Equal,p_Item.name),
                        new ConditionExpression("sdkmessageid", ConditionOperator.Equal,this._addon.g_MessageId),
                        new ConditionExpression("mode", ConditionOperator.Equal,int.Parse(p_Item.mode)),
                        new ConditionExpression("stage", ConditionOperator.Equal,int.Parse(p_Item.stage)),
                        new ConditionExpression("rank", ConditionOperator.Equal,1),
                    }
                }
                };

                //  if (xmlReader["PrimaryEntityName"] != null && xmlReader["PrimaryEntityName"] != "")
                //      queryExpression.get_Criteria().AddCondition("sdkmessagefilterid", (ConditionOperator)0, new object[1]
                //      {
                //(object) messageFilterId
                //      });

                this._ExistProcessStep = this._addon.ServiceClient.RetrieveMultiple((QueryBase)queryExpression);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

                this.RetrieveStep(item);
                if (this._ExistProcessStep.Entities.Count > 0) {
                    _messageStep.Id = this._ExistProcessStep.Entities[0].Id;
                    this._addon.g_MessageStep = this._ExistProcessStep.Entities[0].Id;
                    this._addon.ServiceClient.Update(_messageStep);
                }
                else
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

        /// <summary>
        /// 
        /// </summary>
        private void CreateStepImage()
        {
            try
            {
                foreach (var item in this._addon.l_Stepimage)
                {
                    Entity pluginSetpImageToBeUpdated = new Entity("sdkmessageprocessingstepimage");
                    pluginSetpImageToBeUpdated.Attributes["name"] = item.entityalias;
                    pluginSetpImageToBeUpdated.Attributes["attributes"] = item.attributes;
                    pluginSetpImageToBeUpdated.Attributes["entityalias"] = item.entityalias;
                    pluginSetpImageToBeUpdated.Attributes["messagepropertyname"] = "Id";
                    pluginSetpImageToBeUpdated.Attributes["imagetype"] = new OptionSetValue(int.Parse(item.imagetype));
                    pluginSetpImageToBeUpdated.Attributes["sdkmessageprocessingstepid"] = new EntityReference(SdkMessageProcessingStep.EntityLogicalName, this._addon.g_MessageStep);


                    Guid _guid = Guid.NewGuid();
                    pluginSetpImageToBeUpdated.Attributes["sdkmessageprocessingstepimageid"] = _guid;
                    this._addon.ServiceClient.Create(pluginSetpImageToBeUpdated);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void RetrievePluginType(string p_PluginTypeName)
        {
            QueryExpression queryExpression = new QueryExpression("plugintype")
            {
                ColumnSet = new ColumnSet(true),
                Criteria = {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, p_PluginTypeName)
                    }
                }
            };

            this._ExistPlyginType = this._addon.ServiceClient.RetrieveMultiple(queryExpression);
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

                    this.RetrievePluginType(item.name);

                    if (_ExistPlyginType.Entities.Count > 0)
                    {
                        _pluginType.Id = _ExistPlyginType.Entities[0].Id;
                        this._addon.g_PluginType = _ExistPlyginType.Entities[0].Id;
                        this._addon.ServiceClient.Update(_pluginType);
                    }
                    else {
                        this._addon.g_PluginType = this._addon.ServiceClient.Create(_pluginType);
                    }

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

        private void RetrievePluginAssemblyByName(string pluginAssemblyName)
        {
            QueryExpression queryExpression = new QueryExpression("pluginassembly") {
                ColumnSet = new ColumnSet(true),
                Criteria = {
                    Conditions = {
                        new ConditionExpression("name", ConditionOperator.Equal, pluginAssemblyName)
                    }
                }
            };

            this._ExistAssembly =  this._addon.ServiceClient.RetrieveMultiple(queryExpression);
        }

    }
}
