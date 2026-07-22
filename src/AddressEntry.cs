using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Siemens.Engineering;
using Siemens.Engineering.HW;


[Serializable]
public class AddressEntry
{
    public string Path { get; set; } = string.Empty;
    public int StartAddr { get; set; }
    public int Length { get; set; }

    // ---------- Single object ----------

    public string ToXml()
    {
        var serializer = new XmlSerializer(typeof(AddressEntry));
        using (var writer = new StringWriter())
        {
            serializer.Serialize(writer, this);
            return writer.ToString();
        }
    }

    public static AddressEntry FromXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(AddressEntry));
        using (var reader = new StringReader(xml))
        {
            return (AddressEntry)serializer.Deserialize(reader);
        }
    }

    // ---------- List helpers ----------

public static string ListToXml(List<AddressEntry> entries)
{
    if (entries == null)
        throw new ArgumentNullException(nameof(entries));

    var root = new XmlRootAttribute("AddressEntries");
    var serializer = new XmlSerializer(typeof(List<AddressEntry>), root);

    var settings = new XmlWriterSettings
    {
        Encoding = new UTF8Encoding(false), // ✅ UTF‑8 without BOM
        Indent = true,
        OmitXmlDeclaration = false
    };

    using (var stream = new MemoryStream())
    using (var writer = XmlWriter.Create(stream, settings))
    {
        serializer.Serialize(writer, entries);
        writer.Flush();

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}

public static List<AddressEntry> ListFromXml(string xml)
{
    if (string.IsNullOrWhiteSpace(xml))
        throw new ArgumentException("XML cannot be null or empty.", nameof(xml));

    var root = new XmlRootAttribute("AddressEntries");
    var serializer = new XmlSerializer(typeof(List<AddressEntry>), root);

    using (var reader = new StringReader(xml.TrimStart('\uFEFF'))) // strip BOM if present
    {
        return (List<AddressEntry>)serializer.Deserialize(reader);
    }
}
}


[Serializable]
[XmlRoot("IpAddrEntry")]
public class IpAddrEntry
{
    public string path;
    public string deviceName;
    public string ipAddress;
    public string profinetDeviceName;
    public string profinetCleanDeviceName;

    public static string ListToXml(List<IpAddrEntry> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        var root = new XmlRootAttribute("IpAddrEntries");
        var serializer = new XmlSerializer(typeof(List<IpAddrEntry>), root);

        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
            OmitXmlDeclaration = false
        };

        using (var stream = new MemoryStream())
        using (var writer = XmlWriter.Create(stream, settings))
        {
            serializer.Serialize(writer, entries);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    public static List<IpAddrEntry> ListFromXml(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            throw new ArgumentException("XML cannot be null or empty.", nameof(xml));

        var root = new XmlRootAttribute("IpAddrEntries");
        var serializer = new XmlSerializer(typeof(List<IpAddrEntry>), root);

        using (var reader = new StringReader(xml.TrimStart('\uFEFF')))
        {
            return (List<IpAddrEntry>)serializer.Deserialize(reader);
        }
    }
}

