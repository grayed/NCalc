// Copyright 2025 Vadim Zhukov <persgray@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom
// the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR
// THE USE OR OTHER DEALINGS IN THE SOFTWARE.using System.Globalization;

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NCalc;

internal class MainWindowViewModel : INotifyDataErrorInfo, INotifyPropertyChanged
{
    private string  l1_CurrentText = "0",  l2_CurrentText = "0", l3_CurrentText = "0";
    private string  l1_AngleText = "0",  l2_AngleText = "120", l3_AngleText = "240";
    
    private bool l1_CurrentIsValid = true, l2_CurrentIsValid = true, l3_CurrentIsValid = true;
    private bool l1_AngleIsValid = true, l2_AngleIsValid = true, l3_AngleIsValid = true;
    
    private string resultText = "0";
    private bool hasErrors;

    // Do not use helpers that add

    public string L1_Current {
        get => l1_CurrentText;
        set {
            if (l1_CurrentText != value) {
                l1_CurrentText = value;
                OnPropertyChanged();
                UpdateResult();
            }
        }
    }
    public string L2_Current {
        get => l2_CurrentText;
        set {
            if (l2_CurrentText != value) {
                l2_CurrentText = value;
                OnPropertyChanged();
                UpdateResult();
            }
        }
    }
    public string L3_Current {
        get => l3_CurrentText;
        set {
            if (l3_CurrentText != value) {
                l3_CurrentText = value;
                OnPropertyChanged();
                UpdateResult();
            }
        }
    }

    public string L1_Angle {
        get => l1_AngleText;
        set {
            if (l1_AngleText != value) {
                l1_AngleText = value;
                OnPropertyChanged();
                UpdateResult();
            }
        }
    }
    public string L2_Angle {
        get => l2_AngleText;
        set {
            if (l2_AngleText != value) {
                l2_AngleText = value;
                OnPropertyChanged();
                UpdateResult();
            }
        }
    }
    public string L3_Angle {
        get => l3_AngleText;
        set {
            if (l3_AngleText != value) {
                l3_AngleText = value;
                OnPropertyChanged();
                UpdateResult();
            }
        }
    }

    public string ResultText {
        get => resultText;
        private set {
            if (resultText != value) {
                resultText = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<CalcHistoryItem> History { get; } = new ObservableCollection<CalcHistoryItem>();

    private const NumberStyles numberStyle = NumberStyles.AllowDecimalPoint|NumberStyles.AllowLeadingWhite|NumberStyles.AllowThousands|NumberStyles.AllowLeadingSign;
    private void UpdateResult() {
        void parseIt(string text, out double value, ref bool isValid, string propName) {
            bool newIsValid;
            if (string.IsNullOrWhiteSpace(text)) {
                value = 0;
                newIsValid = true;
            } else {
                newIsValid = double.TryParse(text, numberStyle, CultureInfo.CurrentCulture, out value) ||
                             double.TryParse(text, numberStyle, CultureInfo.InvariantCulture, out value);
            }
            if (isValid != newIsValid)
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propName));
            isValid = newIsValid;
        }

        parseIt(L1_Current, out double l1_current, ref l1_CurrentIsValid, nameof(L1_Current));
        parseIt(L2_Current, out double l2_current, ref l2_CurrentIsValid, nameof(L2_Current));
        parseIt(L3_Current, out double l3_current, ref l3_CurrentIsValid, nameof(L3_Current));
        parseIt(L1_Angle, out double l1_angle, ref l1_AngleIsValid, nameof(L1_Angle));
        parseIt(L2_Angle, out double l2_angle, ref l2_AngleIsValid, nameof(L2_Angle));
        parseIt(L3_Angle, out double l3_angle, ref l3_AngleIsValid, nameof(L3_Angle));

        HasErrors = !l1_CurrentIsValid || !l2_CurrentIsValid || !l3_CurrentIsValid || !l1_AngleIsValid || !l2_AngleIsValid || !l3_AngleIsValid;
        if (HasErrors) {
            ResultText = "";
            return;
        }

        l1_angle = l1_angle / 180 * Math.PI;
        l2_angle = l2_angle / 180 * Math.PI;
        l3_angle = l3_angle / 180 * Math.PI;

        var n = l1_current * Math.Cos(l1_angle) + l2_current * Math.Cos(l2_angle) + l3_current * Math.Cos(l3_angle) +
            Complex.ImaginaryOne * (l1_current * Math.Sin(l1_angle) + l2_current * Math.Sin(l2_angle) + l3_current * Math.Sin(l3_angle));

        ResultText = Math.Round(n.Magnitude, 3).ToString();
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public IEnumerable GetErrors(string? propertyName) {
        switch (propertyName) {
        case nameof(L1_Current):
            if (!l1_CurrentIsValid)
                yield return "Неверное значение";
            break;
        case nameof(L2_Current):
            if (!l2_CurrentIsValid)
                yield return "Неверное значение";
            break;
        case nameof(L3_Current):
            if (!l3_CurrentIsValid)
                yield return "Неверное значение";
            break;

        case nameof(L1_Angle):
            if (!l1_AngleIsValid)
                yield return "Неверное значение";
            break;
        case nameof(L2_Angle):
            if (!l2_AngleIsValid)
                yield return "Неверное значение";
            break;
        case nameof(L3_Angle):
            if (!l3_AngleIsValid)
                yield return "Неверное значение";
            break;
        }
    }

    public bool HasErrors {
        get => hasErrors;
        private set {
            if (hasErrors != value) {
                hasErrors = value;
                OnPropertyChanged();
            }
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public event PropertyChangedEventHandler? PropertyChanged;
}
