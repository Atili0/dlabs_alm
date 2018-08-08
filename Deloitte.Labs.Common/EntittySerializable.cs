// -----------------------------------------------------------------------------------------------------------------------------------------------------------------
//
// Copyright (c) 2016 Bas van de Sande - JourneyIntoCRM - http://journeyintocrm.com 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// -----------------------------------------------------------------------------------------------------------------------------------------------------------------
using System;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Data;
using System.Globalization;
using System.IO;

namespace Deloitte.Labs
{
    public class EntitySerializer
    {
        public string SerializeObject(Entity entity)
        {
            var ds = new DataSet("XmlData");
            var dt = new DataTable(entity.LogicalName);
            ConvertEntityToDataTable(dt, entity);
            ds.Tables.Add(dt);
            return ds.GetXml();
        }

        private void ConvertEntityToDataTable(DataTable dataTable, Entity entity)
        {
            DataRow row = dataTable.NewRow();
            dataTable.Columns.Add("Id");
            row["Id"] = getAttributeValue(entity.Id).ToString();
            foreach (var attribute in entity.Attributes)
            {
                if (!dataTable.Columns.Contains(attribute.Key))
                    dataTable.Columns.Add(attribute.Key);
                row[attribute.Key] = getAttributeValue(attribute.Value).ToString();
            }
            foreach (var fv in entity.FormattedValues)
            {
                if (!dataTable.Columns.Contains(fv.Key + "name"))
                    dataTable.Columns.Add(fv.Key + "name");
                row[fv.Key + "name"] = fv.Value;
            }
            dataTable.Rows.Add(row);
        }

        private string getAttributeValue(object entityValue)
        {
            var objectValue = "";
            var objectType = "";
            var objectReference = "";

            objectType = $"{entityValue.GetType()}";

            switch (entityValue.ToString())
            {
                case "Microsoft.Xrm.Sdk.EntityReference":
                    objectValue = ((EntityReference)entityValue).Id.ToString();
                    objectReference = ((EntityReference)entityValue).LogicalName;
                    break;
                case "Microsoft.Xrm.Sdk.OptionSetValue":
                    objectValue = ((OptionSetValue)entityValue).Value.ToString();
                    break;
                case "Microsoft.Xrm.Sdk.Money":
                    objectValue = Convert.ToString(((Money)entityValue).Value, CultureInfo.InvariantCulture.NumberFormat);
                    break;
                case "Microsoft.Xrm.Sdk.AliasedValue":
                    var av = (Microsoft.Xrm.Sdk.AliasedValue)entityValue;
                    objectValue = getAttributeValue(av.Value);
                    objectReference = $"{av.EntityLogicalName},{av.AttributeLogicalName}";
                    break;
                case "System.Guid":
                    objectValue = entityValue.ToString();
                    break;
                default:
                    if (entityValue.IsNumeric())
                        objectValue = Convert.ToString(entityValue, CultureInfo.InvariantCulture.NumberFormat);
                    else if (entityValue.IsDateTime())
                        objectValue = ((DateTime)entityValue).ToUniversalTime().ToString("u");
                    else
                        objectValue = entityValue.ToString();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(objectReference)) objectReference += ",";

            return $"{objectType}|{objectReference}{objectValue}";
        }


        public static Entity DeserializeObject(string xml)
        {
            Entity entity = null;
            var dataSet = new DataSet();

            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);

            using (MemoryStream ms = new MemoryStream(xmlBytes))
            {
                dataSet.ReadXml(ms);
            }
            if (dataSet.Tables.Count > 0)
            {
                var dt = dataSet.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    entity = new Entity(dt.TableName);
                    var row = dt.Rows[0];

                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName == "Id")
                            entity.Id = (Guid)SetAttributeValue(row[column.ColumnName]);
                        else
                            entity[column.ColumnName] = SetAttributeValue(row[column.ColumnName]);
                    }
                }
            }

            return entity;
        }

        private static object SetAttributeValue(object val)
        {
            if (val != null)
            {
                var input = val.ToString();
                var tokens = input.Split('|');

                if (tokens.Length == 2)
                {

                    switch (tokens[0])
                    {
                        case "Microsoft.Xrm.Sdk.EntityReference":
                            var er = tokens[1].Split(',');
                            if (er.Length == 2) return new EntityReference(er[0], new Guid(er[1]));
                            break;
                        case "Microsoft.Xrm.Sdk.OptionSetValue":
                            var i = int.MinValue;
                            int.TryParse(tokens[1], out i);
                            if (i != int.MinValue) return new OptionSetValue(i);
                            break;
                        case "Microsoft.Xrm.Sdk.Money":
                            var d = Decimal.MinValue;
                            decimal.TryParse(tokens[1], NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out d);
                            if (d != decimal.MinValue) return new Money(d);
                            break;
                        case "Microsoft.Xrm.Sdk.AliasedValue":
                            var av = tokens[1].Split(',');
                            if (av.Length == 3) return new AliasedValue(av[0], av[1], SetAttributeValue(av[2]));
                            break;
                        case "System.Guid":
                            var g = Guid.Empty;
                            Guid.TryParse(tokens[1], out g);
                            if (g != Guid.Empty) return g;
                            break;
                        case "System.DateTime":
                            var dt = DateTime.MinValue;
                            DateTime.TryParse(tokens[1], out dt);
                            if (dt != DateTime.MinValue) return dt.ToUniversalTime();
                            break;
                        case "System.Int32":
                            var i32 = int.MinValue;
                            int.TryParse(tokens[1], out i32);
                            if (i32 != int.MinValue) return i32;
                            break;
                        case "System.Boolean":
                            return (tokens[1].ToUpper() == "TRUE");
                        case "System.Decimal":
                            var sd = Decimal.MinValue;
                            decimal.TryParse(tokens[1], NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out sd);
                            if (sd != decimal.MinValue) return sd;
                            break;
                        default:
                            // all other values
                            return tokens[1].ToString();
                    }
                }
            }
            return null;
        }
    }

    public static class ExtensionMethods
    {
        public static bool IsNumeric(this object obj)
        {
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsDateTime(this object obj)
        {
            return (Type.GetTypeCode(obj.GetType()) == TypeCode.DateTime);
        }

    }

}