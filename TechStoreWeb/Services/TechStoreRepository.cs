using System.Data;
using Microsoft.Data.SqlClient;
using TechStoreWeb.Models;

namespace TechStoreWeb.Services;

public sealed class TechStoreRepository
{
    private readonly string _connectionString;

    public TechStoreRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TechStoreSqlServer")
            ?? throw new InvalidOperationException("Missing connection string 'TechStoreSqlServer'.");
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        const string sql = @"
SELECT ProductID, ProductName, Price, Category
FROM dbo.SanPham
ORDER BY ProductID;";

        var results = new List<Product>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(new Product
            {
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Category = reader.GetString(reader.GetOrdinal("Category"))
            });
        }

        return results;
    }

    public async Task<ProductCrudResult> CreateProductAsync(ProductFormModel input)
    {
        const string insertHqSql = @"
INSERT INTO dbo.SanPham (ProductName, Price, Category)
VALUES (@ProductName, @Price, @Category);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

        const string insertBranchSql = @"
INSERT OPENQUERY(MYSQL,
'SELECT ProductID, ProductName, Price, Category FROM TechStore_Branch.SanPham')
VALUES (@ProductID, @ProductName, @Price, @Category);";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        int productId;

        await using (var insertHqCommand = new SqlCommand(insertHqSql, connection))
        {
            insertHqCommand.Parameters.AddWithValue("@ProductName", input.ProductName.Trim());
            insertHqCommand.Parameters.AddWithValue("@Price", input.Price);
            insertHqCommand.Parameters.AddWithValue("@Category", input.Category.Trim());

            var scalar = await insertHqCommand.ExecuteScalarAsync();
            productId = scalar is null ? 0 : Convert.ToInt32(scalar);
        }

        if (productId <= 0)
        {
            return new ProductCrudResult
            {
                Success = false,
                Message = "Failed to create product in HQ database."
            };
        }

        try
        {
            await using var insertBranchCommand = new SqlCommand(insertBranchSql, connection);
            insertBranchCommand.Parameters.AddWithValue("@ProductID", productId);
            insertBranchCommand.Parameters.AddWithValue("@ProductName", input.ProductName.Trim());
            insertBranchCommand.Parameters.AddWithValue("@Price", input.Price);
            insertBranchCommand.Parameters.AddWithValue("@Category", input.Category.Trim());
            await insertBranchCommand.ExecuteNonQueryAsync();

            return new ProductCrudResult
            {
                Success = true,
                ProductID = productId,
                BranchSyncSuccess = true,
                Message = "Product created in HQ and mirrored to Branch."
            };
        }
        catch (Exception ex)
        {
            return new ProductCrudResult
            {
                Success = true,
                ProductID = productId,
                BranchSyncSuccess = false,
                BranchSyncError = ex.Message,
                Message = "Product created in HQ, but Branch mirror failed."
            };
        }
    }

    public async Task<ProductCrudResult> UpdateProductAsync(int productId, ProductFormModel input)
    {
        const string getPriceSql = @"
SELECT Price
FROM dbo.SanPham
WHERE ProductID = @ProductID;";

        const string updateHqSql = @"
UPDATE dbo.SanPham
SET ProductName = @ProductName,
    Price = @Price,
    Category = @Category
WHERE ProductID = @ProductID;";

        const string updateBranchMetaSql = @"
UPDATE OPENQUERY(MYSQL,
'SELECT ProductID, ProductName, Category FROM TechStore_Branch.SanPham')
SET ProductName = @ProductName,
    Category = @Category
WHERE ProductID = @ProductID;";

        const string insertBranchSql = @"
INSERT OPENQUERY(MYSQL,
'SELECT ProductID, ProductName, Price, Category FROM TechStore_Branch.SanPham')
VALUES (@ProductID, @ProductName, @Price, @Category);";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        decimal oldPrice;

        await using (var getPriceCommand = new SqlCommand(getPriceSql, connection))
        {
            getPriceCommand.Parameters.AddWithValue("@ProductID", productId);
            var scalar = await getPriceCommand.ExecuteScalarAsync();
            if (scalar is null || scalar == DBNull.Value)
            {
                return new ProductCrudResult
                {
                    Success = false,
                    Message = "ProductID not found."
                };
            }

            oldPrice = Convert.ToDecimal(scalar);
        }

        await using (var updateHqCommand = new SqlCommand(updateHqSql, connection))
        {
            updateHqCommand.Parameters.AddWithValue("@ProductName", input.ProductName.Trim());
            updateHqCommand.Parameters.AddWithValue("@Price", input.Price);
            updateHqCommand.Parameters.AddWithValue("@Category", input.Category.Trim());
            updateHqCommand.Parameters.AddWithValue("@ProductID", productId);

            var affected = await updateHqCommand.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                return new ProductCrudResult
                {
                    Success = false,
                    Message = "No row updated in HQ."
                };
            }
        }

        bool branchSyncSuccess = true;
        string? branchSyncError = null;

        try
        {
            await using var updateBranchMetaCommand = new SqlCommand(updateBranchMetaSql, connection);
            updateBranchMetaCommand.Parameters.AddWithValue("@ProductName", input.ProductName.Trim());
            updateBranchMetaCommand.Parameters.AddWithValue("@Category", input.Category.Trim());
            updateBranchMetaCommand.Parameters.AddWithValue("@ProductID", productId);

            var affected = await updateBranchMetaCommand.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                await using var insertBranchCommand = new SqlCommand(insertBranchSql, connection);
                insertBranchCommand.Parameters.AddWithValue("@ProductID", productId);
                insertBranchCommand.Parameters.AddWithValue("@ProductName", input.ProductName.Trim());
                insertBranchCommand.Parameters.AddWithValue("@Price", input.Price);
                insertBranchCommand.Parameters.AddWithValue("@Category", input.Category.Trim());
                await insertBranchCommand.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            branchSyncSuccess = false;
            branchSyncError = ex.Message;
        }

        if (oldPrice != input.Price)
        {
            await using var processQueueCommand = new SqlCommand("EXEC dbo.sp_ProcessPriceSyncQueue;", connection);
            await processQueueCommand.ExecuteNonQueryAsync();
        }

        return new ProductCrudResult
        {
            Success = true,
            ProductID = productId,
            BranchSyncSuccess = branchSyncSuccess,
            BranchSyncError = branchSyncError,
            Message = branchSyncSuccess
                ? "Product updated successfully in HQ and Branch metadata sync completed."
                : "Product updated in HQ, but Branch metadata sync failed."
        };
    }

    public async Task<ProductCrudResult> DeleteProductAsync(int productId)
    {
        const string branchInvoiceCountSql = @"
SELECT COUNT(1)
FROM OPENQUERY(MYSQL,
'SELECT ProductID FROM TechStore_Branch.HoaDon')
WHERE ProductID = @ProductID;";

        const string deleteHqSql = @"
DELETE FROM dbo.SanPham
WHERE ProductID = @ProductID;";

        const string deleteBranchSql = @"
DELETE OPENQUERY(MYSQL,
'SELECT ProductID FROM TechStore_Branch.SanPham')
WHERE ProductID = @ProductID;";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using (var countCommand = new SqlCommand(branchInvoiceCountSql, connection))
        {
            countCommand.Parameters.AddWithValue("@ProductID", productId);
            var countScalar = await countCommand.ExecuteScalarAsync();
            var count = countScalar is null ? 0 : Convert.ToInt32(countScalar);
            if (count > 0)
            {
                return new ProductCrudResult
                {
                    Success = false,
                    ProductID = productId,
                    Message = "Cannot delete product because Branch invoices reference this ProductID."
                };
            }
        }

        await using (var deleteHqCommand = new SqlCommand(deleteHqSql, connection))
        {
            deleteHqCommand.Parameters.AddWithValue("@ProductID", productId);
            var affected = await deleteHqCommand.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                return new ProductCrudResult
                {
                    Success = false,
                    ProductID = productId,
                    Message = "ProductID not found in HQ."
                };
            }
        }

        bool branchSyncSuccess = true;
        string? branchSyncError = null;

        try
        {
            await using var deleteBranchCommand = new SqlCommand(deleteBranchSql, connection);
            deleteBranchCommand.Parameters.AddWithValue("@ProductID", productId);
            await deleteBranchCommand.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            branchSyncSuccess = false;
            branchSyncError = ex.Message;
        }

        return new ProductCrudResult
        {
            Success = true,
            ProductID = productId,
            BranchSyncSuccess = branchSyncSuccess,
            BranchSyncError = branchSyncError,
            Message = branchSyncSuccess
                ? "Product deleted from HQ and Branch."
                : "Product deleted from HQ, but Branch delete failed."
        };
    }

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        const string sql = @"
SELECT ProductID, ProductName, Price, Category
FROM dbo.SanPham
WHERE ProductID = @ProductID;";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@ProductID", productId);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Product
        {
            ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
            Category = reader.GetString(reader.GetOrdinal("Category"))
        };
    }

    public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync()
    {
        const string sql = @"
SELECT
    CAST(InvoiceID AS BIGINT) AS InvoiceID,
    CAST(ProductID AS INT) AS ProductID,
    CAST(Quantity AS INT) AS Quantity,
    CAST(SaleDate AS DATETIME2(0)) AS SaleDate,
    CAST(Total AS DECIMAL(18,2)) AS Total
FROM OPENQUERY(MYSQL,
'    SELECT InvoiceID, ProductID, Quantity, SaleDate, Total
     FROM TechStore_Branch.HoaDon')
ORDER BY SaleDate DESC;";

        var results = new List<Invoice>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(new Invoice
            {
                InvoiceID = reader.GetInt64(reader.GetOrdinal("InvoiceID")),
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total"))
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<TransparentJoinRow>> GetTransparentJoinRowsAsync(DateTime? fromDate, DateTime? toDate, int take = 200)
    {
        const string sql = @"
SELECT TOP (@Take)
    p.ProductID,
    p.ProductName,
    CAST(p.Price AS DECIMAL(18,2)) AS HQPrice,
    CAST(h.Quantity AS INT) AS Quantity,
    CAST(h.SaleDate AS DATETIME2(0)) AS SaleDate,
    CAST(h.Total AS DECIMAL(18,2)) AS BranchTotal,
    CAST(h.Quantity * p.Price AS DECIMAL(18,2)) AS RevenueByHQPrice
FROM dbo.SanPham p
INNER JOIN OPENQUERY(MYSQL,
'    SELECT ProductID, Quantity, SaleDate, Total
     FROM TechStore_Branch.HoaDon') h
    ON p.ProductID = CAST(h.ProductID AS INT)
WHERE (@FromDate IS NULL OR CAST(h.SaleDate AS DATETIME2(0)) >= @FromDate)
  AND (@ToDate IS NULL OR CAST(h.SaleDate AS DATETIME2(0)) < DATEADD(DAY, 1, @ToDate))
ORDER BY CAST(h.SaleDate AS DATETIME2(0)) DESC;";

        var results = new List<TransparentJoinRow>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Take", take);
        command.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime2)
        {
            Value = fromDate.HasValue ? fromDate.Value : DBNull.Value
        });
        command.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime2)
        {
            Value = toDate.HasValue ? toDate.Value : DBNull.Value
        });

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new TransparentJoinRow
            {
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                HQPrice = reader.GetDecimal(reader.GetOrdinal("HQPrice")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                BranchTotal = reader.GetDecimal(reader.GetOrdinal("BranchTotal")),
                RevenueByHQPrice = reader.GetDecimal(reader.GetOrdinal("RevenueByHQPrice"))
            });
        }

        return results;
    }

    public async Task<IntegrationSnapshot> GetIntegrationSnapshotAsync()
    {
        var snapshot = new IntegrationSnapshot();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        snapshot.HqProductCount = await ExecuteScalarIntAsync(connection, "SELECT COUNT(1) FROM dbo.SanPham;");
        snapshot.QueuePendingCount = await ExecuteScalarIntAsync(connection, "SELECT COUNT(1) FROM dbo.PriceSyncQueue WHERE Status = 'P';");
        snapshot.QueueSuccessCount = await ExecuteScalarIntAsync(connection, "SELECT COUNT(1) FROM dbo.PriceSyncQueue WHERE Status = 'S';");
        snapshot.QueueErrorCount = await ExecuteScalarIntAsync(connection, "SELECT COUNT(1) FROM dbo.PriceSyncQueue WHERE Status = 'E';");

        try
        {
            await using var testLinkedCommand = new SqlCommand("EXEC master.dbo.sp_testlinkedserver @servername = N'MYSQL';", connection);
            await testLinkedCommand.ExecuteNonQueryAsync();

            snapshot.LinkedServerHealthy = true;
            snapshot.BranchProductCount = await ExecuteScalarIntAsync(connection,
                "SELECT COUNT(1) FROM OPENQUERY(MYSQL, 'SELECT ProductID FROM TechStore_Branch.SanPham');");
            snapshot.BranchInvoiceCount = await ExecuteScalarIntAsync(connection,
                "SELECT COUNT(1) FROM OPENQUERY(MYSQL, 'SELECT InvoiceID FROM TechStore_Branch.HoaDon');");
        }
        catch (Exception ex)
        {
            snapshot.LinkedServerHealthy = false;
            snapshot.LinkedServerError = ex.Message;
        }

        return snapshot;
    }

    public async Task<IReadOnlyList<RevenueRow>> GetRevenueAsync(DateTime? fromDate, DateTime? toDate)
    {
        var results = new List<RevenueRow>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand("dbo.sp_BaoCaoDoanhThu", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime2)
        {
            Value = fromDate.HasValue ? fromDate.Value : DBNull.Value
        });

        command.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime2)
        {
            Value = toDate.HasValue ? toDate.Value : DBNull.Value
        });

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new RevenueRow
            {
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                GiaTaiHQ = reader.GetDecimal(reader.GetOrdinal("GiaTaiHQ")),
                SoLuongBan = reader.GetInt32(reader.GetOrdinal("SoLuongBan")),
                DoanhThu = reader.GetDecimal(reader.GetOrdinal("DoanhThu")),
                LanBanGanNhat = reader.IsDBNull(reader.GetOrdinal("LanBanGanNhat"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("LanBanGanNhat"))
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<PriceSyncQueueItem>> GetLatestQueueAsync(int take = 20)
    {
        const string sql = @"
SELECT TOP (@Take)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    CreatedAt,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
ORDER BY QueueID DESC;";

        var results = new List<PriceSyncQueueItem>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Take", take);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new PriceSyncQueueItem
            {
                QueueID = reader.GetInt64(reader.GetOrdinal("QueueID")),
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                NewPrice = reader.GetDecimal(reader.GetOrdinal("NewPrice")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                ProcessedAt = reader.IsDBNull(reader.GetOrdinal("ProcessedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ProcessedAt")),
                LastError = reader.IsDBNull(reader.GetOrdinal("LastError"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("LastError"))
            });
        }

        return results;
    }

    public async Task<UpdatePriceResult> UpdatePriceAndSyncAsync(int productId, decimal newPrice)
    {
        const string updateSql = @"
UPDATE dbo.SanPham
SET Price = @NewPrice
WHERE ProductID = @ProductID;";

        const string processQueueSql = "EXEC dbo.sp_ProcessPriceSyncQueue;";

        const string latestQueueSql = @"
SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    CreatedAt,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
WHERE ProductID = @ProductID
ORDER BY QueueID DESC;";

        const string branchPriceSql = @"
SELECT TOP (1) CAST(Price AS DECIMAL(18,2)) AS Price
FROM OPENQUERY(MYSQL,
'    SELECT ProductID, Price
     FROM TechStore_Branch.SanPham')
WHERE ProductID = @ProductID;";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using (var updateCommand = new SqlCommand(updateSql, connection))
        {
            updateCommand.Parameters.AddWithValue("@NewPrice", newPrice);
            updateCommand.Parameters.AddWithValue("@ProductID", productId);
            var affected = await updateCommand.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                return new UpdatePriceResult
                {
                    Success = false,
                    Message = "Khong tim thay san pham de cap nhat."
                };
            }
        }

        await using (var processCommand = new SqlCommand(processQueueSql, connection))
        {
            await processCommand.ExecuteNonQueryAsync();
        }

        PriceSyncQueueItem? latestQueue = null;

        await using (var queueCommand = new SqlCommand(latestQueueSql, connection))
        {
            queueCommand.Parameters.AddWithValue("@ProductID", productId);
            await using var queueReader = await queueCommand.ExecuteReaderAsync();
            if (await queueReader.ReadAsync())
            {
                latestQueue = new PriceSyncQueueItem
                {
                    QueueID = queueReader.GetInt64(queueReader.GetOrdinal("QueueID")),
                    ProductID = queueReader.GetInt32(queueReader.GetOrdinal("ProductID")),
                    NewPrice = queueReader.GetDecimal(queueReader.GetOrdinal("NewPrice")),
                    Status = queueReader.GetString(queueReader.GetOrdinal("Status")),
                    CreatedAt = queueReader.GetDateTime(queueReader.GetOrdinal("CreatedAt")),
                    ProcessedAt = queueReader.IsDBNull(queueReader.GetOrdinal("ProcessedAt"))
                        ? null
                        : queueReader.GetDateTime(queueReader.GetOrdinal("ProcessedAt")),
                    LastError = queueReader.IsDBNull(queueReader.GetOrdinal("LastError"))
                        ? null
                        : queueReader.GetString(queueReader.GetOrdinal("LastError"))
                };
            }
        }

        decimal? branchPrice = null;

        await using (var priceCommand = new SqlCommand(branchPriceSql, connection))
        {
            priceCommand.Parameters.AddWithValue("@ProductID", productId);
            var scalar = await priceCommand.ExecuteScalarAsync();
            if (scalar != null && scalar != DBNull.Value)
            {
                branchPrice = Convert.ToDecimal(scalar);
            }
        }

        var success = latestQueue?.Status == "S";

        return new UpdatePriceResult
        {
            Success = success,
            Message = success
                ? "Cap nhat gia thanh cong va da dong bo sang chi nhanh."
                : "Da cap nhat gia tai HQ, nhung dong bo chi nhanh gap loi. Xem LastError trong queue.",
            LatestQueueItem = latestQueue,
            BranchPriceAfterSync = branchPrice
        };
    }

    private static async Task<int> ExecuteScalarIntAsync(SqlConnection connection, string sql)
    {
        await using var command = new SqlCommand(sql, connection);
        var scalar = await command.ExecuteScalarAsync();
        if (scalar is null || scalar == DBNull.Value)
        {
            return 0;
        }

        return Convert.ToInt32(scalar);
    }
}
