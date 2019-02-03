namespace Plugins
{
    public static partial class Assimp
    {
#if UNITY_64 && !(UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX)

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr aiImportFile(string filename, uint flags);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern void aiReleaseImport(IntPtr scene);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr aiImportFileWithProperties(string filename, uint flags, IntPtr pProps);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr aiCreatePropertyStore();
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern void aiReleasePropertyStore(IntPtr pProps);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern void aiSetImportPropertyInteger(IntPtr pProps, string name, int value);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern void aiSetImportPropertyFloat(IntPtr pProps, string name, float value);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern void aiSetImportPropertyString(IntPtr pProps, string name, string value);


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiScene_HasMaterials(IntPtr pScene);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiScene_GetNumMaterials(IntPtr pScene);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiScene_GetNumMeshes(IntPtr pScene);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiScene_GetNumAnimations(IntPtr pScene);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiScene_HasMeshes(IntPtr pScene);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiScene_HasAnimation(IntPtr pScene);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr aiScene_GetRootNode(IntPtr pScene);


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMaterial_GetTextureCount(IntPtr pScene, int Layer, int type);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMaterial_GetShininess(IntPtr pScene, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMaterial_GetTransparency(IntPtr pScene, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall, EntryPoint = "aiMaterial_GetTexture", ExactSpelling = false)]
        private static extern IntPtr _aiMaterial_GetTexture(IntPtr pScene, int Layer, int type);
        public static string aiMaterial_GetTexture(IntPtr pScene, int Layer, int type) {
            return getString(_aiMaterial_GetTexture(pScene, Layer, type));
        }

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Color aiMaterial_GetAmbient(IntPtr pScene, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Color aiMaterial_GetDiffuse(IntPtr pScene, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Color aiMaterial_GetSpecular(IntPtr pScene, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Color aiMaterial_GetEmissive(IntPtr pScene, int Layer);


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall, EntryPoint = "aiMaterial_GetTexture", ExactSpelling = false)]
        private static extern IntPtr _aiMaterial_GetName(IntPtr pScene, int Layer);
        public static string aiMaterial_GetName(IntPtr pScene, int Layer) {
            return getString(_aiMaterial_GetName(pScene, Layer));
        }


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetNumVertices(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetNumFaces(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetNumBones(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetMaterialIndex(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetNumUVChannels(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetNumColorChannels(IntPtr pScene, int mesh);


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall, EntryPoint = "aiMesh_GetName", ExactSpelling = false)]
        private static extern IntPtr _aiMesh_GetName(IntPtr pScene, int mesh);
        public static string aiMesh_GetName(IntPtr pScene, int mesh) {
            return getString(_aiMesh_GetName(pScene, mesh));
        }


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetNumIndicesPerFace(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_Indice(IntPtr pScene, int mesh, int Index, int Face);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiMesh_HasPositions(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiMesh_HasFaces(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiMesh_HasNormals(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiMesh_HasTangentsAndBitangents(IntPtr pScene, int mesh);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiMesh_HasVertexColors(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern bool aiMesh_HasTextureCoords(IntPtr pScene, int mesh, int Index);


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiMesh_Vertex(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiMesh_Normal(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiMesh_Tangent(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiMesh_Bitangent(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Color aiMesh_Color(IntPtr pScene, int mesh, int Index, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        public static extern Vector3 aiMesh_TextureCoord(IntPtr pScene, int mesh, int Index, int Layer);



        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_VertexX(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_VertexY(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_VertexZ(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_VertexNX(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_VertexNY(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_VertexNZ(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_TangentX(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_TangentY(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_TangentZ(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_BitangentX(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_BitangentY(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_BitangentZ(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_TextureCoordX(IntPtr pScene, int mesh, int Index, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_TextureCoordY(IntPtr pScene, int mesh, int Index, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_ColorRed(IntPtr pScene, int mesh, int Index, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_ColorGreen(IntPtr pScene, int mesh, int Index, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_ColorBlue(IntPtr pScene, int mesh, int Index, int Layer);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_ColorAlpha(IntPtr pScene, int mesh, int Index, int Layer);




        [DllImport("assimp", CallingConvention = CallingConvention.StdCall, EntryPoint = "aiMesh_GetBoneName", ExactSpelling = false)]
        private static extern IntPtr _aiMesh_GetBoneName(IntPtr pScene, int mesh, int Index);
        public static string aiMesh_GetBoneName(IntPtr pScene, int mesh, int Index) {
            return getString(_aiMesh_GetBoneName(pScene, mesh, Index));
        }
        //! Matrix that transforms from mesh space to bone space in bind pose(bone->mOffsetMatrix)
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Matrix4x4 aiMesh_GetBoneTransform(IntPtr pNode, int mesh, int Index);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Quaternion aiMesh_GetBoneRotation(IntPtr pNode, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_GetBoneRotationX(IntPtr pNode, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_GetBoneRotationY(IntPtr pNode, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_GetBoneRotationZ(IntPtr pNode, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_GetBoneRotationW(IntPtr pNode, int mesh, int Index);


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiMesh_GetBoneEulerRotation(IntPtr pNode, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_GetBoneEulerRotationX(IntPtr pNode, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_GetBoneEulerRotationY(IntPtr pNode, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_GetBoneEulerRotationZ(IntPtr pNode, int mesh, int Index);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiMesh_GetBonePosition(IntPtr pNode, int mesh, int Index);




        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetNumBoneWeights(IntPtr pScene, int mesh, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiMesh_GetBoneWeight(IntPtr pScene, int mesh, int Index, int Weight);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiMesh_GetBoneVertexId(IntPtr pScene, int mesh, int Index, int Weight);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern double aiAnim_GetDuration(IntPtr pScene, int anim);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern double aiAnim_GetTicksPerSecond(IntPtr pScene, int anim);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiAnim_GetNumChannels(IntPtr pScene, int anim);


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall, EntryPoint = "aiAnim_GetName", ExactSpelling = false)]
        private static extern IntPtr _aiAnim_GetName(IntPtr pScene, int anim);
        public static string aiAnim_GetName(IntPtr pScene, int anim) {
            return getString(_aiAnim_GetName(pScene, anim));
        }

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall, EntryPoint = "aiAnim_GetChannelName", ExactSpelling = false)]
        private static extern IntPtr _aiAnim_GetChannelName(IntPtr pScene, int anim, int Index);
        public static string aiAnim_GetChannelName(IntPtr pScene, int anim, int Index) {
            return getString(_aiAnim_GetChannelName(pScene, anim, Index));
        }


        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiAnim_GetPositionKey(IntPtr pScene, int anim, int Index, int key);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiAnim_GetScalingKey(IntPtr pScene, int anim, int Index, int key);

        //assimp quaternion is w,x,y,z by i fix this and the quaternion is x,y,z,w
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Quaternion aiAnim_GetRotationKey(IntPtr pScene, int anim, int Index, int key);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiAnim_GetEurlRotationKey(IntPtr pScene, int anim, int Index, int key);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiAnim_GetChannelIndex(IntPtr pScene, int anim, string name);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiAnim_GetNumPositionKeys(IntPtr pScene, int anim, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiAnim_GetNumRotationKeys(IntPtr pScene, int anim, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiAnim_GetNumScalingKeys(IntPtr pScene, int anim, int Index);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetPositionKeyX(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetPositionKeyY(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetPositionKeyZ(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern double aiAnim_GetPositionFrame(IntPtr pScene, int anim, int Index, int Key);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetScalingKeyX(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetScalingKeyY(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetScalingKeyZ(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern double aiAnim_GetScalingFrame(IntPtr pScene, int anim, int Index, int Key);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetRotationKeyX(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetRotationKeyY(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetRotationKeyZ(IntPtr pScene, int anim, int Index, int Key);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiAnim_GetRotationKeyW(IntPtr pScene, int anim, int Index, int Key);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern double aiAnim_GetRotationFrame(IntPtr pScene, int anim, int Index, int Key);



        [DllImport("assimp", CallingConvention = CallingConvention.StdCall, EntryPoint = "aiNode_GetName", ExactSpelling = false)]
        private static extern IntPtr _aiNode_GetName(IntPtr pNode);
        public static string aiNode_GetName(IntPtr pNode) {
            return getString(_aiNode_GetName(pNode));
        }

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern int aiNode_GetNumChildren(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr aiNode_GetChild(IntPtr pNode, int Index);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr aiNode_GetParent(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr aiNode_FindChild(IntPtr pNode, string name);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Matrix4x4 aiNode_GetTransformation(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]

        //assimp quaternion is w,x,y,z by i fix this and the quaternion is x,y,z,w
        public static extern Quaternion aiNode_GetRotation(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetRotationX(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetRotationY(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetRotationZ(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetRotationW(IntPtr pNode);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiNode_GetPosition(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetPositionX(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetPositionY(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetPositionZ(IntPtr pNode);

        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern Vector3 aiNode_GetEurlRotation(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetEurlRotationX(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetEurlRotationY(IntPtr pNode);
        [DllImport("assimp", CallingConvention = CallingConvention.StdCall)]
        public static extern float aiNode_GetEurlRotationZ(IntPtr pNode);

#endif
    }
}