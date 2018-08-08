namespace Deloitte.Labs
{
    using Microsoft.Xrm.Sdk;
    using System.IO;

    public class Import : IImport
    {
        private Super _super;

        public void SetImport(Super pSuper)
        {
            _super = pSuper;
        }

        public void ImportaData()
        {
            string[] files = Common.GetFileOnFolder(_super);
            foreach (var file in files)
            {
                var _lines = File.ReadAllText(file);
                EntityCollection _object = Newtonsoft.Json.JsonConvert.DeserializeObject<EntityCollection>(_lines);
            }
        }
    }
}