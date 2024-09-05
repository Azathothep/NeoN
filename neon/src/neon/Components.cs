using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;

namespace neon
{
    public partial class Components
    {
        private class ComponentsStorage {
            public Dictionary<EntityID, (Archetype, int)> EntityToArchetype = new();

            public Dictionary<Archetype, List<EntityID>> ArchetypeToEntities = new();

            public Dictionary<ComponentSet, Archetype> ComponentSetToArchetype = new();

            public Dictionary<ComponentID, Dictionary<ArchetypeID, int>> ComponentIDToArchetypeSet = new();
        }

        private static ComponentsStorage storage = new();

        private Components() { }

        public static bool Has<T>(EntityID entityID) where T : class, IComponent
        {
            ComponentID componentID = Components.GetID<T>();

            // Getting entity's archetype & row index
            if (!storage.EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
            {
                Debug.WriteLine($"Error: Archetype for entity {entityID} not found in dictionary");
                return false;
            }

            (Archetype archetype, int _) = archetypeRecord;

            // Getting every archetypeID that contains the required component & the component position in their column
            if (!storage.ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
            {
                Debug.WriteLine($"Error: ArchetypeSet for component {componentID} not found in dictionary");
                return false;
            }

            // if entity's archetype ID is in archetypeID set, entity has the component
            return archetypeSet.ContainsKey(archetype.ID);
        }

        public static T? Get<T>(EntityID entityID) where T : class, IComponent => GetInternal<T>(entityID);

        public static bool TryGet<T>(EntityID entityID, out T? result) where T : class, IComponent
        {
            result = GetInternal<T>(entityID);
            return result != null;
        }

        private static T? GetInternal<T>(EntityID entityID) where T : class, IComponent
        {
            ComponentID componentID = Components.GetID<T>();

            // Getting entity's archetype & row index
            if (!storage.EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
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

		public static T? Add<T>(EntityID entityID) where T : class, IComponent, new() => AddInternal(entityID, new T());

        public static T? Add<T>(EntityID entityID, T inputComponent) where T : class, IComponent => AddInternal(entityID, (T)inputComponent.Clone());

        private static T? AddInternal<T>(EntityID entityID, T component) where T : class, IComponent
        {
            ComponentID componentID = Components.GetID<T>();

            // If the entity isn't recorded yet for having any component
            if (!storage.EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
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

        public static void Remove<T>(EntityID entityID) where T : class, IComponent => RemoveInternal<T>(entityID);

        private static void RemoveInternal<T>(EntityID entityID) where T : class, IComponent
        {
            ComponentID componentID = Components.GetID<T>();

            // If the entity isn't recorded yet for having any component
            if (!storage.EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
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
                storage.EntityToArchetype.Remove(entityID);
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

        public static void RemoveAll(EntityID entityID)
        {
            if (!storage.EntityToArchetype.TryGetValue(entityID, out (Archetype, int) value))
                return;

            Archetype archetype = value.Item1;
            int row = value.Item2;

            RemoveEntityFromArchetype(entityID, archetype, row);
        }

        private static int GetColumn(ComponentID componentID, Archetype archetype)
        {
            // Getting every archetypeID that contains the required component & the component position in their column
            if (!storage.ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
                return -1;

            // Getting entity's archetype column for required component
            // (if false, archetype - consequently, the entity - does not posess the required component)
            if (!archetypeSet.TryGetValue(archetype.ID, out int column))
                return -1;

            return column;
        }

        private static Archetype GetOrCreateArchetype(ComponentSet componentSet)
        {
            // if there is no archetype registered for this ComponentSet
            if (!storage.ComponentSetToArchetype.TryGetValue(componentSet, out Archetype archetype))
            {
                archetype = new Archetype(componentSet);
                storage.ComponentSetToArchetype.Add(componentSet, archetype);

                // foreach componentID in componentSet, add lookup data to the componenID-to-archetype-infos dictionary
                for (int i = 0; i < componentSet.ComponentIDs.Count; i++)
                {
                    ComponentID componentID = componentSet.ComponentIDs[i];

                    // if archetypeSet not yet exists (= first time this componentID is brought to it), create a new one
                    if (!storage.ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
                    {
                        archetypeSet = new Dictionary<ArchetypeID, int>();
                        storage.ComponentIDToArchetypeSet.Add(componentID, archetypeSet);
                    }

                    archetypeSet.Add(archetype.ID, i);
                }
            }

            return archetype;
        }

        private static void AddEntityToArchetype(EntityID entityID, List<IComponent> components, Archetype archetype)
        {
            int row = archetype.AddEntity(components);

            storage.EntityToArchetype[entityID] = (archetype, row);

            if (!storage.ArchetypeToEntities.TryGetValue(archetype, out List<EntityID> entities)) {
                entities = new List<EntityID>();
                storage.ArchetypeToEntities.Add(archetype, entities);
            }

            entities.Add(entityID);
        }

        private static List<IComponent> RemoveEntityFromArchetype(EntityID entityID, Archetype archetype, int row)
        {
            List<IComponent> components = archetype.RemoveEntity(row);

            storage.ArchetypeToEntities[archetype].Remove(entityID);

            List<EntityID> entities = storage.ArchetypeToEntities[archetype];
            for (int i = row; i < entities.Count; i++)
            {
                (Archetype a, int r) = storage.EntityToArchetype[entities[i]];
                storage.EntityToArchetype[entities[i]] = (a, r - 1);
            }

            return components;
        }
    }
}
