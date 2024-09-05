using neon;

namespace neoNTests
{
    [TestClass]
    public class ComponentsTests
    {
        [TestMethod]
        public void SimpleAddTest()
        {
            if (Components.storage == null) new Components();
            if (Entities.instance == null) new Entities();

            List<EntityID> entities = new List<EntityID>(1000);

            for (int i = 0; i < 1000; i++)
            {
                EntityID entityID = Entities.GetID();
                entities.Add(entityID);

                Components.Add<Transform>(entityID);
            }

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsNotNull(Components.Get<Transform>(entities[i]));
            }
        }

        [TestMethod]
        public void ComplexAddTest()
        {
            if (Components.storage == null) new Components();
            if (Entities.instance == null) new Entities();

            List<EntityID> simpleEntities = new List<EntityID>(500);
            List<EntityID> complexEntities = new List<EntityID>(500);

            for (int i = 0; i < 1000; i++)
            {
                EntityID entityID = Entities.GetID();

                Components.Add<Transform>(entityID);

                Random random = new Random();
                if (random.Next(2) == 0)
                    simpleEntities.Add(entityID);
                else
                {
                    complexEntities.Add(entityID);
                    Components.Add<BoxCollider>(entityID);
                }
            }

            for (int i = 0; i < simpleEntities.Count; i++)
            {
                Assert.IsNotNull(Components.Get<Transform>(simpleEntities[i]));
                Assert.IsNull(Components.Get<BoxCollider>(simpleEntities[i]));
            }

            for (int i = 0; i < complexEntities.Count; i++)
            {
                Assert.IsNotNull(Components.Get<Transform>(complexEntities[i]));
                Assert.IsNotNull(Components.Get<BoxCollider>(complexEntities[i]));
            }
        }

        [TestMethod]
        public void ComplexAddRemoveTest()
        {
            if (Components.storage == null) new Components();
            if (Entities.instance == null) new Entities();

            List<EntityID> simpleEntities = new List<EntityID>(500);
            List<EntityID> complexEntities = new List<EntityID>(500);
            List<EntityID> voidEntities = new List<EntityID>(500);

            for (int i = 0; i < 1000; i++)
            {
                EntityID entityID = Entities.GetID();

                Components.Add<Transform>(entityID);

                Random random = new Random();
                if (random.Next(2) == 0)
                    simpleEntities.Add(entityID);
                else
                {
                    complexEntities.Add(entityID);
                    Components.Add<BoxCollider>(entityID);
                }
            }

            for (int i = simpleEntities.Count - 1; i >= 0; i--)
            {
                Random random = new Random();
                if (random.Next(2) == 0)
                {
                    EntityID id = simpleEntities[i];
                    voidEntities.Add(id);
                    Components.Remove<Transform>(id);
                }
                else
                {
                    EntityID id = simpleEntities[i];
                    complexEntities.Add(id);
                    Components.Add<BoxCollider>(id);
                    Components.Remove<Transform>(id);
                    Components.Add<Transform>(id);
                }
            }

            simpleEntities.Clear();

            for (int i = 0; i < voidEntities.Count; i++)
            {
                Assert.IsNull(Components.Get<Transform>(voidEntities[i]));
                Assert.IsNull(Components.Get<BoxCollider>(voidEntities[i]));
            }

            for (int i = 0; i < complexEntities.Count; i++)
            {
                Assert.IsNotNull(Components.Get<Transform>(complexEntities[i]));
                Assert.IsNotNull(Components.Get<BoxCollider>(complexEntities[i]));
            }
        }
    }
}