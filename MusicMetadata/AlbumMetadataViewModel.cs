using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace MusicMetadata
{
    class AlbumMetadataViewModel : ViewModelBase
    {
        static internal AlbumMetadataViewModel Create(DirectoryInfo directoryInfo)
        {
            return new AlbumMetadataViewModel(directoryInfo);
        }

        readonly DirectoryInfo _directoryInfo;

        AlbumMetadataViewModel(DirectoryInfo directoryInfo)
        {
            Debug.WriteLine("AlbumMetadataViewModel.ctor on thread {0}", Thread.CurrentThread.ManagedThreadId);

            if (directoryInfo == null)
                throw new ArgumentNullException("directoryInfo");

            _directoryInfo = directoryInfo;

            Tracks.CollectionChanged += Tracks_CollectionChanged;

            LoadTracks();
        }
        
        void Tracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    RaisePropertyChanged("IsBroken");
                    RaisePropertyChanged("IsComplete");
                    break;
            }
        }

        void LoadTracks()
        {
            Tracks.Clear();
            Task.Run(() => FileSystem.QueryMetadataOf(_directoryInfo.FullName))
                .ToObservable()
                .ObserveOnDispatcher()
                .SelectMany(x => x)
                .Select(TrackMetadataViewModel.Create)
                .Subscribe(Tracks.Add);
        }

        public string Album
        {
            get
            {
                if (Tracks.Where(x => !x.IsBroken).Select(x => x.Album).Distinct().Count() == 1)
                {
                    return Tracks.First(x => !x.IsBroken).Album;
                }
                else
                {
                    return "<different>";
                }
            }
            set
            {
                if (value != Album)
                {
                    foreach (var track in Tracks)
                    {
                        track.Album = value;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public int? Year
        {
            get
            {
                if (Tracks.Where(x => !x.IsBroken).Select(x => x.Year).Distinct().Count() == 1)
                {
                    return Tracks.First(x => !x.IsBroken).Year;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != Year)
                {
                    foreach (var track in Tracks)
                    {
                        track.Year = value;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public string Genre
        {
            get
            {
                if (Tracks.Where(x => !x.IsBroken).Select(x => x.Genre).Distinct().Count() == 1)
                {
                    return Tracks.First(x => !x.IsBroken).Genre;
                }
                else
                {
                    return "<different>";
                }
            }
            set
            {
                if (value != Genre)
                {
                    foreach (var track in Tracks)
                    {
                        track.Genre = value;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public string AlbumArtist
        {
            get
            {
                if (Tracks.Where(x => !x.IsBroken).Select(x => x.AlbumArtist).Distinct().Count() == 1)
                {
                    return Tracks.First(x => !x.IsBroken).AlbumArtist;
                }
                else
                {
                    return "<different>";
                }
            }
            set
            {
                if (value != AlbumArtist)
                {
                    foreach (var track in Tracks)
                    {
                        track.AlbumArtist = value;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public int? Compilation
        {
            get
            {
                if (Tracks.Where(x => !x.IsBroken).Select(x => x.Compilation).Distinct().Count() == 1)
                {
                    return Tracks.First(x => !x.IsBroken).Compilation;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != Compilation)
                {
                    foreach (var track in Tracks)
                    {
                        track.Compilation = value;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        private ObservableCollection<TrackMetadataViewModel> _tracks;
        public ObservableCollection<TrackMetadataViewModel> Tracks
        {
            get { return _tracks ?? (_tracks = new ObservableCollection<TrackMetadataViewModel>()); }
        }



        private ICommand _fillArtistWithAlbumArtistCommand;
        public ICommand FillArtistWithAlbumArtistCommand
        {
            get { return _fillArtistWithAlbumArtistCommand ?? (_fillArtistWithAlbumArtistCommand = new RelayCommand(FillArtistWithAlbumArtist)); }
        }

        private void FillArtistWithAlbumArtist()
        {
            foreach (var track in Tracks)
            {
                track.Artist = AlbumArtist;
            }
        }

        public bool IsBroken
        {
            get { return Tracks.Any(x => x.IsBroken); }
        }

        public bool IsComplete
        {
            get { return Tracks.Any() && Tracks.All(x => x.IsComplete); }
        }

        public void Save()
        {
            Tracks
                .ToObservable()
                //.ObserveOn(NewThreadScheduler.Default)
                .Subscribe(x => x.Save(), LoadTracks);
        }
    }
}
