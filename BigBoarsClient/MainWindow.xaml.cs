using BigBoarsClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace Refferal_of_patients
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool logincheck = false;

        public MainWindow()
        {
            InitializeComponent();
            LoginForm.Visibility = Visibility.Collapsed;
            LoadPatientsDb();
        }

        public void LoadPatientsDb()
        {
            using (var entity = new BigBoarsEntities())
            {
                PatientGrid.ItemsSource = entity.Patient.ToList();
            }

            CustomizePatientGridColumns();
        }

        private void CustomizePatientGridColumns()
        {
            PatientGrid.Loaded += (sender, e) =>
            {
                if (PatientGrid.Columns.Count > 1)
                {
                    PatientGrid.Columns[0].Header = "Номер";
                    PatientGrid.Columns[1].Visibility = Visibility.Collapsed;
                    PatientGrid.Columns[2].Header = "Имя";
                    PatientGrid.Columns[3].Header = "Фамилия";
                    PatientGrid.Columns[4].Header = "Отчество";

                    for (int i = 5; i < PatientGrid.Columns.Count; i++)
                    {
                        PatientGrid.Columns[i].Visibility = Visibility.Collapsed;
                    }
                }
            };
        }

        private void ProfileBg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (logincheck != true)
            {
                LoginForm.Visibility = Visibility.Visible;
                ClearAllTextboxes();
            }
            else { }
        }

        private void LoginForm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoginForm.Visibility = Visibility.Collapsed;
        }

        private void ProfileSumbit_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBx.Password;

            using (var entity = new BigBoarsEntities())
            {
                var autorization = entity.Acounts.FirstOrDefault(u => u.Логин == login && u.Пароль == password);
                if (autorization != null)
                {
                    LoginForm.Visibility = Visibility.Collapsed;
                    ProfileImg.Source = new BitmapImage(new Uri("/Resources/Sponge.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
                    PatientItem.Visibility = Visibility.Visible;
                    logincheck = true;
                }
                else { MessageBox.Show("Ошибка", "Неправильный логин и/или пароль"); }
            }
        }

        public void ClearAllTextboxes()
        {
            LoginBox.Clear();
            PasswordBx.Clear();
        }

        private void PatientGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientGrid.SelectedCells.Count == 0) return;

            var selectedPatient = PatientGrid.SelectedCells[0].Item as Patient;
            if (selectedPatient == null) return;

            LoadTherapyMeasureData(selectedPatient);
            DisplayPatientDetails(selectedPatient);
        }

        private void LoadTherapyMeasureData(Patient selectedPatient)
        {
            using (var entity = new BigBoarsEntities())
            {
                var therapyMeasure = entity.TherapMeasures.FirstOrDefault(p => p.PatientId == selectedPatient.PatientId);

                if (therapyMeasure != null)
                {
                    DateMeasuresBx.Text = therapyMeasure.EventDate.ToShortDateString();
                    DoctorBx.Text = therapyMeasure.Doctor;
                    TypeMeasuresCb.Text = therapyMeasure.Event;
                    NameMeasures.Text = therapyMeasure.EventName;
                    ResultMeasures.Text = therapyMeasure.Results;
                    RecomendMeasuresBx.Text = therapyMeasure.Recomendations;
                }
            }
        }

        private void DisplayPatientDetails(Patient selectedPatient)
        {
            string imagePath = selectedPatient.Photo;
            if (!string.IsNullOrEmpty(imagePath))
            {
                ProfilePic.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            }

            PatientIdBx.Text = selectedPatient.PatientId.ToString();
            FioBx.Text = $"{selectedPatient.Surname} {selectedPatient.Name} {selectedPatient.MiddleName}";
            PassBx.Text = selectedPatient.PassportNumSerial;
            DateBx.Text = selectedPatient.Birthday.ToShortDateString();

            GenderBx.Text = selectedPatient.Gender == 0 ? "М" : "Ж";

            AddressBx.Text = selectedPatient.Address;
            PhoneBx.Text = selectedPatient.PhoneNumber;
            MailBx.Text = selectedPatient.EmailAddress;
            CardBx.Text = selectedPatient.MedCardNum;
            DateCardBx.Text = selectedPatient.MedCardDate.ToShortDateString();
            LastVisitBx.Text = selectedPatient.LastVizitDate.ToString();
            NextVisitBx.Text = selectedPatient.NextVizitDate.ToString();
            PolisBx.Text = selectedPatient.PolisNum;
            PolisDateBx.Text = selectedPatient.PolisDate.ToShortDateString();
            DiagnoseBx.Text = selectedPatient.Diagnose;
            MedHisBx.Text = selectedPatient.MedicalHistory;
        }

        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            SaveBr.Visibility = Visibility.Visible;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            int PatientId = Convert.ToInt32(PatientIdBx.Text);
            using (var entity = new BigBoarsEntities())
            {
                var patients = entity.Patient.FirstOrDefault(p => p.PatientId == PatientId);
                if(patients != null)
                {
                    patients.Diagnose = DiagnoseBx.Text;
                    patients.MedicalHistory = MedHisBx.Text;

                    entity.SaveChanges();
                }
                else { MessageBox.Show("Текущий клиент не найден"); }
            }
            SaveBr.Visibility = Visibility.Hidden;
        }

        private void SaveBtn2_Click(object sender, RoutedEventArgs e)
        {
            int patientId = Convert.ToInt32(PatientIdBx.Text);
            DateTime eventDate = Convert.ToDateTime(DateMeasuresBx.Text);
            string doctor = DoctorBx.Text;
            string eventType = TypeMeasuresCb.Text;
            string eventName = NameMeasures.Text;
            string result = ResultMeasures.Text;
            string recommendation = RecomendMeasuresBx.Text;

            using (var entity = new BigBoarsEntities())
            {
                var therapyMeasure = entity.TherapMeasures.FirstOrDefault(tm => tm.PatientId == patientId);

                if (therapyMeasure != null)
                {
                    therapyMeasure.EventDate = eventDate;
                    therapyMeasure.Doctor = doctor;
                    therapyMeasure.Event = eventType;
                    therapyMeasure.EventName = eventName;
                    therapyMeasure.Results = result;
                    therapyMeasure.Recomendations = recommendation;
                }
                else
                {
                    var newTherapyMeasure = new TherapMeasures
                    {
                        PatientId = patientId,
                        EventDate = eventDate,
                        Doctor = doctor,
                        Event = eventType,
                        EventName = eventName,
                        Results = result,
                        Recomendations = recommendation
                    };
                    entity.TherapMeasures.Add(newTherapyMeasure);
                }

                entity.SaveChanges();
            }
        }

        private void DoctorBx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                if (!char.IsLetter(e.Text, 0))
                {
                    e.Handled = true; // Отменяет ввод небуквенных символов
                }
            }
        }
    }
}
