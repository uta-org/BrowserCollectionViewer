using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BrowserCollectionViewer
{
    public static class F
    {
        //public static ToolTipHandler<T> DefineToolTip<T>(this T instance, string text, string title = "", int autoPopDelay = 5000, int initialDelay = 0, int reshowDelay = 1000, bool showAlways = true, bool isBalloon = true, ToolTipIcon icon = default)
        //    where T : Control
        //{
        //    if (icon == default)
        //        icon = ToolTipIcon.Info;

        //    var toolTip = new ToolTip
        //    {
        //        //AutoPopDelay = autoPopDelay,
        //        InitialDelay = initialDelay,
        //        //ReshowDelay = reshowDelay,
        //        ShowAlways = showAlways,
        //        IsBalloon = isBalloon,
        //        //ToolTipIcon = icon,
        //        //ToolTipTitle = title,
        //        Active = true
        //    };

        //    toolTip.SetToolTip(instance, text);

        //    return new ToolTipHandler<T>(instance, text, toolTip);
        //}

        /*
         *
         * ctrl.MouseHover += new EventHandler(delegate(Object o, EventArgs a)
            {
                var btn = (Control)o;
                ToolTip1.SetToolTip(btn, btn.Tag.ToString());
            });
         *
         */

        public static void ShowToolTip<T>(this T control, string text)
            where T : Control
        {
            var toolTip = CreateToolTip();

            control.On("MouseHover", () =>
            {
                toolTip.SetToolTip(control, text);
            });
        }

        public static ToolTip CreateToolTip()
        {
            return CreateToolTip(string.Empty);
        }

        public static ToolTip CreateToolTip(string title, ToolTipIcon icon = default)
        {
            if (icon == default)
                return new ToolTip { ShowAlways = true, IsBalloon = true, InitialDelay = 0, };

            return new ToolTip { ShowAlways = true, IsBalloon = true, InitialDelay = 0, ToolTipIcon = icon, ToolTipTitle = title };
        }

        public static IWin32Window GetWindowFromHandle<T>(this T control)
            where T : Control
        {
            var nativeWindow = new NativeWindow();
            nativeWindow.AssignHandle(control.Handle);

            return nativeWindow;
        }

        public static void On<T>(this T control, string @event, Action callback)
            where T : Control
        {
            var eventInfo = typeof(T).GetEvent(@event);

            //var handler =
            //    Delegate.CreateDelegate(
            //        eventInfo.EventHandlerType,
            //        control,
            //        callback.Method);

            Delegate @delegate = (EventHandler)((sender, e) => callback());
            eventInfo.AddEventHandler(control, @delegate);
            //eventInfo.AddEventHandler(p, (Func<TPredicate>)PredicateDelegate);
        }

        public static void On<TControl, T>(this TControl control, T val, Action callback)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Not an enum");

            var @enum = (Enum)(object)val;

            string name = typeof(T).Name.Replace("Events", string.Empty);
            string @event = @enum.ToString();

            var type = TypeConverter<Control>.FromString(name);

            if (type.Name != name)
                throw new InvalidOperationException();

            var eventInfo = typeof(T).GetEvent(@event);

            var handler =
                Delegate.CreateDelegate(
                    eventInfo.EventHandlerType,
                    control,
                    callback.Method);

            eventInfo.AddEventHandler(control, handler);
        }

        public static void On<TDelegate>(this Control control, Enum @enum, TDelegate callback)
            where TDelegate : Delegate
        {
            var enumType = @enum.GetType();

            if (!enumType.IsEnum)
                throw new ArgumentException("Not an enum");

            string name = enumType.Name.Replace("Events", string.Empty);
            string @event = @enum.ToString();

            var type = TypeConverter<Control>.FromString(name);

            if (type.Name != name)
                throw new InvalidOperationException();

            var eventInfo = control.GetType().GetEvent(@event);

            //var handler =
            //    Delegate.CreateDelegate(
            //        eventInfo.EventHandlerType,
            //        control,
            //        callback.Method);

            eventInfo.AddEventHandler(control, callback);
        }

        public static bool IsValidURL(this string URL)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return Rgx.IsMatch(URL);
        }

        public static bool IsAbsoluteUrl(this string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri result);
        }

        public static void RemoveAll(this IList list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.RemoveAt(i);
            }
        }

        public static void ToggleAll(this CheckedListBox instance, bool value)
        {
            for (int i = 0; i < instance.Items.Count; i++)
            {
                instance.SetItemChecked(i, value);
            }
        }

        //public static void On<TElement, TEventArgs>(
        //    TElement element,
        //    Action<EventHandler<TEventArgs>> subscription)
        //    where TElement : Control

        //    where TEventArgs : EventArgs
        //{
        //    var control = (Control)element;
        //}
    }
}