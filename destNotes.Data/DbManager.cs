using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

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

        public async Task<T> LoadData<T>(string name, string id)
        {
            return (await _connection.QueryAsync<T>($"select * from {name} where Id = @id", 
                new {id})).FirstOrDefault();
        }

        public async Task AddData<T>(string name, T obj)
        {
            var fields = obj.GetType().GetProperties().Select(s => s.Name);
            await _connection.ExecuteAsync($"insert into {name} ({string.Join(", ", fields)}) values (@{string.Join(", @", fields)})", obj);
        }

        public async Task OverrideData<T>(string name, T obj, string id)
        {
            var fields = obj.GetType().GetProperties().Select(s => $"{s.Name} = @{s.Name}");
            await _connection.ExecuteAsync($"update {name} set {string.Join(", ", fields)} where Id = {id}", obj);
        }

        public async Task TruncateData(string name)
        {
            await _connection.ExecuteAsync($"delete * from {name}");
        }
    }
}