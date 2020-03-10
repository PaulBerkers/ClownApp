using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Clown.Classes;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace Clown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase db = new DataBase();

        public MainWindow()
        {
            InitializeComponent();

            GetAllClowns();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] encode = getJPGFromImageControl(ClownPhoto.Source as BitmapImage);

            db.AddClown(FirstName.Text, LastName.Text, int.Parse(Age.Text), Description.Text, encode);
            MessageBox.Show("Je hebt zojuist de volgende clown toegevoegd: " + FirstName.Text + " " + LastName.Text);
            GetAllClowns();
        }

        private void GetAllClowns()
        {
            try
            {
                lvClowns.Children.Clear();
                lvFoto.Children.Clear();

                SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\mssqllocaldb; 
                                            Initial Catalog=Clowns;Integrated Security=True");

                conn.Open();

                SqlCommand command = conn.CreateCommand();
                command.CommandText = $"SELECT * FROM TheClownTable;";

                SqlDataReader reader = command.ExecuteReader();

                DataTable dtData = new DataTable();
                dtData.Load(reader);

                foreach (DataRow row in dtData.Rows)
                { 
                    StackPanel sp = new StackPanel();
                    sp.Margin = new Thickness(10);

                    Image imPhoto = new Image();
                    imPhoto.Width = 80;
                    imPhoto.Height = 80;
                    imPhoto.HorizontalAlignment = HorizontalAlignment.Right;

                    TextBlock tblName = new TextBlock();
                    tblName.Text += "Naam:                       " + row["Name"].ToString();

                    TextBlock tblLastName = new TextBlock();
                    tblLastName.Text += "Achternaam:             " + row["LastName"].ToString();

                    TextBlock tblAge = new TextBlock();
                    tblAge.Text += "Leeftijd:                    " + row["Age"].ToString();

                    TextBlock tblDescription = new TextBlock();
                    tblDescription.Text += "Omschrijving:           " + row["Description"].ToString();

                    try
                    {
                        byte[] data = (byte[])row["ClownFoto"];
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            var imageSource = new BitmapImage();
                            imageSource.BeginInit();
                            imageSource.StreamSource = ms;
                            imageSource.CacheOption = BitmapCacheOption.OnLoad;
                            imageSource.EndInit();

                            imPhoto.Source = imageSource;
                        }
                    }

                    catch { }
                    lvFoto.Children.Add(imPhoto);

                    sp.Children.Add(tblName);
                    sp.Children.Add(tblLastName);
                    sp.Children.Add(tblAge);
                    sp.Children.Add(tblDescription);


                    Border b = new Border();
                    b.BorderBrush = Brushes.Black;
                    b.BorderThickness = new Thickness(0.5);
                    b.Child = sp;

                    lvClowns.Children.Add(b);

                }

                conn.Close();
            }
            catch (Exception) { }
        }

        private void SelecteerFoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Selecteer een partijfoto";
                dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                "Portable Network Graphic (*.png)|*.png";

                if (dialog.ShowDialog() == true)
                {
                    ClownPhoto.Source = new BitmapImage(new Uri(dialog.FileName));
                    ClownPhotoName.Text = dialog.FileName;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Er is een error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }
    }
}
