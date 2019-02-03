using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Plugins;
using UnityEditor;
using UnityEngine;

namespace Editor {

    public static class ImportAssimpSkinned {
        public static bool _SaveAssets = true;
        public static bool _UseTangents = true;
        public static bool _IgnoreRotations = false;
        public static bool _IgnorePositions = false;
        public static bool _IgnoreScalling = true;

        private static StreamWriter _stream_writer;
        private static List<AssimpJoint> _list_joints = new List<AssimpJoint>();

        [MenuItem(Assimp._MenuPath +"ImportSkinned")]
        static void Init() {
            var filename = Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject));
            var rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
            /*
                    FileStream fileStream = new FileStream("Assets/Log.txt",
                                                 FileMode.Create,
                                                  FileAccess.Write,
                                                  FileShare.ReadWrite);
                    streamWriter = new StreamWriter(fileStream);//"Assets/Log.txt",false);
                    streamWriter.WriteLine("Import mesh");
            */

            ReadMesh(rootPath, filename, "Textures/");
            /*
            streamWriter.Flush();
            streamWriter.Close();
            */
        }



        private static void Trace(string msg) {
            //	Debug.Log("LOG:" + msg);
        }

        public class AssimpMesh {
            public Mesh _Geometry;
            public string _Name;
            public List<AssimpJoint> _Joints;
            public List<Transform> _Bones;
            public List<BoneVertex> _BoneVertexList;
            public List<BoneWeight> _BoneWeightsList;
            public Material _Material;
            public List<Vector3> _Vertices;
            public List<Vector3> _Normals;
            public List<Vector4> _Tangents;
            public List<Vector2> _Uvcoords;
            public List<int> _Faces;
            public GameObject _MeshContainer;
            public SkinnedMeshRenderer _MeshRenderer;
            public GameObject _Root;
            public int _MaxBones;


            public AssimpMesh(GameObject parent, GameObject root, string name) {
                this._Name = name;
                this._MeshContainer = new GameObject(name);
                this._Root = root;
                this._MeshContainer.transform.parent = parent.transform;
                this._MeshRenderer = (SkinnedMeshRenderer)this._MeshContainer.AddComponent(typeof(SkinnedMeshRenderer));
                this._MeshRenderer.quality = SkinQuality.Auto;
                this._MeshRenderer.sharedMesh = new Mesh();
                this._MeshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
                this._Geometry = this._MeshRenderer.sharedMesh;
                this._Joints = new List<AssimpJoint>();
                this._BoneVertexList = new List<BoneVertex>();
                this._BoneWeightsList = new List<BoneWeight>();
                this._Vertices = new List<Vector3>();
                this._Normals = new List<Vector3>();
                this._Uvcoords = new List<Vector2>();
                this._Tangents = new List<Vector4>();
                this._Faces = new List<int>();
                this._Bones = new List<Transform>();
                this._MaxBones = 1;
            }

            public void AddVertex(Vector3 pos, Vector3 normal, Vector4 tan, Vector2 uv) {
                this._Vertices.Add(pos);
                this._Normals.Add(normal);
                this._Uvcoords.Add(uv);
                if (_UseTangents) this._Tangents.Add(tan);
                this._BoneVertexList.Add(new BoneVertex());
            }

            public void Setmaterial(Material mat) {
                this._MeshRenderer.sharedMaterial = mat;
            }

            public void AddFace(int a, int b, int c) {
                this._Faces.Add(a);
                this._Faces.Add(b);
                this._Faces.Add(c);
            }

            public void AddJoint(AssimpJoint joint) {
                this._Joints.Add(joint);
                this._Bones.Add(joint._Transform);
            }

            public void AddBone(int vertexId, int jointId, float force) {
                var vertex = this._BoneVertexList[vertexId];
                vertex.AddBone(jointId, force);
                this._BoneVertexList[vertexId] = vertex;
            }

            public void BuilSkin() {
                Debug.Log("****** build skin  ******************");

                for (var i = 0; i < this._BoneVertexList.Count; i++) {
                    var vertex = this._BoneVertexList[i];
                    var wight = new BoneWeight();
                    float len = 0;

                    var orderdWeights = vertex._Bones.OrderByDescending(t => t._Weight).ToList();
                    switch (orderdWeights.Count) {
                        case 0:
                            break;
                        case 1:
                            wight.boneIndex0 = orderdWeights[0]._BoneIndex;
                            wight.weight0 = orderdWeights[0]._Weight;
                            break;
                        case 2:
                            wight.boneIndex0 = orderdWeights[0]._BoneIndex;
                            wight.boneIndex1 = orderdWeights[1]._BoneIndex;
                            len = Mathf.Sqrt(orderdWeights[0]._Weight * orderdWeights[0]._Weight +
                                             orderdWeights[1]._Weight * orderdWeights[1]._Weight);
                            wight.weight0 = orderdWeights[0]._Weight / len;
                            wight.weight1 = orderdWeights[1]._Weight / len;
                            break;
                        case 3:
                            wight.boneIndex0 = orderdWeights[0]._BoneIndex;
                            wight.boneIndex1 = orderdWeights[1]._BoneIndex;
                            wight.boneIndex2 = orderdWeights[2]._BoneIndex;

                            len = Mathf.Sqrt(orderdWeights[0]._Weight * orderdWeights[0]._Weight +
                                             orderdWeights[1]._Weight * orderdWeights[1]._Weight +
                                             orderdWeights[2]._Weight * orderdWeights[2]._Weight);
                            wight.weight0 = orderdWeights[0]._Weight / len;
                            wight.weight1 = orderdWeights[1]._Weight / len;
                            wight.weight2 = orderdWeights[2]._Weight / len;
                            break;
                        default:
                            wight.boneIndex0 = orderdWeights[0]._BoneIndex;
                            wight.boneIndex1 = orderdWeights[1]._BoneIndex;
                            wight.boneIndex2 = orderdWeights[2]._BoneIndex;
                            wight.boneIndex3 = orderdWeights[3]._BoneIndex;

                            len = Mathf.Sqrt(orderdWeights[0]._Weight * orderdWeights[0]._Weight +
                                             orderdWeights[1]._Weight * orderdWeights[1]._Weight +
                                             orderdWeights[2]._Weight * orderdWeights[2]._Weight +
                                             orderdWeights[3]._Weight * orderdWeights[3]._Weight);
                            wight.weight0 = orderdWeights[0]._Weight / len;
                            wight.weight1 = orderdWeights[1]._Weight / len;
                            wight.weight2 = orderdWeights[2]._Weight / len;
                            wight.weight3 = orderdWeights[3]._Weight / len;
                            break;
                    }
                    //    trace("----------");
                    //    trace(" ");
                    //  trace(string.Format(" vertex:{0} num Bones:{1} force {2}", i, vertex.numbones,len));
                    this._MaxBones = Math.Max(this._MaxBones, vertex._Numbones);

                    //	Debug.Log(string.Format(" vertex:{0} num Bones:{1} force {2}", i, vertex.numbones,len));

                    //    trace(string.Format(" bone index :{0}  w:{1}", wight.boneIndex0, wight.weight0));
                    //    trace(string.Format(" bone index :{0}  w:{1}", wight.boneIndex1, wight.weight1));
                    //     trace(string.Format(" bone index :{0}  w:{1}", wight.boneIndex2, wight.weight2));
                    //     trace(string.Format(" bone index :{0}  w:{1}", wight.boneIndex3, wight.weight3));

                    this._BoneWeightsList.Add(wight);
                }
            }

            public void Build() {
                Debug.Log("****** build mesh  ******************");
                var bindposes = new List<Matrix4x4>();

                for (var i = 0; i < this._Joints.Count; i++) {
                    var joint = this._Joints[i];
                    var bone = this._Bones[i];
                    bindposes.Add(bone.worldToLocalMatrix * this._Root.transform.localToWorldMatrix);
                }

                this._Geometry.vertices = this._Vertices.ToArray();
                this._Geometry.normals = this._Normals.ToArray();
                this._Geometry.uv = this._Uvcoords.ToArray();
                this._Geometry.triangles = this._Faces.ToArray();
                this._Geometry.bindposes = bindposes.ToArray();
                this._Geometry.boneWeights = this._BoneWeightsList.ToArray();
                if (_UseTangents) this._Geometry.tangents = this._Tangents.ToArray();
                this._Geometry.RecalculateBounds();
                //    geometry.RecalculateNormals();
                ;
                this._MeshRenderer.sharedMesh = this._Geometry;
                this._MeshRenderer.bones = this._Bones.ToArray();
                Debug.Log(string.Format("mesh quality:{0}(num Bones)", this._MaxBones));

                switch (this._MaxBones) {
                    case 0: {
                            Debug.Log(" ?0?  set 1 bones ");
                            this._MeshRenderer.quality = SkinQuality.Bone1;
                            break;
                        }
                    case 1: {
                            Debug.Log(" is 1 set 1 bones ");
                            this._MeshRenderer.quality = SkinQuality.Bone2;
                            break;
                        }
                    case 2: {
                            this._MeshRenderer.quality = SkinQuality.Bone2;
                            Debug.Log(" set 2 bones ");
                            break;
                        }

                    case 3: {
                            this._MeshRenderer.quality = SkinQuality.Bone4;
                            Debug.Log(" set 4 bones ");
                            break;
                        }
                    default: {
                            Debug.Log("????? set " + this._MaxBones.ToString() + " bones ???");
                            this._MeshRenderer.quality = SkinQuality.Bone1;
                            break;
                        }
                }
                //	meshRenderer.quality = maxBones;
            }

            public void Dispose() {
                this._Joints.Clear();
                this._BoneWeightsList.Clear();
                this._BoneVertexList.Clear();
                this._Vertices.Clear();
                this._Normals.Clear();
                this._Faces.Clear();
                this._Tangents.Clear();
                this._Uvcoords.Clear();
            }
        }

        private static void ReadMesh(string path, string filename, string texturepath) {
            string importingAssetsDir;

            if (File.Exists(path + "/" + filename)) {
                var flags = (
                    //  Assimp.PostProcessSteps.MakeLeftHanded |
                   Assimp.PostProcessSteps.Triangulate |
                   Assimp.PostProcessSteps.CalculateTangentSpace |
                   Assimp.PostProcessSteps.GenerateUVCoords |
                   Assimp.PostProcessSteps.GenerateSmoothNormals |
                   Assimp.PostProcessSteps.RemoveComponent |
                   Assimp.PostProcessSteps.JoinIdenticalVertices
                );

                var config = Assimp.aiCreatePropertyStore();

                Assimp.aiSetImportPropertyFloat(config, Assimp.AI_CONFIG_PP_CT_MAX_SMOOTHING_ANGLE, 60.0f);
                Assimp.aiSetImportPropertyInteger(config, Assimp.AI_CONFIG_PP_LBW_MAX_WEIGHTS, 4);
                //  IntPtr scene = Assimp.aiImportFile(path + "/" + filename, (uint)flags);
                var scene = Assimp.aiImportFileWithProperties(path + "/" + filename, (uint)flags, config);
                Assimp.aiReleasePropertyStore(config);
                if (scene == null) {
                    Debug.LogWarning("failed to read file: " + path + "/" + filename);
                    return;
                } else {
                    var nm = Path.GetFileNameWithoutExtension(filename);
                    importingAssetsDir = "Assets/Prefabs/" + nm + "/";

                    if (_SaveAssets) {
                        if (!Directory.Exists(importingAssetsDir)) {
                            Directory.CreateDirectory(importingAssetsDir);
                        }
                        AssetDatabase.Refresh();
                    }
                    var objectRoot = new GameObject(nm);
                    var meshContainer = new GameObject(nm + "_Mesh");
                    meshContainer.transform.parent = objectRoot.transform;
                    //  List<CombineInstance> combineInstances = new List<CombineInstance>();
                    var materials = new List<Material>();
                    var meshList = new List<AssimpMesh>();

                    for (var i = 0; i < Assimp.aiScene_GetNumMaterials(scene); i++) {
                        var matName = Assimp.aiMaterial_GetName(scene, i);
                        matName = nm + "_mat" + i;

                        //  string fname = Path.GetFileNameWithoutExtension(Assimp.aiMaterial_GetTexture(scene, i, (int)Assimp.TextureType.Diffuse));
                        var fname = Path.GetFileName(Assimp.aiMaterial_GetTexture(scene, i, (int)Assimp.TextureType.Diffuse));
                        Debug.Log("texture " + fname + "Material :" + matName);

                        var ambient = Assimp.aiMaterial_GetAmbient(scene, i);
                        var diffuse = Assimp.aiMaterial_GetDiffuse(scene, i);
                        var specular = Assimp.aiMaterial_GetSpecular(scene, i);
                        var emissive = Assimp.aiMaterial_GetEmissive(scene, i);

                        var mat = new Material(Shader.Find("Diffuse"));
                        mat.name = matName;
                        mat.color = diffuse;

                        //	Debug.Log(ambient.ToString());
                        //	Debug.Log(diffuse.ToString());
                        //	Debug.Log(specular.ToString());
                        //	Debug.Log(emissive.ToString());

                        var texturename = path + "/" + fname;

                        var tex = Utils.LoadTex(texturename);

                        if (tex != null) {
                            Debug.Log("LOAD (" + texturename + ") texture");
                            mat.SetTexture("_MainTex", tex);
                        } else {
                            Debug.LogError("Fail LOAD (" + texturename + ") error");
                        }

                        if (_SaveAssets) {
                            var materialAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + mat.name + ".asset");
                            AssetDatabase.CreateAsset(mat, materialAssetPath);
                        }
                        materials.Add(mat);
                    }

                    AssetDatabase.Refresh();

                    if (Assimp.aiScene_GetRootNode(scene) != null) {
                        objectRoot.transform.position = Assimp.aiNode_GetPosition(Assimp.aiScene_GetRootNode(scene));

                        //assimp quaternion is w,x,y,z and unity x,y,z,w bu int this lib i fix this for unity
                        var assQuad = Assimp.aiNode_GetRotation(Assimp.aiScene_GetRootNode(scene));
                        objectRoot.transform.rotation = assQuad;

                        var skeleton = new GameObject("Skeleton");
                        skeleton.transform.parent = objectRoot.transform;
                        ProcessNodes(scene, Assimp.aiScene_GetRootNode(scene), ref _list_joints);

                        for (var i = 0; i < _list_joints.Count; i++) {

                            var joint = _list_joints[i];
                            var bone = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                            //	bone.transform.localScale.Set(2,2,2);
                            //    Transform bone = new GameObject(joint.Name).transform;
                            //   DebugBone debug = (DebugBone)bone.gameObject.AddComponent(typeof(DebugBone));

                            bone.name = joint._Name;
                            bone.parent = skeleton.transform;

                            if (GetBoneByName(joint._ParentName) != null) {
                                var index = FindBoneByName(joint._ParentName);
                                bone.parent = joint._Parent._Transform;
                            }
                            bone.localPosition = joint._Position;
                            bone.localRotation = joint._Orientation;

                            joint._Transform = bone;
                        }
                    }

                    if (Assimp.aiScene_HasMeshes(scene)) {
                        for (var i = 0; i < Assimp.aiScene_GetNumMeshes(scene); i++) {
                            var name = "Mesh_";
                            name += i.ToString();

                            var hasNormals = Assimp.aiMesh_HasNormals(scene, i);
                            var hasTexCoord = Assimp.aiMesh_HasTextureCoords(scene, i, 0);
                            var hasFaces = Assimp.aiMesh_HasFaces(scene, i);

                            var mesh = new AssimpMesh(meshContainer, objectRoot, name);
                            mesh.Setmaterial(materials[Assimp.aiMesh_GetMaterialIndex(scene, i)]);
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
                                mesh.AddVertex(vertex, n, outputTangent, new Vector2(x, y));
                            }

                            for (var f = 0; f < Assimp.aiMesh_GetNumFaces(scene, i); f++) {
                                var a = Assimp.aiMesh_Indice(scene, i, f, 0);
                                var b = Assimp.aiMesh_Indice(scene, i, f, 1);
                                var c = Assimp.aiMesh_Indice(scene, i, f, 2);
                                mesh.AddFace(a, b, c);
                            }

                            //****
                            var numBone = Assimp.aiMesh_GetNumBones(scene, i);

                            for (var b = 0; b < numBone; b++) {
                                var bname = Assimp.aiMesh_GetBoneName(scene, i, b);
                                var joint = GetBoneByName(bname);
                                var boneId = FindBoneByName(bname);
                                var numWeights = Assimp.aiMesh_GetNumBoneWeights(scene, i, b);
                                for (var w = 0; w < numWeights; w++) {
                                    var weight = Assimp.aiMesh_GetBoneWeight(scene, i, b, w);
                                    var vertexId = Assimp.aiMesh_GetBoneVertexId(scene, i, b, w);
                                    mesh.AddBone(vertexId, boneId, weight);
                                }
                            }

                            for (var j = 0; j < _list_joints.Count; j++) {
                                var joint = _list_joints[j];
                                mesh.AddJoint(joint);
                            }
                            //**********
                            mesh.BuilSkin();
                            mesh.Build();
                            if (_SaveAssets) {
                                var meshAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + mesh._Name + ".asset");
                                AssetDatabase.CreateAsset(mesh._Geometry, meshAssetPath);
                            }
                            mesh.Dispose();
                        }
                    }

                    //create key frames
                    if (Assimp.aiScene_HasAnimation(scene)) {
                        var anim = (Animation)objectRoot.AddComponent(typeof(Animation));
                        var numAnimation = Assimp.aiScene_GetNumAnimations(scene);
                        Debug.Log("count animation  :" + numAnimation);
                        for (var a = 0; a < numAnimation; a++) {
                            var clip = new AnimationClip();
                            var anima = Assimp.aiAnim_GetName(scene, a);
                            clip.name = nm + "_" + anima + "_" + a;
                            clip.legacy = true;

                            clip.wrapMode = WrapMode.Loop;

                            var tinks = (float)Assimp.aiAnim_GetTicksPerSecond(scene, a);
                            if (tinks <= 1f) tinks = 1f;
                            var fps = tinks;
                            clip.frameRate = tinks;
                            Debug.Log("animation fps :" + fps);
                            var numchannels = Assimp.aiAnim_GetNumChannels(scene, a);
                            for (var i = 0; i < numchannels; i++) {
                                var name = Assimp.aiAnim_GetChannelName(scene, a, i);
                                var joint = GetBoneByName(name);
                                //  Debug.Log(String.Format("anim channel {0} bone name {1}  poskeys {2}  rotkeys{2}", i, name, Assimp.aiAnim_GetNumPositionKeys(scene, 0, i), Assimp.aiAnim_GetNumRotationKeys(scene, 0, i)));
                                //public static bool ignoreRotations = true;
                                // public static bool ignorePositions = true;
                                // public static bool ignoreScalling = true;
                                if (!_IgnoreScalling) {
                                    if (Assimp.aiAnim_GetNumScalingKeys(scene, a, i) != 0) {
                                        var scaleXcurve = new AnimationCurve();
                                        var scaleYcurve = new AnimationCurve();
                                        var scaleZcurve = new AnimationCurve();

                                        for (var j = 0; j < Assimp.aiAnim_GetNumScalingKeys(scene, a, i); j++) {
                                            var time = (float)Assimp.aiAnim_GetScalingFrame(scene, a, i, j);// / fps;
                                            var scale = Assimp.aiAnim_GetScalingKey(scene, a, i, j);
                                            scaleXcurve.AddKey(time, scale.x);
                                            scaleYcurve.AddKey(time, scale.y);
                                            scaleZcurve.AddKey(time, scale.z);
                                        }
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "m_LocalScale.x", scaleXcurve);
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "m_LocalScale.y", scaleYcurve);
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "m_LocalScale.z", scaleZcurve);
                                    }
                                }

                                if (!_IgnorePositions) {
                                    if (Assimp.aiAnim_GetNumPositionKeys(scene, a, i) != 0) {
                                        var posXcurve = new AnimationCurve();
                                        var posYcurve = new AnimationCurve();
                                        var posZcurve = new AnimationCurve();

                                        for (var j = 0; j < Assimp.aiAnim_GetNumPositionKeys(scene, a, i); j++) {
                                            var time = (float)Assimp.aiAnim_GetPositionFrame(scene, a, i, j);// / fps;
                                            var pos = Assimp.aiAnim_GetPositionKey(scene, a, i, j);
                                            posXcurve.AddKey(time, pos.x);
                                            posYcurve.AddKey(time, pos.y);
                                            posZcurve.AddKey(time, pos.z);
                                        }

                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localPosition.x", posXcurve);
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localPosition.y", posYcurve);
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localPosition.z", posZcurve);
                                    }
                                }

                                if (!_IgnoreRotations) {
                                    if (Assimp.aiAnim_GetNumRotationKeys(scene, a, i) != 0) {

                                        var rotXcurve = new AnimationCurve();
                                        var rotYcurve = new AnimationCurve();
                                        var rotZcurve = new AnimationCurve();
                                        var rotWcurve = new AnimationCurve();

                                        for (var j = 0; j < Assimp.aiAnim_GetNumRotationKeys(scene, a, i); j++) {
                                            var time = (float)Assimp.aiAnim_GetRotationFrame(scene, a, i, j);// / fps;

                                            var rotation = Assimp.aiAnim_GetRotationKey(scene, a, i, j);
                                            rotXcurve.AddKey(time, rotation.x);
                                            rotYcurve.AddKey(time, rotation.y);
                                            rotZcurve.AddKey(time, rotation.z);
                                            rotWcurve.AddKey(time, rotation.w);
                                        }
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localRotation.x", rotXcurve);
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localRotation.y", rotYcurve);
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localRotation.z", rotZcurve);
                                        clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localRotation.w", rotWcurve);
                                    }
                                }
                            }

                            clip.EnsureQuaternionContinuity();
                            anim.AddClip(clip, clip.name);
                            anim.clip = clip;

                            var clipAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + clip.name + ".asset");
                            AssetDatabase.CreateAsset(clip, clipAssetPath);

                            //     AssetDatabase.CreateAsset(clip, "Assets/Models/" + nm +"_"+a+ ".anim");
                            //    AssetDatabase.SaveAssets();
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

                _list_joints.Clear();

                Assimp.aiReleaseImport(scene);
                Debug.LogWarning(path + "/" + filename + " Imported ;) ");
            }
        }

        private static AssimpJoint GetBoneByName(string name) {
            for (var i = 0; i < _list_joints.Count; i++) {
                var bone = _list_joints[i];
                if (bone._Name == name) {
                    return bone;
                }
            }
            return null;
        }

        private static int FindBoneByName(string name) {
            for (var i = 0; i < _list_joints.Count; i++) {
                var bone = _list_joints[i];
                if (bone._Name == name) {
                    return i;
                }
            }
            return -1;
        }

        private static void ProcessNodes(IntPtr scene, IntPtr node, ref List<AssimpJoint> listJoints) {
            var joint = new AssimpJoint();

            var name = Assimp.aiNode_GetName(node);
            if (name == "") name = "REMOVE-ME";
            joint._Name = name;
            joint._ParentName = "NONE";
            joint._Position = Assimp.aiNode_GetPosition(node);
            //assimp quaternion is w,x,y,z and unity x,y,z,w bu int this lib i fix this for unity
            var quad = Assimp.aiNode_GetRotation(node);
            joint._Orientation = quad;
            if (Assimp.aiNode_GetParent(node) != null) {
                var parentName = Assimp.aiNode_GetName(Assimp.aiNode_GetParent(node));
                joint._ParentName = parentName;
            }
            listJoints.Add(joint);
            for (var i = 0; i < listJoints.Count; i++) {
                var parent = listJoints[i];
                if (joint._ParentName == parent._Name) {
                    joint._Parent = parent;
                    joint._Path += parent._Path + "/";
                    break;
                }
            }

            joint._Path += name;

            if (joint._Parent != null) {
                //   Debug.Log(string.Format(" Joint  name: {0}  ; parent:{1} ;  animation path:{2} ", joint.Name, joint.parent.Name,"Skeleton/" +  joint.Path));
            } else {
                //   Debug.Log(string.Format(" Joint  name: {0}  ; animation path:{1} ", joint.Name,"Skeleton/" + joint.Path));
            }

            for (var n = 0; n < Assimp.aiNode_GetNumChildren(node); n++) {
                ProcessNodes(scene, Assimp.aiNode_GetChild(node, n), ref listJoints);
            }
        }
    }
}