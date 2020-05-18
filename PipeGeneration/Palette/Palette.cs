using System;
using System.Collections.Generic;
using AcadApp = Bricscad.ApplicationServices.Application;
using Bricscad.ApplicationServices;
using Bricscad.Windows;
using Application = Bricscad.ApplicationServices.Application;
using System.Drawing;
using System.Windows.Controls;

namespace PipeGeneration.Palette
{
    public class SettingPalette : PaletteSet
    {
        bool initialized = false;
        public SettingPalette(string showCommand)
            : base(showCommand)
        {
            Init();
        }

        public SettingPalette(string showCommand, Guid guid)
            : base(showCommand, guid)
        {
            Init();
        }

        // Common construction operations.  
        // If you overload the constructor in a derived type, 
        // your overload must either super message one of the 
        // above constructors, or must call this method:

        protected void Init()
        {
            if (!initialized)
            {
                initialized = true;
                this.StateChanged += stateChanged;
            }
        }

        // Event to be fired on close.

        // Note:
        //
        // If you derive your PaletteSet class from
        // this class, then you should override the 
        // OnPaletteSetClosed() virtual method below,
        // rather than handling this event. This event
        // is primarily intended to be used from the
        // outside, rather than from a derived type.

        public event EventHandler PaletteSetClosed;

        // flag indicating if Application.Idle is being handled

        bool idleHandled = false;

        // Flag indicating if the PaletteSet was rolled up when
        // the StateChanged event fired:

        bool wasRolledUp = false;

        void stateChanged(object sender, PaletteSetStateEventArgs e)
        {
            try
            {
                if (!idleHandled && e.NewState == StateEventIndex.Hide)
                {
                    idleHandled = true;
                    if (base.Handle != IntPtr.Zero)
                        this.wasRolledUp = base.RolledUp;
                    AcadApp.Idle += onIdle;
                }
                else if (idleHandled && e.NewState == StateEventIndex.Show)
                {
                    idleHandled = false;
                    AcadApp.Idle -= onIdle;
                }
            }
            catch (System.Exception ex)
            {


            }

        }


        // If the palette set is docked/unlocked or rolled-up,
        // the idle event should not fire until the entire 
        // operation is completed, allowing it to be used to 
        // detect if the paletteSet is no longer visible:

        // [TT] 08-12-12:
        // Revised to deal with auto-hide, which was being
        // mis-interpreted as closing the palette set.

        void onIdle(object sender, EventArgs e)
        {

            Application.Idle -= onIdle;
            idleHandled = false;


            if (!(base.Visible || (wasRolledUp ^ this.RolledUp)))
            {
                this.OnPaletteSetClosed();
            }
        }

        // If you derive your PaletteSet class from this class (recommended),
        // your derived class can override this method rather than handing 
        // the PaletteSetClosed event:

        protected virtual void OnPaletteSetClosed()
        {
            if (this.PaletteSetClosed != null)
                this.PaletteSetClosed(this, EventArgs.Empty);
        }

        // hong_nx: 09/20/2018: Flag for Palette show to fix in BRC
        public static bool PaletteShow = false;
        public static bool PaletteReShow = false;
    }

    public class SettingPaletteSingleton
    {
        static private SettingPalette instance;

        static public Dictionary<int, UserControl> Tabs { get; set; }
        static public bool IsCreateNew = false;
        public static SettingPalette Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = new SettingPalette("Setting");

                instance.Size = new Size(600, 300);
                instance.Dock = DockSides.Left;
                instance.Style = PaletteSetStyles.Snappable |
                    PaletteSetStyles.ShowCloseButton |
                    PaletteSetStyles.ShowAutoHideButton |
                    PaletteSetStyles.ShowPropertiesMenu;
                IsCreateNew = true;

                Tabs = new Dictionary<int, UserControl>();
                //               }

                return instance;
            }
        }
        public static UserControl getTabAt(int key)
        {
            try
            {
                if (Tabs != null)
                {
                    foreach (var pair in Tabs)
                    {
                        if (pair.Key == key)
                            return pair.Value;
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
            return null;

        }
        public static void addTab(int key, UserControl userControl)
        {
            try
            {
                if (Tabs != null)
                {
                    Tabs.Add(key, userControl);
                }
            }
            catch (System.Exception ex)
            {

            }
        }
    }
}
