using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Src.Model;
using Windows.Web.Http;

namespace StockApp.Services.Api
{
    public interface IChatReportApiService
    {
        Task<List<ChatReport>> GetReportsAsync();

        Task<ChatReport?> GetReportByIdAsync(int id);

        Task<bool> CreateReportAsync(ChatReport report);

        Task<bool> DeleteReportAsync(int id);

        Task<bool> DoNotPunishUser(ChatReport chatReport);

        Task<bool> PunishUser(ChatReport chatReportToBeSolved);
    }
}
