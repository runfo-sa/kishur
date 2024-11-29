using System.Xml.Serialization;

namespace kishur.Models;

[XmlRoot("Cache")]
public record struct Kishur(
    [XmlElement("Start")]
    int Start,

    [XmlElement("End")]
    int End,

    [XmlElement("PrinterIp")]
    string PrinterIp,

    [XmlElement("Day")]
    Day Day
);
