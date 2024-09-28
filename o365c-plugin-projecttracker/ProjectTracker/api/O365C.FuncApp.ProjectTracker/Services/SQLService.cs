using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using O365C.FuncApp.ProjectTracker.Models;

namespace O365C.FuncApp.ProjectTracker.Services
{
    public interface ISQLService
    {
        Task<IEnumerable<T>> GetDataAsync<T>(string query, object parameters = null);        
        Task<int> UpdateTableAsync(string query, object parameters = null);
        Task<IEnumerable<Project>> GetProjectsByStatusAsync<Project>(string storedProcedureName, object parameters = null);
        Task<IEnumerable<ProjectTask>> GetProjectTasksAsync<ProjectTask>(string storedProcedureName, object parameters = null);
        Task<IEnumerable<ProjectTask>> GetTaskByIdAsync<ProjectTask>(string storedProcedureName, object parameters = null);
        Task UpdateTaskAsync(string storedProcedureName, object parameters = null); 

    }
    public class SQLService: ISQLService
    {
        private readonly AzureFunctionSettings _azureFunctionSettings;

        public SQLService(AzureFunctionSettings azureFunctionSettings)
        {
            _azureFunctionSettings = azureFunctionSettings;
        }        

        public async Task<IEnumerable<T>> GetDataAsync<T>(string query, object parameters = null)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_azureFunctionSettings.SqlConnectionString))
                {
                    
                    return await db.QueryAsync<T>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public async Task<int> UpdateTableAsync(string query, object parameters = null)
        {
            using (IDbConnection db = new SqlConnection(_azureFunctionSettings.SqlConnectionString))
            {
                return await db.ExecuteAsync(query, parameters);
            }
        }
        public async Task<IEnumerable<Project>> GetProjectsByStatusAsync<Project>(string storedProcedureName, object parameters = null)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_azureFunctionSettings.SqlConnectionString))
                {
                    return await db.QueryAsync<Project>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ProjectTask>> GetTaskByIdAsync<ProjectTask>(string storedProcedureName, object parameters = null)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_azureFunctionSettings.SqlConnectionString))
                {
                    return await db.QueryAsync<ProjectTask>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ProjectTask>> GetProjectTasksAsync<ProjectTask>(string storedProcedureName, object parameters = null)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_azureFunctionSettings.SqlConnectionString))
                {
                    return await db.QueryAsync<ProjectTask>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

         public async Task UpdateTaskAsync(string storedProcedureName, object parameters = null) // New method implementation
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_azureFunctionSettings.SqlConnectionString))
                {
                    await db.QueryAsync(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}