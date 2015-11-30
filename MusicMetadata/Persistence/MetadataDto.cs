using System;

namespace MusicMetadata.Persistence
{
    public class MetadataDto
    {
        public MetadataDto(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Id = id;
        }

        public string Id { get; private set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Track { get; set; }
        public int? Year { get; set; }
        public string Genre { get; set; }
        public string AccurateRipDiscId { get; set; }
        public string AlbumArtist { get; set; }
        public int? Compilation { get; set; }
        public string Disc { get; set; }
    }
}
