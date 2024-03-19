using Content.Shared.Roles;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.Jobs
{
    [UsedImplicitly]
    public sealed partial class AddComponentSpecial : JobSpecial
    {
        [DataField("components")]
        [AlwaysPushInheritance]
        public ComponentRegistry Components { get; private set; } = new();

        public override void BeforeEquip(EntityUid mob) // SPACE STORIES
        {
            // now its a registry of components, still throws i bet.
            // TODO: This is hot garbage and probably needs an engine change to not be a POS.
            var factory = IoCManager.Resolve<IComponentFactory>();
            var entityManager = IoCManager.Resolve<IEntityManager>();
            var serializationManager = IoCManager.Resolve<ISerializationManager>();

            foreach (var (name, data) in Components)
            {
                var component = (Component) factory.GetComponent(name);
                component.Owner = mob;

                var temp = (object) component;
                serializationManager.CopyTo(data.Component, ref temp);
                entityManager.RemoveComponent(mob, temp!.GetType());
                entityManager.AddComponent(mob, (Component) temp);
            }
        }
    }
}
