namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Services.Api;

    public class ChatReportsViewModel
    {
        private readonly IChatReportApiService chatReportService;

        public ObservableCollection<ChatReport> ChatReports { get; set; }

        public ChatReportsViewModel()
        {
            this.ChatReports = new ObservableCollection<ChatReport>();
        }

        public async Task LoadChatReports()
        {
            try
            {
                var reports = await this.chatReportService.GetReportsAsync();
                foreach (var report in reports)
                {
                    this.ChatReports.Add(report);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }
        }
    }
}
