using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
namespace QuantityMeasurementAppRepository.Utilities

{
    // Purpose:
    // Manages a pool of reusable SqlConnection objects.
    // The DatabaseRepository borrows a connection when it needs one
    // and returns it when done so connections are reused efficiently.

    public class ConnectionPool : IDisposable
    {
        private static ConnectionPool instance;
        private static readonly object lockObject = new object();

        private Stack<SqlConnection> availableConnections;
        private int totalCreated;
        private int maxPoolSize;
        private string connectionString;

        private ConnectionPool(ApplicationConfig config)
        {
            connectionString     = config.GetConnectionString();
            maxPoolSize          = config.GetMaxPoolSize();
            availableConnections = new Stack<SqlConnection>();
            totalCreated         = 0;

            Console.WriteLine("[ConnectionPool] Initializing. Max size: " + maxPoolSize);

            // Pre-open 2 connections at startup so first calls are fast
            int initialSize = Math.Min(2, maxPoolSize);
            for (int i = 0; i < initialSize; i++)
            {
                SqlConnection conn = CreateNewConnection();
                availableConnections.Push(conn);
            }

            Console.WriteLine("[ConnectionPool] Ready. Pre-created: " + initialSize);
        }

        // Singleton accessor - thread safe double-checked locking
        public static ConnectionPool GetInstance(ApplicationConfig config)
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new ConnectionPool(config);
                    }
                }
            }
            return instance;
        }

        // Opens and returns a brand new SqlConnection
        private SqlConnection CreateNewConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            totalCreated++;
            return conn;
        }

        // Borrows a connection from the pool
        public SqlConnection GetConnection()
        {
            lock (lockObject)
            {
                if (availableConnections.Count > 0)
                {
                    SqlConnection conn = availableConnections.Pop();
                    // Re-open if the connection was closed (e.g. network timeout)
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    return conn;
                }

                // Pool not full - create a new connection
                if (totalCreated < maxPoolSize)
                {
                    Console.WriteLine("[ConnectionPool] Creating new connection. Total: "
                        + (totalCreated + 1));
                    return CreateNewConnection();
                }

                // Pool is exhausted - wait briefly and retry
                Console.WriteLine("[ConnectionPool] Pool exhausted. Waiting 500ms...");
                System.Threading.Thread.Sleep(500);

                if (availableConnections.Count > 0)
                {
                    return availableConnections.Pop();
                }

                // Last resort - create an emergency connection beyond pool limit
                Console.WriteLine("[ConnectionPool] Creating emergency connection.");
                return CreateNewConnection();
            }
        }

        // Returns a borrowed connection back to the pool
        public void ReturnConnection(SqlConnection conn)
        {
            if (conn == null)
            {
                return;
            }

            lock (lockObject)
            {
                if (availableConnections.Count < maxPoolSize
                    && conn.State == ConnectionState.Open)
                {
                    availableConnections.Push(conn);
                }
                else
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        // Returns a human readable summary of the pool state
        public string GetPoolStatistics()
        {
            return "Total created: " + totalCreated
                + " | Idle: " + availableConnections.Count
                + " | Max: " + maxPoolSize;
        }

        // Closes all connections in the pool - call on application shutdown
        public void Dispose()
        {
            lock (lockObject)
            {
                Console.WriteLine("[ConnectionPool] Disposing all connections...");
                while (availableConnections.Count > 0)
                {
                    SqlConnection conn = availableConnections.Pop();
                    conn.Close();
                    conn.Dispose();
                }
                instance = null; // Allow re-creation if needed
                Console.WriteLine("[ConnectionPool] All connections disposed.");
            }
        }
    }
}