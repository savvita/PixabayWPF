using System;
using System.IO;

namespace PixabayWPF
{
    public static class GlobalSettings
    {
        public static string Request { get; set; } = "https://pixabay.com/api/?key=28501253-dc1a8d51bf69d331ee97bb7a5&q=";

        public static string ImageFolderPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "images");

        public static string DownloadImagePath { get; set; } = Path.Combine(Environment.CurrentDirectory, "giphy.gif");
    }
}
