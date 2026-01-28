using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ProductPriceCalculator
{
    /// <summary>
    /// Database Manager - Handles all SQLite database operations
    /// Stores products, subproducts, and operating costs
    /// </summary>
    public class DatabaseManager
    {
        private readonly string connectionString;
        private readonly string dbPath;

        public DatabaseManager()
        {
            // Store database in user's AppData folder
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataPath, "ProductPriceCalculator");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            dbPath = Path.Combine(appFolder, "products.db");
            connectionString = $"Data Source={dbPath}";
            
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                -- Products Table
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    BaseCost REAL NOT NULL,
                    Markup REAL NOT NULL,
                    TaxRate REAL NOT NULL,
                    ExpectedMonthlyUnits REAL NOT NULL DEFAULT 100,
                    UnitsPerPackage REAL NOT NULL DEFAULT 1,
                    IsComponent INTEGER NOT NULL DEFAULT 0,
                    CreatedDate TEXT NOT NULL,
                    LastModified TEXT NOT NULL
                );

                -- Subproducts Table
                CREATE TABLE IF NOT EXISTS Subproducts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Cost REAL NOT NULL,
                    TaxRate REAL NOT NULL,
                    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
                );

                -- Operating Costs Table (GLOBAL - applies to all products)
                CREATE TABLE IF NOT EXISTS OperatingCosts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    IsMonthly INTEGER NOT NULL DEFAULT 1
                );

                -- Create index for faster queries
                CREATE INDEX IF NOT EXISTS idx_subproducts_productid ON Subproducts(ProductId);
            ";
            command.ExecuteNonQuery();

            // Migration: Add ExpectedMonthlyUnits column if it doesn't exist
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM pragma_table_info('Products') 
                WHERE name='ExpectedMonthlyUnits';
            ";
            var columnExists = (long)command.ExecuteScalar() > 0;

            if (!columnExists)
            {
                command.CommandText = @"
                    ALTER TABLE Products 
                    ADD COLUMN ExpectedMonthlyUnits REAL NOT NULL DEFAULT 100;
                ";
                command.ExecuteNonQuery();
            }

            // Migration: Add UnitsPerPackage column if it doesn't exist
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM pragma_table_info('Products') 
                WHERE name='UnitsPerPackage';
            ";
            var unitsPerPackageExists = (long)command.ExecuteScalar() > 0;

            if (!unitsPerPackageExists)
            {
                command.CommandText = @"
                    ALTER TABLE Products 
                    ADD COLUMN UnitsPerPackage REAL NOT NULL DEFAULT 1;
                ";
                command.ExecuteNonQuery();
            }

            // Migration: Add IsComponent column if it doesn't exist
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM pragma_table_info('Products') 
                WHERE name='IsComponent';
            ";
            var isComponentExists = (long)command.ExecuteScalar() > 0;

            if (!isComponentExists)
            {
                command.CommandText = @"
                    ALTER TABLE Products 
                    ADD COLUMN IsComponent INTEGER NOT NULL DEFAULT 0;
                ";
                command.ExecuteNonQuery();
            }

            // Migration: Remove ProductId from OperatingCosts if it exists (make it global)
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM pragma_table_info('OperatingCosts') 
                WHERE name='ProductId';
            ";
            var productIdExists = (long)command.ExecuteScalar() > 0;

            if (productIdExists)
            {
                // SQLite doesn't support DROP COLUMN, so we need to recreate the table
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS OperatingCosts_New (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Amount REAL NOT NULL,
                        IsMonthly INTEGER NOT NULL DEFAULT 1
                    );

                    INSERT INTO OperatingCosts_New (Name, Amount, IsMonthly)
                    SELECT DISTINCT Name, Amount, IsMonthly FROM OperatingCosts;

                    DROP TABLE OperatingCosts;

                    ALTER TABLE OperatingCosts_New RENAME TO OperatingCosts;
                ";
                command.ExecuteNonQuery();
            }
        }

        // ============ PRODUCTS ============
        
        public long SaveProduct(Product product)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            
            if (product.Id == 0)
            {
                // Insert new product
                command.CommandText = @"
                    INSERT INTO Products (Name, BaseCost, Markup, TaxRate, ExpectedMonthlyUnits, UnitsPerPackage, IsComponent, CreatedDate, LastModified)
                    VALUES (@name, @baseCost, @markup, @taxRate, @expectedUnits, @unitsPerPackage, @isComponent, @created, @modified);
                    SELECT last_insert_rowid();
                ";
                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@baseCost", product.BaseCost);
                command.Parameters.AddWithValue("@markup", product.Markup);
                command.Parameters.AddWithValue("@taxRate", product.TaxRate);
                command.Parameters.AddWithValue("@expectedUnits", product.ExpectedMonthlyUnits);
                command.Parameters.AddWithValue("@unitsPerPackage", product.UnitsPerPackage);
                command.Parameters.AddWithValue("@isComponent", product.IsComponent ? 1 : 0);
                command.Parameters.AddWithValue("@created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                
                return (long)command.ExecuteScalar();
            }
            else
            {
                // Update existing product
                command.CommandText = @"
                    UPDATE Products 
                    SET Name = @name, BaseCost = @baseCost, Markup = @markup, 
                        TaxRate = @taxRate, ExpectedMonthlyUnits = @expectedUnits, UnitsPerPackage = @unitsPerPackage, 
                        IsComponent = @isComponent, LastModified = @modified
                    WHERE Id = @id
                ";
                command.Parameters.AddWithValue("@id", product.Id);
                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@baseCost", product.BaseCost);
                command.Parameters.AddWithValue("@markup", product.Markup);
                command.Parameters.AddWithValue("@taxRate", product.TaxRate);
                command.Parameters.AddWithValue("@expectedUnits", product.ExpectedMonthlyUnits);
                command.Parameters.AddWithValue("@unitsPerPackage", product.UnitsPerPackage);
                command.Parameters.AddWithValue("@isComponent", product.IsComponent ? 1 : 0);
                command.Parameters.AddWithValue("@modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                
                command.ExecuteNonQuery();
                return product.Id;
            }
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, BaseCost, Markup, TaxRate, ExpectedMonthlyUnits, UnitsPerPackage, IsComponent, CreatedDate, LastModified FROM Products ORDER BY IsComponent ASC, LastModified DESC";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    BaseCost = reader.GetDouble(2),
                    Markup = reader.GetDouble(3),
                    TaxRate = reader.GetDouble(4),
                    ExpectedMonthlyUnits = reader.GetDouble(5),
                    UnitsPerPackage = reader.GetDouble(6),
                    IsComponent = reader.GetInt32(7) == 1,
                    CreatedDate = DateTime.Parse(reader.GetString(8)),
                    LastModified = DateTime.Parse(reader.GetString(9))
                });
            }

            return products;
        }

        public Product GetProduct(long id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, BaseCost, Markup, TaxRate, ExpectedMonthlyUnits, UnitsPerPackage, IsComponent, CreatedDate, LastModified FROM Products WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    BaseCost = reader.GetDouble(2),
                    Markup = reader.GetDouble(3),
                    TaxRate = reader.GetDouble(4),
                    ExpectedMonthlyUnits = reader.GetDouble(5),
                    UnitsPerPackage = reader.GetDouble(6),
                    IsComponent = reader.GetInt32(7) == 1,
                    CreatedDate = DateTime.Parse(reader.GetString(8)),
                    LastModified = DateTime.Parse(reader.GetString(9))
                };
            }

            return null;
        }

        public void DeleteProduct(long id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Products WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        // ============ SUBPRODUCTS ============

        public long SaveSubproduct(SubproductDb subproduct)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            
            if (subproduct.Id == 0)
            {
                command.CommandText = @"
                    INSERT INTO Subproducts (ProductId, Name, Description, Cost, TaxRate)
                    VALUES (@productId, @name, @description, @cost, @taxRate);
                    SELECT last_insert_rowid();
                ";
                command.Parameters.AddWithValue("@productId", subproduct.ProductId);
                command.Parameters.AddWithValue("@name", subproduct.Name);
                command.Parameters.AddWithValue("@description", subproduct.Description ?? "");
                command.Parameters.AddWithValue("@cost", subproduct.Cost);
                command.Parameters.AddWithValue("@taxRate", subproduct.TaxRate);
                
                return (long)command.ExecuteScalar();
            }
            else
            {
                command.CommandText = @"
                    UPDATE Subproducts 
                    SET Name = @name, Description = @description, Cost = @cost, TaxRate = @taxRate
                    WHERE Id = @id
                ";
                command.Parameters.AddWithValue("@id", subproduct.Id);
                command.Parameters.AddWithValue("@name", subproduct.Name);
                command.Parameters.AddWithValue("@description", subproduct.Description ?? "");
                command.Parameters.AddWithValue("@cost", subproduct.Cost);
                command.Parameters.AddWithValue("@taxRate", subproduct.TaxRate);
                
                command.ExecuteNonQuery();
                return subproduct.Id;
            }
        }

        public List<SubproductDb> GetSubproducts(long productId)
        {
            var subproducts = new List<SubproductDb>();
            
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, ProductId, Name, Description, Cost, TaxRate FROM Subproducts WHERE ProductId = @productId";
            command.Parameters.AddWithValue("@productId", productId);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                subproducts.Add(new SubproductDb
                {
                    Id = reader.GetInt64(0),
                    ProductId = reader.GetInt64(1),
                    Name = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Cost = reader.GetDouble(4),
                    TaxRate = reader.GetDouble(5)
                });
            }

            return subproducts;
        }

        public void DeleteSubproduct(long id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Subproducts WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        // ============ OPERATING COSTS ============

        public long SaveOperatingCost(OperatingCost cost)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            
            if (cost.Id == 0)
            {
                command.CommandText = @"
                    INSERT INTO OperatingCosts (Name, Amount, IsMonthly)
                    VALUES (@name, @amount, @isMonthly);
                    SELECT last_insert_rowid();
                ";
                command.Parameters.AddWithValue("@name", cost.Name);
                command.Parameters.AddWithValue("@amount", cost.Amount);
                command.Parameters.AddWithValue("@isMonthly", cost.IsMonthly ? 1 : 0);
                
                return (long)command.ExecuteScalar();
            }
            else
            {
                command.CommandText = @"
                    UPDATE OperatingCosts 
                    SET Name = @name, Amount = @amount, IsMonthly = @isMonthly
                    WHERE Id = @id
                ";
                command.Parameters.AddWithValue("@id", cost.Id);
                command.Parameters.AddWithValue("@name", cost.Name);
                command.Parameters.AddWithValue("@amount", cost.Amount);
                command.Parameters.AddWithValue("@isMonthly", cost.IsMonthly ? 1 : 0);
                
                command.ExecuteNonQuery();
                return cost.Id;
            }
        }

        public List<OperatingCost> GetOperatingCosts()
        {
            var costs = new List<OperatingCost>();
            
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Amount, IsMonthly FROM OperatingCosts";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                costs.Add(new OperatingCost
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Amount = reader.GetDouble(2),
                    IsMonthly = reader.GetInt32(3) == 1
                });
            }

            return costs;
        }

        public void DeleteOperatingCost(long id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM OperatingCosts WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public string GetDatabasePath()
        {
            return dbPath;
        }
    }

    // ============ DATA MODELS ============

    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double BaseCost { get; set; }
        public double Markup { get; set; }
        public double TaxRate { get; set; }
        public double ExpectedMonthlyUnits { get; set; }
        public double UnitsPerPackage { get; set; }
        public bool IsComponent { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class SubproductDb
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public double TaxRate { get; set; }
    }

    public class OperatingCost
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public bool IsMonthly { get; set; }
        
        public string FrequencyText 
        { 
            get => IsMonthly ? "(monthly / monatlich)" : "(one-time / einmalig)"; 
        }
    }
}
