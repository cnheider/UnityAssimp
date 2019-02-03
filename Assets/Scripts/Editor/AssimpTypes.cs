using UnityEngine;

namespace Editor {
    public class Bone {
        public int _BoneIndex;
        public float _Weight;
        public Bone() {
            this._BoneIndex = 0;
            this._Weight = 0;
        }
    }

    public class BoneVertex {
        public Bone[] _Bones;
        public int _Numbones;
        public BoneVertex() {
            this._Bones = new Bone[4];
            for (var i = 0; i < 4; i++) {
                this._Bones[i] = new Bone();
            }
            this._Numbones = 0;
        }

        public void AddBone(int bone, float w) {
            for (var i = 0; i < 4; i++) {
                if (this._Bones[i]._Weight == 0) {
                    this._Bones[i]._BoneIndex = bone;
                    this._Bones[i]._Weight = w;
                    this._Numbones++;
                    return;
                }
            }
        }
    }

    public class AssimpJoint {
        public string _ParentName;
        public string _Name;
        public string _Path;
        public Vector3 _Position;
        public Quaternion _Orientation;
        public AssimpJoint _Parent;
        public Transform _Transform;

        public AssimpJoint() {
            this._Name = "";
            this._ParentName = "";
            this._Parent = null;
            this._Path = "";
        }
    }
}