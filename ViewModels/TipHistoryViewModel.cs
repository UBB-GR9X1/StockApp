using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using StockApp.Models;
using StockApp.Repositories;

namespace StockApp.ViewModels
{

    public class TipHistoryViewModel
    {
        private readonly IMessagesRepository messagesRepository;
        private readonly ITipsRepository tipsRepository;

        public ObservableCollection<Message> MessageHistory { get; private set; }
        public ObservableCollection<Tip> TipHistory { get; private set; }

        public TipHistoryViewModel(IMessagesRepository messagesRepository, ITipsRepository tipsRepository)
        {
            this.messagesRepository = messagesRepository;
            this.tipsRepository = tipsRepository;
            this.MessageHistory = new ObservableCollection<Message>();
            this.TipHistory = new ObservableCollection<Tip>();
        }

        public async Task LoadUserData(User user)
        {
            if (user == null) return;
            try
            {
                List<Message> messages = await this.messagesRepository.GetMessagesForUserAsync(user.CNP);
                List<Tip> tips = await this.tipsRepository.GetTipsForGivenUserAsync(user.CNP);

                this.LoadHistory(messages);
                this.LoadHistory(tips);
            }
            catch (HttpRequestException ex)
            {
                this.MessageHistory.Clear();
                this.TipHistory.Clear();
                // FIXME: Add 404 dialog or error handling
            }
        }

        private void LoadHistory(List<Message> messages)
        {
            this.MessageHistory.Clear();
            foreach (Message message in messages)
            {
                this.MessageHistory.Add(message);
            }
        }

        private void LoadHistory(List<Tip> tips)
        {
            this.TipHistory.Clear();
            foreach (Tip tip in tips)
            {
                this.TipHistory.Add(tip);
            }
        }
    }
}
