using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3dSoftware_Rendering
{
    public class OBJModel
    {
        private class OBJIndex
        {
            private int m_vertexIndex;
            private int m_texCoordIndex;
            private int m_normalIndex;

            public int GetVertexIndex() { return m_vertexIndex; }
            public int GetTexCoordIndex() { return m_texCoordIndex; }
            public int GetNormalIndex() { return m_normalIndex; }

            public void SetVertexIndex(int val) { m_vertexIndex = val; }
            public void SetTexCoordIndex(int val) { m_texCoordIndex = val; }
            public void SetNormalIndex(int val) { m_normalIndex = val; }

            //@Override
            public override bool Equals(Object obj)
            {
                OBJIndex index = (OBJIndex)obj;

                return m_vertexIndex == index.m_vertexIndex
                        && m_texCoordIndex == index.m_texCoordIndex
                        && m_normalIndex == index.m_normalIndex;
            }

            //@Override
            public override int GetHashCode()
            {
                const int BASE = 17;
                const int MULTIPLIER = 31;

                int result = BASE;

                result = MULTIPLIER * result + m_vertexIndex;
                result = MULTIPLIER * result + m_texCoordIndex;
                result = MULTIPLIER * result + m_normalIndex;

                return result;
            }
        }

        private List<Vector4f> m_positions;
        private List<Vector4f> m_texCoords;
        private List<Vector4f> m_normals;
        private List<OBJIndex> m_indices;
        private bool m_hasTexCoords;
        private bool m_hasNormals;

        private static String[] RemoveEmptyStrings(String[] data)
        {
            List<String> result = new List<String>();

            for (int i = 0; i < data.Length; i++)
                if (!data[i].Equals(""))
                    result.Add(data[i]);

            String[] res = new String[result.Count];
            res = result.ToArray();

            return res;
        }

        public OBJModel(String fileName) //throws IOException
        {
            try
            {
                m_positions = new List<Vector4f>();
                m_texCoords = new List<Vector4f>();
                m_normals = new List<Vector4f>();
                m_indices = new List<OBJIndex>();
                m_hasTexCoords = false;
                m_hasNormals = false;

                StreamReader meshReader = null;

                meshReader = new StreamReader(fileName);
                String line;

                while ((line = meshReader.ReadLine()) != null)
                {
                    String[] tokens = line.Split(' ');
                    tokens = RemoveEmptyStrings(tokens);

                    if (tokens.Length == 0 || tokens[0].Equals("#"))
                        continue;
                    else if (tokens[0].Equals("v"))
                    {
                        m_positions.Add(new Vector4f(float.Parse(tokens[1]),
                                float.Parse(tokens[2]),
                                float.Parse(tokens[3]), 1));
                    }
                    else if (tokens[0].Equals("vt"))
                    {
                        m_texCoords.Add(new Vector4f(float.Parse(tokens[1]),
                                1.0f - float.Parse(tokens[2]), 0, 0));
                    }
                    else if (tokens[0].Equals("vn"))
                    {
                        m_normals.Add(new Vector4f(float.Parse(tokens[1]),
                                float.Parse(tokens[2]),
                                float.Parse(tokens[3]), 0));
                    }
                    else if (tokens[0].Equals("f"))
                    {
                        for (int i = 0; i < tokens.Length - 3; i++)
                        {
                            m_indices.Add(ParseOBJIndex(tokens[1]));
                            m_indices.Add(ParseOBJIndex(tokens[2 + i]));
                            m_indices.Add(ParseOBJIndex(tokens[3 + i]));
                        }
                    }
                }


                meshReader.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IndexedModel ToIndexedModel()
        {
            IndexedModel result      = new IndexedModel();
            IndexedModel normalModel = new IndexedModel();
            Dictionary<OBJIndex, int> resultIndexMap = new Dictionary<OBJIndex, int>();
            Dictionary<int, int> normalIndexMap      = new Dictionary<int, int>();
            Dictionary<int, int> indexMap            = new Dictionary<int, int>();

            for (int i = 0; i < m_indices.Count; i++)
            {
                OBJIndex currentIndex = m_indices[i];

                Vector4f currentPosition = m_positions[currentIndex.GetVertexIndex()];
                Vector4f currentTexCoord;
                Vector4f currentNormal;

                if (m_hasTexCoords)
                    currentTexCoord = m_texCoords[currentIndex.GetTexCoordIndex()];
                else
                    currentTexCoord = new Vector4f(0, 0, 0, 0);

                if (m_hasNormals)
                    currentNormal = m_normals[currentIndex.GetNormalIndex()];
                else
                    currentNormal = new Vector4f(0, 0, 0, 0);
                int modelVertexIndex;
                try
                {
                    modelVertexIndex = resultIndexMap[currentIndex];
                }
                catch (Exception)
                {
                    modelVertexIndex = 0;
                }
                if (modelVertexIndex == 0) //null
                {
                    modelVertexIndex = result.GetPositions().Count;
                    resultIndexMap[currentIndex] = modelVertexIndex;

                    result.GetPositions().Add(currentPosition);
                    result.GetTexCoords().Add(currentTexCoord);
                    if (m_hasNormals)
                        result.GetNormals().Add(currentNormal);
                }

                int normalModelIndex;
                try
                {
                    normalModelIndex = normalIndexMap[currentIndex.GetVertexIndex()];

                }
                catch (Exception)
                {
                    normalModelIndex = 0;
                }
                if (normalModelIndex == 0) //null
                {
                    normalModelIndex = normalModel.GetPositions().Count;
                    normalIndexMap[currentIndex.GetVertexIndex()] = normalModelIndex;

                    normalModel.GetPositions().Add(currentPosition);
                    normalModel.GetTexCoords().Add(currentTexCoord);
                    normalModel.GetNormals().Add(currentNormal);
                    normalModel.GetTangents().Add(new Vector4f(0, 0, 0, 0));
                }

                result.GetIndices().Add(modelVertexIndex);
                normalModel.GetIndices().Add(normalModelIndex);
                indexMap[modelVertexIndex] = normalModelIndex;
            }

            if (!m_hasNormals)
            {
                normalModel.CalcNormals();

                for (int i = 0; i < result.GetPositions().Count; i++)
                    result.GetNormals().Add(normalModel.GetNormals()[indexMap[i]]);
            }

            normalModel.CalcTangents();

            for (int i = 0; i < result.GetPositions().Count; i++)
                result.GetTangents().Add(normalModel.GetTangents()[indexMap[i]]);

            return result;
        }

        private OBJIndex ParseOBJIndex(String token)
        {
            String[] values = token.Split('/');

            OBJIndex result = new OBJIndex();
            result.SetVertexIndex(int.Parse(values[0]) - 1);

            if (values.Length > 1)
            {
                if (values[1] != string.Empty)
                {
                    m_hasTexCoords = true;
                    result.SetTexCoordIndex(int.Parse(values[1]) - 1);
                }

                if (values.Length > 2)
                {
                    m_hasNormals = true;
                    result.SetNormalIndex(int.Parse(values[2]) - 1);
                }
            }

            return result;
        }
    }
}
