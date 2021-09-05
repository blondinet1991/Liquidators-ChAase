using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;


public static class Parser
{
    public static int c=0;

    public static string lastEleName="";

    public static string eleSequence="";

    public static object JsonToDict(string command)
    {
        var a = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(command), new System.Xml.XmlDictionaryReaderQuotas());
        var root = XElement.Load(a);
        c = 0;
        eleSequence="";
        var dict = xEleToObjects(root);

        if (isDict(dict)) {
                return dict;
        } else return null;
        
    }

    public static object xEleToObjects(XElement ele){
        c++;
        if (ele.HasElements) {
            eleSequence = eleSequence + "|GroupName:" + ele.Name.ToString();
            Dictionary<string, object> elements = ele.Elements().ToDictionary(e => e.Name.ToString(), e => xEleToObjects(e));
            return (object)elements;
        } 
        else if (!ele.IsEmpty) {
          eleSequence = eleSequence + "{"+ ele.Name.ToString() + ":" + ele.Value + "}";
         return (object)ele.Value;
        }
        else return null;
    }

    public static bool isDict(object obj){
        return (obj is Dictionary<string, object>);
    }

    public static Dictionary<string, object> toDict(object obj){
        return (Dictionary<string, object>) obj;
    }


    public static bool extract_int_from_data(Dictionary<string,object> data, string data_type, string ID, out int output){
        var items = Parser.toDict(data[data_type]);
        object out_put;
        var result = items.TryGetValue(ID, out out_put);
        if (result)
            result = result & int.TryParse(out_put.ToString(), out output);
        else output = 0;
        return result;
    }

    

    public static bool extract_int_from_data_v2(Dictionary<string,object> data, string data_type, out int output){
        object outP;
        var result = data.TryGetValue(data_type, out outP);
        if (result)
            output = int.Parse(outP.ToString());
        else output = 0;
        return result;
    }


    public static bool extract_str_from_data_v2(Dictionary<string,object> data, string data_type, out string output){
        object outP;
        var result = data.TryGetValue(data_type, out outP);
        if (result)
            output = outP.ToString();
        else output = "";
        return result;
    }
    public static bool extract_bool_from_data_v2(Dictionary<string,object> data, string data_type, out bool output){
        object outP;
        var result = data.TryGetValue(data_type, out outP);
        if (result)
            output = bool.Parse(outP.ToString());
        else output = false;
        return result;
    }
    public static string ToJson(IEnumerable<string> collection)
    {
        DataContractJsonSerializer ser = new DataContractJsonSerializer(collection.GetType());
        string json;
        using (MemoryStream m = new MemoryStream())
        {
            XmlDictionaryWriter writer = JsonReaderWriterFactory.CreateJsonWriter(m);
            ser.WriteObject(m, collection);
            writer.Flush();

            json = Encoding.UTF8.GetString(m.ToArray());
        }
        return json;
    }

}
