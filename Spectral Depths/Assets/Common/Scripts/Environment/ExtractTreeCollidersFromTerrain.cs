using System.Linq;
using UnityEngine;
using UnityEngine.AI;
 
[RequireComponent(typeof(Terrain))]
public class ExtractTreeCollidersFromTerrain : MonoBehaviour
{
    [ContextMenu("Extract")]
    public void Extract()
    {
        Debug.Log("ExtractTreeCollidersFromTerrain::Extract");
        Terrain terrain = GetComponent<Terrain>();
        Transform[] transforms = terrain.GetComponentsInChildren<Transform>();
        //Skip the first, since its the Terrain Collider
        for (int i = 1; i < transforms.Length; i++)
        {
            //Delete all previously created colliders first
            DestroyImmediate(transforms[i].gameObject);
        }
        Debug.Log("Tree prototypes count: "+ terrain.terrainData.treePrototypes.Length);
        for (int i = 0; i < terrain.terrainData.treePrototypes.Length; i++)
        {
            TreePrototype tree = terrain.terrainData.treePrototypes[i];
            //Get all instances matching the prefab index
            TreeInstance[] instances = terrain.terrainData.treeInstances.Where(x => x.prototypeIndex == i).ToArray();
            Debug.Log("Tree prototypes["+ i +"] instance count: "+ instances.Length);
            for (int j = 0; j < instances.Length; j++)
            {
                //Un-normalize positions so they're in world-space
                instances[j].position = Vector3.Scale(instances[j].position, terrain.terrainData.size);
                instances[j].position += terrain.GetPosition();
                NavMeshObstacle nav_mesh_obstacle = tree.prefab.GetComponent<NavMeshObstacle>();
                if(!nav_mesh_obstacle)
                {
                    Debug.LogWarning("Tree with prototype["+ i +"] instance["+ j +"] did not have a NavMeshObstacle component, skipping!");
                    continue;
                }
 
                Vector3 primitive_scale = nav_mesh_obstacle.size;
                if(nav_mesh_obstacle.shape == NavMeshObstacleShape.Capsule)
                {
                    primitive_scale = nav_mesh_obstacle.radius * Vector3.one;
                }
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                obj.name = tree.prefab.name + j;
                if (terrain.preserveTreePrototypeLayers) obj.layer = tree.prefab.layer;
                else obj.layer = terrain.gameObject.layer;
                obj.transform.localScale = primitive_scale;
                obj.transform.position = instances[j].position;
                obj.transform.parent = terrain.transform;
                obj.isStatic = true;
            }
        }
    }
}