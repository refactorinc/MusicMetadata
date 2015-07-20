using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace MusicMetadata
{
    class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            Debug.WriteLine("MainWindowViewModel.ctor on Thread {0}", Thread.CurrentThread.ManagedThreadId);

            System.Windows.Application.Current.Exit += (sender, args) =>
                {
                    Save();
                    Settings.Default.DefaultPath = SelectedPath;
                    Settings.Default.Save();
                };

            var defaultPath = Settings.Default.DefaultPath;
            if (!String.IsNullOrEmpty(defaultPath))
            {
                SelectedPath = defaultPath;
            }
            SetFilter();
        }

        public int Top
        {
            get { return Settings.Default.WindowTop; }
            set
            {
                Settings.Default.WindowTop = value;
                RaisePropertyChanged();
            }
        }

        public int Left
        {
            get { return Settings.Default.WindowLeft; }
            set
            {
                Settings.Default.WindowLeft = value;
                RaisePropertyChanged();
            }
        }

        public int Width
        {
            get { return Settings.Default.WindowWidth; }
            set
            {
                Settings.Default.WindowWidth = value;
                RaisePropertyChanged();
            }
        }

        public int Height
        {
            get { return Settings.Default.WindowHeight; }
            set
            {
                Settings.Default.WindowHeight = value;
                RaisePropertyChanged();
            }
        }

        private ICommand _openCommand;
        public ICommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(Open)); }
        }

        private void Open()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                var dialogResult = folderBrowserDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    SelectedPath = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private string _selectedPath;
        public string SelectedPath
        {
            get { return _selectedPath; }
            private set
            {
                if (value != _selectedPath)
                {
                    _selectedPath = value;
                    RaisePropertyChanged();
                    Refresh();
                }
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(Refresh)); }
        }

        private void Refresh()
        {
            //Save();
            LoadFolders();
        }

        private void LoadFolders()
        {
            Folders.Clear();
            Task.Run(() => FileSystem.QueryLeafFoldersOf(SelectedPath))
                .ToObservable()
                .ObserveOnDispatcher()
                .SelectMany(x => x)
                .Select(FolderViewModel.Create)
                .Subscribe(Folders.Add);//, SetFilter);
        }

        private void SetFilter()
        {
            Debug.WriteLine("SetFilter on Thread {0}", Thread.CurrentThread.ManagedThreadId);
            var collectionView = new CollectionViewSource { Source = Folders }.View;
            collectionView.Filter = x => ((FolderViewModel)x).HasTracks;
            var collectionViewLiveShaping = collectionView as ICollectionViewLiveShaping;
            collectionViewLiveShaping.LiveFilteringProperties.Add("HasTracks");
            collectionViewLiveShaping.IsLiveFiltering = true;
            FoldersView = collectionViewLiveShaping;
        }

        private ICollectionViewLiveShaping _foldersView;

        public ICollectionViewLiveShaping FoldersView
        {
            get { return _foldersView; }
            set {
                _foldersView = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<FolderViewModel> _folders = new ObservableCollection<FolderViewModel>();
        public ObservableCollection<FolderViewModel> Folders
        {
            get { return _folders; }
        }

        private FolderViewModel _selectedFolder;
        public FolderViewModel SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                if (value != _selectedFolder)
                {
                    Save();
                    _selectedFolder = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void Save()
        {
            if (SelectedFolder != null && SelectedFolder.AlbumMetadata != null)
            {
                SelectedFolder.AlbumMetadata.Save();
            }
        }
    }
}
