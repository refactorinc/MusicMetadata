using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MusicMetadata
{
    class FolderViewModel : ViewModelBase
    {
        static internal FolderViewModel Create(DirectoryInfo directoryInfo)
        {
            Debug.WriteLine("Create FolderViewModel on Thread {0}", Thread.CurrentThread.ManagedThreadId);
            return new FolderViewModel(directoryInfo);
        }

        private readonly DirectoryInfo _directoryInfo;

        public FolderViewModel(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
                throw new ArgumentNullException("directoryInfo");

            _directoryInfo = directoryInfo;

            LoadAlbumMetadata();
        }

        public string Title
        {
            get
            {
                var regexMatch = _regex.Match(_directoryInfo.Name);
                return regexMatch.Success ? regexMatch.Groups["title"].Value : _directoryInfo.Parent.Name;
            }
        }

        public string SubTitle
        {
            get
            {
                var regexMatch = _regex.Match(_directoryInfo.Name);
                return regexMatch.Success ? regexMatch.Groups["subtitle"].Value : _directoryInfo.Name;
            }
        }

        private void LoadAlbumMetadata()
        {
            AlbumMetadata = AlbumMetadataViewModel.Create(_directoryInfo);
        }

        private AlbumMetadataViewModel _albumMetadata;
        public AlbumMetadataViewModel AlbumMetadata
        {
            get { return _albumMetadata; }
            set
            {
                if (_albumMetadata != null)
                {
                    _albumMetadata.Tracks.CollectionChanged -= UpdateHasTracks;
                }
                if (value != _albumMetadata)
                {
                    _albumMetadata = value;
                    RaisePropertyChanged();
                }
                if (_albumMetadata != null)
                {
                    _albumMetadata.Tracks.CollectionChanged += UpdateHasTracks;
                }
            }
        }

        private void UpdateHasTracks(object sender, EventArgs args)
        {
            HasTracks = AlbumMetadata.Tracks.Any();
        }

        private bool _hasTracks;
        public bool HasTracks
        {
            get { return _hasTracks; }
            set
            {
                if (value != _hasTracks)
                {
                    _hasTracks = value;
                    RaisePropertyChanged();
                }
            }
        }

        static readonly Regex _regex = new Regex(REGEX_PATTERN, RegexOptions.Compiled);
        const string REGEX_PATTERN = @"^(?<title>.*?) - (?<subtitle>.*)$";
    }
}
