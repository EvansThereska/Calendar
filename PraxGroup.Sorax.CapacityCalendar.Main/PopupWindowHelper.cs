using System;
using System.Drawing;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public delegate void PopupCancelEventHandler(object sender, PopupCancelEventArgs e);

    public delegate void PopupClosedEventHandler(object sender, PopupClosedEventArgs e);


    public class PopupWindowHelper : NativeWindow
    {
        private Form _owner;
        private Form _popup;

        private EventHandler _closedHandler;

        private readonly PopupWindowHelperMessageFilter _filter;

        private bool _isShowing;

        public event PopupClosedEventHandler PopupClosed;

        public event PopupCancelEventHandler PopupCancel;

        
        public PopupWindowHelper()
        {
            _filter = new PopupWindowHelperMessageFilter(this);
            _filter.PopupCancel += popup_Cancel;
        }

        private void popup_Cancel(object sender, PopupCancelEventArgs e)
        {
            PopupCancel?.Invoke(this, e);
        }


        /// <summary>
        /// Subclasses the owning form's existing Window Procedure to enables the 
        /// title bar to remain active when a popup is show, and to detect if
        /// the user clicks onto another application whilst the popup is visible.
        /// </summary>
        /// <param name="m">Window Procedure Message</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (this._isShowing)
            {
                // check for WM_ACTIVATE and WM_NCACTIVATE
                if (m.Msg == NativeCalls.WM_NCACTIVATE)
                {
                    // Check if the title bar will made inactive:
                    if (((int) m.WParam) == 0)
                    {
                        // If so reactivate it.
                        NativeCalls.SendMessage(this.Handle, NativeCalls.WM_NCACTIVATE, (IntPtr) 1, IntPtr.Zero);
						
                        // Note it's no good to try and consume this message;
                        // if you try to do that you'll end up with windows
                        // that don't respond.
                    }

                }
                else if (m.Msg == NativeCalls.WM_ACTIVATEAPP)
                {
                    // Check if the application is being deactivated.
                    if ((int)m.WParam == 0)
                    {
                        // It is so cancel the popup:
                        ClosePopup();
                        // And put the title bar into the inactive state:
                        NativeCalls.PostMessage(this.Handle, NativeCalls.WM_NCACTIVATE, 0, IntPtr.Zero);
                    }
                }
            }
        }

        public void ShowPopup(Form owner, Form popup, Point location)
        {
            this._owner = owner;
            this._popup = popup;

            // Start checking for the popup being cancelled
            Application.AddMessageFilter(_filter);

            // Set the location of the popup form:
            popup.StartPosition = FormStartPosition.Manual;
            popup.Location = location; 
            // Make it owned by the window that's displaying it:
            owner.AddOwnedForm(popup);			
            // Respond to the Closed event in case the popup
            // is closed by its own internal means
            _closedHandler = popup_Closed;
            popup.Closed += _closedHandler;

            // Show the popup:
            this._isShowing = true;
            popup.Show();
            popup.Activate();
			
            // Start filtering for mouse clicks outside the popup
            _filter.Popup = popup;
					
        }

        public void ClosePopup()
        {
            if (_isShowing)
            {
                OnPopupClosed(new PopupClosedEventArgs(_popup));
                _owner.RemoveOwnedForm(_popup);
                _isShowing = false;
                _popup.Closed -= _closedHandler;
                _closedHandler = null;
                _popup.Close();
                Application.RemoveMessageFilter(_filter);
                _owner.Activate();
                _popup = null;
                _owner = null;
            }
        }

        private void OnPopupClosed(PopupClosedEventArgs e)
        {
            PopupClosed?.Invoke(this, e);
        }

        private void popup_Closed(object sender, EventArgs e)
        {
            ClosePopup();
        }
    }
}