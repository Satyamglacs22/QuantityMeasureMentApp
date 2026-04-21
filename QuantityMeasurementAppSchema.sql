-- ================================================
-- Database: QuantityMeasurementAppDB
-- UC-17: EF Core manages schema via migrations.
-- This script can be used for manual setup or reference.
-- ================================================

CREATE DATABASE QuantityMeasurementAppDB;
GO

USE QuantityMeasurementAppDB;
GO

-- ================================================
-- Table: quantity_measurements
-- ================================================
CREATE TABLE quantity_measurements
(
    id               BIGINT        IDENTITY(1,1) PRIMARY KEY,
    operation        NVARCHAR(50)  NOT NULL,
    first_value      FLOAT         NOT NULL DEFAULT 0,
    first_unit       NVARCHAR(50)  NULL,
    second_value     FLOAT         NOT NULL DEFAULT 0,
    second_unit      NVARCHAR(50)  NULL,
    result_value     FLOAT         NOT NULL DEFAULT 0,
    measurement_type NVARCHAR(50)  NULL,
    is_error         BIT           NOT NULL DEFAULT 0,
    error_message    NVARCHAR(500) NULL,
    created_at       DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    updated_at       DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE INDEX IX_quantity_measurements_operation        ON quantity_measurements(operation);
CREATE INDEX IX_quantity_measurements_measurement_type ON quantity_measurements(measurement_type);
CREATE INDEX IX_quantity_measurements_is_error         ON quantity_measurements(is_error);
GO
