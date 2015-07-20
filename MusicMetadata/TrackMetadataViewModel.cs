using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MusicMetadata
{
    class TrackMetadataViewModel : ViewModelBase
    {
        static internal TrackMetadataViewModel Create(Metadata metadataFile)
        {
            return new TrackMetadataViewModel(metadataFile);
        }

        readonly Metadata _metadataFile;

        public TrackMetadataViewModel(Metadata metadataFile)
        {
            if (metadataFile == null)
                throw new ArgumentNullException("metadataFile");

            _metadataFile = metadataFile;

            PropertyChanged += (sender, args) => { if (args.PropertyName != "IsDirty") IsDirty = true; };
        }

        public string Artist
        {
            get { return _metadataFile.Artist; }
            set
            {
                if (value != _metadataFile.Artist)
                {
                    _metadataFile.Artist = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Title
        {
            get { return _metadataFile.Title; }
            set
            {
                if (value != _metadataFile.Title)
                {
                    _metadataFile.Title = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Album
        {
            get { return _metadataFile.Album; }
            set
            {
                if (value != _metadataFile.Album)
                {
                    _metadataFile.Album = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Track
        {
            get { return _metadataFile.Track; }
        }

        public int? Year
        {
            get { return _metadataFile.Year; }
            set
            {
                if (value != _metadataFile.Year)
                {
                    _metadataFile.Year = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string Genre
        {
            get { return _metadataFile.Genre; }
            set
            {
                if (value != _metadataFile.Genre)
                {
                    _metadataFile.Genre = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string AlbumArtist
        {
            get { return _metadataFile.AlbumArtist; }
            set
            {
                if (value != _metadataFile.AlbumArtist)
                {
                    _metadataFile.AlbumArtist = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public int? Compilation
        {
            get { return _metadataFile.Compilation; }
            set
            {
                if (value != _metadataFile.Compilation)
                {
                    _metadataFile.Compilation = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Disc
        {
            get { return _metadataFile.Disc; }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value != _isDirty)
                {
                    _isDirty = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsBroken
        {
            get { return _metadataFile.IsBroken; }
        }

        public bool IsComplete
        {
            get
            {
                var isIncomplete = IsBroken
                    || String.IsNullOrEmpty(Artist)
                    || String.IsNullOrEmpty(Title)
                    || String.IsNullOrEmpty(Album)
                    || String.IsNullOrEmpty(Track)
                    || Year.HasValue == false
                    || String.IsNullOrEmpty(Genre)
                    || String.IsNullOrEmpty(Disc);
                return !isIncomplete;
            }
        }

        public void Save()
        {
            if (IsDirty)
            {
                Debug.WriteLine("Saving on thread {0}", Thread.CurrentThread.ManagedThreadId);
                FileSystem.Save(_metadataFile);
                IsDirty = false;
            }
        }
    }
}
