using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using messageBroker.Models;
using System.Linq;

namespace messageBroker.Connectors
{
    public class MysqlConnector
    {
        
        private MySqlConnection connection;
        private const string server = "localhost";
        private const string database = "MessageBroker";
        private const string uid = "mbworker";
        private const string password = "qwe123";
        
        private const string MessagesTable = "Messages";
        public MysqlConnector(){
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + 
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            this.connection = new MySqlConnection(connectionString);
        }
        public bool CloseConnection()
        {
            try{
            connection.Close();
            return true;
            }
            catch(Exception){
                return false;
            }
        }

        public bool OpenConnection(){
            try{
            connection.Open();
            var query = "use MessageBroker;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            return true;
            }
            catch(Exception){
                return false;
            }
        }
        public List<string> GetUserCategories(string username){
            var output = new List<string>();
            
            string query = $"SELECT * FROM Messages WHERE author = '{username}';";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                //output.Append(
                var a = dataReader["category"].ToString().
                    Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);//);
                output.Add(a[0]);
            }
            var newOutput = output.Distinct().ToList();
            return newOutput;
        }

        public void InsertCategoryToUser(string username, string category)
        {
            connection.Open();
            var oldCategories = GetUserCategories(username);
            oldCategories.Add(category);
            var newCategories = string.Join(';', oldCategories);

            string query = $"UPDATE Users SET categories = {newCategories} WHERE name = {username};";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }


        public void InsertMessage(string username, string category, string message)
        {
            connection.Open();

            string query = $@"INSERT INTO Messages (category, message, timestamp, author) VALUES('{category}','{message}','{DateTime.Now.ToString()}','{username}');";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();

        }

        public List<MessageModel> GetMessagesForCategory(string category)
        {
            connection.Open();
            var messages = new List<MessageModel>();

            string query = $"SELECT * FROM Messages WHERE category = '{category}';";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                var temp = new MessageModel();
                temp.ID = dataReader["id"].ToString();
                temp.Category = dataReader["category"].ToString();
                temp.Author = dataReader["author"].ToString();
                temp.Timestamp = dataReader["timestamp"].ToString();
                temp.Message = dataReader["message"].ToString();
                messages.Add(temp);
            }

            return messages;
        }

    }
}