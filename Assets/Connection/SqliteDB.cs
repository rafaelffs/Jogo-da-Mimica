using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//References
using Mono.Data.SqliteClient;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;
using Assets.Models;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEditor;
using System.Runtime.InteropServices;

namespace Assets.Connection
{
    public class SqliteDB : MonoBehaviour
    {
        private string conn, sqlQuery;
        IDbConnection dbconn;
        IDbCommand dbcmd;
        private IDataReader reader;

        string DatabaseName = "database.s3db";
        public SqliteDB()
        {
            //Application database Path android
            string filepath = Application.persistentDataPath + "/" + DatabaseName;
            if (!File.Exists(filepath))
            {
                // If not found on android will create Tables and database

                Debug.LogWarning("File \"" + filepath + "\" does not exist. Attempting to create from \"" +
                                 Application.dataPath + "!/assets/Employers");



                // UNITY_ANDROID
                WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Employers.s3db");
                while (!loadDB.isDone) { }
                // then save to Application.persistentDataPath
                File.WriteAllBytes(filepath, loadDB.bytes);




            }

            conn = "URI=file:" + filepath;

            Debug.Log("Stablishing connection to: " + conn);
            dbconn = new SqliteConnection(conn);
            dbconn.Open(); ;

            string queryCategory = "CREATE TABLE IF NOT EXISTS Category (ID INTEGER PRIMARY KEY   AUTOINCREMENT, Name varchar(255), Download_Url varchar(200))";
            string queryItem = "CREATE TABLE IF NOT EXISTS Item (ID INTEGER PRIMARY KEY   AUTOINCREMENT, Name varchar(255), CategoryId INTEGER)";
            try
            {
                dbcmd = dbconn.CreateCommand(); // create empty command
                dbcmd.CommandText = queryCategory; // fill the command
                reader = dbcmd.ExecuteReader(); // execute command which returns a reader
                dbcmd.CommandText = queryItem; // fill the command
                reader = dbcmd.ExecuteReader(); // execute command which returns a reader
            }
            catch (Exception e)
            {

                Debug.Log(e);

            }
            //  reader_function();
        }

        //Category
        public void InsertCategory(List<Category> categories)
        {
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                int i = 0;
                sqlQuery = "";
                dbcmd = dbconn.CreateCommand();
                foreach (Assets.Models.Category category in categories)
                {
                    i++;
                    sqlQuery += string.Format("insert into Category (Id, Name, Download_Url) values (\"{0}\",\"{1}\",\"{2}\")", i, category.FormattedName, category.Download_Url);// table name
                    sqlQuery += ";";
                    category.Id = i;
                }
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();

                dbconn.Close();
            }
            //data_staff.text = "";
            Debug.Log("Insert Done ");

            //reader_function();
        }
        public List<Category> GetCategories()
        {
            List<Category> listCategory = new List<Category>();
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Id, Name, Download_Url " + "FROM Category";// table name
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();
                while (reader.Read())
                {
                    Category category = new Category()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Download_Url = reader.GetString(2)
                    };
                    listCategory.Add(category);
                }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
            }
            return listCategory;
        }
        public Category GetCategoryById(int id)
        {
            Category category = new Category();
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Id, Name, Download_Url " + "FROM Category WHERE Id = " + id;// table name
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();
                while (reader.Read())
                {
                    category = new Category()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Download_Url = reader.GetString(2)
                    };
                }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
            }
            return category;
        }

        //Item
        public void InsertItem(List<Item> items)
        {
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                sqlQuery = "";
                dbcmd = dbconn.CreateCommand();
                sqlQuery += "BEGIN TRANSACTION; ";
                foreach (Item item in items)
                {
                    sqlQuery += string.Format(" insert into Item (Name, CategoryId) values (\"{0}\",\"{1}\")", item.Name, item.Category.Id);// table name
                    sqlQuery += ";";
                }
                sqlQuery += " COMMIT;";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();
                dbconn.Close();
            }
            Debug.Log("Insert Done ");
        }

        public List<Item> GetItemsByCategories(List<Category> categories)
        {
            List<Item> listItems = new List<Item>();
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT Id, Name, CategoryId " + "FROM Item WHERE CategoryId IN ( ";
                for (int i = 0; i < categories.Count; i++)
                {
                    sqlQuery += categories[i].Id;
                    if (i != (categories.Count - 1))
                        sqlQuery += ", ";
                }
                sqlQuery += ")";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();
                while (reader.Read())
                {
                    Item item = new Item()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Category = categories.FirstOrDefault(x => x.Id == reader.GetInt32(2))
                    };
                    listItems.Add(item);
                }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
            }
            return listItems;
        }

        public Item GetRandomItem(List<Item> items, List<Item> selectedItems)
        {
            List<Item> listItems = items.Where(x => !selectedItems.Any(y => y.Id == x.Id)).ToList();
            System.Random rnd = new System.Random();
            int r = rnd.Next(listItems.Count);
            return listItems[r];
        }

        public async System.Threading.Tasks.Task Sync()
        {
            DeleteCategories();
            DeleteItems();
            List<Category> categories = await Connection.DownloadCategories();
            InsertCategory(categories);

            List<Item> listItems = new List<Item>();
            foreach (var category in categories)
            {
                listItems = await Connection.DownloadItemsByCategory(category);
                InsertItem(listItems);
            }
        }

        //Read All Data For To Database
        //private void reader_function()
        //{
        //    // int idreaders ;
        //    string Namereaders, DownloadUrlreaders;
        //    using (dbconn = new SqliteConnection(conn))
        //    {
        //        dbconn.Open(); //Open connection to the database.
        //        IDbCommand dbcmd = dbconn.CreateCommand();
        //        string sqlQuery = "SELECT  Name, Download_Url " + "FROM Category";// table name
        //        dbcmd.CommandText = sqlQuery;
        //        IDataReader reader = dbcmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            // idreaders = reader.GetString(1);
        //            Namereaders = reader.GetString(0);
        //            DownloadUrlreaders = reader.GetString(1);

        //            //data_staff.text += Namereaders + " - " + DownloadUrlreaders + "\n";
        //            Debug.Log(" name =" + Namereaders + "Download_Url=" + DownloadUrlreaders);
        //        }
        //        reader.Close();
        //        reader = null;
        //        dbcmd.Dispose();
        //        dbcmd = null;
        //        dbconn.Close();
        //        //       dbconn = null;

        //    }
        //}
        ////Search on Database by ID
        //private void Search_function(string Search_by_id)
        //{
        //    using (dbconn = new SqliteConnection(conn))
        //    {
        //        string Name_readers_Search, Download_Url_readers_Search;
        //        dbconn.Open(); //Open connection to the database.
        //        IDbCommand dbcmd = dbconn.CreateCommand();
        //        string sqlQuery = "SELECT Name,Download_Url " + "FROM Category where id =" + Search_by_id;// table name
        //        dbcmd.CommandText = sqlQuery;
        //        IDataReader reader = dbcmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            //  string id = reader.GetString(0);
        //            Name_readers_Search = reader.GetString(0);
        //            Download_Url_readers_Search = reader.GetString(1);
        //            //data_staff.text += Name_readers_Search + " - " + Download_Url_readers_Search + "\n";

        //            Debug.Log(" Name =" + Name_readers_Search + "Download_Url=" + Download_Url_readers_Search);

        //        }
        //        reader.Close();
        //        reader = null;
        //        dbcmd.Dispose();
        //        dbcmd = null;
        //        dbconn.Close();


        //    }

        //}

        ////Search on Database by ID
        //private void F_to_update_function(string Search_by_id)
        //{
        //    using (dbconn = new SqliteConnection(conn))
        //    {
        //        string Name_readers_Search, Download_Url_readers_Search;
        //        dbconn.Open(); //Open connection to the database.
        //        IDbCommand dbcmd = dbconn.CreateCommand();
        //        string sqlQuery = "SELECT Name,Download_Url " + "FROM Category where id =" + Search_by_id;// table name
        //        dbcmd.CommandText = sqlQuery;
        //        IDataReader reader = dbcmd.ExecuteReader();
        //        while (reader.Read())
        //        {

        //            Name_readers_Search = reader.GetString(0);
        //            Download_Url_readers_Search = reader.GetString(1);
        //            t_name = Name_readers_Search;
        //            t_download_url = Download_Url_readers_Search;

        //        }
        //        reader.Close();
        //        reader = null;
        //        dbcmd.Dispose();
        //        dbcmd = null;
        //        dbconn.Close();


        //    }

        //}
        ////Update on  Database 
        //private void update_function(string update_id, string update_name, string update_Download_Url)
        //{
        //    using (dbconn = new SqliteConnection(conn))
        //    {
        //        dbconn.Open(); //Open connection to the database.
        //        dbcmd = dbconn.CreateCommand();
        //        sqlQuery = string.Format("UPDATE Category set Name = @Name ,Download_Url = @Download_Url where ID = @id ");

        //        SqliteParameter P_update_name = new SqliteParameter("@Name", update_name);
        //        SqliteParameter P_update_Download_Url = new SqliteParameter("@Download_Url", update_Download_Url);
        //        SqliteParameter P_update_id = new SqliteParameter("@id", update_id);

        //        dbcmd.Parameters.Add(P_update_name);
        //        dbcmd.Parameters.Add(P_update_Download_Url);
        //        dbcmd.Parameters.Add(P_update_id);

        //        dbcmd.CommandText = sqlQuery;
        //        dbcmd.ExecuteScalar();
        //        dbconn.Close();
        //        Search_function(t_id);
        //    }

        //    // SceneManager.LoadScene("home");
        //}

        //Delete
        public void DeleteCategories()
        {
            using (dbconn = new SqliteConnection(conn))
            {

                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "DELETE FROM Category";// table name
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
            }

        }
        public void DeleteItems()
        {
            using (dbconn = new SqliteConnection(conn))
            {

                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "DELETE FROM Item";// table name
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
            }

        }

    }
}
