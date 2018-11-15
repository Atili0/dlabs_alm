using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoredConsole;
using ExcelDataReader;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using NLog;
using System.Data;
using System.Net;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace Deloitte.Labs.Test
{
    public class Program
    {
        CrmServiceClient _crmsvc;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        DataSet _dataset;
        string _options;
        OptionSetMetadata _optionMetadata;
        DataRow _row;
        AttributeRequiredLevelManagedProperty _requiredLevel;
        IOrganizationService IOS;

        static void Main(string[] args)
        {
            logger.Info("Se cambia el aspecto de la consola");
            ChangeConsoleColor();

            logger.Info("Se crea el objeto del programa");
            var _program = new Program();

            logger.Info("Conecta al CRM");
            _program.GetConnection();

            ColorConsole.WriteLine("Deloitte Labs.".Yellow().OnBlue(), "Creación de campos.".Cyan().OnMagenta());
            Console.WriteLine("1- Crear picklist");
            Console.WriteLine("2- Get List of Plugins");
            Console.WriteLine("3- Create Plugins");
            var selec = Console.ReadLine();
            logger.Info($"Seleccion de datos { selec }");
            switch (selec)
            {
                case "1":
                    Console.WriteLine("Set the entity to create the field".Yellow().OnBlue());
                    var _entity = Console.ReadLine();
                    _program.ReadExcel();
                    _program.CreatePickLIst(_entity);
                    break;
                case "2":
                    Console.WriteLine("Get all plugin".Yellow().OnBlue());
                    _program.GetAllPluginsList(_program._crmsvc);
                    break;
                case "3":
                    Console.WriteLine("Create assembly".Yellow().OnBlue());
                    _program.CreateAssembly();
                    break;
            }

            ColorConsole.WriteLine("FINALIZADO".Red().OnYellow());
        }

        private void ReadExcel()
        {
            var _path = @"C:\\CODE\\SVN\\dlabs_alm\\Deloitte.Labs.Test\\Configuration\\Product.xlsx";
            using (var stream = File.Open(_path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    this._dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                }
            }
        }

        private void GetOptions() {
            try
            {
                this._optionMetadata = new OptionSetMetadata();

                var _firstSplit = this._options.Split('|');
                foreach (var item in _firstSplit)
                {
                    var _seconedSplit = item.Split(':');
                    this._optionMetadata.Options.Add(new OptionMetadata(new Microsoft.Xrm.Sdk.Label(_seconedSplit[1], 1033), int.Parse(_seconedSplit[0])));
                }

                this._optionMetadata.IsGlobal = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private void CreatePickLIst(string pEntity)
        {
            foreach (var _rows in _dataset.Tables[0].Rows)
            {
                this._row = ((DataRow)_rows);
                this._options = this._row[7].ToString().Replace("\n", "|");

                GetOptions();

                switch (this._row[5])
                {
                    case "None":
                        _requiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None);
                        break;
                    case "ApplicationRequired":
                        _requiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.ApplicationRequired);
                        break;
                }

                try
                {
                    PicklistAttributeMetadata dxPicklistAttributeMetadata = new PicklistAttributeMetadata
                    {
                        SchemaName = this._row[1].ToString(),
                        DisplayName = new Microsoft.Xrm.Sdk.Label(this._row[2].ToString(), 1033),
                        Description = new Microsoft.Xrm.Sdk.Label(this._row[4].ToString(), 1033),

                        RequiredLevel = this._requiredLevel,
                        OptionSet = this._optionMetadata

                    };
                    AttributeMetadata attributeMetadata = dxPicklistAttributeMetadata;

                    CreateAttributeRequest createBankNameAttributeRequest = new CreateAttributeRequest
                    {
                        EntityName = pEntity,
                        Attribute = attributeMetadata,
                    };

                    _crmsvc.OrganizationServiceProxy.Execute(createBankNameAttributeRequest);
                    ColorConsole.WriteLine($"Create done { this._row[1].ToString() }".Red().OnGreen());
                }
                catch (Exception ex)
                {
                    ColorConsole.WriteLine($"Create has an error {ex.Message}".Yellow().OnDarkRed());
                    logger.Error(ex.Message);
                }
            }
        }

        public void GetConnection()
        {
            logger.Info("Conectando al CRM");
            ColorConsole.WriteLine("Conectando al D365".Cyan().OnRed());
            var s1 = $@"Url=https://rciberiauat.crm4.dynamics.com/;
            AuthType=Office365;
            UserName=admin.iberia@rccrmeu.onmicrosoft.com;
            Password=Deloitte.2018$";


            _crmsvc = new CrmServiceClient(s1);

            if (_crmsvc.IsReady)
                ColorConsole.WriteLine("Conectado".Red().OnGreen());
            else
                ColorConsole.WriteLine($"Falla en la conexión {_crmsvc.LastCrmError}".Yellow().OnRed());


            //     _crmsvc = new CrmServiceClient(
            //new NetworkCredential("SVCCRM21@rcad.net", "Mars1234", "rcad.net"),
            //Microsoft.Xrm.Tooling.Connector.AuthenticationType.AD,
            // "crm.extranet.royalcanin.org", "", "RCESPB2B", useSsl: true);


            //IOS = _crmsvc.OrganizationWebProxyClient != null ? _crmsvc.OrganizationWebProxyClient : (IOrganizationService)_crmsvc.OrganizationServiceProxy;

            //if (IOS == null)
            //    throw new Exception("No es posible conectar con CRM");
        }

        private static void ChangeConsoleColor()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public List<PluginObject> GetAllPluginsList(CrmServiceClient configuration)
        {
            var lstPlugins = new List<PluginObject>();

            using (var lServiceContext = new CrmServiceContext(configuration.OrganizationServiceProxy))
            {
                var arPlugin = from plugin in lServiceContext.PluginAssemblySet
                               where plugin.Content != null
                               where plugin.IsManaged == true
                               select plugin;


                lstPlugins.AddRange(arPlugin.Select(entity => new PluginObject
                {
                    Content = entity.Attributes["content"].ToString(),
                    Name = entity.Attributes["name"].ToString(),
                    PluginAssemblyId = new Guid(entity.Attributes["pluginassemblyid"].ToString()),
                    Componentstate = GetComponentName(((OptionSetValue)entity.Attributes["componentstate"]).Value),
                    Customizationlevel = entity.Attributes["customizationlevel"].ToString(),
                    Introducedversion = entity.Attributes["introducedversion"].ToString(),
                    Ismanaged = entity.Attributes["ismanaged"].ToString(),
                    Isolationmode = GetInsolationName(((OptionSetValue)entity.Attributes["isolationmode"]).Value),
                    Solutionid = new Guid(entity.Attributes["solutionid"].ToString()),
                    Id = entity.Id
                    
                }));

                //var _pluginStep = (from step in lServiceContext.SdkMessageProcessingStepSet
                //                  select step).ToArray();

                foreach (PluginObject item in lstPlugins)
                {
                   //var fffff = from i in lServiceContext.SdkMessageFilterSet
                   //            where i.
                }
                //6b86bd92-e880-e011-bba4-00155da91e01
            }
            return lstPlugins;
        }

        public void CreateAssembly() {
            PConfig _pconfig = JsonConvert.DeserializeObject<PConfig>(File.ReadAllText(@"C:\CODE\SVN\dlabs_alm\Deloitte.Labs.PowerShell\Config\PConfig.json"));

            ObjectAddon _objectaddon = new ObjectAddon() {
                ServiceClient = _crmsvc,
                o_Config = _pconfig

            };

            IAddonCore _addconre = new AddonCore();
            _addconre._addon = _objectaddon;
            _addconre.LoadAssembly();
            _addconre.CreateAssembly();
            _addconre.CreatePluginType();

        }

        private static string GetComponentName(int value)
        {
            switch (value)
            {
                case 0:
                    return "Published";
                    break;
                case 1:
                    return "Unpublished";
                    break;
                case 2:
                    return "Deleted";
                    break;
                case 3:
                    return "DeleteUnpublished";
                    break;
            }
            return string.Empty;
        }

        private static string GetInsolationName(int value)
        {
            switch (value)
            {
                case 1:
                    return "None";
                    break;
                case 2:
                    return "Sandbox";
                    break;
            }
            return string.Empty;
        }

    }

    
}
