using System;
using System.Collections.Generic;
using Game.Runtime.Utils.Helpers;
using UnityEngine;

namespace Game.Runtime.CMS
{
    public static class CMS
    {
        private static bool _loaded;
        private static CMSTable<CMSEntity> _entitiesDatabase;
        
        public static void Load()
        {
            if (_loaded) return;

            AutoCacheEntities();
            _loaded = true;
        }
        
        public static void Unload()
        {
            _loaded = false;
            _entitiesDatabase = new CMSTable<CMSEntity>();
        }

        public static T GetEntity<T>() where T : CMSEntity
        {
            var entityId = CMSHelper.GetEntityId<T>();

            if (_entitiesDatabase.GetEntityOrDefault(entityId) is not T entity)
                throw new Exception($"[CMS] Unable to resolve entity id '{entityId}'");

            return entity;
        }
        
        public static T GetData<T>() where T : CMSComponent, new()
        {
            return GetEntity<CMSEntity>().GetComponent<T>();
        }

        public static List<T> GetAll<T>() where T : CMSEntity
        {
            var allSearch = new List<T>();

            foreach (var cmsEntity in _entitiesDatabase.GetAll())
            {
                if (cmsEntity is T entity)
                    allSearch.Add(entity);
            }

            return allSearch;
        }
        
        public static List<(CMSEntity entity, T component)> GetAllData<T>() where T : CMSComponent, new()
        {
            var filteredEntities = new List<(CMSEntity, T)>();
            
            foreach (var entity in _entitiesDatabase.GetAll())
            {
                if (entity.Is<T>(out var component))
                    filteredEntities.Add((entity, component));
            }

            return filteredEntities;
        }

        private static void AutoCacheEntities()
        {
            _entitiesDatabase = new CMSTable<CMSEntity>();

            var entities = Helpers.ReflectionHelper.FindAllSubsClasses<CMSEntity>();
            foreach (var entity in entities)
            {
                try
                {
                    CMSEntity entityInstance = Activator.CreateInstance(entity, entity.FullName) as CMSEntity;
                    _entitiesDatabase.Add(entityInstance);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"[CMS] Failed to initialize {entity.Name}: {exception.Message}");
                }
            }
            
            var entityPrefabs = Resources.LoadAll<CMSEntityPrefab>("CMS");
            foreach (var entityPrefab in entityPrefabs)
            {
                try
                {
                    var entityId = entityPrefab.GetEntityId();
                    Debug.Log("[CMS] Load entity " + entityId);
                    
                    var entity = new CMSEntity(entityId, entityPrefab.Components);
                    _entitiesDatabase.Add(entity);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"[CMS] Failed to initialize {entityPrefab.GetEntityId()}: {exception.Message}");
                }
              
            }
        }
    }
}

