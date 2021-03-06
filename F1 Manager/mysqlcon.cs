﻿using System;
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

        public int checkRegistration(User TargetUser, decimal Balance, int IsAdmin)
        {

            int result = 0;
            using (MySqlCommand cmd = new MySqlCommand("RegistrateUser", connection))
            {
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Uname", TargetUser.Username);
                    cmd.Parameters.AddWithValue("Pword", TargetUser.Password);
                    cmd.Parameters.AddWithValue("Blnce", Balance);
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
                        LoggedInUser.UserID = (int)Reader["id"];
                        LoggedInUser.Username = (string)Reader["Username"];
                        LoggedInUser.Balance = (decimal)Reader["Balance"];
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
            using (MySqlCommand cmd = new MySqlCommand("ChangePassword", connection))
            {
                
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usrnm", Username);
                    cmd.Parameters.AddWithValue("@Psswrd", HashedPassword);

                    OpenConnectionIfClosed();

                    cmd.ExecuteNonQuery();
                    result = 1;
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

            //int result;
            List<string> AllUsers = new List<string>();

            //string constring = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Vincenzo\Documents\Fontys\Trashure\KillerApp\App_Data\Trashure.mdf; Integrated Security = True; Connect Timeout = 30"; ;
            using (MySqlCommand cmd = new MySqlCommand("GetAllUsers", connection))
            {
                //Nog Stored precedure aanmaken voor het ophalen van alle users
                try
                {
                    OpenConnectionIfClosed();

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
                    //result = 0;
                }

                finally
                {
                    connection.Close();
                }

            }
            return AllUsers;
        }

        public List<Driver> getMyTeam(int UserID)
        {
            List<Driver> driverList = new List<Driver>();

            using (MySqlCommand cmd = new MySqlCommand("GetMyTeam", connection))
            {
                try
                {
                    OpenConnectionIfClosed();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("userid", UserID);


                    MySqlDataReader Reader = cmd.ExecuteReader();

                    while (Reader.Read())
                    {
                        driverList.Add(new Driver { DriverID = (int)Reader["DriverID"],
                        Name = (string)Reader["FirstName"],
                        Team = (string)Reader["Team"],
                        Cost = (decimal)Reader["Cost"],
                        Points = (int)Reader["Points"] });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    CloseConnection();
                }
            }
            return driverList;
        }

        public List<Driver> getRaceResult(int raceID)
        {
            List<Driver> driverList = new List<Driver>();

            using (MySqlCommand cmd = new MySqlCommand("GetRaceResult", connection))
            {
                try
                {
                    OpenConnectionIfClosed();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("raceID", raceID);


                    MySqlDataReader Reader = cmd.ExecuteReader();

                    while (Reader.Read())
                    {
                        driverList.Add(new Driver
                        {
                            Position = (int)Reader["Position"],
                            DriverID = (int)Reader["DriverID"],
                            Name = (string)Reader["FirstName"],
                            Team = (string)Reader["Team"],
                            Points = (int)Reader["Points"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    CloseConnection();
                }
            }
            return driverList;
        }

        public List<User> getUsersFromDriver(int driverID)
        {
            List<User> userList = new List<User>();

            string command = $"SELECT UserID FROM userdriver WHERE DriverID = '{driverID}'";
            using (MySqlCommand cmd = new MySqlCommand(command, connection))
            {
                try
                {
                    OpenConnectionIfClosed();
                    


                    MySqlDataReader Reader = cmd.ExecuteReader();

                    while (Reader.Read())
                    {
                        userList.Add(new User
                        {
                            UserID = (int)Reader["UserID"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    CloseConnection();
                }
            }
            return userList;
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


