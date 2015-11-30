using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MusicMetadata.ViewModels
{
    class DiscViewModel : ViewModelBase
    {
        public DiscViewModel()
        {
            Debug.WriteLine("[{0}] .ctor DiscViewModel", Thread.CurrentThread.ManagedThreadId);
        }

        private int _discNumber;
        public int DiscNumber
        {
            get { return _discNumber; }
            set
            {
                if (value != _discNumber)
                {
                    _discNumber = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _accurateRipDiscId;
        public string AccurateRipDiscId
        {
            get { return _accurateRipDiscId; }
            set
            {
                if (value != _accurateRipDiscId)
                {
                    _accurateRipDiscId = value;
                    RaisePropertyChanged();
                }
            }
        }

        private TrackViewModelCollection _tracks = new TrackViewModelCollection();
        public TrackViewModelCollection Tracks { get { return _tracks; } }
    }
}
