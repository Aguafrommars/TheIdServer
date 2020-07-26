using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class GridState
    {
        private string _sortedProperty;
        private string _sortedDirection;
        public event Func<SortEventArgs, Task> OnHeaderClicked;
        public event Func<bool, Task> OnSelectAllClicked;

        public bool AllSelected { get; set; }

        public string GetHeaderArrowClassSuffix(string propertyName)
        {
            if (_sortedProperty == propertyName)
            {
                return _sortedDirection;
            }
            return null;
        }

        public void HeaderClicked(string propertyName)
        {
            string orderBy;

            if (_sortedProperty != propertyName || _sortedDirection == null)
            {
                _sortedDirection = "bottom";
                orderBy = $"{propertyName} desc";
            }
            else if (_sortedDirection == "bottom")
            {
                _sortedDirection = "top";
                orderBy = propertyName;
            }
            else
            {
                _sortedDirection = null;
                orderBy = null;
            }

            _sortedProperty = propertyName;

            OnHeaderClicked?.Invoke(new SortEventArgs
            {
                OrderBy = orderBy
            });
        }

        public void SelectAllClicked(bool value)
        {
            OnSelectAllClicked?.Invoke(value);
        }
    }
}
