using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using StockApp.Commands;
using StockApp.Models;
using StockApp.Repositories;

namespace StockApp.ViewModels
{

    public class TipHistoryViewModel
    {
        private readonly IMessagesRepository messagesRepository;
        private readonly ITipsRepository tipsRepository;
        private User? selectedUser;

        public ObservableCollection<Message> MessageHistory { get; private set; }

        public ObservableCollection<Tip> TipHistory { get; private set; }

        public ICommand AddTipCommand { get; private set; }


        public TipHistoryViewModel(IMessagesRepository messagesRepository, ITipsRepository tipsRepository)
        {
            this.messagesRepository = messagesRepository;
            this.tipsRepository = tipsRepository;
            this.MessageHistory = [];
            this.TipHistory = [];
            this.AddTipCommand = new RelayCommand((object sender) => { _ = this.AddTip(); }, (object sender) => true);
        }

        private async Task AddTip()
        {
            if (this.selectedUser == null) return;
            if (this.selectedUser.CreditScore < 600)
            {
                await this.tipsRepository.GiveLowBracketTipAsync(this.selectedUser.CNP);
            }
            else
            if (this.selectedUser.CreditScore < 700)
            {
                await this.tipsRepository.GiveMediumBracketTipAsync(this.selectedUser.CNP);
            }
            else
            {
                await this.tipsRepository.GiveHighBracketTipAsync(this.selectedUser.CNP);
            }

            await this.LoadUserData(this.selectedUser);
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
                this.selectedUser = user;
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
