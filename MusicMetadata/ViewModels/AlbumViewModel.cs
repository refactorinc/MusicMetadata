using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace MusicMetadata.ViewModels
{
    class AlbumViewModel : ViewModelBase
    {
        public AlbumViewModel()
        {
            Debug.WriteLine("[{0}] .ctor AlbumViewModel", Thread.CurrentThread.ManagedThreadId);
        }

        private string _artist;
        public string Artist
        {
            get { return _artist; }
            set
            {
                if (value != _artist)
                {
                    _artist = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int? _year;
        public int? Year
        {
            get { return _year; }
            set
            {
                if (value != _year)
                {
                    _year = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _genre;
        public string Genre
        {
            get { return _genre; }
            set
            {
                if (value != _genre)
                {
                    _genre = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _compilation;
        public bool Compilation
        {
            get { return _compilation; }
            set
            {
                if (value != _compilation)
                {
                    _compilation = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DiscViewModelCollection _discs = new DiscViewModelCollection();
        public DiscViewModelCollection Discs { get { return _discs; } }

        private ICommand _fillArtistWithAlbumArtistCommand;
        public ICommand FillArtistWithAlbumArtistCommand
        {
            get
            {
                return _fillArtistWithAlbumArtistCommand
                    ?? (_fillArtistWithAlbumArtistCommand = new RelayCommand(FillArtistWithAlbumArtist));
            }
        }

        private void FillArtistWithAlbumArtist()
        {
            foreach (var disc in Discs)
            foreach (var track in disc.Tracks)
            {
                track.Artist = Artist;
            }
        }
    }
}
