using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public static class TrackingContextFactory
    {
        #region Fields

        private static TrackingContext _default = null;
        private static Func<TrackingContext> _current = null;

        #endregion

        #region Properties

        public static TrackingContext Current
        {
            get
            {
                if (_current != null)
                    return _current();

                if (_default == null)
                {
                    _default = new TrackingContext();
                    _current = () => _default;
                }

                return _current();
            }
        }

        #endregion

        public static void SetTrackingContext(Func<TrackingContext> callback)
        {
            _current = callback;
        }
    }
}
