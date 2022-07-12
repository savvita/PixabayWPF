using GalaSoft.MvvmLight.Command;
using PixabayWPF.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PixabayWPF.ViewModel
{
    public class GalleryViewModel : INotifyPropertyChanged
    {
        public PixabayModel Pixabay { get; set; } = new PixabayModel();

        public string? Search { get; set; }

        private DownloadedFile? currentImage;
        public DownloadedFile? CurrentImage
        {
            get => currentImage;
            set
            {
                currentImage = value;
                OnPropertyChanged(nameof(currentImage));
            }
        }

        #region Commands
        private RelayCommand? searchCommand;
        public RelayCommand SearchCommand
        {
            get => searchCommand ?? new RelayCommand(() => SearchImages());
        }

        private RelayCommand? selectionCommand;
        public RelayCommand SelectionCommand
        {
            get => selectionCommand ?? new RelayCommand(SetCurrentImage);
        }

        private RelayCommand? previousCommand;
        public RelayCommand PreviousCommand
        {
            get => previousCommand ?? new RelayCommand(() => GetPreviousImage());
        }

        private RelayCommand? nextCommand;
        public RelayCommand NextCommand
        {
            get => nextCommand ?? new RelayCommand(() => GetNextImage());
        } 
        #endregion


        private void SearchImages()
        {
            if (Search != null)
            {
                Pixabay?.DownloadImagesInfo(Search);
            }
        }


        private void GetPreviousImage()
        {
            if(Pixabay == null || Pixabay.PreviewImages == null)
            {
                return;
            }

            if(CurrentImage == null)
            {
                return;
            }

            DownloadedFile? current = Pixabay.PreviewImages.Where(x => x.PreviewFilePath != null && x.PreviewFilePath.Equals(CurrentImage.PreviewFilePath))
                .FirstOrDefault();

            if(current != null)
            {
                int idx = Pixabay.PreviewImages.IndexOf(current);

                if(idx > 0)
                {
                    CurrentImage = Pixabay.PreviewImages[idx - 1];
                }
            }
        }
        
        private void GetNextImage()
        {
            if (Pixabay == null || Pixabay.PreviewImages == null)
            {
                return;
            }

            if (CurrentImage == null)
            {
                return;
            }

            DownloadedFile? current = Pixabay.PreviewImages.Where(x => x.PreviewFilePath != null && x.PreviewFilePath.Equals(CurrentImage.PreviewFilePath))
                .FirstOrDefault();

            if (current != null)
            {
                int idx = Pixabay.PreviewImages.IndexOf(current);

                if (idx < Pixabay.PreviewImages.Count - 1)
                {
                    CurrentImage = Pixabay.PreviewImages[idx + 1];
                }
            }
        }

        private async void SetCurrentImage()
        {    
            if(CurrentImage == null)
            {
                return;
            }

            if (!File.Exists(CurrentImage.FullImagePath))
            {
                CurrentImage.FullImagePath = GlobalSettings.DownloadImagePath;
                OnPropertyChanged(nameof(CurrentImage));
                CurrentImage.FullImagePath = await Pixabay.DownloadImage(CurrentImage.FullImageURL);
            }

            OnPropertyChanged(nameof(CurrentImage));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
