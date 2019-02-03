using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class Utils {

        public static Texture2D LoadTex(string texture_name) {

            Texture2D tex = null;
            if (File.Exists(texture_name)) {
                tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texture_name, typeof(Texture2D));
            } else if (File.Exists(texture_name + ".PNG")) {
                tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texture_name + ".PNG", typeof(Texture2D));
            } else if (File.Exists(texture_name + ".JPG")) {
                tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texture_name + ".JPG", typeof(Texture2D));
            } else if (File.Exists(texture_name + ".BMP")) {
                tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texture_name + ".BMP", typeof(Texture2D));
            } else if (File.Exists(texture_name + ".TGA")) {
                tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texture_name + ".TGA", typeof(Texture2D));
            } else if (File.Exists(texture_name + ".DDS")) {
                tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texture_name + ".DDS", typeof(Texture2D));
            }
            return tex;
        }

        public void TangentSolver(Mesh mesh) {
            var tan2 = new Vector3[mesh.vertices.Length];
            var tan1 = new Vector3[mesh.vertices.Length];
            var tangents = new Vector4[mesh.vertices.Length];
            //Vector3[] binormal = new Vector3[mesh.vertices.Length];
            for (var a = 0; a < (mesh.triangles.Length); a += 3) {
                long i1 = mesh.triangles[a + 0];
                long i2 = mesh.triangles[a + 1];
                long i3 = mesh.triangles[a + 2];

                var v1 = mesh.vertices[i1];
                var v2 = mesh.vertices[i2];
                var v3 = mesh.vertices[i3];

                var w1 = mesh.uv[i1];
                var w2 = mesh.uv[i2];
                var w3 = mesh.uv[i3];

                var x1 = v2.x - v1.x;
                var x2 = v3.x - v1.x;
                var y1 = v2.y - v1.y;
                var y2 = v3.y - v1.y;
                var z1 = v2.z - v1.z;
                var z2 = v3.z - v1.z;

                var s1 = w2.x - w1.x;
                var s2 = w3.x - w1.x;
                var t1 = w2.y - w1.y;
                var t2 = w3.y - w1.y;

                var r = 1.0F / (s1 * t2 - s2 * t1);
                var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (var a = 0; a < mesh.vertices.Length; a++) {
                var n = mesh.normals[a];
                var t = tan1[a];

                Vector3.OrthoNormalize(ref n, ref t);
                tangents[a].x = t.x;
                tangents[a].y = t.y;
                tangents[a].z = t.z;

                // Calculate handedness
                tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;

                //To calculate binormals if required as vector3 try one of below:-
                //Vector3 binormal[a] = (Vector3.Cross(n, t) * tangents[a].w).normalized;
                //Vector3 binormal[a] = Vector3.Normalize(Vector3.Cross(n, t) * tangents[a].w)
            }
            mesh.tangents = tangents;
        }

    }
    
}