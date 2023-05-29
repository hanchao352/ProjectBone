

using MongoDB.Driver;

public class MongoDBManager:Singleton<MongoDBManager>
{
    private IMongoClient _client;
    private IMongoDatabase _database;

    string connectionString = "mongodb://localhost:27017";
    string databaseName = "GameDB";
   

    public override void Initialize()
    {
        base.Initialize();
        Initialize(connectionString,databaseName);
    }
     void Initialize(string connectionString, string databaseName)
    {
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
      
    }
    public async Task ConnectAsync()
    {
        await _client.StartSessionAsync();
    }

    public async Task DisconnectAsync()
    {
        // Dispose client to close the connection
        await Task.Run(() => _client = null);
    }

    public async Task InsertAsync<T>(string collectionName, T item)
    {
        var collection = _database.GetCollection<T>(collectionName);
        await collection.InsertOneAsync(item);
        
    }

    public async Task UpdateAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        var collection = _database.GetCollection<T>(collectionName);
        await collection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync<T>(string collectionName, FilterDefinition<T> filter)
    {
        var collection = _database.GetCollection<T>(collectionName);
        await collection.DeleteOneAsync(filter);
    }

    public async Task<T> FindOneAsync<T>(string collectionName, FilterDefinition<T> filter)
    {
        var collection = _database.GetCollection<T>(collectionName);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IMongoClient> GetClientAsync()
    {
        return await Task.FromResult(_client);
    }
    public async Task<List<T>> FindAllAsync<T>(string collectionName)
    {
        var collection = _database.GetCollection<T>(collectionName);
        return await collection.Find(Builders<T>.Filter.Empty).ToListAsync();
    }
    public async Task<List<T>> FindManyAsync<T>(string collectionName, FilterDefinition<T> filter)
    {
        var collection = _database.GetCollection<T>(collectionName);
        var cursor = await collection.FindAsync(filter);
        return await cursor.ToListAsync();
    }
    public async Task<T> FindOneByUsernameAsync<T>(string collectionName, FilterDefinition<T> filter)
    {
        var collection = _database.GetCollection<T>(collectionName);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }
}