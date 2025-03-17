using System;

namespace GameObjects.Command
{
    public interface ICommand
    {
        bool IsCancelable { get; }

        public void Execute();
        public void Undo();
    }
}