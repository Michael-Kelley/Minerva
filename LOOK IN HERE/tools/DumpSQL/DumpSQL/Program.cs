using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace DumpSQL
{
    class Program
    {
        static DataTable schema;
        static SqlCommand cmd;
        static SqlConnection sql;
        static SqlDataReader reader;
        static StreamWriter file;
        static string read;
        static List<string> types = new List<string>();

        static void Main(string[] args)
        {
            Console.Write("Table name: ");
            read = Console.ReadLine();

            sql = new SqlConnection("Server=localhost;Database=login;User=your_username;Password=your_password");
            sql.Open();

            while (read != "quit")
            {
                try
                {
                    file = File.CreateText(read + ".sql");

                    string query = String.Format("select * from {0}", read);
                    cmd = new SqlCommand(query, sql);
                    reader = cmd.ExecuteReader();

                    string columns = ColumnNames;

                    while (reader.Read())
                    {
                        string insert = String.Format("insert into [dbo].[{0}] ({1}) values ({2})", read, columns, Rows);
                        file.WriteLine(insert);
                    }

                    file.Close();
                    reader.Close();

                    Console.Write("Table name: ");
                    read = Console.ReadLine();
                }
                catch (Exception e)
                {
#if DEBUG
                    throw e;
#endif
                    Console.WriteLine(e.Message);
                    Console.Read();
                    file.Close();
                    break;
                }
            }

            sql.Close();
        }

        static string ColumnNames
        {
            get
            {
                string value = "";
                schema = reader.GetSchemaTable();

                types.Clear();

                foreach (DataRow r in schema.Rows)
                    if (r["ColumnName"].ToString() != "id")
                    {
                        value += r["ColumnName"].ToString() + ", ";
                        types.Add(r["DataType"].ToString());
                    }

                value = value.Substring(0, value.Length - 2);

                return value;
            }
        }

        static string Rows
        {
            get
            {
                string value = "";
                string container = "";

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (types[i] == "System.String" ||
                        types[i] == "System.DateTime" ||
                        types[i] == "System.Boolean")
                        container = "'";
                    else
                        container = "";

                    object val = reader[i] as object;

                    if (val.GetType().ToString() == "System.DBNull")
                        value += "NULL, ";
                    else
                    if (types[i] == "System.Byte[]")
                        value += String.Format("0x{0}, ", BitConverter.ToString((byte[])val).Replace("-",""));
                    else
                        value += String.Format("{0}{1}{0}, ", container, val);
                }

                value = value.Substring(0, value.Length - 2);

                return value;
            }
        }
    }
}