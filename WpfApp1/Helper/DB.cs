using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Windows;

namespace WpfApp1.Helper
{
    static class DB
    {
        public static QueryReturn ExecuteSqlQuery(string query)
        {
            string connectionString = "Server="+ Properties.Settings.Default.Host + ";Database="+ Properties.Settings.Default.DatabaseName + ";User Id="+ Properties.Settings.Default.Username+ ";Password="+ Properties.Settings.Default.Password+ ";TrustServerCertificate=True;";

            try
            {
                Debug.WriteLine("Executing query: " + query);

                using (SqlConnection connection = new(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    return new QueryReturn(true, reader[0]?.ToString() ?? string.Empty);
                                }
                            }
                            else
                            {
                                return new QueryReturn(false, string.Empty);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wykonywania zapytania SQL: {ex.Message}");
                return new QueryReturn(false, string.Empty);
            }

            return new QueryReturn(false, string.Empty);
        }

        public class QueryReturn
        {
            public bool Success { get; set; }
            public string Value { get; set; }

            public QueryReturn(bool success, string value)
            {
                Success = success;
                Value = value;
            }
        }
    }
}
