using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace DropBoxApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string _token = "3UBQst2oblAAAAAAAAAC5WYQmfXcLpukRuewe791v7L7ZnjMrMSuswPjC0bXVEhu";


        public MainWindow()
        {
            InitializeComponent();

            Run(new object { });
        }

          async void Run(object obj)
          {
            using (var dbx = new DropboxClient(_token))
            {
                var list = await  dbx.Files.ListFolderAsync(string.Empty);
                foreach (var item in list.Entries)
                {
                    if(item.IsFolder)
                    listView.Items.Add(item.Name + "/");
                    else
                    listView.Items.Add(item.Name);
                }               
            }
          }


        private void SignIn(object sender, RoutedEventArgs e)
        {
            CheckLogin();     
        }

        private void CheckLogin()
        {
            using (var context = new UserContext())
            {
                foreach (var user in context.Users.ToList())
                {
                    if (user.Login == loginBox.Text)
                    {
                        if (user.Password == passwordBox.Password)
                        {
                            loginGrid.Visibility = Visibility.Collapsed;
                            registerGrid.Visibility = Visibility.Collapsed;
                            mainGrid.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void CheckRegister()
        {
            using (var context = new UserContext())
            {
                foreach (var user in context.Users.ToList())
                {
                    if (user.Login == loginRegister.Text)
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует!", "Ошибка");
                        loginRegister.Text = null;
                        return;
                    }
                }

                if (passwordRegister.Password == repeatPasswordRegister.Password)
                {
                    context.Users.Add(new User { Login = loginRegister.Text, Password = passwordRegister.Password });
                    context.SaveChanges();                 
                }
                registerGrid.Visibility = Visibility.Collapsed;
                    mainGrid.Visibility = Visibility.Visible;
            }
        }


        async Task Upload(DropboxClient dbx, string folder, string file, string content)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var updated = await dbx.Files.UploadAsync(
                    folder + "/" + file,
                    WriteMode.Overwrite.Instance,
                    body: mem);
                MessageBox.Show($"Saved {folder}/{file} rev {updated.Rev}");
            }
        }

        async Task Download(DropboxClient dbx, string folder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(folder + "/" + file))
            {
                var s = response.GetContentAsByteArrayAsync();
                var result = s.Result;
                File.WriteAllBytes(file, result);
                MessageBox.Show("Success!");
            }
        }


        private async void DownloadIn(object sender, RoutedEventArgs e)
        {
            string name = listView.SelectedItem as string;
            await Download(new DropboxClient(_token), "", name);
        }

        private void RegisterButton(object sender, RoutedEventArgs e)
        {
            registerGrid.Visibility = Visibility.Visible;
            loginGrid.Visibility = Visibility.Collapsed;

        }

        private void RegSend(object sender, RoutedEventArgs e)
        {
            CheckRegister();
        }
    }

  
}
