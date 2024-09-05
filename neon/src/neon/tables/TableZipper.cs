using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    // Zips 2 tables, managed by TableCollection ? Or a ZipperCollection ? Something that gets them where they need it

    public class TableZipper<T, U> : IEnumerable<KeyValuePair<T, U>> where T : class, IComponent where U : class, IComponent
    {
        private ITable<T> m_FirstTable;
        private ITable<U> m_SecondTable;

        private bool m_UpdatedThisFrame = false;

        private List<Tuple<int, int>> m_ZippedIndices = new List<Tuple<int, int>>();

        public TableZipper(ITable<T> firstTable, ITable<U> secondTable)
        {
            m_FirstTable = firstTable;
            m_SecondTable = secondTable;

            Refresh();
        }

        public void SetNewFrame() => m_UpdatedThisFrame = false;

        public void RequestUpdate()
        {
            if (m_UpdatedThisFrame)
                return;

            m_UpdatedThisFrame = true;

            // Refresh
        }

        private void Refresh()
        {

        }

        public IEnumerator<KeyValuePair<T, U>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
