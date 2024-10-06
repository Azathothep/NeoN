using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Reflection;

namespace neon
{
    public static class Entities
    {
        private static IEntityStorage storage;

        public static void SetStorage(IEntityStorage storage)
        {
            Entities.storage = storage;
        }

        public static EntityID GetID(bool isComponent = false) => storage.GetID(isComponent);

        public static void Destroy(EntityID entityID) => storage.Destroy(entityID);

        public static void SetRelation(EntityID parentID, EntityID childID) => storage.SetRelation(parentID, childID);

        public static EntityID[] GetRoots() => storage.GetRoots();

        public static EntityID GetParent(EntityID entityID) => storage.GetParent(entityID);

        public static EntityID[] GetChildren(EntityID entityID, bool includeComponents = true) => storage.GetChildren(entityID, includeComponents);

        public static void UpdateState(EntityID entityID) => storage.UpdateState(entityID);

        private struct CopyArchitect
        {
            public Dictionary<EntityID, EntityID> ModelToCopy = new();
            public Dictionary<Type, MemberInfo[]> DependencyMembers = new();
            public Dictionary<IComponent, EntityID[]> Dependencies = new();

            public CopyArchitect() { }
        }

        public static EntityID Copy(EntityID entityModel)
        {
            CopyArchitect copyArchitect = new();

            EntityID entityCopy = DeepCopy(entityModel, copyArchitect);

            copyArchitect.ModelToCopy.Add(entityModel, entityCopy);

            entityCopy.SetParent(entityModel.GetParent());

            entityCopy.active = entityModel.active;

            ResolveDependencies(copyArchitect);

            return entityCopy;
        }

        private static EntityID DeepCopy(EntityID entityModel, CopyArchitect copyArchitect)
        {
            EntityID entityCopy = Entities.GetID();

            copyArchitect.ModelToCopy.Add(entityModel, entityCopy);

            EntityID[] childrenModels = entityModel.GetChildren();

            foreach (var childModel in childrenModels)
            {
                EntityID childCopy = DeepCopy(childModel, copyArchitect);
                childCopy.SetParent(entityCopy);
            }

            entityCopy.active = entityModel.active;

            IComponent[] componentModels = entityModel.GetAll();

            foreach (var componentModel in componentModels)
            {
                IComponent componentCopy = Components.Add(entityCopy, componentModel, componentModel.GetType());

                copyArchitect.ModelToCopy.Add(componentModel.EntityID, componentCopy.EntityID);

                RegisterDependencies(componentCopy, copyArchitect);

                componentCopy.EntityID.active = componentModel.EntityID.active;
            }

            return entityCopy;
        }

        private static void RegisterDependencies(IComponent component, CopyArchitect copyArchitect)
        {
            Type componentType = component.GetType();
            MemberInfo[] memberDependencies;

            if (!copyArchitect.DependencyMembers.TryGetValue(componentType, out memberDependencies))
            {
                memberDependencies = GetTypeDependencies(componentType);
                copyArchitect.DependencyMembers.Add(componentType, memberDependencies);
            }

            if (memberDependencies.Length > 0)
            {
                EntityID[] entityDependencies = new EntityID[memberDependencies.Length];

                for (int i = 0; i < memberDependencies.Length; i++)
                {
                    if (memberDependencies[i] is FieldInfo field)
                    {
                        if (field.FieldType == typeof(EntityID))
                            entityDependencies[i] = (EntityID)field.GetValue(component);
                        else if (typeof(IComponent).IsAssignableFrom(field.FieldType))
                            entityDependencies[i] = ((IComponent)field.GetValue(component)).EntityID;
                    }
                    else if (memberDependencies[i] is PropertyInfo property)
                    {
                        if (property.PropertyType == typeof(EntityID))
                            entityDependencies[i] = (EntityID)property.GetValue(component);
                        else if (typeof(IComponent).IsAssignableFrom(property.PropertyType))
                            entityDependencies[i] = ((IComponent)property.GetValue(component)).EntityID;
                    }
                }

                copyArchitect.Dependencies.Add(component, entityDependencies);
            }
        }

        private static MemberInfo[] GetTypeDependencies(Type type)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            MemberInfo[] fieldDependencies = type.GetFields(flags).Where(d => {
                Type fieldType = d.FieldType;
                return fieldType == typeof(EntityID) || typeof(IComponent).IsAssignableFrom(fieldType);
            }).ToArray();

            MemberInfo[] propertyDependencies = type.GetProperties(flags).Where(d => {
                Type propertyType = d.PropertyType;
                return d.CanWrite && propertyType == typeof(EntityID) || typeof(IComponent).IsAssignableFrom(propertyType);
            }).ToArray();

            return fieldDependencies.Concat(propertyDependencies).ToArray();
        }

        private static void ResolveDependencies(CopyArchitect copyArchitect)
        {
            foreach (var dependency in copyArchitect.Dependencies)
            {
                ResolveObjectDependency(dependency.Key, dependency.Value, copyArchitect);
            }
        }

        private static void ResolveObjectDependency(IComponent component, EntityID[] references, CopyArchitect copyArchitect)
        {
            Type componentType = component.GetType();

            MemberInfo[] memberInfos = copyArchitect.DependencyMembers[componentType];

            for (int i = 0; i < memberInfos.Length; i++)
            {
                if (!copyArchitect.ModelToCopy.TryGetValue(references[i], out EntityID copyReference))
                    continue;

                Type memberType = memberInfos[i] is FieldInfo fieldInfo ? fieldInfo.FieldType : ((PropertyInfo)memberInfos[i]).PropertyType;

                if (memberType == typeof(EntityID))
                    ResolveDependency(memberInfos[i], component, copyReference);
                else if (typeof(IComponent).IsAssignableFrom(memberType))
                {
                    IComponent componentToAssign = copyReference.GetParent().GetComponentOfEntityID(copyReference);

                    ResolveDependency(memberInfos[i], component, componentToAssign);
                }
            }
        }

        private static void ResolveDependency(MemberInfo memberInfo, IComponent component, object copyReference)
        {
            if (memberInfo is FieldInfo field)
                field.SetValue(component, copyReference);
            else if (memberInfo is PropertyInfo property)
                property.SetValue(component, copyReference);
        }
    }
}
