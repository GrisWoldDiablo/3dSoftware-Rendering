using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3dSoftware_Rendering
{
    public class IndexedModel
    {
        private List<Vector4f> m_positions;
        private List<Vector4f> m_texCoords;
        private List<Vector4f> m_normals;
        private List<Vector4f> m_tangents;
        private List<int> m_indices;

        public IndexedModel()
        {
            m_positions = new List<Vector4f>();
            m_texCoords = new List<Vector4f>();
            m_normals   = new List<Vector4f>();
            m_tangents  = new List<Vector4f>();
            m_indices   = new List<int>();
        }

        public void CalcNormals()
        {
            for (int i = 0; i < m_indices.Count; i += 3)
            {
                int i0 = m_indices[i];
                int i1 = m_indices[i + 1];
                int i2 = m_indices[i + 2];

                Vector4f v1 = m_positions[i1] - m_positions[i0];
                Vector4f v2 = m_positions[i2] - m_positions[i0];

                Vector4f normal = v1.Cross(v2).Normalized();

                m_normals[i0]= m_normals[i0]+normal;
                m_normals[i1]= m_normals[i1]+normal;
                m_normals[i2]= m_normals[i2]+normal;
            }

            for (int i = 0; i < m_normals.Count; i++)
            {
                m_normals[i] = m_normals[i].Normalized();
            }
        }

        public void CalcTangents()
        {
            for (int i = 0; i < m_indices.Count; i += 3)
            {
                int i0 = m_indices[i];
                int i1 = m_indices[i + 1];
                int i2 = m_indices[i + 2];

                Vector4f edge1 = m_positions[i1] - m_positions[i0];
                Vector4f edge2 = m_positions[i2] - m_positions[i0];

                float deltaU1 = m_texCoords[i1][0] - m_texCoords[i0][0];
                float deltaV1 = m_texCoords[i1][1] - m_texCoords[i0][1];
                float deltaU2 = m_texCoords[i2][0] - m_texCoords[i0][0];
                float deltaV2 = m_texCoords[i2][1] - m_texCoords[i0][1];

                float dividend = (deltaU1 * deltaV2 - deltaU2 * deltaV1);
                float f = dividend == 0 ? 0.0f : 1.0f / dividend;

                Vector4f tangent = new Vector4f(
                        f * (deltaV2 * edge1[0] - deltaV1 * edge2[0]),
                        f * (deltaV2 * edge1[1] - deltaV1 * edge2[1]),
                        f * (deltaV2 * edge1[2] - deltaV1 * edge2[2]),
                        0);

                m_tangents[i0] = m_tangents[i0] + tangent;
                m_tangents[i1] = m_tangents[i1] + tangent;
                m_tangents[i2] = m_tangents[i2] + tangent;
            }

            for (int i = 0; i < m_tangents.Count; i++)
            { 
                m_tangents[i] = m_tangents[i].Normalized();
            }
        }

        public List<Vector4f> GetPositions() { return m_positions; }
        public List<Vector4f> GetTexCoords() { return m_texCoords; }
        public List<Vector4f> GetNormals()   { return m_normals; }
        public List<Vector4f> GetTangents()  { return m_tangents; }
        public List<int> GetIndices()        { return m_indices; }
    }
}
