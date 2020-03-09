using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clown.Classes
{
    public class DataBase
    {
        SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\mssqllocaldb; 
                                                Initial Catalog=Clowns;Integrated Security=True");

        public void AddClown(string FirstName, string LastName, int Age, string Description, byte[] encode)
        {
            conn.Open();

            SqlCommand command = conn.CreateCommand();
            command.CommandText = "INSERT INTO TheClownTable (Name, LastName, Age, Description, ClownFoto) VALUES (@firstname, @lastname, @age, @description, @foto); ";

            SqlParameter blob = new SqlParameter("@foto", SqlDbType.Binary, encode.Length);

            blob.Value = encode;
            command.Parameters.Add(blob);

            command.Parameters.AddWithValue("@firstname", FirstName);
            command.Parameters.AddWithValue("@lastname", LastName);
            command.Parameters.AddWithValue("@age", Age);
            command.Parameters.AddWithValue("@description", Description);

            command.ExecuteNonQuery();

            conn.Close();
        }


    }
}
