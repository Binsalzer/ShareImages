using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareImageHw.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Password { get; set; }
        public int ViewCount { get; set; }
    }


    public class ImageRepo
    {
        private string _connection;
        public ImageRepo(string connection)
        {
            _connection = connection;
        }

        public int AddImage(Image image)
        {
            SqlConnection con = new(_connection);
            var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT INTO Images
                                VALUES (@path, @password, 0) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@path", image.Path);
            cmd.Parameters.AddWithValue("@password", image.Password);
            con.Open();
            return (int)(decimal) cmd.ExecuteScalar();
        }

        public Image GetImageById(int id)
        {
            SqlConnection con = new(_connection);
            var cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Images
                                WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            var reader = cmd.ExecuteReader();
            if(!reader.Read())
            {
                return null;
            }
            return new Image
            {
                Id = (int)reader["Id"],
                Path = (string)reader["Path"],
                Password = (string)reader["Password"],
                ViewCount = (int)reader["ViewCount"]
            };
        }
    }
}
