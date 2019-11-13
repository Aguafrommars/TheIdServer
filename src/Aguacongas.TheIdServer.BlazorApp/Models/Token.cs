using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class Token : INotifyPropertyChanged
    {
        public const string RegulatExpression = @"(?<DaysTime>^\d+\.\d\d?:\d\d?:\d\d?$)|(?<Time>^\d\d?:\d\d?:\d\d?$)|(?<MinutesSecondes>^\d\d?:\d\d?$)|(?<Days>^\d+d$)|(?<Hours>^\d+h$)|(?<Minutes>^\d+m$)|(?<Secondes>^\d+s?$)";
        public event PropertyChangedEventHandler PropertyChanged;

        private string _valueString;
        [RegularExpression(RegulatExpression, ErrorMessage = "The token expression doesn't match a valid format. You can use the forms d.hh:mm:ss, hh.mm:ss, mm:ss, a number of days (365d), a number of hours (12h), a number of minutes (30m), a number of second")]
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
