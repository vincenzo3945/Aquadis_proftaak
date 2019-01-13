using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using F1_Manager.Models;

namespace F1_Manager
{
    public class mysqlcon
    {

        //public string Hostname = "ams-web01.vanloon.xyz";
        public string Hostname = "aquadisdb.mysql.database.azure.com";
            public string DBName = "aquadis_proftaak";
            public string ID = "AquadisAdmin@aquadisdb";
            public string Password = "Welkom01";

        /*public string Hostname = "aquadis-db.mysql.database.azure.com";
            public string DBName = "aquadis_proftaak";
            public string ID = "aquadis@aquadis-db";
            public string Password = "Welkom01";*/

        private MySqlConnection connection;

        public mysqlcon()
        {
            connection = new MySqlConnection($"SERVER={Hostname};DATABASE={DBName};UID={ID};PASSWORD={Password};SslMode=none");
        }

        private void OpenConnectionIfClosed()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public int checkRegistration(User TargetUser, double Balance, int IsAdmin)
        {

            int result = 0;
            using (MySqlCommand cmd = new MySqlCommand("RegistrateUser", connection))
            {
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Uname", TargetUser.Username);
                    cmd.Parameters.AddWithValue("Pword", TargetUser.Password);
                    cmd.Parameters.AddWithValue("Balance", Balance);
                    cmd.Parameters.AddWithValue("Admin", IsAdmin);

                    OpenConnectionIfClosed();
                    cmd.ExecuteNonQuery();
                    result = 1;

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        public UserLogin CheckLogin(string Username, string Password, ref int Result)
        {
            UserLogin LoggedInUser = new UserLogin();
            using (MySqlCommand cmd = new MySqlCommand("LoginCheck", connection))
            {
                
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Uname", Username);
                    cmd.Parameters.AddWithValue("Pword", Password);

                    OpenConnectionIfClosed();
                    MySqlDataReader Reader = cmd.ExecuteReader();

                    //UserLogin LoggedInUser = new UserLogin();

                    while (Reader.Read())
                    {
                        LoggedInUser.Username = (string)Reader["Username"];
                        LoggedInUser.IsAdmin = (int)Reader["IsAdmin"];
                    }
                    Result = 1;

                    return LoggedInUser;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return LoggedInUser;
        }

        public int ResetPassword(string Username, string HashedPassword)
        {

            int result;


            //string constring = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Vincenzo\Documents\Fontys\Trashure\KillerApp\App_Data\Trashure.mdf; Integrated Security = True; Connect Timeout = 30"; ;
            using (MySqlCommand cmd = new MySqlCommand("ResetPassword", connection))
            {
                
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Uname", Username);
                    cmd.Parameters.AddWithValue("@Pword", HashedPassword);
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    result = 0;
                }

                finally
                {                   
                    connection.Close();                    
                }                

            }
            return result;
        }

        public List<string> GetAllUsers()
        {

            int result;
            List<string> AllUsers = new List<string>();

            //string constring = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Vincenzo\Documents\Fontys\Trashure\KillerApp\App_Data\Trashure.mdf; Integrated Security = True; Connect Timeout = 30"; ;
            using (MySqlCommand cmd = new MySqlCommand("GetAllUsers", connection))
            {
                //Nog Stored precedure aanmaken voor het ophalen van alle users
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    MySqlDataReader Reader = cmd.ExecuteReader();

                    while (Reader.Read())
                    {                       
                        AllUsers.Add((string)Reader["Username"]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    result = 0;
                }

                finally
                {
                    connection.Close();
                }

            }
            return AllUsers;
        }

        public void mysql(string command)
        {
            using (MySqlCommand cmd = new MySqlCommand(command, connection))
            {
                try
                {
                    OpenConnectionIfClosed();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public string getString(string command)
        {
            string result = null;
            using (MySqlCommand cmd = new MySqlCommand(command, connection))
            {
                try
                {
                    OpenConnectionIfClosed();
                    result = (string)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        public int getID(string command)
        {
            int result = 0;

            using (MySqlCommand cmd = new MySqlCommand(command, connection))
            {
                try
                {
                    OpenConnectionIfClosed();
                    result = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        public MySqlDataReader ReadSQL(string command)
        {
            MySqlCommand cmd = new MySqlCommand(command, connection);
            try
            {
                OpenConnectionIfClosed();
                MySqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public void testDbCon()
        {
            try
            {
                connection.Open();
                Console.WriteLine("MySQL server connected!");
                connection.Close();
            }
            catch
            {
                mysqlError();
            }
        }
        private void mysqlError()
        {
            Console.WriteLine("Can't connect to the MySQL Server!");
        }
        private void mysqlError(Exception ex)
        {
            Console.WriteLine("Something went wrong!" + "Environment.NewLine" + ex.ToString());
        }
    }
}


