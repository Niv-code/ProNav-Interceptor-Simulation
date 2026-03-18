using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Input;

namespace ProNavSimulator
{
    public class MainViewModel : INotifyPropertyChanged
    {
        [DllImport("ProNavEngine.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double CalculateNewHeading(
            double targetX, double targetY,
            double missileX, double missileY,
            double currentHeading, double navConstant,
            double previousLosAngle, out double outNewLosAngle);

        private DispatcherTimer _timer;
        
        private double _navConstant = 4.0;
        public double NavigationConstant { get => _navConstant; set { _navConstant = value; OnPropertyChanged(); } }

        private double _targetManeuver = 5.0;
        public double TargetManeuverIntensity { get => _targetManeuver; set { _targetManeuver = value; OnPropertyChanged(); } }

        public double TargetX { get; private set; }
        public double TargetY { get; private set; }
        public double MissileX { get; private set; }
        public double MissileY { get; private set; }
        public double MissileHeadingDegrees { get; private set; }

        private double _missileHeadingRads;
        private double _previousLosAngle = 0;
        private double _targetTime = 0;
        private bool _isFirstTick = true;

        private const double MissileSpeed = 12.0;
        private const double TargetSpeed = 6.0;

        private string _status = "STANDBY";
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        private string _distanceText = "Distance: ---";
        public string DistanceText { get => _distanceText; set { _distanceText = value; OnPropertyChanged(); } }

        public ICommand ResetCommand { get; }

        public MainViewModel()
        {
            ResetCommand = new RelayCommand(ResetSimulation);
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; 
            _timer.Tick += UpdateSimulation;
            ResetSimulation();
        }

        private void ResetSimulation(object obj = null)
        {
            _timer.Stop();
            TargetX = 50; TargetY = 300; _targetTime = 0;
            MissileX = 850; MissileY = 550;
            _missileHeadingRads = Math.PI; 
            MissileHeadingDegrees = _missileHeadingRads * (180.0 / Math.PI);
            _isFirstTick = true;
            Status = "MISSILE LAUNCHED!";
            _timer.Start();
        }
        
        private void UpdateSimulation(object sender, EventArgs e)
        {
            _targetTime += 0.1;
            TargetX += TargetSpeed;
            TargetY += Math.Sin(_targetTime) * TargetManeuverIntensity;

            
            double distance = Math.Sqrt(Math.Pow(TargetX - MissileX, 2) + Math.Pow(TargetY - MissileY, 2));
            DistanceText = $"Distance: {Math.Round(distance)} m";
            OnPropertyChanged(nameof(DistanceText));

            if (distance < 15) { Status = "TARGET DESTROYED!"; _timer.Stop(); return; }
            if (TargetX > 1000 || MissileY < 0 || TargetY < 0 || TargetY > 650) { Status = "MISSED (OUT OF BOUNDS)"; _timer.Stop(); return; }

            
            if (!_isFirstTick)
            {
                _missileHeadingRads = CalculateNewHeading(
                    TargetX, TargetY,
                    MissileX, MissileY,
                    _missileHeadingRads, NavigationConstant,
                    _previousLosAngle, out _previousLosAngle);
            }
            else
            {
                _previousLosAngle = Math.Atan2(TargetY - MissileY, TargetX - MissileX);
                _isFirstTick = false;
            }
            
            MissileX += MissileSpeed * Math.Cos(_missileHeadingRads);
            MissileY += MissileSpeed * Math.Sin(_missileHeadingRads);
            MissileHeadingDegrees = _missileHeadingRads * (180.0 / Math.PI);

            OnPropertyChanged(nameof(TargetX)); OnPropertyChanged(nameof(TargetY));
            OnPropertyChanged(nameof(MissileX)); OnPropertyChanged(nameof(MissileY));
            OnPropertyChanged(nameof(MissileHeadingDegrees));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        public RelayCommand(Action<object> execute) { _execute = execute; }
        public event EventHandler CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter);
    }
}