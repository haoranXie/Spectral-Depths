using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class PLSkinnedMeshUpdater : MonoBehaviour
{
	[SerializeField]
	SkinnedMeshRenderer original;
	[SerializeField]
	SkinnedMeshRenderer target;
	[SerializeField]
    Transform rootBone;

	#region UNITYC_CALLBACK

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			//UpdateMeshRenderer (original);
            ReplaceBones(original,target,rootBone);
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmosSelected ()
	{
		var meshrenderer = GetComponentInChildren<SkinnedMeshRenderer> ();
		Vector3 before = meshrenderer.bones [0].position;
		for (int i = 0; i < meshrenderer.bones.Length; i++) {
			Gizmos.DrawLine (meshrenderer.bones [i].position, before);
			UnityEditor.Handles.Label (meshrenderer.bones [i].transform.position, i.ToString ());
			before = meshrenderer.bones [i].position;
		}
	}
	#endif

	#endregion

	public void UpdateMeshRenderer (SkinnedMeshRenderer newMeshRenderer)
	{
		// update mesh
		var meshrenderer = GetComponentInChildren<SkinnedMeshRenderer> ();
		//meshrenderer.sharedMesh = newMeshRenderer.sharedMesh;
		Transform[] childrens = transform.GetComponentsInChildren<Transform> (true);

		// sort bones.
		Transform[] bones = new Transform[newMeshRenderer.bones.Length];
		for (int boneOrder = 0; boneOrder < newMeshRenderer.bones.Length; boneOrder++) {
			bones [boneOrder] = Array.Find<Transform> (childrens, c => c.name == newMeshRenderer.bones [boneOrder].name);
		}
		meshrenderer.bones = bones;
	}

    public void ReplaceBones(SkinnedMeshRenderer origin, SkinnedMeshRenderer target, Transform skeletonRoot)
    {
        Dictionary<string, Transform> allBones = new Dictionary<string, Transform>(); // you can just cache this (and consequently the foreach below) or pass through parameter if in a static context. Leaving here for simplicity
        var childrenBones = skeletonRoot.GetComponentsInChildren<Transform>();
        foreach(Transform b in childrenBones)
        {
            allBones.Add(b.name, b);
        }
        var originBones = origin.bones;
        var targetBones = new List<Transform>();
        foreach(Transform b in originBones)
        {
            if(allBones.TryGetValue(b.name, out var foundBone))
            {
                targetBones.Add(foundBone);
            }
        }
        Debug.Log("ho");
        target.bones = targetBones.ToArray();
    }
}