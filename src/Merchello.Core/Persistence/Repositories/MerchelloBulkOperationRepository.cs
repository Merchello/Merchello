namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// A repository for bulk SQL operations.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The Type of entity
    /// </typeparam>
    /// <typeparam name="TDto">
    /// The type of Dto object
    /// </typeparam>
    internal abstract class MerchelloBulkOperationRepository<TEntity, TDto> : MerchelloPetaPocoRepositoryBase<TEntity>
        where TEntity : class, IEntity
        where TDto : class, IDto
    {
        /// <summary>
        /// The factory for creating the DTOs.
        /// </summary>
        private readonly Func<IEntityFactory<TEntity, TDto>> _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloBulkOperationRepository{TEntity, TDto}"/> class.
        /// </summary>
        /// <param name="work">
        /// The database unit of work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        protected MerchelloBulkOperationRepository(IDatabaseUnitOfWork work, CacheHelper cache, ILogger logger, ISqlSyntaxProvider sqlSyntax, Func<IEntityFactory<TEntity, TDto>> factory)
            : base(work, cache, logger, sqlSyntax)
        {
            _factory = factory;
        }

        protected void BulkInsertRecordsWithKey<T>(IEnumerable<T> collection)
        {
            //don't do anything if there are no records.
            if (collection.Any() == false)
                return;

            using (var tr = Database.GetTransaction())
            {
                BulkInsertRecordsWithKey(collection, tr, true);
            }
        }

        /// <summary>
        /// Performs the bulk insertion in the context of a current transaction with an optional parameter to complete the transaction
        /// when finished
        /// </summary>
        protected void BulkInsertRecordsWithKey<T>(IEnumerable<T> collection, Transaction tr, bool commitTrans = false)
        {
            //don't do anything if there are no records.
            var dtos = collection as T[] ?? collection.ToArray();
            if (dtos.Any() == false)
                return;

            try
            {
                //if it is sql ce or it is a sql server version less than 2008, we need to do individual inserts.
                var sqlServerSyntax = SqlSyntax as SqlServerSyntaxProvider;
                if (sqlServerSyntax == null || SqlSyntax is SqlCeSyntaxProvider)
                {
                    //SqlCe doesn't support bulk insert statements!
                    foreach (var poco in dtos)
                    {
                        Database.Insert(poco);
                    }
                }
                else
                {
                    string[] sqlStatements;
                    var cmds = GenerateBulkInsertCommand(dtos, Database.Connection, out sqlStatements);
                    for (var i = 0; i < sqlStatements.Length; i++)
                    {
                        using (var cmd = cmds[i])
                        {
                            cmd.CommandText = sqlStatements[i];
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                if (commitTrans)
                {
                    tr.Complete();
                }
            }
            catch
            {
                if (commitTrans)
                {
                    tr.Dispose();
                }
                throw;
            }
        }

        /// <summary>
        /// Executes a batch update.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        protected void ExecuteBatchUpdate(IEnumerable<TEntity> entities)
        {

            var entitiesArray = entities as TEntity[] ?? entities.ToArray();
            if (!entitiesArray.Any()) return;


            if (SqlSyntax is SqlCeSyntaxProvider)
            {
                foreach (var e in entitiesArray) PersistUpdatedItem(e);
                return;
            }


            var factory = _factory.Invoke();

            var dtos = new List<TDto>();
            
            foreach (var e in entitiesArray)
            {
                ApplyAddingOrUpdating(TransactionType.Update, e);
                dtos.Add(factory.BuildDto(e));
            }

            // get the table name and column counts
            var tableMeta = dtos.First().TableNameColumnCount();
            var tableName = tableMeta.Item1;
            var parameterCount = tableMeta.Item2;


            // Each record requires parameters, so we need to split into batches due to the
            // hard-coded 2100 parameter limit imposed by SQL Server
            var batchSize = 2100 / parameterCount;
            var batchCount = ((dtos.Count() - 1) / batchSize) + 1;
            for (var batchIndex = 0; batchIndex < batchCount; batchIndex++)
            {
                var sqlStatement = string.Empty;

                int paramIndex = 0;
                var parms = new List<object>();

                foreach (var dto in dtos.Skip(batchIndex * batchSize).Take(batchSize))
                {
                    var allColumns = dto.ColumnValues().ToArray();
                    var keyColumn = allColumns.First(x => x.Item1 == "pk"); // this is always named pk in Merchello
                    var paramColumns = allColumns.Where(x => x.Item1 != "pk");
                    
                    sqlStatement += string.Format(" UPDATE [{0}] SET", tableName);
                    var firstParam = true;

                    foreach (var pc in paramColumns)
                    {
                        sqlStatement += string.Format("{0} [{1}] = @{2}", firstParam ? string.Empty : ",", pc.Item1, paramIndex++);
                        parms.Add(pc.Item2);
                        if (firstParam) firstParam = false;
                    }
              
                    sqlStatement += string.Format(" WHERE [pk] = @{0};", paramIndex++);

                    parms.Add(keyColumn.Item2);
                }

                // Commit the batch
                Database.Execute(sqlStatement, parms.ToArray());
            }
        }

        /// <summary>
        /// Calls AddingEntity or Updating Entity.
        /// </summary>
        /// <param name="transactionType">
        /// The transaction type.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <remarks>
        /// This needs to be done in the typed repository as some entities override the method defined in EntityBase
        /// </remarks>
        protected abstract void ApplyAddingOrUpdating(TransactionType transactionType, TEntity entity);


        private IDbCommand[] GenerateBulkInsertCommand<T>(
            IEnumerable<T> collection,
            IDbConnection connection,
            out string[] sql)
        {
            //A filter used below a few times to get all columns except result cols and not the primary key if it is auto-incremental
            Func<Database.PocoData, KeyValuePair<string, Database.PocoColumn>, bool> includeColumn = (data, column) =>
            {
                if (column.Value.ResultColumn) return false;
                if (data.TableInfo.AutoIncrement && column.Key == data.TableInfo.PrimaryKey) return false;
                return true;
            };

            var pd = Umbraco.Core.Persistence.Database.PocoData.ForType(typeof(T));
            var tableName = Database.EscapeTableName(pd.TableInfo.TableName);

            //get all columns to include and format for sql
            var cols = string.Join(", ",
                pd.Columns
                .Where(c => includeColumn(pd, c))
                .Select(c => tableName + "." + Database.EscapeSqlIdentifier(c.Key)).ToArray());

            var itemArray = collection.ToArray();

            //calculate number of parameters per item
            var paramsPerItem = pd.Columns.Count(i => includeColumn(pd, i));

            //Example calc:
            // Given: we have 4168 items in the itemArray, each item contains 8 command parameters (values to be inserterted)                
            // 2100 / 8 = 262.5
            // Math.Floor(2100 / 8) = 262 items per trans
            // 4168 / 262 = 15.908... = there will be 16 trans in total

            //all items will be included if we have disabled db parameters
            var itemsPerTrans = Math.Floor(2000.00 / paramsPerItem);
            //there will only be one transaction if we have disabled db parameters
            var numTrans = Math.Ceiling(itemArray.Length / itemsPerTrans);

            var sqlQueries = new List<string>();
            var commands = new List<IDbCommand>();

            for (var tIndex = 0; tIndex < numTrans; tIndex++)
            {
                var itemsForTrans = itemArray
                    .Skip(tIndex * (int)itemsPerTrans)
                    .Take((int)itemsPerTrans);

                var cmd = Database.CreateCommand(connection, "");
                var pocoValues = new List<string>();
                var index = 0;
                foreach (var poco in itemsForTrans)
                {
                    var values = new List<string>();
                    //get all columns except result cols and not the primary key if it is auto-incremental
                    foreach (var i in pd.Columns.Where(x => includeColumn(pd, x)))
                    {
                        AddParam(cmd, i.Value.GetValue(poco), "@");
                        values.Add(string.Format("{0}{1}", "@", index++));
                    }
                    pocoValues.Add("(" + string.Join(",", values.ToArray()) + ")");
                }

                var sqlResult = string.Format("INSERT INTO {0} ({1}) VALUES {2}", tableName, cols, string.Join(", ", pocoValues));
                sqlQueries.Add(sqlResult);
                commands.Add(cmd);
            }

            sql = sqlQueries.ToArray();

            return commands.ToArray();
        }


        private void AddParam(IDbCommand cmd, object item, string ParameterPrefix)
        {
            // Convert value to from poco type to db type
            if (Umbraco.Core.Persistence.Database.Mapper != null && item != null)
            {
                var fn = Umbraco.Core.Persistence.Database.Mapper.GetToDbConverter(item.GetType());
                if (fn != null)
                    item = fn(item);
            }

            // Support passed in parameters
            var idbParam = item as IDbDataParameter;
            if (idbParam != null)
            {
                idbParam.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }

            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
            if (item == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                var t = item.GetType();
                if (t.IsEnum)       // PostgreSQL .NET driver wont cast enum to int
                {
                    p.Value = (int)item;
                }
                else if (t == typeof(Guid))
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                }
                else if (t == typeof(string))
                {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. 
                    // Set before attempting to set Size, or Size will always max out at 4000
                    if ((item as string).Length + 1 > 4000 && p.GetType().Name == "SqlCeParameter")
                        p.GetType().GetProperty("SqlDbType").SetValue(p, SqlDbType.NText, null);

                    p.Size = (item as string).Length + 1;
                    if (p.Size < 4000)
                        p.Size = Math.Max((item as string).Length + 1, 4000);       // Help query plan caching by using common size

                    p.Value = item;
                }
                else if (t == typeof(AnsiString))
                {
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    p.Size = Math.Max((item as AnsiString).Value.Length + 1, 4000);
                    p.Value = (item as AnsiString).Value;
                    p.DbType = DbType.AnsiString;
                }
                else if (t == typeof(bool))
                {
                    p.Value = ((bool)item) ? 1 : 0;
                }
                else if (item.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                    p.Value = item;
                }

                else if (item.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                    p.Value = item;
                }
                else
                {
                    p.Value = item;
                }
            }

            cmd.Parameters.Add(p);
        }
    }
}