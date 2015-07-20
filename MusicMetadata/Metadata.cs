using System;

namespace MusicMetadata
{
    public class Metadata
    {
        private readonly string _filePath;

        public Metadata(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            _filePath = filePath;
        }

        public string FilePath { get { return _filePath; } }

        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Track { get; set; }
        public int? Year { get; set; }
        public string Genre { get; set; }
        public string AlbumArtist { get; set; }
        public int? Compilation { get; set; }
        public string Disc { get; set; }

        public bool IsBroken { get; set; }
    }
}
