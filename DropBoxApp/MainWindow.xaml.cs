using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Dropbox.Api;
using Dropbox.Api.Files;

namespace DropBoxApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DropboxClient dbx = new DropboxClient("3UBQst2oblAAAAAAAAAC4fKD45c4rbgxpPQ2IrnnEJUQA11dL7vNP1cQaln2_fnh");

        public MainWindow()
        {
            InitializeComponent();
            var task = Task.Run((Func<Task>)Run);
            task.Wait();
        }

         async Task Run()
        {
            using (var dbx = new DropboxClient("3UBQst2oblAAAAAAAAAC4fKD45c4rbgxpPQ2IrnnEJUQA11dL7vNP1cQaln2_fnh"))
            {
                var list = await dbx.Files.ListFolderAsync(string.Empty);
                foreach (var item in list.Entries.Where(i => i.IsFolder)) 
                {
                    dataGrid.Items.Add(item.Name);
                }
              
            }
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
           

            using (var context = new UserContext())
            {
                foreach(var user in context.Users.ToList())
                {
                    if(user.Login == loginBox.Text)
                    {
                        if(user.Password == passwordBox.Password)
                        {
                        
                        }
                    }
                }
            }
        }

        private void RegisterSend(object sender, RoutedEventArgs e)
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
                    context.SaveChangesAsync();
                    registerGrid.Visibility = Visibility.Collapsed;

                }
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
               MessageBox.Show(await response.GetContentAsStringAsync());
            }
        }
    }
}
