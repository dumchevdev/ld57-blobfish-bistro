using System;
using System.Collections.Generic;
using _Game.Utils;

namespace _Game.CMS
{
    public static class CMS
    {
        private static bool _loaded;
        private static CMSTable<CMSEntity> _entitiesDatabase;
    
        public static void Load()
        {
            if (_loaded) return;

            _entitiesDatabase = new CMSTable<CMSEntity>();
            
            var entities = ReflectionUtil.FindAllSubsClasses<CMSEntity>();
            foreach (var entity in entities)
            {
                try
                {
                    CMSEntity entityInstance = Activator.CreateInstance(entity, entity.FullName) as CMSEntity;
                    _entitiesDatabase.Add(entityInstance);
                }
                catch (Exception exception)
                {
                    UnityEngine.Debug.LogError($"[CMS] Failed to initialize {entity.Name}: {exception.Message}");
                }
            }
            
            _loaded = true;
        }
        
        public static void Unload()
        {
            _loaded = false;
            _entitiesDatabase = new CMSTable<CMSEntity>();
        }

        public static T Get<T>() where T : CMSEntity
        {
            var entityId = CMSHelper.GetEntityId<T>();

            if (_entitiesDatabase.GetEntityOrDefault(entityId) is not T entity)
                throw new Exception($"[CMS] Unable to resolve entity id '{entityId}'");

            return entity;
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
    }
}

