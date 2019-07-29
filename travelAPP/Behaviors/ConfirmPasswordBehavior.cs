using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using travelAPP.Validators.Contracts;
using Xamarin.Forms;

namespace travelAPP.Behaviors
{
    class ConfirmPasswordBehavior : Behavior<Entry>
    {
        IErrorStyle _style = new Validators.Implementations.BasicErrorStyle();
        View _view;
        public string PropertyName { get; set; }
        public string oldpassword { get; set; }
        public ObservableCollection<IValidator> Validators { get; set; } = new ObservableCollection<IValidator>();
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(ConfirmPasswordBehavior), false);
        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public static readonly BindableProperty CompareToEntryProperty = BindableProperty.Create("CompareToEntry", typeof(Entry), typeof(ConfirmPasswordBehavior), null);

        public bool Validate()
        {
            bool isValid = true;
            string errorMessage = "";

            foreach (IValidator validator in Validators)
            {
                bool result = validator.CheckAsync(_view.GetType()
                                       .GetProperty(PropertyName)
                                       .GetValue(_view) as string);
                isValid = isValid && result;

                if (!result)
                {
                    errorMessage = validator.Message;
                    break;
                }
            }

            if (!isValid)
            {
                _style.ShowError(_view, errorMessage);

                return false;
            }
            else
            {
                _style.RemoveError(_view);

                return true;
            }

        }
        public Entry CompareToEntry
        {
            get { return (Entry)base.GetValue(CompareToEntryProperty); }
            set { base.SetValue(CompareToEntryProperty, value); }
        }
        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }
        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            var password = CompareToEntry.Text;
            oldpassword = CompareToEntry.Text;
            var confirmPassword = e.NewTextValue;
            IsValid = password.Equals(confirmPassword);
            ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
        }
    }
}
