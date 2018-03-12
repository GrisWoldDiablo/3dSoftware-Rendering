using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3dSoftware_Rendering
{
    public class Mesh
    {
        private List<Vertex> m_vertices;
        private List<int> m_indices;

        public Vertex GetVertex(int i) { return m_vertices[i]; }
        public int GetIndex(int i)     { return m_indices[i]; }
        public int GetNumIndices()     { return m_indices.Count; }


        public Mesh(string fileName)
        {
            IndexedModel model = new OBJModel(fileName).ToIndexedModel();

            m_vertices = new List<Vertex>();
            for (int i = 0; i < model.GetPositions().Count; i++)
            {
                m_vertices.Add(new Vertex(model.GetPositions()[i], model.GetTexCoords()[i]));
            }

            m_indices = model.GetIndices();
            //try
            //{
            //    
            //}
            //catch (Exception ex)
            //{
            //    //throw ex;
            //}
        }
    }
}
