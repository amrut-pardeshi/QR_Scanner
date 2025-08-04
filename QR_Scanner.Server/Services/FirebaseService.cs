using Google.Cloud.Firestore;

namespace QR_Scanner.Server.Services
{
    public interface IFirebaseService
    {
        Task<string> CreateAsync<T>(string collection, T entity) where T : class;
        Task<T?> GetByIdAsync<T>(string collection, string id) where T : class;
        Task<List<T>> GetAllAsync<T>(string collection) where T : class;
        Task<List<T>> GetByFieldAsync<T>(string collection, string field, object value) where T : class;
        Task<bool> UpdateAsync<T>(string collection, string id, T entity) where T : class;
        Task<bool> DeleteAsync(string collection, string id);
        Task<List<T>> QueryAsync<T>(string collection, Query query) where T : class;
    }

    public class FirebaseService : IFirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly ILogger<FirebaseService> _logger;

        public FirebaseService(FirestoreDb firestoreDb, ILogger<FirebaseService> logger)
        {
            _firestoreDb = firestoreDb;
            _logger = logger;
        }

        public async Task<string> CreateAsync<T>(string collection, T entity) where T : class
        {
            try
            {
                var collectionRef = _firestoreDb.Collection(collection);
                
                // Set Id if it's null and the entity has an Id property
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null && idProperty.GetValue(entity) == null)
                {
                    var docRef = collectionRef.Document();
                    idProperty.SetValue(entity, docRef.Id);
                }

                // Set CreatedAt and UpdatedAt if they exist
                SetTimestampProperties(entity, isUpdate: false);

                var documentRef = collectionRef.Document(idProperty?.GetValue(entity)?.ToString() ?? collectionRef.Document().Id);
                await documentRef.SetAsync(entity);
                
                _logger.LogInformation($"Created document with ID: {documentRef.Id} in collection: {collection}");
                return documentRef.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating document in collection: {collection}");
                throw;
            }
        }

        public async Task<T?> GetByIdAsync<T>(string collection, string id) where T : class
        {
            try
            {
                var documentRef = _firestoreDb.Collection(collection).Document(id);
                var snapshot = await documentRef.GetSnapshotAsync();
                
                if (!snapshot.Exists)
                {
                    _logger.LogWarning($"Document with ID: {id} not found in collection: {collection}");
                    return null;
                }

                var entity = snapshot.ConvertTo<T>();
                _logger.LogInformation($"Retrieved document with ID: {id} from collection: {collection}");
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving document with ID: {id} from collection: {collection}");
                throw;
            }
        }

        public async Task<List<T>> GetAllAsync<T>(string collection) where T : class
        {
            try
            {
                var collectionRef = _firestoreDb.Collection(collection);
                var snapshot = await collectionRef.GetSnapshotAsync();
                
                var entities = snapshot.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
                _logger.LogInformation($"Retrieved {entities.Count} documents from collection: {collection}");
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving all documents from collection: {collection}");
                throw;
            }
        }

        public async Task<List<T>> GetByFieldAsync<T>(string collection, string field, object value) where T : class
        {
            try
            {
                var collectionRef = _firestoreDb.Collection(collection);
                var query = collectionRef.WhereEqualTo(field, value);
                var snapshot = await query.GetSnapshotAsync();
                
                var entities = snapshot.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
                _logger.LogInformation($"Retrieved {entities.Count} documents from collection: {collection} where {field} = {value}");
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error querying documents from collection: {collection} where {field} = {value}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync<T>(string collection, string id, T entity) where T : class
        {
            try
            {
                var documentRef = _firestoreDb.Collection(collection).Document(id);
                var snapshot = await documentRef.GetSnapshotAsync();
                
                if (!snapshot.Exists)
                {
                    _logger.LogWarning($"Document with ID: {id} not found in collection: {collection}");
                    return false;
                }

                // Set UpdatedAt if it exists
                SetTimestampProperties(entity, isUpdate: true);

                await documentRef.SetAsync(entity, SetOptions.MergeAll);
                _logger.LogInformation($"Updated document with ID: {id} in collection: {collection}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating document with ID: {id} in collection: {collection}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string collection, string id)
        {
            try
            {
                var documentRef = _firestoreDb.Collection(collection).Document(id);
                var snapshot = await documentRef.GetSnapshotAsync();
                
                if (!snapshot.Exists)
                {
                    _logger.LogWarning($"Document with ID: {id} not found in collection: {collection}");
                    return false;
                }

                await documentRef.DeleteAsync();
                _logger.LogInformation($"Deleted document with ID: {id} from collection: {collection}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting document with ID: {id} from collection: {collection}");
                throw;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string collection, Query query) where T : class
        {
            try
            {
                var snapshot = await query.GetSnapshotAsync();
                var entities = snapshot.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
                _logger.LogInformation($"Query returned {entities.Count} documents from collection: {collection}");
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing query on collection: {collection}");
                throw;
            }
        }

        private void SetTimestampProperties<T>(T entity, bool isUpdate) where T : class
        {
            var type = typeof(T);
            
            if (isUpdate)
            {
                var updatedAtProperty = type.GetProperty("UpdatedAt");
                if (updatedAtProperty != null && updatedAtProperty.PropertyType == typeof(DateTime))
                {
                    updatedAtProperty.SetValue(entity, DateTime.UtcNow);
                }
            }
            else
            {
                var createdAtProperty = type.GetProperty("CreatedAt");
                if (createdAtProperty != null && createdAtProperty.PropertyType == typeof(DateTime))
                {
                    createdAtProperty.SetValue(entity, DateTime.UtcNow);
                }
                
                var updatedAtProperty = type.GetProperty("UpdatedAt");
                if (updatedAtProperty != null && updatedAtProperty.PropertyType == typeof(DateTime))
                {
                    updatedAtProperty.SetValue(entity, DateTime.UtcNow);
                }
            }
        }
    }
}