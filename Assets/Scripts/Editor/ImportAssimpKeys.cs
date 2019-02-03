using System;
using System.Collections.Generic;
using System.IO;
using Plugins;
using UnityEditor;
using UnityEngine;

namespace Editor {

    public static class ImportAssimpKeys {
        public static bool _SaveAssets = true;
        public static bool _UseTangents = true;

        private static StreamWriter _stream_writer;
        private static List<AssimpJoint> _list_joints = new List<AssimpJoint>();

        [MenuItem(Assimp._MenuPath +"ImportKeyFrames")]
        static void Init() {

            var filename = Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject));
            var rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));

            ReadMesh(rootPath, filename, "Textures/");
        }

        private static void Trace(string msg) {
            //  streamWriter.WriteLine("LOG:" + msg);
        }

        private static void ReadMesh(string path, string filename, string texturepath) {
            string importingAssetsDir;

            if (File.Exists(path + "/" + filename)) {
                var flags = (Assimp.PostProcessSteps.RemoveComponent);

                var config = Assimp.aiCreatePropertyStore();

                Assimp.aiSetImportPropertyFloat(config, Assimp.AI_CONFIG_PP_CT_MAX_SMOOTHING_ANGLE, 60.0f);
                Assimp.aiSetImportPropertyInteger(config, Assimp.AI_CONFIG_PP_LBW_MAX_WEIGHTS, 4);
                // IntPtr scene = Assimp.aiImportFile(path + "/" + filename, (uint)flags);
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
                            //  Transform bone = new GameObject(joint.Name).transform;
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

                    //create key frames
                    if (Assimp.aiScene_HasAnimation(scene)) {
                        var anim = (Animation)objectRoot.AddComponent(typeof(Animation));

                        var numAnimation = Assimp.aiScene_GetNumAnimations(scene);

                        for (var a = 0; a < numAnimation; a++) {

                            var clip = new AnimationClip();
                            var anima = Assimp.aiAnim_GetName(scene, a);
                            clip.name = nm + "_" + anima + "_" + a;
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
                                if (Assimp.aiAnim_GetNumScalingKeys(scene, a, i) != 0) {
                                    var scaleXcurve = new AnimationCurve();
                                    var scaleYcurve = new AnimationCurve();
                                    var scaleZcurve = new AnimationCurve();

                                    for (var j = 0; j < Assimp.aiAnim_GetNumScalingKeys(scene, a, i); j++) {
                                        var time = (float)Assimp.aiAnim_GetScalingFrame(scene, a, i, j);// *fps;
                                        var scale = Assimp.aiAnim_GetScalingKey(scene, a, i, j);
                                        //time = (float)j;
                                        scaleXcurve.AddKey(time, scale.x);
                                        scaleYcurve.AddKey(time, scale.y);
                                        scaleZcurve.AddKey(time, scale.z);
                                    }
                                    clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "m_LocalScale.x", scaleXcurve);
                                    clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "m_LocalScale.y", scaleYcurve);
                                    clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "m_LocalScale.z", scaleZcurve);

                                }


                                if (Assimp.aiAnim_GetNumPositionKeys(scene, a, i) != 0) {
                                    var posXcurve = new AnimationCurve();
                                    var posYcurve = new AnimationCurve();
                                    var posZcurve = new AnimationCurve();


                                    for (var j = 0; j < Assimp.aiAnim_GetNumPositionKeys(scene, a, i); j++) {
                                        var time = (float)Assimp.aiAnim_GetPositionFrame(scene, a, i, j);// *fps;
                                        var pos = Assimp.aiAnim_GetPositionKey(scene, a, i, j);
                                        //  time = (float)j;
                                        posXcurve.AddKey(time, pos.x);
                                        posYcurve.AddKey(time, pos.y);
                                        posZcurve.AddKey(time, pos.z);
                                    }

                                    clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localPosition.x", posXcurve);
                                    clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localPosition.y", posYcurve);
                                    clip.SetCurve("Skeleton/" + joint._Path, typeof(Transform), "localPosition.z", posZcurve);
                                }
                                if (Assimp.aiAnim_GetNumRotationKeys(scene, a, i) != 0) {

                                    var rotXcurve = new AnimationCurve();
                                    var rotYcurve = new AnimationCurve();
                                    var rotZcurve = new AnimationCurve();
                                    var rotWcurve = new AnimationCurve();

                                    for (var j = 0; j < Assimp.aiAnim_GetNumRotationKeys(scene, a, i); j++) {
                                        var time = (float)Assimp.aiAnim_GetRotationFrame(scene, a, i, j);// *fps;

                                        var rotation = Assimp.aiAnim_GetRotationKey(scene, a, i, j);
                                        //    time = (float)j;
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

                            clip.EnsureQuaternionContinuity();
                            anim.AddClip(clip, clip.name);
                            anim.clip = clip;

                            var clipAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + clip.name + ".asset");
                            AssetDatabase.CreateAsset(clip, clipAssetPath);

                            //  AssetDatabase.CreateAsset(clip, "Assets/Models/" + nm +"_"+a+ ".anim");
                            //  AssetDatabase.SaveAssets();
                        }
                    }

                    if (_SaveAssets) {
                        var prefabPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + filename + ".prefab");
                        var prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
                        PrefabUtility.ReplacePrefab(objectRoot, prefab, ReplacePrefabOptions.ConnectToPrefab);
                        AssetDatabase.Refresh();
                    }
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
            joint._ParentName = "";
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
                //  trace(string.Format(" Joint  name: {0}  ; parent:{1} ;  path:{2} ", joint.Name, joint.parent.Name, joint.Path));
            } else {
                //  trace(string.Format(" Joint  name: {0}  ;  path:{1} ", joint.Name, joint.Path));
            }

            for (var n = 0; n < Assimp.aiNode_GetNumChildren(node); n++) {
                ProcessNodes(scene, Assimp.aiNode_GetChild(node, n), ref listJoints);
            }
        }
    }
}