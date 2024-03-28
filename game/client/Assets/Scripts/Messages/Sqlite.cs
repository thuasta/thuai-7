using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using Unity.VisualScripting;

namespace Thubg.Sdk
{

    public class Sqlite
    {
        private IDbConnection _dbConnection;

        public void ConnectToDatabase(string path)
        {
            Debug.Log("DataPath:" + path);
            string connectionString = "URI-file:" + path;
            _dbConnection = new SqliteConnection(connectionString);
            _dbConnection.Open();
        }

        public void InsertElement(string tableName, string labelNames, string values)
        {
            if (_dbConnection == null)
            {
                Debug.LogError("Database connection is not established");
                return;
            }
            string insertElementQuery = "INSERT INTO " + tableName + " " + labelNames + " VALUES " + values;
            IDbCommand dbCommand = _dbConnection.CreateCommand();
            dbCommand.CommandText = insertElementQuery;
            dbCommand.ExecuteNonQuery();
        }

        public IDataReader ReadFromDatabase(string tableName)
        {
            if (_dbConnection == null)
            {
                Debug.LogError("Database connection is not established");
                return null;
            }
            string selectElementQuery = "SELECT * FROM " + tableName;
            IDbCommand dbCommand = _dbConnection.CreateCommand();
            dbCommand.CommandText = selectElementQuery;
            IDataReader reader = dbCommand.ExecuteReader();
            return reader;
        }

        public IDataReader ReadFromDatabase(string tableName, string condition)
        {
            if (_dbConnection == null)
            {
                Debug.LogError("Database connection is not established");
                return null;
            }
            string selectElementQuery = "SELECT * FROM " + tableName + " WHERE " + condition;
            IDbCommand dbCommand = _dbConnection.CreateCommand();
            dbCommand.CommandText = selectElementQuery;
            IDataReader reader = dbCommand.ExecuteReader();
            return reader;
        }

        void DisconnectFromDatabase()
        {
            _dbConnection.Close();
        }

        void OnDestroy() 
        {
            _dbConnection.Close();
        }
    }
}