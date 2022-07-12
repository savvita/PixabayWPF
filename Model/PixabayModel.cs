using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PixabayWPF.Model
{
    public class PixabayModel : INotifyPropertyChanged
    {
        readonly HttpClient? client = new HttpClient();

        private ImageListModel? imagesListModel;


        private ObservableCollection<DownloadedFile>? previewImages;
        public ObservableCollection<DownloadedFile>? PreviewImages
        {
            get => previewImages;

            set
            {
                previewImages = value;
                OnPropertyChanged(nameof(PreviewImages));
            }
        }

        public PixabayModel()
        {
            if (!Directory.Exists(GlobalSettings.ImageFolderPath))
            {
                Directory.CreateDirectory(GlobalSettings.ImageFolderPath);
            }
        }

        public async Task DownloadImagesInfo(string search)
        {
            string[] words = Regex.Split(search, @"\W+");

            string request = $"{GlobalSettings.Request}{String.Join('+', words)}";

            request += "&per_page=200";

            if (client != null)
            {
                string response = await client.GetStringAsync(request);

                imagesListModel = JsonSerializer.Deserialize<ImageListModel>(response);

                if (imagesListModel == null || imagesListModel == null)
                {
                    PreviewImages = null;
                }
                else
                {
                    PreviewImages = new ObservableCollection<DownloadedFile>();

                    DownloadPreviewImages(0, imagesListModel!.hits!.Count);
                }
            }
        }

        public void DownloadPreviewImages(int startIndex, int length)
        {
            if (imagesListModel == null || imagesListModel.hits == null)
            {
                return;
            }

            startIndex = startIndex > 0 ? startIndex : 0;

            for (int i = startIndex; i < startIndex + length && i < imagesListModel.hits.Count; i++)
            {
                DownloadPreviewImage(i);
            }
        }

        private async void DownloadPreviewImage(int idx)
        {
            if (imagesListModel == null || imagesListModel.hits == null)
            {
                return;
            }

            if (client == null)
            {
                return;
            }

            if (imagesListModel.hits[idx].previewURL != null)
            {
                string fileName = Path.Combine(GlobalSettings.ImageFolderPath, $"preview{idx}{Path.GetExtension(imagesListModel.hits[idx].previewURL)}");

                try
                {
                    byte[] bytes = await client.GetByteArrayAsync(imagesListModel.hits[idx].previewURL);

                    using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    {
                        file.Write(bytes, 0, bytes.Count());
                    }

                    PreviewImages?.Add(new DownloadedFile() { PreviewFilePath = fileName, FullImageURL = imagesListModel.hits[idx].largeImageURL });
                }
                catch { }
            }
        }

        public async Task<string?> DownloadImage(string? uri)
        {
            if (imagesListModel == null || imagesListModel.hits == null)
            {
                return null;
            }

            if (client == null)
            {
                return null;
            }

            if (uri != null)
            {
                string fileName = Path.Combine(GlobalSettings.ImageFolderPath, Path.GetFileName(uri));

                try
                {
                    byte[] bytes = await client.GetByteArrayAsync(uri);

                    using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    {
                        file.Write(bytes, 0, bytes.Count());
                    }

                    return fileName;
                }
                catch { }
            }

            return null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
