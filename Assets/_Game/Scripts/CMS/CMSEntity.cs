using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.CMS
{
    public class TestEntity_0 : CMSEntity
    {
        public TestEntity_0(string entityId) : base(entityId)
        {
            Define<TestComponent_3>();
            Define<TestComponent_1>();
        }
    }
    
    public class TestEntity_1 : CMSEntity
    {
        public TestEntity_1(string entityId) : base(entityId)
        {
            Define<TestComponent_2>();
            Define<TestComponent_3>();
        }
    }

    
    public class TestEntity_2 : CMSEntity
    {
        public TestEntity_2(string entityId) : base(entityId)
        {
            Define<TestComponent_0>();
            Define<TestComponent_1>();
        }
    }

    
    public class TestEntity_3 : CMSEntity
    {
        public TestEntity_3(string entityId) : base(entityId)
        {
            Define<TestComponent_0>();
            Define<TestComponent_1>();
            Define<TestComponent_2>();
            Define<TestComponent_3>();
        }
    }

    
    public class TestEntity_4 : CMSEntity
    {
        public TestEntity_4(string entityId) : base(entityId)
        {
            
        }
    }

    public class TestComponent_0 : CMSComponent
    {
        public bool IsTest;
    }
    
    public class TestComponent_1 : CMSComponent
    {
        public string TestText;
    }
    
    public class TestComponent_2 : CMSComponent
    {
        public Sprite TestSprite;
    }
    
    public class TestComponent_3 : CMSComponent
    {
        public Transform TestTransform;
    }


    public abstract class CMSEntity
    {
        public string EntityId { get; private set; }
        
        private readonly List<CMSComponent> _components;

        protected CMSEntity(string entityId)
        {
            EntityId = entityId;
            _components = new List<CMSComponent>();
        }
        
        public T Define<T>() where T : CMSComponent, new()
        {
            var component = Get<T>();
            if (component != null)
                return component;

            component = new T();
            _components.Add(component);
            
            return component;
        }

        public bool Is<T>(out T unknown) where T : CMSComponent, new()
        {
            unknown = Get<T>();
            return unknown != null;
        }

        public bool Is<T>() where T : CMSComponent, new()
        {
            return Get<T>() != null;
        }

        public bool Is(Type type)
        {
            return _components.Find(m => m.GetType() == type) != null;
        }

        public T Get<T>() where T : CMSComponent, new()
        {
            return _components.Find(m => m is T) as T;
        }

        public List<CMSComponent> GetAll()
        {
            return _components;
        }
    }
}