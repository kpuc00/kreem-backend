using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        UserService users;

        public LoginWindow()
        {
            InitializeComponent();

            // Run initialization on separate thread so that the UI doesn't hang 
            new Thread( () => users = new UserService() ).Start();
        }



        string Email => EmailTextBox.Text;

        string Password => SecretPasswordBox.Password;

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {


            //If the thread initializing users hasn't yet finished
            //users might still be null, hence safe navigation operator ?.
            var user = users?.AuthenticateByCredentials(Email, Password);

            if (user != null)
                DislayMainWindow(user);
            else
                MessageBox.Show("Either email or password is wrong");
        }

        private void DislayMainWindow(User user)
        {
            var window = new MainWindow(user);
            window.Show();

            this.Hide();

            // after 'MainWindow' is closed, also close This Window
            window.Closed += (Sender, E) => this.Close();
        }
    }
}
