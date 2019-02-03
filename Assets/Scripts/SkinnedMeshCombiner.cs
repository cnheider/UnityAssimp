using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshCombiner : MonoBehaviour {
    void Start() {
        var smRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        var bones = new List<Transform>();
        var boneWeights = new List<BoneWeight>();
        var combineInstances = new List<CombineInstance>();
        var textures = new List<Texture2D>();
        var numSubs = 0;

        foreach (var smr in smRenderers) {
            numSubs += smr.sharedMesh.subMeshCount;
        }

        var meshIndex = new int[numSubs];
        var boneOffset = 0;
        for (var s = 0; s < smRenderers.Length; s++) {
            var smr = smRenderers[s];

            var meshBoneweight = smr.sharedMesh.boneWeights;

            // May want to modify this if the renderer shares bones as unnecessary bones will get added.
            foreach (var bw in meshBoneweight) {
                var bWeight = bw;

                bWeight.boneIndex0 += boneOffset;
                bWeight.boneIndex1 += boneOffset;
                bWeight.boneIndex2 += boneOffset;
                bWeight.boneIndex3 += boneOffset;

                boneWeights.Add(bWeight);
            }
            boneOffset += smr.bones.Length;

            var meshBones = smr.bones;
            foreach (var bone in meshBones) {
                bones.Add(bone);
            }

            if (smr.material.mainTexture != null) {
                textures.Add(smr.GetComponent<Renderer>().material.mainTexture as Texture2D);
            }

            var ci = new CombineInstance();
            ci.mesh = smr.sharedMesh;
            meshIndex[s] = ci.mesh.vertexCount;
            ci.transform = smr.transform.localToWorldMatrix;
            combineInstances.Add(ci);

            UnityEngine.Object.Destroy(smr.gameObject);
        }

        var bindposes = new List<Matrix4x4>();

        for (var b = 0; b < bones.Count; b++) {
            bindposes.Add(bones[b].worldToLocalMatrix * transform.worldToLocalMatrix);
        }

        var r = gameObject.AddComponent<SkinnedMeshRenderer>();
        r.sharedMesh = new Mesh();
        r.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        var skinnedMeshAtlas = new Texture2D(128, 128);
        var packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        var originalUVs = r.sharedMesh.uv;
        var atlasUVs = new Vector2[originalUVs.Length];

        var rectIndex = 0;
        var vertTracker = 0;
        for (var i = 0; i < atlasUVs.Length; i++) {
            atlasUVs[i].x = Mathf.Lerp(packingResult[rectIndex].xMin, packingResult[rectIndex].xMax, originalUVs[i].x);
            atlasUVs[i].y = Mathf.Lerp(packingResult[rectIndex].yMin, packingResult[rectIndex].yMax, originalUVs[i].y);

            if (i >= meshIndex[rectIndex] + vertTracker) {
                vertTracker += meshIndex[rectIndex];
                rectIndex++;
            }
        }

        var combinedMat = new Material(Shader.Find("Diffuse"));
        combinedMat.mainTexture = skinnedMeshAtlas;
        r.sharedMesh.uv = atlasUVs;
        r.sharedMaterial = combinedMat;

        r.bones = bones.ToArray();
        r.sharedMesh.boneWeights = boneWeights.ToArray();
        r.sharedMesh.bindposes = bindposes.ToArray();
        r.sharedMesh.RecalculateBounds();
    }
}