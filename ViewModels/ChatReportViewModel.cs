using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Services;

namespace StockApp.ViewModels
{
    public class ChatReportsViewModel
    {
        private readonly IChatReportService chatReportService;

        public ObservableCollection<ChatReport> ChatReports { get; set; }

        public ChatReportsViewModel()
        {
            this.ChatReports = new ObservableCollection<ChatReport>();
        }

        public async Task LoadChatReports()
        {
            try
            {
                var reports = this.chatReportService.GetChatReports();
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
