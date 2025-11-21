using System;

namespace LineHelper.Models
{
    public class MarkerSession
    {
        private int _currentLineNumber = 1;
        private int _currentLoutNumber = 1;
        private int _markerRange = 50;

        public int CurrentLineNumber
        {
            get => _currentLineNumber;
            set
            {
                if (value >= 1 && value <= MarkerRange)
                    _currentLineNumber = value;
            }
        }

        public int CurrentLoutNumber
        {
            get => _currentLoutNumber;
            set
            {
                if (value >= 1 && value <= MarkerRange)
                    _currentLoutNumber = value;
            }
        }

        public int MarkerRange
        {
            get => _markerRange;
            set
            {
                if (value > 0)
                    _markerRange = value;
            }
        }

        public string GetLineMarker() => $"line{CurrentLineNumber:D4}";
        public string GetLoutMarker() => $"lout{CurrentLoutNumber:D4}";

        public string IncrementLine()
        {
            var marker = GetLineMarker();
            if (CurrentLineNumber < MarkerRange)
                CurrentLineNumber++;
            return marker;
        }

        public string IncrementLout()
        {
            var marker = GetLoutMarker();
            if (CurrentLoutNumber < MarkerRange)
                CurrentLoutNumber++;
            return marker;
        }

        public void DecrementLine()
        {
            if (CurrentLineNumber > 1)
                CurrentLineNumber--;
        }

        public void DecrementLout()
        {
            if (CurrentLoutNumber > 1)
                CurrentLoutNumber--;
        }

        public void Reset()
        {
            CurrentLineNumber = 1;
            CurrentLoutNumber = 1;
        }

        public bool IsLineAtLimit() => CurrentLineNumber >= MarkerRange;
        public bool IsLoutAtLimit() => CurrentLoutNumber >= MarkerRange;
    }
}