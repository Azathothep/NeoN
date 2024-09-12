using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace neon
{
    // The component storage architecture has been created following Sander Mertens' (creator or FLECS) "Building an ECS" article series
    // You can read the first one here : https://ajmmertens.medium.com/building-an-ecs-1-where-are-my-entities-and-components-63d07c7da742

    public class ComponentStorage : IComponentStorage
    {
        public class ComponentIteratorProvider : IComponentIteratorProvider
        {
            private Dictionary<ComponentID, HashSet<IComponentIterator>> m_IterablesCollection = new();

            private ComponentStorage m_Storage;

            public ComponentIteratorProvider(ComponentStorage componentStorage)
            {
                m_Storage = componentStorage;
                m_Storage.m_OnArchetypeAdded += SetDirty;
            }

            public void SetDirty(ComponentSet componentSet)
            {
                for (int i = 0; i < componentSet.ComponentIDs.Count; i++)
                {
                    ComponentID componentID = componentSet.ComponentIDs[i];
                    if (m_IterablesCollection.TryGetValue(componentID, out HashSet<IComponentIterator> iterableSet))
                    {
                        foreach (var iterable in iterableSet)
                            iterable.SetDirty();
                    }
                }
            }

            public IComponentIterator Get<T>(IQueryFilter[] filters, QueryType queryType) where T : class, IComponent
            {
                IComponentIterator iterableQuery = new ComponentIterator<T>(() => RequestArchetypes(filters));
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2>(IQueryFilter[] filters, QueryType queryType) where T1 : class, IComponent where T2 : class, IComponent
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2>(() => RequestArchetypes(filters));
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2, T3>(IQueryFilter[] filters, QueryType queryType) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2, T3>(() => RequestArchetypes(filters));
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2, T3, T4>(IQueryFilter[] filters, QueryType queryType) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2, T3, T4>(() => RequestArchetypes(filters));
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(filters, iterableQuery);
                return iterableQuery;
            }

            private void AddIterableToCollection(IQueryFilter[] filters, IComponentIterator iterableQuery)
            {
                for (int i = 0; i < filters.Length; i++)
                {
                    if (!m_IterablesCollection.TryGetValue(filters[i].ComponentID, out HashSet<IComponentIterator> iterableSet))
                    {
                        iterableSet = new HashSet<IComponentIterator>();
                        m_IterablesCollection.Add(filters[i].ComponentID, iterableSet);
                    }

                    iterableSet.Add(iterableQuery);
                }
            }

            private (Archetype, List<EntityID>)[] RequestArchetypes(IQueryFilter[] queryFilters)
            {
                List<(Archetype, List<EntityID>)> archetypes = new();

                List<ArchetypeID> archetypesLeft = new();

                int filterIndex = 0;

                if (queryFilters[0].Term == FilterTerm.Has)
                {
                    if (m_Storage.m_ComponentIDToArchetypeSet.TryGetValue(queryFilters[0].ComponentID, out Dictionary<ArchetypeID, int> satisfyingArchetypes))
                    {
                        foreach (var a in satisfyingArchetypes)
                            archetypesLeft.Add(a.Key);
                    }

                    filterIndex = 1;
                }
                else
                {
                    foreach (var a in m_Storage.m_ArchetypeIDToArchetype)
                        archetypesLeft.Add(a.Key);
                }

                // Remove all that have not

                for (; filterIndex < queryFilters.Length; filterIndex++)
                {
                    if (queryFilters[filterIndex].Term != FilterTerm.Has)
                        break;

                    if (m_Storage.m_ComponentIDToArchetypeSet.TryGetValue(queryFilters[filterIndex].ComponentID, out Dictionary<ArchetypeID, int> satisfyingArchetypes))
                    {
                        for (int j = archetypesLeft.Count - 1; j >= 0; j--)
                        {
                            if (satisfyingArchetypes.ContainsKey(archetypesLeft[j]) == false)
                                archetypesLeft.RemoveAt(j);
                        }
                    }
                }

                // Remove all that have

                for (; filterIndex < queryFilters.Length; filterIndex++)
                {
                    if (queryFilters[filterIndex].Term != FilterTerm.HasNot)
                        break;

                    if (m_Storage.m_ComponentIDToArchetypeSet.TryGetValue(queryFilters[filterIndex].ComponentID, out Dictionary<ArchetypeID, int> satisfyingArchetypes))
                    {
                        for (int j = archetypesLeft.Count - 1; j >= 0; j--)
                        {
                            if (satisfyingArchetypes.ContainsKey(archetypesLeft[j]))
                                archetypesLeft.RemoveAt(j);
                        }
                    }
                }

                for (int j = 0; j < archetypesLeft.Count; j++)
                {
                    Archetype archetype = m_Storage.m_ArchetypeIDToArchetype[archetypesLeft[j]];
                    List<EntityID> entities = m_Storage.m_ArchetypeToEntities[archetype];

                    archetypes.Add((archetype, entities));
                }

                return archetypes.ToArray();
            }
        }

        private Dictionary<EntityID, (Archetype, int)> m_EntityToArchetype = new();

        private Dictionary<Archetype, List<EntityID>> m_ArchetypeToEntities = new();

        private Dictionary<ComponentSet, Archetype> m_ComponentSetToArchetype = new();

        private Dictionary<ComponentID, Dictionary<ArchetypeID, int>> m_ComponentIDToArchetypeSet = new();

        private Dictionary<ArchetypeID, Archetype> m_ArchetypeIDToArchetype = new();

        private event Action<ComponentSet> m_OnArchetypeAdded;

        private ComponentStorageNotifier m_ComponentStorageNotifier;

        private IComponentIteratorProvider m_IteratorProvider;
        public IComponentIteratorProvider IteratorProvider => m_IteratorProvider;

        public ComponentStorage(ComponentStorageNotifier notifier)
        {
            m_ComponentStorageNotifier = notifier;

            m_IteratorProvider = new ComponentIteratorProvider(this);
        }

        public T? Get<T>(EntityID entityID) where T : class, IComponent
        {
            ComponentID componentID = Components.GetID<T>();

            // Getting entity's archetype & row index
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
            {
                // The entity doesn't have any component yet
                //Debug.WriteLine($"Error: Archetype for entity {entityID} not found in dictionary");
                return null;
            }

            (Archetype archetype, int row) = archetypeRecord;

            int column = GetColumn(componentID, archetype);

            if (column == -1)
            {
                // Debug.WriteLine("Something Happened");
                return null;
            }

            return (T)archetype.Columns[column][row];
        }

        public bool Has<T>(EntityID entityID) where T : class, IComponent
        {
            ComponentID componentID = Components.GetID<T>();

            // Getting entity's archetype & row index
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
            {
                Debug.WriteLine($"Error: Archetype for entity {entityID} not found in dictionary");
                return false;
            }

            (Archetype archetype, int _) = archetypeRecord;

            // Getting every archetypeID that contains the required component & the component position in their column
            if (!m_ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
            {
                Debug.WriteLine($"Error: ArchetypeSet for component {componentID} not found in dictionary");
                return false;
            }

            // if entity's archetype ID is in archetypeID set, entity has the component
            return archetypeSet.ContainsKey(archetype.ID);
        }

        public T? Add<T>(EntityID entityID, T component) where T : class, IComponent
        {
            component.ID = Entities.GetID();

            ComponentID componentID = Components.GetID<T>();

            // If the entity isn't recorded yet for having any component
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
            {
                ComponentSet componentSet = new ComponentSet(componentID);

                Archetype archetype = GetOrCreateArchetype(componentSet);

                AddEntityToArchetype(entityID, new List<IComponent> { component }, archetype);

                return component;
            }

            {
                Archetype archetype = archetypeRecord.Item1;
                int row = archetypeRecord.Item2;

                {
                    if (archetype.ComponentSet.ComponentIDs.Contains(componentID))
                    {
                        // Already has this component

                        int column = GetColumn(componentID, archetype);
                        archetype.Columns[column][row] = component; // Replace ? Or leave unchecked ?
                        return component;
                    }
                }

                {
                    if (archetype == null)
                        throw new ArgumentException($"Archetype for {componentID} is null");

                    if (!archetype.Edges.TryGetValue(componentID, out ArchetypeEdges archetypeEdges))
                    {
                        archetypeEdges = new ArchetypeEdges();
                    }

                    Archetype nextArchetype = archetypeEdges.Add;

                    // If Edges.Add hasn't been set yet
                    if (nextArchetype == null)
                    {
                        ComponentSet newComponentSet = archetype.ComponentSet.Add(componentID);

                        nextArchetype = GetOrCreateArchetype(newComponentSet);

                        archetypeEdges.Add = nextArchetype;
                        archetype.Edges.Add(componentID, archetypeEdges);
                    }

                    // Removing components from archetype, add it to the list, then to the nextArchetype & save result

                    List<IComponent> components = RemoveEntityFromArchetype(entityID, archetype, row);

                    int column = GetColumn(componentID, nextArchetype);

                    if (column == -1)
                    {
                        // Something weird happened
                        return null;
                    }

                    components.Insert(column, component); // This should stay in order, because we're inserting it at its right location

                    AddEntityToArchetype(entityID, components, nextArchetype);
                }
            }

            return component;
        }

        public void Remove<T>(EntityID entityID) where T : class, IComponent
        {
            ComponentID componentID = Components.GetID<T>();

            // If the entity isn't recorded yet for having any component
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
                return;

            (Archetype archetype, int row) = archetypeRecord;

            if (archetype == null)
                throw new ArgumentException($"Archetype for {componentID} is null");

            // if archetype doesn't have this component
            if (!archetype.ComponentSet.ComponentIDs.Contains(componentID))
                return;

            // if archetype contains only 1 (this) component
            if (archetype.ComponentSet.ComponentIDs.Count == 1)
            {
                RemoveEntityFromArchetype(entityID, archetype, row);
                m_EntityToArchetype.Remove(entityID);
                return;
            }

            if (!archetype.Edges.TryGetValue(componentID, out ArchetypeEdges archetypeEdges))
            {
                archetypeEdges = new ArchetypeEdges();
            }

            Archetype nextArchetype = archetypeEdges.Remove;

            // If Edges.Remove hasn't been set yet
            if (nextArchetype == null)
            {
                ComponentSet newComponentSet = archetype.ComponentSet.Remove(componentID);

                nextArchetype = GetOrCreateArchetype(newComponentSet);

                archetypeEdges.Remove = nextArchetype;
                archetype.Edges.Add(componentID, archetypeEdges);
            }

            // Removing component from archetype & list, then adding it to nextArchetype & save result

            List<IComponent> components = RemoveEntityFromArchetype(entityID, archetype, row);

            int column = GetColumn(componentID, archetype);

            components.RemoveAt(column); // This will remove the component set at column

            AddEntityToArchetype(entityID, components, nextArchetype);
        }

        public void Remove(EntityID entityID)
        {
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) value))
                return;

            Archetype archetype = value.Item1;
            int row = value.Item2;

            List<IComponent> components = RemoveEntityFromArchetype(entityID, archetype, row);

            for (int i = 0; i < components.Count; i++)
            {
                Entities.Destroy(components[i].ID);
            }
        }

        public object[] GetComponentsInternal(EntityID entityID, ComponentID[] componentIDs)
        {
            object[] components = new object[componentIDs.Length];

            (Archetype, int) archetypeRecord = m_EntityToArchetype[entityID];

            Archetype archetype = archetypeRecord.Item1;
            int row = archetypeRecord.Item2;

            for (int i = 0; i < componentIDs.Length; i++)
            {
                Dictionary<ArchetypeID, int> componentArchetypes = m_ComponentIDToArchetypeSet[componentIDs[i]];

                if (componentArchetypes.TryGetValue(archetype.ID, out int column))
                    components[i] = archetype.Columns[column][row];
            }

            return components;
        }

        private int GetColumn(ComponentID componentID, Archetype archetype)
        {
            // Getting every archetypeID that contains the required component & the component position in their column
            if (!m_ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
                return -1;

            // Getting entity's archetype column for required component
            // (if false, archetype - consequently, the entity - does not posess the required component)
            if (!archetypeSet.TryGetValue(archetype.ID, out int column))
                return -1;

            return column;
        }

        private Archetype GetOrCreateArchetype(ComponentSet componentSet)
        {
            // if there is no archetype registered for this ComponentSet
            if (!m_ComponentSetToArchetype.TryGetValue(componentSet, out Archetype archetype))
            {
                archetype = new Archetype(componentSet);
                m_ComponentSetToArchetype.Add(componentSet, archetype);
                m_ArchetypeIDToArchetype.Add(archetype.ID, archetype);

                m_OnArchetypeAdded?.Invoke(componentSet);

                // foreach componentID in componentSet, add lookup data to the componenID-to-archetype-infos dictionary
                for (int i = 0; i < componentSet.ComponentIDs.Count; i++)
                {
                    ComponentID componentID = componentSet.ComponentIDs[i];

                    // if archetypeSet not yet exists (= first time this componentID is brought to it), create a new one

                    if (!m_ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
                    {
                        archetypeSet = new Dictionary<ArchetypeID, int>();
                        m_ComponentIDToArchetypeSet.Add(componentID, archetypeSet);
                    }

                    archetypeSet.Add(archetype.ID, i);
                }
            }

            return archetype;
        }

        private void AddEntityToArchetype(EntityID entityID, List<IComponent> components, Archetype archetype)
        {
            int row = archetype.AddEntity(components);

            m_EntityToArchetype[entityID] = (archetype, row);

            if (!m_ArchetypeToEntities.TryGetValue(archetype, out List<EntityID> entities))
            {
                entities = new List<EntityID>();
                m_ArchetypeToEntities.Add(archetype, entities);
            }

            entities.Add(entityID);

            foreach (var cID in archetype.ComponentSet.ComponentIDs)
            {
                m_ComponentStorageNotifier.Raise(cID);
            }
        }

        private List<IComponent> RemoveEntityFromArchetype(EntityID entityID, Archetype archetype, int row)
        {
            List<IComponent> components = archetype.RemoveEntity(row);

            m_ArchetypeToEntities[archetype].Remove(entityID);

            List<EntityID> entities = m_ArchetypeToEntities[archetype];
            for (int i = row; i < entities.Count; i++)
            {
                (Archetype a, int r) = m_EntityToArchetype[entities[i]];
                m_EntityToArchetype[entities[i]] = (a, r - 1);
            }

            foreach (var cID in archetype.ComponentSet.ComponentIDs)
            {
                m_ComponentStorageNotifier.Raise(cID);
            }

            return components;
        }

    }
}
