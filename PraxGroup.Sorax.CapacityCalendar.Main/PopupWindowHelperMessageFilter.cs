using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PopupWindowHelperMessageFilter : IMessageFilter
    {
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_NCLBUTTONDOWN = 0x0A1;
        private const int WM_NCRBUTTONDOWN = 0x0A4;
        private const int WM_NCMBUTTONDOWN = 0x0A7;

        private readonly PopupWindowHelper _owner;

        public event PopupCancelEventHandler PopupCancel;

        public Form Popup { get; set; }

        public PopupWindowHelperMessageFilter(PopupWindowHelper owner)
        {
            _owner = owner;
        }


        public bool PreFilterMessage(ref Message m)
        {
            if (Popup == null)
            {
                return false;
            }


            switch (m.Msg)
            {				
                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_MBUTTONDOWN:
                case WM_NCLBUTTONDOWN:
                case WM_NCRBUTTONDOWN:
                case WM_NCMBUTTONDOWN:
                    OnMouseDown();
                    break;
            }
            return false;
        }

        private void OnMouseDown()
        {
            var cursorPosition = Cursor.Position;
            if (!Popup.Bounds.Contains(cursorPosition))
            {
                OnCancelPopup(new PopupCancelEventArgs(Popup, cursorPosition));
            }
        }

        private void OnCancelPopup(PopupCancelEventArgs e)
        {
            PopupCancel?.Invoke(this, e);
            if (!e.Cancel)
            {
                _owner.ClosePopup();
                Popup = null;
            }
        }
    }
}