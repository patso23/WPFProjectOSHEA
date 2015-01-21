/**
 * Programming Test
 * Patrick O'Shea
 * 01/20/2015
 * 
 * Application searches for customer using REST API
 * Implemented Using the RestSharp 3rd party client for REST opererations
 * 
 * I created a simple php based rest API in order to test while the server listed in the requirements was down.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RestSharp; // 3rd party rest library
using System.Xml.Serialization;
using System.IO;

namespace WPFProjectOSHEA
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        // range values
        private int min = 1;
        private int max = 5;

        // popup strings
        private string messageBoxText;
        private string caption;

        // Rest objects
        private RestClient client = new RestClient();
        

        /// <summary>
        /// Class represents the customer xml
        /// </summary>
        [Serializable, XmlRoot(ElementName = "CUSTOMER")]
        [XmlType("CUSTOMER")]
        public class Customer
        {
            [XmlElement("ID")]
            public int Id { get; set; }
            [XmlElement("FIRSTNAME")]
            public string FirstName { get; set; }
            [XmlElement("LASTNAME")]
            public string LastName { get; set; }
            [XmlElement("STREET")]
            public string Street { get; set; }
            [XmlElement("CITY")]
            public string City { get; set; }
        }


        public Window1()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // resize window when results groupbox becomes visible
            SizeToContent = SizeToContent.Height;
        }
        
        /// <summary>
        /// SearchEvent():
        /// Bound to search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SearchEvent(object sender, RoutedEventArgs e)
        {
            int num;
            bool result = int.TryParse(searchTextBox.Text, out num);

            if (result)
            {
                // result is an integer
                this.messageBoxText = num.ToString();
                this.caption = "Result";
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Information;
                    
                // test range
                if (num >= this.min && num <= this.max)
                {
                   
                    // perform request
                    Customer cust = this.getRequest(num);

                    //MessageBox.Show(cust.Id.ToString(), this.caption, button, icon);
                    
                    if (cust != null)
                    {
                        // update search groupbox
                        searchTextBox.Visibility = Visibility.Hidden;
                        searchButton.Visibility = Visibility.Hidden;
                        searchLabel.Content = "Customer " + cust.Id.ToString() + "'s results are below";


                        // update results groupbox
                        firstNameTextBox.Text = cust.FirstName;
                        lastNameTextBox.Text = cust.LastName;
                        streetTextBox.Text = cust.Street;
                        cityTextBox.Text = cust.City;
                        resultsGroupBox.Visibility = Visibility.Visible;
                        searchButton.Visibility = Visibility.Hidden;
                        resetButton.Visibility = Visibility.Visible;    

                    }
                    else
                    {
                        icon = MessageBoxImage.Error;
                        this.caption = "Error";
                        this.messageBoxText = "There was an error retrieving that customer.";
                        MessageBox.Show(this.messageBoxText, this.caption, button, icon);
                    }
                }
                else
                {
                    this.messageBoxText = "Please enter a number within the specified range.";
                    this.caption = "Result";
                    button = MessageBoxButton.OKCancel;
                    icon = MessageBoxImage.Error;
                    MessageBox.Show(this.messageBoxText, this.caption, button, icon);
                }
            }
            else
            {
                // not an integer

                this.messageBoxText = "Input must be a number in the specified range.";
                this.caption = "Result";
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(this.messageBoxText, this.caption, button, icon);
            }
        }


        /// <summary>
        /// ResetEvent():
        /// Bound to reset button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ResetEvent(object sender, RoutedEventArgs e)
        {
            searchLabel.Content = "Please enter a number between 1 and 5";
            searchTextBox.Text = "";
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            streetTextBox.Text = "";
            cityTextBox.Text = "";
            resetButton.Visibility = Visibility.Hidden;  
            resultsGroupBox.Visibility = Visibility.Collapsed;
            searchTextBox.Visibility = Visibility.Visible;
            searchButton.Visibility = Visibility.Visible;
            resetButton.Visibility = Visibility.Hidden;  

        }

        /// <summary>
        /// getRequest():
        /// Calls REST API, returns customer
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public Customer getRequest(int num)
        {

            RestRequest request = new RestRequest("CUSTOMER/{id}", Method.GET);
            client.BaseUrl = "http://192.168.6.59/"; // testing on personal server while other server is down
            
            // sever is currently down
            // client.BaseUrl = "http://www.thomas-bayer.com/sqlrest/";
            
            request.RootElement = "customer";
            request.XmlNamespace = "http://www.w3.org/1999/xlink";
            request.AddUrlSegment("id", num.ToString());
            request.RequestFormat = RestSharp.DataFormat.Xml;
            
            try
            {

                var response = client.Execute<Customer>(request);

                if (response.Content != null)
                {
                    XmlSerializer xml = new XmlSerializer(typeof(Customer));
                    Customer cust = (Customer)xml.Deserialize(new StringReader(response.Content));

                    return cust;
                }
                
                return null;

            }
            catch(Exception e)
            {
                return null;
            }

        }

    }

}
