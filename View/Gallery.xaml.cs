using PixabayWPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PixabayWPF.View
{
    /// <summary>
    /// Interaction logic for Gallery.xaml
    /// </summary>
    public partial class Gallery : Window
    {
        readonly GalleryViewModel viewModel = new GalleryViewModel();

        public Gallery()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = lb.SelectedIndex;
            var item = lb.Items.GetItemAt(index);
            lb.ScrollIntoView(item);
        }
    }
}
