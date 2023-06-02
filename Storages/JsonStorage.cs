using JsonConsole.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConsole.Storages
{
    public class JsonStorage<T> : IStorage<T> where T : ICloneable<T>, IHaveId
    {
        private readonly string Filepath;
        private readonly List<T> entities;
        public JsonStorage(string filepath)
        {
            Filepath = filepath;
            try
            {
                using (StreamReader reader = File.OpenText(Filepath))
                {
                    string json = reader.ReadToEnd();
                    entities = JsonConvert.DeserializeObject<List<T>>(json) ?? new();
                }
            }
            catch (FileNotFoundException ex)
            {
                File.Create(Filepath);
                entities = new();
            }
            catch
            {
                throw;
            }
        }

        public OperationResult<T> Add(T entity)
        {
            entity.Id = GetFreeId();
            entities.Add(entity);
            return OperationResult<T>.EmptyOk;
        }

        public OperationResult<T> Update(T entity)
        {
            T? entityToUpdate = entities.FirstOrDefault(x => x.Id == entity.Id);
            if (entityToUpdate == null)
                return new OperationResult<T>(false, EntityNotFound(entity.Id), default);

            int index = entities.IndexOf(entityToUpdate);
            entities[index] = entity;

            return OperationResult<T>.EmptyOk;
        }

        public OperationResult<T> Get(int id)
        {
            T? entity = entities.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                return new OperationResult<T>(false, EntityNotFound(id), default);

            return new OperationResult<T>(true, "Ok", entity);
        }

        public OperationResult<T> Delete(int id)
        {
            T? entity = entities.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                return new OperationResult<T>(false, EntityNotFound(id), default);

            entities.Remove(entity);

            return OperationResult<T>.EmptyOk;
        }

        public OperationResult<T> GetAll()
            => new OperationResult<T>(true, string.Empty, entities.Select(x => x.Clone()).ToArray());

        public OperationResult<T> Save()
        {
            Console.WriteLine(entities.Count);
            try
            {
                using (StreamWriter file = File.CreateText(Filepath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, entities);
                }
                return OperationResult<T>.EmptyOk;
            }
            catch (Exception ex)
            {
                return new OperationResult<T>(false, ex.Message, default);
            }

        }

        private int GetFreeId()
            => entities.Count > 0 ? entities.Max(x => x.Id) + 1 : 1;

        #region Messages
        private string EntityNotFound(int id)
            => $"Record with Id:{id} not found";
        #endregion
    }
}
