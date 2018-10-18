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
   Password=Deloitte.2018";


            _crmsvc = new CrmServiceClient(s1);

            if(_crmsvc.IsReady)
                ColorConsole.WriteLine("Conectado".Red().OnGreen());
            else
                ColorConsole.WriteLine($"Falla en la conexión {_crmsvc.LastCrmError}".Yellow().OnRed());
        }

        private static void ChangeConsoleColor()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }

    }

    
}
