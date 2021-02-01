// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class AuthorizedToken<T>
    {
        private const string DISPLAY_FORMAT = @"d\.hh\:mm\:ss";
        private readonly Regex _regex = new Regex(Token.RegulatExpression, RegexOptions.Compiled);
        private Token _token;
        private bool _updatingValue;

        [Parameter]
        public T Value { get; set; }

        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        [Parameter]
        public EventCallback TokenValueChanged { get; set; }

        [Parameter]
        public IDictionary<string, TimeSpan?> QuickValues { get; set; }

        [Parameter]
        public string Name { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            if (Value != null)
            {
                var timeSpan = TimeSpan.FromSeconds(Convert.ToInt32(Value));
                _token = new Token
                {
                    ValueString = timeSpan.ToString(DISPLAY_FORMAT)
                };
            }
            else
            {
                _token = new Token();
            }

            _token.PropertyChanged += (s, e) =>
            {
                if (_updatingValue)
                {
                    return;
                }

                var match = _regex.Match(_token.ValueString);
                if (!match.Success)
                {
                    return;
                }

                var groups = match.Groups;
                if (groups["DaysTime"].Success || groups["Time"].Success)
                {
                    var newTimeSpan = TimeSpan.Parse(_token.ValueString);
                    SetValue(newTimeSpan);
                }
                else if (groups["MinutesSecondes"].Success)
                {
                    var newTimeSpan = TimeSpan.Parse($"00:{_token.ValueString}");
                    SetValue(newTimeSpan);
                }
                else if (groups["Days"].Success)
                {
                    var newTimeSpan = TimeSpan.FromDays(int.Parse(_token.ValueString[0..^1]));
                    SetValue(newTimeSpan);
                }
                else if (groups["Hours"].Success)
                {
                    var newTimeSpan = TimeSpan.FromHours(int.Parse(_token.ValueString[0..^1]));
                    SetValue(newTimeSpan);
                }
                else if (groups["Minutes"].Success)
                {
                    var newTimeSpan = TimeSpan.FromMinutes(int.Parse(_token.ValueString[0..^1]));
                    SetValue(newTimeSpan);
                }
                else if (_token.ValueString.EndsWith("s"))
                {
                    var newTimeSpan = TimeSpan.FromSeconds(int.Parse(_token.ValueString[0..^1]));
                    SetValue(newTimeSpan);
                }
                else
                {
                    var newTimeSpan = TimeSpan.FromSeconds(int.Parse(_token.ValueString));
                    SetValue(newTimeSpan);
                }
            };
            base.OnInitialized();
        }


        private void SetValue(TimeSpan? time)
        {
            _updatingValue = true;

            if (!time.HasValue)
            {
                Value = default;
                ValueChanged.InvokeAsync(Value);
                TokenValueChanged.InvokeAsync(Value);
                _token.ValueString = null;
            }
            else
            {
                var timeValue = time.Value;
                var type = typeof(T);
                if (type.IsGenericType)
                {
                    type = type.GetGenericArguments()[0];
                }
                Value = (T)Convert.ChangeType(timeValue.TotalSeconds, type);
                ValueChanged.InvokeAsync(Value);
                TokenValueChanged.InvokeAsync(Value);
                _token.ValueString = timeValue.ToString(DISPLAY_FORMAT);
            }

            StateHasChanged();
            _updatingValue = false;
        }
    }
}
