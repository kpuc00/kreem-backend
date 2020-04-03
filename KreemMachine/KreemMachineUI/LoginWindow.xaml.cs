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
using KreemMachineLibrary;
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

            if (UserLogInIsValid(user))
                DislayMainWindow(user);
            else
                MessageBox.Show("Either email or password is wrong");
        }

        private static bool UserLogInIsValid(User user) =>
            user != null && SecurityContext.HasRole(Role.Administrator, Role.Depot, Role.Manager);
        

        private void DislayMainWindow(User user)
        {
            var window = new MainWindow(user);
            window.Show();

            this.Hide();

            // after 'MainWindow' is closed, also close This Window
            window.Closed += (Sender, E) => this.Close();
        }

        private void EmailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LogInButton_Click(this, new RoutedEventArgs());
            }
        }

        private void SecretPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LogInButton_Click(this, new RoutedEventArgs());
            }
        }
    }
}
