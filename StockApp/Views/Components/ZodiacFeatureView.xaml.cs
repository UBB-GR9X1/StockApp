// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace StockApp.Views.Components
{
    using System;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.ViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ZodiacFeatureView : Page
    {
        private readonly ZodiacFeatureViewModel zodiacViewModel;

        public ZodiacFeatureView(ZodiacFeatureViewModel zodiacViewModel)
        {
            this.zodiacViewModel = zodiacViewModel ?? throw new ArgumentNullException(nameof(zodiacViewModel));
            this.DataContext = zodiacViewModel;
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.zodiacViewModel.RunZodiacFeatures();
        }
    }
}
