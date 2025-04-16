using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace StockApp.Model
{
    public class User
    {
        private string cnp;
        private string username;
        private string description;
        private bool is_moderator;
        private string image; // BASE64 // TODO: update to image??
        private bool is_hidden;

        public User(string cnp, string username, string description, bool isModerator, string image, bool isHidden)
        {
            this.cnp = cnp;
            this.username = username;
            this.description = description;
            this.is_moderator = isModerator;
            this.image = image;
            this.is_hidden = isHidden;
        }

        public string Cnp
        {
            get { return cnp; }
            set { cnp = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public bool IsModerator
        {
            get { return is_moderator; }
            set { is_moderator = value; }
        }

        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        public bool IsHidden
        {
            get { return is_hidden; }
            set { is_hidden = value; }
        }
    }
}
