// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System.ComponentModel;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class Token : INotifyPropertyChanged
    {
        public const string RegulatExpression = @"(?<DaysTime>^\d+\.\d\d?:\d\d?:\d\d?$)|(?<Time>^\d\d?:\d\d?:\d\d?$)|(?<MinutesSecondes>^\d\d?:\d\d?$)|(?<Days>^\d+d$)|(?<Hours>^\d+h$)|(?<Minutes>^\d+m$)|(?<Secondes>^\d+s?$)";
        public event PropertyChangedEventHandler PropertyChanged;

        private string _valueString;
        public string ValueString 
        {
            get { return _valueString; }
            set
            {
                _valueString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueString)));
            }
        }
    }
}
