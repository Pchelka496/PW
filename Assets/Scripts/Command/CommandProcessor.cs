using System;
using GameObjects.Command;

namespace _Project.Scripts.Game.Command
{
    public static class GlobalCommandProcessor
    {
        static Action[] _globalUndoActions = new Action[20];

        static int _addIndex = 0;
        static int _removeIndex = 0;

        public static void RegisterCommand(Action command)
        {
            _globalUndoActions[_addIndex] = command;
            _addIndex = (_addIndex + 1) % _globalUndoActions.Length;

            if (_addIndex == _removeIndex)
            {
                _removeIndex = (_removeIndex + 1) % _globalUndoActions.Length;
            }
        }

        public static void UndoLastCommand()
        {
            if (_globalUndoActions[_removeIndex] == null) return;

            _globalUndoActions[_removeIndex]?.Invoke();
            _removeIndex = (_removeIndex + 1) % _globalUndoActions.Length;
        }
    }

    public static class CommandProcessor<T> where T : ICommand
    {
        static int _commandStackSize = 5;
        static T[] _commands = new T[_commandStackSize];

        static int _addIndex = 0;  
        static int _count = 0;       

        public static int CommandStackSize
        {
            get => _commandStackSize;
            set
            {
                if (_commandStackSize == value) return;

                _commandStackSize = value;
                _commands = new T[value];
                _addIndex = 0;
                _count = 0;
            }
        }

        public static void ExecuteCommand(T command)
        {
            command.Execute();

            if (!command.IsCancelable) return;

            AddCommandToUndoStack(command);
            GlobalCommandProcessor.RegisterCommand(UndoLastCommand);
        }

        private static void AddCommandToUndoStack(in T command)
        {
            _commands[_addIndex] = command;
            _addIndex = (_addIndex + 1) % _commandStackSize;

            if (_count < _commandStackSize)
            {
                _count++;
            }
        }

        public static void UndoLastCommand()
        {
            if (_count == 0) return;

            _addIndex = (_addIndex - 1 + _commandStackSize) % _commandStackSize;
        
            _commands[_addIndex]?.Undo();
            _commands[_addIndex] = default;
        
            _count--;
        }
    }

}