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

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CopyResultClick(object sender, RoutedEventArgs e) {
            Clipboard.SetText(Result.Text);
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void AddToHistoryClick(object sender, RoutedEventArgs e) {
            var l1 = new PhaseCurrent(L1_Current.Text, L1_Angle.Text);
            var l2 = new PhaseCurrent(L2_Current.Text, L2_Angle.Text);
            var l3 = new PhaseCurrent(L3_Current.Text, L3_Angle.Text);
            var n = Result.Text + 'A';
            var historyItem = new CalcHistoryItem(l1, l2, l3, n);
            ViewModel.History.Add(historyItem);
        }

        private void CopyHistoryClick(object sender, RoutedEventArgs e) {
            var sb = new StringBuilder(ViewModel.History.Count * 100);
            foreach (var item in ViewModel.History) {
                sb.AppendLine(item.ToString());
            }
            Clipboard.SetText(sb.ToString());
        }

        private void ClearHistoryClick(object sender, RoutedEventArgs e) {
            ViewModel.History.Clear();
        }
    }
}