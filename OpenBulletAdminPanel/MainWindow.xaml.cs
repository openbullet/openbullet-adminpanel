using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenBulletAdminPanel
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Settings settings;
        public Uri UsersUri
        {
            get
            {
                var baseUrl = settings.ApiUrl;
                if (baseUrl.EndsWith("/")) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                return new Uri($"{baseUrl}/api/users");
            }
        }

        private Uri UserUri(string key)
        {
            var baseUrl = settings.ApiUrl;
            if (baseUrl.EndsWith("/")) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
            return new Uri($"{baseUrl}/api/users/{key}");
        }

        public MainWindow()
        {
            InitializeComponent();

            settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
            apiTextbox.Text = settings.ApiUrl;
            secretKeyTextbox.Text = settings.SecretKey;
        }

        private void refreshUsersButton_Click(object sender, RoutedEventArgs e)
        {
            if (settings.ApiUrl != "https://example.com")
            {
                try
                {
                    usersListView.Items.Clear();
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.Authorization, settings.SecretKey);
                    var users = wc.DownloadString(UsersUri);
                    foreach (var user in JsonConvert.DeserializeObject<User[]>(users))
                    {
                        usersListView.Items.Add(user);
                    }
                }
                catch (Exception ex) { MessageBox.Show($"Failed to contact the API: {ex.Message}"); }
            }
        }

        private void addUserButton_Click(object sender, RoutedEventArgs e)
        {
            new CreateUser(this).ShowDialog();
        }

        public void AddUser(User user)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.Authorization, settings.SecretKey);
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.UploadString(UsersUri, JsonConvert.SerializeObject(user));
                usersListView.Items.Add(user);
            }
            catch (Exception ex) { MessageBox.Show($"Failed to add the user: {ex.Message}"); }
        }

        private void ApiTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.ApiUrl = apiTextbox.Text;
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
        }

        private void SecretKeyTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.SecretKey = secretKeyTextbox.Text;
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
        }

        private void EditUserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new CreateUser(this, true, usersListView.SelectedItem as User).ShowDialog();
        }

        public void UpdateUser(User user)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.Authorization, settings.SecretKey);
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.UploadString(UserUri(user.Key), "PUT", JsonConvert.SerializeObject(user));
                usersListView.Items.Remove(usersListView.SelectedItem);
                usersListView.Items.Add(user);
            }
            catch (Exception ex) { MessageBox.Show($"Failed to delete the user: {ex.Message}"); }
        }

        private void DeleteUserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = usersListView.SelectedItem as User;
                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.Authorization, settings.SecretKey);
                wc.UploadString(UserUri(user.Key), "DELETE", "");
                usersListView.Items.Remove(user);
            }
            catch (Exception ex) { MessageBox.Show($"Failed to delete the user: {ex.Message}"); }
        }

        private void CopyApiKeyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try { Clipboard.SetText((usersListView.SelectedItem as User).Key); }
            catch { }
        }
    }
}
