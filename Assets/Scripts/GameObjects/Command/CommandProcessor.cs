using System;

namespace GameObjects.Command
{
    public static class GlobalCommandProcessor
    {
        static Action[] _globalUndoActions = new Action[100];

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
        static int _commandStackSize = 20;
        static T[] _commands = new T[_commandStackSize];

        static int _addIndex = 0;
        static int _removeIndex = 0;

        public static int CommandStackSize
        {
            get => _commandStackSize;
            set
            {
                if (_commandStackSize == value) return;

                _commandStackSize = value;
                _commands = new T[value];
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
            _addIndex = (_addIndex + 1) % _commands.Length;

            if (_addIndex == _removeIndex)
            {
                _removeIndex = (_removeIndex + 1) % _commands.Length;
            }
        }

        public static void UndoLastCommand()
        {
            if (_commands[_removeIndex] == null) return;

            _commands[_removeIndex]?.Undo();
            _removeIndex = (_removeIndex + 1) % _commandStackSize;
        }

        public static void RemoveCommand(T command)
        {
            for (var i = 0; i < _commandStackSize; i++)
            {
                var index = (_removeIndex + i) % _commandStackSize;

                if (_commands[index].Equals(command))
                {
                    _commands[index] = default;
                    break;
                }
            }
        }
    }
}