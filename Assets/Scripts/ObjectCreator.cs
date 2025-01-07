using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    public string fbxPath = "Assets/Characters/Knight.fbx"; // Path to your FBX model
    public Vector3 position = new Vector3(0, 0, 0);       // Position of the object

    void Start()
    {
        // Create a new GameObject
        GameObject newObject = new GameObject("NewObject");

        // Set the position of the object
        newObject.transform.position = position;

        // Add Rigidbody component
        Rigidbody rb = newObject.AddComponent<Rigidbody>();
        
        // Add CapsuleCollider component
        CapsuleCollider capsuleCollider = newObject.AddComponent<CapsuleCollider>();

        // Load the FBX model
        GameObject fbxObject = LoadFBX(fbxPath);

        // Set the FBX model as the child of the new object
        fbxObject.transform.SetParent(newObject.transform);

        // Optionally, adjust scale/rotation if necessary
        fbxObject.transform.localScale = Vector3.one;
        fbxObject.transform.localRotation = Quaternion.identity;
    }

    GameObject LoadFBX(string path)
    {
        // Load the FBX model from the specified path
        GameObject fbxModel = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (fbxModel != null)
        {
            return Instantiate(fbxModel);
        }
        else
        {
            Debug.LogError("FBX file not found at path: " + path);
            return null;
        }
    }
}
