using System.Collections;
using System.Diagnostics;

namespace Assets.PatternDesigner.Scripts.misc.patterns
{
    public class JSONObjectEnumer : IEnumerator
    {
        public JSONObject _jobj;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        private int position = -1;

        public JSONObjectEnumer(JSONObject jsonObject)
        {
            Debug.Assert(jsonObject.isContainer); //must be an array or object to itterate
            _jobj = jsonObject;
        }

        public JSONObject Current
        {
            get
            {
                if (_jobj.IsArray) return _jobj[position];

                var key = _jobj.keys[position];
                return _jobj[key];
            }
        }

        public bool MoveNext()
        {
            position++;
            return position < _jobj.Count;
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current => Current;
    }
}