using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.UI;

public class AccessTest : MonoBehaviour
{
    public InputField name;
    public InputField age;
    public InputField background;

    private void Start()
    {
        ReadCharacters();
    }

    private void ReadCharacters()
    {
        string connectionString = "URI=file:" + Application.dataPath + "/unity_database.db";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Character";
                command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        print((string)reader[1] + "\n" +
                              (int)reader[2] + "\n" +
                              (string)reader[3]);
                    }
                }
            }
        }
    }

    public void Save()
    {
        string connectionString = "URI=file:" + Application.dataPath + "/unity_database.db";

        string pName = this.name.text;
        int pAge = int.Parse(age.text);
        string pBackground = background.text;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = "INSERT INTO Character VALUES (null, @name, @age, @background);";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqliteParameter("name", pName));
                        command.Parameters.Add(new SqliteParameter("age", pAge));
                        command.Parameters.Add(new SqliteParameter("background", pBackground));

                        command.Transaction = transaction;

                        var rows = command.ExecuteNonQuery();

                        transaction.Commit();

                        print(rows + " affected");
                    }
                    catch(Exception e)
                    {
                        transaction.Rollback();
                        print(e.Message);
                    }
                }
            }
        }
    }
}
