using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using QuantityMeasurementAppBusiness.Exceptions;
using QuantityMeasurementAppModels.Entity;
using QuantityMeasurementAppRepository.Interfaces;
using QuantityMeasurementAppRepository.Utilities;

namespace QuantityMeasurementAppRepository.Repositories
{
    // Implements IQuantityMeasurementRepository using ADO.NET + SQL Server.
    // All SQL is encapsulated in stored procedures so no inline SQL appears here.
    // Parameterized stored procedure calls prevent SQL injection.

    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        // Connection pool that provides reusable SqlConnection objects
        private ConnectionPool pool;

        // Constructor receives the pool via dependency injection
        public QuantityMeasurementDatabaseRepository(ConnectionPool connectionPool)
        {
            if (connectionPool == null)
            {
                throw new ArgumentException("ConnectionPool cannot be null");
            }

            pool = connectionPool;
            Console.WriteLine("[DatabaseRepository] Initialized with ADO.NET + SQL Server.");
        }

        // -------------------------------------------------------
        // Save - calls sp_SaveMeasurement stored procedure
        // -------------------------------------------------------
        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Entity cannot be null");
            }

            SqlConnection conn = pool.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_SaveMeasurement", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Add each parameter with its SQL type to prevent SQL injection
                SqlParameter opParam = new SqlParameter("@operation", SqlDbType.NVarChar, 50);
                opParam.Value = entity.Operation;
                cmd.Parameters.Add(opParam);

                SqlParameter fvParam = new SqlParameter("@first_value", SqlDbType.Float);
                fvParam.Value = entity.FirstValue;
                cmd.Parameters.Add(fvParam);

                SqlParameter fuParam = new SqlParameter("@first_unit", SqlDbType.NVarChar, 50);
                fuParam.Value = (object)entity.FirstUnit ?? DBNull.Value;
                cmd.Parameters.Add(fuParam);

                SqlParameter svParam = new SqlParameter("@second_value", SqlDbType.Float);
                svParam.Value = entity.SecondValue;
                cmd.Parameters.Add(svParam);

                SqlParameter suParam = new SqlParameter("@second_unit", SqlDbType.NVarChar, 50);
                suParam.Value = (object)entity.SecondUnit ?? DBNull.Value;
                cmd.Parameters.Add(suParam);

                SqlParameter rvParam = new SqlParameter("@result_value", SqlDbType.Float);
                rvParam.Value = entity.ResultValue;
                cmd.Parameters.Add(rvParam);

                SqlParameter mtParam = new SqlParameter("@measurement_type", SqlDbType.NVarChar, 50);
                mtParam.Value = (object)entity.MeasurementType ?? DBNull.Value;
                cmd.Parameters.Add(mtParam);

                SqlParameter ieParam = new SqlParameter("@is_error", SqlDbType.Bit);
                ieParam.Value = entity.IsError;
                cmd.Parameters.Add(ieParam);

                SqlParameter emParam = new SqlParameter("@error_message", SqlDbType.NVarChar, 500);
                emParam.Value = (object)entity.ErrorMessage ?? DBNull.Value;
                cmd.Parameters.Add(emParam);

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                Console.WriteLine("[DatabaseRepository] Saved: " + entity.Operation);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("[DatabaseRepository] Save failed: " + ex.Message);
                throw new DatabaseException(
                    "Save failed (SQL error " + ex.Number + "): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DatabaseRepository] Save error: " + ex.Message);
                throw new DatabaseException("Save failed: " + ex.Message, ex);
            }
            finally
            {
                // Always return the connection to the pool
                pool.ReturnConnection(conn);
            }
        }

        // -------------------------------------------------------
        // GetAll - calls sp_GetAllMeasurements stored procedure
        // Returns every row ordered by created_at DESC
        // -------------------------------------------------------
        public List<QuantityMeasurementEntity> GetAll()
        {
            SqlConnection conn = pool.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllMeasurements", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                List<QuantityMeasurementEntity> results = ReadEntities(cmd);
                cmd.Dispose();
                return results;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    "GetAll failed (SQL error " + ex.Number + "): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetAll failed: " + ex.Message, ex);
            }
            finally
            {
                pool.ReturnConnection(conn);
            }
        }

        // -------------------------------------------------------
        // GetByOperation - calls sp_GetMeasurementsByOperation
        // Returns only records where operation = @operation
        // -------------------------------------------------------
        public List<QuantityMeasurementEntity> GetByOperation(string operation)
        {
            if (operation == null)
            {
                throw new ArgumentException("Operation cannot be null");
            }

            SqlConnection conn = pool.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetMeasurementsByOperation", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter opParam = new SqlParameter("@operation", SqlDbType.NVarChar, 50);
                opParam.Value = operation;
                cmd.Parameters.Add(opParam);

                List<QuantityMeasurementEntity> results = ReadEntities(cmd);
                cmd.Dispose();
                return results;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    "GetByOperation failed (SQL error " + ex.Number + "): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByOperation failed: " + ex.Message, ex);
            }
            finally
            {
                pool.ReturnConnection(conn);
            }
        }

        // -------------------------------------------------------
        // GetByMeasurementType - calls sp_GetMeasurementsByType
        // Returns only records where measurement_type = @measurement_type
        // -------------------------------------------------------
        public List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
        {
            if (measurementType == null)
            {
                throw new ArgumentException("MeasurementType cannot be null");
            }

            SqlConnection conn = pool.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetMeasurementsByType", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter mtParam = new SqlParameter("@measurement_type", SqlDbType.NVarChar, 50);
                mtParam.Value = measurementType;
                cmd.Parameters.Add(mtParam);

                List<QuantityMeasurementEntity> results = ReadEntities(cmd);
                cmd.Dispose();
                return results;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    "GetByMeasurementType failed (SQL error " + ex.Number + "): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(
                    "GetByMeasurementType failed: " + ex.Message, ex);
            }
            finally
            {
                pool.ReturnConnection(conn);
            }
        }

        // -------------------------------------------------------
        // GetTotalCount - calls sp_GetTotalCount
        // Returns a single integer count of all rows
        // -------------------------------------------------------
        public int GetTotalCount()
        {
            SqlConnection conn = pool.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetTotalCount", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                if (reader.Read())
                {
                    count = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                }

                reader.Close();
                cmd.Dispose();
                return count;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    "GetTotalCount failed (SQL error " + ex.Number + "): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetTotalCount failed: " + ex.Message, ex);
            }
            finally
            {
                pool.ReturnConnection(conn);
            }
        }

        // -------------------------------------------------------
        // DeleteAll - calls sp_DeleteAllMeasurements
        // Removes every row from the table
        // -------------------------------------------------------
        public void DeleteAll()
        {
            SqlConnection conn = pool.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteAllMeasurements", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                Console.WriteLine("[DatabaseRepository] All measurements deleted.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    "DeleteAll failed (SQL error " + ex.Number + "): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("DeleteAll failed: " + ex.Message, ex);
            }
            finally
            {
                pool.ReturnConnection(conn);
            }
        }

        // Returns pool statistics from the ConnectionPool
        public string GetPoolStatistics()
        {
            return pool.GetPoolStatistics();
        }

        // Disposes the connection pool and closes all open connections
        public void ReleaseResources()
        {
            Console.WriteLine("[DatabaseRepository] Releasing resources...");
            pool.Dispose();
            Console.WriteLine("[DatabaseRepository] Resources released.");
        }

        // -------------------------------------------------------
        // Private helper: reads rows from SqlDataReader and builds
        // QuantityMeasurementEntity objects.
        // Shared by GetAll, GetByOperation, GetByMeasurementType.
        // -------------------------------------------------------
        private List<QuantityMeasurementEntity> ReadEntities(SqlCommand cmd)
        {
            List<QuantityMeasurementEntity> list = new List<QuantityMeasurementEntity>();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string operation = reader.GetString(reader.GetOrdinal("operation"));
                bool isError     = reader.GetBoolean(reader.GetOrdinal("is_error"));

                QuantityMeasurementEntity entity;

                if (isError)
                {
                    // Error record - only operation and error_message matter
                    string errorMsg = "";
                    int errCol = reader.GetOrdinal("error_message");
                    if (!reader.IsDBNull(errCol))
                    {
                        errorMsg = reader.GetString(errCol);
                    }
                    entity = new QuantityMeasurementEntity(operation, errorMsg);
                }
                else
                {
                    double firstValue  = reader.GetDouble(reader.GetOrdinal("first_value"));
                    double secondValue = reader.GetDouble(reader.GetOrdinal("second_value"));
                    double resultValue = reader.GetDouble(reader.GetOrdinal("result_value"));

                    string firstUnit = "";
                    int fuCol = reader.GetOrdinal("first_unit");
                    if (!reader.IsDBNull(fuCol))
                    {
                        firstUnit = reader.GetString(fuCol);
                    }

                    string secondUnit = "";
                    int suCol = reader.GetOrdinal("second_unit");
                    if (!reader.IsDBNull(suCol))
                    {
                        secondUnit = reader.GetString(suCol);
                    }

                    string measurementType = "";
                    int mtCol = reader.GetOrdinal("measurement_type");
                    if (!reader.IsDBNull(mtCol))
                    {
                        measurementType = reader.GetString(mtCol);
                    }

                    // Two-operand record if second_unit has a value (Add, Subtract, Compare, Divide)
                    if (secondUnit != "")
                    {
                        entity = new QuantityMeasurementEntity(
                            operation,
                            firstValue,  firstUnit,
                            secondValue, secondUnit,
                            resultValue,
                            measurementType);
                    }
                    else
                    {
                        // Single-operand record (Convert)
                        entity = new QuantityMeasurementEntity(
                            operation,
                            firstValue, firstUnit,
                            resultValue,
                            measurementType);
                    }
                }

                list.Add(entity);
            }

            reader.Close();
            return list;
        }
    }
}