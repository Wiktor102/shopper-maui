using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ShopperMaui.Helpers;

public static class XmlSerializationHelper
{
    public static string SerializeToXml<T>(T obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var serializer = new XmlSerializer(typeof(T));
        using var stringWriter = new Utf8StringWriter();
        serializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }

    public static T? DeserializeFromXml<T>(string xml) where T : class
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            return null;
        }

        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xml);
        try
        {
            return serializer.Deserialize(reader) as T;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
