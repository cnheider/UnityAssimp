using System;
using System.Collections.Generic;
using System.IO;
using Plugins;
using UnityEditor;
using UnityEngine;

namespace Editor {

    public static class ImportAssimp {
        public static bool _SaveAssets = true;

        [MenuItem( Assimp._MenuPath + "ImportStatic")]
        static void Init()
        {

            var ao = Selection.activeObject;
            if(ao!=null){
                var ap = AssetDatabase.GetAssetPath(ao);
                var filename = Path.GetFileName(ap);
                var rootPath = Path.GetDirectoryName(ap);

                ReadMesh(rootPath, filename, "");
            }
        }

        private static void Trace(string msg) {
        }

        private class AssimpMesh {
            public Mesh _Geometry;
            public string _Name;
            public Material _Material;
            public List<Vector3> _Vertices;
            public List<Vector3> _Normals;
            public List<Vector4> _Tangents;
            public List<Vector2> _Uvcoords;
            public List<int> _Faces;
            public GameObject _MeshContainer;
            public MeshFilter _MeshFilter;
            public MeshRenderer _MeshRenderer;
            public GameObject _Root;

            public AssimpMesh(GameObject parent, GameObject root, string name) {
                this._Name = name;
                this._MeshContainer = new GameObject(name);

                this._Root = root;
                this._MeshContainer.transform.parent = parent.transform;

                this._MeshContainer.AddComponent<MeshFilter>();
                this._MeshContainer.AddComponent<MeshRenderer>();
                this._MeshFilter = this._MeshContainer.GetComponent<MeshFilter>();
                this._MeshRenderer = this._MeshContainer.GetComponent<MeshRenderer>();

                this._MeshFilter.sharedMesh = new Mesh();
                this._Geometry = this._MeshFilter.sharedMesh;
                this._Vertices = new List<Vector3>();
                this._Normals = new List<Vector3>();
                this._Tangents = new List<Vector4>();
                this._Uvcoords = new List<Vector2>();
                this._Faces = new List<int>();
            }

            public void AddVertex(Vector3 pos, Vector3 normal, Vector2 uv, Vector4 tan) {
                this._Vertices.Add(pos);
                this._Normals.Add(normal);
                this._Uvcoords.Add(uv);
                this._Tangents.Add(tan);
            }

            public void SetMaterial(Material mat) {
                this._MeshRenderer.sharedMaterial = mat;
            }

            public void AddFace(int a, int b, int c) {
                this._Faces.Add(a);
                this._Faces.Add(b);
                this._Faces.Add(c);
            }

            public void Build() {
                this._Geometry.vertices = this._Vertices.ToArray();
                this._Geometry.normals = this._Normals.ToArray();
                this._Geometry.uv = this._Uvcoords.ToArray();
                //geometry.u = uvcoords.ToArray();
                this._Geometry.triangles = this._Faces.ToArray();
                this._Geometry.tangents = this._Tangents.ToArray();
                Unwrapping.GenerateSecondaryUVSet(this._Geometry);
                //geometry.RecalculateNormals();
                this._Geometry.RecalculateBounds();
                //TangentSolver(geometry);
                ;
            }

            public void Dispose() {
                this._Vertices.Clear();
                this._Normals.Clear();
                this._Faces.Clear();
                this._Uvcoords.Clear();
                this._Tangents.Clear();
            }
        }

        public static void ReadMesh(string path, string filename, string texturepath) {
            if (File.Exists(path + "/" + filename)) {
                const Assimp.PostProcessSteps flags = (
                                                   //        Assimp.PostProcessSteps.MakeLeftHanded |

                                                   Assimp.PostProcessSteps.OptimizeMeshes |
                                                   Assimp.PostProcessSteps.OptimizeGraph |
                                                   Assimp.PostProcessSteps.RemoveRedundantMaterials |
                                                   Assimp.PostProcessSteps.SortByPrimitiveType |
                                                   Assimp.PostProcessSteps.SplitLargeMeshes |
                                                   Assimp.PostProcessSteps.Triangulate |
                                                   Assimp.PostProcessSteps.CalculateTangentSpace |
                                                   Assimp.PostProcessSteps.GenerateUVCoords |
                                                   Assimp.PostProcessSteps.GenerateSmoothNormals |
                                                   Assimp.PostProcessSteps.RemoveComponent |
                                                   Assimp.PostProcessSteps.JoinIdenticalVertices
                                               );

                var config = Assimp.aiCreatePropertyStore();

                Assimp.aiSetImportPropertyFloat(config, Assimp.AI_CONFIG_PP_CT_MAX_SMOOTHING_ANGLE, 60.0f);

                // IntPtr scene = Assimp.aiImportFile(path + "/" + filename, (uint)flags);
                var scene = Assimp.aiImportFileWithProperties(path + "/" + filename, (uint)flags, config);
                Assimp.aiReleasePropertyStore(config);
                if (scene == null) {
                    Debug.LogWarning("failed to read file: " + path + "/" + filename);
                    return;
                } else {
                    var nm = Path.GetFileNameWithoutExtension(filename);

                    var importingAssetsDir = "Assets/Prefabs/" + nm + "/";

                    if (_SaveAssets) {
                        if (!Directory.Exists(importingAssetsDir)) {
                            Directory.CreateDirectory(importingAssetsDir);
                        }
                        AssetDatabase.Refresh();
                    }

                    var objectRoot = new GameObject(nm);
                    var meshContainer = new GameObject(nm + "_Mesh");
                    meshContainer.transform.parent = objectRoot.transform;

                    var materials = new List<Material>();
                    var meshList = new List<AssimpMesh>();

                    for (var i = 0; i < Assimp.aiScene_GetNumMaterials(scene); i++) {
                        var matName = Assimp.aiMaterial_GetName(scene, i);
                        matName = nm + "_mat" + i;

                        //  string fname = Path.GetFileNameWithoutExtension(Assimp.aiMaterial_GetTexture(scene, i, (int)Assimp.TextureType.Diffuse));
                        var file_name = Path.GetFileName(Assimp.aiMaterial_GetTexture(scene, i, (int)Assimp.TextureType.Diffuse));
                        Debug.Log("texture " + file_name + "Material :" + matName);

                        var ambient = Assimp.aiMaterial_GetAmbient(scene, i);
                        var diffuse = Assimp.aiMaterial_GetDiffuse(scene, i);
                        var specular = Assimp.aiMaterial_GetSpecular(scene, i);
                        var emissive = Assimp.aiMaterial_GetEmissive(scene, i);

                        var mat = new Material(Shader.Find("Diffuse")) {name = matName};

                        var textureName = path + "/" + file_name;

                        var tex = Utils.LoadTex(textureName);

                        //Texture2D tex = Resources.Load(texturename) as Texture2D;
                        //Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texturename, typeof(Texture2D));
                        if (tex != null) {
                            Debug.Log("LOAD (" + textureName + ") texture");
                            mat.SetTexture("_MainTex", tex);
                        } else {
                            Debug.LogError("Fail LOAD (" + textureName + ") error");
                        }

                        if (_SaveAssets) {
                            var materialAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + mat.name + ".asset");
                            AssetDatabase.CreateAsset(mat, materialAssetPath);
                        }
                        materials.Add(mat);
                    }

                    AssetDatabase.Refresh();

                    if (Assimp.aiScene_HasMeshes(scene)) {
                        for (var i = 0; i < Assimp.aiScene_GetNumMeshes(scene); i++) {
                            var name = "Mesh_";
                            name += i.ToString();

                            var hasNormals = Assimp.aiMesh_HasNormals(scene, i);
                            var hasTexCoord = Assimp.aiMesh_HasTextureCoords(scene, i, 0);
                            var hasFaces = Assimp.aiMesh_HasFaces(scene, i);

                            var mesh = new AssimpMesh(meshContainer, objectRoot, name);
                            mesh.SetMaterial(materials[Assimp.aiMesh_GetMaterialIndex(scene, i)]);
                            meshList.Add(mesh);

                            for (var v = 0; v < Assimp.aiMesh_GetNumVertices(scene, i); v++) {
                                var vertex = Assimp.aiMesh_Vertex(scene, i, v);
                                var n = Assimp.aiMesh_Normal(scene, i, v);
                                var x = Assimp.aiMesh_TextureCoordX(scene, i, v, 0);
                                var y = Assimp.aiMesh_TextureCoordY(scene, i, v, 0);

                                var binormalf = Assimp.aiMesh_Bitangent(scene, i, v);
                                var tangentf = Assimp.aiMesh_Tangent(scene, i, v);
                                var outputTangent = new Vector4(tangentf.x, tangentf.y, tangentf.z, 0.0F);

                                var dp = Vector3.Dot(Vector3.Cross(n, tangentf), binormalf);
                                if (dp > 0.0F) {
                                    outputTangent.w = 1.0F;
                                } else {
                                    outputTangent.w = -1.0F;
                                }

                                mesh.AddVertex(vertex, n, new Vector2(x, y), outputTangent);
                                //mesh.addVertex(vertex, new Vector3(1 * -n.x, n.y, n.z), new Vector2(x, y), outputTangent);
                            }

                            for (var f = 0; f < Assimp.aiMesh_GetNumFaces(scene, i); f++) {
                                var a = Assimp.aiMesh_Indice(scene, i, f, 0);
                                var b = Assimp.aiMesh_Indice(scene, i, f, 1);
                                var c = Assimp.aiMesh_Indice(scene, i, f, 2);
                                mesh.AddFace(a, b, c);
                            }

                            //**********
                            mesh.Build();
                            if (_SaveAssets) {
                                var meshAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + mesh._Name + ".asset");
                                AssetDatabase.CreateAsset(mesh._Geometry, meshAssetPath);
                            }

                            mesh.Dispose();
                        }
                    }

                    if (_SaveAssets) {

                        var prefabPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + filename + ".prefab");
                        var prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
                        PrefabUtility.ReplacePrefab(objectRoot, prefab, ReplacePrefabOptions.ConnectToPrefab);
                        AssetDatabase.Refresh();
                    }

                    meshList.Clear();
                }
                Assimp.aiReleaseImport(scene);
                Debug.LogWarning(path + "/" + filename + " Imported ;) ");
            }
        }
    }
}