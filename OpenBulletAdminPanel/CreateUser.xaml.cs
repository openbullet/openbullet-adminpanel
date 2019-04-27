using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenBulletAdminPanel
{
    /// <summary>
    /// Logica di interazione per CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Window
    {
        object Caller;
        bool Edit = false;

        public CreateUser(object caller, bool edit = false, User user = null)
        {
            InitializeComponent();

            if (user != null)
            {
                apiKeyTextbox.Text = user.Key;
                groupsTextbox.Text = user.GroupsString;
                ipsTextbox.Text = user.IPsString;
            }

            Edit = edit;
            Caller = caller;
        }

        private void GenerateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            string apiKey = BitConverter.ToString(key).Replace("-", "").ToLower().Substring(0, 32);
            apiKeyTextbox.Text = apiKey;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var user = new User() {
                Key = apiKeyTextbox.Text,
                Groups = groupsTextbox.Text.Split(',').Select(g => g.Trim()).ToArray(),
                IPs = ipsTextbox.Text.Split(',').Select(i => i.Trim()).ToArray()
            };

            if (Edit) (Caller as MainWindow).UpdateUser(user);
            else (Caller as MainWindow).AddUser(user);

            Close();
        }
    }
}
