using System;
using JetBrains.Annotations;
using R3;

namespace GameObjects.Construct
{
    public class ConstructRegistry
    {
        [NotNull]
        public ReactiveProperty<Construct> CurrentPlayerConstruct { get; private set; } = new(new Construct());

        public void SetCurrentPlayerConstruct(Construct construct)
        {
            if (construct == null) throw new ArgumentNullException(nameof(construct));

            CurrentPlayerConstruct.Value = construct;
        }
    }
}