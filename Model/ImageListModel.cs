using System.Collections.Generic;

namespace PixabayWPF.Model
{
    public class ImageListModel
    {
        public int total { get; set; }

        public int totalHits { get; set; }

        public List<ImageModel>? hits { get; set; }
    }
}
