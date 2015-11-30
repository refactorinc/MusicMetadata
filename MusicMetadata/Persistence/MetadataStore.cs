using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace MusicMetadata.Persistence
{
    static class MetadataStore
    {
        static readonly object _lock = new object();

        const string ARTIST = "Artist";
        const string TITLE = "Title";
        const string ALBUM = "Album";
        const string TRACK = "Track";
        const string YEAR = "Year";
        const string GENRE = "Genre";
        const string ALBUM_ARTIST = "Album Artist";
        const string COMPILATION = "Compilation";
        const string DISC = "Disc";
        const string ACCURATERIPDISCID = "AccurateRipDiscId";

        static IDictionary<string, Action<MetadataDto, string>> Map = new Dictionary<string, Action<MetadataDto, string>>
        {
            { ARTIST, (data, text) => data.Artist = text },
            { TITLE, (data, text) => data.Title = text },
            { ALBUM, (data, text) => data.Album = text },
            { TRACK, (data, text) => data.Track = text },
            { YEAR, (data, text) => data.Year = text.ToNullableInt32() },
            { GENRE, (data, text) => data.Genre = text },
            { ACCURATERIPDISCID, (data, text) => data.AccurateRipDiscId = text },
            { ALBUM_ARTIST, (data, text) => data.AlbumArtist = text },
            { COMPILATION, (data, text) => data.Compilation = text.ToNullableInt32() },
            { DISC, (data, text) => data.Disc = text },
        };

        static internal MetadataDto Read(string filePath)
        {
            lock (_lock)
            {
                var file = new FileInfo(filePath);
                if (file.Exists)
                {
                    var result = new MetadataDto(filePath);
                    using (var stream = file.OpenRead())
                    {
                        var document = new XmlDocument();
                        document.Load(stream);
                        foreach (XmlNode node in document.SelectSingleNode("/AudioMetadata/SourceFile/IDTags").ChildNodes)
                        {
                            var name = node.Attributes["name"].Value;
                            if (Map.ContainsKey(name))
                            {
                                Map[name].Invoke(result, node.InnerText);
                            }
                        }
                    }
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        static internal void Write(MetadataDto dto)
        {
            lock (_lock)
            {
                using (var stream = new FileStream(dto.Id, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 1;
                    writer.IndentChar = '\t';

                    writer.WriteStartDocument();
                    writer.WriteStartElement("AudioMetadata");
                    writer.WriteStartElement("SourceFile");
                    writer.WriteStartElement("IDTags");

                    writer.WriteMandatoryElement(dto.Artist, ARTIST);
                    writer.WriteMandatoryElement(dto.Title, TITLE);
                    writer.WriteMandatoryElement(dto.Album, ALBUM);
                    writer.WriteMandatoryElement(dto.Track, TRACK);
                    writer.WriteMandatoryElement(dto.Year, YEAR);
                    writer.WriteMandatoryElement(dto.Genre, GENRE);

                    writer.WriteOptionalElement(dto.AccurateRipDiscId, ACCURATERIPDISCID);
                    writer.WriteOptionalElement(dto.AlbumArtist, ALBUM_ARTIST);
                    writer.WriteOptionalElement(dto.Compilation.HasValue && dto.Compilation.Value == 0 ? null : dto.Compilation, COMPILATION);
                    writer.WriteOptionalElement(dto.Disc, DISC);

                    writer.WriteEndDocument();
                }
            }
        }
    }

    static class Int32Extensions
    {
        static internal int? ToNullableInt32(this string value)
        {
            return String.IsNullOrEmpty(value) ? null : (int?)Convert.ToInt32(value);
        }
    }

    static class XmlTextWriterExtensions
    {
        static internal void WriteMandatoryElement(this XmlTextWriter xmlTextWriter, object value, string elementName)
        {
            xmlTextWriter.WriteStartElement(elementName.Replace(" ", String.Empty));
            xmlTextWriter.WriteAttributeString("name", elementName);
            xmlTextWriter.WriteValue(value ?? String.Empty);
            xmlTextWriter.WriteEndElement();
        }

        static internal void WriteOptionalElement(this XmlTextWriter xmlTextWriter, object value, string elementName)
        {
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                WriteMandatoryElement(xmlTextWriter, value, elementName);
            }
        }
    }
}
