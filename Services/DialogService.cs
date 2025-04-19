namespace StockApp.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml.Controls;

    internal class DialogService : IDialogService
    {
        /// <summary>
        /// Shows a message dialog with the specified title and content.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task ShowMessageAsync(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
            };

            await dialog.ShowAsync();
        }
    }
}
