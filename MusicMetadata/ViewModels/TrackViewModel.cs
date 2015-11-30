using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MusicMetadata.ViewModels
{
    class TrackViewModel : ViewModelBase
    {
        public TrackViewModel()
        {
            Debug.WriteLine("[{0}] .ctor TrackViewModel", Thread.CurrentThread.ManagedThreadId);
        }

        public string Id { get; set; }

        private int _trackNumber;
        public int TrackNumber
        {
            get { return _trackNumber; }
            set
            {
                if (value != _trackNumber)
                {
                    _trackNumber = value;
                    RaisePropertyChanged();
                }
            }
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
    }
}
