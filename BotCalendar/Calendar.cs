using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BotCalendar
{
    public class Shedule
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Decription { get; set; }
    }

    public class Calendar
    {
        public Calendar() { }

        public List<Shedule> shedulelist = new List<Shedule>();

        public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\fight\Documents\CalendarDataBase.mdf;Integrated Security=True;Connect Timeout=30";

        public static string selectcommand = @"SELECT * FROM CalendarDB";

        public SqlConnection connection = new SqlConnection(connectionString);

        public SqlCommand command;

        public Shedule[] GetEvent()
        {
            try
            {
                connection.Open();

                command = new SqlCommand(selectcommand, connection);

                SqlDataReader datareader = command.ExecuteReader();

                if (datareader.HasRows)
                {
                    while (datareader.Read())
                    {
                        Shedule shedule = new Shedule
                        {
                            ID = int.Parse(datareader["ID"].ToString()),
                            Date = DateTime.Parse(datareader["Date"].ToString()),
                            Decription = (string)datareader["Eventdescription"]
                        };
                        shedulelist.Add(shedule);
                    }
                }
                datareader.Close();
            }

            catch(Exception e) { Console.WriteLine(e.Message.ToString()); }

            finally
            {
                connection.Close();
            }
            return shedulelist.ToArray();
        }

        public void AddShedule(DateTime insertdate,string description)
        {
            try
            {
                connection.Open();

                SqlCommand insertcommand = new SqlCommand(@"INSERT INTO CalendarDB (Date,EventDescription) VALUES (@Date,@Eventdescription)",connection);

                insertcommand.Parameters.AddWithValue("@Date",insertdate.ToString("yyyy-MM-dd"));
                insertcommand.Parameters.AddWithValue("@EventDescription", description);

                insertcommand.ExecuteNonQuery();
            }

            catch (Exception e) { Console.WriteLine(e.Message.ToString()); }

            finally { connection.Close(); }
        }

        public void DeleteEvent(DateTime deletedate)
        {
            try
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = "DELETE FROM CalendarDB WHERE Date = @Date";
                    cmd.Parameters.AddWithValue("@Date", deletedate);
                    cmd.ExecuteNonQuery();
                }
            }

            catch (Exception e) { Console.WriteLine(e.Message); }

            finally { connection.Close(); }
        }
        
        static void Main() { }
    }
}
