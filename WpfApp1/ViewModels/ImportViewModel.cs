using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using InsERT;
using Microsoft.Win32;
using MimeKit;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Helper;

namespace WpfApp1.ViewModels
{


    class ImportViewModel : BaseViewModel
    {
        #region MVVM
        #region Type
        private ItemModel _selectedItemType;
        public ObservableCollection<ItemModel> ItemsType { get; set; }
        public ItemModel SelectedItemType
        {
            get => _selectedItemType;
            set
            {
                _selectedItemType = value;

                if (SelectedItemType.Brutto == true)
                {
                    IsToggleActive = true;
                }
                else
                {
                    IsToggleActive = false;
                    IsToggled = false;
                }
                OnPropertyChanged(); // Powiadomienie o zmianie
            }
        }
        private void OnSelectionTypeChanged(object parameter)
        {
            if (SelectedItemType != null)
            {
                System.Windows.MessageBox.Show($"Wybrano: {SelectedItemType.Name}");
            }
        }
        public ICommand SelectionTypeChangedCommand { get; }
        #endregion

        #region Identyfication
        private ItemModel _selectedItemIdentyfication;
        public ObservableCollection<ItemModel> ItemsIdentyfication { get; set; }
        public ItemModel SelectedItemIdentyfication
        {
            get => _selectedItemIdentyfication;
            set
            {
                _selectedItemIdentyfication = value;
                OnPropertyChanged(); // Powiadomienie o zmianie
                UpdateTable();
            }
        }
        private void OnSelectionIdentyficationChanged(object parameter)
        {
            if (SelectedItemType != null)
            {
                System.Windows.MessageBox.Show($"Wybrano: {SelectedItemType.Name}");
            }
        }
        public ICommand SelectionIdentyficationChangedCommand { get; }
        #endregion

        #region Switch
        private bool _isToggled;

        public bool IsToggled
        {
            get => _isToggled;
            set
            {
                _isToggled = value;
                OnPropertyChanged();
                UpdateTable();
            }
        }

        private bool _isToggleActive;
        public bool IsToggleActive
        {
            get => _isToggleActive;
            set
            {
                _isToggleActive = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleCommand { get; }

        private void ToggleAction(object parameter)
        {
            //System.Windows.MessageBox.Show(IsToggled ? "Włączone" : "Wyłączone");
        }
        #endregion

        #region isVisible
        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Button Get File
        public ICommand ButtonImportCommand { get; }

        private void ExecuteButtonImportCommand(object parameter)
        {
            // Tworzymy obiekt OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Opcjonalnie: Ustawienia dialogu (np. filtr plików)
            openFileDialog.Filter = "Arkusze kalkulacyjne|*.xlsx;*.xls;*.csv|Wszystkie pliki (*.*)|*.*"; // Możesz ustawić różne filtry plików

            // Pokaż dialog i sprawdź, czy użytkownik wybrał plik
            if (openFileDialog.ShowDialog() == true)
            {
                // Odczytaj wybrany plik
                string filePathTemp = openFileDialog.FileName;

                // Sprawdzanie MIME type pliku
                string mimeType = GetMimeType(filePathTemp);

                ExcelHeaders.ToList().ForEach(cell =>
                {
                    ExcelHeaders.Remove(cell);
                });

                if (mimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
                mimeType == "application/vnd.ms-excel")
                {
                    filePath = filePathTemp;
                    IsVisible = true;
                    // Odczyt nagłówków z pliku Excel
                    ReadExcelHeaders(filePath);
                }
                else if (mimeType == "text/csv")
                {
                    filePath = filePathTemp;
                    IsVisible = true;
                    // Odczyt nagłówków z pliku CSV
                    ReadCsvHeaders(filePath);
                }
                else
                {
                    MessageBox.Show("Wybrany plik nie jest typu Excel ani CSV.");
                }
            }

        }

        private string GetMimeType(string filePath)
        {
            // Odczytujemy MIME type pliku
            try
            {
                // Zamiast FileStream, przekazujemy bezpośrednio ścieżkę pliku
                var mimeType = MimeTypes.GetMimeType(filePath);
                return mimeType;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas odczytywania MIME typu: {ex.Message}");
                return string.Empty;
            }
        }

        private void ReadExcelHeaders(string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Odczytujemy pierwszy arkusz
                    var firstRow = worksheet.FirstRowUsed(); // Odczytujemy pierwszy używany wiersz (nagłówki)

                    var headers = firstRow.Cells().Select(cell => cell.Value.ToString()).ToList();
                    //MessageBox.Show("Nagłówki Excel: " + string.Join(", ", headers));
                    headers.ForEach(cell =>
                    {

                        ExcelHeaders.Add(cell);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas odczytu pliku Excel: {ex.Message}");
            }
        }

        private void ReadCsvHeaders(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Odczytaj nagłówki kolumn
                    csv.Read(); // Wczytaj pierwszy wiersz
                    csv.ReadHeader(); // Odczytaj nagłówki kolumn

                    var headers = csv.HeaderRecord; // Pobierz nagłówki

                    if (headers != null)
                    {
                        //MessageBox.Show("Nagłówki CSV: " + string.Join(", ", headers));
                        headers.ToList().ForEach(cell =>
                        {

                            ExcelHeaders.Add(cell);
                        });
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się odczytać nagłówków CSV.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas odczytu pliku CSV: {ex.Message}");
            }
        }

        #endregion

        #region Table

        public ObservableCollection<MappingRow> TableRows { get; set; }
        public ObservableCollection<string> ExcelHeaders { get; set; }

        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                _isAllSelected = value;
                OnPropertyChanged();
            }
        }
        private void CheckAllSelected()
        {
            IsAllSelected = TableRows.All(row => !string.IsNullOrEmpty(row.SelectedHeader));
        }

        #endregion

        #region Button Import
        public ICommand ButtonImportEndCommand { get; }
        private void ExecuteButtonImportEndCommand(object parameter)
        {
            IsAllSelected = false;

            InsERT.GT gt = new InsERT.GT();
            gt.Produkt = InsERT.ProduktEnum.gtaProduktSubiekt;
            gt.Serwer = Properties.Settings.Default.Host;
            gt.Baza = Properties.Settings.Default.DatabaseName;
            gt.Autentykacja = InsERT.AutentykacjaEnum.gtaAutentykacjaMieszana;
            gt.Uzytkownik = Properties.Settings.Default.Username;
            gt.UzytkownikHaslo = Properties.Settings.Default.Password;
            //gt.Operator = Konf.Subiekt.Operator;
            //gt.OperatorHaslo = Konf.Subiekt.OperatorHaslo;

            InsERT.Subiekt subiekt;
            try
            {
                // Uruchomienie Subiekta GT
                subiekt = (InsERT.Subiekt)gt.Uruchom((Int32)InsERT.UruchomDopasujEnum.gtaUruchomDopasuj, (Int32)InsERT.UruchomEnum.gtaUruchomNieArchiwizujPrzyZamykaniu);
                subiekt.Okno.Widoczne = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się połączyć z Subiekt");
                return;
            }

            SuDokument dokument;
            switch (SelectedItemType.Name)
            {
                case "FZ":
                    dokument = subiekt.SuDokumentyManager.DodajFZ();
                    break;
                case "FS":
                    dokument = subiekt.SuDokumentyManager.DodajFS();
                    break;
                case "MM":
                    dokument = subiekt.SuDokumentyManager.DodajMM();
                    break;
                case "WZ":
                    dokument = subiekt.SuDokumentyManager.DodajWZ();
                    break;
                case "PZ":
                    dokument = subiekt.SuDokumentyManager.DodajPZ();
                    break;
                default:
                    return;

            }
            dokument.LiczonyOdCenBrutto = IsToggled;




            List<Dictionary<string, string>> rows = FileReader.ReadFile(filePath);

            for (int i = 0; i < rows.Count; i++)
            {
                Dictionary<string, string> row = rows[i];

                SuPozycja pozycja;

                //Identyfikacja towaru
                if (SelectedItemIdentyfication.Name == "Symbol")
                {

                    var symbolRow = TableRows.FirstOrDefault(tempRow => tempRow.Label == "Symbol");
                    if (symbolRow == null)
                    {
                        MessageBox.Show("Nie znaleziono kolumny z symbolem  w wierszu " + (i + 2));
                        return;
                    }
                    var value = symbolRow.SelectedHeader;

                    string query = "SELECT tw_Id FROM tw__Towar WHERE tw_Symbol = '" + row[symbolRow.SelectedHeader] + "'";
                    DB.QueryReturn response = DB.ExecuteSqlQuery(query);
                    if (!response.Success)
                    {
                        MessageBox.Show("Towar o symbolu " + row[symbolRow.SelectedHeader] + " nie istnieje w bazie danych");
                        return;
                    }
                    else
                    {
                        int productId = 0;
                        if (!int.TryParse(response.Value, out productId))
                        {
                            MessageBox.Show("Nie udało się odczytać id towaru w wierszu " + (i + 2));
                            return;
                        }
                        pozycja = dokument.Pozycje.Dodaj(productId);
                    }



                }
                else
                {
                    var eanRow = TableRows.FirstOrDefault(tempRow => tempRow.Label == "EAN");
                    if (eanRow == null)
                    {
                        MessageBox.Show("Nie znaleziono kolumny z ean  w wierszu " + (i + 2));
                        return;
                    }
                    var value = eanRow.SelectedHeader;

                    string query = "SELECT tw_Id FROM tw__Towar WHERE tw_PodstKodKresk = '" + row[eanRow.SelectedHeader] + "'";
                    DB.QueryReturn response = DB.ExecuteSqlQuery(query);
                    if (response.Success)
                    {
                        int productId = 0;
                        if (!int.TryParse(response.Value, out productId))
                        {
                            MessageBox.Show("Nie udało się odczytać id towaru w wierszu " + (i + 2));
                            return;
                        }
                        pozycja = dokument.Pozycje.Dodaj(productId);

                    }
                    else
                    {
                        string query2 = "SELECT kk_IdTowar FROM tw_KodKreskowy WHERE kk_Kod = '" + row[eanRow.SelectedHeader] + "'";
                        DB.QueryReturn response2 = DB.ExecuteSqlQuery(query2);

                        if (response2.Success)
                        {
                            int productId = 0;
                            if (!int.TryParse(response2.Value, out productId))
                            {
                                MessageBox.Show("Nie udało się odczytać id towaru w wierszu " + (i + 2));
                                return;
                            }
                            pozycja = dokument.Pozycje.Dodaj(productId);

                        }
                        else
                        {
                            MessageBox.Show("Towar o ean " + row[eanRow.SelectedHeader] + " nie istnieje w bazie danych");
                            return;
                        }
                    }
                }

                //Cena
                MappingRow priceRow;
                if (IsToggled)
                {
                    priceRow = TableRows.FirstOrDefault(tempRow => tempRow.Label == "Cena Brutto");
                }
                else
                {
                    priceRow = TableRows.FirstOrDefault(tempRow => tempRow.Label == "Cena Netto");
                }

                if (priceRow == null)
                {
                    MessageBox.Show("Nie znaleziono kolumny z ceną  w wierszu " + (i + 2));
                    return;
                }



                // Sprawdzenie formatu stringa reprezentującego cenę
                string priceString = row[priceRow.SelectedHeader];

                // Zamiana przecinka na kropkę, jeśli jest używany jako separator dziesiętny
                if (priceString.Contains('.'))
                {
                    priceString = priceString.Replace('.', ',');
                }

                double cena = 0;

                if (priceRow != null)
                {
                    if (!double.TryParse(priceString, out cena))
                    {
                        MessageBox.Show("Nie udało się odczytać ceny w wierszu " + (i + 2));
                        return;
                    }
                }

                if (IsToggled)
                {
                    pozycja.CenaBruttoPrzedRabatem = cena;
                    pozycja.CenaBruttoPoRabacie = cena;
                }
                else
                {
                    pozycja.CenaNettoPrzedRabatem = cena;
                    pozycja.CenaNettoPoRabacie = cena;
                }





                //Ilość
                MappingRow quantityRow = TableRows.FirstOrDefault(tempRow => tempRow.Label == "Ilość");
                if (quantityRow == null)
                {
                    MessageBox.Show("Nie znaleziono kolumny z ilością  w wierszu " + (i + 2));
                    return;
                }
                int ilosc = 0;
                if (!int.TryParse(row[quantityRow.SelectedHeader], out ilosc))
                {
                    MessageBox.Show("Nie udało się odczytać ilości w wierszu " + (i + 2));
                    return;
                }

                pozycja.IloscJm = ilosc;

            }
            dokument.Wyswietl();

            IsAllSelected = true;
            IsVisible= false;
        }

        public class ExcelReader
        {
            public static List<Dictionary<string, string>> ReadExcel(string filePath)
            {
                var rows = new List<Dictionary<string, string>>();

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.First();
                    var headerRow = worksheet.Row(1).Cells().Select(c => c.Value.ToString()).ToList(); // Nagłówki
                    int columnCount = headerRow.Count;

                    foreach (var row in worksheet.RowsUsed().Skip(1)) // Pomijamy pierwszy wiersz (nagłówki)
                    {
                        var rowData = new Dictionary<string, string>();

                        for (int i = 0; i < columnCount; i++) // Pobieramy wszystkie kolumny po indeksie!
                        {
                            var cell = row.Cell(i + 1).Value; // Pobieramy komórkę po indeksie kolumny
                            rowData[headerRow[i]] = cell.IsBlank ? "" : cell.ToString(); // Obsługa pustych wartości
                        }

                        rows.Add(rowData);
                    }
                }
                return rows;
            }
        }

        public class CsvFileReader
        {
            public static List<Dictionary<string, string>> ReadCsv(string filePath)
            {
                var rows = new List<Dictionary<string, string>>();

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.Read();
                    csv.ReadHeader(); // Odczytujemy nagłówki
                    var headers = csv.HeaderRecord;

                    while (csv.Read())
                    {
                        var row = new Dictionary<string, string>();
                        foreach (var header in headers)
                        {
                            row[header] = csv.GetField(header);
                        }
                        rows.Add(row);
                    }
                }
                return rows;
            }
        }

        public class FileReader
        {
            public static List<Dictionary<string, string>> ReadFile(string filePath)
            {
                string extension = Path.GetExtension(filePath).ToLower();

                return extension switch
                {
                    ".csv" => CsvFileReader.ReadCsv(filePath),
                    ".xlsx" or ".xls" => ExcelReader.ReadExcel(filePath),
                    _ => throw new NotSupportedException("Obsługiwane formaty to tylko CSV, XLSX, XLS")
                };
            }
        }

        #endregion
        #endregion




        private string filePath;

        public ImportViewModel()
        {
            ButtonImportCommand = new RelayCommand(ExecuteButtonImportCommand);
            ButtonImportEndCommand = new RelayCommand(ExecuteButtonImportEndCommand);


            // Inicjalizacja listy elementów
            ItemsType = new ObservableCollection<ItemModel>
            {
                new ItemModel { Name = "FZ", Brutto=true },
                new ItemModel { Name = "FS", Brutto=true },
                new ItemModel { Name = "MM", Brutto=false },
                new ItemModel { Name = "WZ", Brutto=false },
                new ItemModel { Name = "PZ", Brutto=false },
            };
            SelectedItemType = ItemsType[0];

            // Inicjalizacja listy elementów
            ItemsIdentyfication = new ObservableCollection<ItemModel>
            {
                new ItemModel { Name = "Symbol" },
                new ItemModel { Name = "EAN" },
            };
            SelectedItemIdentyfication = ItemsIdentyfication[0];

            IsVisible = false;
            IsAllSelected = false;

            ExcelHeaders = new ObservableCollection<string> { };

            TableRows = new ObservableCollection<MappingRow>
            {
                new MappingRow { Label = "Symbol lub EAN", AvailableHeaders = ExcelHeaders, OnSelectionChanged= CheckAllSelected },
                new MappingRow { Label = "Cena Netto lub Brutto", AvailableHeaders = ExcelHeaders,OnSelectionChanged= CheckAllSelected },
                new MappingRow { Label = "Ilość", AvailableHeaders = ExcelHeaders, OnSelectionChanged= CheckAllSelected }
            };

            UpdateTable();


            ToggleCommand = new RelayCommand(ToggleAction);
            SelectionTypeChangedCommand = new RelayCommand(OnSelectionTypeChanged);
            SelectionIdentyficationChangedCommand = new RelayCommand(OnSelectionIdentyficationChanged);
        }


        private void UpdateTable()
        {
            if (TableRows == null) return;
            TableRows[0].Label = SelectedItemIdentyfication.Name == "EAN" ? "EAN" : "Symbol";
            TableRows[1].Label = IsToggled ? "Cena Brutto" : "Cena Netto";
        }






        #region Models
        public class ItemModel
        {
            public required string Name { get; set; }
            public bool Brutto { get; set; }
        }

        public class MappingRow : INotifyPropertyChanged
        {
            private string _label;
            public string Label
            {
                get => _label
             ; set
                {
                    _label = value;
                    OnPropertyChanged();
                }
            }
            public ObservableCollection<string> AvailableHeaders { get; set; } // Opcje dla ComboBoxa

            private string _selectedHeader;
            public string SelectedHeader
            {
                get => _selectedHeader;
                set
                {
                    _selectedHeader = value;
                    OnPropertyChanged();
                    OnSelectionChanged?.Invoke();
                }
            }

            public Action OnSelectionChanged { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
