using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Ink_Canvas.Helpers
{
    public class TimeMachine
    {
        private readonly List<TimeMachineHistory> _currentStrokeHistory = new List<TimeMachineHistory>();

        private int _currentIndex = -1;

        public delegate void OnUndoStateChange(bool status);

        public event OnUndoStateChange OnUndoStateChanged;

        public delegate void OnRedoStateChange(bool status);

        public event OnRedoStateChange OnRedoStateChanged;

        private void CheckHistoryIndex()
        {
            if (_currentIndex + 1 < _currentStrokeHistory.Count)
            {
                _currentStrokeHistory.RemoveRange(_currentIndex + 1, (_currentStrokeHistory.Count - 1) - _currentIndex);
            }
        }

        public void CommitStrokeUserInputHistory(StrokeCollection stroke)
        {
            _currentStrokeHistory.Add(new TimeMachineHistory(stroke, TimeMachineHistoryType.UserInput, false));
            _currentIndex = _currentStrokeHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitStrokeShapeHistory(StrokeCollection strokeToBeReplaced, StrokeCollection generatedStroke)
        {
            CheckHistoryIndex();
            _currentStrokeHistory.Add(new TimeMachineHistory(generatedStroke, TimeMachineHistoryType.ShapeRecognition, false, strokeToBeReplaced));
            _currentIndex = _currentStrokeHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitStrokeManipulationHistory(
            Dictionary<Stroke, Tuple<StylusPointCollection, StylusPointCollection>> stylusPointDictionary,
            Dictionary<string, Tuple<TransformGroup, TransformGroup>> ElementsManipulationHistory)
        {
            CheckHistoryIndex();
            _currentStrokeHistory.Add(new TimeMachineHistory(stylusPointDictionary, ElementsManipulationHistory, TimeMachineHistoryType.Manipulation));
            _currentIndex = _currentStrokeHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitStrokeDrawingAttributesHistory(Dictionary<Stroke, Tuple<DrawingAttributes, DrawingAttributes>> drawingAttributes)
        {
            CheckHistoryIndex();
            _currentStrokeHistory.Add(new TimeMachineHistory(drawingAttributes, TimeMachineHistoryType.DrawingAttributes));
            _currentIndex = _currentStrokeHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitStrokeEraseHistory(StrokeCollection stroke, StrokeCollection sourceStroke = null)
        {
            CheckHistoryIndex();
            _currentStrokeHistory.Add(new TimeMachineHistory(stroke, TimeMachineHistoryType.Clear, true, sourceStroke));
            _currentIndex = _currentStrokeHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitImageInsertHistory(UIElement image, bool strokeHasBeenCleared = false)
        {
            CheckHistoryIndex();
            _currentStrokeHistory.Add(new TimeMachineHistory(image, TimeMachineHistoryType.ImageInsert, strokeHasBeenCleared));
            _currentIndex = _currentStrokeHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void ClearStrokeHistory()
        {
            _currentStrokeHistory.Clear();
            _currentIndex = -1;
            NotifyUndoRedoState();
        }

        public TimeMachineHistory Undo()
        {
            var item = _currentStrokeHistory[_currentIndex];
            item.StrokeHasBeenCleared = !item.StrokeHasBeenCleared;
            _currentIndex--;
            NotifyUndoRedoState();
            return item;
        }

        public TimeMachineHistory Redo()
        {
            var item = _currentStrokeHistory[++_currentIndex];
            item.StrokeHasBeenCleared = !item.StrokeHasBeenCleared;
            NotifyUndoRedoState();
            return item;
        }

        public TimeMachineHistory[] ExportTimeMachineHistory()
        {
            CheckHistoryIndex();
            return _currentStrokeHistory.ToArray();
        }

        public bool ImportTimeMachineHistory(TimeMachineHistory[] sourceHistory)
        {
            _currentStrokeHistory.Clear();
            _currentStrokeHistory.AddRange(sourceHistory);
            _currentIndex = _currentStrokeHistory.Count - 1;
            NotifyUndoRedoState();
            return true;
        }

        private void NotifyUndoRedoState()
        {
            OnUndoStateChanged?.Invoke(_currentIndex > -1);
            OnRedoStateChanged?.Invoke(_currentIndex < _currentStrokeHistory.Count - 1);
        }
    }

    public class TimeMachineHistory
    {
        public TimeMachineHistoryType CommitType;
        public bool StrokeHasBeenCleared = false;
        public StrokeCollection CurrentStroke;
        public StrokeCollection ReplacedStroke;
        public UIElement ImageElement;
        //这里说一下 Tuple 的 Value1 是初始值 ; Value 2 是改变值
        public Dictionary<Stroke, Tuple<StylusPointCollection, StylusPointCollection>> StylusPointDictionary;
        public Dictionary<string, Tuple<TransformGroup, TransformGroup>> ElementsManipulationHistory;
        public Dictionary<Stroke, Tuple<DrawingAttributes, DrawingAttributes>> DrawingAttributes;
        // UserInput
        public TimeMachineHistory(StrokeCollection currentStroke, TimeMachineHistoryType commitType, bool strokeHasBeenCleared)
        {
            CommitType = commitType;
            CurrentStroke = currentStroke;
            StrokeHasBeenCleared = strokeHasBeenCleared;
            ReplacedStroke = null;
        }
        // Clear
        public TimeMachineHistory(StrokeCollection currentStroke, TimeMachineHistoryType commitType, bool strokeHasBeenCleared, StrokeCollection replacedStroke)
        {
            CommitType = commitType;
            CurrentStroke = currentStroke;
            StrokeHasBeenCleared = strokeHasBeenCleared;
            ReplacedStroke = replacedStroke;
        }
        // StrokeManipulation, ElementManipulation
        public TimeMachineHistory(
            Dictionary<Stroke, Tuple<StylusPointCollection, StylusPointCollection>> stylusPointDictionary,
            Dictionary<string, Tuple<TransformGroup, TransformGroup>> elementsManipulationHistory,
            TimeMachineHistoryType commitType)
        {
            CommitType = commitType;
            ElementsManipulationHistory = elementsManipulationHistory;
            StylusPointDictionary = stylusPointDictionary;
        }
        // trokeDrawingAttributes
        public TimeMachineHistory(Dictionary<Stroke, Tuple<DrawingAttributes, DrawingAttributes>> drawingAttributes, TimeMachineHistoryType commitType)
        {
            CommitType = commitType;
            DrawingAttributes = drawingAttributes;
        }
        // Insert Image
        public TimeMachineHistory(UIElement imageElement, TimeMachineHistoryType commitType, bool strokeHasBeenCleared)
        {
            CommitType = commitType;
            ImageElement = imageElement;
            StrokeHasBeenCleared = strokeHasBeenCleared;
        }
    }

    public enum TimeMachineHistoryType
    {
        UserInput,
        ShapeRecognition,
        Clear,
        Manipulation,
        DrawingAttributes,
        ImageInsert
    }
}