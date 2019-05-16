using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace destNotes.Data
{
    public class DbManager
    {
        private readonly IDbConnection _connection;

        public DbManager()
        {
            _connection = new SQLiteConnection("Data Source=DbContext.db;Version=3");
        }

        public async Task<IEnumerable<T>> LoadData<T>(string name)
        {
            return await _connection.QueryAsync<T>($"select * from {name}");
        }

        public async Task<IEnumerable<T>> LoadData<T>(string name, string idName, string id)
        {
            return await _connection.QueryAsync<T>($"select * from {name} where {idName} = '{id}'");
        }

        public async Task<T> LoadData<T>(string name, string id)
        {
            return (await _connection.QueryAsync<T>($"select * from {name} where Id = @id",
                new { id })).FirstOrDefault();
        }

        public async Task AddData<T>(string name, T obj)
        {
            await _connection.ExecuteAsync(GetSqlFromObject(name, obj), obj);
        }

        public async Task AddListOfData<T>(string name, IEnumerable<T> objects, string idName, string id)
        {
            await _connection.ExecuteAsync($"delete from {name} where {idName} = '{id}'");
            if (!(objects.FirstOrDefault() is T obj)) return;
            await _connection.ExecuteAsync(GetSqlFromObject(name, obj), objects);
        }

        public async Task OverrideData<T>(string name, T obj, string id)
        {
            var fields = obj.GetType().GetProperties().Select(s => $"{s.Name} = @{s.Name}");
            var sql = $"update {name} set {string.Join(", ", fields)} where Id = '{id}'";
            await _connection.ExecuteAsync(sql, obj);
        }

        public async Task TruncateData(string name)
        {
            await _connection.ExecuteAsync($"delete from {name}");
        }

        public async Task DeleteData(string name, string id)
        {
            await _connection.ExecuteAsync($"delete from {name} where Id = @id", new {id});
        }

        private static string GetSqlFromObject<T>(string name, T obj)
        {
            var fields = obj.GetType().GetProperties().Select(s => s.Name);
            return $"insert into {name} ({string.Join(", ", fields)}) values (@{string.Join(", @", fields)})";
        }
    }
}