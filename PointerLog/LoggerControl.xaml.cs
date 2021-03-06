﻿using System.Collections.Generic;using Windows.UI.Text;using Windows.UI.Xaml;using Windows.UI.Xaml.Controls;using Windows.UI.Xaml.Input;using MetroLog;
using System;namespace RaptTouchLog{    public sealed partial class LoggerControl : UserControl    {        Logger logger = LogManager.GetLogger("Rapt");
        long lActiveEvents = 0;         class PointerInfo        {            public StackPanel stackPanel;            public string repeatEvent;
            public DateTime prevEventTime;            public TextBlock repeatTextBlock;        };        Dictionary<uint, PointerInfo> pointerDictionary = new Dictionary<uint, PointerInfo>();        public LoggerControl()        {            this.InitializeComponent();            logger.Trace("Application Starting....");
            lActiveEvents = 0;
            TraceLevelLogging = true;        }


        public bool TraceLevelLogging { set; get; }        public bool CaptureOnPress { set; get; }        protected override void OnPointerEntered(PointerRoutedEventArgs args)        {
            Log("Entered", args);
            lActiveEvents++;
            base.OnPointerEntered(args);        }        protected override void OnPointerPressed(PointerRoutedEventArgs args)        {            if (this.CaptureOnPress)                CapturePointer(args.Pointer);            Log("Pressed", args);            base.OnPointerPressed(args);        }        protected override void OnPointerMoved(PointerRoutedEventArgs args)        {            Log("Moved", args);            base.OnPointerMoved(args);        }        protected override void OnPointerReleased(PointerRoutedEventArgs args)        {            Log("Released", args);            base.OnPointerReleased(args);        }        protected override void OnPointerExited(PointerRoutedEventArgs args)        {
            lActiveEvents--;            Log("Exited", args);            base.OnPointerExited(args);        }        protected override void OnPointerCaptureLost(PointerRoutedEventArgs args)        {
            lActiveEvents--;            Log("CaptureLost", args);            base.OnPointerCaptureLost(args);        }        protected override void OnPointerCanceled(PointerRoutedEventArgs args)        {
            lActiveEvents--;            Log("Canceled", args);            base.OnPointerCanceled(args);        }        protected override void OnPointerWheelChanged(PointerRoutedEventArgs args)        {            Log("WheelChanged", args);            base.OnPointerWheelChanged(args);        }        void Log(string eventName, PointerRoutedEventArgs args)        {            uint id = args.Pointer.PointerId;            PointerInfo pointerInfo;            string event_string = "";            string header_string = "";
            DateTime eventTime = DateTime.Now;

            if (lActiveEvents <= 0) {
                Clear();            }            if (pointerDictionary.ContainsKey(id))            {                pointerInfo = pointerDictionary[id];            }            else            {                // New ID, so new StackPanel and header                TextBlock header = new TextBlock                {                    Text = args.Pointer.PointerId + " - " + args.Pointer.PointerDeviceType,                    FontWeight = FontWeights.Bold                };                StackPanel stackPanel = new StackPanel();                stackPanel.Children.Add(header);                // New PointerInfo for dictionary                pointerInfo = new PointerInfo                {                    stackPanel = stackPanel,
                    prevEventTime = eventTime                };                pointerDictionary.Add(id, pointerInfo);                // New column in the Grid for the StackPanel                ColumnDefinition coldef = new ColumnDefinition                {                    Width = new GridLength(1, GridUnitType.Star)                };                contentGrid.ColumnDefinitions.Add(coldef);                Grid.SetColumn(stackPanel, contentGrid.ColumnDefinitions.Count - 1);                contentGrid.Children.Add(stackPanel);            }

            double timeDifferenceMilliseconds = (eventTime - pointerInfo.prevEventTime).TotalMilliseconds;
            pointerInfo.prevEventTime = eventTime;            header_string = id + " - " + args.Pointer.PointerId + " - " + args.Pointer.PointerDeviceType;            // Don't repeat PointerMoved and PointerWheelChanged events            TextBlock txtblk = null;            if (eventName == pointerInfo.repeatEvent)            {                txtblk = pointerInfo.repeatTextBlock;            }            else            {                txtblk = new TextBlock();                pointerInfo.stackPanel.Children.Add(txtblk);            }            event_string = eventName + " ";            if (eventName == "WheelChanged")            {                event_string += args.GetCurrentPoint(this).Properties.MouseWheelDelta;            }            else            {                event_string += args.GetCurrentPoint(this).Position;            }            event_string += args.Pointer.IsInContact ? " C" : "";            event_string += args.Pointer.IsInRange ? " R" : "";

            event_string += " D: " + timeDifferenceMilliseconds;
                        txtblk.Text = event_string;            if (eventName == "Moved" || eventName == "WheelChanged")            {                pointerInfo.repeatEvent = eventName;                pointerInfo.repeatTextBlock = txtblk;            }            else            {                pointerInfo.repeatEvent = null;                pointerInfo.repeatTextBlock = null;            }
            // OK lets add this to the logger in the background.
            if (TraceLevelLogging)
            {
                logger.Trace(header_string + event_string);
            }
            else
            {
                logger.Error(header_string + event_string);
            }        }        public void Clear()        {
            lActiveEvents = 0;            contentGrid.ColumnDefinitions.Clear();            contentGrid.Children.Clear();            pointerDictionary.Clear();        }    }}