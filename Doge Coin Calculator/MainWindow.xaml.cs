using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.Drawing;
/*Carlos Sandoval
 2/25/2014
 Doge Coin Calculator
 This program will connect to vircurex.com and collect the most recent trade values
 for both Dogecoin and Bitcoin to claclulate the current market value for DogeCoin.*/

namespace Doge_Coin_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VircurexConnection conn;

        public MainWindow()
        {
            conn = new VircurexConnection();

            InitializeComponent();

            //I must be true to the DOGE
            this.FontFamily = new System.Windows.Media.FontFamily("Comic Sans");
        }

        /// <summary>
        /// Make sure that only numeric values are handled.
        /// </summary>
        private void IsTextNumeric(object sender, TextCompositionEventArgs e)
        {
            //Check each input character to make sure it is numeric
            char c = Convert.ToChar(e.Text);

            //If numeric let pass
            if (Char.IsNumber(c))
                e.Handled = false;
            else if(c == '.')
                e.Handled = false;
            else
            {
                //Do not allow it to pass through
                e.Handled = true;
            }

            base.OnPreviewTextInput(e);
        }

        /// <summary>
        /// Check for enter key, so we can skip to calulation
        /// </summary>
        private void CoinTextbox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Calculate_Click(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Request current values from website and calculate values.
        /// </summary>
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            //We dont want to lock up that pretty UI so create background worker to handle work on another thread.
            BackgroundWorker RequestWorker = new BackgroundWorker();

            RequestWorker.DoWork += RequestWorker_DoWork;

            RequestWorker.RunWorkerCompleted += RequestWorker_RunWorkerCompleted;

            ProgressLable.Content = "Calculating";
            ProgressLable.Foreground = new SolidColorBrush(Colors.Red);

            //The background worker is ment to keep the UI responsive so unfortunatly it cannot access anything that lives
            //on the UI thread these values must be passed to it before it can continue.
            RequestWorker.RunWorkerAsync(CoinTextbox.Text);
        }

        //Overide backround workers do work function to attach the work we want done.
        void RequestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            decimal DOGEprice = conn.RequestLastPrice("DOGE", "BTC");
            decimal BTCprice = conn.RequestLastPrice("BTC", "USD");
            decimal amnt = decimal.Parse(e.Argument.ToString());

            /*Objects that live with or are calculated before the background worker need to be passed in. Using a list
             of objects we can pass more then one variable if needed*/
            List<object> args = new List<object>();
            args.Add(amnt);
            args.Add(DOGEprice);
            args.Add(BTCprice);

            e.Result = args;
        }

        //Update UI once we have returned from the background worker.
        void RequestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<object> args = (List<object>)e.Result;
            decimal amnt = (decimal)args[0];
            decimal DOGEprice = (decimal)args[1];
            decimal BTCprice = (decimal)args[2];
            CalculationLabel.Content = string.Format("{0} X {1} X {2} =", amnt, DOGEprice, BTCprice);

            decimal value = amnt * DOGEprice * BTCprice;
            ComboBoxItem typeItem = (ComboBoxItem)DecimalCombo.SelectedItem;
            ValueLabel.Content = Math.Round(value, Int32.Parse(typeItem.Content.ToString()));

            ProgressLable.Content = "Finished";
            ProgressLable.Foreground = new SolidColorBrush(Colors.Green);
        }
    }
}
